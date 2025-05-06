using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// 読み取り専用リポジトリインターフェース
    /// </summary>
    public interface IReadRepository<T, TKey> where T : class
    {
        /// <summary>
        /// IDによるエンティティの取得
        /// </summary>
        Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// すべてのエンティティの取得
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 条件に一致するエンティティの取得
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 条件に一致する単一エンティティの取得
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// エンティティの存在確認
        /// </summary>
        Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// エンティティの件数取得
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 書き込み操作リポジトリインターフェース
    /// </summary>
    public interface IWriteRepository<T, TKey> where T : class
    {
        /// <summary>
        /// エンティティの追加
        /// </summary>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数エンティティの追加
        /// </summary>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// エンティティの更新
        /// </summary>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数エンティティの更新
        /// </summary>
        Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// エンティティの削除
        /// </summary>
        Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// エンティティの削除
        /// </summary>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数エンティティの削除
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 完全なリポジトリインターフェース（読み書き）
    /// </summary>
    public interface IRepository<T, TKey> : IReadRepository<T, TKey>, IWriteRepository<T, TKey> where T : class
    {
    }

    /// <summary>
    /// Unit of Workパターン用インターフェース
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// トランザクションのコミット
        /// </summary>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションのロールバック
        /// </summary>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 新しいトランザクションの開始
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションのコミット
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// トランザクションのロールバック
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
