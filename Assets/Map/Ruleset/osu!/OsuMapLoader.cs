using SCKRM;
using SCKRM.Rhythm;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

                StringBuilder splitStringBuilder = new StringBuilder();
                List<string> splitTexts = new List<string>();

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
                            splitTexts.Clear();
                            splitStringBuilder.Clear();

                            for (int i = 0; i < text.Length; i++)
                            {
                                char currentChar = text[i];
                                if (currentChar != ':')
                                {
                                    splitStringBuilder.Append(currentChar);
                                    continue;
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();
                            }

                            splitTexts.Add(splitStringBuilder.ToString());
                            splitStringBuilder.Clear();

                            string key = splitTexts[0].Trim();
                            string value = splitTexts[1].Trim();

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
                            splitTexts.Clear();
                            splitStringBuilder.Clear();

                            for (int i = 0; i < text.Length; i++)
                            {
                                char currentChar = text[i];
                                if (currentChar != ':')
                                {
                                    splitStringBuilder.Append(currentChar);
                                    continue;
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();
                            }

                            splitTexts.Add(splitStringBuilder.ToString());
                            splitStringBuilder.Clear();

                            string key = splitTexts[0].Trim();
                            if (splitTexts.Count < 2)
                                continue;

                            string value = splitTexts[1].Trim();

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
                                splitTexts.Clear();
                                splitStringBuilder.Clear();

                                for (int i = 0; i < text.Length; i++)
                                {
                                    char currentChar = text[i];
                                    if (currentChar != ':')
                                    {
                                        splitStringBuilder.Append(currentChar);
                                        continue;
                                    }

                                    splitTexts.Add(splitStringBuilder.ToString());
                                    splitStringBuilder.Clear();
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();

                                string key = splitTexts[0].Trim();
                                string value = splitTexts[1].Trim();

                                if (key == "CircleSize")
                                {
                                    keyCount = int.Parse(value);

                                    OsuManiaMapFile osuManiaMap = (OsuManiaMapFile)osuMap;
                                    osuManiaMap.notes.Clear();

                                    for (int i = 0; i < keyCount; i++)
                                        osuManiaMap.notes.Add(new TypeList<OsuNoteFile>());
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
                            splitTexts.Clear();
                            splitStringBuilder.Clear();

                            for (int i = 0; i < text.Length; i++)
                            {
                                char currentChar = text[i];
                                if (currentChar != ',')
                                {
                                    splitStringBuilder.Append(currentChar);
                                    continue;
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();
                            }

                            splitTexts.Add(splitStringBuilder.ToString());
                            splitStringBuilder.Clear();

                            string eventType = splitTexts[0];

                            switch (eventType)
                            {
                                case "0":
                                {
                                    if (!eventsSectionBackgroundIgnore)
                                    {
                                        osuMap.globalEffect.background.Add(new BackgroundEffectPair(PathUtility.GetPathWithExtension(splitTexts[2].Trim('"')), ""));
                                        eventsSectionBackgroundIgnore = true;
                                    }

                                    break;
                                }
                                case "1":
                                case "Video":
                                {
                                    if (!eventsSectionVideoIgnore)
                                    {
                                        osuMap.info.videoBackgroundFile = PathUtility.GetPathWithExtension(splitTexts[2].Trim('"'));
                                        osuMap.info.videoOffset = double.Parse(splitTexts[1]);

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
                            splitTexts.Clear();
                            splitStringBuilder.Clear();

                            for (int i = 0; i < text.Length; i++)
                            {
                                char currentChar = text[i];
                                if (currentChar != ',')
                                {
                                    splitStringBuilder.Append(currentChar);
                                    continue;
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();
                            }

                            splitTexts.Add(splitStringBuilder.ToString());
                            splitStringBuilder.Clear();

                            double time = int.Parse(splitTexts[0]) * 0.001;
                            double bpm = (1d / double.Parse(splitTexts[1]) * 1000d * 60d).Floor();
                            int sampleSet = int.Parse(splitTexts[3]);
                            int sampleIndex = int.Parse(splitTexts[4]);
                            int volume = int.Parse(splitTexts[5]);
                            bool uninherited = splitTexts[6] == "0";
                            bool kiai = splitTexts[7] == "1";

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
                    List<string> splitHitsoundTexts = new List<string>();

                    while (true)
                    {
                        string text = stringReader.ReadLine();
                        if (text == null)
                            break;

                        try
                        {
                            splitTexts.Clear();
                            splitStringBuilder.Clear();

                            for (int i = 0; i < text.Length; i++)
                            {
                                char currentChar = text[i];
                                if (currentChar != ',' && currentChar != ':' && currentChar != '|')
                                {
                                    splitStringBuilder.Append(currentChar);
                                    continue;
                                }

                                splitTexts.Add(splitStringBuilder.ToString());
                                splitStringBuilder.Clear();
                            }

                            splitTexts.Add(splitStringBuilder.ToString());
                            splitStringBuilder.Clear();

                            double time = int.Parse(splitTexts[2]) * 0.001;

                            BeatValuePairList<double> bpmList = osuMap.globalEffect.bpm;
                            double beat = GetBeat(time);
                            double holdBeat = 0;

                            //Hitsound
                            TypeList<HitsoundFile> hitsoundFiles = HitsoundFile.defaultHitsounds;
                            TypeList<HitsoundFile> holdHitsoundFiles = HitsoundFile.defaultHitsounds;

                            #region Hitsound Loader
                            {
                                {
                                    int hitsoundColonCount = 0;
                                    for (int i = text.Length - 1; i >= 0; i--)
                                    {
                                        char currentChar = text[i];
                                        if (currentChar == ',' || currentChar == '|')
                                            break;
                                        else if (currentChar != ':')
                                        {
                                            splitStringBuilder.Append(new char[] { currentChar }, 0, 1);
                                            continue;
                                        }

                                        hitsoundColonCount++;
                                        if (hitsoundColonCount >= 5)
                                            break;
                                        
                                        splitHitsoundTexts.Insert(0, splitStringBuilder.ToString());
                                        splitStringBuilder.Clear();
                                    }

                                    splitHitsoundTexts.Insert(0, splitStringBuilder.ToString());
                                    splitStringBuilder.Clear();
                                }

                                if (splitHitsoundTexts.Count == 5)
                                {
                                    int hitsound = int.Parse(splitTexts[4]);
                                    int sampleSet = timingPointsSampleSet.GetValue(beat);
                                    int sampleIndex = timingPointsSampleIndex.GetValue(beat);
                                    int sampleVolume = timingPointsVolume.GetValue(beat);
                                    string fileName;

                                    if (hitsound == 0)
                                    {
                                        int value = int.Parse(splitHitsoundTexts[0]);
                                        if (value != 0)
                                            sampleSet = value - 1;
                                    }
                                    else
                                    {
                                        int value = int.Parse(splitHitsoundTexts[0]);
                                        if (value != 0)
                                            sampleSet = value - 1;
                                    }

                                    {
                                        int value = int.Parse(splitHitsoundTexts[1]);
                                        if (value != 0)
                                            sampleIndex = value;
                                    }

                                    {
                                        int value = int.Parse(splitHitsoundTexts[2]);
                                        if (value != 0)
                                            sampleVolume = value;
                                    }

                                    fileName = splitHitsoundTexts[3];

                                    {
                                        string sampleSetText = sampleSet switch
                                        {
                                            1 => "soft",
                                            2 => "drum",
                                            _ => "normal",
                                        };

                                        string hitsoundText = hitsound switch
                                        {
                                            1 or 4 => "finish",
                                            2 => "whistle",
                                            3 or 8 => "clap",
                                            _ => "normal",
                                        };

                                        HitsoundFile hitsoundFile;
                                        HitsoundFile defaultHitsound = HitsoundFile.defaultHitsound;
                                        defaultHitsound.volume = sampleVolume;

                                        if (sampleIndex == 0 || sampleIndex == 1)
                                            hitsoundFile = new HitsoundFile(sampleSetText + "-hit" + hitsoundText, sampleVolume * 0.01f, 1);
                                        else
                                            hitsoundFile = new HitsoundFile(sampleSetText + "-hit" + hitsoundText + sampleIndex, sampleVolume * 0.01f, 1);

                                        HitsoundFile customHitsoundFile = new HitsoundFile(PathUtility.GetPathWithExtension(fileName), sampleVolume * 0.01f, 1);

                                        hitsoundFiles = new TypeList<HitsoundFile>() { hitsoundFile, customHitsoundFile };
                                        holdHitsoundFiles = new TypeList<HitsoundFile>();
                                    }
                                }
                            }
                            #endregion


                            //Mania Note Loader
                            if (isOsuMania)
                            {
                                OsuManiaMapFile osuManiaMap = (OsuManiaMapFile)osuMap;

                                double holdTime = 0;
                                bool isHold = splitTexts.Count == 11;
                                if (isHold)
                                    holdTime = int.Parse(splitTexts[5]) * 0.001;

                                int index = (int.Parse(splitTexts[0]) * keyCount / 512d).FloorToInt();

                                if (time < holdTime)
                                    holdBeat = GetBeat(holdTime) - beat;

                                osuManiaMap.notes[index].Add(new OsuNoteFile(beat, holdBeat, hitsoundFiles, holdHitsoundFiles));
                            }

                            osuMap.beats.Add(new OsuNoteFile(beat, holdBeat, hitsoundFiles, holdHitsoundFiles));

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

                osuMap.globalEffect.volume.Add(double.MinValue, 0, 0.35f);

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
