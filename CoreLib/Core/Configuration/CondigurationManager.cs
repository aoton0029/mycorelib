using CoreLib.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Configuration
{
    /// <summary>
    /// 構成設定管理インターフェース
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// 設定を取得
        /// </summary>
        /// <typeparam name="T">設定の型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <returns>設定オブジェクト</returns>
        T GetConfiguration<T>() where T : class, new();

        /// <summary>
        /// 設定を取得（セクション名指定）
        /// </summary>
        /// <typeparam name="T">設定の型</typeparam>
        /// <param name="sectionName">セクション名</param>
        /// <returns>設定オブジェクト</returns>
        T GetConfiguration<T>(string sectionName) where T : class, new();

        /// <summary>
        /// 設定を更新
        /// </summary>
        /// <typeparam name="T">設定の型</typeparam>
        /// <param name="config">新しい設定オブジェクト</param>
        /// <param name="sectionName">セクション名</param>
        /// <returns>更新が成功したかどうか</returns>
        bool UpdateConfiguration<T>(T config, string? sectionName = null) where T : class, new();

        /// <summary>
        /// 設定ファイルを再読み込み
        /// </summary>
        void ReloadConfiguration();

        /// <summary>
        /// 設定更新イベントを購読
        /// </summary>
        /// <typeparam name="T">設定の型</typeparam>
        /// <param name="handler">更新ハンドラー</param>
        /// <param name="sectionName">セクション名</param>
        void SubscribeToConfigurationChanges<T>(Action<T> handler, string? sectionName = null) where T : class, new();

        /// <summary>
        /// 設定更新イベントの購読を解除
        /// </summary>
        /// <typeparam name="T">設定の型</typeparam>
        /// <param name="handler">更新ハンドラー</param>
        /// <param name="sectionName">セクション名</param>
        void UnsubscribeFromConfigurationChanges<T>(Action<T> handler, string? sectionName = null) where T : class, new();
    }

    /// <summary>
    /// 構成設定管理クラス
    /// </summary>
    public class ConfigurationManager : IConfigurationManager, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IAppLogger _logger;
        private readonly object _lockObject = new();
        private bool _disposed;

        // 設定キャッシュ（型名+セクション名 -> 設定オブジェクト）
        private readonly ConcurrentDictionary<string, object> _configCache = new();

        // 設定更新通知用のサブスクライバーリスト（型名+セクション名 -> ハンドラーリスト）
        private readonly ConcurrentDictionary<string, List<Delegate>> _subscribers = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigurationManager(IConfiguration configuration, IAppLogger logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 設定を取得
        /// </summary>
        public T GetConfiguration<T>() where T : class, new()
        {
            string sectionName = typeof(T).Name;
            return GetConfiguration<T>(sectionName);
        }

        /// <summary>
        /// 設定を取得（セクション名指定）
        /// </summary>
        public T GetConfiguration<T>(string sectionName) where T : class, new()
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("セクション名を指定する必要があります", nameof(sectionName));

            string cacheKey = GetCacheKey<T>(sectionName);

            // キャッシュから取得を試みる
            if (_configCache.TryGetValue(cacheKey, out var cachedConfig) && cachedConfig is T typedConfig)
            {
                return typedConfig;
            }

            // 構成からセクションを取得
            var section = _configuration.GetSection(sectionName);
            T config = section.Get<T>() ?? new T();

            // 設定バリデーション属性があれば検証を実行
            ValidateConfiguration(config);

            // キャッシュに追加
            _configCache[cacheKey] = config;

            return config;
        }

        /// <summary>
        /// 設定を更新
        /// </summary>
        public bool UpdateConfiguration<T>(T config, string? sectionName = null) where T : class, new()
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            string section = sectionName ?? typeof(T).Name;
            string cacheKey = GetCacheKey<T>(section);

            try
            {
                // 設定バリデーション属性があれば検証を実行
                ValidateConfiguration(config);

                // キャッシュを更新
                _configCache[cacheKey] = config;

                // サブスクライバーに通知
                NotifySubscribers(section, config);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"設定の更新中にエラーが発生しました: {section}");
                return false;
            }
        }

        /// <summary>
        /// 設定ファイルを再読み込み
        /// </summary>
        public void ReloadConfiguration()
        {
            try
            {
                // 設定ソースを再読み込み（ファイルプロバイダーなど）
                if (_configuration is IConfigurationRoot configRoot)
                {
                    configRoot.Reload();
                    _logger.LogInformation("設定が再読み込みされました");

                    // キャッシュをクリア
                    _configCache.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "設定の再読み込み中にエラーが発生しました");
            }
        }

        /// <summary>
        /// 設定更新イベントを購読
        /// </summary>
        public void SubscribeToConfigurationChanges<T>(Action<T> handler, string? sectionName = null) where T : class, new()
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            string section = sectionName ?? typeof(T).Name;
            string key = GetCacheKey<T>(section);

            lock (_lockObject)
            {
                var handlers = _subscribers.GetOrAdd(key, _ => new List<Delegate>());
                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                    _logger.LogDebug($"設定の変更サブスクライバーが追加されました: {section}");
                }
            }
        }

        /// <summary>
        /// 設定更新イベントの購読を解除
        /// </summary>
        public void UnsubscribeFromConfigurationChanges<T>(Action<T> handler, string? sectionName = null) where T : class, new()
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            string section = sectionName ?? typeof(T).Name;
            string key = GetCacheKey<T>(section);

            lock (_lockObject)
            {
                if (_subscribers.TryGetValue(key, out var handlers))
                {
                    handlers.Remove(handler);
                    _logger.LogDebug($"設定の変更サブスクライバーが削除されました: {section}");

                    if (handlers.Count == 0)
                    {
                        _subscribers.TryRemove(key, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 設定オブジェクトのバリデーションを実行
        /// </summary>
        private void ValidateConfiguration<T>(T config) where T : class, new()
        {
            // バリデーション用の属性やインターフェースが実装されている場合は検証する
            if (config is IValidatable validatable)
            {
                var validationResult = validatable.Validate();
                if (!validationResult.IsValid)
                {
                    throw new InvalidOperationException(
                        $"設定の検証に失敗しました: {string.Join(", ", validationResult.Errors)}");
                }
            }
        }

        /// <summary>
        /// 設定が変更された時にサブスクライバーに通知
        /// </summary>
        private void NotifySubscribers<T>(string section, T config) where T : class, new()
        {
            string key = GetCacheKey<T>(section);

            if (_subscribers.TryGetValue(key, out var handlers))
            {
                foreach (var handler in handlers.ToArray())
                {
                    try
                    {
                        if (handler is Action<T> typedHandler)
                        {
                            typedHandler(config);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"設定変更通知中にエラーが発生しました: {section}");
                    }
                }
            }
        }

        /// <summary>
        /// キャッシュキーを生成
        /// </summary>
        private string GetCacheKey<T>(string section) => $"{typeof(T).FullName}:{section}";

        /// <summary>
        /// リソースを破棄
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // メモリリークを避けるためにサブスクライバーをクリア
            _subscribers.Clear();
            _configCache.Clear();

            _disposed = true;
        }
    }

    /// <summary>
    /// 検証可能な設定インターフェース
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// 設定を検証
        /// </summary>
        ValidationResult Validate();
    }

    /// <summary>
    /// ファイル監視設定マネージャークラス
    /// </summary>
    public class FileWatchingConfigurationManager : ConfigurationManager
    {
        private readonly FileSystemWatcher? _fileWatcher;
        private readonly string _configFilePath;
        private readonly Timer _debounceTimer;
        private readonly int _debounceDelayMs = 300;
        private readonly IAppLogger _logger;
        private bool _disposed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileWatchingConfigurationManager(
            IConfiguration configuration,
            IAppLogger logger,
            string configFilePath)
            : base(configuration, logger)
        {
            _logger = logger;
            _configFilePath = configFilePath;

            if (!string.IsNullOrEmpty(configFilePath))
            {
                try
                {
                    var fileInfo = new FileInfo(configFilePath);
                    if (fileInfo.Exists)
                    {
                        _fileWatcher = new FileSystemWatcher
                        {
                            Path = fileInfo.DirectoryName!,
                            Filter = fileInfo.Name,
                            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                        };

                        _fileWatcher.Changed += OnConfigFileChanged;
                        _fileWatcher.EnableRaisingEvents = true;

                        _logger.LogInformation($"設定ファイルの監視を開始しました: {configFilePath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"設定ファイルの監視設定中にエラーが発生しました: {configFilePath}");
                }
            }

            _debounceTimer = new Timer(_ => ReloadConfiguration(), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 設定ファイル変更時の処理
        /// </summary>
        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            // ファイル変更イベントは短時間に複数回発生する可能性があるため、
            // デバウンス処理を行い、一定時間経過後に1回だけ処理を実行する
            _debounceTimer.Change(_debounceDelayMs, Timeout.Infinite);
        }

        /// <summary>
        /// リソースを破棄
        /// </summary>
        public new void Dispose()
        {
            if (_disposed)
                return;

            base.Dispose();

            _debounceTimer.Dispose();

            if (_fileWatcher != null)
            {
                _fileWatcher.Changed -= OnConfigFileChanged;
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// 拡張メソッド
    /// </summary>
    public static class ConfigurationManagerExtensions
    {
        /// <summary>
        /// 構成設定管理サービスを登録
        /// </summary>
        public static IServiceCollection AddConfigurationManager(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IConfigurationManager>(sp =>
                new ConfigurationManager(configuration, sp.GetRequiredService<IAppLogger>()));

            return services;
        }

        /// <summary>
        /// ファイル監視付き構成設定管理サービスを登録
        /// </summary>
        public static IServiceCollection AddFileWatchingConfigurationManager(
            this IServiceCollection services,
            IConfiguration configuration,
            string configFilePath)
        {
            services.AddSingleton<IConfigurationManager>(sp =>
                new FileWatchingConfigurationManager(
                    configuration,
                    sp.GetRequiredService<IAppLogger>(),
                    configFilePath));

            return services;
        }

        /// <summary>
        /// 構成設定のオプションパターンサポートを追加
        /// </summary>
        public static IServiceCollection AddConfigurationOptions<TOptions>(
            this IServiceCollection services,
            string? sectionName = null)
            where TOptions : class, new()
        {
            string section = sectionName ?? typeof(TOptions).Name;

            // 標準オプションパターンの登録
            services.Configure<TOptions>(
                services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection(section));

            // IConfigurationManagerを使ったオプション更新用処理
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(
                new ConfigurationManagerChangeTokenSource<TOptions>(section));

            return services;
        }
    }

    /// <summary>
    /// 構成マネージャー用のチェンジトークンソース
    /// </summary>
    internal class ConfigurationManagerChangeTokenSource<TOptions> : IOptionsChangeTokenSource<TOptions>
        where TOptions : class, new()
    {
        private readonly string _sectionName;

        public ConfigurationManagerChangeTokenSource(string sectionName)
        {
            _sectionName = sectionName;
        }

        public string? Name => _sectionName;

        public IChangeToken GetChangeToken()
        {
            // この実装は簡易的なもので、実際には構成変更を追跡する
            // より複雑なChangeTokenの実装が必要になる場合があります
            return new ConfigurationChangeToken();
        }
    }

    /// <summary>
    /// 構成変更トークン
    /// </summary>
    internal class ConfigurationChangeToken : IChangeToken
    {
        private readonly List<Action<object>> _callbacks = new();
        private readonly object _callbackLock = new();

        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            lock (_callbackLock)
            {
                _callbacks.Add(callback);
            }

            return new ChangeCallbackRegistration(_callbacks, callback);
        }

        internal void NotifyChange()
        {
            Action<object>[] callbackCopy;
            lock (_callbackLock)
            {
                callbackCopy = _callbacks.ToArray();
            }

            foreach (var callback in callbackCopy)
            {
                try
                {
                    callback(this);
                }
                catch
                {
                    // エラー処理
                }
            }
        }

        private class ChangeCallbackRegistration : IDisposable
        {
            private readonly List<Action<object>> _callbacks;
            private readonly Action<object> _callback;

            public ChangeCallbackRegistration(List<Action<object>> callbacks, Action<object> callback)
            {
                _callbacks = callbacks;
                _callback = callback;
            }

            public void Dispose()
            {
                lock (_callbacks)
                {
                    _callbacks.Remove(_callback);
                }
            }
        }
    }
}
