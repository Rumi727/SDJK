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

            FixAllJudgmentBeat(map);
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

            #region Note
            for (int i = 0; i < sdjkMap.notes.Count; i++)
            {
                superHexagonMap.notes.Add(new List<SuperHexagonNoteFile>());

                List<SDJKNoteFile> sdjkNotes = sdjkMap.notes[i];
                for (int j = 0; j < sdjkNotes.Count; j++)
                {
                    SDJKNoteFile sdjkNote = sdjkNotes[j];
                    if (sdjkNote.type == SDJKNoteTypeFile.instantDeath)
                        continue;

                    superHexagonMap.notes[i].Add(new SuperHexagonNoteFile(sdjkNote.beat, sdjkNote.holdLength));
                }
            }
            #endregion

            #region Effect
            superHexagonMap.effect.globalNoteDistance = sdjkMap.effect.globalNoteDistance;
            superHexagonMap.effect.globalNoteSpeed = sdjkMap.effect.globalNoteSpeed;

            superHexagonMap.sides.Add(double.MinValue, 0, sdjkMap.notes.Count);
            superHexagonMap.effect.fieldZRotationSpeed.Add(double.MinValue, 0, 1);
            #endregion

            FixAllJudgmentBeat(superHexagonMap);
            return superHexagonMap;
        }

        static void FixAllJudgmentBeat(SuperHexagonMapFile map)
        {
            map.allJudgmentBeat.Clear();

            for (int i = 0; i < map.notes.Count; i++)
            {
                List<SuperHexagonNoteFile> notes = map.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    SuperHexagonNoteFile note = notes[j];

                    //모든 판정 비트에 노트 추가
                    map.allJudgmentBeat.Add(note.beat);
                    if (note.holdLength > 0)
                        map.allJudgmentBeat.Add(note.beat + note.holdLength);
                }
            }

            map.allJudgmentBeat.Sort();
        }
    }
}