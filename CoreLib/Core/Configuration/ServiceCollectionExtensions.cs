using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Configuration
{
    /// <summary>
    /// サービスコレクション拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// アプリケーション設定のDI登録
        /// </summary>
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // 基本設定
            services.Configure<AppSettings>(configuration);

            // 各セクションの設定
            services.Configure<LogSettings>(configuration.GetSection("LogSettings"));
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));
            services.Configure<NotificationSettings>(configuration.GetSection("NotificationSettings"));

            // 設定マネージャーの登録
            //services.AddSingleton<IConfigurationManager, ConfigurationManager>();

            return services;
        }

        /// <summary>
        /// DB設定をロガーに強く型付きで登録
        /// </summary>
        public static IServiceCollection AddDatabaseOptionsWithValidation(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<DatabaseSettings>()
                .Bind(configuration.GetSection("DatabaseSettings"))
                .ValidateDataAnnotations()
                .Validate(config =>
                {
                    if (string.IsNullOrEmpty(config.ConnectionString))
                        return false;

                    if (config.CommandTimeout <= 0)
                        return false;

                    return true;
                }, "DatabaseSettings validation failed")
                .ValidateOnStart();

            return services;
        }
    }
}
