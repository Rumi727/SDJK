using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Resource;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

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
        #region 생성자
        public ThreadMetaData(ThreadStart method) : base()
        {
            autoRemoveDisable = false;

            thread = new Thread(method);
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public ThreadMetaData(ThreadStart method, NameSpacePathReplacePair name) : base(name)
        {
            autoRemoveDisable = false;

            thread = new Thread(method);
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public ThreadMetaData(ThreadStart method, NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool loop = false, bool autoRemoveDisable = false, bool cantCancel = true) : base(name, info, loop, cantCancel)
        {
            this.autoRemoveDisable = autoRemoveDisable;

            thread = new Thread(method);
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }



        public ThreadMetaData(Action<ThreadMetaData> method) : base()
        {
            autoRemoveDisable = false;

            thread = new Thread(() => method(this));
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public ThreadMetaData(Action<ThreadMetaData> method, NameSpacePathReplacePair name) : base(name)
        {
            autoRemoveDisable = false;

            thread = new Thread(() => method(this));
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }

        public ThreadMetaData(Action<ThreadMetaData> method, NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool loop = false, bool autoRemoveDisable = false, bool cantCancel = true) : base(name, info, loop, cantCancel)
        {
            this.autoRemoveDisable = autoRemoveDisable;

            thread = new Thread(() => method(this));
            thread.Start();

            ThreadManager.runningThreads.Add(this);

            ThreadManager.ThreadAddEventInvoke();
            ThreadManager.ThreadChangeEventInvoke();
        }
        #endregion



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
                if (Kernel.isPlaying && ResourceManager.isInitialLoadLanguageEnd)
                    Debug.ForceLog($"{ResourceManager.SearchLanguage(name.path, name.nameSpace)} Thread Remove! Beware the Join method");
                else
                    Debug.ForceLog($"{name} Thread Remove! Beware the Join method");

                ThreadManager.runningThreads.Remove(this);

                ThreadManager.ThreadChangeEventInvoke();
                ThreadManager.ThreadRemoveEventInvoke();

                if (thread != null)
                {
                    thread.Join();
                    thread = null;
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