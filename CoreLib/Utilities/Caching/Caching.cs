using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Caching
{
    /// <summary>
    /// キャッシュサービスのインターフェース
    /// </summary>
    public interface ICacheService
    {
        T? Get<T>(string key);
        bool TryGet<T>(string key, out T? value);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    }

    /// <summary>
    /// メモリキャッシュを使用したキャッシュサービス実装
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public T? Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool TryGet<T>(string key, out T? value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
            };

            _cache.Set(key, value, cacheOptions);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (!_cache.TryGetValue(key, out T? value))
            {
                value = await factory();
                Set(key, value, expiration);
            }

            return value!;
        }
    }
}
