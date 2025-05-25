using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Extensions
{
    /// <summary>
    /// ストリームシリアライズ拡張メソッド
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// ストリームにオブジェクトをシリアライズ
        /// </summary>
        public static Task SerializeObjectAsync<T>(this Stream stream, T obj, DataFormat format = DataFormat.Json, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = SerializerFactory.GetSerializer(format);
            return serializer.SerializeToStreamAsync(obj, stream, cancellationToken);
        }

        /// <summary>
        /// ストリームからオブジェクトをデシリアライズ
        /// </summary>
        public static Task<T?> DeserializeObjectAsync<T>(this Stream stream, DataFormat format = DataFormat.Json, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = SerializerFactory.GetSerializer(format);
            return serializer.DeserializeFromStreamAsync<T>(stream, cancellationToken);
        }

        /// <summary>
        /// MemoryStreamのデータをファイルに保存
        /// </summary>
        public static async Task SaveToFileAsync(this MemoryStream memoryStream, string filePath)
        {
            if (memoryStream == null)
                throw new ArgumentNullException(nameof(memoryStream));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("ファイルパスが指定されていません。", nameof(filePath));

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(fileStream);
        }
    }
}
