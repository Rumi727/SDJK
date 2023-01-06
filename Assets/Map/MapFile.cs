using Newtonsoft.Json;
using SCKRM.Json;
using SCKRM.Rhythm;
using System;
using System.Collections.Generic;
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

        [JsonIgnore] public List<double> allJudgmentBeat { get; set; } = new List<double>();

        [JsonIgnore] public string mapFilePathParent { get; set; } = "";
        [JsonIgnore] public string mapFilePath { get; set; } = "";
    }

    public sealed class MapInfo
    {
        [Obsolete("Not implemented!", true), JsonIgnore] public Guid id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /*[JsonIgnore]*/ public int randomSeed { get; set; } //=> id.ToUInt64();



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



        public void ResetRandomSeed(string mapFilePath)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            using System.IO.FileStream stream = System.IO.File.OpenRead(mapFilePath);

            randomSeed = BitConverter.ToString(md5.ComputeHash(stream)).GetHashCode();
        }
    }

    public sealed class MapGlobalEffect
    {
        public BeatValuePairList<double> bpm { get; set; } = new(100);
        public BeatValuePairList<bool> yukiMode { get; set; } = new(false);



        public BeatValuePairList<BackgroundEffectPair> background { get; set; } = new(new BackgroundEffectPair("", ""));
        public BeatValuePairAniListColor backgroundColor { get; set; } = new(JColor.one);

        public BeatValuePairAniListColor videoColor { get; set; } = new(JColor.one);



        public BeatValuePairAniListDouble cameraZoom { get; set; } = new(1);
        public BeatValuePairAniListVector3 cameraPos { get; set; } = new(default);
        public BeatValuePairAniListVector3 cameraRotation { get; set; } = new(default);

        public BeatValuePairAniListColor backgroundFlash { get; set; } = new(default);
        public BeatValuePairAniListColor fieldFlash { get; set; } = new(default);
        public BeatValuePairAniListColor uiFlash { get; set; } = new(default);



        public BeatValuePairAniListDouble uiSize { get; set; } = new(1);



        public BeatValuePairAniListDouble pitch { get; set; } = new(1);
        public BeatValuePairAniListDouble tempo { get; set; } = new(1);

        public BeatValuePairAniListDouble volume { get; set; } = new(1);



        public BeatValuePairAniListDouble hpAddValue { get; set; } = new(2);
        public BeatValuePairAniListDouble hpMissValue { get; set; } = new(10);
        public BeatValuePairAniListDouble hpRemoveValue { get; set; } = new(2);



        public BeatValuePairAniListDouble judgmentSize { get; set; } = new(1);
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