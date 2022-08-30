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
        public List<Map> maps { get; } = new List<Map>();
    }

    public class Map
    {
        public MapInfo info { get; } = new MapInfo();
        public MapGlobalEffect globalEffect { get; } = new MapGlobalEffect();

        public List<double> allBeat { get; } = new List<double>();

        [JsonIgnore] public string mapFilePathParent { get; set; } = "";
        [JsonIgnore] public string mapFilePath { get; set; } = "";
    }

    public sealed class MapInfo
    {
        [System.Obsolete("Not implemented!", true), JsonIgnore] public ulong id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }



        public Version sckrmVersion { get; set; } = new Version();
        public Version sdjkVersion { get; set; } = new Version();



        public string mode { get; set; } = "";



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
        public SCKRM.Rhythm.BeatValuePairList<double> bpm { get; } = new();
        public SCKRM.Rhythm.BeatValuePairList<bool> dropPart { get; } = new();



        public BeatValuePairList<BackgroundEffect> background { get; } = new();
        public BeatValuePairAniListColor backgroundColor { get; } = new();



        public BeatValuePairAniListDouble cameraZoom { get; } = new();
        public BeatValuePairAniListVector3 cameraPos { get; } = new();
        public BeatValuePairAniListVector3 cameraRotation { get; } = new();



        public SCKRM.Rhythm.BeatValuePairAniListDouble pitch { get; } = new();
        public SCKRM.Rhythm.BeatValuePairAniListDouble tempo { get; } = new();

        public BeatValuePairAniListDouble volume { get; } = new();



        public SCKRM.Rhythm.BeatValuePairAniListDouble hpAddValue { get; } = new();
        public SCKRM.Rhythm.BeatValuePairAniListDouble hpMissValue { get; } = new();
        public SCKRM.Rhythm.BeatValuePairAniListDouble hpRemoveValue { get; } = new();



        public SCKRM.Rhythm.BeatValuePairAniListDouble judgmentSize { get; } = new();
    }



    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>> { }
    public abstract class BeatValuePairAniList<T> : BeatValuePairAniList<T, BeatValuePairAni<T>> { }



    #region Built-in effect class
    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public override float GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static float ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<float> previousBeatValuePair, IBeatValuePairAni<float> beatValuePair)
            => (float)easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public override double GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static double ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<double> previousBeatValuePair, IBeatValuePairAni<double> beatValuePair)
            => easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public override JVector2 GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static JVector2 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector2> previousBeatValuePair, IBeatValuePairAni<JVector2> beatValuePair)
        {
            JVector2 pre = previousBeatValuePair.value;
            JVector2 value = beatValuePair.value;
            float x = (float)easingFunction.Invoke(pre.x, value.x, t);
            float y = (float)easingFunction.Invoke(pre.y, value.y, t);

            return new JVector2(x, y);
        }
    }

    public class BeatValuePairAniListVector3 : BeatValuePairAniList<JVector3>
    {
        public override JVector3 GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static JVector3 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector3> previousBeatValuePair, IBeatValuePairAni<JVector3> beatValuePair)
        {
            JVector3 pre = previousBeatValuePair.value;
            JVector3 value = beatValuePair.value;
            float x = (float)easingFunction.Invoke(pre.x, value.x, t);
            float y = (float)easingFunction.Invoke(pre.y, value.y, t);
            float z = (float)easingFunction.Invoke(pre.z, value.z, t);

            return new JVector3(x, y, z);
        }
    }

    public class BeatValuePairAniListVector4 : BeatValuePairAniList<JVector4>
    {
        public override JVector4 GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static JVector4 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector4> previousBeatValuePair, IBeatValuePairAni<JVector4> beatValuePair)
        {
            JVector4 pre = previousBeatValuePair.value;
            JVector4 value = beatValuePair.value;
            float x = (float)easingFunction.Invoke(pre.x, value.x, t);
            float y = (float)easingFunction.Invoke(pre.y, value.y, t);
            float z = (float)easingFunction.Invoke(pre.z, value.z, t);
            float w = (float)easingFunction.Invoke(pre.w, value.w, t);

            return new JVector4(x, y, z, w);
        }
    }

    public class BeatValuePairAniListColor : BeatValuePairAniList<JColor>
    {
        public override JColor GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static JColor ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JColor> previousBeatValuePair, IBeatValuePairAni<JColor> beatValuePair)
        {
            JColor pre = previousBeatValuePair.value;
            JColor value = beatValuePair.value;
            float r = (float)easingFunction.Invoke(pre.r, value.r, t);
            float g = (float)easingFunction.Invoke(pre.g, value.g, t);
            float b = (float)easingFunction.Invoke(pre.b, value.b, t);
            float a = (float)easingFunction.Invoke(pre.a, value.a, t);

            return new JColor(r, g, b, a);
        }
    }

    public class BeatValuePairAniListRect : BeatValuePairAniList<JRect>
    {
        public override JRect GetValue(double currentBeat, out double beat, out bool isValueChanged) => GetValueInternal(currentBeat, out beat, out isValueChanged, ValueCalculate);

        static JRect ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JRect> previousBeatValuePair, IBeatValuePairAni<JRect> beatValuePair)
        {
            JRect pre = previousBeatValuePair.value;
            JRect value = beatValuePair.value;
            float r = (float)easingFunction.Invoke(pre.x, value.x, t);
            float g = (float)easingFunction.Invoke(pre.y, value.y, t);
            float b = (float)easingFunction.Invoke(pre.width, value.width, t);
            float a = (float)easingFunction.Invoke(pre.height, value.height, t);

            return new JRect(r, g, b, a);
        }
    }
    #endregion



    #region Beat Value Pair
    public interface IDisturbance
    {
        public bool disturbance { get; set; }
    }

    public struct BeatValuePair<TValue> : IBeatValuePair<TValue>, IDisturbance
    {
        public double beat { get; set; }
        public TValue value { get; set; }

        public bool disturbance { get; set; }

        public BeatValuePair(double beat, TValue value, bool disturbance)
        {
            this.beat = beat;
            this.value = value;
            this.disturbance = disturbance;
        }
    }

    public struct BeatValuePairAni<TValue> : IBeatValuePairAni<TValue>, IDisturbance
    {
        public double beat { get; set; }
        public TValue value { get; set; }

        public double length { get; set; }
        public EasingFunction.Ease easingFunction { get; set; }

        public bool disturbance { get; set; }

        public BeatValuePairAni(double beat, TValue value, double length, EasingFunction.Ease easingFunction, bool disturbance)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;
            this.easingFunction = easingFunction;

            this.disturbance = disturbance;
        }
    }
    #endregion



    #region Effect
    public struct BackgroundEffect
    {
        public string backgroundFile { get; set; }
        public string backgroundNightFile { get; }

        public BackgroundEffect(string backgroundFile, string backgroundNightFile)
        {
            this.backgroundFile = backgroundFile;
            this.backgroundNightFile = backgroundNightFile;
        }
    }
    #endregion
}