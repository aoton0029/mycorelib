using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events.Store
{
    /// <summary>
    /// イベントストア用のインターフェース
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// ドメインイベントを保存
        /// </summary>
        Task SaveAsync(DomainEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// 複数のドメインイベントを保存
        /// </summary>
        Task SaveAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);

        /// <summary>
        /// 特定のエンティティに関連するイベントを取得
        /// </summary>
        Task<IEnumerable<DomainEvent>> GetByEntityIdAsync(string entityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// イベントタイプに基づいてイベントを取得
        /// </summary>
        Task<IEnumerable<DomainEvent>> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);

        /// <summary>
        /// 時間範囲でイベントを取得
        /// </summary>
        Task<IEnumerable<DomainEvent>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// イベントストアのエントリを表すモデル
    /// </summary>
    public class EventStoreEntry
    {
        /// <summary>
        /// エントリID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 関連するエンティティID
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// イベントタイプ
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// イベント発生日時
        /// </summary>
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// シリアライズされたイベントデータ
        /// </summary>
        public string EventData { get; set; }
    }
}
