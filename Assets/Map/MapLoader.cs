using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.FileDialog;
using SCKRM.Json;
using SDJK.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        public delegate Map MapLoaderFunc(Type type, string mapFilePath, string extension);
        public static event MapLoaderFunc mapLoaderFunc;
        public static List<string> extensionToLoad { get; } = new List<string>();

        public static async UniTask<MapPack> MapPackLoad(string packfolderPath, AsyncTask asyncTask)
        {
            string[] packPaths = DirectoryTool.GetFiles(packfolderPath, new ExtensionFilter(extensionToLoad.ToArray()).ToSearchPatterns());
            if (packPaths == null || packPaths.Length <= 0)
                return null;

            MapPack pack = new MapPack();
            for (int i = 0; i < packPaths.Length; i++)
            {
                Map map = MapLoad<Map>(packPaths[i].Replace("\\", "/"));
                if (map != null)
                    pack.maps.Add(map);

                if (await UniTask.NextFrame(asyncTask.cancel).SuppressCancellationThrow())
                    return null;
            }

            return pack;
        }

        public static T MapLoad<T>(string mapFilePath) where T : Map, new()
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

                    return sdjkMap;
                }
            }

            return null;
        }
    }
}
