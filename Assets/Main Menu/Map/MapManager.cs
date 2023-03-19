using Cysharp.Threading.Tasks;
using K4.Threading;
using SCKRM;
using SCKRM.DragAndDrop;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Threads;
using SCKRM.UI.Overlay.MessageBox;
using SDJK.Map;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SDJK.MainMenu
{
    public static class MapManager
    {
        public static List<MapPack> currentMapPacks { get; private set; } = new List<MapPack>();
        public static int currentRulesetMapCount { get; private set; } = 0;

        public static int selectedMapPackIndex
        {
            get => _selectedMapPackIndex;
            set
            {
                _selectedMapPackIndex = value;
                selectedMapPack = currentMapPacks[value];
            }
        }
        static int _selectedMapPackIndex = 0;

        public static MapPack selectedMapPack
        {
            get => _selectedMapPack;
            set
            {
                _selectedMapPack = value;
                selectedMapIndex = 0;
            }
        }
        static MapPack _selectedMapPack = null;

        public static int selectedMapIndex
        {
            get => _selectedMapIndex;
            set
            {
                _selectedMapIndex = value;

                if (selectedMapPack != null)
                {
                    _selectedMap = selectedMapPack.maps[value];
                    selectedMapInfo = selectedMap.info;
                    selectedMapEffect = selectedMap.globalEffect;

                    RhythmManager.MapChange(selectedMap.globalEffect.bpm, selectedMap.info.songOffset, selectedMap.globalEffect.yukiMode);
                }
            }
        }
        public static int _selectedMapIndex = 0;

        public static MapFile selectedMap
        {
            get => _selectedMap;
            set
            {
                selectedMapPack = null;
                _selectedMap = value;

                if (value != null)
                {
                    selectedMapInfo = value.info;
                    selectedMapEffect = value.globalEffect;

                    RhythmManager.MapChange(selectedMap.globalEffect.bpm, selectedMap.info.songOffset, selectedMap.globalEffect.yukiMode);
                }
                else
                {
                    selectedMapInfo = null;
                    selectedMapEffect = null;
                }
            }
        }
        static MapFile _selectedMap = null;

        public static MapInfo selectedMapInfo { get; private set; } = null;
        public static MapGlobalEffect selectedMapEffect { get; private set; } = null;



        public static event Action mapLoadingEnd;



        [Awaken]
        public static void Awaken()
        {
            DragAndDropManager.dragAndDropEvent += DragAndDropEvent;

            ResourceManager.resourceRefreshEvent += MapListLoad;
            RulesetManager.isRulesetChanged += RulesetMapCountRefresh;
        }

        static bool DragAndDropEvent(string path, bool isFolder, ThreadMetaData threadMetaData)
        {
            if (!isFolder)
                return false;

            threadMetaData.name = "sdjk:notice.running_task.drag_and_drop.map_load.name";
            threadMetaData.info = "";

            threadMetaData.progress = 0;
            threadMetaData.maxProgress = 1;

            int index = -1;

            threadMetaData.cancelEvent += CancelEvent;
            K4UnityThreadDispatcher.Execute(async () =>
            {
                int result = await MessageBoxManager.Show(new NameSpacePathReplacePair[] { "sc-krm:gui.yes", "sc-krm:gui.no" },
                1,
                "sdjk:notice.running_task.drag_and_drop.map_load.warning",
                "sc-krm:0:gui/icon/exclamation_mark");

                Interlocked.CompareExchange(ref index, result, -1);
            });

            while (Interlocked.Add(ref index, 0) < 0)
                Thread.Sleep(1);

            if (index == 0)
            {
                DirectoryUtility.Copy(path, PathUtility.Combine(Kernel.persistentDataPath, "Map", Path.GetFileName(path)));
                K4UnityThreadDispatcher.Execute(MapListLoad);
            }

            threadMetaData.progress = 1;
            return true;

            void CancelEvent() => Interlocked.CompareExchange(ref index, 1, -1);
        }

        //void OnApplicationFocus(bool focus) => MapListLoad();

        //void OnDestroy() => ResourceManager.audioResetEnd -= MapListLoad;

        public static void RulesetMapCountRefresh()
        {
            currentRulesetMapCount = 0;

            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                MapPack mapPack = currentMapPacks[i];
                for (int j = 0; j < mapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(mapPack.maps[j].info.ruleset))
                        currentRulesetMapCount++;
                }
            }
        }

        static bool isMapListRefreshing = false;
        public static async UniTask MapListLoad()
        {
            if (isMapListRefreshing)
                return;

            isMapListRefreshing = true;

            try
            {
                Debug.ForceLog("Refreshing map list...", nameof(MapManager));

                AsyncTask asyncTask = new AsyncTask("sdjk:notice.running_task.map_list_refresh.name", "");
                if (ResourceManager.isResourceRefesh)
                    ResourceManager.resourceRefreshDetailedAsyncTask = asyncTask;

                List<MapPack> mapPacks = new List<MapPack>();
                string mapFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Map");
                if (!Directory.Exists(mapFolderPath))
                    Directory.CreateDirectory(mapFolderPath);

                string[] mapPackPaths = Directory.GetDirectories(mapFolderPath);
                if (mapPackPaths == null || mapPackPaths.Length <= 0)
                    return;

                asyncTask.maxProgress = mapPackPaths.Length;

                for (int i = 0; i < mapPackPaths.Length; i++)
                {
                    MapPack sdjkMapPack = await MapLoader.MapPackLoad(mapPackPaths[i].Replace("\\", "/"), asyncTask);
                    if (sdjkMapPack != null && sdjkMapPack.maps.Count > 0)
                        mapPacks.Add(sdjkMapPack);

                    if (asyncTask.isCanceled)
                        return;

                    asyncTask.progress++;
                }

                currentMapPacks = mapPacks;

                if (mapPacks.Count > 0 && ((selectedMapPack == null && selectedMap == null) || selectedMapPackIndex >= mapPacks.Count))
                    selectedMapPackIndex = UnityEngine.Random.Range(0, mapPacks.Count);
                else
                {
                    int mapIndex = selectedMapIndex;
                    selectedMapPackIndex = selectedMapPackIndex;
                    if (selectedMapIndex < selectedMapPack.maps.Count)
                        selectedMapIndex = mapIndex;
                    else
                        selectedMapIndex = 0;
                }

                if (InitialLoadManager.isInitialLoadEnd)
                    mapLoadingEnd?.Invoke();
            }
            finally
            {
                isMapListRefreshing = false;
            }
        }

        #region 가독성 씹창난 맵 이동 코드
        public static void RulesetBackMap()
        {
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (selectedMapIndex - 1 < 0)
                    selectedMapIndex = selectedMapPack.maps.Count - 1;
                else
                    selectedMapIndex--;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapInfo.ruleset))
                    break;
            }
        }

        public static void RulesetNextMap()
        {
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (selectedMapIndex + 1 >= selectedMapPack.maps.Count)
                    selectedMapIndex = 0;
                else
                    selectedMapIndex++;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapInfo.ruleset))
                    break;
            }
        }

        public static void RulesetBackMapPack()
        {
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (selectedMapPackIndex - 1 < 0)
                    selectedMapPackIndex = currentMapPacks.Count - 1;
                else
                    selectedMapPackIndex--;

                for (int j = 0; j < selectedMapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapPack.maps[j].info.ruleset))
                        return;
                }
            }
        }

        public static void RulesetNextMapPack()
        {
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (selectedMapPackIndex + 1 >= currentMapPacks.Count)
                    selectedMapPackIndex = 0;
                else
                    selectedMapPackIndex++;

                for (int j = 0; j < selectedMapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapPack.maps[j].info.ruleset))
                        return;
                }
            }
        }
        #endregion
    }
}