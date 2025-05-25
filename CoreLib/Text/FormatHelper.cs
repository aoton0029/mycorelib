using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Text
{
    /// <summary>
    /// テキストフォーマット関連のユーティリティクラス
    /// </summary>
    public static class FormatHelper
    {
        private static readonly CultureInfo JapaneseCulture = new CultureInfo("ja-JP");

        /// <summary>
        /// 日本の通貨形式に変換
        /// </summary>
        public static string ToJapaneseCurrency(this decimal value)
        {
            return string.Format(JapaneseCulture, "{0:C}", value);
        }

        /// <summary>
        /// 桁区切りのある数値形式に変換
        /// </summary>
        public static string ToFormattedNumber(this int value)
        {
            return string.Format(JapaneseCulture, "{0:#,0}", value);
        }

        /// <summary>
        /// 桁区切りのある数値形式に変換（小数点以下指定可能）
        /// </summary>
        public static string ToFormattedNumber(this double value, int decimalPlaces = 2)
        {
            string format = "{0:#,0" + (decimalPlaces > 0 ? "." + new string('0', decimalPlaces) : "") + "}";
            return string.Format(JapaneseCulture, format, value);
        }

        /// <summary>
        /// 日本の日付形式に変換（yyyy年MM月dd日）
        /// </summary>
        public static string ToJapaneseDate(this DateTime date)
        {
            return date.ToString("yyyy年MM月dd日", JapaneseCulture);
        }

        /// <summary>
        /// 日本の時刻形式に変換（HH時mm分ss秒）
        /// </summary>
        public static string ToJapaneseTime(this DateTime time)
        {
            return time.ToString("HH時mm分ss秒", JapaneseCulture);
        }

        /// <summary>
        /// 日本の日時形式に変換（yyyy年MM月dd日 HH時mm分ss秒）
        /// </summary>
        public static string ToJapaneseDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy年MM月dd日 HH時mm分ss秒", JapaneseCulture);
        }

        /// <summary>
        /// 指定された形式で日時を表示（今日/昨日/日付）
        /// </summary>
        public static string ToRelativeDateString(this DateTime date)
        {
            DateTime today = DateTime.Today;

            if (date.Date == today)
                return "今日 " + date.ToString("HH:mm");
            else if (date.Date == today.AddDays(-1))
                return "昨日 " + date.ToString("HH:mm");
            else
                return date.ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// ファイルサイズを人間が読みやすい形式に変換
        /// </summary>
        public static string ToFileSizeString(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
