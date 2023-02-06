using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Json;
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

        public static void ReplaySave<T>(this T replayFile, MapFile map, IMode[] modes, string savePath) where T : ReplayFile, new()
        {
            replayFile.mapId = map.info.id;
            
            replayFile.sckrmVersion = Kernel.sckrmVersion;
            replayFile.sdjkVersion = new Version(Application.version);

            {
                replayFile.modes = new ReplayModeFile[modes.Length];
                ReplayModeFile[] replayModeFiles = replayFile.modes;
                for (int i = 0; i < modes.Length; i++)
                {
                    IMode mode = modes[i];
                    replayModeFiles[i] = new ReplayModeFile(mode.GetType(), mode.modeConfig);
                }
            }

            File.WriteAllText(savePath, JsonManager.ObjectToJson(replayFile));
        }
    }
}
