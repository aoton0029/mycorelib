using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Mapping
{
    /// <summary>
    /// オブジェクトマッピングユーティリティ
    /// </summary>
    public static class ObjectMapper
    {
        /// <summary>
        /// ソースオブジェクトからターゲットオブジェクトへ同名プロパティの値をコピー
        /// </summary>
        /// <typeparam name="TSource">ソースオブジェクトの型</typeparam>
        /// <typeparam name="TTarget">ターゲットオブジェクトの型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <param name="target">ターゲットオブジェクト</param>
        /// <param name="ignoreProperties">無視するプロパティ名</param>
        /// <returns>更新されたターゲットオブジェクト</returns>
        public static TTarget Map<TSource, TTarget>(
            TSource source,
            TTarget target,
            params string[] ignoreProperties)
            where TTarget : class
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var ignoredProps = new HashSet<string>(ignoreProperties, StringComparer.OrdinalIgnoreCase);
            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties()
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var sourceProp in sourceProperties)
            {
                // 無視するプロパティはスキップ
                if (ignoredProps.Contains(sourceProp.Name))
                    continue;

                // ターゲットに同名のプロパティがあるかチェック
                if (targetProperties.TryGetValue(sourceProp.Name, out var targetProp))
                {
                    // 型が代入可能かチェック
                    if (IsAssignable(sourceProp.PropertyType, targetProp.PropertyType))
                    {
                        // 値をコピー
                        var value = sourceProp.GetValue(source);
                        targetProp.SetValue(target, value);
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// ソースオブジェクトから新しいターゲットオブジェクトを作成してプロパティをコピー
        /// </summary>
        /// <typeparam name="TSource">ソースオブジェクトの型</typeparam>
        /// <typeparam name="TTarget">ターゲットオブジェクトの型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <param name="ignoreProperties">無視するプロパティ名</param>
        /// <returns>生成されたターゲットオブジェクト</returns>
        public static TTarget MapToNew<TSource, TTarget>(
            TSource source,
            params string[] ignoreProperties)
            where TTarget : class, new()
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var target = new TTarget();
            return Map(source, target, ignoreProperties);
        }

        /// <summary>
        /// コレクションの各要素をマッピングして新しいリストを作成
        /// </summary>
        /// <typeparam name="TSource">ソースオブジェクトの型</typeparam>
        /// <typeparam name="TTarget">ターゲットオブジェクトの型</typeparam>
        /// <param name="sourceCollection">ソースコレクション</param>
        /// <param name="ignoreProperties">無視するプロパティ名</param>
        /// <returns>マッピングされた新しいリスト</returns>
        public static List<TTarget> MapList<TSource, TTarget>(
            IEnumerable<TSource> sourceCollection,
            params string[] ignoreProperties)
            where TTarget : class, new()
            where TSource : class
        {
            if (sourceCollection == null)
                throw new ArgumentNullException(nameof(sourceCollection));

            var result = new List<TTarget>();
            foreach (var item in sourceCollection)
            {
                result.Add(MapToNew<TSource, TTarget>(item, ignoreProperties));
            }
            return result;
        }

        /// <summary>
        /// オブジェクトの一部のプロパティだけを取得して新しいオブジェクトを作成
        /// </summary>
        /// <typeparam name="TSource">ソースオブジェクトの型</typeparam>
        /// <typeparam name="TTarget">ターゲットオブジェクトの型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <param name="includeProperties">含めるプロパティ名</param>
        /// <returns>生成されたターゲットオブジェクト</returns>
        public static TTarget MapSelected<TSource, TTarget>(
            TSource source,
            params string[] includeProperties)
            where TTarget : class, new()
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (includeProperties == null || includeProperties.Length == 0)
                throw new ArgumentException("含めるプロパティを少なくとも1つ指定してください", nameof(includeProperties));

            var includedProps = new HashSet<string>(includeProperties, StringComparer.OrdinalIgnoreCase);
            var target = new TTarget();

            var sourceProperties = typeof(TSource).GetProperties()
                .Where(p => includedProps.Contains(p.Name))
                .ToList();

            var targetProperties = typeof(TTarget).GetProperties()
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var sourceProp in sourceProperties)
            {
                if (targetProperties.TryGetValue(sourceProp.Name, out var targetProp))
                {
                    if (IsAssignable(sourceProp.PropertyType, targetProp.PropertyType))
                    {
                        var value = sourceProp.GetValue(source);
                        targetProp.SetValue(target, value);
                    }
                }
            }

            return target;
        }

        /// <summary>
        /// ディープコピーを実行（シンプルなオブジェクトに対して）
        /// </summary>
        /// <typeparam name="T">オブジェクトの型</typeparam>
        /// <param name="source">ソースオブジェクト</param>
        /// <returns>ディープコピーされたオブジェクト</returns>
        public static T DeepCopy<T>(T source) where T : class, new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var target = new T();
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanWrite && p.CanRead);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source);
                if (value == null)
                {
                    prop.SetValue(target, null);
                    continue;
                }

                var propType = prop.PropertyType;
                if (propType.IsPrimitive || propType == typeof(string) || propType == typeof(decimal)
                    || propType == typeof(DateTime) || propType == typeof(DateTimeOffset)
                    || propType == typeof(TimeSpan) || propType == typeof(Guid))
                {
                    // 値型やイミュータブルな型は直接コピー
                    prop.SetValue(target, value);
                }
                else if (propType.IsClass && !propType.IsArray)
                {
                    // クラス型はリフレクションでディープコピー（簡易版）
                    var deepCopyMethod = typeof(ObjectMapper).GetMethod(nameof(DeepCopy))?.MakeGenericMethod(propType);
                    if (deepCopyMethod != null)
                    {
                        var copiedValue = deepCopyMethod.Invoke(null, new[] { value });
                        prop.SetValue(target, copiedValue);
                    }
                }
                // 配列やコレクションは必要に応じて追加実装
            }

            return target;
        }

        /// <summary>
        /// 型が代入可能かどうかをチェック
        /// </summary>
        private static bool IsAssignable(Type sourceType, Type targetType)
        {
            if (targetType.IsAssignableFrom(sourceType))
                return true;

            // null許容値型への変換をサポート
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return sourceType == Nullable.GetUnderlyingType(targetType);
            }

            return false;
        }
    }
}
