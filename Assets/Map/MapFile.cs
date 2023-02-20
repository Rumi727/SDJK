using Newtonsoft.Json;
using SCKRM;
using SCKRM.Json;
using SCKRM.Rhythm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

using Version = SCKRM.Version;

namespace SDJK.Map
{
    public sealed class MapPack
    {
        public List<MapFile> maps { get; set; } = new List<MapFile>();
    }

    public class MapFile
    {
        public MapInfo info { get; set; } = new MapInfo();
        public MapGlobalEffect globalEffect { get; set; } = new MapGlobalEffect();
        public MapVisualizerEffect visualizerEffect { get; set; } = new MapVisualizerEffect();

        [JsonIgnore] public List<double> allJudgmentBeat { get; set; } = new List<double>();

        [JsonIgnore] public string mapFilePathParent { get; set; } = null;
        [JsonIgnore] public string mapFilePath { get; set; } = null;

        [JsonIgnore] public bool isInit { get; private set; } = false;

        public virtual void Init(string mapFilePath)
        {
            mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
            this.mapFilePath = mapFilePath;

            info.sckrmVersion = Kernel.sckrmVersion;
            info.sdjkVersion = (Version)Kernel.version;

            info.ResetMapID(mapFilePath);
            SetVisualizerEffect();

            isInit = true;
        }

        public virtual void SetVisualizerEffect() { }
    }

    public sealed class MapInfo
    {
        [JsonIgnore] bool idIsReseted = false;
        public string id
        {
            get
            {
                if (!idIsReseted)
                    throw new Exception(nameof(id) + " not reseted!");

                return _id;
            }
        }
        [JsonIgnore] private string _id;
        [JsonIgnore] public int randomSeed
        {
            get
            {
                if (!idIsReseted)
                    throw new Exception(nameof(id) + " not reseted!");

                return _randomSeed;
            }
        }
        private int _randomSeed;



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



        public double clearBeat { get; set; } = double.MaxValue;



        public void ResetMapID(string mapFilePath)
        {
            using SHA256 sha256 = SHA256.Create();
            using FileStream stream = File.OpenRead(mapFilePath);

            _id = BitConverter.ToString(sha256.ComputeHash(stream));
            _randomSeed = BitConverter.ToInt32(Encoding.UTF8.GetBytes(_id));

            idIsReseted = true;
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

    public sealed class MapVisualizerEffect
    {
        public BeatValuePairAniListInt divide { get; set; } = new(5);
        public BeatValuePairList<bool> leftMove { get; set; } = new(true);

        public BeatValuePairAniListInt offset { get; set; } = new(0);
        public BeatValuePairAniListFloat size { get; set; } = new(12);

        public BeatValuePairAniListFloat moveDelay { get; set; } = new(0.001f);
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