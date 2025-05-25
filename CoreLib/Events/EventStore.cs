using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// イベントを永続化して履歴を管理するEventStoreインターフェース
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// イベントをストアに追加します
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="event">イベント</param>
        /// <param name="expectedVersion">予想されるバージョン（楽観的並行制御用）</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>保存されたイベントのバージョン</returns>
        Task<long> AppendToStreamAsync<TEvent>(string streamId, TEvent @event, long? expectedVersion = null, CancellationToken cancellationToken = default) where TEvent : class;

        /// <summary>
        /// 複数のイベントをストアに追加します
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="events">イベントコレクション</param>
        /// <param name="expectedVersion">予想されるバージョン（楽観的並行制御用）</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>保存された最後のイベントのバージョン</returns>
        Task<long> AppendToStreamAsync<TEvent>(string streamId, IEnumerable<TEvent> events, long? expectedVersion = null, CancellationToken cancellationToken = default) where TEvent : class;

        /// <summary>
        /// 特定のストリームからイベントを読み込みます
        /// </summary>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="fromVersion">開始バージョン</param>
        /// <param name="count">読み込むイベント数（デフォルトは全て）</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>イベントのコレクション</returns>
        Task<IEnumerable<StoredEvent>> ReadStreamAsync(string streamId, long fromVersion = 0, int? count = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 特定の型のイベントをすべてのストリームから読み込みます
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>イベントのコレクション</returns>
        Task<IEnumerable<StoredEvent>> ReadEventsByTypeAsync<TEvent>(CancellationToken cancellationToken = default) where TEvent : class;

        /// <summary>
        /// すべてのイベントを読み込みます
        /// </summary>
        /// <param name="fromPosition">開始位置</param>
        /// <param name="count">読み込むイベント数（デフォルトは全て）</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>イベントのコレクション</returns>
        Task<IEnumerable<StoredEvent>> ReadAllEventsAsync(long fromPosition = 0, int? count = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// ストリームのバージョンを取得します
        /// </summary>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>最新バージョン（存在しない場合は-1）</returns>
        Task<long> GetStreamVersionAsync(string streamId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 保存されたイベントを表すクラス
    /// </summary>
    public class StoredEvent
    {
        /// <summary>
        /// グローバルな位置
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// イベントストリームID
        /// </summary>
        public string StreamId { get; set; }

        /// <summary>
        /// ストリーム内のバージョン
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// イベントの型名
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// イベントデータ（JSON形式）
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// メタデータ（JSON形式）
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// イベントのタイムスタンプ
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// イベントデータを特定の型にデシリアライズします
        /// </summary>
        /// <typeparam name="T">デシリアライズ先の型</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public T DeserializeData<T>()
        {
            return JsonSerializer.Deserialize<T>(Data);
        }

        /// <summary>
        /// メタデータを特定の型にデシリアライズします
        /// </summary>
        /// <typeparam name="T">デシリアライズ先の型</typeparam>
        /// <returns>デシリアライズされたオブジェクト</returns>
        public T DeserializeMetadata<T>()
        {
            return string.IsNullOrEmpty(Metadata)
                ? default
                : JsonSerializer.Deserialize<T>(Metadata);
        }
    }

    /// <summary>
    /// インメモリ実装のEventStore
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, List<StoredEvent>> _streams = new Dictionary<string, List<StoredEvent>>();
        private long _globalPosition = 0;

        /// <summary>
        /// イベントをストアに追加します
        /// </summary>
        public async Task<long> AppendToStreamAsync<TEvent>(string streamId, TEvent @event, long? expectedVersion = null, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            return await AppendToStreamAsync(streamId, new[] { @event }, expectedVersion, cancellationToken);
        }

        /// <summary>
        /// 複数のイベントをストアに追加します
        /// </summary>
        public async Task<long> AppendToStreamAsync<TEvent>(string streamId, IEnumerable<TEvent> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));

            if (events == null || !events.Any())
                throw new ArgumentException("イベントが指定されていません", nameof(events));

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (!_streams.TryGetValue(streamId, out var stream))
                {
                    stream = new List<StoredEvent>();
                    _streams[streamId] = stream;
                }

                // 現在のバージョンチェック（楽観的並行制御）
                long currentVersion = stream.Count > 0 ? stream.Max(e => e.Version) : -1;
                if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                {
                    throw new InvalidOperationException($"バージョンの競合が発生しました。予想: {expectedVersion}, 実際: {currentVersion}");
                }

                var eventList = events.ToList();
                var storedEvents = new List<StoredEvent>();

                // イベントのシリアライズと追加
                foreach (var @event in eventList)
                {
                    currentVersion++;
                    _globalPosition++;

                    var storedEvent = new StoredEvent
                    {
                        Position = _globalPosition,
                        StreamId = streamId,
                        Version = currentVersion,
                        EventType = @event.GetType().FullName,
                        Data = JsonSerializer.Serialize(@event),
                        Metadata = null, // 必要に応じてメタデータを設定
                        Timestamp = DateTime.UtcNow
                    };

                    stream.Add(storedEvent);
                    storedEvents.Add(storedEvent);
                }

                return currentVersion;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 特定のストリームからイベントを読み込みます
        /// </summary>
        public async Task<IEnumerable<StoredEvent>> ReadStreamAsync(string streamId, long fromVersion = 0, int? count = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (!_streams.TryGetValue(streamId, out var stream))
                {
                    return Enumerable.Empty<StoredEvent>();
                }

                var events = stream
                    .Where(e => e.Version >= fromVersion)
                    .OrderBy(e => e.Version);

                if (count.HasValue)
                {
                    events = events.Take(count.Value);
                }

                return events.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 特定の型のイベントをすべてのストリームから読み込みます
        /// </summary>
        public async Task<IEnumerable<StoredEvent>> ReadEventsByTypeAsync<TEvent>(CancellationToken cancellationToken = default) where TEvent : class
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var eventTypeName = typeof(TEvent).FullName;
                var events = _streams
                    .SelectMany(s => s.Value)
                    .Where(e => e.EventType == eventTypeName)
                    .OrderBy(e => e.Position)
                    .ToList();

                return events;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// すべてのイベントを読み込みます
        /// </summary>
        public async Task<IEnumerable<StoredEvent>> ReadAllEventsAsync(long fromPosition = 0, int? count = null, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var events = _streams
                    .SelectMany(s => s.Value)
                    .Where(e => e.Position >= fromPosition)
                    .OrderBy(e => e.Position);

                if (count.HasValue)
                {
                    events = events.Take(count.Value);
                }

                return events.ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// ストリームのバージョンを取得します
        /// </summary>
        public async Task<long> GetStreamVersionAsync(string streamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(streamId))
                throw new ArgumentNullException(nameof(streamId));

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (!_streams.TryGetValue(streamId, out var stream) || stream.Count == 0)
                {
                    return -1;
                }

                return stream.Max(e => e.Version);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    /// <summary>
    /// イベントストアとイベントバス（EventManager）を統合するサービス
    /// </summary>
    public class EventStoreService
    {
        private readonly IEventStore _eventStore;
        private readonly EventManager _eventManager;

        public EventStoreService(IEventStore eventStore, EventManager eventManager)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
        }

        /// <summary>
        /// イベントを保存して発行します
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="event">イベント</param>
        /// <param name="sender">イベント発行者</param>
        /// <param name="expectedVersion">予想されるバージョン</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>保存されたイベントのバージョン</returns>
        public async Task<long> SaveAndPublishAsync<TEvent>(string streamId, TEvent @event, object sender, long? expectedVersion = null, CancellationToken cancellationToken = default)
            where TEvent : EventArgs
        {
            // イベントの保存
            var version = await _eventStore.AppendToStreamAsync(streamId, @event, expectedVersion, cancellationToken);

            // イベントの発行
            await _eventManager.PublishAsync(sender, @event, cancellationToken);

            return version;
        }

        /// <summary>
        /// 複数のイベントを保存して発行します
        /// </summary>
        /// <typeparam name="TEvent">イベントの型</typeparam>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="events">イベントとその発行者のペアのコレクション</param>
        /// <param name="expectedVersion">予想されるバージョン</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>保存された最後のイベントのバージョン</returns>
        public async Task<long> SaveAndPublishManyAsync<TEvent>(string streamId, IEnumerable<(TEvent Event, object Sender)> events, long? expectedVersion = null, CancellationToken cancellationToken = default)
            where TEvent : EventArgs
        {
            // イベントだけを抽出して保存
            var eventList = events.Select(e => e.Event).ToList();
            var version = await _eventStore.AppendToStreamAsync(streamId, eventList, expectedVersion, cancellationToken);

            // 各イベントを発行
            foreach (var (eventObj, sender) in events)
            {
                await _eventManager.PublishAsync(sender, eventObj, cancellationToken);
            }

            return version;
        }

        /// <summary>
        /// 指定したストリームから履歴イベントを読み込みます
        /// </summary>
        /// <param name="streamId">イベントストリームID</param>
        /// <param name="fromVersion">読み込みを開始するバージョン</param>
        /// <param name="count">読み込むイベント数</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        /// <returns>イベントのコレクション</returns>
        public Task<IEnumerable<StoredEvent>> ReadStreamEventsAsync(string streamId, long fromVersion = 0, int? count = null, CancellationToken cancellationToken = default)
        {
            return _eventStore.ReadStreamAsync(streamId, fromVersion, count, cancellationToken);
        }
    }

    /// <summary>
    /// DIコンテナ用の拡張メソッド
    /// </summary>
    public static class EventStoreExtensions
    {
        /// <summary>
        /// インメモリEventStoreをDIコンテナに登録します
        /// </summary>
        public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
        {
            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddTransient<EventStoreService>();
            return services;
        }
    }
}
