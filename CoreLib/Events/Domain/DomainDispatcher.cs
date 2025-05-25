using CoreLib.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// ドメインイベントディスパッチャーの実装
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 単一のイベントをディスパッチ
        /// </summary>
        public async Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation($"ドメインイベントをディスパッチ: {@event.EventType}, ID: {@event.Id}");

            var eventType = @event.GetType();

            // リフレクションを使用して、イベントタイプに対応する適切なハンドラーを取得
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            // すべての適合するハンドラーをDIコンテナから解決
            var handlers = _serviceProvider.GetServices(handlerType);

            if (!handlers.Any())
            {
                _logger.LogWarning($"イベント {@event.EventType} に対するハンドラーが登録されていません");
                return;
            }

            // すべてのハンドラーを非同期に実行
            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                try
                {
                    // リフレクションを使用して HandleAsync メソッドを呼び出す
                    var method = handlerType.GetMethod("HandleAsync");
                    if (method != null)
                    {
                        var task = (Task)method.Invoke(handler, new object[] { @event, cancellationToken });
                        tasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"イベントハンドラーの呼び出し中にエラーが発生: {@event.EventType}");
                    throw;
                }
            }

            // すべてのハンドラーが完了するのを待機
            await Task.WhenAll(tasks);

            _logger.LogDebug($"ドメインイベント処理完了: {@event.EventType}, ID: {@event.Id}");
        }

        /// <summary>
        /// 複数のイベントをディスパッチ
        /// </summary>
        public async Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
            {
                await DispatchAsync(@event, cancellationToken);
            }
        }
    }
}
