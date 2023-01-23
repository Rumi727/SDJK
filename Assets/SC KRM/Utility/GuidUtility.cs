using ExtendedNumerics;
using System.Numerics;
using System;

namespace SCKRM
{
    public static class GuidUtility
    {
        static byte[] maxGuidToBytes = new byte[16] { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue };

        public static byte ToByte(this Guid value) => (byte)(value.ToBigInteger().Repeat(byte.MaxValue));
        public static ushort ToUInt16(this Guid value) => (ushort)(value.ToBigInteger().Repeat(ushort.MaxValue));
        public static uint ToUInt32(this Guid value) => (uint)(value.ToBigInteger().Repeat(uint.MaxValue));
        public static ulong ToUInt64(this Guid value) => (ulong)(value.ToBigInteger().Repeat(ulong.MaxValue));
        public static BigInteger ToBigInteger(this Guid value) => new BigInteger(value.ToByteArray());
        public static BigDecimal ToBigDecimal(this Guid value) => new BigDecimal(value.ToBigInteger());

        public static Guid ToGuid(this byte value) => new Guid(FixByteArrayLength(new byte[] { value }));
        public static Guid ToGuid(this ushort value) => new Guid(FixByteArrayLength(BitConverter.GetBytes(value)));
        public static Guid ToGuid(this uint value) => new Guid(FixByteArrayLength(BitConverter.GetBytes(value)));
        public static Guid ToGuid(this ulong value) => new Guid(FixByteArrayLength(BitConverter.GetBytes(value)));
        public static Guid ToGuid(this BigInteger value) => new Guid(FixByteArrayLength(value.ToByteArray(true)));

        static byte[] FixByteArrayLength(byte[] array)
        {
            byte[] tempArray = new byte[16];
            if (tempArray.Length == array.Length)
                return array;
            else if (tempArray.Length < array.Length)
                return maxGuidToBytes;

            for (int i = 0; i < array.Length; i++)
                tempArray[tempArray.Length - array.Length + i] = array[i];

            return tempArray;
        }
    }
}
