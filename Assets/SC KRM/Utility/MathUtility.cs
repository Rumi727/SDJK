using ExtendedNumerics;
using System.Numerics;
using System;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace SCKRM
{
    public static class MathUtility
    {
        const int maxRoundingDigits = 15;
        static float[] roundPower10Float = new float[16]
        {
            1E+0f, 1E+1f, 1E+2f, 1E+3f, 1E+4f, 1E+5f, 1E+6f, 1E+7f, 1E+8f, 1E+9f,
            1E+10f, 1E+11f, 1E+12f, 1E+13f, 1E+14f, 1E+15f
        };

        static double[] roundPower10Double = new double[16]
        {
            1E+0, 1E+1, 1E+2, 1E+3, 1E+4, 1E+5, 1E+6, 1E+7, 1E+8, 1E+9,
            1E+10, 1E+11, 1E+12, 1E+13, 1E+14, 1E+15
        };

        static decimal[] roundPower10Decimal = new decimal[16]
        {
            1E+0m, 1E+1m, 1E+2m, 1E+3m, 1E+4m, 1E+5m, 1E+6m, 1E+7m, 1E+8m, 1E+9m,
            1E+10m, 1E+11m, 1E+12m, 1E+13m, 1E+14m, 1E+15m
        };

        static BigDecimal[] roundPower10BigDecimal = new BigDecimal[16]
        {
            1E+0, 1E+1, 1E+2, 1E+3, 1E+4, 1E+5, 1E+6, 1E+7, 1E+8, 1E+9,
            1E+10, 1E+11, 1E+12, 1E+13, 1E+14, 1E+15
        };

        public const float pi = (float)Math.PI;
        public const float e = (float)Math.E;
        public const float deg2Rad = pi / 180;
        public const float rad2Deg = 180 / pi;

        public const double piDouble = Math.PI;
        public const double eDouble = Math.E;
        public const double deg2RadDouble = piDouble / 180;
        public const double rad2DegDouble = 180 / piDouble;

        public const float epsilonFloatWithAccuracy = 0.0001f;

        /*public static void FunctionList()
        {
            Abs();
            Acos();
            Acosh();
            Approximately(); //Mathf
            Asin();
            Asinh();
            Atan();
            Atan2();
            Atanh();
            Cbrt();
            Ceiling(); //Ceil
            Clamp();
            Clamp01(); //Mathf
            ClosestPowerOfTwo(); //Mathf
            CorrelatedColorTemperatureToRGB(); //Mathf
            Cos();
            Cosh();
            DeltaAngle(); //Mathf
            Exp();
            Floor();
            Gamma(); //Mathf
            GammaToLinearSpace(); //Mathf
            IEEERemainder();
            InverseLerp(); //Mathf
            IsPowerOfTwo(); //Mathf
            Lerp(); //Mathf
            LerpAngle(); //Mathf
            LinearToGammaSpace(); //Mathf
            Log();
            Log10();
            Max();
            Min();
            MoveTowards(); //Mathf
            MoveTowardsAngle(); //Mathf
            NextPowerOfTwo(); //Mathf
            PerlinNoise(); //Mathf
            PingPong(); //Mathf
            Pow();
            Repeat(); //Mathf
            Round();
            Sign();
            Sin();
            Sinh();
            SmoothDamp(); //Mathf
            SmoothDampAngle(); //Mathf
            SmoothStep(); //Mathf
            Sqrt();
            Tan();
            Tanh();
            Truncate();
        }*/

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

        #region Acos
        public static float Acos(this float value) => (float)Math.Acos(value);
        [WikiIgnore] public static double Acos(this double value) => Math.Acos(value);
        #endregion

        #region Acosh
        public static float Acosh(this float value) => (float)Math.Acosh(value);
        [WikiIgnore] public static double Acosh(this double value) => Math.Acosh(value);
        #endregion

        #region Arithmetic Sequence Sum
        public static sbyte ArithmeticSequenceSum(this sbyte start, sbyte end) => (sbyte)((start.Distance(end) + 1) * (start + end) / 2);
        [WikiIgnore] public static byte ArithmeticSequenceSum(this byte start, byte end) => (byte)((start.Distance(end) + 1) * (start + end) / 2);
        [WikiIgnore] public static short ArithmeticSequenceSum(this short start, short end) => (short)((start.Distance(end) + 1) * (start + end) / 2);
        [WikiIgnore] public static ushort ArithmeticSequenceSum(this ushort start, ushort end) => (ushort)((start.Distance(end) + 1) * (start + end) / 2);
        [WikiIgnore] public static int ArithmeticSequenceSum(this int start, int end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static uint ArithmeticSequenceSum(this uint start, uint end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static long ArithmeticSequenceSum(this long start, long end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static ulong ArithmeticSequenceSum(this ulong start, ulong end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static float ArithmeticSequenceSum(this float start, float end) => (start.Distance(end) + 1) * (start + end) * 0.5f;
        [WikiIgnore] public static double ArithmeticSequenceSum(this double start, double end) => (start.Distance(end) + 1) * (start + end) * 0.5;
        [WikiIgnore] public static decimal ArithmeticSequenceSum(this decimal start, decimal end) => (start.Distance(end) + 1) * (start + end) * 0.5m;
        [WikiIgnore] public static BigInteger ArithmeticSequenceSum(this BigInteger start, BigInteger end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static BigDecimal ArithmeticSequenceSum(this BigDecimal start, BigDecimal end) => (start.Distance(end) + 1) * (start + end) * 0.5;
        [WikiIgnore] public static nint ArithmeticSequenceSum(this nint start, nint end) => (start.Distance(end) + 1) * (start + end) / 2;
        [WikiIgnore] public static nuint ArithmeticSequenceSum(this nuint start, nuint end) => (start.Distance(end) + 1) * (start + end) / 2;
        #endregion

        public static float Angle(this Vector2 current, Vector2 target)
        {
            Vector2 dir = target - current;
            return Atan2(dir.y, dir.x) * rad2Deg;
        }

        #region Approximately
        public static bool Approximately(this float a, float b) => (b - a).Abs() < Max(1E-06f * Max(a.Abs(), b.Abs()), float.Epsilon * 8f);
        [WikiIgnore] public static bool Approximately(this double a, double b) => (b - a).Abs() < Max(1E-06d * Max(a.Abs(), b.Abs()), double.Epsilon * 8f);
        #endregion

        #region Asin
        public static float Asin(this float value) => (float)Math.Asin(value);
        [WikiIgnore] public static double Asin(this double value) => Math.Asin(value);
        #endregion

        #region Asinh
        public static float Asinh(this float value) => (float)Math.Asinh(value);
        [WikiIgnore] public static double Asinh(this double value) => Math.Asinh(value);
        #endregion

        #region Atan
        public static float Atan(this float value) => (float)Math.Atan(value);
        [WikiIgnore] public static double Atan(this double value) => Math.Atan(value);
        #endregion

        #region Atan2
        public static float Atan2(this float y, float x) => (float)Math.Atan2(y, x);
        [WikiIgnore] public static double Atan2(this double y, double x) => Math.Atan2(y, x);
        #endregion

        #region Atanh
        public static float Atanh(this float value) => (float)Math.Atanh(value);
        [WikiIgnore] public static double Atanh(this double value) => Math.Atanh(value);
        #endregion

        #region Cbrt
        public static float Cbrt(this float value) => (float)Math.Cbrt(value);
        [WikiIgnore] public static double Cbrt(this double value) => Math.Cbrt(value);
        #endregion

        #region Ceil
        public static float Ceil(this float value) => (float)Math.Ceiling(value);
        [WikiIgnore] public static double Ceil(this double value) => Math.Ceiling(value);
        [WikiIgnore] public static decimal Ceil(this decimal value) => Math.Ceiling(value);
        [WikiIgnore] public static BigDecimal Ceil(this BigDecimal value) => BigDecimal.Ceiling(value);

        [WikiIgnore]
        public static float Ceil(this float value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            float num = roundPower10Float[digits];
            return (value * num).Ceil() / num;
        }
        [WikiIgnore]
        public static double Ceil(this double value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            double num = roundPower10Double[digits];
            return (value * num).Ceil() / num;
        }
        [WikiIgnore]
        public static decimal Ceil(this decimal value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            decimal num = roundPower10Decimal[digits];
            return (value * num).Ceil() / num;
        }
        [WikiIgnore]
        public static BigDecimal Ceil(this BigDecimal value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            BigDecimal num = roundPower10BigDecimal[digits];
            return (value * num).Ceil() / num;
        }

        public static int CeilToInt(this float value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static int CeilToInt(this double value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static int CeilToInt(this decimal value) => (int)Math.Ceiling(value);
        [WikiIgnore] public static BigInteger CeilToInt(this BigDecimal value) => (BigInteger)BigDecimal.Ceiling(value);
        #endregion

        #region Clamp
        public static sbyte Clamp(this sbyte value, sbyte min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static byte Clamp(this byte value, byte min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static byte Clamp(this byte value, byte min, byte max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static short Clamp(this short value, short min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static short Clamp(this short value, short min, short max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static ushort Clamp(this ushort value, ushort min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static ushort Clamp(this ushort value, ushort min, ushort max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static int Clamp(this int value, int min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static uint Clamp(this uint value, uint min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static uint Clamp(this uint value, uint min, uint max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static long Clamp(this long value, long min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static long Clamp(this long value, long min, long max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static ulong Clamp(this ulong value, ulong min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        [WikiIgnore]
        public static float Clamp(this float value, float min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static float Clamp(this float value, float min, float max)
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
        public static double Clamp(this double value, double min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static double Clamp(this double value, double min, double max)
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
        public static decimal Clamp(this decimal value, decimal min)
        {
            if (value < min)
                return min;
            else
                return value;
        }

        [WikiIgnore]
        public static decimal Clamp(this decimal value, decimal min, decimal max)
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

        [WikiIgnore] public static Vector2 Clamp(this Vector2 value, Vector2 min) => new Vector2(value.x.Clamp(min.x), value.y.Clamp(min.y));
        [WikiIgnore] public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max) => new Vector2(value.x.Clamp(min.x, max.x), value.y.Clamp(min.y, max.y));
        [WikiIgnore] public static Vector3 Clamp(this Vector3 value, Vector3 min) => new Vector3(value.x.Clamp(min.x), value.y.Clamp(min.y), value.z.Clamp(min.z));
        [WikiIgnore] public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max) => new Vector3(value.x.Clamp(min.x, max.x), value.y.Clamp(min.y, max.y), value.z.Clamp(min.z, max.z));
        [WikiIgnore] public static Vector4 Clamp(this Vector4 value, Vector4 min) => new Vector4(value.x.Clamp(min.x), value.y.Clamp(min.y), value.z.Clamp(min.z), value.w.Clamp(min.w));
        [WikiIgnore] public static Vector4 Clamp(this Vector4 value, Vector4 min, Vector4 max) => new Vector4(value.x.Clamp(min.x, max.x), value.y.Clamp(min.y, max.y), value.z.Clamp(min.z, max.z), value.w.Clamp(min.w, max.w));
        [WikiIgnore] public static Rect Clamp(this Rect value, Rect min) => new Rect(value.x.Clamp(min.x), value.y.Clamp(min.y), value.width.Clamp(min.width), value.height.Clamp(min.height));
        [WikiIgnore] public static Rect Clamp(this Rect value, Rect min, Rect max) => new Rect(value.x.Clamp(min.x, max.x), value.y.Clamp(min.y, max.y), value.width.Clamp(min.width, max.width), value.height.Clamp(min.height, max.height));
        [WikiIgnore] public static Color Clamp(this Color value, Color min) => new Color(value.r.Clamp(min.r), value.g.Clamp(min.g), value.b.Clamp(min.b), value.a.Clamp(min.a));
        [WikiIgnore] public static Color Clamp(this Color value, Color min, Color max) => new Color(value.r.Clamp(min.r, max.r), value.g.Clamp(min.g, max.g), value.b.Clamp(min.b, max.b), value.a.Clamp(min.a, max.a));
        #endregion

        #region Clamp01
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
        public static byte Clamp01(this byte value)
        {
            if (value > 1)
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

        [WikiIgnore] public static Vector2 Clamp01(this Vector2 value) => new Vector2(value.x.Clamp01(), value.y.Clamp01());
        [WikiIgnore] public static Vector3 Clamp01(this Vector3 value) => new Vector3(value.x.Clamp01(), value.y.Clamp01(), value.z.Clamp01());
        [WikiIgnore] public static Vector4 Clamp01(this Vector4 value) => new Vector4(value.x.Clamp01(), value.y.Clamp01(), value.z.Clamp01(), value.w.Clamp01());
        [WikiIgnore] public static Rect Clamp01(this Rect value) => new Rect(value.x.Clamp01(), value.y.Clamp01(), value.width.Clamp01(), value.height.Clamp01());
        [WikiIgnore] public static Color Clamp01(this Color value) => new Color(value.r.Clamp01(), value.g.Clamp01(), value.b.Clamp01(), value.a.Clamp01());
        #endregion

        public static int ClosestPowerOfTwo(this int value) => Mathf.ClosestPowerOfTwo(value);

        public static Color CorrelatedColorTemperatureToRGB(this float value) => Mathf.CorrelatedColorTemperatureToRGB(value);

        #region Cos
        public static float Cos(this float value) => (float)Math.Cos(value);
        [WikiIgnore] public static double Cos(this double value) => Math.Cos(value);
        #endregion

        #region Cosh
        public static float Cosh(this float value) => (float)Math.Cosh(value);
        [WikiIgnore] public static double Cosh(this double value) => Math.Cosh(value);
        #endregion

        #region Delta Angle
        public static short DeltaAngle(this short value, short target)
        {
            short result = (short)(target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static ushort DeltaAngle(this ushort value, ushort target)
        {
            ushort result = (ushort)(target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static int DeltaAngle(this int value, int target)
        {
            int result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static uint DeltaAngle(this uint value, uint target)
        {
            uint result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static long DeltaAngle(this long value, long target)
        {
            long result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static ulong DeltaAngle(this ulong value, ulong target)
        {
            ulong result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static float DeltaAngle(this float value, float target)
        {
            float result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static double DeltaAngle(this double value, double target)
        {
            double result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static decimal DeltaAngle(this decimal value, decimal target)
        {
            decimal result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static BigInteger DeltaAngle(this BigInteger value, BigInteger target)
        {
            BigInteger result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static BigDecimal DeltaAngle(this BigDecimal value, BigDecimal target)
        {
            BigDecimal result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static nint DeltaAngle(this nint value, nint target)
        {
            nint result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }

        [WikiIgnore]
        public static nuint DeltaAngle(this nuint value, nuint target)
        {
            nuint result = (target - value).Repeat(360);
            if (result > 180)
                result -= 360;

            return result;
        }
        #endregion

        #region Distance
        public static sbyte Distance(this sbyte a, sbyte b) => (sbyte)(a - b).Abs();

        [WikiIgnore] public static byte Distance(this byte a, byte b) => (byte)(a - b).Abs();

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

        #region Exp
        public static float Exp(this float value) => (float)Math.Exp(value);
        [WikiIgnore] public static double Exp(this double value) => Math.Exp(value);
        [WikiIgnore] public static BigDecimal Exp(this BigInteger value) => BigDecimal.Exp(value);
        #endregion

        #region Floor
        public static float Floor(this float value) => (float)Math.Floor(value);
        [WikiIgnore] public static double Floor(this double value) => Math.Floor(value);
        [WikiIgnore] public static decimal Floor(this decimal value) => Math.Floor(value);
        [WikiIgnore] public static BigDecimal Floor(this BigDecimal value) => BigDecimal.Floor(value);

        [WikiIgnore]
        public static float Floor(this float value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            float num = roundPower10Float[digits];
            return (value * num).Floor() / num;
        }
        [WikiIgnore]
        public static double Floor(this double value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            double num = roundPower10Double[digits];
            return (value * num).Floor() / num;
        }
        [WikiIgnore]
        public static decimal Floor(this decimal value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            decimal num = roundPower10Decimal[digits];
            return (value * num).Floor() / num;
        }
        [WikiIgnore]
        public static BigDecimal Floor(this BigDecimal value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            BigDecimal num = roundPower10BigDecimal[digits];
            return (value * num).Floor() / num;
        }

        public static int FloorToInt(this float value) => (int)Math.Floor(value);
        [WikiIgnore] public static int FloorToInt(this double value) => (int)Math.Floor(value);
        [WikiIgnore] public static int FloorToInt(this decimal value) => (int)Math.Floor(value);
        [WikiIgnore] public static BigInteger FloorToInt(this BigDecimal value) => (BigInteger)BigDecimal.Floor(value);
        #endregion

        #region Gamma
        public static float Gamma(this float value, float absMax, float gamma)
        {
            bool flag = value < 0;
            float temp = value.Abs();

            if (temp > absMax)
            {
                if (flag)
                    return -temp;
                else
                    return temp;
            }

            float result = temp / absMax.Pow(gamma) * absMax;
            if (flag)
                return -result;
            else
                return result;
        }

        [WikiIgnore]
        public static double Gamma(this double value, double absMax, double gamma)
        {
            bool flag = value < 0;
            double temp = value.Abs();

            if (temp > absMax)
            {
                if (flag)
                    return -temp;
                else
                    return temp;
            }

            double result = temp / absMax.Pow(gamma) * absMax;
            if (flag)
                return -result;
            else
                return result;
        }
        #endregion

        public static float GammaToLinearSpace(this float value) => Mathf.GammaToLinearSpace(value);

        #region IEEE Remainder
        public static float IEEERemainder(this float x, float y) => (float)Math.IEEERemainder(x, y);
        [WikiIgnore] public static double IEEERemainder(this double x, double y) => Math.IEEERemainder(x, y);
        #endregion

        #region Inverse Lerp
        public static sbyte InverseLerp(this sbyte a, sbyte b, sbyte t)
        {
            if (a != b)
                return (sbyte)((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static byte InverseLerp(this byte a, byte b, byte t)
        {
            if (a != b)
                return (byte)((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static short InverseLerp(this short a, short b, short t)
        {
            if (a != b)
                return (short)((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static ushort InverseLerp(this ushort a, ushort b, ushort t)
        {
            if (a != b)
                return (ushort)((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static int InverseLerp(this int a, int b, int t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static uint InverseLerp(this uint a, uint b, uint t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static long InverseLerp(this long a, long b, long t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static ulong InverseLerp(this ulong a, ulong b, ulong t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static float InverseLerp(this float a, float b, float t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static double InverseLerp(this double a, double b, double t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static decimal InverseLerp(this decimal a, decimal b, decimal t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static BigInteger InverseLerp(this BigInteger a, BigInteger b, BigInteger t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }

        [WikiIgnore]
        public static BigDecimal InverseLerp(this BigDecimal a, BigDecimal b, BigDecimal t)
        {
            if (a != b)
                return ((t - a) / (b - a)).Clamp01();

            return 0;
        }
        #endregion

        #region Inverse Lerp Unclamped
        public static sbyte InverseLerpUnclamped(this sbyte a, sbyte b, sbyte t)
        {
            if (a != b)
                return (sbyte)((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static byte InverseLerpUnclamped(this byte a, byte b, byte t)
        {
            if (a != b)
                return (byte)((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static short InverseLerpUnclamped(this short a, short b, short t)
        {
            if (a != b)
                return (short)((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static ushort InverseLerpUnclamped(this ushort a, ushort b, ushort t)
        {
            if (a != b)
                return (ushort)((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static int InverseLerpUnclamped(this int a, int b, int t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static uint InverseLerpUnclamped(this uint a, uint b, uint t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static long InverseLerpUnclamped(this long a, long b, long t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static ulong InverseLerpUnclamped(this ulong a, ulong b, ulong t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static float InverseLerpUnclamped(this float a, float b, float t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static double InverseLerpUnclamped(this double a, double b, double t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static decimal InverseLerpUnclamped(this decimal a, decimal b, decimal t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static BigInteger InverseLerpUnclamped(this BigInteger a, BigInteger b, BigInteger t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }

        [WikiIgnore]
        public static BigDecimal InverseLerpUnclamped(this BigDecimal a, BigDecimal b, BigDecimal t)
        {
            if (a != b)
                return ((t - a) / (b - a));

            return 0;
        }
        #endregion

        public static bool IsPowerOfTwo(this int value) => Mathf.IsPowerOfTwo(value);

        #region Lerp
        public static sbyte Lerp(this sbyte current, sbyte target, sbyte t)
        {
            t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, float t)
        {
            t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, double t)
        {
            t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, decimal t)
        {
            t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static sbyte Lerp(this sbyte current, sbyte target, BigDecimal t)
        {
            t = t.Clamp01();
            return (sbyte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, byte t)
        {
            t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, float t)
        {
            t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, double t)
        {
            t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, decimal t)
        {
            t = t.Clamp01();
            return (byte)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static byte Lerp(this byte current, byte target, BigDecimal t)
        {
            t = t.Clamp01();
            return (byte)(decimal)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, short t)
        {
            t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, float t)
        {
            t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, double t)
        {
            t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, decimal t)
        {
            t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static short Lerp(this short current, short target, BigDecimal t)
        {
            t = t.Clamp01();
            return (short)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, ushort t)
        {
            t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, float t)
        {
            t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, double t)
        {
            t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, decimal t)
        {
            t = t.Clamp01();
            return (ushort)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ushort Lerp(this ushort current, ushort target, BigDecimal t)
        {
            t = t.Clamp01();
            return (ushort)(decimal)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, int t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, float t)
        {
            t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, double t)
        {
            t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, decimal t)
        {
            t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static int Lerp(this int current, int target, BigDecimal t)
        {
            t = t.Clamp01();
            return (int)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, uint t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, float t)
        {
            t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, double t)
        {
            t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, decimal t)
        {
            t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static uint Lerp(this uint current, uint target, BigDecimal t)
        {
            t = t.Clamp01();
            return (uint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, long t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, float t)
        {
            t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, double t)
        {
            t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, decimal t)
        {
            t = t.Clamp01();
            return (long)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static long Lerp(this long current, long target, BigDecimal t)
        {
            t = t.Clamp01();
            return (long)(decimal)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, ulong t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, float t)
        {
            t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, double t)
        {
            t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, decimal t)
        {
            t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static ulong Lerp(this ulong current, ulong target, BigDecimal t)
        {
            t = t.Clamp01();
            return (ulong)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static float Lerp(this float current, float target, float t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static double Lerp(this double current, double target, double t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static decimal Lerp(this decimal current, decimal target, decimal t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static BigInteger Lerp(this BigInteger current, BigInteger target, BigInteger t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static BigDecimal Lerp(this BigDecimal current, BigDecimal target, BigDecimal t)
        {
            t = t.Clamp01();
            return ((1 - t) * current) + (target * t);
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, float t)
        {
            t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, double t)
        {
            t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nint Lerp(this nint current, nint target, decimal t)
        {
            t = t.Clamp01();
            return (nint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, float t)
        {
            t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, double t)
        {
            t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static nuint Lerp(this nuint current, nuint target, decimal t)
        {
            t = t.Clamp01();
            return (nuint)(((1 - t) * current) + (target * t));
        }

        [WikiIgnore]
        public static Vector2 Lerp(this Vector2 current, Vector2 target, float t)
        {
            t = t.Clamp01();
            return new Vector2(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t));
        }

        [WikiIgnore]
        public static Vector3 Lerp(this Vector3 current, Vector3 target, float t)
        {
            t = t.Clamp01();
            return new Vector3(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.z + ((target.z - current.z) * t));
        }

        [WikiIgnore]
        public static Vector4 Lerp(this Vector4 current, Vector4 target, float t)
        {
            t = t.Clamp01();
            return new Vector4(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.z + ((target.z - current.z) * t), current.w + ((target.w - current.w) * t));
        }

        [WikiIgnore]
        public static Rect Lerp(this Rect current, Rect target, float t)
        {
            t = t.Clamp01();
            return new Rect(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.width + ((target.width - current.width) * t), current.height + ((target.height - current.height) * t));
        }

        [WikiIgnore]
        public static Color Lerp(this Color current, Color target, float t)
        {
            t = t.Clamp01();
            return new Color(current.r + ((target.r - current.r) * t), current.g + ((target.g - current.g) * t), current.b + ((target.b - current.b) * t), current.a + ((target.a - current.a) * t));
        }

        public static Color LerpNoAlpha(this Color current, Color target, float t)
        {
            t = t.Clamp01();
            return new Color(current.r + ((target.r - current.r) * t), current.g + ((target.g - current.g) * t), current.b + ((target.b - current.b) * t), current.a);
        }
        #endregion

        #region Lerp Angle
        public static float LerpAngle(this float current, float target, float t)
        {
            float num = (target - current).Repeat(360);
            if (num > 180)
                num -= 360;

            return current + num * t.Clamp01();
        }

        [WikiIgnore]
        public static double LerpAngle(this double current, double target, double t)
        {
            double num = (target - current).Repeat(360);
            if (num > 180)
                num -= 360;

            return current + num * t.Clamp01();
        }

        [WikiIgnore]
        public static decimal LerpAngle(this decimal current, decimal target, decimal t)
        {
            decimal num = (target - current).Repeat(360);
            if (num > 180)
                num -= 360;

            return current + num * t.Clamp01();
        }

        [WikiIgnore]
        public static BigDecimal LerpAngle(this BigDecimal current, BigDecimal target, BigDecimal t)
        {
            BigDecimal num = (target - current).Repeat(360);
            if (num > 180)
                num -= 360;

            return current + num * t.Clamp01();
        }
        #endregion

        #region Lerp Unclamped
        public static sbyte LerpUnclamped(this sbyte current, sbyte target, sbyte t) => (sbyte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static sbyte LerpUnclamped(this sbyte current, sbyte target, float t) => (sbyte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static sbyte LerpUnclamped(this sbyte current, sbyte target, double t) => (sbyte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static sbyte LerpUnclamped(this sbyte current, sbyte target, decimal t) => (sbyte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static sbyte LerpUnclamped(this sbyte current, sbyte target, BigDecimal t) => (sbyte)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static byte LerpUnclamped(this byte current, byte target, byte t) => (byte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static byte LerpUnclamped(this byte current, byte target, float t) => (byte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static byte LerpUnclamped(this byte current, byte target, double t) => (byte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static byte LerpUnclamped(this byte current, byte target, decimal t) => (byte)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static byte LerpUnclamped(this byte current, byte target, BigDecimal t) => (byte)(decimal)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static short LerpUnclamped(this short current, short target, short t) => (short)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static short LerpUnclamped(this short current, short target, float t) => (short)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static short LerpUnclamped(this short current, short target, double t) => (short)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static short LerpUnclamped(this short current, short target, decimal t) => (short)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static short LerpUnclamped(this short current, short target, BigDecimal t) => (short)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static ushort LerpUnclamped(this ushort current, ushort target, ushort t) => (ushort)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ushort LerpUnclamped(this ushort current, ushort target, float t) => (ushort)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ushort LerpUnclamped(this ushort current, ushort target, double t) => (ushort)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ushort LerpUnclamped(this ushort current, ushort target, decimal t) => (ushort)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ushort LerpUnclamped(this ushort current, ushort target, BigDecimal t) => (ushort)(decimal)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static int LerpUnclamped(this int current, int target, int t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static int LerpUnclamped(this int current, int target, float t) => (int)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static int LerpUnclamped(this int current, int target, double t) => (int)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static int LerpUnclamped(this int current, int target, decimal t) => (int)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static int LerpUnclamped(this int current, int target, BigDecimal t) => (int)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static uint LerpUnclamped(this uint current, uint target, uint t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static uint LerpUnclamped(this uint current, uint target, float t) => (uint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static uint LerpUnclamped(this uint current, uint target, double t) => (uint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static uint LerpUnclamped(this uint current, uint target, decimal t) => (uint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static uint LerpUnclamped(this uint current, uint target, BigDecimal t) => (uint)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static long LerpUnclamped(this long current, long target, long t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static long LerpUnclamped(this long current, long target, float t) => (long)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static long LerpUnclamped(this long current, long target, double t) => (long)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static long LerpUnclamped(this long current, long target, decimal t) => (long)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static long LerpUnclamped(this long current, long target, BigDecimal t) => (long)(decimal)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static ulong LerpUnclamped(this ulong current, ulong target, ulong t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static ulong LerpUnclamped(this ulong current, ulong target, float t) => (ulong)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ulong LerpUnclamped(this ulong current, ulong target, double t) => (ulong)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ulong LerpUnclamped(this ulong current, ulong target, decimal t) => (ulong)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static ulong LerpUnclamped(this ulong current, ulong target, BigDecimal t) => (ulong)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static float LerpUnclamped(this float current, float target, float t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static double LerpUnclamped(this double current, double target, double t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static decimal LerpUnclamped(this decimal current, decimal target, decimal t) => ((1 - t) * current) + (target * t);

        [WikiIgnore] public static BigInteger LerpUnclamped(this BigInteger current, BigInteger target, BigInteger t) => ((1 - t) * current) + (target * t);
        [WikiIgnore] public static BigDecimal LerpUnclamped(this BigDecimal current, BigDecimal target, BigDecimal t) => ((1 - t) * current) + (target * t);

        [WikiIgnore] public static nint LerpUnclamped(this nint current, nint target, float t) => (nint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static nint LerpUnclamped(this nint current, nint target, double t) => (nint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static nint LerpUnclamped(this nint current, nint target, decimal t) => (nint)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static nuint LerpUnclamped(this nuint current, nuint target, float t) => (nuint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static nuint LerpUnclamped(this nuint current, nuint target, double t) => (nuint)(((1 - t) * current) + (target * t));
        [WikiIgnore] public static nuint LerpUnclamped(this nuint current, nuint target, decimal t) => (nuint)(((1 - t) * current) + (target * t));

        [WikiIgnore] public static Vector2 LerpUnclamped(this Vector2 current, Vector2 target, float t) => new Vector2(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t));
        [WikiIgnore] public static Vector3 LerpUnclamped(this Vector3 current, Vector3 target, float t) => new Vector3(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.z + ((target.z - current.z) * t));
        [WikiIgnore] public static Vector4 LerpUnclamped(this Vector4 current, Vector4 target, float t) => new Vector4(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.z + ((target.z - current.z) * t), current.w + ((target.w - current.w) * t));

        [WikiIgnore] public static Rect LerpUnclamped(this Rect current, Rect target, float t) => new Rect(current.x + ((target.x - current.x) * t), current.y + ((target.y - current.y) * t), current.width + ((target.width - current.width) * t), current.height + ((target.height - current.height) * t));

        [WikiIgnore] public static Color LerpUnclamped(this Color current, Color target, float t) => new Color(current.r + ((target.r - current.r) * t), current.g + ((target.g - current.g) * t), current.b + ((target.b - current.b) * t), current.a + ((target.a - current.a) * t));
        [WikiIgnore] public static Color LerpNoAlphaUnclamped(this Color current, Color target, float t) => new Color(current.r + ((target.r - current.r) * t), current.g + ((target.g - current.g) * t), current.b + ((target.b - current.b) * t), current.a);
        #endregion

        public static float LinearToGammaSpace(this float value) => Mathf.LinearToGammaSpace(value);

        #region Log
        public static float Log(this float value) => (float)Math.Log(value);
        [WikiIgnore] public static double Log(this double value) => Math.Log(value);
        #endregion

        #region Log 10
        public static float Log10(this float value) => (float)Math.Log10(value);
        [WikiIgnore] public static double Log10(this double value) => Math.Log10(value);
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

        #region Move Towards
        public static sbyte MoveTowards(this sbyte current, sbyte target, sbyte maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return (sbyte)(current + ((target - current).Sign() * maxDelta));
        }

        [WikiIgnore]
        public static byte MoveTowards(this byte current, byte target, byte maxDelta)
        {
            if ((target - current) <= maxDelta)
                return target;

            return (byte)(current + maxDelta);
        }

        [WikiIgnore]
        public static short MoveTowards(this short current, short target, short maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return (short)(current + ((target - current).Sign() * maxDelta));
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

            return current + ((target - current).Sign() * maxDelta);
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

            return current + ((target - current).Sign() * maxDelta);
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

            return current + ((target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static double MoveTowards(this double current, double target, double maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + ((target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static decimal MoveTowards(this decimal current, decimal target, decimal maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + ((target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static BigInteger MoveTowards(this BigInteger current, BigInteger target, BigInteger maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + ((target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static BigDecimal MoveTowards(this BigDecimal current, BigDecimal target, BigDecimal maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + ((target - current).Sign() * maxDelta);
        }

        [WikiIgnore]
        public static nint MoveTowards(this nint current, nint target, nint maxDelta)
        {
            if ((target - current).Abs() <= maxDelta)
                return target;

            return current + ((target - current).Sign() * maxDelta);
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
            float num3 = (num * num) + (num2 * num2);
            if (num3 == 0f || (maxDistanceDelta >= 0f && num3 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num4 = (float)Math.Sqrt(num3);
            return new Vector2(current.x + (num / num4 * maxDistanceDelta), current.y + (num2 / num4 * maxDistanceDelta));
        }
        [WikiIgnore]
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = (num * num) + (num2 * num2) + (num3 * num3);
            if (num4 == 0f || (maxDistanceDelta >= 0f && num4 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num5 = (float)Math.Sqrt(num4);
            return new Vector3(current.x + (num / num5 * maxDistanceDelta), current.y + (num2 / num5 * maxDistanceDelta), current.z + (num3 / num5 * maxDistanceDelta));
        }
        [WikiIgnore]
        public static Vector4 MoveTowards(this Vector4 current, Vector4 target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float num4 = target.w - current.w;
            float num5 = (num * num) + (num2 * num2) + (num3 * num3) + (num4 * num4);
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Vector4(current.x + (num / num6 * maxDistanceDelta), current.y + (num2 / num6 * maxDistanceDelta), current.z + (num3 / num6 * maxDistanceDelta), current.w + (num4 / num6 * maxDistanceDelta));
        }
        [WikiIgnore]
        public static Rect MoveTowards(this Rect current, Rect target, float maxDistanceDelta)
        {
            float num = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.width - current.width;
            float num4 = target.height - current.height;
            float num5 = (num * num) + (num2 * num2) + (num3 * num3) + (num4 * num4);
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Rect(current.x + (num / num6 * maxDistanceDelta), current.y + (num2 / num6 * maxDistanceDelta), current.width + (num3 / num6 * maxDistanceDelta), current.height + (num4 / num6 * maxDistanceDelta));
        }
        [WikiIgnore]
        public static Color MoveTowards(this Color current, Color target, float maxDistanceDelta)
        {
            float num = target.r - current.r;
            float num2 = target.g - current.g;
            float num3 = target.b - current.b;
            float num4 = target.a - current.a;
            float num5 = (num * num) + (num2 * num2) + (num3 * num3) + (num4 * num4);
            if (num5 == 0f || (maxDistanceDelta >= 0f && num5 <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float num6 = (float)Math.Sqrt(num5);
            return new Color(current.r + (num / num6 * maxDistanceDelta), current.g + (num2 / num6 * maxDistanceDelta), current.b + (num3 / num6 * maxDistanceDelta), current.a + (num4 / num6 * maxDistanceDelta));
        }
        #endregion

        #region Move Towards Angle
        public static short MoveTowardsAngle(this short current, short target, short maxDelta)
        {
            short temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = (short)(current + temp);
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static ushort MoveTowardsAngle(this ushort current, ushort target, ushort maxDelta)
        {
            ushort temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = (ushort)(current + temp);
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static int MoveTowardsAngle(this int current, int target, int maxDelta)
        {
            int temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static uint MoveTowardsAngle(this uint current, uint target, uint maxDelta)
        {
            uint temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static long MoveTowardsAngle(this long current, long target, long maxDelta)
        {
            long temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static ulong MoveTowardsAngle(this ulong current, ulong target, ulong maxDelta)
        {
            ulong temp = current.DeltaAngle(target);
            if (0 - maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static float MoveTowardsAngle(this float current, float target, float maxDelta)
        {
            float temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static double MoveTowardsAngle(this double current, double target, double maxDelta)
        {
            double temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static decimal MoveTowardsAngle(this decimal current, decimal target, decimal maxDelta)
        {
            decimal temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static BigInteger MoveTowardsAngle(this BigInteger current, BigInteger target, BigInteger maxDelta)
        {
            BigInteger temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static BigDecimal MoveTowardsAngle(this BigDecimal current, BigDecimal target, BigDecimal maxDelta)
        {
            BigDecimal temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static nint MoveTowardsAngle(this nint current, nint target, nint maxDelta)
        {
            nint temp = current.DeltaAngle(target);
            if (-maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }

        [WikiIgnore]
        public static nuint MoveTowardsAngle(this nuint current, nuint target, nuint maxDelta)
        {
            nuint temp = current.DeltaAngle(target);
            if (0 - maxDelta < temp && temp < maxDelta)
                return target;

            target = current + temp;
            return MoveTowards(current, target, maxDelta);
        }
        #endregion

        #region Next Power Of
        public static sbyte NextPowerOf(this sbyte value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = (sbyte)-value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = (sbyte)(tempValue * power);
                if (negative)
                    return (sbyte)-result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = (sbyte)tempValue;
                if (negative)
                    return (sbyte)-result;
                else
                    return result;
            }
        }

        public static byte NextPowerOf(this byte value, int power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return (byte)(tempValue * power);
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return (byte)tempValue;
            }
        }

        public static short NextPowerOf(this short value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = (short)-value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = (short)(tempValue * power);
                if (negative)
                    return (short)-result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = (short)tempValue;
                if (negative)
                    return (short)-result;
                else
                    return result;
            }
        }

        public static ushort NextPowerOf(this ushort value, int power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return (ushort)(tempValue * power);
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return (ushort)tempValue;
            }
        }

        public static int NextPowerOf(this int value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static uint NextPowerOf(this uint value, uint power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            uint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue * power;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue;
            }
        }

        public static long NextPowerOf(this long value, long power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }


            power = power.Clamp(1);

            long tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static ulong NextPowerOf(this ulong value, ulong power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            ulong tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue * power;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue;
            }
        }

        public static float NextPowerOf(this float value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            float tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static double NextPowerOf(this double value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            double tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static decimal NextPowerOf(this decimal value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            decimal tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static BigInteger NextPowerOf(this BigInteger value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            BigInteger tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static BigDecimal NextPowerOf(this BigDecimal value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            BigDecimal tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static nint NextPowerOf(this nint value, nint power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            nint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue * power;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static nuint NextPowerOf(this nuint value, nuint power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            nuint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue * power;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue;
            }
        }
        #endregion

        public static int NextPowerOfTwo(this int value) => Mathf.NextPowerOfTwo(value);

        public static float PerlinNoise(this float x, float y) => Mathf.PerlinNoise(x, y);

        #region Pre Power Of
        public static sbyte PrePowerOf(this sbyte value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = (sbyte)-value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = (sbyte)tempValue;
                if (negative)
                    return (sbyte)-result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = (sbyte)(tempValue / power);
                if (negative)
                    return (sbyte)-result;
                else
                    return result;
            }
        }

        public static byte PrePowerOf(this byte value, int power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return (byte)tempValue;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return (byte)(tempValue / power);
            }
        }

        public static short PrePowerOf(this short value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = (short)-value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = (short)tempValue;
                if (negative)
                    return (short)-result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = (short)(tempValue / power);
                if (negative)
                    return (short)-result;
                else
                    return result;
            }
        }

        public static ushort PrePowerOf(this ushort value, int power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return (ushort)tempValue;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return (ushort)(tempValue / power);
            }
        }

        public static int PrePowerOf(this int value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            int tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static uint PrePowerOf(this uint value, uint power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            uint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue / power;
            }
        }

        public static long PrePowerOf(this long value, long power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }


            power = power.Clamp(1);

            long tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static ulong PrePowerOf(this ulong value, ulong power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            ulong tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue / power;
            }
        }

        public static float PrePowerOf(this float value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            float tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static double PrePowerOf(this double value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            double tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static decimal PrePowerOf(this decimal value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            decimal tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static BigInteger PrePowerOf(this BigInteger value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            BigInteger tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static BigDecimal PrePowerOf(this BigDecimal value, int power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            BigDecimal tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static nint PrePowerOf(this nint value, nint power)
        {
            bool negative = false;
            if (value == 0)
                return 0;
            else if (value < 0)
            {
                value = -value;
                negative = true;
            }

            power = power.Clamp(1);

            nint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                var result = tempValue;
                if (negative)
                    return -result;
                else
                    return result;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                var result = tempValue / power;
                if (negative)
                    return -result;
                else
                    return result;
            }
        }

        public static nuint PrePowerOf(this nuint value, nuint power)
        {
            if (value == 0)
                return 0;

            power = power.Clamp(1);

            nuint tempValue = 1;
            if (value < 1)
            {
                while (tempValue > value)
                    tempValue /= power;

                return tempValue;
            }
            else
            {
                while (tempValue < value)
                    tempValue *= power;

                return tempValue / power;
            }
        }
        #endregion

        #region Ping Pong
        public static sbyte PingPong(this sbyte value, sbyte length) => (sbyte)(length - (value.Repeat((sbyte)(length * 2)) - length).Abs());
        [WikiIgnore] public static byte PingPong(this byte value, byte length) => (byte)(length - (value.Repeat((byte)(length * 2)) - length));
        [WikiIgnore] public static short PingPong(this short value, short length) => (short)(length - (value.Repeat((short)(length * 2)) - length).Abs());
        [WikiIgnore] public static ushort PingPong(this ushort value, ushort length) => (ushort)(length - (value.Repeat((ushort)(length * 2)) - length));
        [WikiIgnore] public static int PingPong(this int value, int length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static uint PingPong(this uint value, uint length) => length - (value.Repeat(length * 2) - length);
        [WikiIgnore] public static long PingPong(this long value, long length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static ulong PingPong(this ulong value, ulong length) => length - (value.Repeat(length * 2) - length);
        [WikiIgnore] public static float PingPong(this float value, float length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static double PingPong(this double value, double length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static decimal PingPong(this decimal value, decimal length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static BigInteger PingPong(this BigInteger value, BigInteger length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static BigDecimal PingPong(this BigDecimal value, BigDecimal length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static nint PingPong(this nint value, nint length) => length - (value.Repeat(length * 2) - length).Abs();
        [WikiIgnore] public static nuint PingPong(this nuint value, nuint length) => length - (value.Repeat(length * 2) - length);
        #endregion

        #region Pow
        public static float Pow(this float x, float y) => (float)Math.Pow(x, y);
        [WikiIgnore] public static double Pow(this double x, double y) => Math.Pow(x, y);
        [WikiIgnore] public static BigInteger Pow(this BigInteger x, int y) => BigInteger.Pow(x, y);
        [WikiIgnore] public static BigDecimal Pow(this BigDecimal x, BigInteger y) => BigDecimal.Pow(x, y);
        #endregion

        #region Repeat
        public static sbyte Repeat(this sbyte t, sbyte length) => (sbyte)(t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static byte Repeat(this byte t, byte length) => (byte)(t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static short Repeat(this short t, short length) => (short)(t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static ushort Repeat(this ushort t, ushort length) => (ushort)(t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static int Repeat(this int t, int length) => (t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static uint Repeat(this uint t, uint length) => (t - ((uint)((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static long Repeat(this long t, long length) => (t - (((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static ulong Repeat(this ulong t, ulong length) => (t - ((ulong)((float)t / length).FloorToInt() * length)).Clamp(0, length);
        [WikiIgnore] public static float Repeat(this float t, float length) => (t - ((t / length).Floor() * length)).Clamp(0, length);
        [WikiIgnore] public static double Repeat(this double t, double length) => (t - ((t / length).Floor() * length)).Clamp(0, length);
        [WikiIgnore] public static decimal Repeat(this decimal t, decimal length) => (t - ((t / length).Floor() * length)).Clamp(0, length);
        [WikiIgnore] public static BigInteger Repeat(this BigInteger t, BigInteger length) => (t - ((BigInteger)((BigDecimal)t / (BigDecimal)length).Floor() * length)).Clamp(0, length);
        [WikiIgnore] public static BigDecimal Repeat(this BigDecimal t, BigDecimal length) => (t - ((t / length).Floor() * length)).Clamp(0, length);
        [WikiIgnore] public static nint Repeat(this nint t, nint length) => (t - ((t / length) * length)).Clamp(0, length);
        [WikiIgnore] public static nuint Repeat(this nuint t, nuint length) => (t - ((t / length) * length)).Clamp(0, length);
        #endregion

        #region Repeat While
        public static sbyte RepeatWhile(this sbyte t, sbyte start, sbyte length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static byte RepeatWhile(this byte t, byte start, byte length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static short RepeatWhile(this short t, short start, short length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ushort RepeatWhile(this ushort t, ushort start, ushort length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static int RepeatWhile(this int t, int start, int length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static uint RepeatWhile(this uint t, uint start, uint length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static long RepeatWhile(this long t, long start, long length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ulong RepeatWhile(this ulong t, ulong start, ulong length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static float RepeatWhile(this float t, float start, float length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static double RepeatWhile(this double t, double start, double length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static decimal RepeatWhile(this decimal t, decimal start, decimal length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigInteger RepeatWhile(this BigInteger t, BigInteger start, BigInteger length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigDecimal RepeatWhile(this BigDecimal t, BigDecimal start, BigDecimal length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nint RepeatWhile(this nint t, nint start, nint length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nuint RepeatWhile(this nuint t, nuint start, nuint length)
        {
            if (t >= start)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static sbyte RepeatWhile(this sbyte t, sbyte length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static byte RepeatWhile(this byte t, byte length)
        {
            while (t > length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static short RepeatWhile(this short t, short length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ushort RepeatWhile(this ushort t, ushort length)
        {
            while (t > length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static int RepeatWhile(this int t, int length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static uint RepeatWhile(this uint t, uint length)
        {
            while (t > length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static long RepeatWhile(this long t, long length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ulong RepeatWhile(this ulong t, ulong length)
        {
            while (t > length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static float RepeatWhile(this float t, float length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static double RepeatWhile(this double t, double length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static decimal RepeatWhile(this decimal t, decimal length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigInteger RepeatWhile(this BigInteger t, BigInteger length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigDecimal RepeatWhile(this BigDecimal t, BigDecimal length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nint RepeatWhile(this nint t, nint length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nuint RepeatWhile(this nuint t, nuint length)
        {
            if (t >= 0)
            {
                while (t > length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }
        #endregion

        #region Repeat While Other
        public static sbyte RepeatWhileOther(this sbyte t, sbyte start, sbyte length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static byte RepeatWhileOther(this byte t, byte start, byte length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static short RepeatWhileOther(this short t, short start, short length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ushort RepeatWhileOther(this ushort t, ushort start, ushort length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static int RepeatWhileOther(this int t, int start, int length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static uint RepeatWhileOther(this uint t, uint start, uint length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static long RepeatWhileOther(this long t, long start, long length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ulong RepeatWhileOther(this ulong t, ulong start, ulong length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static float RepeatWhileOther(this float t, float start, float length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static double RepeatWhileOther(this double t, double start, double length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static decimal RepeatWhileOther(this decimal t, decimal start, decimal length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigInteger RepeatWhileOther(this BigInteger t, BigInteger start, BigInteger length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigDecimal RepeatWhileOther(this BigDecimal t, BigDecimal start, BigDecimal length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nint RepeatWhileOther(this nint t, nint start, nint length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nuint RepeatWhileOther(this nuint t, nuint start, nuint length)
        {
            if (t >= start)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < start)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static sbyte RepeatWhileOther(this sbyte t, sbyte length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static byte RepeatWhileOther(this byte t, byte length)
        {
            while (t >= length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static short RepeatWhileOther(this short t, short length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ushort RepeatWhileOther(this ushort t, ushort length)
        {
            while (t >= length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static int RepeatWhileOther(this int t, int length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static uint RepeatWhileOther(this uint t, uint length)
        {
            while (t >= length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static long RepeatWhileOther(this long t, long length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static ulong RepeatWhileOther(this ulong t, ulong length)
        {
            while (t >= length)
                t -= length;

            return t;
        }

        [WikiIgnore]
        public static float RepeatWhileOther(this float t, float length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static double RepeatWhileOther(this double t, double length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static decimal RepeatWhileOther(this decimal t, decimal length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigInteger RepeatWhileOther(this BigInteger t, BigInteger length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static BigDecimal RepeatWhileOther(this BigDecimal t, BigDecimal length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nint RepeatWhileOther(this nint t, nint length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }

        [WikiIgnore]
        public static nuint RepeatWhileOther(this nuint t, nuint length)
        {
            if (t >= 0)
            {
                while (t >= length)
                    t -= length;
            }
            else
            {
                while (t < 0)
                    t += length;
            }

            return t;
        }
        #endregion

        #region Round
        public static float Round(this float value) => (float)Math.Round(value);
        [WikiIgnore] public static double Round(this double value) => Math.Round(value);
        [WikiIgnore] public static decimal Round(this decimal value) => Math.Round(value);
        [WikiIgnore] public static BigDecimal Round(this BigDecimal value) => BigDecimal.Round(value);

        [WikiIgnore] public static float Round(this float value, MidpointRounding midpointRounding) => (float)Math.Round(value, midpointRounding);
        [WikiIgnore] public static double Round(this double value, MidpointRounding midpointRounding) => Math.Round(value, midpointRounding);
        [WikiIgnore] public static decimal Round(this decimal value, MidpointRounding midpointRounding) => Math.Round(value, midpointRounding);
        [WikiIgnore] public static BigDecimal Round(this BigDecimal value, MidpointRounding midpointRounding) => BigDecimal.Round(value, midpointRounding);

        [WikiIgnore] public static float Round(this float value, int digits) => (float)Math.Round(value, digits);
        [WikiIgnore] public static double Round(this double value, int digits) => Math.Round(value, digits);
        [WikiIgnore] public static decimal Round(this decimal value, int digits) => Math.Round(value, digits);
        [WikiIgnore]
        public static BigDecimal Round(this BigDecimal value, int digits)
        {
            if (digits < 0 || digits > maxRoundingDigits)
                throw new ArgumentOutOfRangeException(nameof(digits));

            BigDecimal num = roundPower10BigDecimal[digits];
            return BigDecimal.Round(value * num) / num;
        }

        [WikiIgnore] public static float Round(this float value, int digits, MidpointRounding midpointRounding) => (float)Math.Round(value, digits, midpointRounding);
        [WikiIgnore] public static double Round(this double value, int digits, MidpointRounding midpointRounding) => Math.Round(value, digits, midpointRounding);
        [WikiIgnore] public static decimal Round(this decimal value, int digits, MidpointRounding midpointRounding) => Math.Round(value, digits, midpointRounding);

        public static int RoundToInt(this float value) => (int)Math.Round(value);
        [WikiIgnore] public static int RoundToInt(this double value) => (int)Math.Round(value);
        [WikiIgnore] public static int RoundToInt(this decimal value) => (int)Math.Round(value);
        [WikiIgnore] public static BigInteger RoundToInt(this BigDecimal value) => BigDecimal.Round(value);
        #endregion

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

        #region Sin
        public static float Sin(this float value) => (float)Math.Sin(value);
        [WikiIgnore] public static double Sin(this double value) => Math.Sin(value);
        #endregion

        #region Sinh
        public static float Sinh(this float value) => (float)Math.Sinh(value);
        [WikiIgnore] public static double Sinh(this double value) => Math.Sinh(value);
        #endregion

        #region Smooth Damp
        public static float SmoothDamp(this float current, float target, ref float currentVelocity, float smoothTime) => Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, float.PositiveInfinity, Kernel.deltaTime);
        [WikiIgnore] public static float SmoothDamp(this float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed) => Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, Kernel.deltaTime);
        [WikiIgnore] public static float SmoothDamp(this float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) => Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        #endregion

        #region Smooth Damp Angle
        public static float SmoothDampAngle(this float current, float target, ref float currentVelocity, float smoothTime) => Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, float.PositiveInfinity, Kernel.deltaTime);
        [WikiIgnore] public static float SmoothDampAngle(this float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed) => Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, Kernel.deltaTime);
        [WikiIgnore] public static float SmoothDampAngle(this float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) => Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        #endregion

        public static float SmoothStep(this float from, float to, float t) => Mathf.SmoothStep(from, to, t);

        #region Sqrt
        public static float Sqrt(this float value) => (float)Math.Sqrt(value);
        [WikiIgnore] public static double Sqrt(this double value) => Math.Sqrt(value);
        [WikiIgnore] public static decimal Sqrt(this decimal value) => (decimal)Math.Sqrt((double)value);
        #endregion

        #region Tan
        public static float Tan(this float value) => (float)Math.Tan(value);
        [WikiIgnore] public static double Tan(this double value) => Math.Tan(value);
        #endregion

        #region Tanh
        public static float Tanh(this float value) => (float)Math.Tanh(value);
        [WikiIgnore] public static double Tanh(this double value) => Math.Tanh(value);
        #endregion

        #region Truncate
        public static float Truncate(this float value) => (float)Math.Truncate(value);
        [WikiIgnore] public static double Truncate(this double value) => Math.Truncate(value);
        [WikiIgnore] public static decimal Truncate(this decimal value) => Math.Truncate(value);
        [WikiIgnore] public static BigDecimal Truncate(this BigDecimal value) => BigDecimal.Truncate(value);
        #endregion
    }
}
