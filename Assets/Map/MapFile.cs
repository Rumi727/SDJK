using Newtonsoft.Json;
using SCKRM;
using SCKRM.Json;
using SCKRM.Rhythm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

using Version = SCKRM.Version;

namespace SDJK.Map
{
    public sealed class MapPack
    {
        public TypeList<MapFile> maps { get; set; } = new TypeList<MapFile>();
    }

    public abstract class MapFile
    {
        public MapFile(string mapFilePath) => Init(mapFilePath);

        public void Init(string mapFilePath)
        {
            if (File.Exists(mapFilePath))
            {
                mapFilePathParent = Directory.GetParent(mapFilePath).ToString();
                this.mapFilePath = mapFilePath;
            }
            else
            {
                mapFilePathParent = "";
                this.mapFilePath = "";
            }

            info.sckrmVersion = Kernel.sckrmVersion;
            info.sdjkVersion = (Version)Kernel.version;

            info.ResetMapID(mapFilePath);
            SetVisualizerEffect();
        }

        public MapInfo info { get; set; } = new MapInfo();
        public MapGlobalEffect globalEffect { get; set; } = new MapGlobalEffect();
        public MapVisualizerEffect visualizerEffect { get; set; } = new MapVisualizerEffect();
        public MapPostProcessEffect postProcessEffect { get; set; } = new MapPostProcessEffect();

        [JsonIgnore] public TypeList<double> allJudgmentBeat
        {
            get
            {
                if (_allJudgmentBeat == null)
                    FixAllJudgmentBeat();

                return _allJudgmentBeat;
            }
            protected set => _allJudgmentBeat = value;
        }
        [JsonIgnore] TypeList<double> _allJudgmentBeat = null;

        [JsonIgnore] public TypeList<double> difficulty => _difficulty ??= GetDifficulty();
        [JsonIgnore] TypeList<double> _difficulty = null;

        [JsonIgnore] public double difficultyAverage => _difficultyAverage ??= difficulty.Average();
        [JsonIgnore] double? _difficultyAverage = null;

        [JsonIgnore] public string mapFilePathParent { get; set; } = null;
        [JsonIgnore] public string mapFilePath { get; set; } = null;

        public virtual void SetVisualizerEffect() { }

        /// <summary>
        /// 0 ~ 10 ~
        /// </summary>
        public virtual TypeList<double> GetDifficulty() => DifficultyCalculation(allJudgmentBeat);

        public TypeList<double> DifficultyCalculation(IList<double> beatList, double size = 0.33, double ignoreBeat = 0.03125)
        {
            TypeList<double> diff = new TypeList<double>();

            for (int i = 0; i < beatList.Count - 1; i++)
            {
                double beat = beatList[i];
                double nextBeat = beatList[i + 1];
                double bpm = globalEffect.bpm.GetValue(beat) * globalEffect.tempo.GetValue(beat);

                double delayBeat = nextBeat - beat;
                double delay = RhythmManager.BeatToSecond(delayBeat, bpm);

                if (delayBeat >= ignoreBeat)
                    diff.Add(size / delay);
            }

            return diff;
        }

        public abstract void FixAllJudgmentBeat();
    }

    public sealed class MapInfo
    {
        [JsonIgnore] bool idIsReseted = false;
        [JsonIgnore] public string id
        {
            get
            {
                if (!idIsReseted)
                    throw new Exception(nameof(id) + " not reseted!");

                return _id;
            }
        }
        [JsonIgnore] private string _id = "";
        [JsonIgnore] public int randomSeed
        {
            get
            {
                if (!idIsReseted)
                    throw new Exception(nameof(id) + " not reseted!");

                return _randomSeed;
            }
        }
        [JsonIgnore] private int _randomSeed;



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
            byte[] bytes;
            if (File.Exists(mapFilePath))
                bytes = File.ReadAllBytes(mapFilePath);
            else
                bytes = Encoding.UTF8.GetBytes(JsonManager.ObjectToJson(this));

            _id = BitConverter.ToString(sha256.ComputeHash(bytes));
            _randomSeed = BitConverter.ToInt32(bytes);

            if (_id == null)
                _id = "";

