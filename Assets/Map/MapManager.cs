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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SDJK.Map
{
    public sealed class MapManager : Manager<MapManager>
    {
        public static List<SDJKMapPack> currentMapPacks { get; } = new List<SDJKMapPack>();


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

        public static SDJKMapPack selectedMapPack 
        {
            get => _selectedMapPack;
            set
            {
                _selectedMapPack = value;
                selectedMapIndex = 0;
            }
        }
        static SDJKMapPack _selectedMapPack = null;

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

        public static SDJKMap selectedMap 
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
        static SDJKMap _selectedMap = null;

        public static SDJKMapInfo selectedMapInfo { get; private set; } = null;
        public static SDJKMapGlobalEffect selectedMapEffect { get; private set; } = null;



        public static bool isMapLoading { get; private set; } = false;



        async UniTaskVoid Awake()
        {
            if (SingletonCheck(this))
            {
                if (await UniTask.WaitUntil(() => InitialLoadManager.isInitialLoadEnd, PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;

                MapListLoad();
                //ResourceManager.audioResetEnd += MapListLoad;
            }
        }

        void Update()
        {
            if (!InitialLoadManager.isInitialLoadEnd)
                return;

            if (InputManager.GetKey(KeyCode.LeftArrow))
            {
                if (selectedMapPackIndex - 1 < 0)
                    selectedMapPackIndex = currentMapPacks.Count - 1;
                else
                    selectedMapPackIndex--;

                selectedMapPack = currentMapPacks[selectedMapPackIndex];
            }
            if (InputManager.GetKey(KeyCode.RightArrow))
            {
                if (selectedMapPackIndex + 1 >= currentMapPacks.Count)
                    selectedMapPackIndex = 0;
                else
                    selectedMapPackIndex++;

                selectedMapPack = currentMapPacks[selectedMapPackIndex];
            }
        }

        void OnApplicationFocus(bool focus) => MapListLoad();

        //void OnDestroy() => ResourceManager.audioResetEnd -= MapListLoad;



        public static void MapListLoad()
        {
            if (isMapLoading)
                return;

            isMapLoading = true;

            currentMapPacks.Clear();

            string mapFolderPath = PathTool.Combine(Kernel.persistentDataPath, "Map");
            if (!Directory.Exists(mapFolderPath))
                Directory.CreateDirectory(mapFolderPath);
            
            string[] mapPackPaths = Directory.GetDirectories(mapFolderPath);
            if (mapPackPaths == null || mapPackPaths.Length <= 0)
            {
                isMapLoading = false;
                return;
            }

            for (int i = 0; i < mapPackPaths.Length; i++)
            {
                string mapPackPath = mapPackPaths[i].Replace("\\", "/");
                string[] mapPaths = Directory.GetFiles(mapPackPath, "*.sdjk");
                if (mapPaths == null || mapPaths.Length <= 0)
                    continue;

                SDJKMapPack sdjkMapPack = new SDJKMapPack();
                for (int j = 0; j < mapPaths.Length; j++)
                {
                    string mapFilePath = mapPaths[j].Replace("\\", "/");
                    SDJKMap sdjkMap = JsonManager.JsonRead<SDJKMap>(mapFilePath, true);
                    if (sdjkMap == null)
                        continue;

                    sdjkMap.mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
                    sdjkMap.mapFilePath = mapFilePath;

                    sdjkMapPack.maps.Add(sdjkMap);
                }

                if (sdjkMapPack.maps.Count > 0)
                    currentMapPacks.Add(sdjkMapPack);
            }

            isMapLoading = false;

            if (selectedMapPack == null && selectedMap == null && currentMapPacks.Count > 0)
                selectedMapPackIndex = UnityEngine.Random.Range(0, currentMapPacks.Count);
        }
    }
}