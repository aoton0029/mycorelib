using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreLib.Cmds.CommandExecutor;

namespace CoreLib.Cmds
{
    public class DockerWslExecutor
    {
        private readonly WslCommandExecutor _wslExecutor;
        private readonly WslCommandExecutor.WslCommandOptions _defaultOptions;

        public class DockerContainer
        {
            public string ContainerId { get; set; }
            public string Image { get; set; }
            public string Command { get; set; }
            public string Created { get; set; }
            public string Status { get; set; }
            public string Ports { get; set; }
            public string Names { get; set; }
        }

        public class DockerImage
        {
            public string Repository { get; set; }
            public string Tag { get; set; }
            public string ImageId { get; set; }
            public string Created { get; set; }
            public string Size { get; set; }
        }

        public class DockerBuildOptions
        {
            public string Dockerfile { get; set; } = "Dockerfile";
            public string BuildContext { get; set; } = ".";
            public Dictionary<string, string> BuildArgs { get; set; } = new();
            public string[] Tags { get; set; } = Array.Empty<string>();
            public bool NoCache { get; set; } = false;
            public string Platform { get; set; }
            public bool Quiet { get; set; } = false;
        }

        public class DockerRunOptions
        {
            public bool Detach { get; set; } = false;
            public bool Interactive { get; set; } = false;
            public bool Tty { get; set; } = false;
            public string Name { get; set; }
            public Dictionary<string, string> Environment { get; set; } = new();
            public Dictionary<string, string> Volumes { get; set; } = new();
            public Dictionary<int, int> Ports { get; set; } = new();
            public string Network { get; set; }
            public string WorkingDirectory { get; set; }
            public string User { get; set; }
            public bool RemoveOnExit { get; set; } = false;
            public string[] Command { get; set; } = Array.Empty<string>();
        }

        public DockerWslExecutor(WslCommandExecutor.WslCommandOptions wslOptions = null)
        {
            _defaultOptions = wslOptions ?? new WslCommandExecutor.WslCommandOptions
            {
                Distribution = "Ubuntu", // デフォルトのWSLディストリビューション
                TimeoutMilliseconds = 300000 // 5分（Dockerコマンドは時間がかかる場合がある）
            };

            _wslExecutor = new WslCommandExecutor(_defaultOptions);

            // リアルタイム出力のイベント転送
            _wslExecutor.OutputReceived += (sender, output) => OutputReceived?.Invoke(sender, output);
            _wslExecutor.ErrorReceived += (sender, error) => ErrorReceived?.Invoke(sender, error);
        }

        public event EventHandler<string> OutputReceived;
        public event EventHandler<string> ErrorReceived;

        #region Docker基本操作

        /// <summary>
        /// Dockerが利用可能かチェック
        /// </summary>
        public async Task<bool> IsDockerAvailableAsync()
        {
            try
            {
                var result = await _wslExecutor.ExecuteWslCommandAsync("docker --version", _defaultOptions);
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Docker情報を取得
        /// </summary>
        public async Task<CommandResult> GetDockerInfoAsync()
        {
            return await _wslExecutor.ExecuteWslCommandAsync("docker info", _defaultOptions);
        }

        /// <summary>
        /// Dockerバージョン情報を取得
        /// </summary>
        public async Task<CommandResult> GetDockerVersionAsync()
        {
            return await _wslExecutor.ExecuteWslCommandAsync("docker version", _defaultOptions);
        }

        #endregion

        #region コンテナ操作

        /// <summary>
        /// コンテナ一覧を取得
        /// </summary>
        public async Task<DockerContainer[]> GetContainersAsync(bool includeAll = false)
        {
            var command = includeAll ? "docker ps -a --format table" : "docker ps --format table";
            var result = await _wslExecutor.ExecuteWslCommandAsync(
                $"{command} \"{{{{.ID}}}}\\t{{{{.Image}}}}\\t{{{{.Command}}}}\\t{{{{.CreatedAt}}}}\\t{{{{.Status}}}}\\t{{{{.Ports}}}}\\t{{{{.Names}}}}\"",
                _defaultOptions);

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to get containers: {result.StandardError}");

            return ParseContainerOutput(result.StandardOutput);
        }

        /// <summary>
        /// コンテナを実行
        /// </summary>
        public async Task<CommandResult> RunContainerAsync(
            string image,
            DockerRunOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var command = BuildRunCommand(image, options ?? new DockerRunOptions());
            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions, cancellationToken);
        }

        /// <summary>
        /// コンテナを開始
        /// </summary>
        public async Task<CommandResult> StartContainerAsync(string containerIdOrName)
        {
            return await _wslExecutor.ExecuteWslCommandAsync($"docker start {containerIdOrName}", _defaultOptions);
        }

        /// <summary>
        /// コンテナを停止
        /// </summary>
        public async Task<CommandResult> StopContainerAsync(string containerIdOrName, int timeoutSeconds = 10)
        {
            return await _wslExecutor.ExecuteWslCommandAsync(
                $"docker stop -t {timeoutSeconds} {containerIdOrName}",
                _defaultOptions);
        }

        /// <summary>
        /// コンテナを削除
        /// </summary>
        public async Task<CommandResult> RemoveContainerAsync(string containerIdOrName, bool force = false)
        {
            var command = force
                ? $"docker rm -f {containerIdOrName}"
                : $"docker rm {containerIdOrName}";

            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions);
        }

