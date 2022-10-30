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
        public SCKRM.Rhythm.BeatValuePairList<double> bpm { get; } = new(100);
        public SCKRM.Rhythm.BeatValuePairList<bool> dropPart { get; } = new(false);



        public BeatValuePairList<BackgroundEffectPair> background { get; } = new(default);
        public BeatValuePairAniListColor backgroundColor { get; } = new(JColor.one);

        public BeatValuePairAniListColor videoColor { get; } = new(JColor.one);



        public BeatValuePairAniListDouble cameraZoom { get; } = new(1);
        public BeatValuePairAniListVector3 cameraPos { get; } = new(default);
        public BeatValuePairAniListVector3 cameraRotation { get; } = new(default);

        public BeatValuePairAniListColor backgroundFlash { get; } = new(default);
        public BeatValuePairAniListColor fieldFlash { get; } = new(default);
        public BeatValuePairAniListColor uiFlash { get; } = new(default);



        public SCKRM.Rhythm.BeatValuePairAniListDouble pitch { get; } = new(1);
        public SCKRM.Rhythm.BeatValuePairAniListDouble tempo { get; } = new(1);

        public BeatValuePairAniListDouble volume { get; } = new(1);



        public SCKRM.Rhythm.BeatValuePairAniListDouble hpAddValue { get; } = new(1);
        public SCKRM.Rhythm.BeatValuePairAniListDouble hpMissValue { get; } = new(1);
        public SCKRM.Rhythm.BeatValuePairAniListDouble hpRemoveValue { get; } = new(1);



        public SCKRM.Rhythm.BeatValuePairAniListDouble judgmentSize { get; } = new(1);
    }



    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>>
    {
        public BeatValuePairList(T defaultValue) : base(defaultValue) { }

        public virtual void Add(double beat, bool disturbance) => Add(new BeatValuePair<T>() { beat = beat, value = defaultValue, disturbance = disturbance });
        public virtual void Add(T value, bool disturbance) => Add(new BeatValuePair<T>() { beat = double.MinValue, value = value, disturbance = disturbance });
        public virtual void Add(double beat, T value, bool disturbance) => Add(new BeatValuePair<T>() { beat = beat, value = value, disturbance = disturbance });
    }
    public abstract class BeatValuePairAniList<T> : BeatValuePairAniList<T, BeatValuePairAni<T>>
    {
        public BeatValuePairAniList(T defaultValue) : base(defaultValue) { }

        public virtual void Add(double beat, bool disturbance) => Add(new BeatValuePairAni<T>() { beat = beat, value = defaultValue, disturbance = disturbance });
        public virtual void Add(T value, bool disturbance) => Add(new BeatValuePairAni<T>() { beat = double.MinValue, value = value, disturbance = disturbance });
        public virtual void Add(double beat, T value, bool disturbance) => Add(new BeatValuePairAni<T>() { beat = beat, value = value, disturbance = disturbance });
        public virtual void Add(double beat, double length, bool disturbance) => Add(new BeatValuePairAni<T>() { beat = beat, length = length, value = defaultValue, disturbance = disturbance });
        public virtual void Add(double beat, double length, T value, EasingFunction.Ease easingFunction, bool disturbance) => Add(new BeatValuePairAni<T>() { beat = beat, length = length, value = value, easingFunction = easingFunction, disturbance = disturbance });
    }



    #region Built-in effect class
    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public BeatValuePairAniListFloat(float defaultValue) : base(defaultValue) { }

        public override float ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<float> previousBeatValuePair, IBeatValuePairAni<float> beatValuePair)
            => (float)easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public BeatValuePairAniListDouble(double defaultValue) : base(defaultValue) { }

        public override double ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<double> previousBeatValuePair, IBeatValuePairAni<double> beatValuePair)
            => easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public BeatValuePairAniListVector2(JVector2 defaultValue) : base(defaultValue) { }

        public override JVector2 ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector2> previousBeatValuePair, IBeatValuePairAni<JVector2> beatValuePair)
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
        public BeatValuePairAniListVector3(JVector3 defaultValue) : base(defaultValue) { }

        public override JVector3 ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector3> previousBeatValuePair, IBeatValuePairAni<JVector3> beatValuePair)
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
        public BeatValuePairAniListVector4(JVector4 defaultValue) : base(defaultValue) { }

        public override JVector4 ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector4> previousBeatValuePair, IBeatValuePairAni<JVector4> beatValuePair)
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
        public BeatValuePairAniListColor(JColor defaultValue) : base(defaultValue) { }

        public override JColor ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JColor> previousBeatValuePair, IBeatValuePairAni<JColor> beatValuePair)
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
        public BeatValuePairAniListRect(JRect defaultValue) : base(defaultValue) { }

        public override JRect ValueCalculate(double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JRect> previousBeatValuePair, IBeatValuePairAni<JRect> beatValuePair)
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
    public struct BackgroundEffectPair
    {
        public string backgroundFile { get; set; }
        public string backgroundNightFile { get; }

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