using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Monitor
{
    /// <summary>
    /// ファイル変更監視クラス
    /// </summary>
    public class FileWatcher : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, DateTime> _lastEventTime = new Dictionary<string, DateTime>();
        private readonly TimeSpan _debounceTime;

        /// <summary>
        /// ファイル作成イベント
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? FileCreated;

        /// <summary>
        /// ファイル変更イベント
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? FileChanged;

        /// <summary>
        /// ファイル削除イベント
        /// </summary>
        public event EventHandler<FileSystemEventArgs>? FileDeleted;

        /// <summary>
        /// ファイル名変更イベント
        /// </summary>
        public event EventHandler<RenamedEventArgs>? FileRenamed;

        /// <summary>
        /// FileWatcherコンストラクタ
        /// </summary>
        public FileWatcher(
            string path,
            string filter = "*.*",
            bool includeSubdirectories = false,
            int debounceMilliseconds = 300)
        {
            _watcher = new FileSystemWatcher
            {
                Path = path,
                Filter = filter,
                NotifyFilter = NotifyFilters.FileName
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Size,
                IncludeSubdirectories = includeSubdirectories,
                EnableRaisingEvents = false
            };

            _debounceTime = TimeSpan.FromMilliseconds(debounceMilliseconds);

            // イベントハンドラの登録
            _watcher.Created += OnCreated;
            _watcher.Changed += OnChanged;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;
        }

        /// <summary>
        /// 監視を開始
        /// </summary>
        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 監視を停止
        /// </summary>
        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            await ProcessEventAsync(e, FileCreated);
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            await ProcessEventAsync(e, FileChanged);
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            await ProcessEventAsync(e, FileDeleted);
        }

        private async void OnRenamed(object sender, RenamedEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                var key = $"{e.ChangeType}_{e.FullPath}";
                var now = DateTime.Now;

                if (_lastEventTime.TryGetValue(key, out var lastTime) && (now - lastTime) < _debounceTime)
                    return;

                _lastEventTime[key] = now;

                FileRenamed?.Invoke(this, e);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            // エラー処理
            Console.WriteLine($"FileWatcher error: {e.GetException().Message}");
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
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed -= OnRenamed;
            _watcher.Error -= OnError;
            _watcher.Dispose();
            _semaphore.Dispose();
        }
    }
}
