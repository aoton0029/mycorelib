using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Logging
{
    /// <summary>
    /// アプリケーションログを管理するサービスクラス
    /// </summary>
    public class LoggingService
    {
        private readonly ILogger _logger;

        public LoggingService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("Application") ??
                throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// デバッグレベルのログを記録します
        /// </summary>
        public void LogDebug(string message, params object[] args) =>
            _logger.LogDebug(message, args);

        /// <summary>
        /// 情報レベルのログを記録します
        /// </summary>
        public void LogInformation(string message, params object[] args) =>
            _logger.LogInformation(message, args);

        /// <summary>
        /// 警告レベルのログを記録します
        /// </summary>
        public void LogWarning(string message, params object[] args) =>
            _logger.LogWarning(message, args);

        /// <summary>
        /// エラーレベルのログを記録します
        /// </summary>
        public void LogError(Exception exception, string message, params object[] args) =>
            _logger.LogError(exception, message, args);

        /// <summary>
        /// 致命的エラーレベルのログを記録します
        /// </summary>
        public void LogCritical(Exception exception, string message, params object[] args) =>
            _logger.LogCritical(exception, message, args);
    }

    /// <summary>
    /// DIサービス拡張メソッド
    /// </summary>
    public static class LoggingServiceExtensions
    {
        /// <summary>
        /// ロギングサービスをサービスコンテナに登録します
        /// </summary>
        public static IServiceCollection AddLoggingService(this IServiceCollection services)
        {
            services.AddSingleton<LoggingService>();
            return services;
        }
    }
}
