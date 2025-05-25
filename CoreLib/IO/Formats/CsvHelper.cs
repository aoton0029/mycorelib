using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Formats
{
    /// <summary>
    /// CSV操作ユーティリティ
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// CSVデータを解析
        /// </summary>
        public static async Task<List<string[]>> ParseCsvAsync(
            string filePath,
            char delimiter = ',',
            bool hasHeaderRow = true,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            var result = new List<string[]>();

            using var reader = new StreamReader(filePath, encoding);
            string? line;
            int lineNumber = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (lineNumber == 0 && hasHeaderRow)
                {
                    lineNumber++;
                    continue;
                }

                var fields = ParseCsvLine(line, delimiter);
                result.Add(fields.ToArray());
                lineNumber++;
            }

            return result;
        }

        /// <summary>
        /// CSV行を解析（引用符処理を含む）
        /// </summary>
        public static List<string> ParseCsvLine(string line, char delimiter = ',')
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(line))
                return result;

            int pos = 0;
            while (pos < line.Length)
            {
                string value = string.Empty;

                // スペースをスキップ
                while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                    pos++;

                if (pos < line.Length)
                {
                    // 引用符で始まる場合
                    if (line[pos] == '"')
                    {
                        pos++; // 引用符をスキップ

                        // 引用符で囲まれた値を解析
                        while (pos < line.Length)
                        {
                            // 引用符が見つかった場合
                            if (line[pos] == '"')
                            {
                                pos++; // 引用符をスキップ

                                // 連続する引用符（エスケープシーケンス）
                                if (pos < line.Length && line[pos] == '"')
                                {
                                    value += '"';
                                    pos++;
                                }
                                else
                                {
                                    break; // 引用符終了
                                }
                            }
                            else
                            {
                                value += line[pos++];
                            }
                        }

                        // 区切り文字までスキップ
                        while (pos < line.Length && line[pos] != delimiter)
                            pos++;
                    }
                    else
                    {
                        // 引用符がない場合、区切り文字まで読み込む
                        while (pos < line.Length && line[pos] != delimiter)
                        {
                            value += line[pos++];
                        }

                        // 末尾の空白を除去
                        value = value.TrimEnd();
                    }

                    result.Add(value);

                    // 区切り文字をスキップ
                    if (pos < line.Length && line[pos] == delimiter)
                        pos++;
                }
            }

            return result;
        }

        /// <summary>
        /// データをCSV形式で保存
        /// </summary>
        public static async Task WriteCsvAsync<T>(
            string filePath,
            IEnumerable<T> data,
            Func<T, IEnumerable<object>> rowSelector,
            IEnumerable<string>? headers = null,
            char delimiter = ',',
            Encoding? encoding = null,
            bool quoteAllFields = false)
        {
            encoding ??= Encoding.UTF8;

            using var writer = new StreamWriter(filePath, false, encoding);

            // ヘッダー行を書き込み
            if (headers != null && headers.Any())
            {
                await writer.WriteLineAsync(string.Join(delimiter.ToString(),
                    headers.Select(h => QuoteCsvField(h, delimiter, true))));
            }

            // データ行を書き込み
            foreach (var item in data)
            {
                var fields = rowSelector(item).Select(field =>
                    QuoteCsvField(field?.ToString() ?? string.Empty, delimiter, quoteAllFields));

                await writer.WriteLineAsync(string.Join(delimiter.ToString(), fields));
            }
        }

        /// <summary>
        /// CSV用にフィールドを引用符で囲む
        /// </summary>
        private static string QuoteCsvField(string field, char delimiter, bool quoteAlways)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            bool requiresQuote = quoteAlways ||
                field.Contains(delimiter) ||
                field.Contains('"') ||
                field.Contains('\r') ||
                field.Contains('\n');

            if (!requiresQuote)
                return field;

            // 引用符をエスケープして値全体を引用符で囲む
            return '"' + field.Replace("\"", "\"\"") + '"';
        }
    }
}
