using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Json;
using SCKRM.NTP;
using SDJK.Map;
using SDJK.Mode;
using System;
using System.IO;
using UnityEngine;
using Version = SCKRM.Version;

namespace SDJK.Replay
{
    public static class ReplayLoader
    {
        public static T CreateReplay<T>(MapFile map, IMode[] modes) where T : ReplayFile, new()
        {
            T replay = new T();
            replay.InternalReplayFileSetting(map, modes);

            return replay;
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

        public static event Action<ReplayFile> replaySaveEvent;
        public static void ReplaySave<T>(this T replay, MapFile map, IMode[] modes) where T : ReplayFile, new()
        {
            replay.InternalReplayFileSetting(map, modes);

            string replayFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Replay");
            replay.replayFilePath = PathUtility.Combine(replayFolderPath, $"{map.info.id}.{replay.clearUTCTime.Ticks}.sdjk-replay");

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
