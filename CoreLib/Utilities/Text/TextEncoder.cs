using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Text
{
    /// <summary>
    /// テキストエンコーディング関連のユーティリティクラス
    /// </summary>
    public static class TextEncoder
    {
        /// <summary>
        /// 文字列をBase64にエンコード
        /// </summary>
        public static string ToBase64(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }

        /// <summary>
        /// Base64文字列をデコード
        /// </summary>
        public static string FromBase64(this string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return string.Empty;

            try
            {
                byte[] textBytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(textBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 文字列をURLエンコード
        /// </summary>
        public static string UrlEncode(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Uri.EscapeDataString(text);
        }

        /// <summary>
        /// URLエンコードされた文字列をデコード
        /// </summary>
        public static string UrlDecode(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        /// HTMLエスケープ処理
        /// </summary>
        public static string HtmlEscape(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

        /// <summary>
        /// HTMLエスケープ解除処理
        /// </summary>
        public static string HtmlUnescape(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Replace("&#39;", "'");
        }

        /// <summary>
        /// 指定されたエンコーディングでバイト配列に変換
        /// </summary>
        public static byte[] ToBytes(this string text, Encoding? encoding = null)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            encoding ??= Encoding.UTF8;
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// バイト配列から文字列に変換
        /// </summary>
        public static string ToString(this byte[] bytes, Encoding? encoding = null)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            encoding ??= Encoding.UTF8;
            return encoding.GetString(bytes);
        }
    }
}
