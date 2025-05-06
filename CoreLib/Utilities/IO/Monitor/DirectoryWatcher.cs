using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Monitor
{
    /// <summary>
    /// ディレクトリ構造変更監視クラス
    /// </summary>
    public class DirectoryWatcher : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, DateTime> _lastEventTime = new Dictionary<string, DateTime>();
        private readonly TimeSpan _debounceTime;
        private bool _disposed;

        /// <summary>
        /// ディレクトリ作成イベント
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? DirectoryCreated;

        /// <summary>
        /// ディレクトリ削除イベント
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? DirectoryDeleted;

        /// <summary>
        /// ディレクトリ名変更イベント
        /// </summary>
        public event EventHandler<RenamedEventArgs>? DirectoryRenamed;

        /// <summary>
        /// DirectoryWatcherコンストラクタ
        /// </summary>
        /// <param name="path">監視対象のルートディレクトリパス</param>
        /// <param name="includeSubdirectories">サブディレクトリも監視するかどうか</param>
        /// <param name="debounceMilliseconds">イベントのデバウンス時間（ミリ秒）</param>
        public DirectoryWatcher(
            string path,
            bool includeSubdirectories = true,
            int debounceMilliseconds = 300)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"指定されたディレクトリが見つかりません: {path}");

            _watcher = new FileSystemWatcher
            {
                Path = path,
                Filter = "*", // すべてのディレクトリを監視
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.CreationTime,
                IncludeSubdirectories = includeSubdirectories,
                EnableRaisingEvents = false
            };

            _debounceTime = TimeSpan.FromMilliseconds(debounceMilliseconds);

            // イベントハンドラの登録
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;
        }

        /// <summary>
        /// 監視を開始
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 監視を停止
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();
            _watcher.EnableRaisingEvents = false;
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            // ディレクトリかどうかを確認
            if (IsDirectory(e.FullPath))
            {
                await ProcessEventAsync(e, DirectoryCreated);
            }
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            // 削除されたものはDirectory.Existsでチェックできないので、
            // ファイル拡張子がない場合はディレクトリと仮定
            if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath)))
            {
                await ProcessEventAsync(e, DirectoryDeleted);
            }
        }

        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            // ディレクトリかどうかを確認
            if (IsDirectory(e.FullPath))
            {
                await _semaphore.WaitAsync();
                try
                {
                    var key = $"{e.ChangeType}_{e.FullPath}";
                    var now = DateTime.Now;

                    if (_lastEventTime.TryGetValue(key, out var lastTime) && (now - lastTime) < _debounceTime)
                        return;

                    _lastEventTime[key] = now;

                    DirectoryRenamed?.Invoke(this, e);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            // エラー処理
            Console.WriteLine($"DirectoryWatcher error: {e.GetException().Message}");
        }

        private async Task ProcessEventAsync(FileSystemEventArgs e, EventHandler<FileSystemEventArgs>? eventHandler)
        {
            if (eventHandler == null)
                return;

            await _semaphore.WaitAsync();
            try
            {
                var key = $"{e.ChangeType}_{e.FullPath}";
                var now = DateTime.Now;

                if (_lastEventTime.TryGetValue(key, out var lastTime) && (now - lastTime) < _debounceTime)
                    return;

                _lastEventTime[key] = now;

                eventHandler.Invoke(this, e);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 指定されたパスがディレクトリかどうかを確認
        /// </summary>
        private bool IsDirectory(string path)
        {
            try
            {
                return Directory.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DirectoryWatcher));
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _watcher.Created -= OnCreated;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed -= OnRenamed;
            _watcher.Error -= OnError;
            _watcher.Dispose();
            _semaphore.Dispose();

            _disposed = true;
        }
    }
}