        /// <summary>
        /// コンテナのログを取得
        /// </summary>
        public async Task<CommandResult> GetContainerLogsAsync(
            string containerIdOrName,
            bool follow = false,
            int tailLines = 0)
        {
            var options = new List<string>();

            if (follow) options.Add("-f");
            if (tailLines > 0) options.Add($"--tail {tailLines}");

            var command = $"docker logs {string.Join(" ", options)} {containerIdOrName}";
            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions);
        }

        /// <summary>
        /// コンテナでコマンドを実行
        /// </summary>
        public async Task<CommandResult> ExecInContainerAsync(
            string containerIdOrName,
            string command,
            bool interactive = false,
            bool tty = false)
        {
            var options = new List<string>();
            if (interactive) options.Add("-i");
            if (tty) options.Add("-t");

            var dockerCommand = $"docker exec {string.Join(" ", options)} {containerIdOrName} {command}";
            return await _wslExecutor.ExecuteWslCommandAsync(dockerCommand, _defaultOptions);
        }

        #endregion

        #region イメージ操作

        /// <summary>
        /// イメージ一覧を取得
        /// </summary>
        public async Task<DockerImage[]> GetImagesAsync()
        {
            var result = await _wslExecutor.ExecuteWslCommandAsync(
                "docker images --format \"{{.Repository}}\\t{{.Tag}}\\t{{.ID}}\\t{{.CreatedAt}}\\t{{.Size}}\"",
                _defaultOptions);

            if (!result.IsSuccess)
                throw new InvalidOperationException($"Failed to get images: {result.StandardError}");

            return ParseImageOutput(result.StandardOutput);
        }

