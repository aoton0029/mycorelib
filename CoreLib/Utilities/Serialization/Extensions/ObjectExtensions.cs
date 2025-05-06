using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization.Extensions
{
    /// <summary>
    /// オブジェクトシリアライズ拡張メソッド
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// オブジェクトを指定フォーマットの文字列にシリアライズ
        /// </summary>
        public static string ToSerializedString<T>(this T obj, DataFormat format = DataFormat.Json)
        {
            return SerializationHelper.Serialize(obj, format);
        }

        /// <summary>
        /// オブジェクトを指定フォーマットでファイルに保存
        /// </summary>
        public static Task SaveToFileAsync<T>(this T obj, string filePath)
        {
            return SerializationHelper.SaveToFileAsync(obj, filePath);
        }

        /// <summary>
        /// オブジェクトをJSON文字列に変換（ショートカット）
        /// </summary>
        public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
        {
            if (obj == null)
                return string.Empty;

            options ??= IO.Formats.JsonHelper.DefaultOptions;
            return System.Text.Json.JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// オブジェクトの深いコピーを作成
        /// </summary>
        public static T? DeepClone<T>(this T obj, DataFormat format = DataFormat.Json)
        {
            return SerializationHelper.DeepCopy(obj, format);
        }
    }
}
