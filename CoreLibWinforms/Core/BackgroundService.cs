using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    public class BackgroundService : IDisposable
    {
        private readonly System.Threading.Timer _timer;
        private readonly TimeSpan _interval;
        private bool _isRunning;

        // 処理完了時のイベント
        public event EventHandler<ProcessCompletedEventArgs> ProcessCompleted;

        public BackgroundService(TimeSpan interval)
        {
            _interval = interval;
            _timer = new System.Threading.Timer(ExecuteTask, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            _timer.Change(TimeSpan.Zero, _interval);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void ExecuteTask(object state)
        {
            if (_isRunning)
                return;

            _isRunning = true;

            try
            {
                // ここに実行したいバックグラウンド処理を実装
                var result = await PerformBackgroundTaskAsync();

                // 処理完了イベントを発行
                OnProcessCompleted(new ProcessCompletedEventArgs(result));
            }
            catch (Exception ex)
            {
                // エラーハンドリング
                OnProcessCompleted(new ProcessCompletedEventArgs(ex));
            }
            finally
            {
                _isRunning = false;
            }
        }

        private Task<string> PerformBackgroundTaskAsync()
        {
            // 実際の処理を実装
            return Task.FromResult($"処理が完了しました - {DateTime.Now}");
        }

        protected virtual void OnProcessCompleted(ProcessCompletedEventArgs e)
        {
            ProcessCompleted?.Invoke(this, e);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

    public class ProcessCompletedEventArgs : EventArgs
    {
        public string Result { get; }
        public Exception Error { get; }
        public bool IsSuccess => Error == null;

        public ProcessCompletedEventArgs(string result)
        {
            Result = result;
        }

        public ProcessCompletedEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
