using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Threads;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCKRM
{
    [WikiDescription("비동기 작업을 관리하는 클래스 입니다")]
    public static class AsyncTaskManager
    {
        static readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        public static CancellationToken cancelToken => _cancel.Token;



        public static event Action asyncTaskAdd = () => { };
        public static event Action asyncTaskChange = () => { };
        public static event Action asyncTaskRemove = () => { };

        public static void AsyncTaskAddEventInvoke() => asyncTaskAdd();
        public static void AsyncTaskRemoveEventInvoke() => asyncTaskRemove();
        public static void AsyncTaskChangeEventInvoke() => asyncTaskChange();



        public static List<AsyncTask> asyncTasks { get; } = new List<AsyncTask>();

        public static void AllAsyncTaskCancel(bool onlyAsyncTaskClass = true)
        {
            for (int i = 0; i < asyncTasks.Count; i++)
            {
                AsyncTask asyncTask = asyncTasks[i];
                if (!asyncTask.cantCancel)
                {
                    asyncTask.Remove();
                    i--;
                }
            }

            if (!onlyAsyncTaskClass)
            {
                _cancel.Cancel();
                _cancel.Dispose();
            }
        }
    }

    [WikiDescription("비동기 작업 클래스")]
    public class AsyncTask : IRemoveableForce
    {
        public AsyncTask() : this("", "", false, true) { }

        public AsyncTask(NameSpacePathReplacePair name) : this(name, "", false, true) { }

        public AsyncTask(NameSpacePathReplacePair name, NameSpacePathReplacePair info, bool loop = false, bool cantCancel = true)
        {
            this.name = name;
            this.info = info;
            this.loop = loop;
            this.cantCancel = cantCancel;

            AsyncTaskManager.asyncTasks.Add(this);

            AsyncTaskManager.AsyncTaskAddEventInvoke();
            AsyncTaskManager.AsyncTaskChangeEventInvoke();

            if (Kernel.isPlaying && ResourceManager.isInitialLoadLanguageEnd)
                Debug.Log($"{ResourceManager.SearchLanguage(name.path, name.nameSpace)} async task created");
            else
                Debug.Log($"{name} async task created");
        }



        int nameLock = 0;
        NameSpacePathReplacePair _name = "";
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual NameSpacePathReplacePair name
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
        NameSpacePathReplacePair _info = "";
        /// <summary>
        /// Thread-Safe
        /// </summary>
        public virtual NameSpacePathReplacePair info
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
        public virtual bool loop
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
        public virtual bool cantCancel
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
        public virtual float progress
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
        public virtual float maxProgress
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
        public virtual bool isCanceled
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
        public virtual bool isRemoved { get => isCanceled; }



        int cancelEventLock = 0;
        event Action _cancelEvent;
        /// <summary>
        /// Thread-Safe, cancelEvent += () => { cancelEvent += () => { }; }; Do not add more methods to this event from inside this event method like this. This causes deadlock
        /// </summary>
        public event Action cancelEvent
        {
            add
            {
                while (Interlocked.CompareExchange(ref cancelEventLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _cancelEvent += value;

                Interlocked.Decrement(ref cancelEventLock);
            }
            remove
            {
                while (Interlocked.CompareExchange(ref cancelEventLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _cancelEvent -= value;

                Interlocked.Decrement(ref cancelEventLock);
            }
        }



        readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        public CancellationToken cancel => _cancel.Token;



        /// <summary>
        /// 이 함수는 메인 스레드에서만 실행할수 있습니다
        /// This function can only be executed on the main thread
        /// </summary>
        /// <exception cref="NotMainThreadMethodException"></exception>
        public virtual bool Remove() => Remove(false);

        public virtual bool Remove(bool force)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();

            if (!isCanceled && (!cantCancel || force))
            {
                isCanceled = true;

                while (Interlocked.CompareExchange(ref cancelEventLock, 1, 0) != 0)
                    Thread.Sleep(1);

                try
                {
                    _cancelEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                Interlocked.Decrement(ref cancelEventLock);

                AsyncTaskManager.asyncTasks.Remove(this);

                AsyncTaskManager.AsyncTaskChangeEventInvoke();
                AsyncTaskManager.AsyncTaskRemoveEventInvoke();

                _cancel.Cancel();

                if (Kernel.isPlaying && ResourceManager.isInitialLoadLanguageEnd)
                    Debug.Log($"{ResourceManager.SearchLanguage(name.path, name.nameSpace)} async task ended");
                else
                    Debug.Log($"{name} async task ended");

                return true;
            }

            return false;
        }

        void IRemoveable.Remove() => Remove(false);
    }
}
