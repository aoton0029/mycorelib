using CoreLib.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Messaging
{
    internal class _Sample
    {
        /*
         サービスバスの利点
        1.	疎結合性の向上: サービスやコンポーネント間の直接的な依存関係を削減します。
        2.	スケーラビリティ: メッセージングシステムを使用することで、システムの個々の部分を独立してスケールできます。
        3.	耐障害性: コンポーネント間の通信が非同期で行われるため、一部の障害が全体に影響しにくくなります。
        4.	拡張性: 新しい機能やサービスを追加する際に、既存のコードを変更せずに統合できます。
        5.	パイプライン処理: クロスカッティング関心事（ロギング、エラーハンドリングなど）を一貫して適用できます。
         */
        // プログラム起動時の設定例
        public void ConfigureServices(IServiceCollection services)
        {
            // 基本的なメッセージングサービスの登録
            services.AddMessaging();
            services.AddDistributedMessageBus();

            // パイプライン動作の登録
            services.AddTransient(typeof(IMessagePipelineBehavior<>), typeof(LoggingPipelineBehavior<>));
            services.AddTransient(typeof(IMessagePipelineBehavior<>), typeof(ErrorHandlingPipelineBehavior<>));

            // メッセージハンドラーの登録
            services.AddMessageHandler<UserCreatedMessage, UserCreatedMessageHandler>();
            //services.AddMessageHandler<OrderPlacedMessage, OrderPlacedMessageHandler>();
        }

        // メッセージの定義例
        public class UserCreatedMessage : Message
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }

        // メッセージハンドラーの実装例
        public class UserCreatedMessageHandler : IMessageHandler<UserCreatedMessage>
        {
            private readonly IAppLogger _logger;

            public UserCreatedMessageHandler(IAppLogger logger)
            {
                _logger = logger;
            }

            public async Task HandleAsync(UserCreatedMessage message, CancellationToken cancellationToken = default)
            {
                _logger.LogInformation($"新しいユーザーが作成されました: {message.UserId}, {message.Username}");
                // 実際の処理ロジック...
                await Task.CompletedTask;
            }
        }

        // サービスバスの使用例
        public class UserService
        {
            private readonly IServiceBus _serviceBus;

            public UserService(IServiceBus serviceBus)
            {
                _serviceBus = serviceBus;
            }

            public async Task CreateUserAsync(string username, string email)
            {
                // ユーザー作成ロジック...
                string userId = Guid.NewGuid().ToString();

                // メッセージの発行
                var message = new UserCreatedMessage
                {
                    UserId = userId,
                    Username = username,
                    Email = email
                };

                await _serviceBus.PublishAsync(message);
            }
        }

        // 分散メッセージバスの使用例
        public class NotificationService
        {
            private readonly IDistributedMessageBus _messageBus;

            public NotificationService(IDistributedMessageBus messageBus)
            {
                _messageBus = messageBus;
            }

            public async Task InitializeAsync(CancellationToken cancellationToken = default)
            {
                // ユーザー作成メッセージをサブスクライブ
                await _messageBus.SubscribeAsync<UserCreatedMessage>(HandleUserCreatedAsync, cancellationToken);
            }

            private async Task HandleUserCreatedAsync(UserCreatedMessage message, CancellationToken cancellationToken)
            {
                // 通知処理...
                Console.WriteLine($"ようこそメールを送信: {message.Email}");
                await Task.CompletedTask;
            }
        }
    }
}
