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