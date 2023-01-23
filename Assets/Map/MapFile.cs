using Newtonsoft.Json;
using SCKRM.Json;
using SCKRM.Rhythm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

using Version = SCKRM.Version;

namespace SDJK.Map
{
    public sealed class MapPack
    {
        public List<MapFile> maps { get; } = new List<MapFile>();
    }

    public class MapFile
    {
        public MapInfo info { get; set; } = new MapInfo();
        public MapGlobalEffect globalEffect { get; set; } = new MapGlobalEffect();
        public MapVisualizerEffect visualizerEffect { get; set; } = new MapVisualizerEffect();

        [JsonIgnore] public List<double> allJudgmentBeat { get; set; } = new List<double>();

        [JsonIgnore] public string mapFilePathParent { get; set; } = "";
        [JsonIgnore] public string mapFilePath { get; set; } = "";

        public virtual void SetVisualizerEffect() { }
    }

    public sealed class MapInfo
    {
        [JsonIgnore] public string id { get; private set; }
        [JsonIgnore] public int randomSeed => id.GetHashCode();



        public Version sckrmVersion { get; set; } = new Version();
        public Version sdjkVersion { get; set; } = new Version();



        public string ruleset { get; set; } = "";



        public string songFile { get; set; } = "";




        public string videoBackgroundFile { get; set; } = "";
        public string videoBackgroundNightFile { get; set; } = "";

        public double videoOffset { get; set; } = 0;



        public string artist { get; set; } = "";
        public string songName { get; set; } = "";

        public string difficultyLabel { get; set; } = "";

        public string original { get; set; } = "";
        public string[] tag { get; set; } = new string[0];



        public double songOffset { get; set; } = 0;
        public double mainMenuStartTime { get; set; } = 0;



        public string author { get; set; } = "";



        public void ResetMapID(string mapFilePath)
        {
            using SHA256 sha256 = SHA256.Create();
            using FileStream stream = File.OpenRead(mapFilePath);

            id = BitConverter.ToString(sha256.ComputeHash(stream));
        }
    }

    public sealed class MapGlobalEffect
    {
        public BeatValuePairList<double> bpm { get; } = new(100);
        public BeatValuePairList<bool> yukiMode { get; } = new(false);



        public BeatValuePairList<BackgroundEffectPair> background { get; } = new(new BackgroundEffectPair("", ""));
        public BeatValuePairAniListColor backgroundColor { get; } = new(JColor.one);

        public BeatValuePairAniListColor videoColor { get; } = new(JColor.one);



        public BeatValuePairAniListDouble cameraZoom { get; } = new(1);
        public BeatValuePairAniListVector3 cameraPos { get; } = new(default);
        public BeatValuePairAniListVector3 cameraRotation { get; } = new(default);

        public BeatValuePairAniListColor backgroundFlash { get; } = new(default);
        public BeatValuePairAniListColor fieldFlash { get; } = new(default);
        public BeatValuePairAniListColor uiFlash { get; } = new(default);



        public BeatValuePairAniListDouble uiSize { get; } = new(1);



        public BeatValuePairAniListDouble pitch { get; } = new(1);
        public BeatValuePairAniListDouble tempo { get; } = new(1);

        public BeatValuePairAniListDouble volume { get; } = new(1);



        public BeatValuePairAniListDouble hpAddValue { get; } = new(2);
        public BeatValuePairAniListDouble hpMissValue { get; } = new(10);
        public BeatValuePairAniListDouble hpRemoveValue { get; } = new(2);



        public BeatValuePairAniListDouble judgmentSize { get; } = new(1);
    }

    public sealed class MapVisualizerEffect
    {
        public BeatValuePairAniListInt divide { get; } = new(5);
        public BeatValuePairList<bool> leftMove { get; } = new(true);

        public BeatValuePairAniListInt offset { get; } = new(0);
        public BeatValuePairAniListFloat size { get; } = new(1);

        public BeatValuePairAniListFloat moveDelay { get; } = new(0);
    }



    #region Effect Class And Struct
    public struct BackgroundEffectPair
    {
        public string backgroundFile { get; set; }
        public string backgroundNightFile { get; set; }

        public BackgroundEffectPair(string backgroundFile, string backgroundNightFile)
        {
            this.backgroundFile = backgroundFile;
            this.backgroundNightFile = backgroundNightFile;
        }
    }

    [SerializeField]
    public enum FlashOrder
    {
        background,
        field,
        ui
    }
    #endregion
}