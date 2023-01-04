using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace SCKRM
{
    public static class MathTool
    {
        public const float epsilonFloatWithAccuracy = 0.0001f;

        #region Trigonometric functions
        public static float Sin(this float value) => (float)Math.Sin(value);
        [WikiIgnore] public static double Sin(this double value) => Math.Sin(value);
        [WikiIgnore] public static decimal Sin(this decimal value) => (decimal)Math.Sin((double)value);

        public static float Asin(this float value) => (float)Math.Asin(value);
        [WikiIgnore] public static double Asin(this double value) => Math.Asin(value);
        [WikiIgnore] public static decimal Asin(this decimal value) => (decimal)Math.Asin((double)value);

        public static float Cos(this float value) => (float)Math.Cos(value);
        [WikiIgnore] public static double Cos(this double value) => Math.Cos(value);
        [WikiIgnore] public static decimal Cos(this decimal value) => (decimal)Math.Cos((double)value);

        public static float Acos(this float value) => (float)Math.Acos(value);
        [WikiIgnore] public static double Acos(this double value) => Math.Acos(value);
        [WikiIgnore] public static decimal Acos(this decimal value) => (decimal)Math.Acos((double)value);

        public static float Tan(this float value) => (float)Math.Tan(value);
        [WikiIgnore] public static double Tan(this double value) => Math.Tan(value);
        [WikiIgnore] public static decimal Tan(this decimal value) => (decimal)Math.Tan((double)value);

        public static float Atan(this float value) => (float)Math.Atan(value);
        [WikiIgnore] public static double Atan(this double value) => Math.Atan(value);
        [WikiIgnore] public static decimal Atan(this decimal value) => (decimal)Math.Atan((double)value);
        #endregion

        #region Abs
        public static sbyte Abs(this sbyte value)
        {
            if (value < 0)
                return (sbyte)-value;
            else
                return value;
        }

        [WikiIgnore]
        public static short Abs(this short value)
        {
            if (value < 0)
                return (sbyte)-value;
            else
                return value;
        }

        [WikiIgnore]
        public static int Abs(this int value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static long Abs(this long value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static float Abs(this float value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static double Abs(this double value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static decimal Abs(this decimal value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static BigInteger Abs(this BigInteger value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static BigDecimal Abs(this BigDecimal value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }

        [WikiIgnore]
        public static nint Abs(this nint value)
        {
            if (value < 0)
                return -value;
            else
                return value;
        }
        #endregion Abs

        #region Sign
        public static int Sign(this sbyte value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static short Sign(this short value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this int value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static long Sign(this long value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this float value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this double value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this decimal value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this BigInteger value)
        {
            if (value < BigInteger.Zero)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this BigDecimal value)
        {
            if (value < BigDecimal.Zero)
                return -1;
            else
                return 1;
        }

        [WikiIgnore]
        public static int Sign(this nint value)
        {
            if (value < 0)
                return -1;
            else
                return 1;
        }
        #endregion

        #region Clamp
        public static byte Clamp(this byte value, byte min, byte max = byte.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max = sbyte.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static short Clamp(this short value, short min, short max = short.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static ushort Clamp(this ushort value, ushort min, ushort max = ushort.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static int Clamp(this int value, int min, int max = int.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static uint Clamp(this uint value, uint min, uint max = uint.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static long Clamp(this long value, long min, long max = long.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static ulong Clamp(this ulong value, ulong min, ulong max = ulong.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static float Clamp(this float value, float min, float max = float.PositiveInfinity)
        {
            if (float.IsNaN(value))
                return 0;

            if (value < min || float.IsNegativeInfinity(value))
                return min;
            else if (value > max || float.IsPositiveInfinity(value))
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static double Clamp(this double value, double min, double max = double.PositiveInfinity)
        {
            if (double.IsNaN(value))
                return 0;

            if (value < min || double.IsNegativeInfinity(value))
                return min;
            else if (value > max || double.IsPositiveInfinity(value))
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static decimal Clamp(this decimal value, decimal min, decimal max = decimal.MaxValue)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static BigInteger Clamp(this BigInteger value, BigInteger min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static BigInteger Clamp(this BigInteger value, BigInteger min, BigInteger max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static BigDecimal Clamp(this BigDecimal value, BigDecimal min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static BigDecimal Clamp(this BigDecimal value, BigDecimal min, BigDecimal max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static nint Clamp(this nint value, nint min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static nint Clamp(this nint value, nint min, nint max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static nuint Clamp(this nuint value, nuint min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static nuint Clamp(this nuint value, nuint min, nuint max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }
        #endregion

        #region Clamp01
        public static byte Clamp01(this byte value)
        {
            if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static sbyte Clamp01(this sbyte value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static short Clamp01(this short value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static ushort Clamp01(this ushort value)
        {
            if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static int Clamp01(this int value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static uint Clamp01(this uint value)
        {
            if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static long Clamp01(this long value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static ulong Clamp01(this ulong value)
        {
            if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static float Clamp01(this float value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static double Clamp01(this double value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static decimal Clamp01(this decimal value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static BigInteger Clamp01(this BigInteger value)
        {
            if (value < 0)
                return BigInteger.Zero;
            else if (value > 1)
                return BigInteger.One;
            else
                return value;
        }

        [WikiIgnore]
        public static BigDecimal Clamp01(this BigDecimal value)
        {
            if (value < 0)
                return BigDecimal.Zero;
            else if (value > 1)
                return BigDecimal.One;
            else
                return value;
        }

        [WikiIgnore]
        public static nint Clamp01(this nint value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }

        [WikiIgnore]
        public static nuint Clamp01(this nuint value)
        {
            if (value < 0)
                return 0;
            else if (value > 1)
                return 1;
            else
                return value;
        }
        #endregion

        #region Distance
        public static byte Distance(this byte a, byte b) => (byte)(a - b).Abs();

        [WikiIgnore] public static sbyte Distance(this sbyte a, sbyte b) => (sbyte)(a - b).Abs();

        [WikiIgnore] public static short Distance(this short a, short b) => (short)(a - b).Abs();

        [WikiIgnore] public static ushort Distance(this ushort a, ushort b) => (ushort)(a - b).Abs();

        [WikiIgnore] public static int Distance(this int a, int b) => (a - b).Abs();

        [WikiIgnore]
        public static uint Distance(this uint a, uint b)
        {
            if (b > a)
                return b - a;
            else
                return a - b;
        }

        [WikiIgnore] public static long Distance(this long a, long b) => (a - b).Abs();

        [WikiIgnore]
        public static ulong Distance(this ulong a, ulong b)
        {
            if (b > a)
                return b - a;
            else
                return a - b;
        }

        [WikiIgnore] public static float Distance(this float a, float b) => (a - b).Abs();

        [WikiIgnore] public static double Distance(this double a, double b) => (a - b).Abs();

        [WikiIgnore] public static decimal Distance(this decimal a, decimal b) => (a - b).Abs();

        [WikiIgnore] public static BigInteger Distance(this BigInteger a, BigInteger b) => (a - b).Abs();

        [WikiIgnore] public static BigDecimal Distance(this BigDecimal a, BigDecimal b) => (a - b).Abs();

        [WikiIgnore] public static nint Distance(this nint a, nint b) => (a - b).Abs();

        [WikiIgnore]
        public static nuint Distance(this nuint a, nuint b)
        {
            if (b > a)
                return b - a;
            else
                return a - b;
        }
        #endregion

        #region Reapeat
        public static sbyte Reapeat(this sbyte t, sbyte length) => (sbyte)(t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static byte Reapeat(this byte t, byte length) => (byte)(t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static short Reapeat(this short t, short length) => (short)(t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static ushort Reapeat(this ushort t, ushort length) => (ushort)(t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static int Reapeat(this int t, int length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static uint Reapeat(this uint t, uint length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static long Reapeat(this long t, long length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static ulong Reapeat(this ulong t, ulong length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static float Reapeat(this float t, float length) => (t - (t / length).Floor() * length).Clamp(0, length);
        [WikiIgnore] public static double Reapeat(this double t, double length) => (t - (t / length).Floor() * length).Clamp(0, length);
        [WikiIgnore] public static decimal Reapeat(this decimal t, decimal length) => (t - (t / length).Floor() * length).Clamp(0, length);
        [WikiIgnore] public static BigInteger Reapeat(this BigInteger t, BigInteger length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static BigDecimal Reapeat(this BigDecimal t, BigDecimal length) => (t - (t / length).Floor() * length).Clamp(0, length);
        [WikiIgnore] public static nint Reapeat(this nint t, nint length) => (t - (t / length) * length).Clamp(0, length);
        [WikiIgnore] public static nuint Reapeat(this nuint t, nuint length) => (t - (t / length) * length).Clamp(0, length);
        #endregion

        #region Lerp
        public static byte Lerp(this byte current, byte target, byte t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, sbyte t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, short t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, ushort t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, int t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, uint t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, long t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, ulong t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static float Lerp(this float current, float target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static double Lerp(this double current, double target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static decimal Lerp(this decimal current, decimal target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static BigInteger Lerp(this BigInteger current, BigInteger target, BigInteger t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static BigDecimal Lerp(this BigDecimal current, BigDecimal target, BigDecimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, double t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, decimal t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static Vector2 Lerp(this Vector2 current, Vector2 target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return new Vector2(current.x + (target.x - current.x) * t, current.y + (target.y - current.y) * t);
        }

        [WikiIgnore]
        public static Vector3 Lerp(this Vector3 current, Vector3 target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return new Vector3(current.x + (target.x - current.x) * t, current.y + (target.y - current.y) * t, current.z + (target.z - current.z) * t);
        }

        [WikiIgnore]
        public static Vector4 Lerp(this Vector4 current, Vector4 target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return new Vector4(current.x + (target.x - current.x) * t, current.y + (target.y - current.y) * t, current.z + (target.z - current.z) * t, current.w + (target.w - current.w) * t);
        }

        [WikiIgnore]
        public static Rect Lerp(this Rect current, Rect target, float t, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();
            return new Rect(current.x + (target.x - current.x) * t, current.y + (target.y - current.y) * t, current.width + (target.width - current.width) * t, current.height + (target.height - current.height) * t);
        }

        [WikiIgnore]
        public static Color Lerp(this Color current, Color target, float t, bool alpha = true, bool unclamped = false)
        {
            if (!unclamped)
                t = t.Clamp01();

            if (alpha)
                return new Color(current.r + (target.r - current.r) * t, current.g + (target.g - current.g) * t, current.b + (target.b - current.b) * t, current.a + (target.a - current.a) * t);
            else
                return new Color(current.r + (target.r - current.r) * t, current.g + (target.g - current.g) * t, current.b + (target.b - current.b) * t, current.a);
        }
        #endregion

        #region MoveTowards
        public static byte MoveTowards(this byte current, byte target, byte maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return (byte)(current + maxDelta);
        }

        [WikiIgnore]
        public static sbyte MoveTowards(this sbyte current, sbyte target, sbyte maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return (sbyte)(current + (target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static short MoveTowards(this short current, short target, short maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return (short)(current + (target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static ushort MoveTowards(this ushort current, ushort target, ushort maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return (ushort)(current + maxDelta);
        }

        [WikiIgnore]
        public static int MoveTowards(this int current, int target, int maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static uint MoveTowards(this uint current, uint target, uint maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return current + maxDelta;
        }

        [WikiIgnore]
        public static long MoveTowards(this long current, long target, long maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static ulong MoveTowards(this ulong current, ulong target, ulong maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return current + maxDelta;
        }

        [WikiIgnore]
        public static float MoveTowards(this float current, float target, float maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static double MoveTowards(this double current, double target, double maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static decimal MoveTowards(this decimal current, decimal target, decimal maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static BigInteger MoveTowards(this BigInteger current, BigInteger target, BigInteger maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static BigDecimal MoveTowards(this BigDecimal current, BigDecimal target, BigDecimal maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static nint MoveTowards(this nint current, nint target, nint maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + (target - current).Sign() * maxDelta;
        }

        [WikiIgnore]
        public static nuint MoveTowards(this nuint current, nuint target, nuint maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return current + maxDelta;
        }

        [WikiIgnore]
        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = num * num + num2 * num2;
            if (num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num4 = (float)Math.Sqrt(num3);
            return new Vector2(current.x + num / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta);
        }
        [WikiIgnore]
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            if (num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num5 = (float)Math.Sqrt(num4);
            return new Vector3(current.x + num / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
        }
        [WikiIgnore]
        public static Vector4 MoveTowards(this Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = target.w - current.w;
            float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Vector4(current.x + num / num6 * maxDistanceDelta, current.y + num2 / num6 * maxDistanceDelta, current.z + num3 / num6 * maxDistanceDelta, current.w + num4 / num6 * maxDistanceDelta);
        }
        [WikiIgnore]
        public static Rect MoveTowards(this Rect current, Rect target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.width - current.width;
            float num4 = target.height - current.height;
            float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Rect(current.x + num / num6 * maxDistanceDelta, current.y + num2 / num6 * maxDistanceDelta, current.width + num3 / num6 * maxDistanceDelta, current.height + num4 / num6 * maxDistanceDelta);
        }
        [WikiIgnore]
        public static Color MoveTowards(this Color current, Color target, float maxDistanceDelta)
        {
            float num = target.r - current.r;
            float num2 = target.g - current.g;
            float num3 = target.b - current.b;
            float num4 = target.a - current.a;
            float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Color(current.r + num / num6 * maxDistanceDelta, current.g + num2 / num6 * maxDistanceDelta, current.b + num3 / num6 * maxDistanceDelta, current.a + num4 / num6 * maxDistanceDelta);
        }
        #endregion

        #region Ceil
        public static float Ceil(this float value) => (float)Math.Ceiling(value);
        [WikiIgnore] public static double Ceil(this double value) => Math.Ceiling(value);
        [WikiIgnore] public static decimal Ceil(this decimal value) => Math.Ceiling(value);
        [WikiIgnore] public static BigDecimal Ceil(this BigDecimal value) => BigDecimal.Ceiling(value);

        public static int CeilToInt(this float value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static int CeilToInt(this double value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static int CeilToInt(this decimal value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static BigInteger CeilToInt(this BigDecimal value) => (BigInteger)BigDecimal.Ceiling(value);
        #endregion

        #region Floor
        public static float Floor(this float value) => (float)Math.Floor(value);
        [WikiIgnore] public static double Floor(this double value) => Math.Floor(value);
        [WikiIgnore] public static decimal Floor(this decimal value) => Math.Floor(value);
        [WikiIgnore] public static BigDecimal Floor(this BigDecimal value) => BigDecimal.Floor(value);

        public static int FloorToInt(this float value) => (int)Math.Floor(value);
        [WikiIgnore] public static int FloorToInt(this double value) => (int)Math.Floor(value);
        [WikiIgnore] public static int FloorToInt(this decimal value) => (int)Math.Floor(value);
        [WikiIgnore] public static BigInteger FloorToInt(this BigDecimal value) => (BigInteger)BigDecimal.Floor(value);
        #endregion

        #region Round
        public static float Round(this float value) => (float)Math.Round(value);
        [WikiIgnore] public static double Round(this double value) => Math.Round(value);
        [WikiIgnore] public static decimal Round(this decimal value) => Math.Round(value);
        [WikiIgnore] public static BigDecimal Round(this BigDecimal value) => BigDecimal.Round(value);

        public static int RoundToInt(this float value) => (int)Math.Round(value);
        [WikiIgnore] public static int RoundToInt(this double value) => (int)Math.Round(value);
        [WikiIgnore] public static int RoundToInt(this decimal value) => (int)Math.Round(value);
        [WikiIgnore] public static BigInteger RoundToInt(this BigDecimal value) => BigDecimal.Round(value);

        public static float Round(this float value, int digits) => (float)Math.Round(value, digits);
        [WikiIgnore] public static double Round(this double value, int digits) => Math.Round(value, digits);
        [WikiIgnore] public static decimal Round(this decimal value, int digits) => Math.Round(value, digits);
        #endregion

        #region Pow
        public static float Pow(this float x, float y) => (float)Math.Pow(x, y);
        public static double Pow(this double x, double y) => Math.Pow(x, y);
        public static decimal Pow(this decimal x, decimal y) => (decimal)Math.Pow((double)x, (double)y);
        #endregion

        #region Sqrt
        public static float Sqrt(this float value) => (float)Math.Sqrt(value);
        [WikiIgnore] public static double Sqrt(this double value) => Math.Sqrt(value);
        [WikiIgnore] public static decimal Sqrt(this decimal value) => (decimal)Math.Sqrt((double)value);
        #endregion

        #region Min
        public static sbyte Min(this sbyte a, sbyte b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static byte Min(this byte a, byte b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static short Min(this short a, short b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static ushort Min(this ushort a, ushort b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static int Min(this int a, int b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static uint Min(this uint a, uint b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static long Min(this long a, long b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static ulong Min(this ulong a, ulong b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static float Min(this float a, float b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static double Min(this double a, double b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static decimal Min(this decimal a, decimal b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static BigInteger Min(this BigInteger a, BigInteger b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static BigDecimal Min(this BigDecimal a, BigDecimal b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static nint Min(this nint a, nint b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static nuint Min(this nuint a, nuint b)
        {
            if (a < b)
                return a;
            else
                return b;
        }
        #endregion

        #region Min Array
        [WikiIgnore]
        public static sbyte Min(this sbyte value, params sbyte[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            sbyte num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static byte Min(this byte value, params byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            byte num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static short Min(this short value, params short[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            short num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static ushort Min(this ushort value, params ushort[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            ushort num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static int Min(this int value, params int[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            int num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static uint Min(this uint value, params uint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            uint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static long Min(this long value, params long[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            long num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static ulong Min(this ulong value, params ulong[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            ulong num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static float Min(this float value, params float[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            float num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static double Min(this double value, params double[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            double num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static decimal Min(this decimal value, params decimal[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            decimal num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static BigInteger Min(this BigInteger value, params BigInteger[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            BigInteger num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static BigDecimal Min(this BigDecimal value, params BigDecimal[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            BigDecimal num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static nint Min(this nint value, params nint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            nint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static nuint Min(this nuint value, params nuint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            nuint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] < num2)
                    num2 = values[i];
            }

            return num2;
        }
        #endregion

        #region Max
        public static sbyte Max(this sbyte a, sbyte b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static byte Max(this byte a, byte b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static short Max(this short a, short b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static ushort Max(this ushort a, ushort b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static int Max(this int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static uint Max(this uint a, uint b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static long Max(this long a, long b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static ulong Max(this ulong a, ulong b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static float Max(this float a, float b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static double Max(this double a, double b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static decimal Max(this decimal a, decimal b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static BigInteger Max(this BigInteger a, BigInteger b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static BigDecimal Max(this BigDecimal a, BigDecimal b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static nint Max(this nint a, nint b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        [WikiIgnore]
        public static nuint Max(this nuint a, nuint b)
        {
            if (a > b)
                return a;
            else
                return b;
        }
        #endregion

        #region Max Array
        [WikiIgnore]
        public static sbyte Max(this sbyte value, params sbyte[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            sbyte num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static byte Max(this byte value, params byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            byte num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static short Max(this short value, params short[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            short num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static ushort Max(this ushort value, params ushort[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            ushort num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static int Max(this int value, params int[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            int num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static uint Max(this uint value, params uint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            uint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static long Max(this long value, params long[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            long num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static ulong Max(this ulong value, params ulong[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            ulong num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static float Max(this float value, params float[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            float num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static double Max(this double value, params double[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            double num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static decimal Max(this decimal value, params decimal[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            decimal num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static BigInteger Max(this BigInteger value, params BigInteger[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            BigInteger num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static BigDecimal Max(this BigDecimal value, params BigDecimal[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            BigDecimal num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static nint Max(this nint value, params nint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            nint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }

        [WikiIgnore]
        public static nuint Max(this nuint value, params nuint[] values)
        {
            if (values == null)
                throw new ArgumentNullException();

            int length = values.Length;
            if (length == 0)
                return 0;

            nuint num2 = value;
            for (int i = 0; i < length; i++)
            {
                if (values[i] > num2)
                    num2 = values[i];
            }

            return num2;
        }
        #endregion

        #region Arithmetic Sequence Sum
        public static byte ArithmeticSequenceSum(this byte start, byte end) => (byte)((start.Distance(end) + 1) * (start + end) / 2);
        public static sbyte ArithmeticSequenceSum(this sbyte start, sbyte end) => (sbyte)((start.Distance(end) + 1) * (start + end) / 2);
        public static short ArithmeticSequenceSum(this short start, short end) => (short)((start.Distance(end) + 1) * (start + end) / 2);
        public static ushort ArithmeticSequenceSum(this ushort start, ushort end) => (ushort)((start.Distance(end) + 1) * (start + end) / 2);
        public static int ArithmeticSequenceSum(this int start, int end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static uint ArithmeticSequenceSum(this uint start, uint end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static long ArithmeticSequenceSum(this long start, long end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static ulong ArithmeticSequenceSum(this ulong start, ulong end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static float ArithmeticSequenceSum(this float start, float end) => (start.Distance(end) + 1) * (start + end) * 0.5f;
        public static double ArithmeticSequenceSum(this double start, double end) => (start.Distance(end) + 1) * (start + end) * 0.5;
        public static decimal ArithmeticSequenceSum(this decimal start, decimal end) => (start.Distance(end) + 1) * (start + end) * 0.5m;
        public static BigInteger ArithmeticSequenceSum(this BigInteger start, BigInteger end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static BigDecimal ArithmeticSequenceSum(this BigDecimal start, BigDecimal end) => (start.Distance(end) + 1) * (start + end) * 0.5;
        public static nint ArithmeticSequenceSum(this nint start, nint end) => (start.Distance(end) + 1) * (start + end) / 2;
        public static nuint ArithmeticSequenceSum(this nuint start, nuint end) => (start.Distance(end) + 1) * (start + end) / 2;
        #endregion
    }

    public static class ListTool
    {
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            T temp = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, temp);
        }

        public static void Change<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            T temp = list[newIndex];
            list[newIndex] = list[oldIndex];
            list[oldIndex] = temp;
        }

        #region Close Value
        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        public static byte CloseValue(this IEnumerable<byte> list, byte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target) < (y - target) ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static sbyte CloseValue(this IEnumerable<sbyte> list, sbyte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static short CloseValue(this IEnumerable<short> list, short target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ushort CloseValue(this IEnumerable<ushort> list, ushort target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target) < (y - target) ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValue(this IEnumerable<int> list, int target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static uint CloseValue(this IEnumerable<uint> list, uint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target) < (y - target) ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static long CloseValue(this IEnumerable<long> list, long target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ulong CloseValue(this IEnumerable<ulong> list, ulong target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target) < (y - target) ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static float CloseValue(this IEnumerable<float> list, float target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static double CloseValue(this IEnumerable<double> list, double target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static decimal CloseValue(this IEnumerable<decimal> list, decimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigInteger CloseValue(this IEnumerable<BigInteger> list, BigInteger target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigDecimal CloseValue(this IEnumerable<BigDecimal> list, BigDecimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nint CloseValue(this IEnumerable<nint> list, nint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target).Abs() < (y - target).Abs() ? x : y);

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nuint CloseValue(this IEnumerable<nuint> list, nuint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
                return list.Aggregate((x, y) => (x - target) < (y - target) ? x : y);

            return 0;
        }
        #endregion

        #region Close Value Get Number Func
        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        public static sbyte CloseValue<T>(this IEnumerable<T> list, sbyte target, Func<T, sbyte> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                sbyte val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    sbyte currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static byte CloseValue<T>(this IEnumerable<T> list, byte target, Func<T, byte> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                byte val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    byte currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static short CloseValue<T>(this IEnumerable<T> list, short target, Func<T, short> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                short val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    short currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ushort CloseValue<T>(this IEnumerable<T> list, ushort target, Func<T, ushort> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                ushort val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    ushort currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValue<T>(this IEnumerable<T> list, int target, Func<T, int> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                int val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    int currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static uint CloseValue<T>(this IEnumerable<T> list, uint target, Func<T, uint> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                uint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    uint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static long CloseValue<T>(this IEnumerable<T> list, long target, Func<T, long> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                long val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    long currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ulong CloseValue<T>(this IEnumerable<T> list, ulong target, Func<T, ulong> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                ulong val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    ulong currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static float CloseValue<T>(this IEnumerable<T> list, float target, Func<T, float> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                float val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    float currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static double CloseValue<T>(this IEnumerable<T> list, double target, Func<T, double> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                double val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    double currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static decimal CloseValue<T>(this IEnumerable<T> list, decimal target, Func<T, decimal> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                decimal val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    decimal currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigInteger CloseValue<T>(this IEnumerable<T> list, BigInteger target, Func<T, BigInteger> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                BigInteger val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    BigInteger currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigDecimal CloseValue<T>(this IEnumerable<T> list, BigDecimal target, Func<T, BigDecimal> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                BigDecimal val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    BigDecimal currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nint CloseValue<T>(this IEnumerable<T> list, nint target, Func<T, nint> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                nint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    nint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nuint CloseValue<T>(this IEnumerable<T> list, nuint target, Func<T, nuint> getNumberFunc)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                nuint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    nuint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }
        #endregion

        #region Close Value Predicate
        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        public static sbyte CloseValue<T>(this IEnumerable<T> list, sbyte target, Func<T, sbyte> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                sbyte val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    sbyte currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static byte CloseValue<T>(this IEnumerable<T> list, byte target, Func<T, byte> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                byte val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    byte currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static short CloseValue<T>(this IEnumerable<T> list, short target, Func<T, short> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                short val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    short currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ushort CloseValue<T>(this IEnumerable<T> list, ushort target, Func<T, ushort> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                ushort val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    ushort currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValue<T>(this IEnumerable<T> list, int target, Func<T, int> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                int val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    int currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static uint CloseValue<T>(this IEnumerable<T> list, uint target, Func<T, uint> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                uint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    uint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static long CloseValue<T>(this IEnumerable<T> list, long target, Func<T, long> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                long val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    long currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static ulong CloseValue<T>(this IEnumerable<T> list, ulong target, Func<T, ulong> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                ulong val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    ulong currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static float CloseValue<T>(this IEnumerable<T> list, float target, Func<T, float> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                float val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    float currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static double CloseValue<T>(this IEnumerable<T> list, double target, Func<T, double> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                double val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    double currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static decimal CloseValue<T>(this IEnumerable<T> list, decimal target, Func<T, decimal> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                decimal val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    decimal currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigInteger CloseValue<T>(this IEnumerable<T> list, BigInteger target, Func<T, BigInteger> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                BigInteger val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    BigInteger currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static BigDecimal CloseValue<T>(this IEnumerable<T> list, BigDecimal target, Func<T, BigDecimal> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                BigDecimal val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    BigDecimal currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nint CloseValue<T>(this IEnumerable<T> list, nint target, Func<T, nint> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                nint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    nint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target).Abs() < (currentNumber - target).Abs() ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾습니다
        /// </summary>
        /// <param name="list">리스트</param>
        /// <param name="target">기준</param>
        /// <param name="getNumberFunc">리스트에서 숫자를 가져올 함수</param>
        /// <returns></returns>
        [WikiIgnore]
        public static nuint CloseValue<T>(this IEnumerable<T> list, nuint target, Func<T, nuint> getNumberFunc, Predicate<T> predicate)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count() > 0)
            {
                using IEnumerator<T> enumerator = list.GetEnumerator();
                enumerator.MoveNext();

                bool setVal = false;
                nuint val = getNumberFunc(enumerator.Current);
                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                    {
                        if (!setVal)
                        {
                            val = getNumberFunc(enumerator.Current);
                            setVal = true;
                        }

                        continue;
                    }

                    nuint currentNumber = getNumberFunc(enumerator.Current);
                    val = (val - target) < (currentNumber - target) ? val : currentNumber;
                }

                return val;
            }

            return 0;
        }
        #endregion

        #region Close Value Index
        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int CloseValueIndex(this IList<byte> list, byte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<sbyte> list, sbyte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<short> list, short target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<ushort> list, ushort target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<int> list, int target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<uint> list, uint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<long> list, long target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<ulong> list, ulong target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<float> list, float target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<double> list, double target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<decimal> list, decimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<BigInteger> list, BigInteger target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<BigDecimal> list, BigDecimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<nint> list, nint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndex(this IList<nuint> list, nuint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.IndexOf(list.CloseValue(target));

            return 0;
        }
        #endregion

        #region Close Value Index Binary Search
        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int CloseValueIndexBinarySearch(this List<byte> list, byte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<sbyte> list, sbyte target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<short> list, short target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<ushort> list, ushort target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<int> list, int target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<uint> list, uint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<long> list, long target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<ulong> list, ulong target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<float> list, float target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<double> list, double target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<decimal> list, decimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<BigInteger> list, BigInteger target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<BigDecimal> list, BigDecimal target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<nint> list, nint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }

        /// <summary>
        /// 가장 가까운 수를 찾고 이진 검색으로 인덱스를 반환합니다
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static int CloseValueIndexBinarySearch(this List<nuint> list, nuint target)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count > 0)
                return list.BinarySearch(list.CloseValue(target));

            return 0;
        }
        #endregion

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Empty sequence");

                var comparer = Comparer<TKey>.Default;
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);

                while (sourceIterator.MoveNext())
                {
                    TSource current = sourceIterator.Current;
                    TKey currentKey = selector(current);

                    if (comparer.Compare(currentKey, minKey) >= 0)
                        continue;

                    min = current;
                    minKey = currentKey;
                }

                return min;
            }
        }

        #region Deduplicate
        public static void Deduplicate(this IList<float> values, float delta)
        {
            int index = 0;
            while (index < values.Count)
            {
                float value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values.RemoveAt(i);
                    else
                        i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<float> values, float delta, float setValue)
        {
            int index = 0;
            while (index < values.Count)
            {
                float value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values[i] = setValue;

                    i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<double> values, double delta)
        {
            int index = 0;
            while (index < values.Count)
            {
                double value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values.RemoveAt(i);
                    else
                        i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<double> values, double delta, double setValue)
        {
            int index = 0;
            while (index < values.Count)
            {
                double value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values[i] = setValue;

                    i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<decimal> values, decimal delta)
        {
            int index = 0;
            while (index < values.Count)
            {
                decimal value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values.RemoveAt(i);
                    else
                        i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<decimal> values, decimal delta, decimal setValue)
        {
            int index = 0;
            while (index < values.Count)
            {
                decimal value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values[i] = setValue;

                    i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<BigDecimal> values, BigDecimal delta)
        {
            int index = 0;
            while (index < values.Count)
            {
                BigDecimal value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values.RemoveAt(i);
                    else
                        i++;
                }

                index++;
            }
        }

        [WikiIgnore]
        public static void Deduplicate(this IList<BigDecimal> values, BigDecimal delta, BigDecimal setValue)
        {
            int index = 0;
            while (index < values.Count)
            {
                BigDecimal value = values[index];

                int i = index + 1;
                while (i < values.Count)
                {
                    if ((value - values[i]).Abs() < delta)
                        values[i] = setValue;

                    i++;
                }

                index++;
            }
        }
        #endregion
    }

    public static class StringTool
    {
        public static string ConstEnvironmentVariable(this string value)
        {
            if (value == null)
                return null;

            value = value.Replace("%DataPath%", Kernel.dataPath);
            value = value.Replace("%StreamingAssetsPath%", Kernel.streamingAssetsPath);
            value = value.Replace("%PersistentDataPath%", Kernel.persistentDataPath);

            value = value.Replace("%CompanyName%", Kernel.companyName);
            value = value.Replace("%ProductName%", Kernel.productName);
            value = value.Replace("%Version%", Kernel.version);

            return value;
        }

        /// <summary>
        /// (text = "AddSpacesToSentence") = "Add Spaces To Sentence"
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <param name="preserveAcronyms">약어(준말) 보존 (true = (UnscaledFPSDeltaTime = Unscaled FPS Delta Time), false = (UnscaledFPSDeltaTime = Unscaled FPSDelta Time))</param>
        /// <returns></returns>
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }

        #region KeyCode to String
        /// <summary>
        /// (keyCode = KeyCode.RightArrow) = "→"
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyCodeToString(this KeyCode keyCode, bool simply = false)
        {
            string text;
            switch (keyCode)
            {
                case KeyCode.Escape:
                    text = "ESC";
                    break;
                case KeyCode.Return when !simply:
                    text = "Enter";
                    break;
                case KeyCode.Return when simply:
                    text = "↵";
                    break;
                case KeyCode.Alpha0:
                    text = "0";
                    break;
                case KeyCode.Alpha1:
                    text = "1";
                    break;
                case KeyCode.Alpha2:
                    text = "2";
                    break;
                case KeyCode.Alpha3:
                    text = "3";
                    break;
                case KeyCode.Alpha4:
                    text = "4";
                    break;
                case KeyCode.Alpha5:
                    text = "5";
                    break;
                case KeyCode.Alpha6:
                    text = "6";
                    break;
                case KeyCode.Alpha7:
                    text = "7";
                    break;
                case KeyCode.Alpha8:
                    text = "8";
                    break;
                case KeyCode.Alpha9:
                    text = "9";
                    break;
                case KeyCode.AltGr when simply:
                    text = "AG";
                    break;
                case KeyCode.Ampersand:
                    text = "&";
                    break;
                case KeyCode.Asterisk:
                    text = "*";
                    break;
                case KeyCode.At:
                    text = "@";
                    break;
                case KeyCode.BackQuote:
                    text = "`";
                    break;
                case KeyCode.Backslash:
                    text = "\\";
                    break;
                case KeyCode.Caret:
                    text = "^";
                    break;
                case KeyCode.Colon:
                    text = ":";
                    break;
                case KeyCode.Comma:
                    text = ",";
                    break;
                case KeyCode.Dollar:
                    text = "$";
                    break;
                case KeyCode.DoubleQuote:
                    text = "\"";
                    break;
                case KeyCode.Equals:
                    text = "=";
                    break;
                case KeyCode.Exclaim:
                    text = "!";
                    break;
                case KeyCode.Greater:
                    text = ">";
                    break;
                case KeyCode.Hash:
                    text = "#";
                    break;
                case KeyCode.Keypad0 when !simply:
                    text = "Keypad 0";
                    break;
                case KeyCode.Keypad1 when !simply:
                    text = "Keypad 1";
                    break;
                case KeyCode.Keypad2 when !simply:
                    text = "Keypad 2";
                    break;
                case KeyCode.Keypad3 when !simply:
                    text = "Keypad 3";
                    break;
                case KeyCode.Keypad4 when !simply:
                    text = "Keypad 4";
                    break;
                case KeyCode.Keypad5 when !simply:
                    text = "Keypad 5";
                    break;
                case KeyCode.Keypad6 when !simply:
                    text = "Keypad 6";
                    break;
                case KeyCode.Keypad7 when !simply:
                    text = "Keypad 7";
                    break;
                case KeyCode.Keypad8 when !simply:
                    text = "Keypad 8";
                    break;
                case KeyCode.Keypad9 when !simply:
                    text = "Keypad 9";
                    break;
                case KeyCode.KeypadDivide when !simply:
                    text = "Keypad /";
                    break;
                case KeyCode.KeypadEnter when !simply:
                    text = "Keypad ↵";
                    break;
                case KeyCode.KeypadEquals when !simply:
                    text = "Keypad =";
                    break;
                case KeyCode.KeypadMinus when !simply:
                    text = "Keypad -";
                    break;
                case KeyCode.KeypadMultiply when !simply:
                    text = "Keypad *";
                    break;
                case KeyCode.KeypadPeriod when !simply:
                    text = "Keypad .";
                    break;
                case KeyCode.KeypadPlus when !simply:
                    text = "Keypad +";
                    break;
                case KeyCode.Keypad0 when simply:
                    text = "K0";
                    break;
                case KeyCode.Keypad1 when simply:
                    text = "K1";
                    break;
                case KeyCode.Keypad2 when simply:
                    text = "K2";
                    break;
                case KeyCode.Keypad3 when simply:
                    text = "K3";
                    break;
                case KeyCode.Keypad4 when simply:
                    text = "K4";
                    break;
                case KeyCode.Keypad5 when simply:
                    text = "K5";
                    break;
                case KeyCode.Keypad6 when simply:
                    text = "K6";
                    break;
                case KeyCode.Keypad7 when simply:
                    text = "K7";
                    break;
                case KeyCode.Keypad8 when simply:
                    text = "K8";
                    break;
                case KeyCode.Keypad9 when simply:
                    text = "K9";
                    break;
                case KeyCode.KeypadDivide when simply:
                    text = "K/";
                    break;
                case KeyCode.KeypadEnter when simply:
                    text = "K↵";
                    break;
                case KeyCode.KeypadEquals when simply:
                    text = "K=";
                    break;
                case KeyCode.KeypadMinus when simply:
                    text = "K-";
                    break;
                case KeyCode.KeypadMultiply when simply:
                    text = "K*";
                    break;
                case KeyCode.KeypadPeriod when simply:
                    text = "K.";
                    break;
                case KeyCode.KeypadPlus when simply:
                    text = "K+";
                    break;
                case KeyCode.LeftApple:
                    text = "Left Command";
                    break;
                case KeyCode.LeftBracket:
                    text = "[";
                    break;
                case KeyCode.LeftCurlyBracket:
                    text = "{";
                    break;
                case KeyCode.LeftParen:
                    text = "(";
                    break;
                case KeyCode.LeftWindows when simply:
                    text = "LW";
                    break;
                case KeyCode.Less:
                    text = "<";
                    break;
                case KeyCode.Minus:
                    text = "-";
                    break;
                case KeyCode.Mouse0 when !simply:
                    text = "Left Mouse";
                    break;
                case KeyCode.Mouse1 when !simply:
                    text = "Right Mouse";
                    break;
                case KeyCode.Mouse2 when !simply:
                    text = "Middle Mouse";
                    break;
                case KeyCode.Mouse0 when simply:
                    text = "LM";
                    break;
                case KeyCode.Mouse1 when simply:
                    text = "RM";
                    break;
                case KeyCode.Mouse2 when simply:
                    text = "MM";
                    break;
                case KeyCode.Mouse3 when simply:
                    text = "M3";
                    break;
                case KeyCode.Mouse4 when simply:
                    text = "M4";
                    break;
                case KeyCode.Mouse5 when simply:
                    text = "M5";
                    break;
                case KeyCode.Mouse6 when simply:
                    text = "M6";
                    break;
                case KeyCode.Percent:
                    text = "%";
                    break;
                case KeyCode.Period:
                    text = ".";
                    break;
                case KeyCode.Pipe:
                    text = "|";
                    break;
                case KeyCode.Plus:
                    text = "+";
                    break;
                case KeyCode.Print when !simply:
                    text = "Print Screen";
                    break;
                case KeyCode.Print when simply:
                    text = "PS";
                    break;
                case KeyCode.Question:
                    text = "?";
                    break;
                case KeyCode.Quote:
                    text = "'";
                    break;
                case KeyCode.RightApple:
                    text = "Right Command";
                    break;
                case KeyCode.RightBracket:
                    text = "]";
                    break;
                case KeyCode.RightCurlyBracket:
                    text = "}";
                    break;
                case KeyCode.RightParen:
                    text = ")";
                    break;
                case KeyCode.RightWindows when simply:
                    text = "LW";
                    break;
                case KeyCode.Semicolon:
                    text = ";";
                    break;
                case KeyCode.Slash:
                    text = "/";
                    break;
                case KeyCode.Space when simply:
                    text = "␣";
                    break;
                case KeyCode.SysReq when !simply:
                    text = "Print Screen";
                    break;
                case KeyCode.SysReq when simply:
                    text = "PS";
                    break;
                case KeyCode.Tilde:
                    text = "~";
                    break;
                case KeyCode.Underscore:
                    text = "_";
                    break;
                case KeyCode.UpArrow:
                    text = "↑";
                    break;
                case KeyCode.DownArrow:
                    text = "↓";
                    break;
                case KeyCode.LeftArrow:
                    text = "←";
                    break;
                case KeyCode.RightArrow:
                    text = "→";
                    break;
                case KeyCode.LeftControl when !simply:
                    text = "Left Ctrl";
                    break;
                case KeyCode.RightControl when !simply:
                    text = "Right Ctrl";
                    break;
                case KeyCode.LeftControl when simply:
                    text = "LC";
                    break;
                case KeyCode.RightControl when simply:
                    text = "RC";
                    break;
                case KeyCode.LeftAlt when simply:
                    text = "LA";
                    break;
                case KeyCode.RightAlt when simply:
                    text = "RA";
                    break;
                case KeyCode.LeftShift when simply:
                    text = "L⇧";
                    break;
                case KeyCode.RightShift when simply:
                    text = "R⇧";
                    break;
                case KeyCode.Backspace when simply:
                    text = "B←";
                    break;
                case KeyCode.Delete when simply:
                    text = "D←";
                    break;
                case KeyCode.PageUp when simply:
                    text = "P↑";
                    break;
                case KeyCode.PageDown when simply:
                    text = "P↓";
                    break;
                default:
                    text = keyCode.ToString().AddSpacesToSentence();
                    break;
            }

            return text;
        }
        #endregion

        #region To Bar
        /// <summary>
        /// (value = 5, max = 10, length = 10) = "■■■■■□□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToBar(this int value, int max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [WikiIgnore] public static string ToBar(this float value, float max, int length, string fill = "■", string half = "▣", string empty = "□") => ToBar((double)value, max, length, fill, half, empty);

        /// <summary>
        /// (value = 5.5, max = 10, length = 10) = "■■■■■▣□□□□"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToBar(this double value, double max, int length, string fill = "■", string half = "▣", string empty = "□")
        {
            if (fill == null)
                fill = "";
            if (half == null)
                half = "";
            if (empty == null)
                empty = "";

            string text = "";

            for (double i = 0.5; i < length + 0.5; i++)
            {
                if (value / max >= i / length)
                    text += fill;
                else
                {
                    if (value / max >= (i - 0.5) / length)
                        text += half;
                    else
                        text += empty;
                }
            }
            return text;
        }
        #endregion

        public static string DataSizeToString(this long byteSize) => ByteTo(byteSize, out string space) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 문자열로 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        /// <param name="digits">계산된 용량에서 표시할 소수점 자리수</param>
        /// <returns>계산된 용량</returns>
        [WikiIgnore] public static string DataSizeToString(this long byteSize, int digits) => ByteTo(byteSize, out string space).Round(digits) + space;

        /// <summary>
        /// 데이터 크기를(바이트) 적절하게 바꿔줍니다 (B, KB, MB, GB, TB, PB, EB, ZB, YB)
        /// </summary>
        /// <param name="byteSize">계산할 용량</param>
        public static double ByteTo(long byteSize, out string space)
        {
            int loopCount = 0;
            double size = byteSize;

            while (size > 1024.0)
            {
                size /= 1024.0;
                loopCount++;
            }

            if (loopCount == 0)
                space = "B";
            else if (loopCount == 1)
                space = "KB";
            else if (loopCount == 2)
                space = "MB";
            else if (loopCount == 3)
                space = "GB";
            else if (loopCount == 4)
                space = "TB";
            else if (loopCount == 5)
                space = "PB";
            else if (loopCount == 6)
                space = "EB";
            else if (loopCount == 7)
                space = "ZB";
            else
                space = "YB";

            return size;
        }

        [WikiIgnore]
        public static double ByteTo(long byteSize, DataSizeType dataSizeType, out string space)
        {
            double size = byteSize / Math.Pow(1024, (int)dataSizeType);
            space = dataSizeType.ToString().ToUpper();

            return size;
        }

        public enum DataSizeType
        {
            b,
            kb,
            mb,
            gb,
            tb,
            pb,
            eb,
            zb,
            yb
        }
    }

    public static class PathTool
    {
        public const string urlPathPrefix = "file://";

        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <returns></returns>
        public static string Combine(string path1, string path2) => Path.Combine(path1, path2).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <param name="path3">세번째 경로</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(string path1, string path2, string path3) => Path.Combine(path1, path2, path3).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="path1">첫번째 경로</param>
        /// <param name="path2">두번째 경로</param>
        /// <param name="path3">세번째 경로</param>
        /// <param name="path4">네번째 경로</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(string path1, string path2, string path3, string path4) => Path.Combine(path1, path2, path3, path4).Replace("\\", "/");
        /// <summary>
        /// (paths = ("asdf", "asdf")) = "asdf/asdf" (Path.Combine is "asdf\asdf")
        /// </summary>
        /// <param name="paths">경로들</param>
        /// <returns></returns>
        [WikiIgnore] public static string Combine(params string[] paths) => Path.Combine(paths).Replace("\\", "/");

        public static string RemoveInvalidPathChars(string filename) => string.Concat(filename.Split(Path.GetInvalidPathChars()));
        public static string ReplaceInvalidPathChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidPathChars()));

        public static string RemoveInvalidFileNameChars(string filename) => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        public static string ReplaceInvalidFileNameChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static string GetPathWithExtension(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension != "")
                return path.Remove(path.Length - extension.Length);
            else
                return path;
        }

        public static string UrlPathPrefix(this string path) => urlPathPrefix + path;
    }

    public static class DirectoryTool
    {
        public static void Copy(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                Copy(folder, dest);
            }
        }

        public static string[] GetFiles(string path, params string[] searchPatterns) => GetFiles(path, searchPatterns, SearchOption.TopDirectoryOnly);
        [WikiIgnore]
        public static string[] GetFiles(string path, string[] searchPatterns, SearchOption searchOption)
        {
            List<string> paths = new List<string>();
            for (int i = 0; i < searchPatterns.Length; i++)
            {
                string searchPattern = searchPatterns[i];
                paths.AddRange(Directory.GetFiles(path, searchPattern, searchOption));
            }

            return paths.ToArray();
        }
    }

    public static class TimeTool
    {
        #region To Time
        /// <summary>
        /// (second = 70) = "1:10"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        public static string ToTime(this int second, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            try
            {
                int secondAbs = second.Abs();
                if (second <= TimeSpan.MinValue.TotalSeconds)
                    return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss");
                else if (second > TimeSpan.MaxValue.TotalSeconds)
                    return TimeSpan.MaxValue.ToString(@"d\:h\:mm\:ss");
                else if (secondAbs >= 86400 || dayAlwayShow)
                    return TimeSpan.FromSeconds(second).ToString(@"d\:hh\:mm\:ss");
                else if (secondAbs >= 3600 || hourAlwayShow)
                    return TimeSpan.FromSeconds(second).ToString(@"h\:mm\:ss");
                else if (secondAbs >= 60 || minuteAlwayShow)
                    return TimeSpan.FromSeconds(second).ToString(@"m\:ss");
                else
                    return TimeSpan.FromSeconds(second).ToString(@"s");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "--:--";
            }
        }

        /// <summary>
        /// (second = 70.1f) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        [WikiIgnore] public static string ToTime(this float second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false) => ToTime((double)second, decimalShow, minuteAlwayShow, hourAlwayShow, dayAlwayShow);

        /// <summary>
        /// (second = 70.1) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToTime(this double second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            try
            {
                double secondAbs = second.Abs();
                if (decimalShow)
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds || secondAbs == float.NegativeInfinity)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (second > TimeSpan.MaxValue.TotalSeconds || secondAbs == float.PositiveInfinity)
                        return TimeSpan.FromSeconds(TimeSpan.MaxValue.TotalSeconds).ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"h\:mm\:ss\.ff");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"m\:ss\.ff");
                    else
                        return TimeSpan.FromSeconds(second).ToString(@"s\.ff");
                }
                else
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds || secondAbs == float.NegativeInfinity)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss");
                    else if (second > TimeSpan.MaxValue.TotalSeconds || secondAbs == float.PositiveInfinity)
                        return TimeSpan.FromSeconds(TimeSpan.MaxValue.TotalSeconds).ToString(@"d\:h\:mm\:ss");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"d\:hh\:mm\:ss");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"h\:mm\:ss");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return TimeSpan.FromSeconds(second).ToString(@"m\:ss");
                    else
                        return TimeSpan.FromSeconds(second).ToString(@"s");
                }
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return "--:--";
            }
        }
        #endregion

        static readonly KoreanLunisolarCalendar klc = new KoreanLunisolarCalendar();
        public static DateTime ToLunarDate(this DateTime dateTime)
        {
            int year = klc.GetYear(dateTime);
            int month = klc.GetMonth(dateTime);
            int day = klc.GetDayOfMonth(dateTime);

            //1년이 12이상이면 윤달이 있음..
            if (klc.GetMonthsInYear(year) > 12)
            {
                //년도의 윤달이 몇월인지?
                int leapMonth = klc.GetLeapMonth(year);

                //달이 윤월보다 같거나 크면 -1을 함 즉 윤8은->9 이기때문
                if (month >= leapMonth)
                    month--;
            }

            return new DateTime(year, month, day);
        }

        public static DateTime ToSolarDate(this DateTime dateTime, bool isLeapMonth = false)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;

            if (klc.GetMonthsInYear(year) > 12)
            {
                int leapMonth = klc.GetLeapMonth(year);

                if (month > leapMonth - 1)
                    month++;
                else if (month == leapMonth - 1 && isLeapMonth)
                    month++;
            }

            return klc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
    }

    public static class ComponentTool
    {
        public static T GetComponentFieldSave<T>(this Component component, T fieldToSave, GetComponentMode mode = GetComponentMode.addIfNull) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponent<T>();
                if (fieldToSave == null)
                {
                    if (mode == GetComponentMode.addIfNull)
                        return component.gameObject.AddComponent<T>();
                    else if (mode == GetComponentMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsFieldSave<T>(this Component component, T[] fieldToSave, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponents<T>();
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }



        public static T GetComponentInParentFieldSave<T>(this Component component, T fieldToSave, bool includeInactive = false, GetComponentInMode mode = GetComponentInMode.none) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponentInParent<T>(includeInactive);

                if (fieldToSave == null && mode == GetComponentInMode.destroyIfNull)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                    return null;
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsInParentFieldSave<T>(this Component component, T[] fieldToSave, bool includeInactive = false, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponentsInParent<T>(includeInactive);
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }



        public static T GetComponentInChildrenFieldSave<T>(this Component component, T fieldToSave, bool includeInactive = false, GetComponentInMode mode = GetComponentInMode.none) where T : Component
        {
            if (fieldToSave == null || fieldToSave.gameObject != component.gameObject)
            {
                fieldToSave = component.GetComponentInChildren<T>(includeInactive);
                if (fieldToSave == null && mode == GetComponentInMode.destroyIfNull)
                {
                    UnityEngine.Object.DestroyImmediate(component);
                    return null;
                }
            }

            return fieldToSave;
        }

        public static T[] GetComponentsInChildrenFieldSave<T>(this Component component, T[] fieldToSave, bool includeInactive = false, GetComponentsMode mode = GetComponentsMode.addZeroLengthIfNull)
        {
            if (fieldToSave == null)
            {
                fieldToSave = component.GetComponentsInChildren<T>(includeInactive);
                if (fieldToSave == null)
                {
                    if (mode == GetComponentsMode.addZeroLengthIfNull)
                        return new T[0];
                    else if (mode == GetComponentsMode.destroyIfNull)
                    {
                        UnityEngine.Object.DestroyImmediate(component);
                        return null;
                    }
                }
            }

            return fieldToSave;
        }

        public enum GetComponentMode
        {
            none,
            addIfNull,
            destroyIfNull
        }

        public enum GetComponentInMode
        {
            none,
            destroyIfNull
        }

        public enum GetComponentsMode
        {
            none,
            addZeroLengthIfNull,
            destroyIfNull
        }
    }
}