using SCKRM;
using SCKRM.Rhythm;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SDJK.Map.Ruleset.Osu.OsuMapLoader;

namespace SDJK.Map.Ruleset.Osu
{
    public static class OsuMapLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("osu");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension, IMode[] modes) =>
            {
                bool typeIsOsuManiaMap = type == typeof(OsuMapFile);
                if (typeIsOsuManiaMap && !File.Exists(mapFilePath))
                    return new OsuMapFile("");

                if (extension == ".osu" && (type == typeof(MapFile) || typeIsOsuManiaMap))
                    return MapLoad(mapFilePath);
                else
                    return null;
            };
        }

        public static OsuMapFile MapLoad(string mapFilePath)
        {
            OsuMapFile osuMap = new OsuMapFile(mapFilePath);

            if (File.Exists(mapFilePath))
            {
                bool startLine = true;
                string section = "";

                bool eventsSectionBackgroundIgnore = false;
                bool eventsSectionVideoIgnore = false;

                bool timingPointsSectionStartLine = true;
                double timingPointsSectionLastTime = 0;
                double timingPointsSectionLastBeat = 0;
                double timingPointsSectionBPM = 60;

                int keyCount = 4;
                bool isOsuMania = false;

                int beatmapSampleSet = 0;

                BeatValuePairList<int> timingPointsSampleSet = new BeatValuePairList<int>(0);
                BeatValuePairList<int> timingPointsSampleIndex = new BeatValuePairList<int>(0);
                BeatValuePairList<int> timingPointsVolume = new BeatValuePairList<int>(100);

                Dictionary<string, StringBuilder> sections = new Dictionary<string, StringBuilder>();

                #region Stream Reader
                using (StreamReader streamReader = File.OpenText(mapFilePath))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string text = streamReader.ReadLine();

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
                            else if (text.StartsWith("//"))
                                continue;

                            if (sections.ContainsKey(section))
                                sections[section].AppendLine(text);
                            else
                                sections.Add(section, new StringBuilder().AppendLine(text));
                        }
                    }
                }
                #endregion

                #region General Section
                using (StringReader stringReader = new StringReader(sections["[General]"].ToString()))
                {
                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            string[] splitText = text.Split(":");
                            string key = splitText[0].Trim();
                            string value = splitText[1].Trim();

                            switch (key)
                            {
                                case "AudioFilename":
                                    osuMap.info.songFile = PathUtility.GetPathWithExtension(value);
                                    break;
                                case "PreviewTime":
                                    osuMap.info.mainMenuStartTime = int.Parse(value) * 0.001f;
                                    break;
                                case "SampleSet":
                                {
                                    if (value == "Soft")
                                        beatmapSampleSet = 1;
                                    else if (value == "Drum")
                                        beatmapSampleSet = 2;
                                    else
                                        beatmapSampleSet = 0;

                                    break;
                                }
                                case "Mode":
                                {
                                    if (value == "0")
                                        osuMap.info.ruleset = "osu!";
                                    else if (value == "3")
                                    {
                                        OsuManiaMapFile osuManiaMap = new OsuManiaMapFile(mapFilePath);

                                        osuManiaMap.info = osuMap.info;
                                        osuManiaMap.globalEffect = osuMap.globalEffect;
                                        osuManiaMap.postProcessEffect = osuMap.postProcessEffect;

                                        osuMap = osuManiaMap;
                                        osuMap.info.ruleset = "osu!mania";

                                        isOsuMania = true;
                                    }
                                    else
                                        throw new NotSupportedException("Only supports osu! and osu!mania beatmap");

                                    break;
                                }
                            }
                        }
                        catch
                        {
                            Debug.ForceLogError("[Text] " + text);
                            throw;
                        }
                    }
                }
                #endregion

                #region MetaData Section
                using (StringReader stringReader = new StringReader(sections["[Metadata]"].ToString()))
                {
                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            string[] splitText = text.QuotedSplit(":");
                            string key = splitText[0].Trim();
                            if (splitText.Length < 2)
                                continue;

                            string value = splitText[1].Trim();

                            switch (key)
                            {
                                case "Title":
                                    osuMap.info.songName = value;
                                    break;
                                case "Artist":
                                    osuMap.info.artist = value;
                                    break;
                                case "Creator":
                                    osuMap.info.author = value;
                                    break;
                                case "Version":
                                {
                                    if (isOsuMania)
                                        osuMap.info.difficultyLabel = value + " (osu!mania)";
                                    else
                                        osuMap.info.difficultyLabel = value;

                                    break;
                                }
                                case "Source":
                                    osuMap.info.original = value;
                                    break;
                                case "Tags":
                                    osuMap.info.tag = value.QuotedSplit(" ");
                                    break;
                            }
                        }
                        catch
                        {
                            Debug.ForceLogError("[Text] " + text);
                            throw;
                        }
                    }
                }
                #endregion

                #region Difficulty Section
                if (isOsuMania)
                {
                    using (StringReader stringReader = new StringReader(sections["[Difficulty]"].ToString()))
                    {
                        while (true)
                        {
                            string text = stringReader.ReadLine();
                            if (text == null)
                                break;

                            try
                            {
                                string[] splitText = text.Split(':');
                                string key = splitText[0].Trim();
                                string value = splitText[1].Trim();

                                if (key == "CircleSize")
                                {
                                    keyCount = int.Parse(value);
                                    
                                    OsuManiaMapFile osuMania = (OsuManiaMapFile)osuMap;
                                    osuMania.notes.Clear();

                                    for (int i = 0; i < keyCount; i++)
                                        osuMania.notes.Add(new TypeList<OsuManiaNoteFile>());
                                }
                            }
                            catch
                            {
                                Debug.ForceLogError("[Text] " + text);
                                throw;
                            }
                        }
                    }
                }
                #endregion

                #region Events Section
                using (StringReader stringReader = new StringReader(sections["[Events]"].ToString()))
                {
                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            string[] splitText = text.Split(',');
                            string eventType = splitText[0];

                            switch (eventType)
                            {
                                case "0":
                                {
                                    if (!eventsSectionBackgroundIgnore)
                                    {
                                        osuMap.globalEffect.background.Add(new BackgroundEffectPair(PathUtility.GetPathWithExtension(splitText[2].Trim('"')), ""));
                                        eventsSectionBackgroundIgnore = true;
                                    }

                                    break;
                                }
                                case "1":
                                case "Video":
                                {
                                    if (!eventsSectionVideoIgnore)
                                    {
                                        osuMap.info.videoBackgroundFile = PathUtility.GetPathWithExtension(splitText[2].Trim('"'));
                                        osuMap.info.videoOffset = double.Parse(splitText[1]);

                                        eventsSectionVideoIgnore = true;
                                    }

                                    break;
                                }
                            }
                        }
                        catch
                        {
                            Debug.ForceLogError("[Text] " + text);
                            throw;
                        }
                    }
                }
                #endregion

                #region Timing Points Section
                using (StringReader stringReader = new StringReader(sections["[TimingPoints]"].ToString()))
                {
                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            string[] splitText = text.Split(',');
                            double time = int.Parse(splitText[0]) * 0.001;
                            double bpm = (1d / double.Parse(splitText[1]) * 1000d * 60d).Floor();
                            int sampleSet = int.Parse(splitText[3]);
                            int sampleIndex = int.Parse(splitText[4]);
                            int volume = int.Parse(splitText[5]);
                            bool uninherited = splitText[6] == "0";
                            bool kiai = splitText[7] == "1";

                            if (timingPointsSectionStartLine)
                            {
                                osuMap.info.songOffset = time;

                                timingPointsSectionBPM = bpm;
                                timingPointsSectionLastTime = time;

                                timingPointsSectionStartLine = false;
                            }

                            double beat = RhythmManager.SecondToBeat(time - timingPointsSectionLastTime, timingPointsSectionBPM) + timingPointsSectionLastBeat;
                            if (!uninherited)
                            {
                                osuMap.globalEffect.bpm.Add(beat, bpm);
                                timingPointsSectionBPM = bpm;
                            }

                            if (sampleSet == 0)
                                timingPointsSampleSet.Add(beat, beatmapSampleSet);
                            else
                                timingPointsSampleSet.Add(beat, sampleSet - 1);

                            timingPointsSampleIndex.Add(beat, sampleIndex);
                            timingPointsVolume.Add(beat, volume);

                            osuMap.globalEffect.yukiMode.Add(beat, kiai, false);

                            timingPointsSectionLastBeat = beat;
                            timingPointsSectionLastTime = time;
                        }
                        catch
                        {
                            Debug.ForceLogError("[Text] " + text);
                            throw;
                        }
                    }
                }
                #endregion

                #region Hit Objects Section
                using (StringReader stringReader = new StringReader(sections["[HitObjects]"].ToString()))
                {
                    //HitsoundFile? lastHitsoundFile = null;
                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            string[] splitText = text.Split(',', ':', '|');
                            double time = int.Parse(splitText[2]) * 0.001;

                            BeatValuePairList<double> bpmList = osuMap.globalEffect.bpm;
                            double beat = GetBeat(time);

                            if (isOsuMania)
                            {
                                double holdTime = 0;
                                bool isHold = splitText.Length == 11;
                                if (isHold)
                                    holdTime = int.Parse(splitText[5]) * 0.001;

                                int index = (int.Parse(splitText[0]) * keyCount / 512d).FloorToInt();

                                double holdBeat = 0;
                                if (time < holdTime)
                                    holdBeat = GetBeat(holdTime) - beat;

                                ((OsuManiaMapFile)osuMap).notes[index].Add(new OsuManiaNoteFile(beat, holdBeat));

                                //Hitsound
                                //너무 이상해서 폐기
                                //오스 맵 구조 진짜 개같이 짜여져 있네
                                /*
                                int hitsound = int.Parse(splitText[4]).Clamp(0, 3);
                                int sampleSet = timingPointsSampleSet.GetValue(beat);
                                int sampleIndex = timingPointsSampleIndex.GetValue(beat);
                                int sampleVolume = timingPointsVolume.GetValue(beat);
                                string fileName;

                                if (hitsound == 0)
                                {
                                    int value = int.Parse(splitText[splitText.Length - 5]);
                                    if (value != 0)
                                        sampleSet = value - 1;
                                }
                                else if (hitsound != 0)
                                {
                                    int value = int.Parse(splitText[splitText.Length - 4]);
                                    if (value != 0)
                                        sampleSet = value - 1;
                                }

                                {
                                    int value = int.Parse(splitText[splitText.Length - 3]);
                                    if (value != 0)
                                        sampleIndex = value;
                                }

                                {
                                    int value = int.Parse(splitText[splitText.Length - 2]);
                                    if (value != 0)
                                        sampleVolume = value;
                                }

                                fileName = splitText[splitText.Length - 1];

                                {
                                    string sampleSetText = "normal";
                                    if (sampleSet == 1)
                                        sampleSetText = "soft";
                                    else if (sampleSet == 2)
                                        sampleSetText = "drum";

                                    string hitsoundText = "normal";
                                    if (hitsound == 1)
                                        hitsoundText = "whistle";
                                    else if (hitsound == 2)
                                        hitsoundText = "finish";
                                    else if (hitsound == 3)
                                        hitsoundText = "clap";

                                    HitsoundFile hitsoundFile;
                                    if (sampleIndex == 0 || sampleIndex == 1)
                                        hitsoundFile = new HitsoundFile($"{sampleSetText}-hit{hitsoundText}", sampleVolume * 0.01f, 1);
                                    else
                                        hitsoundFile = new HitsoundFile($"{sampleSetText}-hit{hitsoundText}{sampleIndex}", sampleVolume * 0.01f, 1);

                                    HitsoundFile customHitsoundFile = new HitsoundFile(PathUtility.GetPathWithExtension(fileName), sampleVolume * 0.01f, 1);
                                    if (!lastHitsoundFile.Equals(hitsoundFile))
                                    {
                                        bool isCustomHitsound = File.Exists(PathUtility.Combine(osuMap.mapFilePathParent, customHitsoundFile.path + ".wav"));

                                        if (File.Exists(PathUtility.Combine(osuMap.mapFilePathParent, hitsoundFile.path + ".wav")))
                                        {
                                            if (isCustomHitsound)
                                                osuMap.globalEffect.hitsoundFile.Add(beat, new HitsoundFile[] { hitsoundFile, customHitsoundFile });
                                            else
                                                osuMap.globalEffect.hitsoundFile.Add(beat, new HitsoundFile[] { hitsoundFile });
                                        }
                                        else
                                        {
                                            if (isCustomHitsound)
                                                osuMap.globalEffect.hitsoundFile.Add(beat, new HitsoundFile[] { HitsoundFile.defaultHitsound, customHitsoundFile });
                                            else
                                                osuMap.globalEffect.hitsoundFile.Add(beat);
                                        }

                                        lastHitsoundFile = hitsoundFile;
                                    }
                                }*/
                            }

                            osuMap.beats.Add(beat);

                            double GetBeat(double time)
                            {
                                double bpm = RhythmManager.TimeUseBPMChangeCalulate(bpmList, time, osuMap.info.songOffset, out double hitObjectsSectionBPMOffsetBeat, out double hitObjectsSectionBPMOffsetTime);
                                return RhythmManager.SecondToBeat(time - hitObjectsSectionBPMOffsetTime - osuMap.info.songOffset, bpm) + hitObjectsSectionBPMOffsetBeat;
                            }
                        }
                        catch
                        {
                            Debug.ForceLogError("[Text] " + text);
                            throw;
                        }
                    }
                }

                #endregion

                osuMap.globalEffect.volume.Add(double.MinValue, 0, 0.5f);

                //어떤 이유에선지 몰라도 오디오가 ogg 인 상태인데도 불구하고 0.04초 정도 음악이 느린 모습을 보임
                if (isOsuMania)
                    osuMap.info.songOffset += 0.02;
                else
                    osuMap.info.songOffset += 0.06;

                if (osuMap.allJudgmentBeat.Count > 0)
                    osuMap.info.clearBeat = osuMap.allJudgmentBeat.Last() + 4;

                osuMap.Init(mapFilePath);
            }

            return osuMap;
        }
    }
}
