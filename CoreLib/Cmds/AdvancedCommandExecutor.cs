using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreLib.Cmds.CommandExecutor;

namespace CoreLib.Cmds
{
    public class AdvancedCommandExecutor
    {
        public event EventHandler<string> OutputReceived;
        public event EventHandler<string> ErrorReceived;
        public event EventHandler<CommandResult> CommandCompleted;

        private readonly CommandOptions _defaultOptions;

        public AdvancedCommandExecutor(CommandOptions defaultOptions = null)
        {
            _defaultOptions = defaultOptions ?? new CommandOptions();
        }

        /// <summary>
        /// リアルタイム出力対応の非同期実行
        /// </summary>
        public async Task<CommandResult> ExecuteWithRealtimeOutputAsync(
            string command,
            CommandOptions options = null,
            CancellationToken cancellationToken = default)
        {
            options = MergeOptions(options);
            var stopwatch = Stopwatch.StartNew();

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = options.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = !options.ShowWindow
            };

            try
            {
                using var process = new Process { StartInfo = psi };

                // イベントハンドラの設定
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                        OutputReceived?.Invoke(this, e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.AppendLine(e.Data);
                        ErrorReceived?.Invoke(this, e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // タイムアウトとキャンセル対応
                var timeoutTask = Task.Delay(options.TimeoutMilliseconds, cancellationToken);
                var processTask = Task.Run(() => process.WaitForExit(), cancellationToken);

                var completedTask = await Task.WhenAny(processTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    process.Kill();
                    var message = cancellationToken.IsCancellationRequested
                        ? "Command was cancelled"
                        : $"Command timed out after {options.TimeoutMilliseconds}ms";
                    throw new OperationCanceledException(message);
                }

                stopwatch.Stop();

                var result = new CommandResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = outputBuilder.ToString(),
                    StandardError = errorBuilder.ToString(),
                    ExecutionTime = stopwatch.Elapsed
                };

                CommandCompleted?.Invoke(this, result);
                return result;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Failed to execute command: {command}", ex);
            }
        }

        /// <summary>
        /// バッチコマンド実行
        /// </summary>
        public async Task<CommandResult[]> ExecuteBatchAsync(
            string[] commands,
            CommandOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var results = new CommandResult[commands.Length];

            for (int i = 0; i < commands.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                results[i] = await ExecuteWithRealtimeOutputAsync(commands[i], options, cancellationToken);

                // 前のコマンドが失敗した場合は停止（オプション）
                if (!results[i].IsSuccess && options?.StopOnError == true)
                    break;
            }

            return results;
        }

        private CommandOptions MergeOptions(CommandOptions options)
        {
            if (options == null) return _defaultOptions;

            return new CommandOptions
            {
                WorkingDirectory = options.WorkingDirectory ?? _defaultOptions.WorkingDirectory,
                TimeoutMilliseconds = options.TimeoutMilliseconds != 0 ? options.TimeoutMilliseconds : _defaultOptions.TimeoutMilliseconds,
                ShowWindow = options.ShowWindow || _defaultOptions.ShowWindow,
                UseShellExecute = options.UseShellExecute || _defaultOptions.UseShellExecute,
                StopOnError = options.StopOnError ?? _defaultOptions.StopOnError
            };
        }
    }

    // CommandOptionsの拡張
    public class CommandOptions
    {
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        public int TimeoutMilliseconds { get; set; } = 30000;
        public bool ShowWindow { get; set; } = false;
        public bool UseShellExecute { get; set; } = false;
        public bool? StopOnError { get; set; } = true;
    }
}
