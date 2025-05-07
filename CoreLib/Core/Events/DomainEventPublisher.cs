using CoreLib.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// ドメインイベントパブリッシャーの実装
    /// </summary>
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IAppLogger _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DomainEventPublisher(IDomainEventDispatcher dispatcher, IAppLogger logger)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// イベントを発行
        /// </summary>
        public async Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation($"ドメインイベントを発行: {@event.EventType}, ID: {@event.Id}");

            try
            {
                await _dispatcher.DispatchAsync(@event, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"イベント発行中にエラーが発生: {@event.EventType}, ID: {@event.Id}");
                throw;
            }
        }

        /// <summary>
        /// 複数のイベントを発行
        /// </summary>
        public async Task PublishAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var eventsList = events.ToList();
            if (!eventsList.Any())
                return;

            _logger.LogInformation($"{eventsList.Count}件のドメインイベントを発行");

            try
            {
                await _dispatcher.DispatchAsync(eventsList, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"複数イベント発行中にエラーが発生");
                throw;
            }
        }
    }
}