            idIsReseted = true;
        }
    }

    public sealed class MapGlobalEffect
    {
        public BeatValuePairList<double> bpm { get; set; } = new(100);
        public BeatValuePairList<bool> yukiMode { get; set; } = new(false);



        public BackgroundEffect backgroundEffect { get; } = new BackgroundEffect();

        public class BackgroundEffect
        {
            public BeatValuePairList<BackgroundFileInfoPair> background { get; set; } = new(new BackgroundFileInfoPair("", ""));
            public BeatValuePairAniListColor backgroundColor { get; set; } = new(JColor.one);

            public BeatValuePairAniListVector2 positionOffset { get; set; } = new(default);
            public BeatValuePairAniListFloat zPositionOffset { get; set; } = new(default);
            public BeatValuePairAniListFloat rotationOffset { get; set; } = new(default);

            public BeatValuePairList<bool> positionUnfreeze { get; set; } = new(false);
            public BeatValuePairList<bool> zPositionUnfreeze { get; set; } = new(false);
            public BeatValuePairList<bool> rotationUnfreeze { get; set; } = new(false);

            public BeatValuePairAniListFloat positionFactor { get; set; } = new(1);
            public BeatValuePairAniListFloat zPositionFactor { get; set; } = new(1);
            public BeatValuePairAniListFloat rotationFactor { get; set; } = new(1);

            public BeatValuePairAniListColor videoColor { get; set; } = new(JColor.one);
        }



        public BeatValuePairAniListDouble cameraZoom { get; set; } = new(1);
        public BeatValuePairAniListVector3 cameraPos { get; set; } = new(new JVector3(0, 0, -14));
        public BeatValuePairAniListVector3 cameraRotation { get; set; } = new(default);

        public BeatValuePairAniListVector3 cameraShakeDrain { get; set; } = new(default);
        public BeatValuePairAniListVector3 cameraShakeOffset { get; set; } = new(new JVector3(0.5f, 0.5f, 0.5f));
        public BeatValuePairAniListDouble cameraShakeDelay { get; set; } = new(default);

        public BeatValuePairAniListColor backgroundFlash { get; set; } = new(default);
        public BeatValuePairAniListColor fieldFlash { get; set; } = new(default);
        public BeatValuePairAniListColor uiFlash { get; set; } = new(default);



        public BeatValuePairAniListDouble uiSize { get; set; } = new(1);



        public BeatValuePairAniListDouble pitch { get; set; } = new(1);
        public BeatValuePairAniListDouble tempo { get; set; } = new(1);

        public BeatValuePairAniListDouble volume { get; set; } = new(1);



        public BeatValuePairList<TypeList<HitsoundFile>> playSounds { get; set; } = new(new TypeList<HitsoundFile>());



        public BeatValuePairAniListDouble hpAddValue { get; set; } = new(6.25);
        public BeatValuePairAniListDouble hpMissValue { get; set; } = new(25);
        public BeatValuePairAniListDouble hpRemoveValue { get; set; } = new(6.25);



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
    public struct BackgroundFileInfoPair
    {
        public string backgroundFile
        {
            get => _backgroundFile ?? "";
            set => _backgroundFile = value;
        }
        string _backgroundFile;

        public string backgroundNightFile
        {
            get => _backgroundNightFile ?? "";
            set => _backgroundNightFile = value;
        }
        string _backgroundNightFile;

        public BackgroundFileInfoPair(string backgroundFile, string backgroundNightFile)
        {
            _backgroundFile = backgroundFile;
            _backgroundNightFile = backgroundNightFile;
        }
    }

    [SerializeField]
    public enum FlashOrder
    {
        background,
        field,
        ui
    }

    public struct HitsoundFile : IEquatable<HitsoundFile>
    {
        public static TypeList<HitsoundFile> defaultHitsounds => new TypeList<HitsoundFile> { defaultHitsound };
        public static HitsoundFile defaultHitsound { get; } = new HitsoundFile("normal", 0.5f, 0.95f);

        public HitsoundFile(string path, float volume, float pitch)
        {
            this.path = path;

            this.volume = volume;
            this.pitch = pitch;
        }

        public string path { get; set; }

        public float volume { get; set; }
        public float pitch { get; set; }

        public bool Equals(HitsoundFile other) => path == other.path && volume == other.volume && pitch == other.pitch;

        public override bool Equals(object obj) => obj is HitsoundFile other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(path, volume, pitch);
    }
    #endregion
}