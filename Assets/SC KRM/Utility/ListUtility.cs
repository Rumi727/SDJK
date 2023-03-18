using ExtendedNumerics;
using System.Collections.Generic;
using System.Numerics;
using System;
using System.Linq;
using System.Collections;

namespace SCKRM
{
    public static class ListUtility
    {
        public static void Move(this IList list, int oldIndex, int newIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            object temp = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, temp);
        }

        public static void Change(this IList list, int oldIndex, int newIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            object temp = list[newIndex];
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

                sbyte val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    sbyte currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                byte val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    byte currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                short val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    short currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                ushort val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    ushort currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                int val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    int currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                uint val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    uint currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                long val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    long currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                ulong val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    ulong currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                float val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    float currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                double val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    double currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                decimal val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    decimal currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                BigInteger val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    BigInteger currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                BigDecimal val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    BigDecimal currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                nint val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    nint currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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

                nuint val = 0;
                bool exists = false;
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                    {
                        val = getNumberFunc(enumerator.Current);
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    return 0;

                while (enumerator.MoveNext())
                {
                    if (!predicate(enumerator.Current))
                        continue;

                    nuint currentNumber = getNumberFunc(enumerator.Current);
                    val = val.Distance(target) < currentNumber.Distance(target) ? val : currentNumber;
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
}
