using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Common
{
    /// <summary>
    /// リフレクション操作のための拡張メソッド
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// プロパティの説明属性から説明文を取得
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// プロパティ値を取得（文字列の名前からリフレクションで）
        /// </summary>
        public static object? GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("プロパティ名は必須です", nameof(propertyName));

            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj);
        }

        /// <summary>
        /// プロパティ値を設定（文字列の名前からリフレクションで）
        /// </summary>
        public static void SetPropertyValue(this object obj, string propertyName, object? value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("プロパティ名は必須です", nameof(propertyName));

            var property = obj.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }

        /// <summary>
        /// オブジェクトの全プロパティをディクショナリに変換
        /// </summary>
        public static Dictionary<string, object?> ToDictionary(this object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj.GetType().GetProperties()
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(obj)
                );
        }

        /// <summary>
        /// 指定された属性を持つプロパティのみを抽出
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetProperties()
                .Where(p => p.GetCustomAttributes<TAttribute>().Any());
        }
    }
}
