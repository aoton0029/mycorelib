using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Specifications
{
    /// <summary>
    /// 仕様をクエリに適用するエバリュエーター
    /// </summary>
    public class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// 仕様に基づいてクエリを構築
        /// </summary>
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // トラッキングの設定
            if (specification.AsNoTracking)
            {
                // EntityFrameworkなどのORMが使用されている場合に適用
                // EF Core用のメソッド呼び出しの例
                // query = query.AsNoTracking();
            }

            // 条件適用
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // インクルード適用
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            // 文字列型のインクルード適用
            query = specification.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));

            // ソート適用（昇順）
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }

            // ソート適用（降順）
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // 追加のソート適用（昇順）
            foreach (var thenBy in specification.ThenByList)
            {
                query = ((IOrderedQueryable<T>)query).ThenBy(thenBy);
            }

            // 追加のソート適用（降順）
            foreach (var thenByDescending in specification.ThenByDescendingList)
            {
                query = ((IOrderedQueryable<T>)query).ThenByDescending(thenByDescending);
            }

            // グループ化適用
            if (specification.GroupBy != null)
            {
                query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
            }

            // ページング適用
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);
            }

            return query;
        }
    }

    // 拡張メソッドの追加
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source, System.Linq.Expressions.Expression<Func<T, TProperty>> navigationPropertyPath)
            where T : class
        {
            // 注: 実際の実装はORMに依存します（例: Entity Framework Core）
            // EntityFrameworkを使用していない場合は、この部分を適切に実装/変更してください
            return source;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, string navigationPropertyPath)
            where T : class
        {
            // 注: 実際の実装はORMに依存します（例: Entity Framework Core）
            // EntityFrameworkを使用していない場合は、この部分を適切に実装/変更してください
            return source;
        }
    }
}
