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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SDJK.Map
{
    public static class MapManager
    {
        public static List<MapPack> currentMapPacks { get; private set; } = new List<MapPack>();


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

        public static Map selectedMap
        {
            get => _selectedMap;
            set
            {
                selectedMapPack = null;

                _selectedMap = value;
                selectedMapInfo = value.info;
                selectedMapEffect = value.globalEffect;
            }
        }
        static Map _selectedMap = null;

        public static MapInfo selectedMapInfo { get; private set; } = null;
        public static MapGlobalEffect selectedMapEffect { get; private set; } = null;



        public static event Action mapLoadingEnd;



        [Awaken]
        public static void Awaken() => ResourceManager.resourceRefreshEvent += MapListLoad;

        //void OnApplicationFocus(bool focus) => MapListLoad();

        //void OnDestroy() => ResourceManager.audioResetEnd -= MapListLoad;



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
    }
}