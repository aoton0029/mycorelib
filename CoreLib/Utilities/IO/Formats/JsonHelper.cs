using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Formats
{
    /// <summary>
    /// JSON操作ユーティリティ
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 既定のJSONシリアライズオプション
        /// </summary>
        public static JsonSerializerOptions DefaultOptions => new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// オブジェクトをJSON文字列にシリアライズ
        /// </summary>
        public static string Serialize<T>(T obj, JsonSerializerOptions? options = null)
        {
            if (obj == null)
                return string.Empty;

            options ??= DefaultOptions;
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// JSON文字列からオブジェクトへデシリアライズ
        /// </summary>
        public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            options ??= DefaultOptions;

            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (JsonException ex)
            {
                throw new JsonException($"JSON文字列'{json.Substring(0, Math.Min(100, json.Length))}...'を{typeof(T).Name}型にデシリアライズできません", ex);
            }
        }

        /// <summary>
        /// ファイルからJSONを読み込みオブジェクトにデシリアライズ
        /// </summary>
        public static async Task<T?> LoadFromFileAsync<T>(string filePath, JsonSerializerOptions? options = null)
        {
            if (!File.Exists(filePath))
                return default;

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                return Deserialize<T>(json, options);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new IOException($"ファイル'{filePath}'からJSONを読み込めません", ex);
            }
        }

        /// <summary>
        /// オブジェクトをシリアライズしてファイルに保存
        /// </summary>
        public static async Task SaveToFileAsync<T>(T obj, string filePath, JsonSerializerOptions? options = null)
        {
            if (obj == null)
                return;

            // 出力先ディレクトリを作成
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // JSONシリアライズ
            string json = Serialize(obj, options);

            try
            {
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new IOException($"JSONをファイル'{filePath}'に保存できません", ex);
            }
        }

        /// <summary>
        /// JSONオブジェクトから指定キーの値を取得
        /// </summary>
        public static T? GetValue<T>(JsonNode jsonNode, string key, T? defaultValue = default)
        {
            if (jsonNode == null || string.IsNullOrEmpty(key))
                return defaultValue;

            try
            {
                var value = jsonNode[key];
                if (value == null)
                    return defaultValue;

                return value.GetValue<T>();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// JSON文字列からJSONノードツリーに変換
        /// </summary>
        public static JsonNode? Parse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonNode.Parse(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 辞書をJSONオブジェクトに変換
        /// </summary>
        public static JsonObject ToJsonObject(IDictionary<string, object> dictionary)
        {
            var jsonObject = new JsonObject();

            foreach (var kvp in dictionary)
            {
                if (kvp.Value == null)
                    continue;

                JsonNode? node = null;

                // 基本型の場合
                if (kvp.Value is string strValue)
                    node = JsonValue.Create(strValue);
                else if (kvp.Value is int intValue)
                    node = JsonValue.Create(intValue);
                else if (kvp.Value is long longValue)
                    node = JsonValue.Create(longValue);
                else if (kvp.Value is double doubleValue)
                    node = JsonValue.Create(doubleValue);
                else if (kvp.Value is decimal decimalValue)
                    node = JsonValue.Create(decimalValue);
                else if (kvp.Value is bool boolValue)
                    node = JsonValue.Create(boolValue);
                // それ以外はオブジェクトとしてシリアライズ
                else
                {
                    string json = Serialize(kvp.Value);
                    node = Parse(json);
                }

                if (node != null)
                    jsonObject.Add(kvp.Key, node);
            }

            return jsonObject;
        }

        /// <summary>
        /// JSONオブジェクトを辞書に変換
        /// </summary>
        public static Dictionary<string, object> ToDictionary(JsonObject jsonObject)
        {
            var result = new Dictionary<string, object>();

            foreach (var property in jsonObject)
            {
                var key = property.Key;
                var value = property.Value;

                if (value == null)
                    continue;

                // JsonObjectの場合は再帰的に変換
                if (value is JsonObject nestedObject)
                {
                    result[key] = ToDictionary(nestedObject);
                }
                // JsonArrayの場合は配列に変換
                else if (value is JsonArray jsonArray)
                {
                    var array = new object[jsonArray.Count];
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        var item = jsonArray[i];
                        if (item is JsonObject itemObject)
                            array[i] = ToDictionary(itemObject);
                        else
                            array[i] = item?.GetValue<object>() ?? new object();
                    }
                    result[key] = array;
                }
                // それ以外は値として取得
                else
                {
                    result[key] = value.GetValue<object>();
                }
            }

            return result;
        }

        /// <summary>
        /// JSON文字列をフォーマット（整形）する
        /// </summary>
        public static string Format(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            try
            {
                // 一度パースして整形済みで出力
                var jsonObj = JsonSerializer.Deserialize<object>(json);
                return JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return json; // パースに失敗した場合は元の文字列を返す
            }
        }

        /// <summary>
        /// JSONオブジェクトを別の型に変換
        /// </summary>
        public static TOutput? Convert<TInput, TOutput>(TInput input, JsonSerializerOptions? options = null)
        {
            if (input == null)
                return default;

            options ??= DefaultOptions;

            // 一度JSONにシリアライズしてから目的の型にデシリアライズ
            string json = Serialize(input, options);
            return Deserialize<TOutput>(json, options);
        }

        /// <summary>
        /// JSONが有効な形式かチェック
        /// </summary>
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
