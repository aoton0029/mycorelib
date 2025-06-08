using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extensions.Common
{
    /// <summary>
    /// HashSet<T>クラスの拡張メソッドを提供します。
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// 指定したコレクションの要素をHashSetに追加し、追加された要素数を返します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="hashSet">拡張対象のHashSet</param>
        /// <param name="items">追加する要素のコレクション</param>
        /// <returns>HashSetに追加された要素の数</returns>
        public static int AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));
            if (items == null) throw new ArgumentNullException(nameof(items));

            int addedCount = 0;
            foreach (var item in items)
            {
                if (hashSet.Add(item))
                {
                    addedCount++;
                }
            }
            return addedCount;
        }

        /// <summary>
        /// 指定したHashSetとの差集合を新しいHashSetとして返します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="hashSet">拡張対象のHashSet</param>
        /// <param name="other">差し引く要素を含むHashSet</param>
        /// <returns>差集合を含む新しいHashSet</returns>
        public static HashSet<T> Except<T>(this HashSet<T> hashSet, HashSet<T> other)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));

            var result = new HashSet<T>(hashSet, hashSet.Comparer);
            if (other != null)
            {
                result.ExceptWith(other);
            }
            return result;
        }

        /// <summary>
        /// 指定したHashSetとの積集合を新しいHashSetとして返します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="hashSet">拡張対象のHashSet</param>
        /// <param name="other">積集合を取る要素を含むHashSet</param>
        /// <returns>積集合を含む新しいHashSet</returns>
        public static HashSet<T> Intersect<T>(this HashSet<T> hashSet, HashSet<T> other)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));

            var result = new HashSet<T>(hashSet, hashSet.Comparer);
            if (other != null)
            {
                result.IntersectWith(other);
            }
            return result;
        }

        /// <summary>
        /// 指定したHashSetとの和集合を新しいHashSetとして返します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="hashSet">拡張対象のHashSet</param>
        /// <param name="other">和集合を取る要素を含むHashSet</param>
        /// <returns>和集合を含む新しいHashSet</returns>
        public static HashSet<T> Union<T>(this HashSet<T> hashSet, HashSet<T> other)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));

            var result = new HashSet<T>(hashSet, hashSet.Comparer);
            if (other != null)
            {
                result.UnionWith(other);
            }
            return result;
        }

        /// <summary>
        /// HashSetを指定した値で初期化します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="items">初期値となる要素のコレクション</param>
        /// <returns>指定した要素を含む新しいHashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return new HashSet<T>(items);
        }

        /// <summary>
        /// HashSetを指定したEqualityComparerで初期化します。
        /// </summary>
        /// <typeparam name="T">HashSetの要素の型</typeparam>
        /// <param name="items">初期値となる要素のコレクション</param>
        /// <param name="comparer">要素の比較に使用するIEqualityComparerの実装</param>
        /// <returns>指定した要素を含む新しいHashSet</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return new HashSet<T>(items, comparer);
        }
    }
}
