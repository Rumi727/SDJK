using SCKRM.Object;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.UI.SideBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Side Bar/Running Task List")]
    public sealed class RunningTaskList : UI
    {
        [System.NonSerialized] List<AsyncTask> tempList = new List<AsyncTask>();

        protected override void Awake()
        {
            AsyncTaskManager.asyncTaskChange -= Refresh;
            AsyncTaskManager.asyncTaskChange += Refresh;
        }

        void Refresh()
        {
            if (!InitialLoadManager.isInitialLoadEnd)
                return;

            RunningTaskInfo[] runningTaskInfos = GetComponentsInChildren<RunningTaskInfo>();
            for (int i = 0; i < runningTaskInfos.Length; i++)
                runningTaskInfos[i].asyncTask = null;

            for (int i = 0; i < AsyncTaskManager.asyncTasks.Count; i++)
            {
                if (i >= runningTaskInfos.Length)
                {
                    RunningTaskInfo runningTaskInfo = (RunningTaskInfo)ObjectPoolingSystem.ObjectCreate("running_task_list.running_task", transform).monoBehaviour;
                    runningTaskInfo.transform.SetSiblingIndex(0);
                    runningTaskInfo.asyncTask = AsyncTaskManager.asyncTasks[i];
                    runningTaskInfo.asyncTaskIndex = i;
                    runningTaskInfo.InfoLoad();
                }
                else
                {
                    RunningTaskInfo runningTaskInfo = runningTaskInfos[i];
                    runningTaskInfo.transform.SetSiblingIndex(0);
                    runningTaskInfo.asyncTask = AsyncTaskManager.asyncTasks[i];
                    runningTaskInfo.asyncTaskIndex = i;
                    runningTaskInfo.InfoLoad();
                }
            }
        }
    }
}