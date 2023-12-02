using Cysharp.Threading.Tasks;
using K4.Threading;
using Newtonsoft.Json;
using SCKRM;
using SCKRM.DragAndDrop;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.SaveLoad;
using SCKRM.Threads;
using SCKRM.UI.Overlay.MessageBox;
using SDJK.Map;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SDJK.MainMenu
{
    public static class MapManager
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool loadInParallel { get; set; } = false;
        }

        public static List<MapPack> currentMapPacks { get; private set; } = new List<MapPack>();
        public static List<MapPack> currentRulesetMapPacks { get; private set; } = new List<MapPack>();

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

                for (int i = 0; i < selectedMapPack.maps.Count; i++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapPack.maps[i].info.ruleset))
                    {
                        selectedMapIndex = i;
                        return;
                    }
                }

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

                    RhythmManager.MapChange(selectedMap.globalEffect.bpm, selectedMap.info.songOffset, selectedMap.globalEffect.yukiMode, selectedMap.info.clearBeat);
                }
            }
        }
        static int _selectedMapIndex = 0;

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

                    RhythmManager.MapChange(selectedMap.globalEffect.bpm, selectedMap.info.songOffset, selectedMap.globalEffect.yukiMode, selectedMap.info.clearBeat);
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
            RulesetManager.rulesetChanged += RulesetMapRefresh;
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

        public static void RulesetMapRefresh()
        {
            currentRulesetMapPacks.Clear();

            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                MapPack mapPack = currentMapPacks[i];
                for (int j = 0; j < mapPack.maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(mapPack.maps[j].info.ruleset))
                    {
                        currentRulesetMapPacks.Add(currentMapPacks[i]);
                        continue;
                    }
                }
            }
        }

        static bool isMapListRefreshing = false;
        public static async UniTask MapListLoad()
        {
            if (isMapListRefreshing)
                return;

            isMapListRefreshing = true;

            Debug.ForceLog("Refreshing map list...", nameof(MapManager));

            AsyncTask asyncTask = new AsyncTask("sdjk:notice.running_task.map_list_refresh.name", "", false, false);
            if (ResourceManager.isResourceRefesh)
                ResourceManager.resourceRefreshDetailedAsyncTask = asyncTask;

            try
            {
                List<MapPack> mapPacks = new List<MapPack>();
                string mapFolderPath = PathUtility.Combine(Kernel.persistentDataPath, "Map");
                if (!Directory.Exists(mapFolderPath))
                    Directory.CreateDirectory(mapFolderPath);

                string[] mapPackPaths = Directory.GetDirectories(mapFolderPath);
                if (mapPackPaths == null || mapPackPaths.Length <= 0)
                    return;

                asyncTask.maxProgress = mapPackPaths.Length;

                {
                    //병렬 로드
                    SynchronizedCollection<MapPack> syncMapPacks = new SynchronizedCollection<MapPack>();
                    int loadedMapCount = 0;

                    for (int i = 0; i < mapPackPaths.Length; i++)
                    {
                        string path = mapPackPaths[i];

                        if (SaveData.loadInParallel)
                        {
                            UniTask.RunOnThreadPool(() => MapLoad(path)).Forget();
                            await UniTask.NextFrame();
                        }
                        else
                            await MapLoad(path);

                        if (asyncTask.isRemoved || !Kernel.isPlaying)
                            return;

                        async UniTask MapLoad(string path)
                        {
                            try
                            {
                                MapPack sdjkMapPack = await MapLoader.MapPackLoad(path.Replace("\\", "/"), asyncTask, true);
                                if (sdjkMapPack != null && sdjkMapPack.maps.Count > 0)
                                    syncMapPacks.Add(sdjkMapPack);

                                if (asyncTask.isCanceled)
                                    return;

                                asyncTask.progress++;
                            }
                            finally
                            {
                                Interlocked.Increment(ref loadedMapCount);
                            }
                        }
                    }

                    if (SaveData.loadInParallel)
                    {
                        if (await UniTask.WaitUntil(() => Interlocked.Add(ref loadedMapCount, 0) >= mapPackPaths.Length, PlayerLoopTiming.Update, asyncTask.cancel).SuppressCancellationThrow()
                            || asyncTask.isRemoved
                            || !Kernel.isPlaying)
                            return;
                    }

                    mapPacks = syncMapPacks.ToList();
                }

                mapPacks = mapPacks.OrderBy(x => x.maps[0].info.songName).ThenBy(x => x.maps[0].info.artist).ToList();
                currentMapPacks = mapPacks;

                if (RulesetManager.selectedRuleset != null)
                {
                    RulesetMapRefresh();

                    if (mapPacks.Count > 0 && ((selectedMapPack == null && selectedMap == null) || selectedMapPackIndex >= mapPacks.Count))
                        RulesetRandomMapPack();
                    else
                    {
                        int mapIndex = selectedMapIndex;
                        selectedMapPackIndex = selectedMapPackIndex;
                        if (selectedMapIndex < selectedMapPack.maps.Count)
                            selectedMapIndex = mapIndex;
                        else
                            selectedMapIndex = 0;
                    }
                }

                if (InitialLoadManager.isInitialLoadEnd)
                    mapLoadingEnd?.Invoke();
            }
            finally
            {
                if (!ResourceManager.isResourceRefesh)
                    asyncTask.Remove(true);

                isMapListRefreshing = false;
            }
        }

        public static void RulesetRandomMapPack()
        {
            int index = UnityEngine.Random.Range(0, currentRulesetMapPacks.Count);
            selectedMapPackIndex = currentMapPacks.FindIndex(x => x == currentRulesetMapPacks[index]);
        }

        #region 가독성 씹창난 맵 이동 코드
        public static void RulesetBackMap()
        {
            int index = selectedMapIndex;

            for (int i = 0; i < selectedMapPack.maps.Count; i++)
            {
                if (index - 1 < 0)
                    index = selectedMapPack.maps.Count - 1;
                else
                    index--;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapPack.maps[index].info.ruleset))
                    break;
            }

            selectedMapIndex = index;
        }

        public static void RulesetNextMap()
        {
            int index = selectedMapIndex;
            for (int i = 0; i < selectedMapPack.maps.Count; i++)
            {
                if (index + 1 >= selectedMapPack.maps.Count)
                    index = 0;
                else
                    index++;

                if (RulesetManager.selectedRuleset.IsCompatibleRuleset(selectedMapPack.maps[index].info.ruleset))
                    break;
            }

            selectedMapIndex = index;
        }

        public static void RulesetBackMapPack()
        {
            int index = selectedMapPackIndex;
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (index - 1 < 0)
                    index = currentMapPacks.Count - 1;
                else
                    index--;

                bool compatible = false;
                List<MapFile> maps = currentMapPacks[index].maps;
                for (int j = 0; j < maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(maps[j].info.ruleset))
                    {
                        compatible = true;
                        break;
                    }
                }

                if (compatible)
                    break;
            }

            selectedMapPackIndex = index;
        }

        public static void RulesetNextMapPack()
        {
            int index = selectedMapPackIndex;
            for (int i = 0; i < currentMapPacks.Count; i++)
            {
                if (index + 1 >= currentMapPacks.Count)
                    index = 0;
                else
                    index++;

                bool compatible = false;
                List<MapFile> maps = currentMapPacks[index].maps;
                for (int j = 0; j < maps.Count; j++)
                {
                    if (RulesetManager.selectedRuleset.IsCompatibleRuleset(maps[j].info.ruleset))
                    {
                        compatible = true;
                        break;
                    }
                }

                if (compatible)
                    break;
            }

            selectedMapPackIndex = index;
        }
        #endregion
    }
}