using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Scheduling
{
    /// <summary>
    /// スケジュールされたジョブのインターフェース
    /// </summary>
    public interface IScheduledJob
    {
        /// <summary>
        /// ジョブのID
        /// </summary>
        string JobId { get; }

        /// <summary>
        /// ジョブの説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// ジョブの最終実行日時
        /// </summary>
        DateTime? LastRunTime { get; }

        /// <summary>
        /// 次回実行予定日時
        /// </summary>
        DateTime NextRunTime { get; }

        /// <summary>
        /// 現在実行中かどうか
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// ジョブの実行
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        DateTime CalculateNextRunTime(DateTime? fromTime = null);
    }

    /// <summary>
    /// スケジュールトリガーのインターフェース
    /// </summary>
    public interface IScheduleTrigger
    {
        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        DateTime GetNextRunTime(DateTime? lastRunTime = null);

        /// <summary>
        /// トリガーの説明文
        /// </summary>
        string Description { get; }
    }

    /// <summary>
    /// 間隔トリガー
    /// </summary>
    public class IntervalTrigger : IScheduleTrigger
    {
        private readonly TimeSpan _interval;
        private readonly DateTime _startTime;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="interval">実行間隔</param>
        /// <param name="startTime">開始時間</param>
        public IntervalTrigger(TimeSpan interval, DateTime? startTime = null)
        {
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("間隔は0より大きい必要があります", nameof(interval));

            _interval = interval;
            _startTime = startTime ?? DateTime.Now;
        }

        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        public DateTime GetNextRunTime(DateTime? lastRunTime = null)
        {
            var referenceTime = lastRunTime ?? _startTime;
            var nextRun = referenceTime.Add(_interval);

            // 過去の時間になった場合は現在時刻から再計算
            if (nextRun < DateTime.Now)
            {
                var elapsedIntervals = (long)Math.Ceiling((DateTime.Now - referenceTime).TotalMilliseconds / _interval.TotalMilliseconds);
                nextRun = referenceTime.AddMilliseconds(elapsedIntervals * _interval.TotalMilliseconds);
            }

            return nextRun;
        }

        /// <summary>
        /// トリガーの説明文
        /// </summary>
        public string Description => $"間隔: {_interval.TotalSeconds}秒";
    }

    /// <summary>
    /// 毎日指定時刻に実行するトリガー
    /// </summary>
    public class DailyTrigger : IScheduleTrigger
    {
        private readonly TimeSpan _timeOfDay;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hour">時</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException(nameof(hour), "時は0から23の間である必要があります");
            if (minute < 0 || minute > 59)
                throw new ArgumentOutOfRangeException(nameof(minute), "分は0から59の間である必要があります");
            if (second < 0 || second > 59)
                throw new ArgumentOutOfRangeException(nameof(second), "秒は0から59の間である必要があります");

            _timeOfDay = new TimeSpan(hour, minute, second);
        }

        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        public DateTime GetNextRunTime(DateTime? lastRunTime = null)
        {
            var now = DateTime.Now;
            var today = now.Date.Add(_timeOfDay);

            // 今日の実行時間がまだ来ていない場合
            if (today > now)
                return today;

            // 今日の実行時間を過ぎている場合は明日の実行時間を返す
            return today.AddDays(1);
        }

        /// <summary>
        /// トリガーの説明文
        /// </summary>
        public string Description => $"毎日: {_timeOfDay.Hours:D2}:{_timeOfDay.Minutes:D2}:{_timeOfDay.Seconds:D2}";
    }

    /// <summary>
    /// 曜日指定トリガー
    /// </summary>
    public class WeeklyTrigger : IScheduleTrigger
    {
        private readonly HashSet<DayOfWeek> _daysOfWeek;
        private readonly TimeSpan _timeOfDay;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="daysOfWeek">実行する曜日</param>
        /// <param name="hour">時</param>
        /// <param name="minute">分</param>
        /// <param name="second">秒</param>
        public WeeklyTrigger(IEnumerable<DayOfWeek> daysOfWeek, int hour, int minute = 0, int second = 0)
        {
            _daysOfWeek = new HashSet<DayOfWeek>(daysOfWeek);
            if (_daysOfWeek.Count == 0)
                throw new ArgumentException("少なくとも1つの曜日を指定する必要があります", nameof(daysOfWeek));

            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException(nameof(hour), "時は0から23の間である必要があります");
            if (minute < 0 || minute > 59)
                throw new ArgumentOutOfRangeException(nameof(minute), "分は0から59の間である必要があります");
            if (second < 0 || second > 59)
                throw new ArgumentOutOfRangeException(nameof(second), "秒は0から59の間である必要があります");

            _timeOfDay = new TimeSpan(hour, minute, second);
        }

        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        public DateTime GetNextRunTime(DateTime? lastRunTime = null)
        {
            var now = DateTime.Now;
            var today = now.Date.Add(_timeOfDay);

            // 今日の実行時間かつ、今日が指定された曜日の場合
            if (today > now && _daysOfWeek.Contains(today.DayOfWeek))
                return today;

            // 次の実行曜日を探す
            for (int i = 1; i <= 7; i++)
            {
                var nextDate = now.Date.AddDays(i);
                if (_daysOfWeek.Contains(nextDate.DayOfWeek))
                {
                    return nextDate.Add(_timeOfDay);
                }
            }

            // ここには到達しないはず
            throw new InvalidOperationException("次の実行時間の計算に失敗しました");
        }

        /// <summary>
        /// トリガーの説明文
        /// </summary>
        public string Description
        {
            get
            {
                var days = string.Join(", ", _daysOfWeek.Select(d => d.ToString()));
                return $"毎週 {days}, {_timeOfDay.Hours:D2}:{_timeOfDay.Minutes:D2}:{_timeOfDay.Seconds:D2}";
            }
        }
    }

    /// <summary>
    /// スケジュールされたジョブの基底クラス
    /// </summary>
    public abstract class ScheduledJob : IScheduledJob
    {
        /// <summary>
        /// ジョブのID
        /// </summary>
        public string JobId { get; }

        /// <summary>
        /// ジョブの説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// ジョブの最終実行日時
        /// </summary>
        public DateTime? LastRunTime { get; protected set; }

        /// <summary>
        /// 次回実行予定日時
        /// </summary>
        public DateTime NextRunTime { get; protected set; }

        /// <summary>
        /// 現在実行中かどうか
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// ジョブのスケジュールトリガー
        /// </summary>
        protected readonly IScheduleTrigger Trigger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="jobId">ジョブID</param>
        /// <param name="description">ジョブの説明</param>
        /// <param name="trigger">トリガー</param>
        protected ScheduledJob(string jobId, string description, IScheduleTrigger trigger)
        {
            JobId = jobId ?? throw new ArgumentNullException(nameof(jobId));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            NextRunTime = trigger.GetNextRunTime();
        }

        /// <summary>
        /// ジョブを実行
        /// </summary>
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (IsRunning)
                return;

            IsRunning = true;
            try
            {
                await ExecuteJobAsync(cancellationToken);
                LastRunTime = DateTime.Now;
            }
            finally
            {
                IsRunning = false;
                NextRunTime = CalculateNextRunTime();
            }
        }

        /// <summary>
        /// 実際のジョブの処理（派生クラスで実装）
        /// </summary>
        protected abstract Task ExecuteJobAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 次回実行時間の計算
        /// </summary>
        public DateTime CalculateNextRunTime(DateTime? fromTime = null)
        {
            return Trigger.GetNextRunTime(fromTime ?? LastRunTime);
        }
    }

    /// <summary>
    /// ジョブスケジューラーサービス
    /// </summary>
    public class JobScheduler : IDisposable
    {
        private readonly ConcurrentDictionary<string, IScheduledJob> _jobs = new();
        private readonly CancellationTokenSource _globalCancellation = new();
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;
        private readonly object _syncLock = new();
        private bool _isRunning;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JobScheduler(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _timer = new Timer(CheckSchedule, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// ジョブを登録
        /// </summary>
        public void RegisterJob(IScheduledJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (_jobs.TryAdd(job.JobId, job))
            {
                _logger.LogInformation($"ジョブを登録: {job.JobId} - {job.Description}, 次回実行時間: {job.NextRunTime}");
            }
            else
            {
                _logger.LogWarning($"ジョブ {job.JobId} は既に登録されています");
            }
        }

        /// <summary>
        /// 登録済みジョブの取得
        /// </summary>
        public IScheduledJob? GetJob(string jobId)
        {
            return _jobs.TryGetValue(jobId, out var job) ? job : null;
        }

        /// <summary>
        /// ジョブの登録解除
        /// </summary>
        public bool UnregisterJob(string jobId)
        {
            if (_jobs.TryRemove(jobId, out _))
            {
                _logger.LogInformation($"ジョブを登録解除: {jobId}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// ジョブスケジューラーを開始
        /// </summary>
        public void Start()
        {
            lock (_syncLock)
            {
                if (_isRunning)
                    return;

                _logger.LogInformation("ジョブスケジューラーを開始");

                _timer?.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
                _isRunning = true;
            }
        }

        /// <summary>
        /// ジョブスケジューラーを停止
        /// </summary>
        public void Stop()
        {
            lock (_syncLock)
            {
                if (!_isRunning)
                    return;

                _logger.LogInformation("ジョブスケジューラーを停止");

                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                _isRunning = false;
            }
        }

        /// <summary>
        /// すべてのジョブを即時実行
        /// </summary>
        public async Task RunAllJobsAsync()
        {
            foreach (var job in _jobs.Values)
            {
                await RunJobAsync(job);
            }
        }

        /// <summary>
        /// 指定したジョブを即時実行
        /// </summary>
        public async Task<bool> RunJobAsync(string jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                await RunJobAsync(job);
                return true;
            }
            return false;
        }

        private async Task RunJobAsync(IScheduledJob job)
        {
            if (job.IsRunning)
            {
                _logger.LogWarning($"ジョブ {job.JobId} は既に実行中です");
                return;
            }

            try
            {
                _logger.LogInformation($"ジョブを実行: {job.JobId} - {job.Description}");

                using var scope = _serviceProvider.CreateScope();
                await job.ExecuteAsync(_globalCancellation.Token);

                _logger.LogInformation($"ジョブ {job.JobId} 実行完了, 次回実行時間: {job.NextRunTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ジョブ {job.JobId} 実行エラー: {ex.Message}");
            }
        }

        private void CheckSchedule(object? state)
        {
            var now = DateTime.Now;

            // 実行すべきジョブを確認
            foreach (var job in _jobs.Values)
            {
                if (!job.IsRunning && job.NextRunTime <= now)
                {
                    // 非同期で実行を開始（結果を待機しない）
                    _ = RunJobAsync(job);
                }
            }
        }

        public void Dispose()
        {
            Stop();
            _globalCancellation.Cancel();
            _globalCancellation.Dispose();
            _timer?.Dispose();
            _timer = null;
        }
    }

    /// <summary>
    /// DIサービス登録用拡張メソッド
    /// </summary>
    public static class SchedulingServiceExtensions
    {
        /// <summary>
        /// ジョブスケジューラーサービスを登録
        /// </summary>
        public static IServiceCollection AddJobScheduler(this IServiceCollection services)
        {
            services.AddSingleton<JobScheduler>();
            return services;
        }
    }
}
