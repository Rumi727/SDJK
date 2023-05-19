using SCKRM;
using SCKRM.Json;
using System;
using System.Collections.Generic;
using SDJK.Mode;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Linq;
using SCKRM.Rhythm;
using SDJK.Mode.Converter;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SDJK.Map.Ruleset.SuperHexagon.Map
{
    public static class SuperHexagonMapLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("super_hexagon");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension, IMode[] modes) =>
            {
                bool typeIsSuperHexagonMap = type == typeof(SuperHexagonMapFile);
                if (typeIsSuperHexagonMap && !File.Exists(mapFilePath))
                    return new SuperHexagonMapFile("");

                if (extension == ".super_hexagon" && (type == typeof(MapFile) || typeIsSuperHexagonMap))
                    return MapLoad(mapFilePath, modes);

                //Ruleset 호환성
                if (typeIsSuperHexagonMap)
                {
                    //SDJK
                    if (extension == ".sdjk") //SDJK
                        return SDJKMapLoad(mapFilePath, modes);
                    else if (extension == ".adofai") //ADOFAI
                        return ADOFAIMapLoad(mapFilePath, modes);
                    else if (extension == ".osu") //osu!mania
                        return OsuMapLoad(mapFilePath, modes);
                }

                return null;
            };
        }



        public static SuperHexagonMapFile MapLoad(string mapFilePath, IMode[] modes)
        {
            SuperHexagonMapFile map = JsonManager.JsonRead<JObject>(mapFilePath, true).ToObject<SuperHexagonMapFile>();
            map.Init(mapFilePath);

            FixMode(map, modes);
            return map;
        }

        static void FixMode(SuperHexagonMapFile map, IMode[] modes)
        {
            if (modes == null)
                return;

            IMode keyCountMode;
            if ((keyCountMode = modes.FindMode<KeyCountModeBase>()) != null)
                KeyCountChange(map, ((KeyCountModeBase.Config)keyCountMode.modeConfig).count);
        }

        static void KeyCountChange(SuperHexagonMapFile map, int count)
        {
            int originalCount = map.notes.Count;
            if (count == originalCount)
                return;

            double offsetCount = (double)count / originalCount;

            TypeList<TypeList<SuperHexagonNoteFile>> newNoteLists = new TypeList<TypeList<SuperHexagonNoteFile>>();
            for (int i = 0; i < count; i++)
                newNoteLists.Add(new TypeList<SuperHexagonNoteFile>());

            if (count > originalCount)
            {
                //기존 노트 키 인덱스 늘리기
                for (int i = 0; i < originalCount; i++)
                    newNoteLists[(i * offsetCount).RoundToInt().Clamp(0, count - 1)] = map.notes[i];

                Random random = new Random(map.info.randomSeed);
                for (int i = 0; i < originalCount; i++)
                {
                    int keyIndex = (i * offsetCount).RoundToInt().Clamp(0, count - 1);
                    TypeList<SuperHexagonNoteFile> newNoteList = newNoteLists[keyIndex];

                    for (int j = 0; j < newNoteList.Count; j++)
                    {
                        //늘려진 노트들을 랜덤으로 쪼개기
                        int moveTargetKeyIndex = random.Next(keyIndex, (keyIndex + 1 + offsetCount).RoundToInt().Clamp(0, count));
                        if (keyIndex == moveTargetKeyIndex)
                            continue;

                        newNoteLists[moveTargetKeyIndex].Add(newNoteList[j]);
                        newNoteList.RemoveAt(j);
                        j--;
                    }
                }
            }
            else
            {
                //줄이는건 간-단
                for (int i = 0; i < originalCount; i++)
                {
                    int keyIndex = (i * offsetCount).RoundToInt().Clamp(0, count - 1);
                    newNoteLists[keyIndex] = newNoteLists[keyIndex].Union(map.notes[i]).ToTypeList();
                }
            }

            //Sides 이펙트
            for (int i = 0; i < map.effect.sidesList.Count; i++)
            {
                BeatValuePairAni<double> sides = map.effect.sidesList[i];
                sides.value = (sides.value * offsetCount).Round();

                map.effect.sidesList[i] = sides;
            }

            for (int i = 0; i < newNoteLists.Count; i++)
                newNoteLists[i] = newNoteLists[i].OrderBy(x => x.beat).ToTypeList();

            map.notes = newNoteLists;
        }

        static void HoldOff(SuperHexagonMapFile map)
        {
            for (int i = 0; i < map.notes.Count; i++)
            {
                TypeList<SuperHexagonNoteFile> notes = map.notes[i];
                for (int j = 0; j < notes.Count; j++)
                {
                    SuperHexagonNoteFile note = notes[j];
                    note.holdLength = 1;

                    notes[j] = note;
                }
            }
        }



        public static SuperHexagonMapFile SDJKMapLoad(string mapFilePath, IMode[] modes) => SDJKMapToSuperHexagonMap(mapFilePath, SDJKLoader.MapLoad(mapFilePath, modes), modes);



        public static SuperHexagonMapFile ADOFAIMapLoad(string mapFilePath, IMode[] modes) => SDJKMapToSuperHexagonMap(mapFilePath, SDJKLoader.ADOFAIMapLoad(mapFilePath, modes, false), modes);



        public static SuperHexagonMapFile OsuMapLoad(string mapFilePath, IMode[] modes) => SDJKMapToSuperHexagonMap(mapFilePath, SDJKLoader.OsuMapLoad(mapFilePath, modes), modes);



        static SuperHexagonMapFile SDJKMapToSuperHexagonMap(string mapFilePath, SDJKMapFile sdjkMap, IMode[] modes)
        {
            SuperHexagonMapFile superHexagonMap = new SuperHexagonMapFile(mapFilePath);
            
            #region Global Info Copy
            superHexagonMap.info = sdjkMap.info;
            superHexagonMap.globalEffect = sdjkMap.globalEffect;
            superHexagonMap.visualizerEffect = sdjkMap.visualizerEffect;
            superHexagonMap.postProcessEffect = sdjkMap.postProcessEffect;

            superHexagonMap.info.ruleset = "super_hexagon";
            #endregion

            #region Note
            for (int i = 0; i < sdjkMap.notes.Count; i++)
            {
                superHexagonMap.notes.Add(new TypeList<SuperHexagonNoteFile>());

                TypeList<SDJKNoteFile> sdjkNotes = sdjkMap.notes[i];
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

            superHexagonMap.effect.sidesList.Add(double.MinValue, 0, sdjkMap.notes.Count);

            if (0 < sdjkMap.effect.fieldEffect.Count)
            {
                SDJKFieldEffectFile sdjkFieldEffect = sdjkMap.effect.fieldEffect[0];
                for (int i = 0; i < sdjkFieldEffect.barEffect.Count; i++)
                {
                    SDJKBarEffectFile sdjkBarEffect = sdjkFieldEffect.barEffect[i];
                    SuperHexagonBarEffectFile superHexagonBarEffect = new SuperHexagonBarEffectFile();
                    superHexagonMap.effect.barEffect.Add(superHexagonBarEffect);

                    superHexagonBarEffect.noteDistance = sdjkBarEffect.noteDistance;
                    superHexagonBarEffect.noteConfig = sdjkBarEffect.noteConfig;
                }
            }
            #endregion

            FixMode(superHexagonMap, modes);
            return superHexagonMap;
        }
    }
}