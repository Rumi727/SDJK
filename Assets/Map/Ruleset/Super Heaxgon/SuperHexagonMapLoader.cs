using static MoreLinq.Extensions.MaxByExtension;
using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.ADOFAI;
using System;
using System.Collections.Generic;
using System.Linq;
using SDJK.Mode;
using SDJK.Mode.Converter;

using Random = System.Random;
using SDJK.Map.Ruleset.SDJK.Map;

namespace SDJK.Map.Ruleset.SuperHexagon.Map
{
    public static class SuperHexagonLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("super_hexagon");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension, IMode[] modes) =>
            {
                bool typeIsSuperHexagonMap = type == typeof(SuperHexagonMapFile);
                if (extension == ".super_hexagon" && (type == typeof(MapFile) || typeIsSuperHexagonMap))
                    return MapLoad(mapFilePath, modes);

                //Ruleset 호환성
                if (typeIsSuperHexagonMap)
                {
                    //SDJK
                    if (extension == ".sdjk")
                        return SDJKMapLoad(mapFilePath, modes);
                }

                return null;
            };
        }

        public static SuperHexagonMapFile MapLoad(string mapFilePath, IMode[] modes)
        {
            SuperHexagonMapFile map = JsonManager.JsonRead<SuperHexagonMapFile>(mapFilePath, true);
            map.Init(mapFilePath);

            return map;
        }

        public static SuperHexagonMapFile SDJKMapLoad(string mapFilePath, IMode[] modes)
        {
            SuperHexagonMapFile superHexagonMap = new SuperHexagonMapFile();
            superHexagonMap.Init(mapFilePath);

            SDJKMapFile sdjkMap = SDJKLoader.MapLoad(mapFilePath, modes);

            #region Global Info Copy
            superHexagonMap.info = sdjkMap.info;
            superHexagonMap.globalEffect = sdjkMap.globalEffect;
            superHexagonMap.visualizerEffect = sdjkMap.visualizerEffect;

            superHexagonMap.info.ruleset = "super_hexagon";
            #endregion

            superHexagonMap.sides.Add(double.MinValue, 0, sdjkMap.notes.Count);
            superHexagonMap.effect.fieldZRotationSpeed.Add(double.MinValue, 0, 1);

            return superHexagonMap;
        }
    }
}