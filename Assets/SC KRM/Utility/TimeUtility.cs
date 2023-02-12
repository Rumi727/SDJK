using System.Globalization;
using System;

namespace SCKRM
{
    public static class TimeUtility
    {
        #region To Time
        /// <summary>
        /// (second = 70) = "1:10"
        /// </summary>
        /// <param name="timeSpan">시간</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        public static string ToTime(this int second, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false) => ToString(TimeSpan.FromSeconds(second), false, minuteAlwayShow, hourAlwayShow, dayAlwayShow);

        /// <summary>
        /// (second = 70.1f) = "1:10.1"
        /// </summary>
        /// <param name="second">초</param>
        /// <param name="decimalShow">소수 표시</param>
        /// <param name="minuteAlwayShow">분 단위 항상 표시</param>
        /// <param name="hourAlwayShow">시간, 분 단위 항상 표시</param>
        /// <param name="dayAlwayShow">하루, 시간, 분 단위 항상 표시</param>
        /// <returns></returns>
        [WikiIgnore]
        public static string ToTime(this float second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            if (!double.IsNormal(second))
                return "--:--";

            return ToString(TimeSpan.FromSeconds(second), decimalShow, minuteAlwayShow, hourAlwayShow, dayAlwayShow);
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
        [WikiIgnore]
        public static string ToTime(this double second, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            if (!double.IsNormal(second))
                return "--:--";

            return ToString(TimeSpan.FromSeconds(second), decimalShow, minuteAlwayShow, hourAlwayShow, dayAlwayShow);
        }

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
        public static string ToString(this TimeSpan timeSpan, bool decimalShow = true, bool minuteAlwayShow = false, bool hourAlwayShow = false, bool dayAlwayShow = false)
        {
            try
            {
                double second = timeSpan.TotalSeconds;
                double secondAbs = second.Abs();
                if (decimalShow)
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds || secondAbs <= double.NegativeInfinity)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (second >= TimeSpan.MaxValue.TotalSeconds || secondAbs >= double.PositiveInfinity)
                        return TimeSpan.MaxValue.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return timeSpan.ToString(@"d\:hh\:mm\:ss\.ff");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return timeSpan.ToString(@"h\:mm\:ss\.ff");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return timeSpan.ToString(@"m\:ss\.ff");
                    else
                        return timeSpan.ToString(@"s\.ff");
                }
                else
                {
                    if (second <= TimeSpan.MinValue.TotalSeconds)
                        return TimeSpan.MinValue.ToString(@"d\:hh\:mm\:ss");
                    else if (second >= TimeSpan.MaxValue.TotalSeconds)
                        return TimeSpan.MaxValue.ToString(@"d\:h\:mm\:ss");
                    else if (secondAbs >= 86400 || dayAlwayShow)
                        return timeSpan.ToString(@"d\:hh\:mm\:ss");
                    else if (secondAbs >= 3600 || hourAlwayShow)
                        return timeSpan.ToString(@"h\:mm\:ss");
                    else if (secondAbs >= 60 || minuteAlwayShow)
                        return timeSpan.ToString(@"m\:ss");
                    else
                        return timeSpan.ToString(@"s");
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
        public static DateTime ToLunarDate(this DateTime dateTime, out bool isLeapMonth)
        {
            int year = klc.GetYear(dateTime);
            int month = klc.GetMonth(dateTime);
            int day = klc.GetDayOfMonth(dateTime);
            int hour = klc.GetHour(dateTime);
            int minute = klc.GetMinute(dateTime);
            int second = klc.GetSecond(dateTime);
            int millisecond = (int)klc.GetMilliseconds(dateTime);

            isLeapMonth = false;

            //1년이 12이상이면 윤달이 있음..
            if (klc.GetMonthsInYear(year) > 12)
            {
                //년도의 윤달이 몇월인지?
                int leapMonth = klc.GetLeapMonth(year);

                //달이 윤월보다 같거나 크면 -1을 함 즉 윤8은->9 이기때문
                if (month >= leapMonth)
                {
                    isLeapMonth = true;
                    month--;
                }
            }

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public static DateTime ToSolarDate(this DateTime dateTime, bool isLeapMonth = false)
        {
            if (klc.GetMonthsInYear(dateTime.Year) > 12)
            {
                int leapMonth = klc.GetLeapMonth(dateTime.Year);

                if (dateTime.Month > leapMonth - 1)
                    dateTime = klc.AddMonths(dateTime, 1);
                else if (dateTime.Month == leapMonth - 1 && isLeapMonth)
                    dateTime = klc.AddMonths(dateTime, 1);
            }

            return klc.ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }
    }
}
