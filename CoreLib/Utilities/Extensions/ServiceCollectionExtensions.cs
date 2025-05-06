using CoreLib.Core.Configuration;
using CoreLib.Utilities.Serialization.Extensions;
using CoreLib.Utilities.Validation.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions
{
    /// <summary>
    /// サービスコレクションの拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 全CoreLibの拡張サービスを登録
        /// </summary>
        public static IServiceCollection AddCoreLibServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 各種サービスを登録
            services.AddCoreSerializationServices();
            services.AddValidationServices();
            services.AddAppSettings(configuration);

            return services;
        }

        /// <summary>
        /// 遅延初期化される単一インスタンスのサービスを登録
        /// </summary>
        public static IServiceCollection AddLazySingleton<TService, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton<Lazy<TService>>(sp => new Lazy<TService>(() =>
                implementationFactory(sp)));

            services.AddSingleton<TService>(sp => sp.GetRequiredService<Lazy<TService>>().Value);

            return services;
        }

        /// <summary>
        /// 遅延初期化される単一インスタンスのサービスを登録
        /// </summary>
        public static IServiceCollection AddLazySingleton<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            services.AddSingleton<Lazy<TService>>(sp => new Lazy<TService>(() =>
                implementationFactory(sp)));

            services.AddSingleton(sp => sp.GetRequiredService<Lazy<TService>>().Value);

            return services;
        }

        /// <summary>
        /// 遅延初期化される単一インスタンスのサービスを登録
        /// </summary>
        public static IServiceCollection AddLazySingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton<Lazy<TService>>(sp => new Lazy<TService>(() =>
                ActivatorUtilities.CreateInstance<TImplementation>(sp)));

            services.AddSingleton<TService>(sp => sp.GetRequiredService<Lazy<TService>>().Value);

            return services;
        }

        /// <summary>
        /// サービスがIDisposableを実装している場合にホストシャットダウン時に解放
        /// </summary>
        public static IServiceCollection AddHostedService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService, IHostedService
        {
            services.AddSingleton<TService, TImplementation>();
            services.AddHostedService<TImplementation>(sp => (TImplementation)sp.GetRequiredService<TService>());
            return services;
        }

        /// <summary>
        /// 複数のインターフェースを実装する単一のサービスインスタンスを登録
        /// </summary>
        public static IServiceCollection AddSingletonForMultipleInterfaces<TImplementation>(
            this IServiceCollection services,
            params Type[] serviceTypes)
            where TImplementation : class
        {
            if (serviceTypes.Length == 0)
                throw new ArgumentException("少なくとも1つのサービス型を指定してください", nameof(serviceTypes));

            services.AddSingleton<TImplementation>();

            foreach (var serviceType in serviceTypes)
            {
                services.AddSingleton(serviceType, sp => sp.GetRequiredService<TImplementation>());
            }

            return services;
        }
    }
}
