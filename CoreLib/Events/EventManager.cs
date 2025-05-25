using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Events
{
    /// <summary>
    /// EventArgs型をキーにしてイベント管理を行うサービス
    /// </summary>
    public class EventManager : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<Type, List<Delegate>> _subscriptions = new ConcurrentDictionary<Type, List<Delegate>>();
        private bool _disposed;

        /// <summary>
        /// イベントを購読します
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="handler">イベントハンドラ</param>
        /// <returns>購読解除用のトークン</returns>
        public SubscriptionToken Subscribe<TEventArgs>(Action<object, TEventArgs> handler) where TEventArgs : EventArgs
        {
            ThrowIfDisposed();

            _semaphore.Wait();
            try
            {
                var eventType = typeof(TEventArgs);
                if (!_subscriptions.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<Delegate>();
                    _subscriptions[eventType] = handlers;
                }

                handlers.Add(handler);
                return new SubscriptionToken(this, eventType, handler);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// イベントの購読を解除します
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="handler">イベントハンドラ</param>
        public void Unsubscribe<TEventArgs>(Action<object, TEventArgs> handler) where TEventArgs : EventArgs
        {
            ThrowIfDisposed();

            _semaphore.Wait();
            try
            {
                var eventType = typeof(TEventArgs);
                if (_subscriptions.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);

                    // リストが空になった場合はエントリを削除
                    if (handlers.Count == 0)
                    {
                        _subscriptions.TryRemove(eventType, out _);
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// イベントを発行します
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="sender">イベント発行者</param>
        /// <param name="eventArgs">イベント引数</param>
        public void Publish<TEventArgs>(object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            ThrowIfDisposed();

            _semaphore.Wait();
            try
            {
                var eventType = typeof(TEventArgs);
                if (_subscriptions.TryGetValue(eventType, out var handlers))
                {
                    // コピーを作成して反復処理（ハンドラ内でUnsubscribeされる可能性があるため）
                    var handlersToNotify = new List<Delegate>(handlers);
                    foreach (var handler in handlersToNotify)
                    {
                        if (handler is Action<object, TEventArgs> typedHandler)
                        {
                            typedHandler(sender, eventArgs);
                        }
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// イベントを非同期で発行します
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="sender">イベント発行者</param>
        /// <param name="eventArgs">イベント引数</param>
        /// <param name="cancellationToken">キャンセレーショントークン</param>
        public async Task PublishAsync<TEventArgs>(object sender, TEventArgs eventArgs, CancellationToken cancellationToken = default) where TEventArgs : EventArgs
        {
            ThrowIfDisposed();

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var eventType = typeof(TEventArgs);
                if (_subscriptions.TryGetValue(eventType, out var handlers))
                {
                    var handlersToNotify = new List<Delegate>(handlers);
                    var tasks = new List<Task>();

                    foreach (var handler in handlersToNotify)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        if (handler is Action<object, TEventArgs> typedHandler)
                        {
                            // 各ハンドラを非同期で実行
                            tasks.Add(Task.Run(() => typedHandler(sender, eventArgs), cancellationToken));
                        }
                        else if (handler is Func<object, TEventArgs, Task> asyncHandler)
                        {
                            tasks.Add(asyncHandler(sender, eventArgs));
                        }
                    }

                    await Task.WhenAll(tasks);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// すべての購読を解除します
        /// </summary>
        public void ClearAllSubscriptions()
        {
            ThrowIfDisposed();

            _semaphore.Wait();
            try
            {
                _subscriptions.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EventManager));
            }
        }

        /// <summary>
        /// リソースを解放します
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _semaphore.Dispose();
            _subscriptions.Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// 購読解除に使用するトークン
    /// </summary>
    public class SubscriptionToken : IDisposable
    {
        private readonly EventManager _eventManager;
        private readonly Type _eventType;
        private readonly Delegate _handler;
        private bool _disposed;

        internal SubscriptionToken(EventManager eventManager, Type eventType, Delegate handler)
        {
            _eventManager = eventManager;
            _eventType = eventType;
            _handler = handler;
        }

        /// <summary>
        /// 購読を解除します
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // イベントマネージャーの動的メソッド呼び出しを行う
            var unsubscribeMethod = typeof(EventManager).GetMethod("Unsubscribe")
                .MakeGenericMethod(_eventType);

            unsubscribeMethod.Invoke(_eventManager, new[] { _handler });
            _disposed = true;
        }
    }

    /// <summary>
    /// イベント管理サービスの拡張メソッド
    /// </summary>
    public static class EventManagerExtensions
    {
        /// <summary>
        /// 非同期ハンドラでイベントを購読します
        /// </summary>
        public static SubscriptionToken SubscribeAsync<TEventArgs>(
            this EventManager eventManager,
            Func<object, TEventArgs, Task> handler) where TEventArgs : EventArgs
        {
            return eventManager.Subscribe<TEventArgs>(async (sender, args) =>
            {
                await handler(sender, args);
            });
        }
    }
}
