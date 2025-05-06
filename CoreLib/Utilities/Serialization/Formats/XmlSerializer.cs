using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoreLib.Utilities.Serialization.Formats
{
    /// <summary>
    /// XMLシリアライザー実装
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        /// <summary>
        /// サポートするフォーマット
        /// </summary>
        public DataFormat Format => DataFormat.Xml;

        /// <summary>
        /// オブジェクトをXML文字列にシリアライズ
        /// </summary>
        public string Serialize<T>(T obj)
        {
            if (obj == null)
                return string.Empty;

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true });

            serializer.Serialize(xmlWriter, obj);
            return stringWriter.ToString();
        }

        /// <summary>
        /// XML文字列からオブジェクトにデシリアライズ
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data))
                return default;

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var stringReader = new StringReader(data);

            try
            {
                return (T?)serializer.Deserialize(stringReader);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"XML文字列をデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをストリームにシリアライズ
        /// </summary>
        public Task SerializeToStreamAsync<T>(T obj, Stream stream, CancellationToken cancellationToken = default)
        {
            if (obj == null || stream == null)
                return Task.CompletedTask;

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Async = true });

            serializer.Serialize(xmlWriter, obj);
            return Task.CompletedTask;
        }

        /// <summary>
        /// ストリームからオブジェクトにデシリアライズ
        /// </summary>
        public Task<T?> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null || stream.Length == 0)
                return Task.FromResult<T?>(default);

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            try
            {
                var result = (T?)serializer.Deserialize(stream);
                return Task.FromResult(result);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"XMLストリームをデシリアライズできません: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// オブジェクトをXMLファイルに保存
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
            await Task.Run(() => SerializeToStreamAsync(obj, fileStream, cancellationToken));
        }

        /// <summary>
        /// XMLファイルからオブジェクトを読み込み
        /// </summary>
        public async Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return default;

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await Task.Run(() => DeserializeFromStreamAsync<T>(fileStream, cancellationToken));
        }
    }
}
