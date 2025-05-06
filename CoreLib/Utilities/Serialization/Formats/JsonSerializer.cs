using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Formats
{
    /// <summary>
    /// JSONシリアライザー実装
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// JSONシリアライザーのコンストラクタ
        /// </summary>
        public JsonSerializer(JsonSerializerOptions? options = null)
        {
            _options = options ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        /// <summary>
        /// サポートするフォーマット
        /// </summary>
        public DataFormat Format => DataFormat.Json;

        /// <summary>
        /// オブジェクトをJSON文字列にシリアライズ
        /// </summary>
        public string Serialize<T>(T obj)
        {
            if (obj == null)
                return string.Empty;

            return System.Text.Json.JsonSerializer.Serialize(obj, _options);
        }

        /// <summary>
        /// JSON文字列からオブジェクトにデシリアライズ
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(data, _options);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"JSON文字列をデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをストリームにシリアライズ
        /// </summary>
        public async Task SerializeToStreamAsync<T>(T obj, Stream stream, CancellationToken cancellationToken = default)
        {
            if (obj == null || stream == null)
                return;

            await System.Text.Json.JsonSerializer.SerializeAsync(stream, obj, _options, cancellationToken);
        }

        /// <summary>
        /// ストリームからオブジェクトにデシリアライズ
        /// </summary>
        public async Task<T?> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null || stream.Length == 0)
                return default;

            try
            {
                return await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"JSONストリームをデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをJSONファイルに保存
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
        /// JSONファイルからオブジェクトを読み込み
        /// </summary>
        public async Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return default;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await DeserializeFromStreamAsync<T>(fileStream, cancellationToken);
        }
    }
}
