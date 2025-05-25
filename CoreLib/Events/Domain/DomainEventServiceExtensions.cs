using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// DIサービス登録用拡張メソッド
    /// </summary>
    public static class DomainEventsServiceExtensions
    {
        /// <summary>
        /// ドメインイベント処理システムをDIに登録
        /// </summary>
        public static IServiceCollection AddDomainEvents(this IServiceCollection services, params Type[] handlerTypes)
        {
            // ディスパッチャーとパブリッシャーを登録
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            // 指定されたハンドラータイプの自動登録
            if (handlerTypes != null && handlerTypes.Length > 0)
            {
                foreach (var handlerType in handlerTypes)
                {
                    RegisterEventHandler(services, handlerType);
                }
            }

            return services;
        }

        /// <summary>
        /// 指定されたアセンブリから全てのイベントハンドラーを登録
        /// </summary>
        public static IServiceCollection AddDomainEventsFromAssemblies(this IServiceCollection services, params System.Reflection.Assembly[] assemblies)
        {
            // ディスパッチャーとパブリッシャーを登録
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            if (assemblies != null && assemblies.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    // アセンブリからイベントハンドラーを検索
                    var handlerTypes = assembly.GetTypes()
                        .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));

                    foreach (var handlerType in handlerTypes)
                    {
                        RegisterEventHandler(services, handlerType);
                    }
                }
            }

            return services;
        }

        private static void RegisterEventHandler(IServiceCollection services, Type handlerType)
        {
            // イベントハンドラーのインターフェースを取得
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                // 具象ハンドラーをインターフェースにマッピングして登録
                services.AddScoped(handlerInterface, handlerType);
            }
        }
    }

    /// <summary>
    /// 集約ルートにイベント処理機能を追加するための拡張機能
    /// </summary>
    public abstract class EventTrackingEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();

        /// <summary>
        /// 記録されたドメインイベント
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// ドメインイベントの追加
        /// </summary>
        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// ドメインイベントの削除
        /// </summary>
        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        /// <summary>
        /// すべてのドメインイベントをクリア
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
