using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// イベント基底インターフェース
    /// </summary>
    public interface IEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
    }

    /// <summary>
    /// イベント発行インターフェース
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// イベントを発行する
        /// </summary>
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }

    /// <summary>
    /// イベントハンドラインターフェース
    /// </summary>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// イベントを処理する
        /// </summary>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}
