using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// イベントのアタッチとデタッチを管理するサービス
    /// </summary>
    public class EventAttachmentManager : IDisposable
    {
        private readonly Dictionary<string, List<EventAttachment>> _attachments = new();
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// イベントをアタッチし、アタッチメントIDを返します
        /// </summary>
        /// <typeparam name="TTarget">イベントを持つターゲットの型</typeparam>
        /// <typeparam name="THandler">ハンドラの型</typeparam>
        /// <param name="target">イベントが定義されているターゲットオブジェクト</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラ</param>
        /// <param name="group">グループ名（オプション）</param>
        /// <returns>アタッチメント識別子</returns>
        public string Attach<TTarget, THandler>(TTarget target, string eventName, THandler handler, string group = null)
            where THandler : Delegate
        {
            ThrowIfDisposed();

            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // リフレクションでイベントを取得
            var eventInfo = typeof(TTarget).GetEvent(eventName) ??
                throw new ArgumentException($"イベント '{eventName}' が '{typeof(TTarget).Name}' に存在しません");

            // イベントハンドラをアタッチ
            eventInfo.AddEventHandler(target, handler);

            // 識別子を生成
            var attachmentId = Guid.NewGuid().ToString();

            // アタッチメント情報を記録
            var attachment = new EventAttachment
            {
                Id = attachmentId,
                Target = target,
                EventName = eventName,
                Handler = handler,
                Group = group ?? string.Empty
            };

            lock (_lock)
            {
                if (!_attachments.TryGetValue(attachment.Group, out var groupAttachments))
                {
                    groupAttachments = new List<EventAttachment>();
                    _attachments[attachment.Group] = groupAttachments;
                }

                groupAttachments.Add(attachment);
            }

            return attachmentId;
        }

        /// <summary>
        /// EventHandlerを使用してイベントをアタッチするためのヘルパーメソッド
        /// </summary>
        public string AttachEventHandler(object target, string eventName, EventHandler handler, string group = null)
        {
            return Attach(target, eventName, handler, group);
        }

        /// <summary>
        /// ジェネリックEventHandlerを使用してイベントをアタッチするためのヘルパーメソッド
        /// </summary>
        public string AttachEventHandler<TEventArgs>(object target, string eventName, EventHandler<TEventArgs> handler, string group = null)
            where TEventArgs : EventArgs
        {
            return Attach(target, eventName, handler, group);
        }

        /// <summary>
        /// 特定のアタッチメントをデタッチします
        /// </summary>
        /// <param name="attachmentId">アタッチメントID</param>
        /// <returns>デタッチが成功したかどうか</returns>
        public bool Detach(string attachmentId)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(attachmentId))
                return false;

            lock (_lock)
            {
                foreach (var group in _attachments.Keys.ToArray())
                {
                    var attachments = _attachments[group];
                    var attachment = attachments.FirstOrDefault(a => a.Id == attachmentId);

                    if (attachment != null)
                    {
                        DetachSingle(attachment);
                        attachments.Remove(attachment);

                        // 空のグループを削除
                        if (attachments.Count == 0)
                        {
                            _attachments.Remove(group);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 指定したグループのすべてのイベントをデタッチします
        /// </summary>
        /// <param name="group">グループ名</param>
        /// <returns>デタッチしたイベントの数</returns>
        public int DetachGroup(string group)
        {
            ThrowIfDisposed();

            group ??= string.Empty;

            lock (_lock)
            {
                if (_attachments.TryGetValue(group, out var attachments))
                {
                    int count = attachments.Count;
                    foreach (var attachment in attachments)
                    {
                        DetachSingle(attachment);
                    }

                    _attachments.Remove(group);
                    return count;
                }
            }

            return 0;
        }

        /// <summary>
        /// 指定したターゲットオブジェクトに関連するすべてのイベントをデタッチします
        /// </summary>
        /// <param name="target">ターゲットオブジェクト</param>
        /// <returns>デタッチしたイベントの数</returns>
        public int DetachTarget(object target)
        {
            ThrowIfDisposed();

            if (target == null)
                return 0;

            int count = 0;

            lock (_lock)
            {
                foreach (var group in _attachments.Keys.ToArray())
                {
                    var attachments = _attachments[group];
                    var attachmentsToRemove = attachments.Where(a => ReferenceEquals(a.Target, target)).ToList();

                    foreach (var attachment in attachmentsToRemove)
                    {
                        DetachSingle(attachment);
                        attachments.Remove(attachment);
                        count++;
                    }

                    // 空のグループを削除
                    if (attachments.Count == 0)
                    {
                        _attachments.Remove(group);
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 登録されているすべてのイベントをデタッチします
        /// </summary>
        /// <returns>デタッチしたイベントの数</returns>
        public int DetachAll()
        {
            ThrowIfDisposed();

            int count = 0;
            lock (_lock)
            {
                foreach (var group in _attachments.Keys.ToArray())
                {
                    foreach (var attachment in _attachments[group])
                    {
                        DetachSingle(attachment);
                        count++;
                    }
                }

                _attachments.Clear();
            }

            return count;
        }

        /// <summary>
        /// 単一のアタッチメントをデタッチする内部メソッド
        /// </summary>
        private void DetachSingle(EventAttachment attachment)
        {
            try
            {
                if (attachment.Target is object target)
                {
                    var eventInfo = target.GetType().GetEvent(attachment.EventName);
                    if (eventInfo != null && attachment.Handler != null)
                    {
                        eventInfo.RemoveEventHandler(target, attachment.Handler);
                    }
                }
            }
            catch (Exception ex)
            {
                // リフレクション中の例外をキャッチ
                System.Diagnostics.Debug.WriteLine($"イベントデタッチエラー: {ex.Message}");
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventAttachmentManager));
            }
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                DetachAll();
                _disposed = true;
            }
        }

        /// <summary>
        /// 内部でイベントアタッチメント情報を保持するクラス
        /// </summary>
        private class EventAttachment
        {
            public string Id { get; init; }
            public object Target { get; init; }
            public string EventName { get; init; }
            public Delegate Handler { get; init; }
            public string Group { get; init; }
        }
    }

    /// <summary>
    /// イベントアタッチメント管理のための便利な拡張メソッド
    /// </summary>
    public static class EventAttachmentExtensions
    {
        /// <summary>
        /// コントロールのイベントを簡単にアタッチするための拡張メソッド
        /// </summary>
        /// <typeparam name="TControl">コントロールの型</typeparam>
        /// <param name="manager">EventAttachmentManager</param>
        /// <param name="control">イベントを持つコントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="action">実行するアクション</param>
        /// <param name="group">グループ名（オプション）</param>
        /// <returns>アタッチメントID</returns>
        public static string AttachAction<TControl>(
            this EventAttachmentManager manager,
            TControl control,
            string eventName,
            Action<TControl> action,
            string group = null)
        {
            EventHandler handler = (sender, args) =>
            {
                if (sender is TControl typedSender)
                {
                    action(typedSender);
                }
            };

            return manager.Attach(control, eventName, handler, group);
        }

        /// <summary>
        /// EventArgs情報も含めたアクションを簡単にアタッチするための拡張メソッド
        /// </summary>
        public static string AttachAction<TControl, TEventArgs>(
            this EventAttachmentManager manager,
            TControl control,
            string eventName,
            Action<TControl, TEventArgs> action,
            string group = null)
            where TEventArgs : EventArgs
        {
            EventHandler<TEventArgs> handler = (sender, args) =>
            {
                if (sender is TControl typedSender)
                {
                    action(typedSender, args);
                }
            };

            return manager.Attach(control, eventName, handler, group);
        }
    }
}
