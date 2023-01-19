using SCKRM;
using SCKRM.Json;
using System.IO;
using UnityEngine;

namespace SDJK.Replay
{
    public static class ReplayLoader
    {
        public static T ReplayLoad<T>(string replayFilePath) where T : ReplayFile, new()
        {
            if (File.Exists(replayFilePath))
            {
                T replayFile = JsonManager.JsonRead<T>(replayFilePath, true);
                replayFile.replayFilePath = replayFilePath;

                return replayFile;
            }

            return null;
        }

        public static void ReplaySave<T>(this T replayFile, string savePath) where T : ReplayFile, new()
        {
            replayFile.sckrmVersion = Kernel.sckrmVersion;
            replayFile.sdjkVersion = new Version(Application.version);

            File.WriteAllText(savePath, JsonManager.ObjectToJson(replayFile));
        }
    }
}
