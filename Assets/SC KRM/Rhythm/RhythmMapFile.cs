using SCKRM.Easing;
using SCKRM.Json;
using System;
using System.Collections.Generic;

namespace SCKRM.Rhythm
{
    #region Beat Value Pair List
    public class BeatValuePairList<T> : BeatValuePairList<T, BeatValuePair<T>>
    {
        public BeatValuePairList(T defaultValue) : base(defaultValue) { }
    }

    public class BeatValuePairList<TValue, TPair> : List<TPair> where TPair : IBeatValuePair<TValue>, new()
    {
        public BeatValuePairList(TValue defaultValue) => this.defaultValue = defaultValue;

        public TValue defaultValue { get; } = default;

        public TValue GetValue() => GetValue(RhythmManager.currentBeat, out _, out _);
        public TValue GetValue(double currentBeat) => GetValue(currentBeat, out _, out _);

        TValue tempValue = default;
        double tempBeat = 0;
        double? tempCurrentBeat = null;
        public virtual TValue GetValue(double currentBeat, out double beat, out bool isValueChanged)
        {
            if (tempCurrentBeat != null && (double)tempCurrentBeat == currentBeat)
            {
                beat = tempBeat;
                isValueChanged = false;

                return tempValue;
            }

            tempCurrentBeat = currentBeat;

            TValue value;
            if (Count <= 0)
            {
                beat = 0;
                value = defaultValue;
            }
            else if (Count <= 1 || this[0].beat >= currentBeat)
            {
                beat = this[0].beat;
                value = this[0].value;
            }
            else
            {
                int findIndex = FindIndex(x => x.beat >= currentBeat);
                if (findIndex < 0)
                    findIndex = Count;

                TPair beatValuePair = this[findIndex - 1];
                beat = beatValuePair.beat;
                value = beatValuePair.value;
            }

            isValueChanged = !tempValue.Equals(value);
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
                    if (pair.beat >= currentBeat && match(lastPair))
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

        public virtual void Add(double beat = double.MinValue) => Add(new TPair() { beat = beat, value = defaultValue });
        public virtual void Add(TValue value) => Add(new TPair() { beat = double.MinValue, value = value });
        public virtual void Add(double beat, TValue value) => Add(new TPair() { beat = beat, value = value });
    }
    #endregion

    #region Beat Value Pair Ani List
    public abstract class BeatValuePairAniList<T> : BeatValuePairAniList<T, BeatValuePairAni<T>>
    {
        public BeatValuePairAniList(T defaultValue) : base(defaultValue) { }
    }

    public abstract class BeatValuePairAniList<TValue, TPair> : BeatValuePairList<TValue, TPair> where TPair : IBeatValuePairAni<TValue>, new()
    {
        public BeatValuePairAniList(TValue defaultValue) : base(defaultValue) { }



        TValue tempValue = default;
        public override TValue GetValue(double currentBeat, out double beat, out bool isValueChanged)
        {
            TValue value;
            if (Count <= 0)
            {
                beat = 0;
                value = defaultValue;
            }
            else if (Count <= 1 || this[0].beat >= currentBeat)
            {
                beat = this[0].beat;
                value = this[0].value;
            }
            else
            {
                int findIndex = FindIndex(x => x.beat >= currentBeat);
                if (findIndex < 0)
                    findIndex = Count;


                int index = findIndex - 1;
                TPair beatValuePair = this[index];
                beat = beatValuePair.beat;

                if (index <= 0 || beatValuePair.length == 0)
                    value = beatValuePair.value;
                else
                {
                    TPair previousBeatValuePair = this[index - 1];
                    double t = ((currentBeat - beatValuePair.beat) / beatValuePair.length).Clamp01();

                    value = ValueCalculate(currentBeat, t, EasingFunction.GetEasingFunction(beatValuePair.easingFunction), previousBeatValuePair, beatValuePair);
                }
            }

            isValueChanged = !tempValue.Equals(value);
            tempValue = value;

            return value;
        }

        public abstract TValue ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<TValue> previousBeatValuePair, IBeatValuePairAni<TValue> beatValuePair);



