using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// キャッシュサービスインターフェース
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// キャッシュからデータを取得
        /// </summary>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// データをキャッシュに設定
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// キャッシュからデータを削除
        /// </summary>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// キャッシュの存在確認
        /// </summary>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// パターンに一致するキャッシュを削除
        /// </summary>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// キャッシュをクリア
        /// </summary>
        Task ClearAsync(CancellationToken cancellationToken = default);
    }
}
