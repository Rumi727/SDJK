//참고: https://m.blog.naver.com/an060875/221686029148

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using SCKRM.Threads;

namespace SCKRM.NTP
{
    public static class NTPDateTime
    {
        /// <summary>
        /// 가져올 NTP 서버의 URL
        /// </summary>
        public const string ntpServerUrl = "time.google.com";

        /// <summary>
        /// NTP 서버와 동기화된 시간입니다
        /// Thread-safe
        /// </summary>
        public static DateTime now
        {
            get
            {
                while (Interlocked.CompareExchange(ref ntpThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                try
                {
                    return _lastSyncedServerDateTime + timer.Elapsed;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Interlocked.Decrement(ref ntpThreadLock);
                }
            }
        }

        /// <summary>
        /// NTP 서버와 동기화된 시간입니다
        /// Thread-safe
        /// </summary>
        public static DateTime utcNow
        {
            get
            {
                while (Interlocked.CompareExchange(ref ntpThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                try
                {
                    return _lastSyncedServerUTCDateTime + timer.Elapsed;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Interlocked.Decrement(ref ntpThreadLock);
                }
            }
        }

        /// <summary>
        /// 마지막으로 NTP 서버와 동기화 될 때의 시간입니다
        /// Thread-safe
        /// </summary>
        public static DateTime lastSyncedServerDateTime
        {
            get
            {
                while (Interlocked.CompareExchange(ref ntpThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                DateTime dateTime = _lastSyncedServerDateTime;

                Interlocked.Decrement(ref ntpThreadLock);

                return dateTime;
            }
        }
        static DateTime _lastSyncedServerDateTime = DateTime.Now;

        /// <summary>
        /// 마지막으로 NTP 서버와 동기화 될 때의 UTC 시간입니다
        /// Thread-safe
        /// </summary>
        public static DateTime lastSyncedServerUTCDateTime
        {
            get
            {
                while (Interlocked.CompareExchange(ref ntpThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                DateTime dateTime = _lastSyncedServerUTCDateTime;

                Interlocked.Decrement(ref ntpThreadLock);

                return dateTime;
            }
        }
        static DateTime _lastSyncedServerUTCDateTime = DateTime.UtcNow;

        /// <summary>
        /// UTC 시간이 동기화 될 때 메인 스레드가 아닌 스레드에서 이벤트가 호출됩니다
        /// Thread-safe
        /// </summary>
        public static event Action<DateTime> onTimeUpdated
        {
            add
            {
                while (Interlocked.CompareExchange(ref ntpEventThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _onTimeUpdated += value;

                Interlocked.Decrement(ref ntpEventThreadLock);
            }
            remove
            {
                while (Interlocked.CompareExchange(ref ntpEventThreadLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _onTimeUpdated -= value;

                Interlocked.Decrement(ref ntpEventThreadLock);
            }
        }
        static Action<DateTime> _onTimeUpdated;

        static int ntpThreadLock = 0;
        static int ntpEventThreadLock = 0;
        static Stopwatch timer = Stopwatch.StartNew();

        [Awaken]
        static void Awaken()
        {
            //응답까지 프리징이 걸리므로 쓰레드 사용하여 처리
            new ThreadMetaData(SyncTime, "sc-krm:ntp.thread.name", "sc-krm:ntp.thread.info.start", true, true, true);
        }

        static void SyncTime(ThreadMetaData metaData)
        {
            int stopLoop = 0;
            metaData.cancelEvent += Cancel;

            while (true)
            {
                if (Kernel.internetReachability == NetworkReachability.NotReachable)
                {
                    metaData.info = "sc-krm:ntp.thread.info.no_internet";
                    continue;
                }

                try
                {
                    metaData.info = "sc-krm:ntp.thread.info.start";

                    byte[] ntpData = new byte[48];
                    ntpData[0] = 0x1B;

                    IPAddress[] addresses = Dns.GetHostEntry(ntpServerUrl).AddressList;
                    IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);

                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        socket.Connect(ipEndPoint);

                        socket.ReceiveTimeout = 3000;

                        socket.Send(ntpData);
                        socket.Receive(ntpData);
                        socket.Close();
                    }

                    //컴퓨터가 계산을 하고 스레드를 대기탈때 오차가 발생하기 때문에 그 오차를 보정해줄 타이머를 하나 더 생성합니다
                    Stopwatch timer = Stopwatch.StartNew();

                    const byte serverReplyTime = 40;
                    ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
                    ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                    intPart = SwapEndianness(intPart);
                    fractPart = SwapEndianness(fractPart);

                    double milliseconds = (intPart * 1000d) + ((fractPart * 1000d) / 0x100000000d);
                    DateTime networkUTCDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
                    DateTime networkDateTime = networkUTCDateTime.ToLocalTime();

                    //시간 적용
                    {
                        while (Interlocked.CompareExchange(ref ntpThreadLock, 1, 0) != 0)
                            Thread.Sleep(1);

                        try
                        {
                            //아까 생성했던 타이머의 시간을 가져와서 NTP 시간에 더합니다
                            TimeSpan elapsed = timer.Elapsed;
                            _lastSyncedServerDateTime = networkDateTime + elapsed;
                            _lastSyncedServerUTCDateTime = networkUTCDateTime + elapsed;

                            NTPDateTime.timer.Restart();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref ntpThreadLock);
                        }
                    }

                    Debug.Log("Time synced : " + networkDateTime);
                    Debug.Log("UTC Time synced : " + networkUTCDateTime);

                    metaData.info = "sc-krm:ntp.thread.info.end";

                    //이벤트 호출
                    {
                        while (Interlocked.CompareExchange(ref ntpEventThreadLock, 1, 0) != 0)
                            Thread.Sleep(1);

                        try
                        {
                            _onTimeUpdated?.Invoke(networkUTCDateTime);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref ntpEventThreadLock);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.ForceLogError("서버 시간을 가져오는 중 에러가 발생했습니다\nError getting server time");
                    Debug.LogException(e);

                    metaData.info = "sc-krm:ntp.thread.info.error";
                }


                /*
                 * 시간 서버에서 4초 안에 2번 이상의 호출이 될 경우 차단 될 수 있으니
                 * 60초간의 딜레이를 가진다.
                */
                for (int i = 0; i < 60000; i++)
                {
                    Thread.Sleep(1);

                    if (Interlocked.Add(ref stopLoop, 0) > 0)
                        return;
                }
            }

            void Cancel() => Interlocked.Increment(ref stopLoop);
        }

        static uint SwapEndianness(ulong x) => (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
    }
}