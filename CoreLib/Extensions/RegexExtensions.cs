using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions
{
    /// <summary>
    /// 正規表現の拡張メソッド
    /// </summary>
    public static class RegexExtensions
    {
        /// <summary>
        /// 正規表現パターンに一致する文字列を抽出
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="groupName">取得するグループ名（省略可）</param>
        /// <returns>マッチした文字列のコレクション</returns>
        public static IEnumerable<string> Extract(this string input, string pattern, string? groupName = null)
        {
            if (string.IsNullOrEmpty(input))
                return Enumerable.Empty<string>();

            var matches = Regex.Matches(input, pattern);
            if (string.IsNullOrEmpty(groupName))
            {
                return matches.Cast<Match>().Select(m => m.Value);
            }
            else
            {
                return matches.Cast<Match>()
                    .Select(m => m.Groups[groupName].Value)
                    .Where(v => !string.IsNullOrEmpty(v));
            }
        }

        /// <summary>
        /// 正規表現パターンに一致する最初の文字列を抽出
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="groupName">取得するグループ名（省略可）</param>
        /// <returns>マッチした最初の文字列、見つからない場合は空文字</returns>
        public static string ExtractFirst(this string input, string pattern, string? groupName = null)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var match = Regex.Match(input, pattern);
            if (!match.Success)
                return string.Empty;

            return string.IsNullOrEmpty(groupName) ? match.Value : match.Groups[groupName].Value;
        }

        /// <summary>
        /// 正規表現パターンに一致する部分を削除
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>パターンに一致する部分を削除した文字列</returns>
        public static string Remove(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, pattern, string.Empty);
        }

        /// <summary>
        /// 正規表現パターンに一致する部分を置換
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="replacement">置換文字列</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplacePattern(this string input, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// 正規表現パターンに一致する部分を置換（関数で置換内容を指定）
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="evaluator">置換関数</param>
        /// <returns>置換後の文字列</returns>
        public static string ReplacePattern(this string input, string pattern, MatchEvaluator evaluator)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, pattern, evaluator);
        }

        /// <summary>
        /// 正規表現パターンに一致する行を抽出（Grep機能）
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>パターンに一致する行のコレクション</returns>
        public static IEnumerable<string> Grep(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return Enumerable.Empty<string>();

            return input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Where(line => Regex.IsMatch(line, pattern));
        }

        /// <summary>
        /// 正規表現パターンに一致する行を置換（Grep置換機能）
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="lineEvaluator">行の置換関数</param>
        /// <returns>置換後の文字列</returns>
        public static string GrepReplace(this string input, string pattern, Func<string, string> lineEvaluator)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (Regex.IsMatch(lines[i], pattern))
                {
                    lines[i] = lineEvaluator(lines[i]);
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// 正規表現パターンに一致する行の内容を置換（Grep行内置換機能）
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="linePattern">行を検索する正規表現パターン</param>
        /// <param name="replacementPattern">行内で置換する正規表現パターン</param>
        /// <param name="replacement">置換文字列</param>
        /// <returns>置換後の文字列</returns>
        public static string GrepReplace(this string input, string linePattern, string replacementPattern, string replacement)
        {
            return GrepReplace(input, linePattern, line => Regex.Replace(line, replacementPattern, replacement));
        }

        /// <summary>
        /// 正規表現パターンに文字列が一致するか検証
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>一致する場合はtrue</returns>
        public static bool IsMatch(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 正規表現パターンで文字列を分割
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>分割された文字列の配列</returns>
        public static string[] Split(this string input, string pattern)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<string>();

            return Regex.Split(input, pattern);
        }
    }
}
