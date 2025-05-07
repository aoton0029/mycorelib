using CoreLib.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Messaging
{
    /// <summary>
    /// メッセージ処理のパイプライン動作を定義
    /// </summary>
    public interface IMessagePipelineBehavior<TMessage> where TMessage : IMessage
    {
        /// <summary>
        /// メッセージをパイプライン処理
        /// </summary>
        Task ProcessAsync(
            TMessage message,
            Func<TMessage, CancellationToken, Task> next,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ロギング用パイプライン動作
    /// </summary>
    public class LoggingPipelineBehavior<TMessage> : IMessagePipelineBehavior<TMessage>
        where TMessage : IMessage
    {
        private readonly IAppLogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggingPipelineBehavior(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージをパイプライン処理
        /// </summary>
        public async Task ProcessAsync(
            TMessage message,
            Func<TMessage, CancellationToken, Task> next,
            CancellationToken cancellationToken = default)
        {
            var messageType = typeof(TMessage).Name;

            try
            {
                _logger.LogDebug($"メッセージ処理開始: {messageType}, ID={message.MessageId}");

                var startTime = DateTime.UtcNow;
                await next(message, cancellationToken);
                var duration = DateTime.UtcNow - startTime;

                _logger.LogDebug($"メッセージ処理完了: {messageType}, ID={message.MessageId}, 所要時間={duration.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"メッセージ処理でエラーが発生: {messageType}, ID={message.MessageId}");
                throw;
            }
        }
    }

    /// <summary>
    /// エラーハンドリング用パイプライン動作
    /// </summary>
    public class ErrorHandlingPipelineBehavior<TMessage> : IMessagePipelineBehavior<TMessage>
        where TMessage : IMessage
    {
        private readonly IAppLogger _logger;
        private readonly bool _swallowExceptions;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ErrorHandlingPipelineBehavior(IAppLogger logger, bool swallowExceptions = false)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _swallowExceptions = swallowExceptions;
        }

        /// <summary>
        /// メッセージをパイプライン処理
        /// </summary>
        public async Task ProcessAsync(
            TMessage message,
            Func<TMessage, CancellationToken, Task> next,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await next(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"メッセージ処理中にエラーが発生: {typeof(TMessage).Name}, ID={message.MessageId}");

                // 例外を飲み込むかどうか
                if (!_swallowExceptions)
                    throw;
            }
        }
    }

    /// <summary>
    /// メッセージパイプラインの提供
    /// </summary>
    public class MessagePipeline<TMessage> where TMessage : IMessage
    {
        private readonly List<IMessagePipelineBehavior<TMessage>> _behaviors;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly IAppLogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MessagePipeline(
            IMessageHandler<TMessage> handler,
            IEnumerable<IMessagePipelineBehavior<TMessage>> behaviors,
            IAppLogger logger)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _behaviors = behaviors?.ToList() ?? new List<IMessagePipelineBehavior<TMessage>>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// パイプラインを実行
        /// </summary>
        public async Task ExecuteAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // パイプラインが空の場合は直接ハンドラーを呼び出す
            if (_behaviors.Count == 0)
            {
                await _handler.HandleAsync(message, cancellationToken);
                return;
            }

            // パイプラインを構築して実行
            await ExecutePipelineAsync(0, message, cancellationToken);
        }

        private async Task ExecutePipelineAsync(int index, TMessage message, CancellationToken cancellationToken)
        {
            if (index >= _behaviors.Count)
            {
                // パイプラインの最後で実際のハンドラーを呼び出す
                await _handler.HandleAsync(message, cancellationToken);
                return;
            }

            // 現在の動作を取得し、次の動作への参照を渡す
            var behavior = _behaviors[index];
            await behavior.ProcessAsync(
                message,
                async (msg, token) => await ExecutePipelineAsync(index + 1, msg, token),
                cancellationToken);
        }
    }

    /// <summary>
    /// メッセージパイプラインファクトリ
    /// </summary>
    public class MessagePipelineFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppLogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MessagePipelineFactory(IServiceProvider serviceProvider, IAppLogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// メッセージタイプのパイプラインを作成
        /// </summary>
        public MessagePipeline<TMessage> CreatePipeline<TMessage>(IMessageHandler<TMessage> handler)
            where TMessage : IMessage
        {
            var behaviors = _serviceProvider.GetServices<IMessagePipelineBehavior<TMessage>>();
            return new MessagePipeline<TMessage>(handler, behaviors, _logger);
        }
    }
}
