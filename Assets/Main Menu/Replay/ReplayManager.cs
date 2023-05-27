using Cysharp.Threading.Tasks;
using K4.Threading;
using MoreLinq;
using Newtonsoft.Json;
using SCKRM;
using SCKRM.DragAndDrop;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.Threads;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SDJK.MainMenu
{
    public static class ReplayManager
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool replayAsyncLoad { get; set; } = false;
        }

        public static Dictionary<string, List<ReplayFile>> currentReplayFiles { get; private set; } = new Dictionary<string, List<ReplayFile>>();

        [Awaken]
        public static void Awaken()
        {
            if (!SaveData.replayAsyncLoad)
                ResourceManager.resourceRefreshEvent += Refresh;

            DragAndDropManager.dragAndDropEvent += DragAndDropEvent;
            ReplayLoader.replaySaveEvent += x =>
            {
                if (!currentReplayFiles.TryGetValue(x.mapId, out List<ReplayFile> value))
                    value = new List<ReplayFile>();

                value.Add(x);
                value = ReplayListSort(value).ToList();

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

        [Starten]
        public static void Starten()
        {
            if (SaveData.replayAsyncLoad)
            {
                ResourceManager.resourceRefreshEvent += Refresh;
                Refresh().Forget();
            }
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
                Dictionary<string, List<ReplayFile>> list = await Load();
                if (list != null)
                    currentReplayFiles = list;

                async UniTask<Dictionary<string, List<ReplayFile>>> Load()
                {
                    string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay");
                    if (!Directory.Exists(replayFolderPath))
                        Directory.CreateDirectory(replayFolderPath);

                    string[] replayPaths = Directory.GetFiles(replayFolderPath, "*.sdjk-replay");
                    if (replayPaths == null || replayPaths.Length <= 0)
                        return null;

                    asyncTask.maxProgress = replayPaths.Length;

                    //병렬 처리
                    {
                        ConcurrentDictionary<string, SynchronizedCollection<ReplayFile>> syncResultReplays = new ConcurrentDictionary<string, SynchronizedCollection<ReplayFile>>();
                        int loadedReplayCount = 0;

                        for (int i = 0; i < replayPaths.Length; i++)
                        {
                            string path = replayPaths[i];
                            UniTask.RunOnThreadPool(() => ReplayLoad(path)).Forget();

                            void ReplayLoad(string path)
                            {
                                try
                                {
                                    ReplayFile replay = ReplayLoader.ReplayLoad<ReplayFile>(path);
                                    if (replay != null)
                                    {
                                        if (syncResultReplays.ContainsKey(replay.mapId))
                                            syncResultReplays[replay.mapId].Add(replay);
                                        else
                                            syncResultReplays[replay.mapId] = new SynchronizedCollection<ReplayFile>() { replay };
                                    }

                                    if (asyncTask.isRemoved)
                                        return;

                                    asyncTask.progress++;
                                }
                                finally
                                {
                                    Interlocked.Increment(ref loadedReplayCount);
                                }
                            }
                        }

                        if (await UniTask.WaitUntil(() => Interlocked.Add(ref loadedReplayCount, 0) >= replayPaths.Length, PlayerLoopTiming.Update, asyncTask.cancel).SuppressCancellationThrow()
                            || asyncTask.isRemoved
                            || !Kernel.isPlaying)
                            return null;

                        //스레드 리스트를 일반 리스트로
                        Dictionary<string, List<ReplayFile>> resultReplays = new Dictionary<string, List<ReplayFile>>();
                        List<KeyValuePair<string, SynchronizedCollection<ReplayFile>>> replays = syncResultReplays.ToList();

                        asyncTask.progress = 0;
                        asyncTask.maxProgress = replays.Count;

                        for (int i = 0; i < replays.Count; i++)
                        {
                            KeyValuePair<string, SynchronizedCollection<ReplayFile>> replay = replays[i];
                            resultReplays[replay.Key] = ReplayListSort(replay.Value).ToList();

                            asyncTask.progress++;

                            if (await UniTask.NextFrame(asyncTask.cancel).SuppressCancellationThrow() || asyncTask.isRemoved || !Kernel.isPlaying)
                                return null;
                        }

                        return resultReplays;
                    }
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

        public static IOrderedEnumerable<ReplayFile> ReplayListSort(IList<ReplayFile> replays) =>
            replays.OrderBy(x => x.scores.Last().value.Distance(JudgementUtility.maxScore))
                .ThenBy(x => x.clearUTCTime, OrderByDirection.Descending);
    }
}
