using Cysharp.Threading.Tasks;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Json;
using SCKRM.NTP;
using SCKRM.SaveLoad;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using Version = SCKRM.Version;

namespace SDJK.Replay
{
    public static class ReplayLoader
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool loadInParallel { get; set; } = false;
        }

        public static T CreateReplay<T>(MapFile map, params IMode[] modes) where T : ReplayFile, new()
        {
            if (modes == null)
                modes = IMode.emptyModes;

            T replay = new T();
            replay.InternalReplayFileSetting(map, modes);

            return replay;
        }

        public static async UniTask<List<ReplayFile>> ReplaysLoad(MapFile map)
        {
            AsyncTask asyncTask = new AsyncTask("sdjk:notice.running_task.replay_list_refresh.name", "", false, false);

            try
            {
                List<ReplayFile> list = await Load();
                return list;

                async UniTask<List<ReplayFile>> Load()
                {
                    string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay", map.info.id);
                    if (!Directory.Exists(replayFolderPath))
                        return null;

                    string[] replayPaths = Directory.GetFiles(replayFolderPath, "*.sdjk-replay");
                    if (replayPaths == null || replayPaths.Length <= 0)
                        return null;

                    asyncTask.maxProgress = replayPaths.Length;

                    //병렬 처리
                    {
                        SynchronizedCollection<ReplayFile> syncResultReplays = new SynchronizedCollection<ReplayFile>();
                        int loadedReplayCount = 0;

                        for (int i = 0; i < replayPaths.Length; i++)
                        {
                            string path = replayPaths[i];

                            //기다리게 되면 병렬 처리가 되지 않음
                            if (SaveData.loadInParallel)
                                UniTask.RunOnThreadPool(() => ReplayLoad(path)).Forget();
                            else
                                ReplayLoad(path);

                            await UniTask.NextFrame();

                            if (asyncTask.isRemoved || !Kernel.isPlaying)
                                return null;

                            void ReplayLoad(string path)
                            {
                                try
                                {
                                    ReplayFile replay = ReplayLoad<ReplayFile>(path);
                                    if (replay != null)
                                        syncResultReplays.Add(replay);

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

                        if (SaveData.loadInParallel)
                        {
                            if (await UniTask.WaitUntil(() => Interlocked.Add(ref loadedReplayCount, 0) >= replayPaths.Length, PlayerLoopTiming.Update, asyncTask.cancel).SuppressCancellationThrow()
                                || asyncTask.isRemoved
                                || !Kernel.isPlaying)
                                return null;
                        }

                        return ReplayListSort(syncResultReplays).ToList();
                    }
                }
            }
            finally
            {
                asyncTask.Remove(true);
            }
        }

        public static T ReplayLoad<T>(string replayFilePath) where T : ReplayFile, new()
        {
            if (File.Exists(replayFilePath))
            {
                T replay = JsonManager.JsonRead<T>(replayFilePath, true);
                replay.replayFilePath = replayFilePath;

                return replay;
            }

            return null;
        }

        public static T ReplayLoad<T>(string replayFilePath, out IMode[] modes) where T : ReplayFile, new()
        {
            if (File.Exists(replayFilePath))
            {
                T replay = JsonManager.JsonRead<T>(replayFilePath, true);
                replay.replayFilePath = replayFilePath;

                modes = new IMode[replay.modes.Length];
                for (int i = 0; i < replay.modes.Length; i++)
                {
                    ReplayModeFile replayMode = replay.modes[i];
                    IMode mode = (IMode)Activator.CreateInstance(replayMode.modeType);

                    if (replayMode.modeConfigType != null)
                        mode.modeConfig = (IModeConfig)(new JObject(replayMode.modeConfig).ToObject(replayMode.modeConfigType));

                    modes[i] = mode;
                }

                return replay;
            }

            modes = null;
            return null;
        }

        public static IOrderedEnumerable<ReplayFile> ReplayListSort(IList<ReplayFile> replays) =>
            replays.OrderBy(x => x.scores.Last().value.Distance(JudgementUtility.maxScore))
                .ThenBy(x => x.clearUTCTime, OrderByDirection.Descending);

        public static event Action<ReplayFile> replaySaveEvent;
        public static void ReplaySave<T>(this T replay, MapFile map, params IMode[] modes) where T : ReplayFile, new()
        {
            if (modes == null)
                modes = IMode.emptyModes;

            replay.InternalReplayFileSetting(map, modes);

            string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay", map.info.id);
            replay.replayFilePath = PathUtility.Combine(replayFolderPath, $"{replay.clearUTCTime.Ticks}.sdjk-replay");

            if (!Directory.Exists(replayFolderPath))
                Directory.CreateDirectory(replayFolderPath);

            File.WriteAllText(replay.replayFilePath, JsonManager.ObjectToJson(replay));
            replaySaveEvent?.Invoke(replay);
        }

        public static event Action<ReplayFile> replayDeleteEvent;
        public static void ReplayDelete<T>(this T replay) where T : ReplayFile, new()
        {
            if (File.Exists(replay.replayFilePath))
                File.Delete(replay.replayFilePath);

            replayDeleteEvent?.Invoke(replay);
        }

        static void InternalReplayFileSetting<T>(this T replay, MapFile map, IMode[] modes) where T : ReplayFile, new()
        {
            replay.mapId = map.info.id;

            replay.sckrmVersion = Kernel.sckrmVersion;
            replay.sdjkVersion = new Version(Application.version);

            replay.clearUTCTime = NTPDateTime.utcNow;
            replay.ruleset = map.info.ruleset;

            replay.mapMaxCombo = map.allJudgmentBeat.Count;

            replay.mapDifficulty = map.difficulty;
            replay.mapDifficultyAverage = map.difficultyAverage;

            {
                replay.modes = new ReplayModeFile[modes.Length];
                ReplayModeFile[] replayModeFiles = replay.modes;
                for (int i = 0; i < modes.Length; i++)
                {
                    IMode mode = modes[i];
                    replayModeFiles[i] = new ReplayModeFile(mode.GetType(), mode.modeConfig);
                }
            }
        }
    }
}
