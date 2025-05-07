using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Shell
{
    /// <summary>
    /// コマンドライン実行ユーティリティ
    /// </summary>
    public static class CommandLineHelper
    {
        /// <summary>
        /// 外部プロセスを実行し、結果を返す
        /// </summary>
        public static async Task<CommandResult> ExecuteCommandAsync(
            string fileName,
            string arguments = "",
            string? workingDirectory = null,
            bool captureOutput = true,
            IDictionary<string, string>? environmentVariables = null,
            CancellationToken cancellationToken = default)
        {
            using var process = new Process();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                RedirectStandardOutput = captureOutput,
                RedirectStandardError = captureOutput,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // 環境変数を追加
            if (environmentVariables != null)
            {
                foreach (var variable in environmentVariables)
                {
                    process.StartInfo.EnvironmentVariables[variable.Key] = variable.Value;
                }
            }

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            if (captureOutput)
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };
            }

            // プロセスの開始と出力キャプチャ
            try
            {
                process.Start();

                if (captureOutput)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                // キャンセルトークンからプロセスをキル
                using var registration = cancellationToken.Register(() =>
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill(true);
                        }
                    }
                    catch { }
                });

                // 非同期で完了を待つ
                await process.WaitForExitAsync(cancellationToken);

                return new CommandResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = outputBuilder.ToString(),
                    StandardError = errorBuilder.ToString(),
                    Success = process.ExitCode == 0
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    ExitCode = -1,
                    StandardOutput = outputBuilder.ToString(),
                    StandardError = $"{errorBuilder} / Exception: {ex.Message}",
                    Success = false,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// コマンドライン引数をパース
        /// </summary>
        public static Dictionary<string, string?> ParseCommandLineArgs(string[] args, string[] validKeys)
        {
            var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                // オプションの形式を確認（--key=value または --key value）
                if (arg.StartsWith("--"))
                {
                    string key = arg.Substring(2);
                    string? value = null;

                    // --key=value 形式
                    int equalIndex = key.IndexOf('=');
                    if (equalIndex >= 0)
                    {
                        value = key.Substring(equalIndex + 1);
                        key = key.Substring(0, equalIndex);
                    }
                    // --key value 形式
                    else if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        value = args[i + 1];
                        i++; // 次の引数をスキップ
                    }

                    // 有効なキーのみ受け入れる
                    if (validKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                    {
                        result[key] = value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// コマンドのタイムアウト付き実行
        /// </summary>
        public static async Task<CommandResult> ExecuteCommandWithTimeoutAsync(
            string fileName,
            string arguments = "",
            TimeSpan? timeout = null,
            string? workingDirectory = null,
            bool captureOutput = true,
            IDictionary<string, string>? environmentVariables = null)
        {
            using var cts = new CancellationTokenSource();

            if (timeout.HasValue)
            {
                cts.CancelAfter(timeout.Value);
            }

            try
            {
                return await ExecuteCommandAsync(
                    fileName,
                    arguments,
                    workingDirectory,
                    captureOutput,
                    environmentVariables,
                    cts.Token);
            }
            catch (OperationCanceledException)
            {
                return new CommandResult
                {
                    ExitCode = -1,
                    StandardError = "コマンド実行がタイムアウトしました",
                    Success = false,
                    TimedOut = true
                };
            }
        }
    }

    /// <summary>
    /// コマンド実行結果
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// 終了コード
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// 標準出力
        /// </summary>
        public string StandardOutput { get; set; } = string.Empty;

        /// <summary>
        /// 標準エラー出力
        /// </summary>
        public string StandardError { get; set; } = string.Empty;

        /// <summary>
        /// 実行が成功したか
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// タイムアウトしたか
        /// </summary>
        public bool TimedOut { get; set; }

        /// <summary>
        /// 発生した例外
        /// </summary>
        public Exception? Exception { get; set; }
    }
}
