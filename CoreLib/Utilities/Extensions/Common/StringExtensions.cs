using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Common
{
    /// <summary>
    /// 文字列の拡張メソッド
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 文字列をLike演算子で比較
        /// </summary>
        /// <remarks>既存の機能をExtensions層に移植</remarks>
        public static bool Like(this string source, string pattern)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
                return false;

            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("%", ".*")
                .Replace("_", ".") + "$";

            return Regex.IsMatch(source, regexPattern);
        }

        /// <summary>
        /// 文字列を指定した長さに省略
        /// </summary>
        /// <remarks>既存の機能をExtensions層に移植</remarks>
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (text.Length <= maxLength) return text;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// ひらがなをカタカナに変換
        /// </summary>
        public static string ToKatakana(this string hiragana)
        {
            if (string.IsNullOrEmpty(hiragana)) return string.Empty;

            return CoreLib.Utilities.Text.StringHelper.HiraganaToKatakana(hiragana);
        }

        /// <summary>
        /// カタカナをひらがなに変換
        /// </summary>
        public static string ToHiragana(this string katakana)
        {
            if (string.IsNullOrEmpty(katakana)) return string.Empty;

            return CoreLib.Utilities.Text.StringHelper.KatakanaToHiragana(katakana);
        }

        /// <summary>
        /// 文字列を全角から半角に変換
        /// </summary>
        /// <remarks>既存の機能をExtensions層に移植</remarks>
        public static string ToHalfWidth(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return CoreLib.Utilities.Text.StringHelper.ToHalfWidth(text);
        }

        /// <summary>
        /// 文字列を半角から全角に変換
        /// </summary>
        /// <remarks>既存の機能をExtensions層に移植</remarks>
        public static string ToFullWidth(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return CoreLib.Utilities.Text.StringHelper.ToFullWidth(text);
        }

        /// <summary>
        /// Base64エンコードされた文字列をデコード
        /// </summary>
        public static string FromBase64(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return string.Empty;

            try
            {
                byte[] data = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 文字列をBase64にエンコード
        /// </summary>
        public static string ToBase64(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            byte[] data = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 文字列が有効なJSONかどうか検証
        /// </summary>
        public static bool IsValidJson(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            return CoreLib.Utilities.IO.Formats.JsonHelper.IsValidJson(json);
        }

        /// <summary>
        /// 文字列をタイトルケースに変換（各単語の先頭を大文字に）
        /// </summary>
        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// 文字列をキャメルケースに変換
        /// </summary>
        /// <example>
        /// "hello_world" -> "helloWorld"
        /// </example>
        public static string ToCamelCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, "([_-])([a-zA-Z0-9])", m => m.Groups[2].Value.ToUpper());
            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// 文字列をパスカルケースに変換
        /// </summary>
        /// <example>
        /// "hello_world" -> "HelloWorld"
        /// </example>
        public static string ToPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, "([_-])([a-zA-Z0-9])", m => m.Groups[2].Value.ToUpper());
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// 文字列をスネークケースに変換
        /// </summary>
        /// <example>
        /// "HelloWorld" -> "hello_world"
        /// </example>
        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "_$1").ToLower();
        }

        /// <summary>
        /// 文字列をケバブケースに変換
        /// </summary>
        /// <example>
        /// "HelloWorld" -> "hello-world"
        /// </example>
        public static string ToKebabCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1").ToLower();
        }

        /// <summary>
        /// 文字列内の改行コードを正規化
        /// </summary>
        public static string NormalizeLineEndings(this string text, string lineEnding = "\n")
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, @"\r\n?|\n", lineEnding);
        }

        /// <summary>
        /// 文字列から指定された文字を削除
        /// </summary>
        public static string RemoveChars(this string text, params char[] chars)
        {
            if (string.IsNullOrEmpty(text) || chars == null || chars.Length == 0)
                return text;

            return new string(text.Where(c => !chars.Contains(c)).ToArray());
        }

        /// <summary>
        /// 文字列の先頭と末尾から指定された文字列を削除
        /// </summary>
        public static string Trim(this string text, string trimString)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.TrimStart(trimString).TrimEnd(trimString);
        }

        /// <summary>
        /// 文字列の先頭から指定された文字列を削除
        /// </summary>
        public static string TrimStart(this string text, string trimString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(trimString))
                return text;

            while (text.StartsWith(trimString))
            {
                text = text.Substring(trimString.Length);
            }

            return text;
        }

        /// <summary>
        /// 文字列の末尾から指定された文字列を削除
        /// </summary>
        public static string TrimEnd(this string text, string trimString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(trimString))
                return text;

            while (text.EndsWith(trimString))
            {
                text = text.Substring(0, text.Length - trimString.Length);
            }

            return text;
        }

        /// <summary>
        /// 指定された数の文字を左から取得
        /// </summary>
        public static string Left(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Substring(0, Math.Min(length, text.Length));
        }

        /// <summary>
        /// 指定された数の文字を右から取得
        /// </summary>
        public static string Right(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Substring(Math.Max(0, text.Length - length));
        }

        /// <summary>
        /// 文字列の指定された位置から指定された数の文字を取得
        /// </summary>
        public static string Mid(this string text, int start, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (start >= text.Length)
                return string.Empty;

            return text.Substring(start, Math.Min(length, text.Length - start));
        }
    }
}
