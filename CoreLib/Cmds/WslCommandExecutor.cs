using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreLib.Cmds.CommandExecutor;

namespace CoreLib.Cmds
{
    public class WslCommandExecutor : AdvancedCommandExecutor
    {
        public class WslCommandOptions : CommandOptions
        {
            /// <summary>
            /// WSLディストリビューション名 (null の場合はデフォルト)
            /// </summary>
            public string Distribution { get; set; }

            /// <summary>
            /// WSL内の作業ディレクトリ (Linuxパス)
            /// </summary>
            public string WslWorkingDirectory { get; set; }

            /// <summary>
            /// 実行するユーザー
            /// </summary>
            public string User { get; set; }

            /// <summary>
            /// 環境変数
            /// </summary>
            public Dictionary<string, string> Environment { get; set; } = new();

            /// <summary>
            /// パス変換を自動で行うか
            /// </summary>
            public bool AutoConvertPaths { get; set; } = true;

            /// <summary>
            /// Windowsパスを自動的にWSLパスに変換するか
            /// </summary>
            public bool ConvertWindowsPaths { get; set; } = true;
        }

        public class WslInfo
        {
            public string Name { get; set; }
            public string State { get; set; }
            public string Version { get; set; }
            public bool IsDefault { get; set; }
        }

        private readonly WslCommandOptions _defaultWslOptions;

        public WslCommandExecutor(WslCommandOptions defaultOptions = null)
            : base(defaultOptions)
        {
            _defaultWslOptions = defaultOptions ?? new WslCommandOptions();
        }

        /// <summary>
        /// WSLコマンドを実行
        /// </summary>
        public async Task<CommandResult> ExecuteWslCommandAsync(
            string command,
            WslCommandOptions options = null,
            CancellationToken cancellationToken = default)
        {
            options = MergeWslOptions(options);

            // WSLコマンドの構築
            var wslCommand = BuildWslCommand(command, options);
            Debug.Print(wslCommand);
            return await ExecuteWithRealtimeOutputAsync(wslCommand, options, cancellationToken);
        }

        /// <summary>
        /// WSLコマンドを同期実行
        /// </summary>
        public CommandResult ExecuteWslCommand(string command, WslCommandOptions options = null)
        {
            return ExecuteWslCommandAsync(command, options).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Bashスクリプトを実行
        /// </summary>
        public async Task<CommandResult> ExecuteBashScriptAsync(
            string script,
            WslCommandOptions options = null,
            CancellationToken cancellationToken = default)
        {
            options = MergeWslOptions(options);

            // スクリプトを一時ファイルに保存
            var tempScriptPath = await CreateTempScriptFileAsync(script, options);

            try
            {
                var command = $"bash {tempScriptPath}";
                return await ExecuteWslCommandAsync(command, options, cancellationToken);
            }
            finally
            {
                // 一時ファイルをクリーンアップ
                await CleanupTempFileAsync(tempScriptPath, options);
            }
        }

        /// <summary>
        /// 複数のWSLコマンドを順次実行
        /// </summary>
        public async Task<CommandResult[]> ExecuteWslBatchAsync(
            string[] commands,
            WslCommandOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var results = new List<CommandResult>();

            foreach (var command in commands)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var result = await ExecuteWslCommandAsync(command, options, cancellationToken);
                results.Add(result);

                if (!result.IsSuccess && options?.StopOnError == true)
                    break;
            }

            return results.ToArray();
        }

        /// <summary>
        /// WSLディストリビューション一覧を取得
        /// </summary>
        public async Task<WslInfo[]> GetDistributionsAsync()
        {
            var result = await base.ExecuteWithRealtimeOutputAsync("wsl --list --verbose");

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to get WSL distributions: {result.StandardError}");

            return ParseWslListOutput(result.StandardOutput);
        }

