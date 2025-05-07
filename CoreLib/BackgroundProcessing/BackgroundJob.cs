using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.BackgroundProcessing
{
    /// <summary>
    /// ジョブの情報
    /// </summary>
    public class JobInfo
    {
        /// <summary>
        /// ジョブID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// ジョブ名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 実行間隔
        /// </summary>
        public TimeSpan Interval { get; }

        /// <summary>
        /// 初回実行の遅延
        /// </summary>
        public TimeSpan InitialDelay { get; }

        /// <summary>
        /// 次回実行予定時刻
        /// </summary>
        public DateTime NextRun { get; set; }

        /// <summary>
        /// 最終実行日時
        /// </summary>
        public DateTime? LastRun { get; set; }

        /// <summary>
        /// 実行回数
        /// </summary>
        public int ExecutionCount { get; set; }

        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// 状態
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// 最後のエラー
        /// </summary>
        public string? LastError { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JobInfo(string name, TimeSpan interval, TimeSpan initialDelay = default)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name;
            Interval = interval;
            InitialDelay = initialDelay;
            NextRun = DateTime.UtcNow.Add(initialDelay);
            Status = JobStatus.Scheduled;
        }
    }

    /// <summary>
    /// ジョブの状態
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// スケジュール済み
        /// </summary>
        Scheduled,

        /// <summary>
        /// 実行中
        /// </summary>
        Running,

        /// <summary>
        /// 一時停止
        /// </summary>
        Paused,

        /// <summary>
        /// 完了
        /// </summary>
        Completed,

        /// <summary>
        /// エラー
        /// </summary>
        Faulted,

        /// <summary>
        /// キャンセル
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// バックグラウンドジョブのインターフェース
    /// </summary>
    public interface IBackgroundJob
    {
        /// <summary>
        /// ジョブ名を取得
        /// </summary>
        string JobName { get; }

        /// <summary>
        /// 実行間隔を取得
        /// </summary>
        TimeSpan Interval { get; }

        /// <summary>
        /// 初期遅延を取得
        /// </summary>
        TimeSpan InitialDelay { get; }

        /// <summary>
        /// ジョブを実行
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// バックグラウンドジョブの基底クラス
    /// </summary>
    public abstract class BackgroundJob : IBackgroundJob
    {
        /// <summary>
        /// ジョブ名
        /// </summary>
        public virtual string JobName => GetType().Name;

        /// <summary>
        /// デフォルトの実行間隔（毎日）
        /// </summary>
        public virtual TimeSpan Interval => TimeSpan.FromDays(1);

        /// <summary>
        /// デフォルトの初期遅延
        /// </summary>
        public virtual TimeSpan InitialDelay => TimeSpan.Zero;

        /// <summary>
        /// ジョブを実行
        /// </summary>
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// ジョブスケジューラーサービスのインターフェース
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// ジョブを登録
        /// </summary>
        string ScheduleJob<TJob>(TimeSpan? interval = null, TimeSpan? initialDelay = null) where TJob : IBackgroundJob;

        /// <summary>
        /// ジョブを登録（アクション形式）
        /// </summary>
        string ScheduleJob(string name, Func<CancellationToken, Task> action, TimeSpan interval, TimeSpan? initialDelay = null);

        /// <summary>
        /// ジョブを一時停止
        /// </summary>
        bool PauseJob(string jobId);

        /// <summary>
        /// ジョブを再開
        /// </summary>
        bool ResumeJob(string jobId);

        /// <summary>
        /// ジョブをキャンセル
        /// </summary>
        bool CancelJob(string jobId);

        /// <summary>
        /// ジョブを即時実行
        /// </summary>
        Task<bool> TriggerJobNowAsync(string jobId);

        /// <summary>
        /// 全てのジョブ情報を取得
        /// </summary>
        IEnumerable<JobInfo> GetAllJobs();

        /// <summary>
        /// ジョブ情報を取得
        /// </summary>
        JobInfo? GetJobInfo(string jobId);
    }

    /// <summary>
    /// バックグラウンドジョブスケジューラのホステッドサービス実装
    /// </summary>
    public class BackgroundJobScheduler : BackgroundService, IJobScheduler
    {
        private readonly ConcurrentDictionary<string, JobInfo> _jobs = new();
        private readonly ConcurrentDictionary<string, IBackgroundJob> _jobInstances = new();
        private readonly ConcurrentDictionary<string, Func<CancellationToken, Task>> _jobActions = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundJobScheduler> _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BackgroundJobScheduler(IServiceProvider serviceProvider, ILogger<BackgroundJobScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// ジョブを登録
        /// </summary>
        public string ScheduleJob<TJob>(TimeSpan? interval = null, TimeSpan? initialDelay = null) where TJob : IBackgroundJob
        {
            // スコープからジョブを取得
            using var scope = _serviceProvider.CreateScope();
            var job = scope.ServiceProvider.GetRequiredService<TJob>();

            // カスタム設定またはデフォルト設定を使用
            var effectiveInterval = interval ?? job.Interval;
            var effectiveInitialDelay = initialDelay ?? job.InitialDelay;

            // ジョブ情報を作成
            var jobInfo = new JobInfo(job.JobName, effectiveInterval, effectiveInitialDelay);

            // コレクションに登録
            _jobs[jobInfo.Id] = jobInfo;
            _jobInstances[jobInfo.Id] = job;

            _logger.LogInformation($"ジョブをスケジュール: {job.JobName}, ID: {jobInfo.Id}, 間隔: {effectiveInterval}");

            return jobInfo.Id;
        }

        /// <summary>
        /// ジョブを登録（アクション形式）
        /// </summary>
        public string ScheduleJob(string name, Func<CancellationToken, Task> action, TimeSpan interval, TimeSpan? initialDelay = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("ジョブ名は必須です", nameof(name));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // ジョブ情報を作成
            var jobInfo = new JobInfo(name, interval, initialDelay ?? TimeSpan.Zero);

            // コレクションに登録
            _jobs[jobInfo.Id] = jobInfo;
            _jobActions[jobInfo.Id] = action;

            _logger.LogInformation($"アクションジョブをスケジュール: {name}, ID: {jobInfo.Id}, 間隔: {interval}");

            return jobInfo.Id;
        }

        /// <summary>
        /// ジョブを一時停止
        /// </summary>
        public bool PauseJob(string jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = JobStatus.Paused;
                _logger.LogInformation($"ジョブを一時停止: {job.Name}, ID: {jobId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジョブを再開
        /// </summary>
        public bool ResumeJob(string jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job) && job.Status == JobStatus.Paused)
            {
                job.Status = JobStatus.Scheduled;
                job.NextRun = DateTime.UtcNow;
                _logger.LogInformation($"ジョブを再開: {job.Name}, ID: {jobId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジョブをキャンセル
        /// </summary>
        public bool CancelJob(string jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = JobStatus.Cancelled;
                _logger.LogInformation($"ジョブをキャンセル: {job.Name}, ID: {jobId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジョブを即時実行
        /// </summary>
        public async Task<bool> TriggerJobNowAsync(string jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job) &&
                (job.Status == JobStatus.Scheduled || job.Status == JobStatus.Paused))
            {
                try
                {
                    _logger.LogInformation($"ジョブを手動実行: {job.Name}, ID: {jobId}");
                    await ExecuteJobAsync(jobId, job, CancellationToken.None);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ジョブの手動実行でエラー発生: {job.Name}, ID: {jobId}");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 全てのジョブ情報を取得
        /// </summary>
        public IEnumerable<JobInfo> GetAllJobs()
        {
            return _jobs.Values.ToList();
        }

        /// <summary>
        /// ジョブ情報を取得
        /// </summary>
        public JobInfo? GetJobInfo(string jobId)
        {
            return _jobs.TryGetValue(jobId, out var job) ? job : null;
        }

        /// <summary>
        /// バックグラウンドサービスの実行
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("バックグラウンドジョブスケジューラを開始しました。");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 現在時刻を取得
                var now = DateTime.UtcNow;

                // 実行すべきジョブをリストアップ
                var jobsToRun = _jobs.Where(j =>
                    j.Value.Status == JobStatus.Scheduled &&
                    !j.Value.IsRunning &&
                    j.Value.NextRun <= now)
                    .ToList();

                // 各ジョブを実行
                foreach (var jobEntry in jobsToRun)
                {
                    var jobId = jobEntry.Key;
                    var jobInfo = jobEntry.Value;

                    // 別タスクでジョブを実行（メインループをブロックしない）
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ExecuteJobAsync(jobId, jobInfo, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"ジョブ実行中にエラーが発生: {jobInfo.Name}, ID: {jobId}");
                        }
                    }, stoppingToken);
                }

                // 1秒間隔でチェック
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            _logger.LogInformation("バックグラウンドジョブスケジューラが停止しました。");
        }

        private async Task ExecuteJobAsync(string jobId, JobInfo jobInfo, CancellationToken cancellationToken)
        {
            // ジョブが既に実行中の場合はスキップ
            if (jobInfo.IsRunning)
                return;

            jobInfo.IsRunning = true;
            jobInfo.Status = JobStatus.Running;

            try
            {
                _logger.LogInformation($"ジョブを実行開始: {jobInfo.Name}, ID: {jobId}");

                // 実行するジョブを特定
                if (_jobInstances.TryGetValue(jobId, out var jobInstance))
                {
                    // DIスコープを作成して実行
                    using var scope = _serviceProvider.CreateScope();

                    // 具象ジョブタイプを取得して新しいインスタンスを作成
                    var jobType = jobInstance.GetType();
                    var job = (IBackgroundJob)scope.ServiceProvider.GetService(jobType)
                        ?? throw new InvalidOperationException($"ジョブ型 {jobType.Name} が登録されていません");

                    await job.ExecuteAsync(cancellationToken);
                }
                else if (_jobActions.TryGetValue(jobId, out var action))
                {
                    // アクションを実行
                    await action(cancellationToken);
                }

                // ジョブの実行情報を更新
                jobInfo.LastRun = DateTime.UtcNow;
                jobInfo.ExecutionCount++;
                jobInfo.NextRun = DateTime.UtcNow.Add(jobInfo.Interval);
                jobInfo.Status = JobStatus.Scheduled;
                jobInfo.LastError = null;

                _logger.LogInformation($"ジョブを実行完了: {jobInfo.Name}, ID: {jobId}, 次回実行: {jobInfo.NextRun}");
            }
            catch (Exception ex)
            {
                // エラー情報を記録
                jobInfo.Status = JobStatus.Faulted;
                jobInfo.LastError = ex.Message;
                jobInfo.NextRun = DateTime.UtcNow.Add(jobInfo.Interval);

                _logger.LogError(ex, $"ジョブ実行でエラー発生: {jobInfo.Name}, ID: {jobId}");
            }
            finally
            {
                jobInfo.IsRunning = false;
            }
        }
    }

    /// <summary>
    /// DIサービス登録用拡張メソッド
    /// </summary>
    public static class BackgroundJobServiceExtensions
    {
        /// <summary>
        /// バックグラウンドジョブサービスを登録
        /// </summary>
        public static IServiceCollection AddBackgroundJobScheduler(this IServiceCollection services)
        {
            // スケジューラーを登録
            services.AddSingleton<IJobScheduler, BackgroundJobScheduler>();
            services.AddHostedService(provider => (BackgroundJobScheduler)provider.GetRequiredService<IJobScheduler>());

            return services;
        }

        /// <summary>
        /// バックグラウンドジョブを登録
        /// </summary>
        public static IServiceCollection AddBackgroundJob<TJob>(this IServiceCollection services)
            where TJob : class, IBackgroundJob
        {
            // ジョブを登録
            services.AddTransient<TJob>();

            return services;
        }
    }
}
