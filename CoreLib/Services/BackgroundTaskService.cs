using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    public interface IBackgroundTaskService
    {
        Task<Guid> QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);
        Task<bool> CancelBackgroundWorkItemAsync(Guid taskId);
        Task<IEnumerable<BackgroundTaskInfo>> GetRunningTasksAsync();
    }

    public class BackgroundTaskInfo
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration => DateTime.Now - StartTime;
        public string Status { get; set; }
    }

    public class BackgroundTaskService : IBackgroundTaskService, IDisposable
    {
        private readonly ILogger<BackgroundTaskService> _logger;
        private readonly Dictionary<Guid, (CancellationTokenSource TokenSource, Task Task, BackgroundTaskInfo Info)> _tasks = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public BackgroundTaskService(ILogger<BackgroundTaskService> logger)
        {
            _logger = logger;
        }

        public async Task<Guid> QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem)
        {
            await _semaphore.WaitAsync();
            try
            {
                var taskId = Guid.NewGuid();
                var cts = new CancellationTokenSource();

                var taskInfo = new BackgroundTaskInfo
                {
                    Id = taskId,
                    StartTime = DateTime.Now,
                    Status = "Running"
                };

                async Task WrappedTask()
                {
                    try
                    {
                        _logger.LogInformation("バックグラウンドタスク {TaskId} を開始しました", taskId);
                        await workItem(cts.Token);
                        taskInfo.Status = "Completed";
                        _logger.LogInformation("バックグラウンドタスク {TaskId} が完了しました", taskId);
                    }
                    catch (OperationCanceledException)
                    {
                        taskInfo.Status = "Cancelled";
                        _logger.LogInformation("バックグラウンドタスク {TaskId} がキャンセルされました", taskId);
                    }
                    catch (Exception ex)
                    {
                        taskInfo.Status = "Failed";
                        _logger.LogError(ex, "バックグラウンドタスク {TaskId} で例外が発生しました", taskId);
                    }
                    finally
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            _tasks.Remove(taskId);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }
                }

                var task = Task.Run(WrappedTask);
                _tasks.Add(taskId, (cts, task, taskInfo));

                return taskId;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<bool> CancelBackgroundWorkItemAsync(Guid taskId)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_tasks.TryGetValue(taskId, out var taskInfo))
                {
                    _logger.LogInformation("バックグラウンドタスク {TaskId} のキャンセルをリクエストしました", taskId);
                    taskInfo.TokenSource.Cancel();
                    return true;
                }
                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<BackgroundTaskInfo>> GetRunningTasksAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                return _tasks.Values.Select(t => t.Info).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            foreach (var (tokenSource, _, _) in _tasks.Values)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }

            _semaphore.Dispose();
        }
    }
}
