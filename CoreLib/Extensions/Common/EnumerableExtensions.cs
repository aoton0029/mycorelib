using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.Common
{
    /// <summary>
    /// コレクションの拡張メソッド
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// シーケンスが空かnullでないかを確認
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// シーケンスが空でなく、かつnullでないことを確認
        /// </summary>
        public static bool HasItems<T>(this IEnumerable<T>? source)
        {
            return source != null && source.Any();
        }

        /// <summary>
        /// シーケンスをバッチに分割
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (batchSize <= 0)
                throw new ArgumentException("バッチサイズは1以上である必要があります", nameof(batchSize));

            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return GetBatch(enumerator, batchSize);
            }

            IEnumerable<T> GetBatch(IEnumerator<T> enumerator, int size)
            {
                do
                {
                    yield return enumerator.Current;
                } while (--size > 0 && enumerator.MoveNext());
            }
        }

        /// <summary>
        /// シーケンスをページングして取得
        /// </summary>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex < 0)
                throw new ArgumentException("ページインデックスは0以上である必要があります", nameof(pageIndex));
            if (pageSize <= 0)
                throw new ArgumentException("ページサイズは1以上である必要があります", nameof(pageSize));

            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// シーケンスをシャッフル
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var random = new Random();
            return source.OrderBy(_ => random.Next());
        }

        /// <summary>
        /// シーケンスの各要素に対してアクションを実行
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// シーケンスの各要素に対して非同期アクションを実行
        /// </summary>
        public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            foreach (var item in source)
            {
                await asyncAction(item);
            }
        }

        /// <summary>
        /// シーケンスの各要素に対して並列で非同期アクションを実行
        /// </summary>
        public static async Task ForEachParallelAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction, int maxDegreeOfParallelism = 0)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            var tasks = maxDegreeOfParallelism > 0
                ? source.Select(asyncAction).ToArray().Buffer(maxDegreeOfParallelism)
                : source.Select(asyncAction).ToArray();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// シーケンスを一定サイズのバッファに分割
        /// </summary>
        private static IEnumerable<Task> Buffer(this Task[] tasks, int bufferSize)
        {
            var activeTasks = new List<Task>(bufferSize);

            foreach (var task in tasks)
            {
                if (activeTasks.Count >= bufferSize)
                {
                    var completedTask = Task.WhenAny(activeTasks).Result;
                    activeTasks.Remove(completedTask);
                }
                activeTasks.Add(task);
                yield return task;
            }

            while (activeTasks.Count > 0)
            {
                var completedTask = Task.WhenAny(activeTasks).Result;
                activeTasks.Remove(completedTask);
                yield return completedTask;
            }
        }

        /// <summary>
        /// シーケンスから重複を除去（プロパティに基づく）
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// シーケンスに要素を追加
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] items)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var item in source)
            {
                yield return item;
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// シーケンスの先頭に要素を追加
        /// </summary>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] items)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (items != null)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }

            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// シーケンスを指定された型のリストに変換（null安全）
        /// </summary>
        public static List<T> ToSafeList<T>(this IEnumerable<T>? source)
        {
            return source?.ToList() ?? new List<T>();
        }

        /// <summary>
        /// シーケンスを指定された型の配列に変換（null安全）
        /// </summary>
        public static T[] ToSafeArray<T>(this IEnumerable<T>? source)
        {
            return source?.ToArray() ?? Array.Empty<T>();
        }
    }
}
