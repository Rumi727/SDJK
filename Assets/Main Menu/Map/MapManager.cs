using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SCKRM;
using SCKRM.FileDialog;
using SCKRM.Input;
using SCKRM.Json;
using SCKRM.NBS;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Ruleset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
                }
            }
        }
        public static int _selectedMapIndex = 0;

        public static Map.MapFile selectedMap
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
                }
                else
                {
                    selectedMapInfo = null;
                    selectedMapEffect = null;
                }
            }
        }
        static Map.MapFile _selectedMap = null;

        public static MapInfo selectedMapInfo { get; private set; } = null;
        public static MapGlobalEffect selectedMapEffect { get; private set; } = null;



        public static event Action mapLoadingEnd;



        [Awaken]
        public static void Awaken()
        {
            ResourceManager.resourceRefreshEvent += MapListLoad;
            RulesetManager.isRulesetChanged += RulesetMapCountRefresh;
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
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(mapPack.maps[j].info.mode))
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
                Debug.Log("MapManager: Refreshing map list...");

                AsyncTask asyncTask = new AsyncTask("notice.running_task.map_list_refresh.name", "");
                if (ResourceManager.isResourceRefesh)
                    ResourceManager.resourceRefreshDetailedAsyncTask = asyncTask;

                List<MapPack> mapPacks = new List<MapPack>();
                string mapFolderPath = PathTool.Combine(Kernel.persistentDataPath, "Map");
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
            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                if (MapManager.selectedMapIndex - 1 < 0)
                    MapManager.selectedMapIndex = MapManager.selectedMapPack.maps.Count - 1;
                else
                    MapManager.selectedMapIndex--;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapInfo.mode))
                    break;
            }
        }

        public static void RulesetNextMap()
        {
            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                if (MapManager.selectedMapIndex + 1 >= MapManager.selectedMapPack.maps.Count)
                    MapManager.selectedMapIndex = 0;
                else
                    MapManager.selectedMapIndex++;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapInfo.mode))
                    break;
            }
        }

        public static void RulesetBackMapPack()
        {
            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                if (MapManager.selectedMapPackIndex - 1 < 0)
                    MapManager.selectedMapPackIndex = MapManager.currentMapPacks.Count - 1;
                else
                    MapManager.selectedMapPackIndex--;

                for (int j = 0; j < MapManager.selectedMapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapPack.maps[j].info.mode))
                        return;
                }
            }
        }

        public static void RulesetNextMapPack()
        {
            for (int i = 0; i < MapManager.currentMapPacks.Count; i++)
            {
                if (MapManager.selectedMapPackIndex + 1 >= MapManager.currentMapPacks.Count)
                    MapManager.selectedMapPackIndex = 0;
                else
                    MapManager.selectedMapPackIndex++;

                for (int j = 0; j < MapManager.selectedMapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapPack.maps[j].info.mode))
                        return;
                }
            }
        }
        #endregion
    }
}