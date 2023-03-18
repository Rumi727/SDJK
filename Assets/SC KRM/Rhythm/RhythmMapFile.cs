using SCKRM.Easing;
using SCKRM.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SCKRM.Rhythm
{
    #region Beat Value Pair List
    //리플랙션 용
    public interface IBeatValuePairList : IList
    {
        Type pairType { get; }
    }

    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>>
    {
        public BeatValuePairList(T defaultValue) : base(defaultValue) { }
    }

    public class BeatValuePairList<TValue, TPair> : List<TPair>, IBeatValuePairList where TPair : IBeatValuePair<TValue>, new()
    {
        public BeatValuePairList(TValue defaultValue) => this.defaultValue = defaultValue;

        public TValue defaultValue { get; } = default;
        public Type pairType => typeof(TPair);

        public TValue GetValue() => GetValue(RhythmManager.currentBeat, out _);
        public TValue GetValue(double currentBeat) => GetValue(currentBeat, out _);

        TValue tempValue = default;
        double tempBeat = 0;
        double? tempCurrentBeat = null;
        public virtual TValue GetValue(double currentBeat, out double beat)
        {
            if (tempCurrentBeat != null && (double)tempCurrentBeat == currentBeat)
            {
                beat = tempBeat;
                return tempValue;
            }

            tempCurrentBeat = currentBeat;

            TValue value;
            if (Count <= 0)
            {
                beat = 0;
                value = defaultValue;
            }
            else
            {
                TPair beatValuePair = this[(GetValueIndexBinarySearch(currentBeat) - 1).Clamp(0)];

                beat = beatValuePair.beat;
                value = beatValuePair.value;
            }

            //isValueChanged = !((IEquatable<TValue>)tempValue).Equals(value);
            tempValue = value;
            tempBeat = beat;

            return value;
        }

        public void FindValue(Predicate<TPair> match) => FindValue(RhythmManager.currentBeat, match, out _, out _);
        public void FindValue(double currentBeat, Predicate<TPair> match) => FindValue(currentBeat, match, out _, out _);

        public virtual void FindValue(double currentBeat, Predicate<TPair> match, out double beat, out int index)
        {
            if (Count <= 0)
            {
                beat = 0;
                index = 0;

                return;
            }

            TPair firstPair = this[0];
            bool firstPairMatch = match(firstPair);
            if ((Count <= 1 && firstPairMatch) || (firstPair.beat >= currentBeat && firstPairMatch))
            {
                beat = this[0].beat;
                index = 0;

                return;
            }
            else
            {
                TPair lastPair = firstPair;
                int lastIndex = 0;
                for (int i = 0; i < Count; i++)
                {
                    TPair pair = this[i];
                    if (pair.beat > currentBeat && match(lastPair))
                    {
                        beat = lastPair.beat;
                        index = lastIndex - 1;

                        return;
                    }

                    lastPair = pair;
                    lastIndex = i;
                }

                beat = 0;
                index = 0;

                return;
            }
        }

        public virtual void Add(double beat = double.MinValue, bool disturbance = false) => Add(new TPair() { beat = beat, value = defaultValue, disturbance = disturbance });
        public virtual void Add(TValue value, bool disturbance = false) => Add(new TPair() { beat = double.MinValue, value = value, disturbance = disturbance });
        public virtual void Add(double beat, TValue value, bool disturbance = false) => Add(new TPair() { beat = beat, value = value, disturbance = disturbance });

        public virtual int GetValueIndexBinarySearch(double beat)
        {
            if (Count <= 0)
                return 0;
            else if (beat < this[0].beat)
                return 0;
            else if (beat >= this[Count - 1].beat)
                return Count;

            int low = 0;
            int high = Count - 1;

            while (low < high)
            {
                int index = (low + high) / 2;
                if (this[index].beat <= beat)
                    low = index + 1;
                else
                    high = index;
            }

            return low;
        }
    }
    #endregion

    #region Beat Value Pair Ani List
    //리플랙션 용
    public interface IBeatValuePairAniList : IBeatValuePairList { }

    public abstract class BeatValuePairAniList<T> : BeatValuePairAniList<T, BeatValuePairAni<T>>
    {
        public BeatValuePairAniList(T defaultValue) : base(defaultValue) { }
    }

    public abstract class BeatValuePairAniList<TValue, TPair> : BeatValuePairList<TValue, TPair>, IBeatValuePairAniList where TPair : IBeatValuePairAni<TValue>, new()
    {
        public BeatValuePairAniList(TValue defaultValue) : base(defaultValue) { }



        public override TValue GetValue(double currentBeat, out double beat)
        {
            TValue value;
            if (Count <= 0)
            {
                beat = 0;
                value = defaultValue;
            }
            else
            {
                int index = (GetValueIndexBinarySearch(currentBeat) - 1).Clamp(0);
                TPair beatValuePair = this[index];
                beat = beatValuePair.beat;

                if (index <= 0 || beatValuePair.length == 0)
                    value = beatValuePair.value;
                else
                {
                    TPair previousBeatValuePair = this[index - 1];
                    double t = ((currentBeat - beatValuePair.beat) / beatValuePair.length).Clamp01();

                    value = ValueCalculate(t, beatValuePair.easingFunction, previousBeatValuePair, beatValuePair);
                }
            }

            return value;
        }

        public abstract TValue ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<TValue> previousBeatValuePair, IBeatValuePairAni<TValue> beatValuePair);



        public virtual void Add(double beat = double.MinValue, double length = 0, bool disturbance = false) => Add(new TPair() { beat = beat, length = length, value = defaultValue, disturbance = disturbance });
        public virtual void Add(double beat, double length, TValue value, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear, bool disturbance = false) => Add(new TPair() { beat = beat, length = length, value = value, easingFunction = easingFunction, disturbance = disturbance });
    }
    #endregion



    #region Built-in effect class
    public class BeatValuePairAniListInt : BeatValuePairAniList<int>
    {
        public BeatValuePairAniListInt(int defaultValue) : base(defaultValue) { }

        public override int ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<int> previousBeatValuePair, IBeatValuePairAni<int> beatValuePair)
            => EasingFunction.EasingCalculate(previousBeatValuePair.value, beatValuePair.value, t, easingFunction).RoundToInt();
    }

    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public BeatValuePairAniListFloat(float defaultValue) : base(defaultValue) { }

        public override float ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<float> previousBeatValuePair, IBeatValuePairAni<float> beatValuePair)
            => (float)EasingFunction.EasingCalculate(previousBeatValuePair.value, beatValuePair.value, t, easingFunction);
    }

    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public BeatValuePairAniListDouble(double defaultValue) : base(defaultValue) { }

        public override double ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<double> previousBeatValuePair, IBeatValuePairAni<double> beatValuePair)
            => EasingFunction.EasingCalculate(previousBeatValuePair.value, beatValuePair.value, t, easingFunction);
    }

    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public BeatValuePairAniListVector2(JVector2 defaultValue) : base(defaultValue) { }

        public override JVector2 ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JVector2> previousBeatValuePair, IBeatValuePairAni<JVector2> beatValuePair)
        {
            JVector2 pre = previousBeatValuePair.value;
            JVector2 value = beatValuePair.value;
            float x = (float)EasingFunction.EasingCalculate(pre.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(pre.y, value.y, t, easingFunction);

            return new JVector2(x, y);
        }
    }

    public class BeatValuePairAniListVector3 : BeatValuePairAniList<JVector3>
    {
        public BeatValuePairAniListVector3(JVector3 defaultValue) : base(defaultValue) { }

        public override JVector3 ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JVector3> previousBeatValuePair, IBeatValuePairAni<JVector3> beatValuePair)
        {
            JVector3 pre = previousBeatValuePair.value;
            JVector3 value = beatValuePair.value;
            float x = (float)EasingFunction.EasingCalculate(pre.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(pre.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(pre.z, value.z, t, easingFunction);

            return new JVector3(x, y, z);
        }
    }

    public class BeatValuePairAniListVector4 : BeatValuePairAniList<JVector4>
    {
        public BeatValuePairAniListVector4(JVector4 defaultValue) : base(defaultValue) { }

        public override JVector4 ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JVector4> previousBeatValuePair, IBeatValuePairAni<JVector4> beatValuePair)
        {
            JVector4 pre = previousBeatValuePair.value;
            JVector4 value = beatValuePair.value;
            float x = (float)EasingFunction.EasingCalculate(pre.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(pre.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(pre.z, value.z, t, easingFunction);
            float w = (float)EasingFunction.EasingCalculate(pre.w, value.w, t, easingFunction);

            return new JVector4(x, y, z, w);
        }
    }

    public class BeatValuePairAniListColor : BeatValuePairAniList<JColor>
    {
        public BeatValuePairAniListColor(JColor defaultValue) : base(defaultValue) { }

        public override JColor ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JColor> previousBeatValuePair, IBeatValuePairAni<JColor> beatValuePair)
        {
            JColor pre = previousBeatValuePair.value;
            JColor value = beatValuePair.value;
            float r = (float)EasingFunction.EasingCalculate(pre.r, value.r, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(pre.g, value.g, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(pre.b, value.b, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(pre.a, value.a, t, easingFunction);

            return new JColor(r, g, b, a);
        }
    }

    public class BeatValuePairAniListColor32 : BeatValuePairAniList<JColor32>
    {
        public BeatValuePairAniListColor32(JColor32 defaultValue) : base(defaultValue) { }

        public override JColor32 ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JColor32> previousBeatValuePair, IBeatValuePairAni<JColor32> beatValuePair)
        {
            JColor32 pre = previousBeatValuePair.value;
            JColor32 value = beatValuePair.value;
            byte r = (byte)EasingFunction.EasingCalculate(pre.r, value.r, t, easingFunction);
            byte g = (byte)EasingFunction.EasingCalculate(pre.g, value.g, t, easingFunction);
            byte b = (byte)EasingFunction.EasingCalculate(pre.b, value.b, t, easingFunction);
            byte a = (byte)EasingFunction.EasingCalculate(pre.a, value.a, t, easingFunction);

            return new JColor32(r, g, b, a);
        }
    }

    public class BeatValuePairAniListRect : BeatValuePairAniList<JRect>
    {
        public BeatValuePairAniListRect(JRect defaultValue) : base(defaultValue) { }

        public override JRect ValueCalculate(double t, EasingFunction.Ease easingFunction, IBeatValuePairAni<JRect> previousBeatValuePair, IBeatValuePairAni<JRect> beatValuePair)
        {
            JRect pre = previousBeatValuePair.value;
            JRect value = beatValuePair.value;
            float r = (float)EasingFunction.EasingCalculate(pre.x, value.x, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(pre.y, value.y, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(pre.width, value.width, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(pre.height, value.height, t, easingFunction);

            return new JRect(r, g, b, a);
        }
    }
    #endregion



    #region Beat Value Pair
    public interface IBeatValuePair
    {
        double beat { get; set; }
        object value { get; set; }

        bool disturbance { get; set; }
    }

    public interface IBeatValuePair<TValue> : IBeatValuePair
    {
        new TValue value { get; set; }
    }

    public interface IBeatValuePairAni : IBeatValuePair
    {
        double length { get; set; }
        EasingFunction.Ease easingFunction { get; set; }
    }

    public interface IBeatValuePairAni<TValue> : IBeatValuePair<TValue>, IBeatValuePairAni { }

    public struct BeatValuePair<TValue> : IBeatValuePair<TValue>
    {
        public double beat { get; set; }

        public TValue value { get; set; }
        object IBeatValuePair.value { get => value; set => this.value = (TValue)value; }

        public bool disturbance { get; set; }

        public BeatValuePair(double beat, TValue value, bool disturbance = false)
        {
            this.beat = beat;
            this.value = value;

            this.disturbance = disturbance;
        }
    }

    public struct BeatValuePairAni<TValue> : IBeatValuePairAni<TValue>
    {
        public double beat { get; set; }

        public TValue value { get; set; }
        object IBeatValuePair.value { get => value; set => this.value = (TValue)value; }

        public double length { get; set; }
        public EasingFunction.Ease easingFunction { get; set; }

        public bool disturbance { get; set; }

        public BeatValuePairAni(double beat, TValue value, double length, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear, bool disturbance = false)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;
            this.easingFunction = easingFunction;

            this.disturbance = disturbance;
        }
    }
    #endregion
}
