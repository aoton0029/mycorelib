using CoreLib.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Messaging
{
    /// <summary>
    /// 分散メッセージバスの抽象化インターフェース
    /// </summary>
    public interface IDistributedMessageBus
    {
        /// <summary>
        /// メッセージを発行
        /// </summary>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// トピックをサブスクライブ
        /// </summary>
        Task SubscribeAsync<TMessage>(Func<TMessage, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// トピックのサブスクリプションを解除
        /// </summary>
        Task UnsubscribeAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IMessage;
    }

    /// <summary>
    /// メッセージシリアライザーインターフェース
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// メッセージをシリアライズ
        /// </summary>
        string Serialize<TMessage>(TMessage message) where TMessage : IMessage;

        /// <summary>
        /// メッセージをデシリアライズ
        /// </summary>
        TMessage Deserialize<TMessage>(string data) where TMessage : IMessage;
    }

    /// <summary>
    /// JSON形式によるメッセージシリアライザー
    /// </summary>
    public class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonMessageSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <summary>
        /// メッセージをシリアライズ
        /// </summary>
        public string Serialize<TMessage>(TMessage message) where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return JsonSerializer.Serialize(message, _options);
        }

        /// <summary>
        /// メッセージをデシリアライズ
        /// </summary>
        public TMessage Deserialize<TMessage>(string data) where TMessage : IMessage
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("デシリアライズするデータがnullまたは空です", nameof(data));

            return JsonSerializer.Deserialize<TMessage>(data, _options);
        }
    }

    /// <summary>
    /// インメモリ実装の分散メッセージバス（シングルプロセス用テスト実装）
    /// </summary>
    public class InMemoryDistributedMessageBus : IDistributedMessageBus
    {
        private readonly IServiceBus _serviceBus;
        private readonly IAppLogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InMemoryDistributedMessageBus(IServiceBus serviceBus, IAppLogger logger)
        {
            _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージを発行
        /// </summary>
        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            return _serviceBus.PublishAsync(message, cancellationToken);
        }

        /// <summary>
        /// トピックをサブスクライブ
        /// </summary>
        public Task SubscribeAsync<TMessage>(Func<TMessage, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            // ハンドラーをメッセージハンドラーとしてラップして登録
            var messageHandler = new DelegateMessageHandler<TMessage>(handler);
            _serviceBus.RegisterHandler(messageHandler);

            _logger.LogInformation($"トピックをサブスクライブ: {typeof(TMessage).Name}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// トピックのサブスクリプションを解除
        /// </summary>
        public Task UnsubscribeAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            // このインメモリ実装では、サブスクリプションの解除は実装しない
            // 実際の実装では、トピックのサブスクリプションを解除するロジックを実装する
            _logger.LogInformation($"トピックのサブスクリプションを解除: {typeof(TMessage).Name}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 委譲関数をラップするメッセージハンドラー
        /// </summary>
        private class DelegateMessageHandler<TMessage> : IMessageHandler<TMessage>
            where TMessage : IMessage
        {
            private readonly Func<TMessage, CancellationToken, Task> _handler;

            public DelegateMessageHandler(Func<TMessage, CancellationToken, Task> handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            public Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
            {
                return _handler(message, cancellationToken);
            }
        }
    }

    /// <summary>
    /// 分散メッセージバスの拡張メソッド
    /// </summary>
    public static class DistributedMessageBusExtensions
    {
        /// <summary>
        /// 分散メッセージバスをDIに登録
        /// </summary>
        public static IServiceCollection AddDistributedMessageBus(this IServiceCollection services)
        {
            services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();
            services.AddSingleton<IDistributedMessageBus, InMemoryDistributedMessageBus>();
            return services;
        }
    }
}
