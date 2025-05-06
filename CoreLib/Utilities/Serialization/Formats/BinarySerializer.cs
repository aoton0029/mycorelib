using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Formats
{
    /// <summary>
    /// バイナリシリアライザー実装
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        /// <summary>
        /// サポートするフォーマット
        /// </summary>
        public DataFormat Format => DataFormat.Binary;

        /// <summary>
        /// オブジェクトをバイナリ文字列（Base64）にシリアライズ
        /// </summary>
        public string Serialize<T>(T obj)
        {
            if (obj == null)
                return string.Empty;

            using var memoryStream = new MemoryStream();
            SerializeToStream(obj, memoryStream);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// バイナリ文字列（Base64）からオブジェクトにデシリアライズ
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                using var memoryStream = new MemoryStream(bytes);
                return DeserializeFromStream<T>(memoryStream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"バイナリデータをデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをストリームにバイナリシリアライズ
        /// </summary>
        public Task SerializeToStreamAsync<T>(T obj, Stream stream, CancellationToken cancellationToken = default)
        {
            if (obj == null || stream == null)
                return Task.CompletedTask;

            SerializeToStream(obj, stream);
            return Task.CompletedTask;
        }

        /// <summary>
        /// ストリームからオブジェクトにバイナリデシリアライズ
        /// </summary>
        public Task<T?> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null || stream.Length == 0)
                return Task.FromResult<T?>(default);

            try
            {
                var result = DeserializeFromStream<T>(stream);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"バイナリストリームをデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをバイナリファイルに保存
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
            await Task.Run(() => SerializeToStream(obj, fileStream), cancellationToken);
        }

        /// <summary>
        /// バイナリファイルからオブジェクトを読み込み
        /// </summary>
        public async Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return default;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await Task.Run(() => DeserializeFromStream<T>(fileStream), cancellationToken);
        }

        // 補助メソッド
        private void SerializeToStream<T>(T obj, Stream stream)
        {
#pragma warning disable SYSLIB0011 // BinaryFormatterは安全でないため非推奨
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011
        }

        private T? DeserializeFromStream<T>(Stream stream)
        {
#pragma warning disable SYSLIB0011 // BinaryFormatterは安全でないため非推奨
            var formatter = new BinaryFormatter();
            return (T?)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
        }
    }
}
