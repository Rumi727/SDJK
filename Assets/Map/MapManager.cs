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

        //void OnApplicationFocus(bool focus) => MapListLoad();

        //void OnDestroy() => ResourceManager.audioResetEnd -= MapListLoad;



        public static void MapListLoad()
        {
            currentMapPacks.Clear();

            string mapFolderPath = PathTool.Combine(Kernel.persistentDataPath, "Map");
            if (!Directory.Exists(mapFolderPath))
                Directory.CreateDirectory(mapFolderPath);
            
            string[] mapPackPaths = Directory.GetDirectories(mapFolderPath);
            if (mapPackPaths == null || mapPackPaths.Length <= 0)
                return;

            for (int i = 0; i < mapPackPaths.Length; i++)
            {
                SDJKMapPack sdjkMapPack = MapLoader.MapPackLoad(mapPackPaths[i].Replace("\\", "/"));
                if (sdjkMapPack != null && sdjkMapPack.maps.Count > 0)
                    currentMapPacks.Add(sdjkMapPack);
            }

            if (currentMapPacks.Count > 0 && ((selectedMapPack == null && selectedMap == null) || selectedMapPackIndex >= currentMapPacks.Count))
                selectedMapPackIndex = UnityEngine.Random.Range(0, currentMapPacks.Count);
            else
            {
                int mapIndex = selectedMapIndex;
                selectedMapPackIndex = selectedMapPackIndex;
                if (selectedMapIndex < selectedMapPack.maps.Count)
                    selectedMapIndex = mapIndex;
                else
                    selectedMapIndex = 0;
            }

            mapLoadingEnd?.Invoke();
        }
    }
}