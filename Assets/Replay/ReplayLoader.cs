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

        public static T ReplayLoad<T>(string replayFilePath, out IMode[] modes) where T : ReplayFile, new()
        {
            if (File.Exists(replayFilePath))
            {
                T replayFile = JsonManager.JsonRead<T>(replayFilePath, true);
                replayFile.replayFilePath = replayFilePath;

                modes = new IMode[replayFile.modes.Length];
                for (int i = 0; i < replayFile.modes.Length; i++)
                {
                    ReplayModeFile replayMode = replayFile.modes[i];
                    IMode mode = (IMode)Activator.CreateInstance(replayMode.modeType);

                    if (replayMode.modeConfigType != null)
                        mode.modeConfig = (IModeConfig)(new JObject(replayMode.modeConfig).ToObject(replayMode.modeConfigType));

                    modes[i] = mode;
                }

                return replayFile;
            }

            modes = null;
            return null;
        }

        public static string GetLastReplayPath(MapFile map) => $"{map.mapFilePath}.{map.info.ruleset}-lastReplay";

        public static void ReplaySave<T>(this T replay, MapFile map, IMode[] modes, string savePath) where T : ReplayFile, new()
        {
            replay.InternalReplayFileSetting(map, modes);
            File.WriteAllText(savePath, JsonManager.ObjectToJson(replay));
        }

        public static void LastReplaySave<T>(this T replay, MapFile map, IMode[] modes) where T : ReplayFile, new()
        {
            replay.InternalReplayFileSetting(map, modes);
            File.WriteAllText(GetLastReplayPath(map), JsonManager.ObjectToJson(replay));
        }

        static void InternalReplayFileSetting<T>(this T replay, MapFile map, IMode[] modes) where T : ReplayFile, new()
        {
            replay.mapId = map.info.id;

            replay.sckrmVersion = Kernel.sckrmVersion;
            replay.sdjkVersion = new Version(Application.version);

            replay.clearUTCTime = NTPDateTime.utcNow;

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
