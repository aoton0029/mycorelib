using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// 非同期メッセンジャーのインターフェース
    /// </summary>
    public interface IAsyncMessenger
    {
        /// <summary>
        /// 指定されたタイプのメッセージを直接発行する
        /// </summary>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// メッセージをキューに追加し、非同期で処理する
        /// </summary>
        Task EnqueueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;

        /// <summary>
        /// メッセージハンドラーを登録解除
        /// </summary>
        void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;
    }

    /// <summary>
    /// 基本メッセージ実装
    /// </summary>
    public abstract class Message : IMessage
    {
        /// <summary>
        /// メッセージID
        /// </summary>
        public Guid MessageId { get; } = Guid.NewGuid();

        /// <summary>
        /// メッセージの作成日時
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
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
    /// 非同期メッセンジャーの実装
    /// </summary>
    public class AsyncMessenger : IAsyncMessenger, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();
        private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _messageQueues = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Dictionary<Type, Task> _processingTasks = new();
        private readonly object _syncRoot = new();
        private bool _isProcessing = false;
        private bool _disposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AsyncMessenger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage
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
        /// メッセージハンドラーを登録解除
        /// </summary>
        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage
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

        /// <summary>
        /// メッセージを直接発行する
        /// </summary>
        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);
            _logger.LogDebug($"メッセージを発行: {messageType.Name}, ID={message.MessageId}");

            if (!_handlers.TryGetValue(messageType, out var handlers) || handlers.Count == 0)
            {
                _logger.LogWarning($"メッセージタイプ {messageType.Name} に対するハンドラーが見つかりません");
                return;
            }

            var typedHandlers = handlers.Cast<IMessageHandler<TMessage>>().ToList();
            var tasks = new List<Task>();

            foreach (var handler in typedHandlers)
            {
                try
                {
                    tasks.Add(handler.HandleAsync(message, cancellationToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"メッセージハンドラーの実行時にエラーが発生: {handler.GetType().Name}");
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// メッセージをキューに追加し、非同期で処理する
        /// </summary>
        public Task EnqueueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);

            // キューの取得または作成
            var queue = _messageQueues.GetOrAdd(messageType, _ => new ConcurrentQueue<object>());

            // メッセージをキューに追加
            queue.Enqueue(message);
            _logger.LogDebug($"メッセージをキューに追加: {messageType.Name}, ID={message.MessageId}, キュー長={queue.Count}");

            // まだ処理が開始されていない場合は、処理を開始
            EnsureProcessingStarted<TMessage>();

            return Task.CompletedTask;
        }

        /// <summary>
        /// 特定タイプのメッセージの処理が開始されていることを確認
        /// </summary>
        private void EnsureProcessingStarted<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (!_isProcessing)
                {
                    StartProcessing();
                }

                if (!_processingTasks.ContainsKey(messageType))
                {
                    StartProcessingForType<TMessage>();
                }
            }
        }

        /// <summary>
        /// メッセージ処理全体を開始
        /// </summary>
        private void StartProcessing()
        {
            _isProcessing = true;
            _logger.LogInformation("非同期メッセージ処理を開始");
        }

        /// <summary>
        /// 特定タイプのメッセージ処理を開始
        /// </summary>
        private void StartProcessingForType<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            var processingTask = Task.Run(async () =>
            {
                try
                {
                    await ProcessQueueAsync<TMessage>(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // 正常なキャンセル
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"メッセージキュー処理中にエラーが発生: {messageType.Name}");
                }
            });

            _processingTasks[messageType] = processingTask;
            _logger.LogDebug($"メッセージタイプの処理を開始: {messageType.Name}");
        }

        /// <summary>
        /// メッセージキューの処理
        /// </summary>
        private async Task ProcessQueueAsync<TMessage>(CancellationToken cancellationToken) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            if (!_messageQueues.TryGetValue(messageType, out var queue))
            {
                _logger.LogWarning($"メッセージタイプ {messageType.Name} のキューが見つかりません");
                return;
            }

            while (!cancellationToken.IsCancellationRequested && _isProcessing)
            {
                if (queue.TryDequeue(out var messageObj) && messageObj is TMessage message)
                {
                    try
                    {
                        await PublishAsync(message, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"メッセージの処理中にエラーが発生: {messageType.Name}, ID={message.MessageId}");
                    }
                }
                else
                {
                    // キューが空の場合は少し待機
                    await Task.Delay(100, cancellationToken);
                }
            }
        }

        /// <summary>
        /// メッセージ処理を停止
        /// </summary>
        public void StopProcessing()
        {
            lock (_syncRoot)
            {
                if (!_isProcessing)
                    return;

                _isProcessing = false;
                _logger.LogInformation("非同期メッセージ処理を停止");
            }
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            StopProcessing();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            // すべての処理タスクの完了を待機
            Task.WhenAll(_processingTasks.Values).Wait(TimeSpan.FromSeconds(5));

            _processingTasks.Clear();
            _messageQueues.Clear();
            _handlers.Clear();

            _logger.LogInformation("非同期メッセンジャーを破棄");
        }
    }


    /// <summary>
    /// サービスバスの実装
    /// </summary>
    public class ServiceBus : IServiceBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();
        private readonly object _syncRoot = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServiceBus(IServiceProvider serviceProvider, ILogger logger)
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
