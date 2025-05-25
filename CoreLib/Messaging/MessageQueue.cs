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
    /// イン・メモリ・メッセージキューインターフェース
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// メッセージをキューに追加
        /// </summary>
        Task EnqueueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// 特定のメッセージタイプの処理を待機
        /// </summary>
        Task<TMessage> DequeueAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IMessage;

        /// <summary>
        /// キューの処理を開始
        /// </summary>
        void Start();

        /// <summary>
        /// キューの処理を停止
        /// </summary>
        void Stop();

        /// <summary>
        /// キューの現在の長さを取得
        /// </summary>
        int GetQueueLength<TMessage>() where TMessage : IMessage;
    }

    /// <summary>
    /// イン・メモリ・メッセージキュー実装
    /// </summary>
    public class InMemoryMessageQueue : IMessageQueue, IDisposable
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Type, object> _queues = new();
        private readonly CancellationTokenSource _cancellationSource = new();
        private readonly Dictionary<Type, Task> _processingTasks = new();
        private bool _isRunning;
        private bool _isDisposed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InMemoryMessageQueue(IServiceBus serviceBus, ILogger logger)
        {
            _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージをキューに追加
        /// </summary>
        public Task EnqueueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // メッセージタイプのキューを取得または作成
            var queue = GetOrCreateQueue<TMessage>();

            // メッセージをキューに追加
            queue.Enqueue(message);
            _logger.LogDebug($"メッセージをキューに追加: {typeof(TMessage).Name}, ID={message.MessageId}, キュー長={queue.Count}");

            return Task.CompletedTask;
        }

        /// <summary>
        /// 特定のメッセージタイプの処理を待機
        /// </summary>
        public async Task<TMessage> DequeueAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IMessage
        {
            var queue = GetOrCreateQueue<TMessage>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (queue.TryDequeue(out var message))
                {
                    _logger.LogDebug($"メッセージをキューから取得: {typeof(TMessage).Name}, ID={message.MessageId}, キュー長={queue.Count}");
                    return message;
                }

                await Task.Delay(100, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return default; // ここには到達しないはず
        }

        /// <summary>
        /// キューの処理を開始
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _logger.LogInformation("メッセージキューの処理を開始");
        }

        /// <summary>
        /// キューの処理を停止
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _logger.LogInformation("メッセージキューの処理を停止");
        }

        /// <summary>
        /// キューの現在の長さを取得
        /// </summary>
        public int GetQueueLength<TMessage>() where TMessage : IMessage
        {
            var queue = GetOrCreateQueue<TMessage>();
            return queue.Count;
        }

        /// <summary>
        /// 特定のメッセージタイプのキューを取得または作成
        /// </summary>
        private ConcurrentQueue<TMessage> GetOrCreateQueue<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            return (ConcurrentQueue<TMessage>)_queues.GetOrAdd(
                messageType,
                _ => new ConcurrentQueue<TMessage>());
        }

        /// <summary>
        /// 特定のメッセージタイプのキュー処理タスクを開始
        /// </summary>
        private void StartProcessingQueue<TMessage>() where TMessage : IMessage
        {
            var messageType = typeof(TMessage);

            if (_processingTasks.ContainsKey(messageType))
                return;

            var processingTask = Task.Run(async () =>
            {
                try
                {
                    await ProcessQueueAsync<TMessage>(_cancellationSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // 正常なキャンセル
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"キュー処理中にエラーが発生: {messageType.Name}");
                }
            });

            _processingTasks[messageType] = processingTask;
        }

        /// <summary>
        /// キュー処理ループ
        /// </summary>
        private async Task ProcessQueueAsync<TMessage>(CancellationToken cancellationToken) where TMessage : IMessage
        {
            var queue = GetOrCreateQueue<TMessage>();

            while (!cancellationToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    if (queue.TryDequeue(out var message))
                    {
                        _logger.LogDebug($"メッセージをキューから処理: {typeof(TMessage).Name}, ID={message.MessageId}");
                        await _serviceBus.PublishAsync(message, cancellationToken);
                    }
                    else
                    {
                        // キューが空の場合は少し待機
                        await Task.Delay(100, cancellationToken);
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    _logger.LogError(ex, $"メッセージ処理中にエラーが発生: {typeof(TMessage).Name}");
                    // エラーが発生しても処理を継続
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            Stop();
            _cancellationSource.Cancel();
            _cancellationSource.Dispose();

            // すべての処理タスクの完了を待機
            Task.WhenAll(_processingTasks.Values).Wait(TimeSpan.FromSeconds(5));

            _processingTasks.Clear();
            foreach (var queue in _queues.Values)
            {
                if (queue is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _queues.Clear();
        }
    }

    /// <summary>
    /// メッセージキューの拡張メソッド
    /// </summary>
    public static class MessageQueueExtensions
    {
        /// <summary>
        /// メッセージキューをDIに登録
        /// </summary>
        public static IServiceCollection AddMessageQueue(this IServiceCollection services)
        {
            services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();
            return services;
        }

        /// <summary>
        /// サービスバスとメッセージキューの両方をDIに登録
        /// </summary>
        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddServiceBus();
            services.AddMessageQueue();
            return services;
        }
    }
}
