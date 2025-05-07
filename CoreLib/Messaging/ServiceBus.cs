using CoreLib.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Messaging
{
    /// <summary>
    /// メッセージの基底インターフェース
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// メッセージID
        /// </summary>
        Guid MessageId { get; }

        /// <summary>
        /// メッセージの作成日時
        /// </summary>
        DateTime CreatedAt { get; }
    }

    /// <summary>
    /// メッセージハンドラーの基底インターフェース
    /// </summary>
    /// <typeparam name="TMessage">処理するメッセージの型</typeparam>
    public interface IMessageHandler<in TMessage> where TMessage : IMessage
    {
        /// <summary>
        /// メッセージを処理
        /// </summary>
        Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 基本メッセージ実装
    /// </summary>
    public abstract class Message : IMessage
    {
        /// <summary>
        /// メッセージID
        /// </summary>
        public Guid MessageId { get; }

        /// <summary>
        /// メッセージの作成日時
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected Message()
        {
            MessageId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// サービスバスのインターフェース
    /// </summary>
    public interface IServiceBus
    {
        /// <summary>
        /// メッセージを発行
        /// </summary>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler)
            where TMessage : IMessage;

        /// <summary>
        /// メッセージハンドラーの登録解除
        /// </summary>
        void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler)
            where TMessage : IMessage;
    }

    /// <summary>
    /// サービスバスの実装
    /// </summary>
    public class ServiceBus : IServiceBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppLogger _logger;
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();
        private readonly object _syncRoot = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceBus(IServiceProvider serviceProvider, IAppLogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージを発行
        /// </summary>
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);
            _logger.LogDebug($"メッセージの発行: {messageType.Name}, ID={message.MessageId}");

            // 登録されたハンドラーを取得
            if (!_handlers.TryGetValue(messageType, out var handlers) || handlers.Count == 0)
            {
                // DIから登録されたハンドラーを取得
                using var scope = _serviceProvider.CreateScope();
                var fromDI = scope.ServiceProvider.GetServices<IMessageHandler<TMessage>>().ToList();

                if (fromDI.Count > 0)
                {
                    _logger.LogDebug($"DIから{fromDI.Count}個のハンドラーを取得: {messageType.Name}");
                    await PublishToDIHandlersAsync(message, fromDI, cancellationToken);
                    return;
                }

                _logger.LogWarning($"メッセージタイプ {messageType.Name} に対するハンドラーが見つかりません");
                return;
            }

            var typedHandlers = handlers.Cast<IMessageHandler<TMessage>>().ToList();
            _logger.LogDebug($"{typedHandlers.Count}個のハンドラーでメッセージを処理: {messageType.Name}");

            // すべてのハンドラーでメッセージを処理
            var tasks = new List<Task>();
            foreach (var handler in typedHandlers)
            {
                try
                {
                    tasks.Add(handler.HandleAsync(message, cancellationToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"メッセージハンドラーの実行中にエラーが発生: {handler.GetType().Name}");
                }
            }

            await Task.WhenAll(tasks);
            _logger.LogDebug($"メッセージ処理完了: {messageType.Name}, ID={message.MessageId}");
        }

        private async Task PublishToDIHandlersAsync<TMessage>(
            TMessage message,
            IEnumerable<IMessageHandler<TMessage>> handlers,
            CancellationToken cancellationToken)
            where TMessage : IMessage
        {
            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                try
                {
                    tasks.Add(handler.HandleAsync(message, cancellationToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DIから取得したメッセージハンドラーの実行中にエラーが発生: {handler.GetType().Name}");
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler)
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(messageType, out var handlers))
                {
                    handlers = new List<object>();
                    _handlers[messageType] = handlers;
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                    _logger.LogDebug($"メッセージハンドラーを登録: {handler.GetType().Name} for {messageType.Name}");
                }
            }
        }

        /// <summary>
        /// メッセージハンドラーの登録解除
        /// </summary>
        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler)
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (_handlers.TryGetValue(messageType, out var handlers))
                {
                    if (handlers.Remove(handler))
                    {
                        _logger.LogDebug($"メッセージハンドラーの登録を解除: {handler.GetType().Name} for {messageType.Name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// サービスバスの拡張メソッド
    /// </summary>
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// サービスバスをDIに登録
        /// </summary>
        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<IServiceBus, ServiceBus>();
            return services;
        }

        /// <summary>
        /// メッセージハンドラーをDIに登録
        /// </summary>
        public static IServiceCollection AddMessageHandler<TMessage, THandler>(this IServiceCollection services)
            where TMessage : IMessage
            where THandler : class, IMessageHandler<TMessage>
        {
            services.AddTransient<IMessageHandler<TMessage>, THandler>();
            return services;
        }
    }
}
