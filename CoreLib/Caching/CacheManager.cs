using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Caching
{
    /// <summary>
    /// キャッシュマネージャーの設定
    /// </summary>
    public class CacheManagerOptions
    {
        /// <summary>
        /// デフォルトのキャッシュ期間（秒）
        /// </summary>
        public int DefaultExpirationSeconds { get; set; } = 300; // 5分

        /// <summary>
        /// スライディング有効期限を使用するかどうか
        /// </summary>
        public bool UseSlidingExpiration { get; set; } = true;

        /// <summary>
        /// キャッシュサイズ制限（アイテム数）
        /// </summary>
        public int? SizeLimit { get; set; }
    }

    /// <summary>
    /// キャッシュマネージャー - メモリキャッシュのラッパー
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly CacheManagerOptions _options;
        private readonly ILogger<CacheManager> _logger;

        public CacheManager(
            IMemoryCache cache,
            IOptions<CacheManagerOptions> options,
            ILogger<CacheManager> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options?.Value ?? new CacheManagerOptions();
            _logger = logger;
        }

        /// <summary>
        /// キャッシュから値を取得、または指定された関数で生成して保存
        /// </summary>
        /// <typeparam name="T">キャッシュする値の型</typeparam>
        /// <param name="key">キャッシュキー</param>
        /// <param name="factory">値が存在しない場合に実行する関数</param>
        /// <param name="expiration">有効期限（秒）、null の場合はデフォルト値</param>
        /// <returns>キャッシュされた値</returns>
        public T GetOrCreate<T>(string key, Func<T> factory, int? expiration = null)
        {
            Guard.IsNotNullOrEmpty(key);
            Guard.IsNotNull(factory);

            return _cache.GetOrCreate(key, entry =>
            {
                ConfigureCacheEntry(entry, expiration);
                _logger.LogDebug("キャッシュ項目を生成: {Key}", key);
                return factory();
            });
        }

        /// <summary>
        /// キャッシュから値を非同期で取得、または指定された関数で生成して保存
        /// </summary>
        /// <typeparam name="T">キャッシュする値の型</typeparam>
        /// <param name="key">キャッシュキー</param>
        /// <param name="factory">値が存在しない場合に実行する非同期関数</param>
        /// <param name="expiration">有効期限（秒）、null の場合はデフォルト値</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>キャッシュされた値</returns>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, int? expiration = null, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNullOrEmpty(key);
            Guard.IsNotNull(factory);

            return await _cache.GetOrCreateAsync(key, async entry =>
            {
                ConfigureCacheEntry(entry, expiration);
                _logger.LogDebug("キャッシュ項目を非同期で生成: {Key}", key);
                return await factory();
            });
        }

        /// <summary>
        /// キャッシュから項目を削除
        /// </summary>
        /// <param name="key">キャッシュキー</param>
        public void Remove(string key)
        {
            Guard.IsNotNullOrEmpty(key);
            _logger.LogDebug("キャッシュ項目を削除: {Key}", key);
            _cache.Remove(key);
        }

        /// <summary>
        /// キャッシュ内に指定されたキーが存在するか確認
        /// </summary>
        /// <param name="key">キャッシュキー</param>
        /// <returns>キーが存在する場合はtrue</returns>
        public bool Contains(string key)
        {
            Guard.IsNotNullOrEmpty(key);
            return _cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// キャッシュから値を取得
        /// </summary>
        /// <typeparam name="T">キャッシュされている値の型</typeparam>
        /// <param name="key">キャッシュキー</param>
        /// <param name="value">取得した値</param>
        /// <returns>値が存在する場合はtrue</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            Guard.IsNotNullOrEmpty(key);
            return _cache.TryGetValue(key, out value);
        }

        private void ConfigureCacheEntry(ICacheEntry entry, int? expiration)
        {
            var expirationSeconds = expiration ?? _options.DefaultExpirationSeconds;

            if (_options.UseSlidingExpiration)
            {
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(expirationSeconds));
            }
            else
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(expirationSeconds));
            }

            if (_options.SizeLimit.HasValue)
            {
                entry.SetSize(1); // 各エントリが1カウントとしてサイズに反映
            }
        }
    }

    /// <summary>
    /// キャッシュマネージャーのインターフェース
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// キャッシュから値を取得、または指定された関数で生成して保存
        /// </summary>
        T GetOrCreate<T>(string key, Func<T> factory, int? expiration = null);

        /// <summary>
        /// キャッシュから値を非同期で取得、または指定された関数で生成して保存
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, int? expiration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// キャッシュから項目を削除
        /// </summary>
        void Remove(string key);

        /// <summary>
        /// キャッシュ内に指定されたキーが存在するか確認
        /// </summary>
        bool Contains(string key);

        /// <summary>
        /// キャッシュから値を取得
        /// </summary>
        bool TryGetValue<T>(string key, out T value);
    }

}
