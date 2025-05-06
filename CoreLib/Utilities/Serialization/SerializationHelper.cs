using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization
{
    /// <summary>
    /// シリアライズ操作のヘルパークラス
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// オブジェクトを指定フォーマットでシリアライズ
        /// </summary>
        public static string Serialize<T>(T obj, DataFormat format = DataFormat.Json)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var serializer = SerializerFactory.GetSerializer(format);
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// 文字列を指定フォーマットからデシリアライズ
        /// </summary>
        public static T? Deserialize<T>(string data, DataFormat format = DataFormat.Json)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("データが空です。", nameof(data));

            var serializer = SerializerFactory.GetSerializer(format);
            return serializer.Deserialize<T>(data);
        }

        /// <summary>
        /// オブジェクトをファイルに保存（拡張子からフォーマットを自動判定）
        /// </summary>
        public static async Task SaveToFileAsync<T>(T obj, string filePath, CancellationToken cancellationToken = default)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("ファイルパスが指定されていません。", nameof(filePath));

            string extension = Path.GetExtension(filePath);
            var serializer = SerializerFactory.GetSerializerFromExtension(extension);
            await serializer.SerializeToFileAsync(obj, filePath, cancellationToken);
        }

        /// <summary>
        /// ファイルからオブジェクトを読み込み（拡張子からフォーマットを自動判定）
        /// </summary>
        public static async Task<T?> LoadFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("ファイルパスが指定されていません。", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("指定されたファイルが見つかりません。", filePath);

            string extension = Path.GetExtension(filePath);
            var serializer = SerializerFactory.GetSerializerFromExtension(extension);
            return await serializer.DeserializeFromFileAsync<T>(filePath, cancellationToken);
        }

        /// <summary>
        /// あるオブジェクトをシリアライズして別の型にデシリアライズすることで型変換を行う
        /// </summary>
        public static TOutput? Convert<TInput, TOutput>(TInput input, DataFormat format = DataFormat.Json)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var serializer = SerializerFactory.GetSerializer(format);
            string serialized = serializer.Serialize(input);
            return serializer.Deserialize<TOutput>(serialized);
        }

        /// <summary>
        /// 深いコピーを作成（完全に新しいインスタンスを作成）
        /// </summary>
        public static T? DeepCopy<T>(T obj, DataFormat format = DataFormat.Json)
        {
            if (obj == null)
                return default;

            var serializer = SerializerFactory.GetSerializer(format);
            string serialized = serializer.Serialize(obj);
            return serializer.Deserialize<T>(serialized);
        }
    }
}
