using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Text
{
    /// <summary>
    /// 文字列操作のユーティリティクラス
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 文字列が有効かどうかを確認
        /// </summary>
        public static bool IsNullOrEmpty(string? value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// 文字列をLike演算子で比較
        /// </summary>
        public static bool Like(this string source, string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("%", ".*")
                .Replace("_", ".") + "$";

            return Regex.IsMatch(source, regexPattern);
        }

        /// <summary>
        /// 文字列を指定した長さに省略
        /// </summary>
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length <= maxLength) return text;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// ひらがなをカタカナに変換
        /// </summary>
        public static string HiraganaToKatakana(string hiragana)
        {
            return string.Concat(
                hiragana.Select(c => char.GetUnicodeCategory(c) == UnicodeCategory.LowercaseLetter
                    ? (char)(c + 0x60)
                    : c)
            );
        }

        /// <summary>
        /// カタカナをひらがなに変換
        /// </summary>
        public static string KatakanaToHiragana(string katakana)
        {
            return string.Concat(
                katakana.Select(c => char.GetUnicodeCategory(c) == UnicodeCategory.UppercaseLetter
                    ? (char)(c - 0x60)
                    : c)
            );
        }

        /// <summary>
        /// 文字列を全角から半角に変換
        /// </summary>
        public static string ToHalfWidth(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return new string(text.Select(c =>
            {
                // 全角英数字を半角に変換
                if (c >= '０' && c <= '９')
                    return (char)(c - '０' + '0');
                if (c >= 'Ａ' && c <= 'Ｚ')
                    return (char)(c - 'Ａ' + 'A');
                if (c >= 'ａ' && c <= 'ｚ')
                    return (char)(c - 'ａ' + 'a');
                // 全角スペースを半角に変換
                if (c == '　')
                    return ' ';
                // その他の文字はそのまま
                return c;
            }).ToArray());
        }

        /// <summary>
        /// 文字列を半角から全角に変換
        /// </summary>
        public static string ToFullWidth(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return new string(text.Select(c =>
            {
                // 半角英数字を全角に変換
                if (c >= '0' && c <= '9')
                    return (char)(c - '0' + '０');
                if (c >= 'A' && c <= 'Z')
                    return (char)(c - 'A' + 'Ａ');
                if (c >= 'a' && c <= 'z')
                    return (char)(c - 'a' + 'ａ');
                // 半角スペースを全角に変換
                if (c == ' ')
                    return '　';
                // その他の文字はそのまま
                return c;
            }).ToArray());
        }
    }
}
