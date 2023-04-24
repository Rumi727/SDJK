using SCKRM;
using SCKRM.Rhythm;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SDJK.Map.Ruleset.OsuMania
{
    public static class OsuManiaMapLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("osu");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension, IMode[] modes) =>
            {
                bool typeIsOsuManiaMap = type == typeof(OsuManiaMapFile);
                if (typeIsOsuManiaMap && !File.Exists(mapFilePath))
                    return new OsuManiaMapFile("");

                if (extension == ".osu" && (type == typeof(MapFile) || typeIsOsuManiaMap))
                    return MapLoad(mapFilePath);
                else
                    return null;
            };
        }

        public static OsuManiaMapFile MapLoad(string mapFilePath)
        {
            OsuManiaMapFile osuManiaMap = new OsuManiaMapFile(mapFilePath);

            if (File.Exists(mapFilePath))
            {
                StreamReader streamReader = File.OpenText(mapFilePath);

                osuManiaMap.info.ruleset = "osu!mania";
                osuManiaMap.globalEffect.volume.Add(double.MinValue, 0, 0.5f);

                /*osuManiaMap.globalEffect.uiSize.Add(double.MinValue, 0, 0.6666666667);

                osuManiaMap.effect.fieldEffect.Add(new OsuManiaFieldEffectFile());
                osuManiaMap.effect.fieldEffect[0].height.Add(double.MinValue, 0, 24);

                osuManiaMap.effect.globalNoteDistance.Add(double.MinValue, 0, 14);*/

                bool startLine = true;
                string section = "";

                bool eventsSectionBackgroundIgnore = false;
                bool eventsSectionVideoIgnore = false;

                bool timingPointsSectionStartLine = true;
                double timingPointsSectionLastTime = 0;
                double timingPointsSectionLastBeat = 0;
                double timingPointsSectionBPM = 60;

                bool isDifficultyCalculated = false;
                bool isTimingPointsCalculated = false;

                int keyCount = 4;
                while (!streamReader.EndOfStream)
                {
                    string text = streamReader.ReadLine();

                    try
                    {
                        if (startLine)
                        {
                            if (!text.StartsWith("osu file format v"))
                                return null;

                            startLine = false;
                        }
                        else if (!string.IsNullOrWhiteSpace(text))
                        {
                            if (text.StartsWith('[') && text.EndsWith("]"))
                            {
                                section = text;
                                continue;
                            }

                            switch (section)
                            {
                                #region General Section
                                case "[General]":
                                {
                                    string[] splitText = text.QuotedSplit(":");
                                    string key = splitText[0].Trim();
                                    string value = splitText[1].Trim();

                                    switch (key)
                                    {
                                        case "AudioFilename":
                                            osuManiaMap.info.songFile = PathUtility.GetPathWithExtension(value);
                                            break;
                                        case "PreviewTime":
                                            osuManiaMap.info.mainMenuStartTime = int.Parse(value) * 0.001f;
                                            break;
                                        case "Mode":
                                        {
                                            if (value != "3")
                                                throw new NotSupportedException("Only supports osu!mania beatmap");

                                            break;
                                        }
                                    }

                                    break;
                                }
                                #endregion

                                #region MetaData Section
                                case "[Metadata]":
                                {
                                    string[] splitText = text.QuotedSplit(":");
                                    string key = splitText[0].Trim();
                                    if (splitText.Length < 2)
                                        continue;

                                    string value = splitText[1].Trim();

                                    switch (key)
                                    {
                                        case "Title":
                                            osuManiaMap.info.songName = value;
                                            break;
                                        case "Artist":
                                            osuManiaMap.info.artist = value;
                                            break;
                                        case "Creator":
                                            osuManiaMap.info.author = value;
                                            break;
                                        case "Version":
                                            osuManiaMap.info.difficultyLabel = value + " (osu!)";
                                            break;
                                        case "Source":
                                            osuManiaMap.info.original = value;
                                            break;
                                        case "Tags":
                                            osuManiaMap.info.tag = value.QuotedSplit(" ");
                                            break;
                                    }

                                    break;
                                }
                                #endregion

                                #region Difficulty Section
                                case "[Difficulty]":
                                {
                                    string[] splitText = text.QuotedSplit(":");
                                    string key = splitText[0].Trim();
                                    string value = splitText[1].Trim();

                                    if (key == "CircleSize")
                                    {
                                        keyCount = int.Parse(value);
                                        osuManiaMap.notes.Clear();

                                        for (int i = 0; i < keyCount; i++)
                                            osuManiaMap.notes.Add(new TypeList<OsuManiaNoteFile>());
                                    }

                                    isDifficultyCalculated = true;
                                    break;
                                }
                                #endregion

                                #region Events Section
                                case "[Events]":
                                {
                                    string[] splitText = text.QuotedSplit(",");
                                    string eventType = splitText[0];

                                    switch (eventType)
                                    {
                                        case "0":
                                        {
                                            if (!eventsSectionBackgroundIgnore)
                                            {
                                                osuManiaMap.globalEffect.background.Add(new BackgroundEffectPair(PathUtility.GetPathWithExtension(splitText[2].Trim('"')), ""));
                                                eventsSectionBackgroundIgnore = true;
                                            }

                                            break;
                                        }
                                        case "1":
                                        case "Video":
                                        {
                                            if (!eventsSectionVideoIgnore)
                                            {
                                                osuManiaMap.info.videoBackgroundFile = PathUtility.GetPathWithExtension(splitText[2].Trim('"'));
                                                osuManiaMap.info.videoOffset = double.Parse(splitText[1]);

                                                eventsSectionVideoIgnore = true;
                                            }

                                            break;
                                        }
                                    }

                                    break;
                                }
                                #endregion

                                #region Timing Points Section
                                case "[TimingPoints]":
                                {
                                    string[] splitText = text.QuotedSplit(",");
                                    double time = int.Parse(splitText[0]) * 0.001;
                                    double bpm = (1d / double.Parse(splitText[1]) * 1000d * 60d).Floor();
                                    bool uninherited = splitText[6] == "0";
                                    bool kiai = splitText[7] == "1";

                                    if (timingPointsSectionStartLine)
                                    {
                                        osuManiaMap.info.songOffset = time;
                                        osuManiaMap.info.videoOffset -= time;

                                        timingPointsSectionBPM = bpm;
                                        timingPointsSectionLastTime = time;

                                        timingPointsSectionStartLine = false;
                                    }

                                    double beat = RhythmManager.SecondToBeat(time - timingPointsSectionLastTime, timingPointsSectionBPM) + timingPointsSectionLastBeat;
                                    if (!uninherited)
                                    {
                                        osuManiaMap.globalEffect.bpm.Add(beat, bpm);
                                        timingPointsSectionBPM = bpm;
                                    }

                                    osuManiaMap.globalEffect.yukiMode.Add(beat, kiai, false);

                                    timingPointsSectionLastBeat = beat;
                                    timingPointsSectionLastTime = time;
                                    isTimingPointsCalculated = true;

                                    break;
                                }
                                #endregion

                                #region Hit Objects Section
                                case "[HitObjects]":
                                {
                                    if (!isDifficultyCalculated)
                                        throw new NotSupportedException("Notes cannot be processed while Difficulty are not calculated!");
                                    else if (!isTimingPointsCalculated)
                                        throw new NotSupportedException("Notes cannot be processed while Timing Points are not calculated!");

                                    string[] splitText = text.QuotedSplit(",");

                                    int index = (int.Parse(splitText[0]) * keyCount / 512d).FloorToInt();
                                    double time = int.Parse(splitText[2]) * 0.001;
                                    double holdTime = int.Parse(splitText[5].Split(':')[0]) * 0.001;

                                    BeatValuePairList<double> bpmList = osuManiaMap.globalEffect.bpm;

                                    double bpm = RhythmManager.TimeUseBPMChangeCalulate(bpmList, time, osuManiaMap.info.songOffset, out double hitObjectsSectionBPMOffsetBeat, out double hitObjectsSectionBPMOffsetTime);
                                    double beat = RhythmManager.SecondToBeat(time - hitObjectsSectionBPMOffsetTime - osuManiaMap.info.songOffset, bpm) + hitObjectsSectionBPMOffsetBeat;

                                    double holdBeat = 0;
                                    if (time < holdTime)
                                    {
                                        double holdBPM = RhythmManager.TimeUseBPMChangeCalulate(bpmList, time, osuManiaMap.info.songOffset, out double hitObjectsSectionBPMOffsetBeatHold, out double hitObjectsSectionBPMOffsetTimeHold);
                                        holdBeat = RhythmManager.SecondToBeat(holdTime - hitObjectsSectionBPMOffsetTimeHold - osuManiaMap.info.songOffset, holdBPM) + hitObjectsSectionBPMOffsetBeatHold - beat;
                                    }

                                    osuManiaMap.notes[index].Add(new OsuManiaNoteFile(beat, holdBeat));
                                    break;
                                }
                                #endregion
                            }
                        }
                    }
                    catch
                    {
                        Debug.LogError("[Text] " + text);
                        Debug.LogError("[Path] " + mapFilePath);

                        throw;
                    }
                }

                //어떤 이유에선지 몰라도 오디오가 ogg 인 상태인데도 불구하고 0.04초 정도 음악이 느린 모습을 보임
                osuManiaMap.info.songOffset += 0.04;
                FixAllJudgmentBeat(osuManiaMap);

                if (osuManiaMap.allJudgmentBeat.Count > 0)
                    osuManiaMap.info.clearBeat = osuManiaMap.allJudgmentBeat.Last() + 4;

                osuManiaMap.Init(mapFilePath);
            }

            return osuManiaMap;
        }

        static void FixAllJudgmentBeat(OsuManiaMapFile map)
        {
            map.allJudgmentBeat.Clear();

            for (int i = 0; i < map.notes.Count; i++)
            {
                TypeList<OsuManiaNoteFile> notes = map.notes[i];

                for (int j = 0; j < notes.Count; j++)
                {
                    OsuManiaNoteFile note = notes[j];

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
