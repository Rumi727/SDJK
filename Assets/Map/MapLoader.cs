using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.FileDialog;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.UI.SideBar;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SDJK.Map
{
    public static class MapLoader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="type">맵의 타입</param>
        /// <param name="mapFilePath">맵 파일의 경로</param>
        /// <param name="extension">맵 파일의 확장자</param>
        /// <returns>
        /// 맵 인스턴스
        /// </returns>
        public delegate MapFile MapLoaderFunc(Type type, string mapFilePath, string extension, params IMode[] modes);
        public static event MapLoaderFunc mapLoaderFunc;
        public static List<string> extensionToLoad { get; } = new List<string>();

        public static async UniTask<MapPack> MapPackLoad(string packfolderPath, AsyncTask asyncTask, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            string[] packPaths = DirectoryUtility.GetFiles(packfolderPath, new ExtensionFilter(extensionToLoad.ToArray()).ToSearchPatterns());
            if (packPaths == null || packPaths.Length <= 0)
                return null;

            MapPack pack = new MapPack();

            {
                //병렬 로드
                SynchronizedCollection<MapFile> maps = new SynchronizedCollection<MapFile>();
                int loadedMapCount = 0;

                for (int i = 0; i < packPaths.Length; i++)
                {
                    string path = packPaths[i];
                    UniTask.RunOnThreadPool(() => MapLoad(path)).Forget();

                    void MapLoad(string path)
                    {
                        try
                        {
                            MapFile map = MapLoad<MapFile>(path.Replace("\\", "/"), modes);
                            if (map != null)
                                maps.Add(map);
                        }
                        finally
                        {
                            Interlocked.Increment(ref loadedMapCount);
                        }
                    }
                }

                if (await UniTask.WaitUntil(() => Interlocked.Add(ref loadedMapCount, 0) >= packPaths.Length, PlayerLoopTiming.Update, asyncTask.cancel).SuppressCancellationThrow()
                    || asyncTask.isCanceled
                    || !Kernel.isPlaying)
                    return null;

                pack.maps = maps.ToTypeList();
            }

            pack.maps = pack.maps.OrderBy(x => x.difficultyAverage).ThenBy(x => x.info.difficultyLabel).ToTypeList();
            return pack;
        }

        public static T MapLoad<T>(string mapFilePath, params IMode[] modes) where T : MapFile
        {
            if (modes == null)
                modes = IMode.emptyModes;

            try
            {
                if (mapLoaderFunc != null)
                {
                    Delegate[] delegates = mapLoaderFunc.GetInvocationList();
                    for (int i = 0; i < delegates.Length; i++)
                    {
                        MapLoaderFunc action = (MapLoaderFunc)delegates[i];
                        object map = action.Invoke(typeof(T), mapFilePath, Path.GetExtension(mapFilePath), modes);

                        if (map != null)
                        {
                            T sdjkMap = (T)map;
                            return sdjkMap;
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                string longPathText = ResourceManager.SearchLanguage("notice.map_loader.long_path", "sdjk");

                Debug.ForceLogError("[Path] " + mapFilePath);
                Debug.ForceLogWarning(longPathText);

                Debug.LogException(e);

                //NameSpacePathReplacePair 구조체로 따로 빼는 이유는 경로에 ':' 문자가 들어가서 앞 부분을 네임스페이스로 인식하기 때문입니다
                string noticeText = $@"[Path]
{mapFilePath}

[Info]
{longPathText}

[Exception]
{e.ToSummaryString()}";
                NoticeManager.Notice("sdjk:notice.map_loader.fall_map_load", new NameSpacePathReplacePair(noticeText), NoticeManager.Type.warning);
            }
            catch (Exception e)
            {
                Debug.ForceLogError("[Path] " + mapFilePath);
                Debug.LogException(e);

                //NameSpacePathReplacePair 구조체로 따로 빼는 이유는 경로에 ':' 문자가 들어가서 앞 부분을 네임스페이스로 인식하기 때문입니다
                string noticeText = $@"[Path]
{mapFilePath}

[Exception]
{e.ToSummaryString()}";
                NoticeManager.Notice("sdjk:notice.map_loader.fall_map_load", new NameSpacePathReplacePair(noticeText), NoticeManager.Type.warning);
            }

            return null;
        }

        /*static void MapCompatibilityPatch(JObject jObjectMap, MapFile map)
        {
            {
                JToken token = jObjectMap.SelectToken("globalEffect.dropPart");
                if (token != null)
                {
                    map.globalEffect.yukiMode.Clear();
                    map.globalEffect.yukiMode.AddRange(token.ToObject<SCKRM.Rhythm.BeatValuePairList<bool>>());
                }
            }
        }*/
    }
}
