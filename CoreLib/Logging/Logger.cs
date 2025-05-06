using CoreLib.Core.Configuration;
using CoreLib.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Logging
{
    /// <summary>
    /// アプリケーションのログ機能を提供するサービス
    /// </summary>
    public interface IAppLogger
    {
        void LogTrace(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogCritical(Exception exception, string message, params object[] args);
        void LogCritical(string message, params object[] args);
    }

    /// <summary>
    /// ILoggerを使用したアプリケーションログの実装
    /// </summary>
    public class AppLogger<T> : IAppLogger where T : class
    {
        private readonly ILogger<T> _logger;

        public AppLogger(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogCritical(Exception exception, string message, params object[] args)
        {
            _logger.LogCritical(exception, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }
    }

    /// <summary>
    /// ロギングサービスの拡張メソッド
    /// </summary>
    public static class LoggingServiceExtensions
    {
        /// <summary>
        /// アプリケーションのロギングサービスを登録します
        /// </summary>
        public static IServiceCollection AddAppLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // ログ設定を取得
            var logSettings = configuration.GetSection("LogSettings").Get<LogSettings>() ?? new LogSettings();

            // ロガーの設定
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));

                // コンソールロガーの設定
                if (logSettings.EnableConsoleLogging)
                {
                    builder.AddConsole(options =>
                    {
                        options.IncludeScopes = logSettings.IncludeScopes;
                        options.TimestampFormat = logSettings.UseUtcTimestamp
                            ? "yyyy-MM-dd HH:mm:ss.fff UTC "
                            : "yyyy-MM-dd HH:mm:ss.fff ";
                    });
                }

                // ファイルロガーの設定（オプション）
                if (logSettings.EnableFileLogging)
                {
                    // 注: 実際のファイルロギングにはSerilogなどの追加パッケージが必要です
                    // builder.AddFile(logSettings.LogFilePath, ...);
                }

                // 最小ログレベルの設定
                if (Enum.TryParse<LogLevel>(logSettings.LogLevel, true, out var logLevel))
                {
                    builder.SetMinimumLevel(logLevel);
                }
            });

            // ジェネリックでないバージョンのIAppLoggerも登録可能
            services.AddSingleton(typeof(IAppLogger), typeof(AppLogger<>));

            return services;
        }
    }
}
