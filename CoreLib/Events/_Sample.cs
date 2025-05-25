using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    internal class _Sample
    {
        // 独自のイベントを定義
        public class UserCreatedEvent : EventArgs
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class OrderPlacedEvent : EventArgs
        {
            public string OrderId { get; set; }
            public string UserId { get; set; }
            public decimal Amount { get; set; }
            public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        }

        class Program
        {
            static async Task Main()
            {
                // DIコンテナの設定
                var services = new ServiceCollection();
                services.AddSingleton<EventManager>();
                services.AddInMemoryEventStore();
                var serviceProvider = services.BuildServiceProvider();

                // EventManagerとEventStoreServiceを取得
                var eventManager = serviceProvider.GetRequiredService<EventManager>();
                var eventStoreService = serviceProvider.GetRequiredService<EventStoreService>();

                // イベントの購読
                using var userCreatedSubscription = eventManager.Subscribe<UserCreatedEvent>((sender, args) =>
                {
                    Console.WriteLine($"ユーザーが作成されました: {args.Username}（ID: {args.UserId}）");
                });

                using var orderPlacedSubscription = eventManager.Subscribe<OrderPlacedEvent>((sender, args) =>
                {
                    Console.WriteLine($"注文が確定しました - 注文ID: {args.OrderId}, 金額: {args.Amount:C}");
                });

                // イベントの発行と保存
                var userEvent = new UserCreatedEvent
                {
                    UserId = "user123",
                    Username = "tanaka"
                };

                await eventStoreService.SaveAndPublishAsync("user-stream", userEvent, new object());

                var orderEvent = new OrderPlacedEvent
                {
                    OrderId = "order456",
                    UserId = "user123",
                    Amount = 5000
                };

                await eventStoreService.SaveAndPublishAsync("order-stream", orderEvent, new object());

                // 履歴イベントの読み込み
                Console.WriteLine("\n--- ユーザーストリームのイベント履歴 ---");
                var userEvents = await eventStoreService.ReadStreamEventsAsync("user-stream");
                foreach (var storedEvent in userEvents)
                {
                    Console.WriteLine($"イベント: {storedEvent.EventType}, バージョン: {storedEvent.Version}, 時刻: {storedEvent.Timestamp}");

                    if (storedEvent.EventType.EndsWith("UserCreatedEvent"))
                    {
                        var data = storedEvent.DeserializeData<UserCreatedEvent>();
                        Console.WriteLine($"  ユーザー名: {data.Username}, ID: {data.UserId}");
                    }
                }

                // DI登録用の簡単な例
                static void ConfigureServices(IServiceCollection services)
                {
                    services.AddSingleton<EventManager>();
                    services.AddInMemoryEventStore();
                    services.AddTransient<EventStoreService>();

                    // 独自の永続化実装がある場合は以下のように登録
                    // services.AddSingleton<IEventStore, SqlEventStore>();
                    // services.AddSingleton<IEventStore, MongoEventStore>();
                }
            }
        }
    }
}