        /// <summary>
        /// WSLディストリビューションが利用可能かチェック
        /// </summary>
        public async Task<bool> IsDistributionAvailableAsync(string distributionName = null)
        {
            try
            {
                var distributions = await GetDistributionsAsync();

                if (string.IsNullOrEmpty(distributionName))
                    return distributions.Any(d => d.IsDefault && d.State == "Running");

                return distributions.Any(d => d.Name.Equals(distributionName, StringComparison.OrdinalIgnoreCase)
                                           && d.State == "Running");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// WindowsパスをWSLパスに変換
        /// </summary>
        public async Task<string> ConvertWindowsPathToWslAsync(
            string windowsPath,
            string distribution = null)
        {
            var command = $"wslpath '{windowsPath}'";
            var options = new WslCommandOptions { Distribution = distribution };

            var result = await ExecuteWslCommandAsync(command, options);

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to convert path: {result.StandardError}");

            return result.StandardOutput.Trim();
        }

        /// <summary>
        /// WSLパスをWindowsパスに変換
        /// </summary>
        public async Task<string> ConvertWslPathToWindowsAsync(
            string wslPath,
            string distribution = null)
        {
            var command = $"wslpath -w '{wslPath}'";
            var options = new WslCommandOptions { Distribution = distribution };

            var result = await ExecuteWslCommandAsync(command, options);

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to convert path: {result.StandardError}");

            return result.StandardOutput.Trim();
        }

        /// <summary>
        /// ファイルをWSLにコピー
        /// </summary>
        public async Task<CommandResult> CopyFileToWslAsync(
            string windowsFilePath,
            string wslDestinationPath,
            WslCommandOptions options = null)
        {
            options = MergeWslOptions(options);

            if (!File.Exists(windowsFilePath))
                throw new FileNotFoundException($"Source file not found: {windowsFilePath}");

            var wslSourcePath = options.ConvertWindowsPaths
                ? await ConvertWindowsPathToWslAsync(windowsFilePath, options.Distribution)
                : windowsFilePath;

            var command = $"cp '{wslSourcePath}' '{wslDestinationPath}'";
            return await ExecuteWslCommandAsync(command, options);
        }

        /// <summary>
        /// ファイルをWSLからWindowsにコピー
        /// </summary>
        public async Task<CommandResult> CopyFileFromWslAsync(
            string wslSourcePath,
            string windowsDestinationPath,
            WslCommandOptions options = null)
        {
            options = MergeWslOptions(options);

            var windowsDestPath = options.ConvertWindowsPaths
                ? await ConvertWslPathToWindowsAsync(windowsDestinationPath, options.Distribution)
                : windowsDestinationPath;

            var command = $"cp '{wslSourcePath}' '{windowsDestPath}'";
            return await ExecuteWslCommandAsync(command, options);
        }

        #region Private Methods

        private string BuildWslCommand(string command, WslCommandOptions options)
        {
            var wslArgs = new List<string> { "wsl" };

            // ディストリビューション指定
            if (!string.IsNullOrEmpty(options.Distribution))
            {
                wslArgs.Add("-d");
                wslArgs.Add(options.Distribution);
            }

            // ユーザー指定
            if (!string.IsNullOrEmpty(options.User))
            {
                wslArgs.Add("-u");
                wslArgs.Add(options.User);
            }

            // 作業ディレクトリの変更
            if (!string.IsNullOrEmpty(options.WslWorkingDirectory))
            {
                command = $"cd '{options.WslWorkingDirectory}' && {command}";
            }

            // 環境変数の設定
            if (options.Environment?.Any() == true)
            {
                var envVars = string.Join(" ", options.Environment
                    .Select(kv => $"{kv.Key}='{kv.Value}'"));
                command = $"env {envVars} {command}";
            }

            // コマンドを追加
            wslArgs.Add(command);

            return string.Join(" ", wslArgs.Select(arg =>
                arg.Contains(" ") && !arg.StartsWith("'") && !arg.EndsWith("'")
                    ? $"\"{arg}\""
                    : arg));
        }

        private WslCommandOptions MergeWslOptions(WslCommandOptions options)
        {
            if (options == null) return _defaultWslOptions;

            return new WslCommandOptions
            {
                Distribution = options.Distribution ?? _defaultWslOptions.Distribution,
                WslWorkingDirectory = options.WslWorkingDirectory ?? _defaultWslOptions.WslWorkingDirectory,
                User = options.User ?? _defaultWslOptions.User,
                Environment = options.Environment?.Any() == true
                    ? options.Environment
                    : _defaultWslOptions.Environment,
                AutoConvertPaths = options.AutoConvertPaths,
                ConvertWindowsPaths = options.ConvertWindowsPaths,
                WorkingDirectory = options.WorkingDirectory ?? _defaultWslOptions.WorkingDirectory,
                TimeoutMilliseconds = options.TimeoutMilliseconds != 0
                    ? options.TimeoutMilliseconds
                    : _defaultWslOptions.TimeoutMilliseconds,
                ShowWindow = options.ShowWindow || _defaultWslOptions.ShowWindow,
                UseShellExecute = options.UseShellExecute || _defaultWslOptions.UseShellExecute,
                StopOnError = options.StopOnError ?? _defaultWslOptions.StopOnError
            };
        }

        private async Task<string> CreateTempScriptFileAsync(string script, WslCommandOptions options)
        {
            var tempFileName = $"/tmp/wsl_script_{Guid.NewGuid():N}.sh";

            // スクリプト内容をWSLに書き込み
            var writeCommand = $"cat << 'WSLSCRIPTEOF' > {tempFileName}\n{script}\nWSLSCRIPTEOF";
            var result = await ExecuteWslCommandAsync(writeCommand, options);

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to create temp script: {result.StandardError}");

            // 実行権限を付与
            await ExecuteWslCommandAsync($"chmod +x {tempFileName}", options);

            return tempFileName;
        }

        private async Task CleanupTempFileAsync(string tempFilePath, WslCommandOptions options)
        {
            try
            {
                await ExecuteWslCommandAsync($"rm -f {tempFilePath}", options);
            }
            catch
            {
                // クリーンアップエラーは無視
            }
        }

        private WslInfo[] ParseWslListOutput(string output)
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var distributions = new List<WslInfo>();

            foreach (var line in lines.Skip(1)) // ヘッダー行をスキップ
            {
                var parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var name = parts[0].TrimStart('*').Trim();
                    var isDefault = parts[0].StartsWith("*");
                    var state = parts[1];
                    var version = parts[2];

                    distributions.Add(new WslInfo
                    {
                        Name = name,
                        State = state,
                        Version = version,
                        IsDefault = isDefault
                    });
                }
            }

            return distributions.ToArray();
        }

        #endregion
    }
}
