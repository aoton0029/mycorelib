イベントの重複登録を防止するよう `EventHandlerManager` クラスを改善しましょう。同じイベントが複数回登録されないようにする機能を追加します。

「同じイベントは一つだけ」とのご要望ですね。先ほど提案した `EventHandlerManager` クラスに、同じイベントハンドラーが重複して登録されないようにする機能を追加します。

以下に、重複登録を防止するように修正したコードを提示します：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsのイベントハンドラーを一元管理するユーティリティクラス
    /// イベントのアタッチとデタッチを追跡し、一括処理を可能にします
    /// 同じイベントは一つだけ登録されることを保証します
    /// </summary>
    public class EventHandlerManager
    {
        // イベントハンドラー情報を格納するための内部クラス
        private class EventInfo
        {
            public object Sender { get; set; }
            public Delegate Handler { get; set; }
            public string EventName { get; set; }

            public EventInfo(object sender, Delegate handler, string eventName)
            {
                Sender = sender;
                Handler = handler;
                EventName = eventName;
            }

            // 同一イベント判定用の比較メソッド
            public bool IsSameEvent(object sender, string eventName, Delegate handler)
            {
                return Sender == sender && EventName == eventName && Handler == handler;
            }
        }

        // 登録済みのイベントハンドラーを保持するリスト
        private readonly List<EventInfo> _registeredEvents = new List<EventInfo>();

        /// <summary>
        /// イベントにハンドラーを登録し、管理対象に追加します
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "TextChanged")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // イベントにハンドラーを追加
            eventInfo.AddEventHandler(control, eventHandler);

            // 登録情報を保持
            _registeredEvents.Add(new EventInfo(control, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 標準的なEventHandlerを使用するイベントに対する、簡略化されたAttachメソッド
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "Load")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // イベントにハンドラーを追加
            eventInfo.AddEventHandler(control, eventHandler);

            // 登録情報を保持
            _registeredEvents.Add(new EventInfo(control, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 指定されたイベントが既に登録されているかを確認します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>既に登録されている場合はtrue</returns>
        public bool IsEventRegistered(Control control, string eventName, Delegate handler)
        {
            return _registeredEvents.Any(e => e.IsSameEvent(control, eventName, handler));
        }

        /// <summary>
        /// 特定のコントロールとイベントからハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>削除に成功したかどうか</returns>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            // 該当する登録を検索
            var eventToRemove = _registeredEvents.Find(e => e.IsSameEvent(control, eventName, handler));

            if (eventToRemove != null)
            {
                // リフレクションを使用してイベントを取得
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    // イベントからハンドラーを削除
                    eventInfo.RemoveEventHandler(control, handler);

                    // 管理リストから削除
                    _registeredEvents.Remove(eventToRemove);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 特定のコントロールに登録されているすべてのイベントハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        public void DetachAllEvents(Control control)
        {
            // 削除対象のイベント登録情報を取得
            var eventsToRemove = _registeredEvents.FindAll(e => e.Sender == control);

            foreach (var eventInfo in eventsToRemove)
            {
                // コントロールからイベントタイプを取得
                var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                if (controlEventInfo != null)
                {
                    // イベントからハンドラーを削除
                    controlEventInfo.RemoveEventHandler(control, eventInfo.Handler);
                }
            }

            // 管理リストから削除
            _registeredEvents.RemoveAll(e => e.Sender == control);
        }

        /// <summary>
        /// 登録されている全てのイベントハンドラーを削除します
        /// フォーム終了時などに呼び出すとメモリリークを防止できます
        /// </summary>
        public void DetachAllEvents()
        {
            foreach (var eventInfo in _registeredEvents)
            {
                if (eventInfo.Sender is Control control)
                {
                    // コントロールからイベントタイプを取得
                    var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                    if (controlEventInfo != null)
                    {
                        // イベントからハンドラーを削除
                        controlEventInfo.RemoveEventHandler(control, eventInfo.Handler);
                    }
                }
            }

            // すべての登録をクリア
            _registeredEvents.Clear();
        }

        /// <summary>
        /// コントロールが登録しているイベントの数を返します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <returns>登録イベント数</returns>
        public int GetRegisteredEventCount(Control control)
        {
            return _registeredEvents.Count(e => e.Sender == control);
        }

        /// <summary>
        /// 管理下にある全てのイベント登録数を返します
        /// </summary>
        /// <returns>登録イベント総数</returns>
        public int GetRegisteredEventCount()
        {
            return _registeredEvents.Count;
        }
    }
}
```

## 主な変更点

1. **重複チェック機能の追加**: 
   - `IsEventRegistered` メソッドで、同じイベントが既に登録されているか確認します。
   - EventInfoクラスに `IsSameEvent` メソッドを追加し、イベントの比較を容易にしました。

2. **AttachEventメソッドの戻り値の変更**:
   - AttachEventメソッドはbool型を返すように変更し、イベントが新規に登録されたかどうかがわかるようにしました。
   - 既に同じイベントが登録されている場合は、重複して登録せず `false` を返します。

## 使用例

```csharp
public partial class MyForm : Form
{
    private EventHandlerManager _eventManager = new EventHandlerManager();
    
    public MyForm()
    {
        InitializeComponent();
        RegisterEvents();
    }
    
    private void RegisterEvents()
    {
        // イベントを登録（重複を許可しない）
        _eventManager.AttachEvent(button1, "Click", Button1_Click);
        
        // 同じイベントを再度登録しようとしても無視される
        bool registered = _eventManager.AttachEvent(button1, "Click", Button1_Click);
        // registered は false になる
        
        // 別のイベントは正常に登録される
        _eventManager.AttachEvent(textBox1, "TextChanged", TextBox1_TextChanged);
    }
    
    private void Button1_Click(object sender, EventArgs e)
    {
        // このハンドラーは一度だけ呼び出される
    }
    
    private void TextBox1_TextChanged(object sender, EventArgs e)
    {
        // 処理
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _eventManager.DetachAllEvents();
        base.OnFormClosing(e);
    }
}
```

この実装により、同じコントロールの同じイベントに同じハンドラーを複数回アタッチしようとしても、一度だけしか登録されなくなります。イベントが重複して発火する問題が解消され、安全にイベント管理を行うことができます。

---
シングルトンのEventBusとEventHandlerManagerを組み合わせて、WinFormsのイベントを一元管理するソリューションを作成しましょう。リポジトリにある既存のメッセージング機能と統合した実装を提案します。リポジトリの既存コードを確認した結果、WinFormsのEventHandlerManagerとシングルトンパターンのEventBusを統合する解決策を提案します。既存のServiceBusとの統合を考慮した実装を作成します。

# シングルトンEventBusとEventHandlerManagerの統合実装

まず、EventBusとEventHandlerManagerの両方を実装した統合クラスを作成します：

```csharp
using CoreLib.Logging;
using CoreLib.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsイベントバスのインターフェース
    /// コントロールイベントとアプリケーションイベントの両方を管理
    /// </summary>
    public interface IWinFormsEventBus
    {
        // イベントハンドラー管理メソッド
        bool AttachEvent(Control control, string eventName, EventHandler eventHandler);
        bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) where TEventArgs : EventArgs;
        bool DetachEvent(Control control, string eventName, Delegate handler);
        void DetachAllEvents(Control control);
        void DetachAllEvents();
        int GetRegisteredEventCount(Control control);
        int GetRegisteredEventCount();

        // アプリケーションイベント管理メソッド
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;
        void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;
        void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;
    }

    /// <summary>
    /// WinFormsイベントバスの実装
    /// シングルトンパターンでイベントの一元管理を行う
    /// </summary>
    public class WinFormsEventBus : IWinFormsEventBus
    {
        // シングルトンインスタンス
        private static readonly Lazy<WinFormsEventBus> _instance = 
            new Lazy<WinFormsEventBus>(() => new WinFormsEventBus());

        // ServiceBusのインスタンス
        private readonly IServiceBus _serviceBus;
        // ロガー
        private readonly IAppLogger _logger;
        // イベント管理のための内部クラスインスタンス
        private readonly EventHandlerManager _eventHandlerManager;

        /// <summary>
        /// シングルトンインスタンスを取得
        /// </summary>
        public static WinFormsEventBus Instance => _instance.Value;

        /// <summary>
        /// プライベートコンストラクタ - シングルトンパターン
        /// </summary>
        private WinFormsEventBus()
        {
            _eventHandlerManager = new EventHandlerManager();

            // DIコンテナがなくても動作するようにフォールバック実装
            try
            {
                // アプリケーション全体のDIコンテナからServiceBusとLoggerを解決
                var serviceProvider = ServiceProviderAccessor.GetServiceProvider();
                if (serviceProvider != null)
                {
                    _serviceBus = serviceProvider.GetService<IServiceBus>();
                    _logger = serviceProvider.GetService<IAppLogger>();
                }
            }
            catch
            {
                // DIコンテナからの取得に失敗した場合は内部でインスタンス作成
            }

            // ServiceBusがDIから解決できなかった場合のフォールバック
            if (_serviceBus == null)
            {
                // ロガーも同様
                if (_logger == null)
                {
                    _logger = new NullLogger(); // ダミーロガー（何も出力しない）
                }

                // 簡易的なServiceBus実装を作成
                _serviceBus = new SimpleServiceBus(_logger);
            }

            _logger.LogInformation($"WinFormsEventBus initialized");
        }

        #region EventHandlerManager委譲メソッド

        /// <summary>
        /// イベントにハンドラーを登録
        /// </summary>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            return _eventHandlerManager.AttachEvent(control, eventName, eventHandler);
        }

        /// <summary>
        /// イベントにハンドラーを登録（ジェネリック版）
        /// </summary>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            return _eventHandlerManager.AttachEvent<TEventArgs>(control, eventName, eventHandler);
        }

        /// <summary>
        /// イベントからハンドラーを削除
        /// </summary>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            return _eventHandlerManager.DetachEvent(control, eventName, handler);
        }

        /// <summary>
        /// 特定のコントロールのすべてのイベントハンドラーを削除
        /// </summary>
        public void DetachAllEvents(Control control)
        {
            _eventHandlerManager.DetachAllEvents(control);
        }

        /// <summary>
        /// すべてのイベントハンドラーを削除
        /// </summary>
        public void DetachAllEvents()
        {
            _eventHandlerManager.DetachAllEvents();
        }

        /// <summary>
        /// コントロールの登録済みイベント数を取得
        /// </summary>
        public int GetRegisteredEventCount(Control control)
        {
            return _eventHandlerManager.GetRegisteredEventCount(control);
        }

        /// <summary>
        /// 登録済みイベント総数を取得
        /// </summary>
        public int GetRegisteredEventCount()
        {
            return _eventHandlerManager.GetRegisteredEventCount();
        }

        #endregion

        #region ServiceBus委譲メソッド

        /// <summary>
        /// メッセージを発行
        /// </summary>
        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IMessage
        {
            return _serviceBus.PublishAsync(message, cancellationToken);
        }

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            _serviceBus.RegisterHandler(handler);
        }

        /// <summary>
        /// メッセージハンドラーの登録を解除
        /// </summary>
        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            _serviceBus.UnregisterHandler(handler);
        }

        #endregion
    }

    /// <summary>
    /// DIからサービスプロバイダーを取得するためのユーティリティクラス
    /// </summary>
    public static class ServiceProviderAccessor
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
    }

    /// <summary>
    /// WinFormsイベントバス用の拡張メソッド
    /// </summary>
    public static class WinFormsEventBusExtensions
    {
        /// <summary>
        /// WinFormsEventBusをDIに登録
        /// </summary>
        public static IServiceCollection AddWinFormsEventBus(this IServiceCollection services)
        {
            // シングルトンインスタンスをDIコンテナに登録
            services.AddSingleton<IWinFormsEventBus>(_ => WinFormsEventBus.Instance);
            return services;
        }

        /// <summary>
        /// WinFormsアプリケーションでDIサービスプロバイダーを初期化
        /// </summary>
        public static void InitializeServiceProvider(this IServiceProvider serviceProvider)
        {
            ServiceProviderAccessor.Initialize(serviceProvider);
        }
    }

    /// <summary>
    /// DIが使用できない場合のシンプルなServiceBus実装
    /// </summary>
    internal class SimpleServiceBus : IServiceBus
    {
        private readonly IAppLogger _logger;
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly object _syncRoot = new object();

        public SimpleServiceBus(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);
            _logger.LogDebug($"メッセージを発行: {messageType.Name}, ID={message.MessageId}");

            List<object> handlers;
            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(messageType, out handlers) || handlers.Count == 0)
                {
                    _logger.LogWarning($"メッセージタイプ {messageType.Name} のハンドラーが登録されていません");
                    return;
                }
                handlers = new List<object>(handlers); // スレッドセーフのためにコピー
            }

            var typedHandlers = handlers.Cast<IMessageHandler<TMessage>>();
            var tasks = new List<Task>();

            foreach (var handler in typedHandlers)
            {
                try
                {
                    tasks.Add(handler.HandleAsync(message, cancellationToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ハンドラーの実行中にエラーが発生: {ex.Message}");
                }
            }

            await Task.WhenAll(tasks);
        }

        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(messageType, out var handlers))
                {
                    handlers = new List<object>();
                    _handlers[messageType] = handlers;
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                    _logger.LogDebug($"ハンドラーを登録: {handler.GetType().Name} for {messageType.Name}");
                }
            }
        }

        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (_handlers.TryGetValue(messageType, out var handlers))
                {
                    if (handlers.Remove(handler))
                    {
                        _logger.LogDebug($"ハンドラーの登録を解除: {handler.GetType().Name} for {messageType.Name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// ロガーが利用できない場合のダミー実装
    /// </summary>
    internal class NullLogger : IAppLogger
    {
        public void LogDebug(string message) { }
        public void LogInformation(string message) { }
        public void LogWarning(string message) { }
        public void LogError(string message) { }
        public void LogError(Exception exception, string message) { }
        public void LogCritical(string message) { }
        public void LogCritical(Exception exception, string message) { }
    }

    /// <summary>
    /// WinFormsイベント用のメッセージクラス
    /// </summary>
    public class WinFormsEventMessage : IMessage
    {
        public Guid MessageId { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public Control Sender { get; }
        public string EventName { get; }
        public object EventArgs { get; }

        public WinFormsEventMessage(Control sender, string eventName, object eventArgs)
        {
            Sender = sender;
            EventName = eventName;
            EventArgs = eventArgs;
        }
    }
}
```

次に、前回提供したEventHandlerManagerクラスに、イベント発火をEventBusに通知する機能を追加します：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CoreLib.Messaging;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsのイベントハンドラーを一元管理するユーティリティクラス
    /// イベントのアタッチとデタッチを追跡し、一括処理を可能にします
    /// 同じイベントは一つだけ登録されることを保証します
    /// </summary>
    public class EventHandlerManager
    {
        // イベントハンドラー情報を格納するための内部クラス
        private class EventInfo
        {
            public Control Sender { get; set; }
            public Delegate Handler { get; set; }
            public Delegate OriginalHandler { get; set; } // 元のハンドラー
            public string EventName { get; set; }

            public EventInfo(Control sender, Delegate handler, Delegate originalHandler, string eventName)
            {
                Sender = sender;
                Handler = handler;
                OriginalHandler = originalHandler;
                EventName = eventName;
            }

            // 同一イベント判定用の比較メソッド
            public bool IsSameEvent(Control sender, string eventName, Delegate handler)
            {
                return Sender == sender && 
                       EventName == eventName && 
                       (Handler == handler || OriginalHandler == handler);
            }
        }

        // 登録済みのイベントハンドラーを保持するリスト
        private readonly List<EventInfo> _registeredEvents = new List<EventInfo>();

        /// <summary>
        /// イベントにハンドラーを登録し、管理対象に追加します
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "TextChanged")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // EventBusへの転送を行うラッパーハンドラーを作成
            EventHandler<TEventArgs> wrapperHandler = (sender, args) =>
            {
                // 元のハンドラーを呼び出し
                eventHandler(sender, args);

                // EventBusがあれば、イベントをグローバルメッセージとして発行
                try
                {
                    // EventBusにイベントを通知
                    var eventMessage = new WinFormsEventMessage(control, eventName, args);
                    WinFormsEventBus.Instance.PublishAsync(eventMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // イベント転送中のエラーはアプリケーションを停止させない
                    System.Diagnostics.Debug.WriteLine($"EventBus通知エラー: {ex.Message}");
                }
            };

            // イベントにラッパーハンドラーを追加
            eventInfo.AddEventHandler(control, wrapperHandler);

            // 登録情報を保持（元のハンドラーとラッパーハンドラーの両方を記録）
            _registeredEvents.Add(new EventInfo(control, wrapperHandler, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 標準的なEventHandlerを使用するイベントに対する、簡略化されたAttachメソッド
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "Load")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // EventBusへの転送を行うラッパーハンドラーを作成
            EventHandler wrapperHandler = (sender, args) =>
            {
                // 元のハンドラーを呼び出し
                eventHandler(sender, args);

                // EventBusがあれば、イベントをグローバルメッセージとして発行
                try
                {
                    // EventBusにイベントを通知
                    var eventMessage = new WinFormsEventMessage(control, eventName, args);
                    WinFormsEventBus.Instance.PublishAsync(eventMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // イベント転送中のエラーはアプリケーションを停止させない
                    System.Diagnostics.Debug.WriteLine($"EventBus通知エラー: {ex.Message}");
                }
            };

            // イベントにラッパーハンドラーを追加
            eventInfo.AddEventHandler(control, wrapperHandler);

            // 登録情報を保持（元のハンドラーとラッパーハンドラーの両方を記録）
            _registeredEvents.Add(new EventInfo(control, wrapperHandler, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 指定されたイベントが既に登録されているかを確認します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>既に登録されている場合はtrue</returns>
        public bool IsEventRegistered(Control control, string eventName, Delegate handler)
        {
            return _registeredEvents.Any(e => e.IsSameEvent(control, eventName, handler));
        }

        /// <summary>
        /// 特定のコントロールとイベントからハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>削除に成功したかどうか</returns>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            // 該当する登録を検索
            var eventToRemove = _registeredEvents.Find(e => e.IsSameEvent(control, eventName, handler));

            if (eventToRemove != null)
            {
                // リフレクションを使用してイベントを取得
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    // イベントからハンドラーを削除
                    eventInfo.RemoveEventHandler(control, eventToRemove.Handler);

                    // 管理リストから削除
                    _registeredEvents.Remove(eventToRemove);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 特定のコントロールに登録されているすべてのイベントハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        public void DetachAllEvents(Control control)
        {
            // 削除対象のイベント登録情報を取得
            var eventsToRemove = _registeredEvents.FindAll(e => e.Sender == control);

            foreach (var eventInfo in eventsToRemove)
            {
                // コントロールからイベントタイプを取得
                var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                if (controlEventInfo != null)
                {
                    // イベントからハンドラーを削除
                    controlEventInfo.RemoveEventHandler(control, even