        /// <summary>
        /// イメージをビルド
        /// </summary>
        public async Task<CommandResult> BuildImageAsync(
            string imageName,
            DockerBuildOptions options = null,
            CancellationToken cancellationToken = default)
        {
            var command = BuildBuildCommand(imageName, options ?? new DockerBuildOptions());
            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions, cancellationToken);
        }

        /// <summary>
        /// イメージをプル
        /// </summary>
        public async Task<CommandResult> PullImageAsync(string imageName, CancellationToken cancellationToken = default)
        {
            return await _wslExecutor.ExecuteWslCommandAsync($"docker pull {imageName}", _defaultOptions, cancellationToken);
        }

        /// <summary>
        /// イメージをプッシュ
        /// </summary>
        public async Task<CommandResult> PushImageAsync(string imageName, CancellationToken cancellationToken = default)
        {
            return await _wslExecutor.ExecuteWslCommandAsync($"docker push {imageName}", _defaultOptions, cancellationToken);
        }

        /// <summary>
        /// イメージを削除
        /// </summary>
        public async Task<CommandResult> RemoveImageAsync(string imageIdOrName, bool force = false)
        {
            var command = force
                ? $"docker rmi -f {imageIdOrName}"
                : $"docker rmi {imageIdOrName}";

            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions);
        }

        #endregion

        #region Docker Compose操作

        /// <summary>
        /// Docker Composeでサービスを起動
        /// </summary>
        public async Task<CommandResult> ComposeUpAsync(
            string composeFilePath = "docker-compose.yml",
            bool detach = true,
            CancellationToken cancellationToken = default)
        {
            var options = detach ? "-d" : "";
            return await _wslExecutor.ExecuteWslCommandAsync(
                $"docker-compose -f {composeFilePath} up {options}",
                _defaultOptions,
                cancellationToken);
        }

        /// <summary>
        /// Docker Composeでサービスを停止
        /// </summary>
        public async Task<CommandResult> ComposeDownAsync(string composeFilePath = "docker-compose.yml")
        {
            return await _wslExecutor.ExecuteWslCommandAsync(
                $"docker-compose -f {composeFilePath} down",
                _defaultOptions);
        }

        /// <summary>
        /// Docker Composeでサービスをビルド
        /// </summary>
        public async Task<CommandResult> ComposeBuildAsync(
            string composeFilePath = "docker-compose.yml",
            CancellationToken cancellationToken = default)
        {
            return await _wslExecutor.ExecuteWslCommandAsync(
                $"docker-compose -f {composeFilePath} build",
                _defaultOptions,
                cancellationToken);
        }

        #endregion

        #region システム操作

        /// <summary>
        /// 未使用のリソースをクリーンアップ
        /// </summary>
        public async Task<CommandResult> PruneSystemAsync(bool force = false)
        {
            var command = force
                ? "docker system prune -a -f"
                : "docker system prune -a";

            return await _wslExecutor.ExecuteWslCommandAsync(command, _defaultOptions);
        }

        /// <summary>
        /// Dockerのシステム使用量を取得
        /// </summary>
        public async Task<CommandResult> GetSystemUsageAsync()
        {
            return await _wslExecutor.ExecuteWslCommandAsync("docker system df", _defaultOptions);
        }

        #endregion

        #region Private Methods

        private string BuildRunCommand(string image, DockerRunOptions options)
        {
            var commands = new List<string> { "docker run" };

            if (options.Detach) commands.Add("-d");
            if (options.Interactive) commands.Add("-i");
            if (options.Tty) commands.Add("-t");
            if (options.RemoveOnExit) commands.Add("--rm");

            if (!string.IsNullOrEmpty(options.Name))
                commands.Add($"--name {options.Name}");

            if (!string.IsNullOrEmpty(options.Network))
                commands.Add($"--network {options.Network}");

            if (!string.IsNullOrEmpty(options.WorkingDirectory))
                commands.Add($"-w {options.WorkingDirectory}");

            if (!string.IsNullOrEmpty(options.User))
                commands.Add($"-u {options.User}");

            // 環境変数
            foreach (var env in options.Environment)
            {
                commands.Add($"-e {env.Key}={env.Value}");
            }

            // ボリューム
            foreach (var volume in options.Volumes)
            {
                commands.Add($"-v {volume.Key}:{volume.Value}");
            }

            // ポート
            foreach (var port in options.Ports)
            {
                commands.Add($"-p {port.Key}:{port.Value}");
            }

            commands.Add(image);

            if (options.Command.Any())
            {
                commands.AddRange(options.Command);
            }

            return string.Join(" ", commands);
        }

        private string BuildBuildCommand(string imageName, DockerBuildOptions options)
        {
            var commands = new List<string> { "docker build" };

            if (options.NoCache) commands.Add("--no-cache");
            if (options.Quiet) commands.Add("-q");

            if (!string.IsNullOrEmpty(options.Platform))
                commands.Add($"--platform {options.Platform}");

            if (!string.IsNullOrEmpty(options.Dockerfile))
                commands.Add($"-f {options.Dockerfile}");

            // Build Args
            foreach (var arg in options.BuildArgs)
            {
                commands.Add($"--build-arg {arg.Key}={arg.Value}");
            }

            // Tags
            if (options.Tags.Any())
            {
                foreach (var tag in options.Tags)
                {
                    commands.Add($"-t {tag}");
                }
            }
            else
            {
                commands.Add($"-t {imageName}");
            }

            commands.Add(options.BuildContext);

            return string.Join(" ", commands);
        }

        private DockerContainer[] ParseContainerOutput(string output)
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var containers = new List<DockerContainer>();

            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length >= 7)
                {
                    containers.Add(new DockerContainer
                    {
                        ContainerId = parts[0],
                        Image = parts[1],
                        Command = parts[2],
                        Created = parts[3],
                        Status = parts[4],
                        Ports = parts[5],
                        Names = parts[6]
                    });
                }
            }

            return containers.ToArray();
        }

        private DockerImage[] ParseImageOutput(string output)
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var images = new List<DockerImage>();

            foreach (var line in lines)
            {
                var parts = line.Split('\t');
                if (parts.Length >= 5)
                {
                    images.Add(new DockerImage
                    {
                        Repository = parts[0],
                        Tag = parts[1],
                        ImageId = parts[2],
                        Created = parts[3],
                        Size = parts[4]
                    });
                }
            }

            return images.ToArray();
        }

        #endregion
    }   
}