        public virtual void Add(double beat = double.MinValue, double length = 0) => Add(new TPair() { beat = beat, length = length, value = defaultValue });
        public virtual void Add(double beat, double length, TValue value, EasingFunction.Ease easingFunction = EasingFunction.Ease.Linear) => Add(new TPair() { beat = beat, length = length, value = value, easingFunction = easingFunction });
    }
    #endregion



    #region Built-in effect class
    public class BeatValuePairAniListFloat : BeatValuePairAniList<float>
    {
        public BeatValuePairAniListFloat(float defaultValue) : base(defaultValue) { }

        public override float ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<float> previousBeatValuePair, IBeatValuePairAni<float> beatValuePair)
            => (float)easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListDouble : BeatValuePairAniList<double>
    {
        public BeatValuePairAniListDouble(double defaultValue) : base(defaultValue) { }

        public override double ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<double> previousBeatValuePair, IBeatValuePairAni<double> beatValuePair)
            => easingFunction.Invoke(previousBeatValuePair.value, beatValuePair.value, t);
    }

    public class BeatValuePairAniListVector2 : BeatValuePairAniList<JVector2>
    {
        public BeatValuePairAniListVector2(JVector2 defaultValue) : base(defaultValue) { }

        public override JVector2 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector2> previousBeatValuePair, IBeatValuePairAni<JVector2> beatValuePair)
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

        public override JVector3 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector3> previousBeatValuePair, IBeatValuePairAni<JVector3> beatValuePair)
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

        public override JVector4 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JVector4> previousBeatValuePair, IBeatValuePairAni<JVector4> beatValuePair)
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

        public override JColor ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JColor> previousBeatValuePair, IBeatValuePairAni<JColor> beatValuePair)
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

    public class BeatValuePairAniListColor32 : BeatValuePairAniList<JColor32>
    {
        public BeatValuePairAniListColor32(JColor32 defaultValue) : base(defaultValue) { }

        public override JColor32 ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JColor32> previousBeatValuePair, IBeatValuePairAni<JColor32> beatValuePair)
        {
            JColor32 pre = previousBeatValuePair.value;
            JColor32 value = beatValuePair.value;
            byte r = (byte)easingFunction.Invoke(pre.r, value.r, t);
            byte g = (byte)easingFunction.Invoke(pre.g, value.g, t);
            byte b = (byte)easingFunction.Invoke(pre.b, value.b, t);
            byte a = (byte)easingFunction.Invoke(pre.a, value.a, t);

            return new JColor32(r, g, b, a);
        }
    }

    public class BeatValuePairAniListRect : BeatValuePairAniList<JRect>
    {
        public BeatValuePairAniListRect(JRect defaultValue) : base(defaultValue) { }

        public override JRect ValueCalculate(double currentBeat, double t, EasingFunction.Function easingFunction, IBeatValuePairAni<JRect> previousBeatValuePair, IBeatValuePairAni<JRect> beatValuePair)
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
    public interface IBeatValuePair<TValue>
    {
        double beat { get; set; }
        TValue value { get; set; }
    }

    public interface IBeatValuePairAni<TValue> : IBeatValuePair<TValue>
    {
        double length { get; set; }
        EasingFunction.Ease easingFunction { get; set; }
    }



    public struct BeatValuePair<TValue> : IBeatValuePair<TValue>
    {
        public double beat { get; set; }
        public TValue value { get; set; }

        public BeatValuePair(double beat, TValue value)
        {
            this.beat = beat;
            this.value = value;
        }
    }

    public struct BeatValuePairAni<TValue> : IBeatValuePairAni<TValue>
    {
        public double beat { get; set; }
        public TValue value { get; set; }

        public double length { get; set; }
        public EasingFunction.Ease easingFunction { get; set; }

        public BeatValuePairAni(double beat, TValue value, double length, EasingFunction.Ease easingFunction)
        {
            this.beat = beat;
            this.value = value;

            this.length = length;
            this.easingFunction = easingFunction;
        }
    }
    #endregion
}
