using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// ドメインイベントの基底クラス
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// イベントID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// イベント種別
        /// </summary>
        public string EventType => GetType().Name;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// ドメインイベントハンドラーのインターフェース
    /// </summary>
    /// <typeparam name="TEvent">処理するイベントの型</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
    {
        /// <summary>
        /// イベントを処理
        /// </summary>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ドメインイベントのディスパッチャー
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// イベントをディスパッチ
        /// </summary>
        Task DispatchAsync(DomainEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数のイベントをディスパッチ
        /// </summary>
        Task DispatchAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ドメインイベントの発行者
    /// </summary>
    public interface IDomainEventPublisher
    {
        /// <summary>
        /// イベントを発行
        /// </summary>
        Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数のイベントを発行
        /// </summary>
        Task PublishAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
    }
}
