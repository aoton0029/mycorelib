using CoreLib.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Specifications
{
    /// <summary>
    /// リポジトリに仕様を適用するための拡張メソッド
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// リポジトリに仕様を適用してエンティティを検索
        /// </summary>
        public static async Task<IEnumerable<T>> FindAsync<T, TKey>(
            this IReadRepository<T, TKey> repository,
            ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class
        {
            // 仕様からクエリ式を構築
            var queryableResult = GetQuery(repository, specification);

            // コレクションとして結果を取得
            return await queryableResult.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// リポジトリに仕様を適用して単一エンティティを取得
        /// </summary>
        public static async Task<T?> FirstOrDefaultAsync<T, TKey>(
            this IReadRepository<T, TKey> repository,
            ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class
        {
            // 仕様からクエリ式を構築
            var queryableResult = GetQuery(repository, specification);

            // 最初の結果を取得
            return await queryableResult.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// リポジトリに仕様を適用した結果の件数を取得
        /// </summary>
        public static async Task<int> CountAsync<T, TKey>(
            this IReadRepository<T, TKey> repository,
            ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class
        {
            // 仕様からクエリ式を構築
            var queryableResult = GetQuery(repository, specification);

            // 件数を取得
            return await queryableResult.CountAsync(cancellationToken);
        }

        /// <summary>
        /// 仕様を適用したIQueryableを取得
        /// </summary>
        private static IQueryable<T> GetQuery<T, TKey>(
            IReadRepository<T, TKey> repository,
            ISpecification<T> specification) where T : class
        {
            // このメソッドはリポジトリの具体的な実装により異なる場合があります
            // リポジトリ実装クラスがIQueryableサポートしていない場合は適宜調整が必要

            // 例：EF Coreリポジトリの場合
            // return repository.GetQueryable().Where(specification.Criteria);

            // ここではリポジトリから全件取得して、仕様をメモリ上で適用する例
            var allEntities = repository.GetAllAsync().Result.AsQueryable();
            return SpecificationEvaluator<T>.GetQuery(allEntities, specification);
        }

        /// <summary>
        /// Task＜IEnumerable＜T＞＞をTask＜List＜T＞＞に変換
        /// </summary>
        private static async Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            // 実際にはORMの実装に依存します
            // 例：Entity Frameworkの場合は専用のToListAsyncメソッドを使用

            // ここではシンプルな実装例
            return await Task.FromResult(queryable.ToList());
        }

        /// <summary>
        /// Task＜IEnumerable＜T＞＞の最初の要素を取得
        /// </summary>
        private static async Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            // 実際にはORMの実装に依存します
            // 例：Entity Frameworkの場合は専用のFirstOrDefaultAsyncメソッドを使用

            // ここではシンプルな実装例
            return await Task.FromResult(queryable.FirstOrDefault());
        }

        /// <summary>
        /// Task＜IEnumerable＜T＞＞の件数を取得
        /// </summary>
        private static async Task<int> CountAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            // 実際にはORMの実装に依存します
            // 例：Entity Frameworkの場合は専用のCountAsyncメソッドを使用

            // ここではシンプルな実装例
            return await Task.FromResult(queryable.Count());
        }
    }
}
