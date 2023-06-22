using K4.Threading;
using SCKRM;
using SCKRM.DragAndDrop;
using SCKRM.Threads;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset;
using System.IO;

namespace SDJK.MainMenu
{
    public static class ReplayManager
    {
        [Awaken]
        public static void Awaken() => DragAndDropManager.dragAndDropEvent += DragAndDropEvent;

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
    }
}
