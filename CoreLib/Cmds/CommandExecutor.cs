using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Cmds
{
    public class CommandExecutor
    {
        public class CommandResult
        {
            public int ExitCode { get; set; }
            public string StandardOutput { get; set; }
            public string StandardError { get; set; }
            public bool IsSuccess => ExitCode == 0;
            public TimeSpan ExecutionTime { get; set; }
        }

        public class CommandOptions
        {
            public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
            public int TimeoutMilliseconds { get; set; } = 30000; // 30秒
            public bool ShowWindow { get; set; } = false;
            public bool UseShellExecute { get; set; } = false;
        }

        /// <summary>
        /// コマンドを同期実行
        /// </summary>
        public static CommandResult Execute(string command, CommandOptions options = null)
        {
            options ??= new CommandOptions();

            var stopwatch = Stopwatch.StartNew();

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = options.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = options.UseShellExecute,
                CreateNoWindow = !options.ShowWindow
            };

            try
            {
                using var process = new Process { StartInfo = psi };
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                bool finished = process.WaitForExit(options.TimeoutMilliseconds);

                if (!finished)
                {
                    process.Kill();
                    throw new TimeoutException($"Command timed out after {options.TimeoutMilliseconds}ms");
                }

                stopwatch.Stop();

                return new CommandResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = outputTask.Result,
                    StandardError = errorTask.Result,
                    ExecutionTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Failed to execute command: {command}", ex);
            }
        }

        /// <summary>
        /// コマンドを非同期実行
        /// </summary>
        public static async Task<CommandResult> ExecuteAsync(string command, CommandOptions options = null)
        {
            options ??= new CommandOptions();

            var stopwatch = Stopwatch.StartNew();

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = options.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = options.UseShellExecute,
                CreateNoWindow = !options.ShowWindow
            };

            try
            {
                using var process = new Process { StartInfo = psi };
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                var timeoutTask = Task.Delay(options.TimeoutMilliseconds);
                var processTask = Task.Run(() => process.WaitForExit());

                var completedTask = await Task.WhenAny(processTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    process.Kill();
                    throw new TimeoutException($"Command timed out after {options.TimeoutMilliseconds}ms");
                }

                await Task.WhenAll(outputTask, errorTask);
                stopwatch.Stop();

                return new CommandResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = await outputTask,
                    StandardError = await errorTask,
                    ExecutionTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Failed to execute command: {command}", ex);
            }
        }
    }
}