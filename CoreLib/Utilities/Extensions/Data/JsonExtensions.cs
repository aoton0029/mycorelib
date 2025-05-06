using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Data
{
    /// <summary>
    /// JSON関連の拡張メソッド
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// オブジェクトをJSON文字列に変換
        /// </summary>
        public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
        {
            return CoreLib.Utilities.IO.Formats.JsonHelper.Serialize(obj, options);
        }

        /// <summary>
        /// JSON文字列からオブジェクトに変換
        /// </summary>
        public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null)
        {
            return CoreLib.Utilities.IO.Formats.JsonHelper.Deserialize<T>(json, options);
        }

        /// <summary>
        /// JSONオブジェクトから特定のプロパティの値を取得
        /// </summary>
        public static T? GetPropertyValue<T>(this JsonNode node, string propertyPath, T? defaultValue = default)
        {
            if (node == null)
                return defaultValue;

            try
            {
                var pathSegments = propertyPath.Split('.');
                JsonNode? currentNode = node;

                foreach (var segment in pathSegments)
                {
                    if (currentNode == null)
                        return defaultValue;

                    currentNode = currentNode[segment];
                }

                if (currentNode == null)
                    return defaultValue;

                return currentNode.GetValue<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// JSONノードをオブジェクトに変換
        /// </summary>
        public static T? ToObject<T>(this JsonNode node, JsonSerializerOptions? options = null)
        {
            if (node == null)
                return default;

            options ??= CoreLib.Utilities.IO.Formats.JsonHelper.DefaultOptions;
            var json = node.ToJsonString(options);
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// ストリームからJSONオブジェクトを読み込む
        /// </summary>
        public static async Task<T?> ReadFromJsonAsync<T>(this Stream stream, JsonSerializerOptions? options = null)
        {
            if (stream == null || stream.Length == 0)
                return default;

            options ??= CoreLib.Utilities.IO.Formats.JsonHelper.DefaultOptions;
            return await JsonSerializer.DeserializeAsync<T>(stream, options);
        }

        /// <summary>
        /// オブジェクトをJSONとしてストリームに書き込む
        /// </summary>
        public static async Task WriteAsJsonAsync<T>(this Stream stream, T value, JsonSerializerOptions? options = null)
        {
            if (stream == null || value == null)
                return;

            options ??= CoreLib.Utilities.IO.Formats.JsonHelper.DefaultOptions;
            await JsonSerializer.SerializeAsync(stream, value, options);
        }

        /// <summary>
        /// JSONノードのプロパティを列挙
        /// </summary>
        public static IEnumerable<string> GetPropertyNames(this JsonNode node)
        {
            if (node == null || node is not JsonObject jsonObject)
                return Enumerable.Empty<string>();

            return jsonObject.Select(property => property.Key);
        }

        /// <summary>
        /// JSONオブジェクトをディクショナリに変換
        /// </summary>
        public static Dictionary<string, object> ToDictionary(this JsonObject jsonObject)
        {
            return CoreLib.Utilities.IO.Formats.JsonHelper.ToDictionary(jsonObject);
        }

        /// <summary>
        /// JSONをフォーマット
        /// </summary>
        public static string FormatJson(this string json)
        {
            return CoreLib.Utilities.IO.Formats.JsonHelper.Format(json);
        }
    }
}
