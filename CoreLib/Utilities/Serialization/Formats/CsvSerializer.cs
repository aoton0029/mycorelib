using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Formats
{
    /// <summary>
    /// CSVシリアライザー実装
    /// </summary>
    public class CsvSerializer : ISerializer
    {
        private readonly string _delimiter;
        private readonly bool _includeHeader;
        private readonly Encoding _encoding;

        /// <summary>
        /// CSVシリアライザーのコンストラクタ
        /// </summary>
        /// <param name="delimiter">区切り文字（デフォルトはカンマ）</param>
        /// <param name="includeHeader">ヘッダー行を含めるかどうか</param>
        /// <param name="encoding">文字エンコーディング</param>
        public CsvSerializer(string delimiter = ",", bool includeHeader = true, Encoding? encoding = null)
        {
            _delimiter = delimiter;
            _includeHeader = includeHeader;
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

            if (obj is IEnumerable<object> collection)
            {
                return SerializeCollection(collection);
            }

            // 単一オブジェクトの場合はコレクションに変換
            return SerializeCollection(new[] { obj });
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
                // コレクション型か確認
                Type type = typeof(T);
                bool isCollection = type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                if (isCollection || type.IsArray)
                {
                    Type elementType = type.IsArray ?
                        type.GetElementType()! :
                        type.GetGenericArguments()[0];

                    var items = DeserializeToCollection(data, elementType);

                    // 適切な型に変換
                    if (type.IsArray)
                    {
                        // 配列に変換
                        Array array = Array.CreateInstance(elementType, items.Count);
                        for (int i = 0; i < items.Count; i++)
                        {
                            array.SetValue(items[i], i);
                        }
                        return (T)(object)array;
                    }
                    else if (type == typeof(List<>).MakeGenericType(elementType))
                    {
                        // Listに変換
                        dynamic list = Activator.CreateInstance(type)!;
                        foreach (var item in items)
                        {
                            list.Add(item);
                        }
                        return (T)(object)list;
                    }

                    // その他のIEnumerable実装型
                    return (T)(object)items;
                }
                else
                {
                    // 単一オブジェクトの場合、最初の行のみを使用
                    var items = DeserializeToCollection(data, typeof(T));
                    return items.Count > 0 ? (T)items[0] : default;
                }
            }
            catch (Exception ex)
            {
                throw new FormatException($"CSV文字列をデシリアライズできません: {ex.Message}", ex);
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

        private string SerializeCollection(IEnumerable<object> collection)
        {
            var sb = new StringBuilder();
            var items = collection.ToList();

            if (!items.Any())
                return string.Empty;

            // オブジェクトのプロパティを取得
            var properties = GetProperties(items[0].GetType());

            // ヘッダー行
            if (_includeHeader)
            {
                sb.AppendLine(string.Join(_delimiter, properties.Select(p => EscapeField(p.Name))));
            }

            // データ行
            foreach (var item in items)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);
                    return value != null ? EscapeField(value.ToString() ?? "") : "";
                });

                sb.AppendLine(string.Join(_delimiter, values));
            }

            return sb.ToString();
        }

        private List<object> DeserializeToCollection(string csv, Type targetType)
        {
            var result = new List<object>();
            var lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                return result;

            int startIndex = 0;
            string[] headers;

            // ヘッダー行が含まれているか確認
            if (_includeHeader)
            {
                headers = SplitLine(lines[0]);
                startIndex = 1;
            }
            else
            {
                // ヘッダーがない場合、プロパティ名を使用
                var properties = GetProperties(targetType);
                headers = properties.Select(p => p.Name).ToArray();
            }

            // データ行からオブジェクトを生成
            for (int i = startIndex; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var values = SplitLine(lines[i]);
                var obj = Activator.CreateInstance(targetType);

                for (int j = 0; j < Math.Min(headers.Length, values.Length); j++)
                {
                    var property = targetType.GetProperty(headers[j]);
                    if (property != null && property.CanWrite)
                    {
                        var value = ConvertValue(values[j], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                }

                result.Add(obj!);
            }

            return result;
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

        private object? ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            // 基本型に変換
            if (targetType == typeof(string))
                return value;
            else if (targetType == typeof(int) || targetType == typeof(int?))
                return int.Parse(value, CultureInfo.InvariantCulture);
            else if (targetType == typeof(long) || targetType == typeof(long?))
                return long.Parse(value, CultureInfo.InvariantCulture);
            else if (targetType == typeof(double) || targetType == typeof(double?))
                return double.Parse(value, CultureInfo.InvariantCulture);
            else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                return decimal.Parse(value, CultureInfo.InvariantCulture);
            else if (targetType == typeof(bool) || targetType == typeof(bool?))
                return bool.Parse(value);
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            else if (targetType == typeof(Guid) || targetType == typeof(Guid?))
                return Guid.Parse(value);
            else if (targetType.IsEnum)
                return Enum.Parse(targetType, value);

            // 他の型への変換が必要な場合は追加
            return Convert.ChangeType(value, targetType);
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

        private PropertyInfo[] GetProperties(Type type)
        {
            // シリアライズ対象のプロパティを取得（パブリックかつインスタンスプロパティ）
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        #endregion
    }
}
