using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    /// <summary>
    /// ヘルスモニタリングサービスインターフェース
    /// </summary>
    public interface IHealthMonitorService
    {
        Task StartMonitoringAsync(int intervalMilliseconds = 5000);
        Task StopMonitoringAsync();
        ResourceUsage GetCurrentResourceUsage();
        IReadOnlyList<HealthWarning> GetActiveWarnings();
        Task<IReadOnlyList<HealthCheck>> RunHealthChecksAsync();
        Task<PerformanceReport> GeneratePerformanceReportAsync(DateTime? startTime = null, DateTime? endTime = null);
        bool IsMonitoring { get; }
        event EventHandler<HealthWarningEventArgs> WarningOccurred;
        event EventHandler<HealthWarningEventArgs> WarningResolved;
        event EventHandler<ResourceUsageEventArgs> ResourceUsageUpdated;
    }

    /// <summary>
    /// リソース使用状況クラス
    /// </summary>
    public class ResourceUsage
    {
        public float CpuUsagePercent { get; set; }
        public long MemoryUsageBytes { get; set; }
        public long PrivateMemoryBytes { get; set; }
        public int ThreadCount { get; set; }
        public long GcTotalMemory { get; set; }
        public int Gen0Collections { get; set; }
        public int Gen1Collections { get; set; }
        public int Gen2Collections { get; set; }
        public TimeSpan ProcessUptime { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public string MemoryUsageFormatted => FormatBytes(MemoryUsageBytes);
        public string PrivateMemoryFormatted => FormatBytes(PrivateMemoryBytes);
        public string GcTotalMemoryFormatted => FormatBytes(GcTotalMemory);

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n2} {suffixes[counter]}";
        }
    }

    /// <summary>
    /// 警告レベル
    /// </summary>
    public enum HealthWarningLevel
    {
        Information,
        Warning,
        Critical
    }

    /// <summary>
    /// ヘルスチェック状態
    /// </summary>
    public enum HealthCheckStatus
    {
        Passed,
        Warning,
        Failed,
        Skipped
    }

    /// <summary>
    /// ヘルス警告クラス
    /// </summary>
    public class HealthWarning
    {
        public string Id { get; set; }
        public HealthWarningLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime OccurredAt { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }

    /// <summary>
    /// ヘルスチェッククラス
    /// </summary>
    public class HealthCheck
    {
        public string Name { get; set; }
        public HealthCheckStatus Status { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string Component { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }

    /// <summary>
    /// パフォーマンスレポートクラス
    /// </summary>
    public class PerformanceReport
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public List<ResourceUsage> ResourceUsageSamples { get; set; } = new();
        public List<HealthWarning> Warnings { get; set; } = new();
        public ResourceUsage PeakResourceUsage { get; set; }
        public ResourceUsage AverageResourceUsage { get; set; }
        public int TotalWarningsCount => Warnings.Count;
        public int CriticalWarningsCount => Warnings.Count(w => w.Level == HealthWarningLevel.Critical);
    }

    /// <summary>
    /// ヘルス警告イベント引数
    /// </summary>
    public class HealthWarningEventArgs : EventArgs
    {
        public HealthWarning Warning { get; }

        public HealthWarningEventArgs(HealthWarning warning)
        {
            Warning = warning;
        }
    }

    /// <summary>
    /// リソース使用状況イベント引数
    /// </summary>
    public class ResourceUsageEventArgs : EventArgs
    {
        public ResourceUsage Usage { get; }

        public ResourceUsageEventArgs(ResourceUsage usage)
        {
            Usage = usage;
        }
    }

    /// <summary>
    /// ヘルスモニタリングサービス
    /// </summary>
    public class HealthMonitorService : IHealthMonitorService, IDisposable
    {
        private readonly ILogger<HealthMonitorService> _logger;
        private readonly ISettingsService _settingsService;
        private readonly List<HealthWarning> _activeWarnings = new();
        private readonly List<ResourceUsage> _resourceHistory = new();
        private readonly List<Func<Task<HealthCheck>>> _healthChecks = new();
        private readonly object _lockObject = new();
        private readonly Process _currentProcess;
        private readonly PerformanceCounter _cpuCounter;
        private CancellationTokenSource _monitoringCts;
        private Task _monitoringTask;
        private readonly int _historyLimit = 1000; // 履歴の最大保持数
        private readonly string _performanceLogPath;
        private DateTime _startTime;
        private int _lastGen0Count = 0;
        private int _lastGen1Count = 0;
        private int _lastGen2Count = 0;

        public event EventHandler<HealthWarningEventArgs> WarningOccurred;
        public event EventHandler<HealthWarningEventArgs> WarningResolved;
        public event EventHandler<ResourceUsageEventArgs> ResourceUsageUpdated;

        public bool IsMonitoring => _monitoringTask != null && !_monitoringTask.IsCompleted;

        public HealthMonitorService(ILogger<HealthMonitorService> logger, ISettingsService settingsService = null)
        {
            _logger = logger;
            _settingsService = settingsService;
            _currentProcess = Process.GetCurrentProcess();
            _startTime = _currentProcess.StartTime;

            // パフォーマンスログのパスを設定
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "YourApp",
                "Logs");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _performanceLogPath = Path.Combine(appDataPath, "performance.log");

            try
            {
                // CPU使用率カウンターの初期化
                _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _currentProcess.ProcessName);
                _cpuCounter.NextValue(); // 最初の呼び出しは常に0を返すので捨てる
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CPU使用率カウンターの初期化に失敗しました");
                _cpuCounter = null;
            }

            // デフォルトのヘルスチェックを登録
            RegisterDefaultHealthChecks();

            // 設定から監視間隔を取得
            if (_settingsService != null)
            {
                Task.Run(async () =>
                {
                    _historyLimit = await _settingsService.GetSettingAsync("HealthMonitor_HistoryLimit", 1000);
                    var autoStart = await _settingsService.GetSettingAsync("HealthMonitor_AutoStart", false);
                    var interval = await _settingsService.GetSettingAsync("HealthMonitor_Interval", 5000);

                    if (autoStart)
                    {
                        await StartMonitoringAsync(interval);
                    }
                }).Wait();
            }
        }

        /// <summary>
        /// モニタリングを開始する
        /// </summary>
        public async Task StartMonitoringAsync(int intervalMilliseconds = 5000)
        {
            if (IsMonitoring)
            {
                _logger.LogWarning("モニタリングは既に実行中です");
                return;
            }

            _monitoringCts = new CancellationTokenSource();
            var token = _monitoringCts.Token;

            _monitoringTask = Task.Run(async () =>
            {
                _logger.LogInformation("リソース使用状況のモニタリングを開始します（間隔: {Interval}ms）", intervalMilliseconds);

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var usage = CollectResourceUsage();

                        // リソース使用状況を履歴に追加
                        lock (_lockObject)
                        {
                            _resourceHistory.Add(usage);
                            if (_resourceHistory.Count > _historyLimit)
                            {
                                _resourceHistory.RemoveAt(0);
                            }
                        }

                        // イベント発行
                        OnResourceUsageUpdated(new ResourceUsageEventArgs(usage));

                        // 閾値チェック
                        CheckResourceThresholds(usage);

                        // パフォーマンスログに記録
                        await LogPerformanceDataAsync(usage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "リソース使用状況の収集中にエラーが発生しました");
                    }

                    await Task.Delay(intervalMilliseconds, token);
                }
            }, token);

            // 設定を保存
            if (_settingsService != null)
            {
                await _settingsService.SaveSettingAsync("HealthMonitor_Interval", intervalMilliseconds);
            }
        }

        /// <summary>
        /// モニタリングを停止する
        /// </summary>
        public async Task StopMonitoringAsync()
        {
            if (!IsMonitoring)
            {
                return;
            }

            try
            {
                _monitoringCts?.Cancel();
                await _monitoringTask;
                _logger.LogInformation("リソース使用状況のモニタリングを停止しました");
            }
            catch (OperationCanceledException)
            {
                // 正常なキャンセル
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "モニタリングの停止中にエラーが発生しました");
            }
            finally
            {
                _monitoringCts?.Dispose();
                _monitoringCts = null;
                _monitoringTask = null;
            }
        }

        /// <summary>
        /// 現在のリソース使用状況を取得する
        /// </summary>
        public ResourceUsage GetCurrentResourceUsage()
        {
            return CollectResourceUsage();
        }

        /// <summary>
        /// アクティブな警告をすべて取得する
        /// </summary>
        public IReadOnlyList<HealthWarning> GetActiveWarnings()
        {
            lock (_lockObject)
            {
                return _activeWarnings.ToList();
            }
        }

        /// <summary>
        /// すべてのヘルスチェックを実行する
        /// </summary>
        public async Task<IReadOnlyList<HealthCheck>> RunHealthChecksAsync()
        {
            var results = new List<HealthCheck>();
            _logger.LogInformation("ヘルスチェックを実行しています...");

            foreach (var check in _healthChecks)
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var result = await check();
                    stopwatch.Stop();

                    result.Duration = stopwatch.Elapsed;
                    results.Add(result);

                    _logger.LogDebug("ヘルスチェック '{Name}' の結果: {Status}, 所要時間: {Duration}ms",
                        result.Name, result.Status, result.Duration.TotalMilliseconds);

                    // 失敗したチェックに対して警告を発生
                    if (result.Status == HealthCheckStatus.Failed)
                    {
                        var warningId = $"HealthCheck_{result.Name}";
                        var warning = new HealthWarning
                        {
                            Id = warningId,
                            Level = HealthWarningLevel.Warning,
                            Message = $"ヘルスチェック '{result.Name}' に失敗しました: {result.Description}",
                            OccurredAt = DateTime.Now,
                            Source = "HealthCheck",
                            Data = result.Data
                        };

                        AddWarning(warning);
                    }
                    else
                    {
                        // 成功した場合は関連する警告を解決
                        var warningId = $"HealthCheck_{result.Name}";
                        ResolveWarning(warningId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ヘルスチェックの実行中にエラーが発生しました");
                    results.Add(new HealthCheck
                    {
                        Name = "UnknownCheck",
                        Status = HealthCheckStatus.Failed,
                        Description = $"チェックの実行中に例外が発生しました: {ex.Message}",
                        Duration = TimeSpan.Zero,
                        Component = "HealthMonitor",
                        Data = new Dictionary<string, object>
                        {
                            ["Exception"] = ex.ToString()
                        }
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// パフォーマンスレポートを生成する
        /// </summary>
        public async Task<PerformanceReport> GeneratePerformanceReportAsync(DateTime? startTime = null, DateTime? endTime = null)
        {
            var now = DateTime.Now;
            var reportStartTime = startTime ?? now.AddHours(-1);
            var reportEndTime = endTime ?? now;

            List<ResourceUsage> samples;
            List<HealthWarning> warnings;

            lock (_lockObject)
            {
                samples = _resourceHistory
                    .Where(u => u.Timestamp >= reportStartTime && u.Timestamp <= reportEndTime)
                    .ToList();

                warnings = _activeWarnings
                    .Where(w => w.OccurredAt >= reportStartTime && w.OccurredAt <= reportEndTime)
                    .ToList();
            }

            // ピークと平均のリソース使用状況を計算
            var peakUsage = new ResourceUsage
            {
                CpuUsagePercent = samples.Count > 0 ? samples.Max(s => s.CpuUsagePercent) : 0,
                MemoryUsageBytes = samples.Count > 0 ? samples.Max(s => s.MemoryUsageBytes) : 0,
                PrivateMemoryBytes = samples.Count > 0 ? samples.Max(s => s.PrivateMemoryBytes) : 0,
                ThreadCount = samples.Count > 0 ? samples.Max(s => s.ThreadCount) : 0,
                GcTotalMemory = samples.Count > 0 ? samples.Max(s => s.GcTotalMemory) : 0,
                Gen0Collections = samples.Count > 0 ? samples.Max(s => s.Gen0Collections) : 0,
                Gen1Collections = samples.Count > 0 ? samples.Max(s => s.Gen1Collections) : 0,
                Gen2Collections = samples.Count > 0 ? samples.Max(s => s.Gen2Collections) : 0,
                Timestamp = now
            };

            var avgUsage = new ResourceUsage
            {
                CpuUsagePercent = samples.Count > 0 ? samples.Average(s => s.CpuUsagePercent) : 0,
                MemoryUsageBytes = samples.Count > 0 ? (long)samples.Average(s => s.MemoryUsageBytes) : 0,
                PrivateMemoryBytes = samples.Count > 0 ? (long)samples.Average(s => s.PrivateMemoryBytes) : 0,
                ThreadCount = samples.Count > 0 ? (int)samples.Average(s => s.ThreadCount) : 0,
                GcTotalMemory = samples.Count > 0 ? (long)samples.Average(s => s.GcTotalMemory) : 0,
                Timestamp = now
            };

            var report = new PerformanceReport
            {
                StartTime = reportStartTime,
                EndTime = reportEndTime,
                ResourceUsageSamples = samples,
                Warnings = warnings,
                PeakResourceUsage = peakUsage,
                AverageResourceUsage = avgUsage
            };

            _logger.LogInformation("パフォーマンスレポートを生成しました: {Start} から {End}まで, {SamplesCount}サンプル",
                reportStartTime, reportEndTime, samples.Count);

            return report;
        }

        #region プライベートメソッド

        /// <summary>
        /// リソース使用状況を収集する
        /// </summary>
        private ResourceUsage CollectResourceUsage()
        {
            try
            {
                // 正確な測定のためにプロセス情報を更新
                _currentProcess.Refresh();

                var uptime = DateTime.Now - _currentProcess.StartTime;
                var cpuUsage = _cpuCounter?.NextValue() ?? 0;
                // CPU使用率をプロセッサ数で調整（100%に正規化）
                cpuUsage /= Environment.ProcessorCount;

                // GCコレクション回数の変化を追跡
                var gen0Count = GC.CollectionCount(0);
                var gen1Count = GC.CollectionCount(1);
                var gen2Count = GC.CollectionCount(2);

                var gen0Delta = gen0Count - _lastGen0Count;
                var gen1Delta = gen1Count - _lastGen1Count;
                var gen2Delta = gen2Count - _lastGen2Count;

                _lastGen0Count = gen0Count;
                _lastGen1Count = gen1Count;
                _lastGen2Count = gen2Count;

                return new ResourceUsage
                {
                    CpuUsagePercent = cpuUsage,
                    MemoryUsageBytes = _currentProcess.WorkingSet64,
                    PrivateMemoryBytes = _currentProcess.PrivateMemorySize64,
                    ThreadCount = _currentProcess.Threads.Count,
                    GcTotalMemory = GC.GetTotalMemory(false),
                    Gen0Collections = gen0Delta,
                    Gen1Collections = gen1Delta,
                    Gen2Collections = gen2Delta,
                    ProcessUptime = uptime,
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "リソース使用状況の収集中にエラーが発生しました");

                // エラーが発生しても最小限の情報を返す
                return new ResourceUsage
                {
                    CpuUsagePercent = 0,
                    MemoryUsageBytes = 0,
                    PrivateMemoryBytes = 0,
                    ThreadCount = 0,
                    GcTotalMemory = 0,
                    ProcessUptime = DateTime.Now - _startTime,
                    Timestamp = DateTime.Now
                };
            }
        }

        /// <summary>
        /// リソース閾値をチェックする
        /// </summary>
        private void CheckResourceThresholds(ResourceUsage usage)
        {
            try
            {
                // メモリ使用率チェック (1GB超過)
                const long highMemoryThreshold = 1024 * 1024 * 1024;
                if (usage.PrivateMemoryBytes > highMemoryThreshold)
                {
                    var warningId = "HighMemoryUsage";
                    var warning = new HealthWarning
                    {
                        Id = warningId,
                        Level = HealthWarningLevel.Warning,
                        Message = $"メモリ使用量が高くなっています: {usage.PrivateMemoryFormatted}",
                        OccurredAt = DateTime.Now,
                        Source = "ResourceMonitor",
                        Data = new Dictionary<string, object>
                        {
                            ["MemoryUsage"] = usage.PrivateMemoryBytes,
                            ["Threshold"] = highMemoryThreshold
                        }
                    };

                    AddWarning(warning);
                }
                else
                {
                    ResolveWarning("HighMemoryUsage");
                }

                // CPU使用率チェック (80%超過)
                const float highCpuThreshold = 80.0f;
                if (usage.CpuUsagePercent > highCpuThreshold)
                {
                    var warningId = "HighCpuUsage";
                    var warning = new HealthWarning
                    {
                        Id = warningId,
                        Level = HealthWarningLevel.Warning,
                        Message = $"CPU使用率が高くなっています: {usage.CpuUsagePercent:F1}%",
                        OccurredAt = DateTime.Now,
                        Source = "ResourceMonitor",
                        Data = new Dictionary<string, object>
                        {
                            ["CpuUsage"] = usage.CpuUsagePercent,
                            ["Threshold"] = highCpuThreshold
                        }
                    };

                    AddWarning(warning);
                }
                else
                {
                    ResolveWarning("HighCpuUsage");
                }

                // GC Gen2コレクションのチェック
                if (usage.Gen2Collections > 0)
                {
                    var warningId = "Gen2GarbageCollection";
                    var warning = new HealthWarning
                    {
                        Id = warningId,
                        Level = HealthWarningLevel.Information,
                        Message = $"Gen2ガベージコレクションが発生しました",
                        OccurredAt = DateTime.Now,
                        Source = "ResourceMonitor",
                        Data = new Dictionary<string, object>
                        {
                            ["Gen0Collections"] = usage.Gen0Collections,
                            ["Gen1Collections"] = usage.Gen1Collections,
                            ["Gen2Collections"] = usage.Gen2Collections,
                            ["TotalMemory"] = usage.GcTotalMemory
                        }
                    };

                    AddWarning(warning);
                }
                else
                {
                    ResolveWarning("Gen2GarbageCollection");
                }

                // スレッド数チェック (100超過)
                const int highThreadThreshold = 100;
                if (usage.ThreadCount > highThreadThreshold)
                {
                    var warningId = "HighThreadCount";
                    var warning = new HealthWarning
                    {
                        Id = warningId,
                        Level = HealthWarningLevel.Warning,
                        Message = $"スレッド数が多くなっています: {usage.ThreadCount}",
                        OccurredAt = DateTime.Now,
                        Source = "ResourceMonitor",
                        Data = new Dictionary<string, object>
                        {
                            ["ThreadCount"] = usage.ThreadCount,
                            ["Threshold"] = highThreadThreshold
                        }
                    };

                    AddWarning(warning);
                }
                else
                {
                    ResolveWarning("HighThreadCount");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "リソース閾値のチェック中にエラーが発生しました");
            }
        }

        /// <summary>
        /// 警告を追加する
        /// </summary>
        private void AddWarning(HealthWarning warning)
        {
            lock (_lockObject)
            {
                var existingWarning = _activeWarnings.FirstOrDefault(w => w.Id == warning.Id);
                if (existingWarning != null)
                {
                    // 既存の警告を更新
                    existingWarning.Message = warning.Message;
                    existingWarning.Level = warning.Level;
                    existingWarning.OccurredAt = warning.OccurredAt;
                    existingWarning.Data = warning.Data;
                    return;
                }

                _activeWarnings.Add(warning);
            }

            _logger.LogWarning("ヘルス警告が発生しました: [{Level}] {Message}", warning.Level, warning.Message);
            OnWarningOccurred(new HealthWarningEventArgs(warning));
        }

        /// <summary>
        /// 警告を解決する
        /// </summary>
        private void ResolveWarning(string warningId)
        {
            HealthWarning resolvedWarning = null;

            lock (_lockObject)
            {
                var existingWarning = _activeWarnings.FirstOrDefault(w => w.Id == warningId);
                if (existingWarning != null)
                {
                    resolvedWarning = existingWarning;
                    _activeWarnings.Remove(existingWarning);
                }
            }

            if (resolvedWarning != null)
            {
                _logger.LogInformation("ヘルス警告が解決されました: {Id} - {Message}",
                    resolvedWarning.Id, resolvedWarning.Message);
                OnWarningResolved(new HealthWarningEventArgs(resolvedWarning));
            }
        }

        /// <summary>
        /// パフォーマンスデータをログに記録する
        /// </summary>
        private async Task LogPerformanceDataAsync(ResourceUsage usage)
        {
            try
            {
                var logEntry = new
                {
                    Timestamp = usage.Timestamp,
                    CpuUsage = usage.CpuUsagePercent,
                    MemoryMB = usage.MemoryUsageBytes / (1024.0 * 1024.0),
                    PrivateMemoryMB = usage.PrivateMemoryBytes / (1024.0 * 1024.0),
                    ThreadCount = usage.ThreadCount,
                    Gen0Collections = usage.Gen0Collections,
                    Gen1Collections = usage.Gen1Collections,
                    Gen2Collections = usage.Gen2Collections,
                    GcTotalMemoryMB = usage.GcTotalMemory / (1024.0 * 1024.0)
                };

                var logLine = JsonSerializer.Serialize(logEntry);
                await File.AppendAllTextAsync(_performanceLogPath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "パフォーマンスデータのログ記録中にエラーが発生しました");
            }
        }

        /// <summary>
        /// デフォルトのヘルスチェックを登録する
        /// </summary>
        private void RegisterDefaultHealthChecks()
        {
            // ディスク空き容量チェック
            _healthChecks.Add(async () =>
            {
                var check = new HealthCheck
                {
                    Name = "DiskSpaceCheck",
                    Component = "Storage",
                    Description = "システムドライブの空き容量をチェックします"
                };

                try
                {
                    var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                    var freeSpaceGB = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    var totalSpaceGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    var freePercent = (freeSpaceGB / totalSpaceGB) * 100;

                    check.Data["FreeSpaceGB"] = freeSpaceGB;
                    check.Data["TotalSpaceGB"] = totalSpaceGB;
                    check.Data["FreePercent"] = freePercent;

                    if (freeSpaceGB < 1)
                    {
                        check.Status = HealthCheckStatus.Failed;
                        check.Description = $"ディスク空き容量が非常に少なくなっています: {freeSpaceGB:F2}GB ({freePercent:F1}%)";
                    }
                    else if (freeSpaceGB < 5)
                    {
                        check.Status = HealthCheckStatus.Warning;
                        check.Description = $"ディスク空き容量が少なくなっています: {freeSpaceGB:F2}GB ({freePercent:F1}%)";
                    }
                    else
                    {
                        check.Status = HealthCheckStatus.Passed;
                        check.Description = $"ディスク空き容量は十分です: {freeSpaceGB:F2}GB ({freePercent:F1}%)";
                    }
                }
                catch (Exception ex)
                {
                    check.Status = HealthCheckStatus.Failed;
                    check.Description = $"ディスク空き容量のチェック中にエラーが発生しました: {ex.Message}";
                    check.Data["Exception"] = ex.ToString();
                }

                return check;
            });

            // メモリ断片化チェック
            _healthChecks.Add(async () =>
            {
                var check = new HealthCheck
                {
                    Name = "MemoryFragmentationCheck",
                    Component = "Memory",
                    Description = "メモリの断片化状態をチェックします"
                };

                try
                {
                    // 強制GCの前後でのメモリ使用量を比較
                    var beforeGc = GC.GetTotalMemory(false);
                    GC.Collect(2, GCCollectionMode.Forced, true, true);
                    var afterGc = GC.GetTotalMemory(true);
                    var diffMB = (beforeGc - afterGc) / (1024.0 * 1024.0);
                    var fragPercent = (diffMB / (beforeGc / (1024.0 * 1024.0))) * 100;

                    check.Data["BeforeGcMB"] = beforeGc / (1024.0 * 1024.0);
                    check.Data["AfterGcMB"] = afterGc / (1024.0 * 1024.0);
                    check.Data["FreedMB"] = diffMB;
                    check.Data["FragmentationPercent"] = fragPercent;

                    if (fragPercent > 30)
                    {
                        check.Status = HealthCheckStatus.Warning;
                        check.Description = $"メモリの断片化が見られます: {fragPercent:F1}% ({diffMB:F2}MB解放)";
                    }
                    else
                    {
                        check.Status = HealthCheckStatus.Passed;
                        check.Description = $"メモリの断片化は正常範囲内です: {fragPercent:F1}% ({diffMB:F2}MB解放)";
                    }
                }
                catch (Exception ex)
                {
                    check.Status = HealthCheckStatus.Failed;
                    check.Description = $"メモリ断片化のチェック中にエラーが発生しました: {ex.Message}";
                    check.Data["Exception"] = ex.ToString();
                }

                return check;
            });

            // 設定ファイルチェック
            _healthChecks.Add(async () =>
            {
                var check = new HealthCheck
                {
                    Name = "ConfigurationCheck",
                    Component = "Settings",
                    Description = "設定ファイルの整合性をチェックします"
                };

                try
                {
                    if (_settingsService == null)
                    {
                        check.Status = HealthCheckStatus.Skipped;
                        check.Description = "設定サービスが利用できないため、チェックをスキップしました";
                        return check;
                    }

                    var keys = await _settingsService.GetSettingKeysAsync();
                    check.Data["SettingKeysCount"] = keys.Count();

                    // テスト用に適当な設定を読み取り
                    var testKey = "HealthMonitor_Interval";
                    var testValue = await _settingsService.GetSettingAsync<int>(testKey, 5000);
                    check.Data["TestSettingValue"] = testValue;

                    check.Status = HealthCheckStatus.Passed;
                    check.Description = $"設定ファイルは正常に動作しています ({keys.Count()}個の設定)";
                }
                catch (Exception ex)
                {
                    check.Status = HealthCheckStatus.Failed;
                    check.Description = $"設定ファイルのチェック中にエラーが発生しました: {ex.Message}";
                    check.Data["Exception"] = ex.ToString();
                }

                return check;
            });

            // 長時間実行チェック
            _healthChecks.Add(async () =>
            {
                var check = new HealthCheck
                {
                    Name = "UptimeCheck",
                    Component = "System",
                    Description = "アプリケーションの稼働時間をチェックします"
                };

                try
                {
                    var uptime = DateTime.Now - _startTime;
                    check.Data["UptimeDays"] = uptime.TotalDays;
                    check.Data["UptimeHours"] = uptime.TotalHours;

                    if (uptime.TotalDays > 7)
                    {
                        check.Status = HealthCheckStatus.Warning;
                        check.Description = $"アプリケーションが長時間実行されています: {uptime.TotalDays:F1}日間";
                    }
                    else
                    {
                        check.Status = HealthCheckStatus.Passed;
                        check.Description = $"アプリケーションの稼働時間は正常範囲内です: {uptime.TotalHours:F1}時間";
                    }
                }
                catch (Exception ex)
                {
                    check.Status = HealthCheckStatus.Failed;
                    check.Description = $"稼働時間のチェック中にエラーが発生しました: {ex.Message}";
                    check.Data["Exception"] = ex.ToString();
                }

                return check;
            });
        }

        /// <summary>
        /// 警告発生イベントを発火
        /// </summary>
        protected virtual void OnWarningOccurred(HealthWarningEventArgs e)
        {
            WarningOccurred?.Invoke(this, e);
        }

        /// <summary>
        /// 警告解決イベントを発火
        /// </summary>
        protected virtual void OnWarningResolved(HealthWarningEventArgs e)
        {
            WarningResolved?.Invoke(this, e);
        }

        /// <summary>
        /// リソース使用状況更新イベントを発火
        /// </summary>
        protected virtual void OnResourceUsageUpdated(ResourceUsageEventArgs e)
        {
            ResourceUsageUpdated?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            StopMonitoringAsync().Wait();
            _cpuCounter?.Dispose();
            _monitoringCts?.Dispose();
            _logger.LogInformation("ヘルスモニタリングサービスを破棄しました");
        }
    }
}
