using Cysharp.Threading.Tasks;
using K4.Threading;
using MoreLinq;
using SCKRM;
using SCKRM.DragAndDrop;
using SCKRM.Resource;
using SCKRM.Threads;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset;
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
            DragAndDropManager.dragAndDropEvent += DragAndDropEvent;

            ResourceManager.resourceRefreshEvent += Refresh;
            ReplayLoader.replaySaveEvent += x =>
            {
                if (!currentReplayFiles.TryGetValue(x.mapId, out List<ReplayFile> value))
                    value = new List<ReplayFile>();

                value.Add(x);
                value = ReplayListSort(value);

                currentReplayFiles[x.mapId] = value;
                replayLoadingEnd?.Invoke();
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

        static bool DragAndDropEvent(string path, bool isFolder, ThreadMetaData threadMetaData)
        {
            if (isFolder)
                return false;

            if (Path.GetExtension(path) == ".sdjk-replay")
            {
                threadMetaData.name = "sdjk:notice.running_task.drag_and_drop.replay_list_load.name";
                threadMetaData.info = "";
                threadMetaData.progress = 0;

                ReplayFile replay = ReplayLoader.ReplayLoad<ReplayFile>(path);
                if (replay == null)
                    return true;

                K4UnityThreadDispatcher.Execute(() =>
                {
                    for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
                    {
                        MapPack mapPack = MapManager.currentMapPacks[i];
                        for (int j = 0; j < mapPack.maps.Count; j++)
                        {
                            MapFile map = mapPack.maps[j];
                            if (map.info.id == replay.mapId)
                            {
                                ResultScreen.Show(RulesetManager.FindRuleset(replay.ruleset), map, replay, null);
                                return;
                            }
                        }
                    }
                });

                threadMetaData.progress = 1;
                return true;
            }

            return false;
        }



        public static event Action replayLoadingEnd;



        static bool isReplayListRefreshing = false;
        public static async UniTask Refresh()
        {
            if (isReplayListRefreshing)
                return;

            isReplayListRefreshing = true;

            Debug.ForceLog("Refreshing replay list...", nameof(ReplayManager));

            AsyncTask asyncTask = new AsyncTask("sdjk:notice.running_task.replay_list_refresh.name", "", false, false);
            if (ResourceManager.isResourceRefesh)
                ResourceManager.resourceRefreshDetailedAsyncTask = asyncTask;

            try
            {
                Dictionary<string, List<ReplayFile>> list = await UniTask.RunOnThreadPool(Load);
                if (list != null)
                    currentReplayFiles = list;

                Dictionary<string, List<ReplayFile>> Load()
                {
                    Dictionary<string, List<ReplayFile>> resultReplays = new Dictionary<string, List<ReplayFile>>();
                    string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay");
                    if (!Directory.Exists(replayFolderPath))
                        Directory.CreateDirectory(replayFolderPath);

                    string[] replayPaths = Directory.GetFiles(replayFolderPath, "*.sdjk-replay");
                    if (replayPaths == null || replayPaths.Length <= 0)
                        return null;

                    asyncTask.maxProgress = replayPaths.Length;

                    for (int i = 0; i < replayPaths.Length; i++)
                    {
                        ReplayFile replay = ReplayLoader.ReplayLoad<ReplayFile>(replayPaths[i]);
                        if (replay != null)
                        {
                            if (resultReplays.ContainsKey(replay.mapId))
                                resultReplays[replay.mapId].Add(replay);
                            else
                                resultReplays[replay.mapId] = new List<ReplayFile>() { replay };
                        }

                        if (asyncTask.isRemoved)
                            return null;

                        asyncTask.progress++;
                    }

                    asyncTask.name = "sdjk:notice.running_task.replay_list_refresh.name";

                    {
                        List<KeyValuePair<string, List<ReplayFile>>> replays = resultReplays.ToList();

                        asyncTask.progress = 0;
                        asyncTask.maxProgress = replays.Count;

                        for (int i = 0; i < replays.Count; i++)
                        {
                            KeyValuePair<string, List<ReplayFile>> replay = replays[i];
                            resultReplays[replay.Key] = ReplayListSort(replay.Value);

                            asyncTask.progress++;
                        }
                    }

                    asyncTask.progress++;
                    return resultReplays;
                }

                if (InitialLoadManager.isInitialLoadEnd)
                    replayLoadingEnd?.Invoke();
            }
            finally
            {
                if (!ResourceManager.isResourceRefesh)
                    asyncTask.Remove(true);

                isReplayListRefreshing = false;
            }
        }

        public static List<ReplayFile> ReplayListSort(List<ReplayFile> replays) =>
            replays.OrderBy(x => x.scores.Last().value.Distance(JudgementUtility.maxScore))
                .ThenBy(x => x.clearUTCTime, OrderByDirection.Descending).ToList();
    }
}
