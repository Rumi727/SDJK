using SCKRM;
using SCKRM.FileDialog;
using SCKRM.Json;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SDJK
{
    public static class MapLoader
    {
        public static MapPack MapPackLoad(string packfolderPath)
        {
            string[] packPaths = DirectoryTool.GetFiles(packfolderPath, new ExtensionFilter(MapCompatibilitySystem.compatibleMapExtensions).ToSearchPatterns());
            if (packPaths == null || packPaths.Length <= 0)
                return null;

            MapPack pack = new MapPack();
            for (int i = 0; i < packPaths.Length; i++)
            {
                Map.Map map = MapLoad(packPaths[i].Replace("\\", "/"));
                if (map != null)
                    pack.maps.Add(map);
            }

            return pack;
        }

        public static Map.Map MapLoad(string mapFilePath)
        {
            Map.Map sdjkMap = MapCompatibilitySystem.GlobalMapCompatibility(mapFilePath);
            if (sdjkMap == null)
                return null;

            sdjkMap.mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
            sdjkMap.mapFilePath = mapFilePath;

            return sdjkMap;   
        }
    }
}
