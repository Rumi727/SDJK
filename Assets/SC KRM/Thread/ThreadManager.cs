using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace SCKRM.Threads
{
    [WikiDescription("스레드를 관리하기 위한 클래스 입니다")]
    public static class ThreadManager
    {
        public static List<ThreadMetaData> runningThreads { get; } = new List<ThreadMetaData>();

        /// <summary>
        /// { get; } = Thread.CurrentThread.ManagedThreadId
        /// </summary>
        public static int mainThreadId { get; } = Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// { get { mainThreadId == Thread.CurrentThread.ManagedThreadId; } }
        /// </summary>
        public static bool isMainThread => mainThreadId == Thread.CurrentThread.ManagedThreadId;


        public static event Action threadAdd = () => { };
        public static event Action threadChange = () => { };
        public static event Action threadRemove = () => { };

        public static void ThreadAddEventInvoke() => threadAdd();
        public static void ThreadRemoveEventInvoke() => threadRemove();
        public static void ThreadChangeEventInvoke() => threadChange();



        public static void AllThreadRemove()
        {
            if (!isMainThread)
                throw new NotMainThreadMethodException();

            for (int i = 0; i < runningThreads.Count; i++)
                runningThreads[i]?.Remove(true);
        }

        public static async UniTaskVoid ThreadAutoRemove()
        {
            if (!isMainThread)
                throw new NotMainThreadMethodException();

            while (true)
            {
                if (!Kernel.isPlaying)
                    return;

                for (int i = 0; i < runningThreads.Count; i++)
                {
                    ThreadMetaData runningThread = runningThreads[i];
                    if (runningThread != null && !runningThread.autoRemoveDisable && (runningThread.thread == null || !runningThread.thread.IsAlive))
                        runningThread.Remove(true);
                }

                if (await UniTask.Delay(100, cancellationToken: AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
            }
        }
    }

    public sealed class ThreadMetaData : AsyncTask
    {
        int nameLock = 0;
        string _name = "";
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override string name
        {
            get
            {
                while (Interlocked.CompareExchange(ref nameLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string value = _name;

                Interlocked.Decrement(ref nameLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref nameLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _name = value;
                Interlocked.Decrement(ref nameLock);
            }
        }

        int infoLock = 0;
        string _info = "";
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override string info
        {
            get
            {
                while (Interlocked.CompareExchange(ref infoLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string value = _info;

                Interlocked.Decrement(ref infoLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref infoLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _info = value;
                Interlocked.Decrement(ref infoLock);
            }
        }

        int loopLock = 0;
        bool _loop = false;
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override bool loop
        {
            get
            {
                while (Interlocked.CompareExchange(ref loopLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool value = _loop;

                Interlocked.Decrement(ref loopLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref loopLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _loop = value;
                Interlocked.Decrement(ref loopLock);
            }
        }

        int cantCancelLock = 0;
        bool _cantCancel = false;
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override bool cantCancel
        {
            get
            {
                while (Interlocked.CompareExchange(ref cantCancelLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool value = _cantCancel;

                Interlocked.Decrement(ref cantCancelLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref cantCancelLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _cantCancel = value;
                Interlocked.Decrement(ref cantCancelLock);
            }
        }



        int progressLock = 0;
        float _progress = 0;
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override float progress
        {
            get
            {
                while (Interlocked.CompareExchange(ref progressLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float value = _progress;

                Interlocked.Decrement(ref progressLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref progressLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _progress = value;
                Interlocked.Decrement(ref progressLock);
            }
        }

        int maxProgressLock = 0;
        float _maxProgress = 0;
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override float maxProgress
        {
            get
            {
                while (Interlocked.CompareExchange(ref maxProgressLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float value = _maxProgress;

                Interlocked.Decrement(ref maxProgressLock);
                return value;
            }
            set
            {
                while (Interlocked.CompareExchange(ref maxProgressLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _maxProgress = value;
                Interlocked.Decrement(ref maxProgressLock);
            }
        }



        int isCanceledLock = 0;
        bool _isCanceled = false;
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public override bool isCanceled
        {
            get
            {
                while (Interlocked.CompareExchange(ref isCanceledLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool value = _isCanceled;

                Interlocked.Decrement(ref isCanceledLock);
                return value;
            }
            protected set
            {
                while (Interlocked.CompareExchange(ref isCanceledLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _isCanceled = value;

                Interlocked.Decrement(ref isCanceledLock);
            }
        }



        public ThreadMetaData(ThreadStart method, string name = "", string info = "", bool loop = false, bool autoRemoveDisable = false, bool cantCancel = true) : base(name, info, loop, cantCancel)
        {
            this.autoRemoveDisable = autoRemoveDisable;

            thread = new Thread(method);
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public ThreadMetaData(Action<ThreadMetaData> method, string name = "", string info = "", bool loop = false, bool autoRemoveDisable = false, bool cantCancel = true) : base(name, info, loop, cantCancel)
        {
            this.autoRemoveDisable = autoRemoveDisable;

            thread = new Thread(() => method(this));
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public Thread thread { get; private set; } = null;
        public bool autoRemoveDisable { get; set; } = false;

        /// <summary>
        /// 이 함수는 메인 스레드에서만 실행할수 있습니다
        /// This function can only be executed on the main thread
        /// </summary>
        /// <exception cref="NotMainThreadMethodException"></exception>
        public override bool Remove(bool force = false)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();

            if (base.Remove(force))
            {
                ThreadManager.runningThreads.Remove(this);

                ThreadManager.ThreadChangeEventInvoke();
                ThreadManager.ThreadRemoveEventInvoke();

                if (thread != null)
                {
                    Thread _thread = thread;
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                    thread = null;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                    _thread.Join();
                }

                return true;
            }

            return false;
        }
    }

    public class NotMainThreadMethodException : Exception
    {
        /// <summary>
        /// {method} function must be executed on the main thread
        /// {method} 함수는 메인 스레드에서 실행되어야 합니다
        /// </summary>
        public NotMainThreadMethodException([CallerMemberName] string method = "") : base($"{method} function must be executed on the main thread\n{method} 함수는 메인 스레드에서 실행되어야 합니다") { }
    }

    public class MainThreadMethodException : Exception
    {
        /// <summary>
        /// {method} function cannot be executed on the main thread
        /// {method} 함수는 메인 스레드에서 실행 할 수 없습니다
        /// </summary>
        public MainThreadMethodException([CallerMemberName] string method = "") : base($"{method} function cannot be executed on the main thread\n{method} 함수는 메인스레드에서 실행 할 수 없습니다") { }
    }

    public class NotPlayModeThreadCreateException : Exception
    {
        /// <summary>
        /// It is forbidden to spawn threads when not in play mode. Please create your own thread
        /// 플레이 모드가 아닐때 스레드를 생성하는건 금지되어있습니다. 직접 스레드를 생성해주세요
        /// </summary>
        public NotPlayModeThreadCreateException() : base("It is forbidden to spawn threads when not in play mode. Please create your own thread\n플레이 모드가 아닐때 스레드를 생성하는건 금지되어있습니다. 직접 스레드를 생성해주세요") { }
    }
}