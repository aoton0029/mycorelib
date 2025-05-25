using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Formats
{
    /// <summary>
    /// 交互にヘッダー行とデータ行が繰り返されるCSV形式のシリアライザー実装
    /// </summary>
    public class MultiHeaderCsvSerializer : ISerializer
    {
        private readonly string _delimiter;
        private readonly Encoding _encoding;

        /// <summary>
        /// MultiHeaderCsvSerializerのコンストラクタ
        /// </summary>
        /// <param name="delimiter">区切り文字（デフォルトはカンマ）</param>
        /// <param name="encoding">文字エンコーディング</param>
        public MultiHeaderCsvSerializer(string delimiter = ",", Encoding? encoding = null)
        {
            _delimiter = delimiter;
            _encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// サポートするフォーマット
        /// </summary>
        public DataFormat Format => DataFormat.Csv;

        /// <summary>
        /// オブジェクトをCSV文字列にシリアライズ
        /// </summary>
        public string Serialize<T>(T obj)
        {
            if (obj == null)
                return string.Empty;

            // 入力オブジェクトは複数のセクション（ヘッダー+データ行のグループ）のリストであることを期待
            if (obj is IEnumerable<MultiHeaderSection> sections)
            {
                return SerializeSections(sections);
            }

            throw new InvalidOperationException("MultiHeaderCsvSerializerはMultiHeaderSectionのコレクションのみをサポートします。");
        }

        /// <summary>
        /// CSV文字列からオブジェクトにデシリアライズ
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            try
            {
                // T が List<MultiHeaderSection> または IEnumerable<MultiHeaderSection> の場合のみデシリアライズ可能
                Type type = typeof(T);
                Type expectedType = typeof(IEnumerable<MultiHeaderSection>);

                if (!expectedType.IsAssignableFrom(type))
                {
                    throw new InvalidOperationException($"MultiHeaderCsvSerializerはIEnumerable<MultiHeaderSection>互換の型のみをデシリアライズできます。");
                }

                var sections = DeserializeToSections(data);

                // 適切な型に変換
                if (type == typeof(List<MultiHeaderSection>))
                {
                    return (T)(object)sections.ToList();
                }
                else if (type.IsArray && type.GetElementType() == typeof(MultiHeaderSection))
                {
                    return (T)(object)sections.ToArray();
                }

                // その他のIEnumerable<MultiHeaderSection>実装型
                return (T)(object)sections;
            }
            catch (Exception ex)
            {
                throw new FormatException($"マルチヘッダーCSV文字列をデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをストリームにシリアライズ
        /// </summary>
        public async Task SerializeToStreamAsync<T>(T obj, Stream stream, CancellationToken cancellationToken = default)
        {
            if (obj == null || stream == null)
                return;

            string csv = Serialize(obj);
            byte[] bytes = _encoding.GetBytes(csv);
            await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        /// <summary>
        /// ストリームからオブジェクトにデシリアライズ
        /// </summary>
        public async Task<T?> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null || stream.Length == 0)
                return default;

            using var reader = new StreamReader(stream, _encoding, leaveOpen: true);
            string csv = await reader.ReadToEndAsync();
            return Deserialize<T>(csv);
        }

        /// <summary>
        /// オブジェクトをCSVファイルに保存
        /// </summary>
        public async Task SerializeToFileAsync<T>(T obj, string filePath, CancellationToken cancellationToken = default)
        {
            if (obj == null || string.IsNullOrEmpty(filePath))
                return;

            // 出力先ディレクトリの作成
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await SerializeToStreamAsync(obj, fileStream, cancellationToken);
        }

        /// <summary>
        /// CSVファイルからオブジェクトを読み込み
        /// </summary>
        public async Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return default;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await DeserializeFromStreamAsync<T>(fileStream, cancellationToken);
        }

        #region Helper Methods

        private string SerializeSections(IEnumerable<MultiHeaderSection> sections)
        {
            var sb = new StringBuilder();

            bool isFirst = true;
            foreach (var section in sections)
            {
                // セクション間に空行を入れない場合は初回以外に改行を追加
                if (!isFirst)
                {
                    sb.AppendLine();
                }

                // ヘッダー行
                sb.AppendLine(string.Join(_delimiter, section.Headers.Select(h => EscapeField(h))));

                // データ行
                foreach (var dataRow in section.DataRows)
                {
                    sb.AppendLine(string.Join(_delimiter, dataRow.Select(field => EscapeField(field?.ToString() ?? ""))));
                }

                isFirst = false;
            }

            return sb.ToString();
        }

        private IEnumerable<MultiHeaderSection> DeserializeToSections(string csv)
        {
            var sections = new List<MultiHeaderSection>();
            var lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                          .Select(line => line.Trim())
                          .ToList();

            if (lines.Count == 0)
                return sections;

            int currentLine = 0;

            while (currentLine < lines.Count)
            {
                // 空行をスキップ
                while (currentLine < lines.Count && string.IsNullOrWhiteSpace(lines[currentLine]))
                {
                    currentLine++;
                }

                if (currentLine >= lines.Count)
                    break;

                // ヘッダー行を読み取り
                string[] headers = SplitLine(lines[currentLine]);
                currentLine++;

                var dataRows = new List<string[]>();

                // データ行を読み取り（次のヘッダー行または空行が見つかるまで）
                while (currentLine < lines.Count &&
                      !string.IsNullOrWhiteSpace(lines[currentLine]))
                {
                    dataRows.Add(SplitLine(lines[currentLine]));
                    currentLine++;
                }

                // セクションを追加
                sections.Add(new MultiHeaderSection
                {
                    Headers = headers,
                    DataRows = dataRows
                });
            }

            return sections;
        }

        private string[] SplitLine(string line)
        {
            // CSVの正しい分割処理（ダブルクォーテーションなどを考慮）
            var result = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    // ダブルクォーテーションの処理
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // エスケープされたダブルクォーテーション
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == _delimiter[0] && !inQuotes)
                {
                    // 区切り文字の処理
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            // 最後のフィールドを追加
            result.Add(current.ToString());

            return result.ToArray();
        }

        private string EscapeField(string field)
        {
            // 区切り文字や改行、ダブルクォーテーションを含む場合は引用符で囲む
            if (field.Contains(_delimiter) || field.Contains("\r") || field.Contains("\n") || field.Contains("\""))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }

        #endregion
    }

    /// <summary>
    /// マルチヘッダーCSVのセクションを表すクラス
    /// （ヘッダー行と、それに続く複数のデータ行）
    /// </summary>
    public class MultiHeaderSection
    {
        /// <summary>
        /// ヘッダー行の各フィールド
        /// </summary>
        public string[] Headers { get; set; } = Array.Empty<string>();

        /// <summary>
        /// データ行のコレクション
        /// </summary>
        public List<string[]> DataRows { get; set; } = new List<string[]>();
    }
}

