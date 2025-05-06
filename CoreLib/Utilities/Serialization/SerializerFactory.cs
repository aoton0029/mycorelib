using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoreLib.Utilities.Serialization
{
    /// <summary>
    /// シリアライザーファクトリ
    /// </summary>
    public static class SerializerFactory
    {
        private static readonly Dictionary<DataFormat, Lazy<ISerializer>> _serializers = new()
        {
            { DataFormat.Json, new Lazy<ISerializer>(() => new JsonSerializer()) },
            { DataFormat.Xml, new Lazy<ISerializer>(() => new XmlSerializer()) },
            { DataFormat.Binary, new Lazy<ISerializer>(() => new BinarySerializer()) },
            // 必要に応じて他のシリアライザーを追加
        };

        /// <summary>
        /// 指定された形式のシリアライザーを取得
        /// </summary>
        public static ISerializer GetSerializer(DataFormat format)
        {
            if (_serializers.TryGetValue(format, out var lazySerializer))
            {
                return lazySerializer.Value;
            }

            throw new NotSupportedException($"指定されたフォーマット '{format}' はサポートされていません。");
        }

        /// <summary>
        /// ファイル拡張子からシリアライザーを取得
        /// </summary>
        public static ISerializer GetSerializerFromExtension(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                throw new ArgumentException("ファイル拡張子が指定されていません。", nameof(fileExtension));

            // 先頭のドットを除去
            fileExtension = fileExtension.TrimStart('.').ToLowerInvariant();

            return fileExtension switch
            {
                "json" => GetSerializer(DataFormat.Json),
                "xml" => GetSerializer(DataFormat.Xml),
                "bin" or "dat" => GetSerializer(DataFormat.Binary),
                // 必要に応じて他の拡張子を追加
                _ => throw new NotSupportedException($"ファイル拡張子 '{fileExtension}' はサポートされていません。")
            };
        }

        /// <summary>
        /// カスタムシリアライザーを登録
        /// </summary>
        public static void RegisterSerializer(DataFormat format, ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _serializers[format] = new Lazy<ISerializer>(() => serializer);
        }
    }
}
