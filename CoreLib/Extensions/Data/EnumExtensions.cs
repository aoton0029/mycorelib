using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extensions
{
    /// <summary>
    /// 列挙型の拡張メソッド
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 列挙型の説明を取得
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// 文字列から列挙型に変換
        /// </summary>
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// 列挙型の値が含まれているか確認
        /// </summary>
        public static bool In<T>(this T value, params T[] values) where T : Enum
        {
            return Array.IndexOf(values, value) >= 0;
        }

        /// <summary>
        /// 列挙型のすべての値を取得
        /// </summary>
        public static IEnumerable<T> GetAllValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// 列挙型の名前と値のペアを取得
        /// </summary>
        public static IEnumerable<(string Name, T Value)> GetNameValuePairs<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => (v.ToString(), v));
        }

        /// <summary>
        /// 列挙型の説明と値のペアを取得
        /// </summary>
        public static IEnumerable<(string Description, T Value)> GetDescriptionValuePairs<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => (v.GetDescription(), v));
        }

        /// <summary>
        /// 列挙型にフラグが含まれているかを確認
        /// </summary>
        public static bool HasFlag<T>(this T value, T flag) where T : Enum
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(long))
                return ((long)(object)value & (long)(object)flag) == (long)(object)flag;
            if (underlyingType == typeof(ulong))
                return ((ulong)(object)value & (ulong)(object)flag) == (ulong)(object)flag;
            if (underlyingType == typeof(int))
                return ((int)(object)value & (int)(object)flag) == (int)(object)flag;
            if (underlyingType == typeof(uint))
                return ((uint)(object)value & (uint)(object)flag) == (uint)(object)flag;
            if (underlyingType == typeof(short))
                return ((short)(object)value & (short)(object)flag) == (short)(object)flag;
            if (underlyingType == typeof(ushort))
                return ((ushort)(object)value & (ushort)(object)flag) == (ushort)(object)flag;
            if (underlyingType == typeof(byte))
                return ((byte)(object)value & (byte)(object)flag) == (byte)(object)flag;
            if (underlyingType == typeof(sbyte))
                return ((sbyte)(object)value & (sbyte)(object)flag) == (sbyte)(object)flag;

            throw new ArgumentException($"Type {typeof(T).Name} is not supported.", nameof(value));
        }

        /// <summary>
        /// 列挙型の値に特定のフラグを追加
        /// </summary>
        public static T AddFlag<T>(this T value, T flag) where T : Enum
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(long))
                return (T)(object)((long)(object)value | (long)(object)flag);
            if (underlyingType == typeof(ulong))
                return (T)(object)((ulong)(object)value | (ulong)(object)flag);
            if (underlyingType == typeof(int))
                return (T)(object)((int)(object)value | (int)(object)flag);
            if (underlyingType == typeof(uint))
                return (T)(object)((uint)(object)value | (uint)(object)flag);
            if (underlyingType == typeof(short))
                return (T)(object)((short)(object)value | (short)(object)flag);
            if (underlyingType == typeof(ushort))
                return (T)(object)((ushort)(object)value | (ushort)(object)flag);
            if (underlyingType == typeof(byte))
                return (T)(object)((byte)(object)value | (byte)(object)flag);
            if (underlyingType == typeof(sbyte))
                return (T)(object)((sbyte)(object)value | (sbyte)(object)flag);

            throw new ArgumentException($"Type {typeof(T).Name} is not supported.", nameof(value));
        }

        /// <summary>
        /// 列挙型の値から特定のフラグを削除
        /// </summary>
        public static T RemoveFlag<T>(this T value, T flag) where T : Enum
        {
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));

            if (underlyingType == typeof(long))
                return (T)(object)((long)(object)value & ~(long)(object)flag);
            if (underlyingType == typeof(ulong))
                return (T)(object)((ulong)(object)value & ~(ulong)(object)flag);
            if (underlyingType == typeof(int))
                return (T)(object)((int)(object)value & ~(int)(object)flag);
            if (underlyingType == typeof(uint))
                return (T)(object)((uint)(object)value & ~(uint)(object)flag);
            if (underlyingType == typeof(short))
                return (T)(object)((short)(object)value & ~(short)(object)flag);
            if (underlyingType == typeof(ushort))
                return (T)(object)((ushort)(object)value & ~(ushort)(object)flag);
            if (underlyingType == typeof(byte))
                return (T)(object)((byte)(object)value & ~(byte)(object)flag);
            if (underlyingType == typeof(sbyte))
                return (T)(object)((sbyte)(object)value & ~(sbyte)(object)flag);

            throw new ArgumentException($"Type {typeof(T).Name} is not supported.", nameof(value));
        }

        /// <summary>
        /// 列挙型の値のフラグを反転
        /// </summary>
        public static T ToggleFlag<T>(this T value, T flag) where T : Enum
        {
            return value.HasFlag(flag) ? value.RemoveFlag(flag) : value.AddFlag(flag);
        }
    }
}
