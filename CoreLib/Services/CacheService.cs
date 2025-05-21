using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    public interface ICacheService
    {
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }

    public class CacheService : ICacheService, IDisposable
    {
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private readonly Timer _cleanupTimer;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private class CacheItem
        {
            public object Value { get; set; }
            public DateTime? ExpirationTime { get; set; }
            public bool IsExpired => ExpirationTime.HasValue && DateTime.Now > ExpirationTime.Value;
        }

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
            // 5分ごとに期限切れのアイテムをクリーンアップ
            _cleanupTimer = new Timer(CleanupExpiredItems, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            // キャッシュに存在し、かつ期限切れでなければそれを返す
            if (_cache.TryGetValue(key, out var item) && !item.IsExpired)
            {
                _logger.LogDebug("キャッシュヒット: {Key}", key);
                return (T)item.Value;
            }

            // 存在しないか期限切れの場合は新しく生成
            await _semaphore.WaitAsync();
            try
            {
                // ロック取得後に再チェック
                if (_cache.TryGetValue(key, out item) && !item.IsExpired)
                {
                    return (T)item.Value;
                }

                _logger.LogDebug("キャッシュミス、値を生成します: {Key}", key);
                var value = await factory();

                await SetAsync(key, value, expiration);
                return value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var item) && !item.IsExpired)
            {
                _logger.LogDebug("キャッシュから取得: {Key}", key);
                return (T)item.Value;
            }

            _logger.LogDebug("キャッシュに存在しないか期限切れ: {Key}", key);
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var expirationTime = expiration.HasValue
                ? DateTime.Now.Add(expiration.Value)
                : DateTime.Now.Add(_defaultExpiration);

            var cacheItem = new CacheItem
            {
                Value = value,
                ExpirationTime = expirationTime
            };

            _cache[key] = cacheItem;
            _logger.LogDebug("キャッシュに追加: {Key}, 有効期限: {ExpirationTime}", key, expirationTime);

            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            if (_cache.TryRemove(key, out _))
            {
                _logger.LogDebug("キャッシュから削除: {Key}", key);
            }

            await Task.CompletedTask;
        }

        public async Task ClearAsync()
        {
            _cache.Clear();
            _logger.LogInformation("キャッシュをクリアしました");

            await Task.CompletedTask;
        }

        private void CleanupExpiredItems(object state)
        {
            var expiredKeys = new List<string>();

            foreach (var (key, item) in _cache)
            {
                if (item.IsExpired)
                {
                    expiredKeys.Add(key);
                }
            }

            foreach (var key in expiredKeys)
            {
                if (_cache.TryRemove(key, out _))
                {
                    _logger.LogDebug("期限切れアイテムを削除: {Key}", key);
                }
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogInformation("{Count}個の期限切れアイテムを削除しました", expiredKeys.Count);
            }
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
            _semaphore?.Dispose();
        }
    }
}
