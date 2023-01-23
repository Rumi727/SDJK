using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.FileDialog;
using System;
using System.Collections.Generic;
using System.IO;
using Version = SCKRM.Version;

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
        public delegate MapFile MapLoaderFunc(Type type, string mapFilePath, string extension);
        public static event MapLoaderFunc mapLoaderFunc;
        public static List<string> extensionToLoad { get; } = new List<string>();

        public static async UniTask<MapPack> MapPackLoad(string packfolderPath, AsyncTask asyncTask)
        {
            string[] packPaths = DirectoryUtility.GetFiles(packfolderPath, new ExtensionFilter(extensionToLoad.ToArray()).ToSearchPatterns());
            if (packPaths == null || packPaths.Length <= 0)
                return null;

            MapPack pack = new MapPack();
            for (int i = 0; i < packPaths.Length; i++)
            {
                MapFile map = MapLoad<MapFile>(packPaths[i].Replace("\\", "/"));
                if (map != null)
                    pack.maps.Add(map);

                if (await UniTask.NextFrame(asyncTask.cancel).SuppressCancellationThrow())
                    return null;
            }

            return pack;
        }

        public static T MapLoad<T>(string mapFilePath) where T : MapFile, new()
        {
            Delegate[] delegates = mapLoaderFunc.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                MapLoaderFunc action = (MapLoaderFunc)delegates[i];
                object map = action.Invoke(typeof(T), mapFilePath, Path.GetExtension(mapFilePath));
                if (map != null)
                {
                    T sdjkMap = (T)map;

                    sdjkMap.mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
                    sdjkMap.mapFilePath = mapFilePath;

                    sdjkMap.info.sckrmVersion = Kernel.sckrmVersion;
                    sdjkMap.info.sdjkVersion = (Version)Kernel.version;

                    sdjkMap.info.ResetMapID(mapFilePath);
                    sdjkMap.SetVisualizerEffect();

                    return sdjkMap;
                }
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
