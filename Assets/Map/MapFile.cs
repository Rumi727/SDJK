using Newtonsoft.Json;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SCKRM.NBS;
using SCKRM.Rhythm;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Map
{
    public sealed class MapPack
    {
        public List<MapFile> maps { get; } = new List<MapFile>();
    }

    public class MapFile
    {
        public MapInfo info { get; } = new MapInfo();
        public MapGlobalEffect globalEffect { get; } = new MapGlobalEffect();

        public List<double> allJudgmentBeat { get; } = new List<double>();

        [JsonIgnore] public string mapFilePathParent { get; set; } = "";
        [JsonIgnore] public string mapFilePath { get; set; } = "";
    }

    public sealed class MapInfo
    {
        [System.Obsolete("Not implemented!", true), JsonIgnore] public ulong id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }



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



    #region Effect
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