using SCKRM.Threads;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SCKRM.Renderer
{
    public static class RendererManager
    {
        public static void AllRefresh(bool thread = true) => Refresh(UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IRefreshable>().ToArray(), thread);

        public static void AllRerender(bool thread = true) => Refresh(UnityEngine.Object.FindObjectsOfType<CustomAllRenderer>(true), thread);

        public static void AllTextRerender(bool thread = true) => Refresh(UnityEngine.Object.FindObjectsOfType<CustomAllTextRenderer>(true), thread);

        static ThreadMetaData rerenderThread;
        public static void Refresh(IRefreshable[] refreshableObjects, bool thread = true)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(Rerender));
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(Rerender));

            if (thread)
            {
                if (rerenderThread != null)
                    rerenderThread.Remove();

                ThreadMetaData threadMetaData = ThreadManager.Create(Rerender, refreshableObjects, "notice.running_task.rerender.name");
                rerenderThread = threadMetaData;
            }
            else
            {
                for (int i = 0; i < refreshableObjects.Length; i++)
                    refreshableObjects[i].Refresh();
            }
        }

        static void Rerender(IRefreshable[] refreshableObjects, ThreadMetaData threadMetaData)
        {
            int stopLoop = 0;

            threadMetaData.maxProgress = refreshableObjects.Length - 1;

            threadMetaData.cancelEvent += CancelEvent;
            threadMetaData.cantCancel = false;

            for (int i = 0; i < refreshableObjects.Length; i++)
            {
                Interlocked.Decrement(ref stopLoop);
                if (Interlocked.Increment(ref stopLoop) > 0)
                    return;

                refreshableObjects[i].Refresh();

                threadMetaData.progress = i;
            }

            void CancelEvent()
            {
                Interlocked.Increment(ref stopLoop);

                threadMetaData.maxProgress = 1;
                threadMetaData.progress = 1;
            }
        }
    }
}