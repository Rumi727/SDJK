using SCKRM.Json;
using System.IO;

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
    }
}
