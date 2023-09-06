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
        void Add();
        void Insert(int index);
    }

    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>>
    {
        public BeatValuePairList(T defaultValue) : base(defaultValue) { }
    }

    public class BeatValuePairList<TValue, TPair> : TypeList<TPair>, IBeatValuePairList where TPair : IBeatValuePair<TValue>, new()
    {
        public BeatValuePairList(TValue defaultValue) => this.defaultValue = defaultValue;

        public TValue defaultValue { get; } = default;

        public TPair First()
        {
            if (Count > 0)
                return this[0];
            else
            {
                TPair pair = new TPair();

                pair.beat = double.MinValue;
                pair.value = defaultValue;

                return pair;
            }
        }

        public TPair Last()
        {
            if (Count > 0)
                return this[Count - 1];
            else
            {
                TPair pair = new TPair();

                pair.beat = double.MinValue;
                pair.value = defaultValue;

                return pair;
            }
        }

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
                TPair beatValuePair = this[GetValueIndexBinarySearch(currentBeat).Clamp(0)];

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

        void IBeatValuePairList.Add() => Add();
        public virtual void Add(double beat = double.MinValue, bool disturbance = false) => Add(new TPair() { beat = beat, value = defaultValue, disturbance = disturbance });
        public virtual void Add(TValue value, bool disturbance = false) => Add(new TPair() { beat = double.MinValue, value = value, disturbance = disturbance });
        public virtual void Add(double beat, TValue value, bool disturbance = false) => Add(new TPair() { beat = beat, value = value, disturbance = disturbance });

        void IBeatValuePairList.Insert(int index) => Insert(index);
        public virtual void Insert(int index, double beat = double.MinValue, bool disturbance = false) => Insert(index, new TPair() { beat = beat, value = defaultValue, disturbance = disturbance });
        public virtual void Insert(int index, TValue value, bool disturbance = false) => Insert(index, new TPair() { beat = double.MinValue, value = value, disturbance = disturbance });
        public virtual void Insert(int index, double beat, TValue value, bool disturbance = false) => Insert(index, new TPair() { beat = beat, value = value, disturbance = disturbance });

        public virtual int GetValueIndexBinarySearch(double beat)
        {
            if (Count <= 0)
                return 0;
            else if (beat < this[0].beat)
                return 0;
            else if (beat >= this[Count - 1].beat)
                return Count - 1;

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

            return low - 1;
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
                int index = GetValueIndexBinarySearch(currentBeat).Clamp(0);
                TPair beatValuePair = this[index];
                beat = beatValuePair.beat;

                if (beatValuePair.length == 0)
                    value = beatValuePair.value;
                else
                {
                    TPair previousBeatValuePair;
                    if (index <= 0)
                        previousBeatValuePair = Last();
                    else
                        previousBeatValuePair = this[index - 1];

                    double t = ((currentBeat - beatValuePair.beat) / beatValuePair.length).Clamp01();
                    if (!double.IsNormal(t))
                        t = 0;

                    value = ValueCalculate(t, beatValuePair.easingFunction, previousBeatValuePair.value, beatValuePair.value);
                }
            }

            return value;
        }

        public abstract TValue ValueCalculate(double t, EasingFunction.Ease easingFunction, TValue previousValue, TValue value);



        public virtual void Add(double beat = double.MinValue, double length = 0, bool disturbance = false) => Add(new TPair() { beat = beat, length = length, value = defaultValue, disturbance = disturbance });
        public virtual void Add(double beat, double length, TValue value, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear, bool disturbance = false) => Add(new TPair() { beat = beat, length = length, value = value, easingFunction = easingFunction, disturbance = disturbance });
    }
    #endregion



    #region Built-in effect class
    public class BeatValuePairAniListInt : BeatValuePairAniList<int>
    {
        public BeatValuePairAniListInt(int defaultValue) : base(defaultValue) { }

        public override int ValueCalculate(double t, EasingFunction.Ease easingFunction, int previousValue, int value)
            => EasingFunction.EasingCalculate(previousValue, value, t, easingFunction).RoundToInt();
    }

    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public BeatValuePairAniListFloat(float defaultValue) : base(defaultValue) { }

        public override float ValueCalculate(double t, EasingFunction.Ease easingFunction, float previousValue, float value)
            => (float)EasingFunction.EasingCalculate(previousValue, value, t, easingFunction);
    }

    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public BeatValuePairAniListDouble(double defaultValue) : base(defaultValue) { }

        public override double ValueCalculate(double t, EasingFunction.Ease easingFunction, double previousValue, double value)
            => EasingFunction.EasingCalculate(previousValue, value, t, easingFunction);
    }

    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public BeatValuePairAniListVector2(JVector2 defaultValue) : base(defaultValue) { }

        public override JVector2 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector2 previousValue, JVector2 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);

            return new JVector2(x, y);
        }
    }

    public class BeatValuePairAniListVector3 : BeatValuePairAniList<JVector3>
    {
        public BeatValuePairAniListVector3(JVector3 defaultValue) : base(defaultValue) { }

        public override JVector3 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector3 previousValue, JVector3 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(previousValue.z, value.z, t, easingFunction);

            return new JVector3(x, y, z);
        }
    }

    public class BeatValuePairAniListVector4 : BeatValuePairAniList<JVector4>
    {
        public BeatValuePairAniListVector4(JVector4 defaultValue) : base(defaultValue) { }

        public override JVector4 ValueCalculate(double t, EasingFunction.Ease easingFunction, JVector4 previousValue, JVector4 value)
        {
            float x = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float y = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float z = (float)EasingFunction.EasingCalculate(previousValue.z, value.z, t, easingFunction);
            float w = (float)EasingFunction.EasingCalculate(previousValue.w, value.w, t, easingFunction);

            return new JVector4(x, y, z, w);
        }
    }

    public class BeatValuePairAniListColor : BeatValuePairAniList<JColor>
    {
        public BeatValuePairAniListColor(JColor defaultValue) : base(defaultValue) { }

        public override JColor ValueCalculate(double t, EasingFunction.Ease easingFunction, JColor previousValue, JColor value)
        {
            float r = (float)EasingFunction.EasingCalculate(previousValue.r, value.r, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(previousValue.g, value.g, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(previousValue.b, value.b, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(previousValue.a, value.a, t, easingFunction);

            return new JColor(r, g, b, a);
        }
    }

    public class BeatValuePairAniListColor32 : BeatValuePairAniList<JColor32>
    {
        public BeatValuePairAniListColor32(JColor32 defaultValue) : base(defaultValue) { }

        public override JColor32 ValueCalculate(double t, EasingFunction.Ease easingFunction, JColor32 previousValue, JColor32 value)
        {
            byte r = (byte)EasingFunction.EasingCalculate(previousValue.r, value.r, t, easingFunction);
            byte g = (byte)EasingFunction.EasingCalculate(previousValue.g, value.g, t, easingFunction);
            byte b = (byte)EasingFunction.EasingCalculate(previousValue.b, value.b, t, easingFunction);
            byte a = (byte)EasingFunction.EasingCalculate(previousValue.a, value.a, t, easingFunction);

            return new JColor32(r, g, b, a);
        }
    }

    public class BeatValuePairAniListRect : BeatValuePairAniList<JRect>
    {
        public BeatValuePairAniListRect(JRect defaultValue) : base(defaultValue) { }

        public override JRect ValueCalculate(double t, EasingFunction.Ease easingFunction, JRect previousValue, JRect value)
        {
            float r = (float)EasingFunction.EasingCalculate(previousValue.x, value.x, t, easingFunction);
            float g = (float)EasingFunction.EasingCalculate(previousValue.y, value.y, t, easingFunction);
            float b = (float)EasingFunction.EasingCalculate(previousValue.width, value.width, t, easingFunction);
            float a = (float)EasingFunction.EasingCalculate(previousValue.height, value.height, t, easingFunction);

            return new JRect(r, g, b, a);
        }
    }
    #endregion



    #region Beat Value Pair
    public interface IBeatValuePair
    {
        Type type { get; }

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
        public Type type => typeof(TValue);

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
        public Type type => typeof(TValue);

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
