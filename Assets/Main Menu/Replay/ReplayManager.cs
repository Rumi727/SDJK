using Cysharp.Threading.Tasks;
using MoreLinq;
using SCKRM;
using SCKRM.Resource;
using SDJK.Replay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SDJK.MainMenu
{
    public static class ReplayManager
    {
        public static Dictionary<string, List<ReplayFile>> currentReplayFiles { get; private set; } = new Dictionary<string, List<ReplayFile>>();

        [Awaken]
        public static void Awaken()
        {
            ResourceManager.resourceRefreshEvent += Refresh;
            ReplayLoader.replaySaveEvent += x =>
            {
                if (currentReplayFiles.TryGetValue(x.mapId, out List<ReplayFile> value))
                {
                    value.Add(x);
                    value = ReplayListSort(value);

                    currentReplayFiles[x.mapId] = value;
                    replayLoadingEnd?.Invoke();
                }
            };

            ReplayLoader.replayDeleteEvent += x =>
            {
                if (currentReplayFiles.TryGetValue(x.mapId, out List<ReplayFile> value))
                {
                    value.Remove(x);
                    replayLoadingEnd?.Invoke();
                }
            };
        }



        public static event Action replayLoadingEnd;



        static bool isReplayListRefreshing = false;
        public static async UniTask Refresh()
        {
            if (isReplayListRefreshing)
                return;

            isReplayListRefreshing = true;

            try
            {
                Debug.ForceLog("Refreshing replay list...", nameof(ReplayManager));

                AsyncTask asyncTask = new AsyncTask("sdjk:notice.running_task.replay_list_refresh.name", "");
                if (ResourceManager.isResourceRefesh)
                    ResourceManager.resourceRefreshDetailedAsyncTask = asyncTask;

                Dictionary<string, List<ReplayFile>> resultReplays = new Dictionary<string, List<ReplayFile>>();
                string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay");
                if (!Directory.Exists(replayFolderPath))
                    Directory.CreateDirectory(replayFolderPath);

                string[] replayPaths = Directory.GetFiles(replayFolderPath, "*.sdjk-replay");
                if (replayPaths == null || replayPaths.Length <= 0)
                    return;

                asyncTask.maxProgress = replayPaths.Length + 1;

                for (int i = 0; i < replayPaths.Length; i++)
                {
                    ReplayFile replay = ReplayLoader.ReplayLoad<ReplayFile>(replayPaths[i], out _);
                    if (replay != null)
                    {
                        if (resultReplays.ContainsKey(replay.mapId))
                            resultReplays[replay.mapId].Add(replay);
                        else
                            resultReplays[replay.mapId] = new List<ReplayFile>() { replay };
                    }

                    await UniTask.NextFrame();

                    if (asyncTask.isCanceled)
                        return;

                    asyncTask.progress++;
                }

                asyncTask.name = "sdjk:notice.running_task.replay_list_refresh.name";

                {
                    List<KeyValuePair<string, List<ReplayFile>>> replays = resultReplays.ToList();
                    for (int i = 0; i < replays.Count; i++)
                    {
                        KeyValuePair<string, List<ReplayFile>> replay = replays[i];
                        resultReplays[replay.Key] = ReplayListSort(replay.Value);

                        await UniTask.NextFrame();

                        if (asyncTask.isCanceled)
                            return;
                    }
                }

                asyncTask.progress++;
                currentReplayFiles = resultReplays;

                if (InitialLoadManager.isInitialLoadEnd)
                    replayLoadingEnd?.Invoke();
            }
            finally
            {
                isReplayListRefreshing = false;
            }
        }

        public static List<ReplayFile> ReplayListSort(List<ReplayFile> replays)
        {
            return replays.OrderBy(x =>
            {
                if (x.scores.Count > 0)
                    return x.scores.Last().value;
                else
                    return x.scores.defaultValue;
            }, OrderByDirection.Descending).ThenBy(x => x.clearUTCTime).ToList();
        }
    }
}
