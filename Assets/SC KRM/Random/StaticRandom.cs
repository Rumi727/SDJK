using ExtendedNumerics;
using System.Numerics;

namespace SCKRM
{
    [WikiDescription("선형 합동 법을 이용한 정적 난수 생성 클래스")]
    public static class StaticRandom
    {
        //Base on https://gist.github.com/ekepes/8aed1310c3e7af31c99d

        const uint m = uint.MaxValue; // aka 2^32
        const uint a = 1664525;
        const uint c = 1013904223;

        public static sbyte Range(sbyte length, sbyte seed) => (sbyte)(Sample(seed) % length);
        public static sbyte Range(sbyte start, sbyte length, sbyte seed) => (sbyte)(start + Range(start, length, seed));

        public static byte Range(byte length, byte seed) => (byte)(Sample(seed) % length);
        public static byte Range(byte start, byte length, byte seed) => (byte)(start + Range(start, length, seed));

        public static short Range(short length, short seed) => (short)(Sample(seed) % length);
        public static short Range(short start, short length, short seed) => (short)(start + Range(start, length, seed));

        public static ushort Range(ushort length, ushort seed) => (ushort)(Sample(seed) % length);
        public static ushort Range(ushort start, ushort length, ushort seed) => (ushort)(start + Range(start, length, seed));

        public static int Range(int length, int seed) => Sample(seed) % length;
        public static int Range(int start, int length, int seed) => start + Range(start, length, seed);

        public static int Range(int length, long seed) => (int)(Sample(seed) % length);
        public static int Range(int start, int length, long seed) => start + Range(start, length, seed);

        public static uint Range(uint length, uint seed) => Sample(seed) % length;
        public static uint Range(uint start, uint length, uint seed) => start + Range(start, length, seed);

        public static long Range(long length, long seed) => Sample(seed) % length;
        public static long Range(long start, long length, long seed) => start + Range(start, length, seed);

        public static ulong Range(ulong length, ulong seed) => Sample(seed) % length;
        public static ulong Range(ulong start, ulong length, ulong seed) => start + Range(start, length, seed);

        public static float Range(float length, long seed) => Sample(seed) % length;
        public static float Range(float start, float length, long seed) => start + Range(start, length, seed);

        public static float Range(float length, float seed) => Sample(seed) % length;
        public static float Range(float start, float length, float seed) => start + Range(start, length, seed);

        public static double Range(double length, long seed) => Sample(seed) % length;
        public static double Range(double start, double length, long seed) => start + Range(start, length, seed);

        public static double Range(double length, double seed) => Sample(seed) % length;
        public static double Range(double start, double length, double seed) => start + Range(start, length, seed);

        public static BigInteger Range(BigInteger length, BigInteger seed) => Sample(seed) % length;
        public static BigInteger Range(BigInteger start, BigInteger length, BigInteger seed) => start + Range(start, length, seed);

        public static BigDecimal Range(BigDecimal length, BigDecimal seed) => Sample(seed) % length;
        public static BigDecimal Range(BigDecimal start, BigDecimal length, BigDecimal seed) => start + Range(start, length, seed);

        public static nint Range(nint length, nint seed) => Sample(seed) % length;
        public static nint Range(nint start, nint length, nint seed) => start + Range(start, length, seed);

        public static nuint Range(nuint length, nuint seed) => Sample(seed) % length;
        public static nuint Range(nuint start, nuint length, nuint seed) => start + Range(start, length, seed);

        static sbyte Sample(sbyte seed) => (sbyte)(((a * seed) + c) % m);
        static byte Sample(byte seed) => (byte)(((a * seed) + c) % m);
        static short Sample(short seed) => (short)(((a * seed) + c) % m);
        static ushort Sample(ushort seed) => (ushort)(((a * seed) + c) % m);
        static int Sample(int seed) => (int)(((a * seed) + c) % m);
        static uint Sample(uint seed) => ((a * seed) + c) % m;
        static long Sample(long seed) => ((a * seed) + c) % m;
        static ulong Sample(ulong seed) => ((a * seed) + c) % m;
        static float Sample(float seed) => ((a * seed) + c) % m;
        static double Sample(double seed) => ((a * seed) + c) % m;
        static BigInteger Sample(BigInteger seed) => ((a * seed) + c) % m;
        static BigDecimal Sample(BigDecimal seed) => ((a * seed) + c) % m;
        static nint Sample(nint seed) => (nint)(((a * seed) + c) % m);
        static nuint Sample(nuint seed) => ((a * seed) + c) % m;
    }
}
