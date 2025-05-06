using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Common
{
    /// <summary>
    /// 日付時刻の拡張メソッド
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 日本の日付形式（yyyy年MM月dd日）に変換
        /// </summary>
        public static string ToJapaneseDate(this DateTime date)
        {
            return date.ToString("yyyy年MM月dd日", CultureInfo.GetCultureInfo("ja-JP"));
        }

        /// <summary>
        /// 日本の時刻形式（HH時mm分ss秒）に変換
        /// </summary>
        public static string ToJapaneseTime(this DateTime time)
        {
            return time.ToString("HH時mm分ss秒", CultureInfo.GetCultureInfo("ja-JP"));
        }

        /// <summary>
        /// 日本の日時形式（yyyy年MM月dd日 HH時mm分ss秒）に変換
        /// </summary>
        public static string ToJapaneseDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 HH時mm分ss秒", CultureInfo.GetCultureInfo("ja-JP"));
        }

        /// <summary>
        /// 指定した曜日の週の開始日を取得
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// 週の終了日を取得
        /// </summary>
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(6);
        }

        /// <summary>
        /// 月の初日を取得
        /// </summary>
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// 月の最終日を取得
        /// </summary>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            return dt.StartOfMonth().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 年の初日を取得
        /// </summary>
        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        /// <summary>
        /// 年の最終日を取得
        /// </summary>
        public static DateTime EndOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 12, 31);
        }

        /// <summary>
        /// 日の開始時刻（00:00:00）を取得
        /// </summary>
        public static DateTime StartOfDay(this DateTime dt)
        {
            return dt.Date;
        }

        /// <summary>
        /// 日の終了時刻（23:59:59.999）を取得
        /// </summary>
        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 次の特定の曜日を取得
        /// </summary>
        public static DateTime NextDayOfWeek(this DateTime dt, DayOfWeek dayOfWeek)
        {
            int daysToAdd = ((int)dayOfWeek - (int)dt.DayOfWeek + 7) % 7;
            return dt.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }

        /// <summary>
        /// 前の特定の曜日を取得
        /// </summary>
        public static DateTime PreviousDayOfWeek(this DateTime dt, DayOfWeek dayOfWeek)
        {
            int daysToSubtract = ((int)dt.DayOfWeek - (int)dayOfWeek + 7) % 7;
            return dt.AddDays(-(daysToSubtract == 0 ? 7 : daysToSubtract));
        }

        /// <summary>
        /// 日付が特定の範囲内かどうかを確認
        /// </summary>
        public static bool IsBetween(this DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }

        /// <summary>
        /// 日付が週末（土日）かどうかを確認
        /// </summary>
        public static bool IsWeekend(this DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// 日付が平日かどうかを確認
        /// </summary>
        public static bool IsWeekday(this DateTime dt)
        {
            return !dt.IsWeekend();
        }

        /// <summary>
        /// 日付がうるう年かどうかを確認
        /// </summary>
        public static bool IsLeapYear(this DateTime dt)
        {
            return DateTime.IsLeapYear(dt.Year);
        }

        /// <summary>
        /// 日本の和暦（元号）を取得
        /// </summary>
        public static string ToJapaneseEra(this DateTime dt)
        {
            CultureInfo japaneseCulture = new CultureInfo("ja-JP");
            japaneseCulture.DateTimeFormat.Calendar = new JapaneseCalendar();
            return dt.ToString("ggy年M月d日", japaneseCulture);
        }

        /// <summary>
        /// 相対的な日時表現を取得
        /// </summary>
        public static string ToRelativeTime(this DateTime dt)
        {
            TimeSpan timeDifference = DateTime.Now - dt;

            if (timeDifference.TotalSeconds < 60)
                return "たった今";
            if (timeDifference.TotalMinutes < 60)
                return $"{(int)timeDifference.TotalMinutes}分前";
            if (timeDifference.TotalHours < 24)
                return $"{(int)timeDifference.TotalHours}時間前";
            if (timeDifference.TotalDays < 7)
                return $"{(int)timeDifference.TotalDays}日前";
            if (timeDifference.TotalDays < 30)
                return $"{(int)(timeDifference.TotalDays / 7)}週間前";
            if (timeDifference.TotalDays < 365)
                return $"{(int)(timeDifference.TotalDays / 30)}ヶ月前";

            return $"{(int)(timeDifference.TotalDays / 365)}年前";
        }

        /// <summary>
        /// 指定した分数で丸める
        /// </summary>
        public static DateTime RoundToMinutes(this DateTime dt, int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentException("minutes must be greater than 0", nameof(minutes));

            long ticks = dt.Ticks;
            long ticksPerMinute = TimeSpan.TicksPerMinute;
            long roundedTicks = (long)Math.Round((double)ticks / (ticksPerMinute * minutes)) * (ticksPerMinute * minutes);
            return new DateTime(roundedTicks, dt.Kind);
        }
    }
}
