using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    /// <summary>
    /// 競合解決方法
    /// </summary>
    public enum ConflictResolutionStrategy
    {
        /// <summary>
        /// 競合を検出したら保存を中止
        /// </summary>
        Abort,
        /// <summary>
        /// 現在の変更で上書き
        /// </summary>
        Overwrite,
        /// <summary>
        /// 自動マージを試行
        /// </summary>
        AutoMerge,
        /// <summary>
        /// 手動マージのため競合ファイルを作成
        /// </summary>
        CreateConflictFile
    }

    /// <summary>
    /// ファイル操作の結果
    /// </summary>
    public class FileOperationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public FileConflictInfo? ConflictInfo { get; set; }
        public string? Version { get; set; }
        public string? MergedContent { get; set; }
        public List<string> ConflictMarkers { get; set; } = new();
    }

    /// <summary>
    /// ファイル競合情報
    /// </summary>
    public class FileConflictInfo
    {
        public string CurrentVersion { get; set; } = "";
        public string ExpectedVersion { get; set; } = "";
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; } = "";
        public string ConflictReason { get; set; } = "";
        public string? CurrentContent { get; set; }
        public string? BaseContent { get; set; }
        public string? IncomingContent { get; set; }
    }

    /// <summary>
    /// ファイルメタデータ
    /// </summary>
    public class FileMetadata
    {
        public string Version { get; set; } = "";
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; } = "";
        public string Hash { get; set; } = "";
        public bool IsLocked { get; set; }
        public string? LockedBy { get; set; }
        public DateTime? LockExpiry { get; set; }
        public string? BaseVersion { get; set; }
    }

    /// <summary>
    /// ファイル操作オプション
    /// </summary>
    public class FileOperationOptions
    {
        public string? ExpectedVersion { get; set; }
        public string? BaseVersion { get; set; }
        public string UserId { get; set; } = Environment.UserName;
        public TimeSpan LockTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public ConflictResolutionStrategy ConflictStrategy { get; set; } = ConflictResolutionStrategy.Abort;
        public bool CreateBackup { get; set; } = true;
        public bool EnableAutoLock { get; set; } = true;
    }

    /// <summary>
    /// ファイル編集セッション
    /// </summary>
    public class FileEditSession : IDisposable
    {
        public string FilePath { get; }
        public string Version { get; }
        public string BaseContent { get; }
        public DateTime StartTime { get; }
        public string UserId { get; }
        private readonly FileService _fileService;

        internal FileEditSession(FileService fileService, string filePath, string version, string baseContent, string userId)
        {
            _fileService = fileService;
            FilePath = filePath;
            Version = version;
            BaseContent = baseContent;
            StartTime = DateTime.UtcNow;
            UserId = userId;
        }

        /// <summary>
        /// 編集セッションを終了して保存
        /// </summary>
        public async Task<FileOperationResult> SaveAsync(string content, ConflictResolutionStrategy? strategy = null)
        {
            var options = new FileOperationOptions
            {
                ExpectedVersion = Version,
                BaseVersion = Version,
                UserId = UserId,
                ConflictStrategy = strategy ?? ConflictResolutionStrategy.AutoMerge
            };

            return await _fileService.SaveFileWithMergeAsync(FilePath, content, BaseContent, options);
        }

        public void Dispose()
        {
            // 編集セッション終了時にロックを解除
            _ = Task.Run(async () =>
            {
                var options = new FileOperationOptions { UserId = UserId };
                await _fileService.UnlockFileAsync(FilePath, options);
            });
        }
    }

    /// <summary>
    /// サーバー上のファイルを安全に操作するサービス
    /// </summary>
    public class FileService
    {
        private readonly string _baseDirectory;
        private readonly string _metadataDirectory;
        private readonly string _versionsDirectory;
        private readonly SemaphoreSlim _globalLock = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, SemaphoreSlim> _fileLocks = new Dictionary<string, SemaphoreSlim>();

        public FileService(string baseDirectory)
        {
            _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));
            _metadataDirectory = Path.Combine(_baseDirectory, ".metadata");
            _versionsDirectory = Path.Combine(_baseDirectory, ".versions");

            Directory.CreateDirectory(_baseDirectory);
            Directory.CreateDirectory(_metadataDirectory);
            Directory.CreateDirectory(_versionsDirectory);
        }

        /// <summary>
        /// ファイル編集セッションを開始
        /// </summary>
        public async Task<(FileOperationResult Result, FileEditSession? Session)> StartEditSessionAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                // ファイルを読み込み
                var (readResult, content) = await ReadFileAsync(filePath, options);
                if (!readResult.Success)
                {
                    return (readResult, null);
                }

                // 自動ロックが有効な場合はロック取得
                if (options.EnableAutoLock)
                {
                    var lockResult = await LockFileAsync(filePath, options);
                    if (!lockResult.Success)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"ファイルロックに失敗: {lockResult.ErrorMessage}";
                        return (result, null);
                    }
                }

                var session = new FileEditSession(this, filePath, readResult.Version!, content!, options.UserId);
                result.Success = true;
                result.Version = readResult.Version;

                return (result, session);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"編集セッション開始エラー: {ex.Message}";
                return (result, null);
            }
        }

        /// <summary>
        /// マージ機能付きファイル保存
        /// </summary>
        public async Task<FileOperationResult> SaveFileWithMergeAsync(
            string filePath,
            string content,
            string baseContent,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                var fullPath = GetFullPath(filePath);
                var semaphore = GetFileLock(filePath);

                await semaphore.WaitAsync();
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        // 新規ファイルの場合
                        return await SaveFileAsync(filePath, content, options);
                    }

                    var currentMetadata = await LoadMetadataAsync(filePath);
                    var currentContent = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);

                    // バージョンチェック（競合検出）
                    if (!string.IsNullOrEmpty(options.ExpectedVersion) &&
                        currentMetadata.Version != options.ExpectedVersion)
                    {
                        // 競合が発生
                        result.ConflictInfo = new FileConflictInfo
                        {
                            CurrentVersion = currentMetadata.Version,
                            ExpectedVersion = options.ExpectedVersion,
                            LastModified = currentMetadata.LastModified,
                            LastModifiedBy = currentMetadata.LastModifiedBy,
                            ConflictReason = "ファイルが他のユーザーによって変更されています",
                            CurrentContent = currentContent,
                            BaseContent = baseContent,
                            IncomingContent = content
                        };

                        // 競合解決戦略に基づいて処理
                        switch (options.ConflictStrategy)
                        {
                            case ConflictResolutionStrategy.Abort:
                                result.Success = false;
                                result.ErrorMessage = "競合により保存が中止されました";
                                return result;

                            case ConflictResolutionStrategy.Overwrite:
                                // そのまま上書き保存
                                break;

                            case ConflictResolutionStrategy.AutoMerge:
                                var mergeResult = await PerformAutoMergeAsync(baseContent, currentContent, content);
                                if (mergeResult.HasConflicts)
                                {
                                    result.Success = false;
                                    result.ErrorMessage = "自動マージに失敗しました";
                                    result.ConflictMarkers = mergeResult.ConflictMarkers;
                                    return result;
                                }
                                content = mergeResult.MergedContent;
                                result.MergedContent = content;
                                break;

                            case ConflictResolutionStrategy.CreateConflictFile:
                                await CreateConflictFileAsync(filePath, baseContent, currentContent, content, options);
                                result.Success = false;
                                result.ErrorMessage = "競合ファイルを作成しました。手動でマージしてください";
                                return result;
                        }
                    }

                    // バックアップとバージョン保存
                    if (options.CreateBackup)
                    {
                        await SaveVersionAsync(filePath, currentContent, currentMetadata.Version);
                    }

                    // ファイル保存
                    await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);

                    // メタデータ更新
                    var newMetadata = new FileMetadata
                    {
                        Version = GenerateVersion(),
                        LastModified = DateTime.UtcNow,
                        LastModifiedBy = options.UserId,
                        Hash = ComputeHash(content),
                        IsLocked = false,
                        BaseVersion = options.BaseVersion
                    };

                    await SaveMetadataAsync(filePath, newMetadata);

                    result.Success = true;
                    result.Version = newMetadata.Version;
                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"ファイル保存エラー: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// ファイルを読み込み
        /// </summary>
        public async Task<(FileOperationResult Result, string? Content)> ReadFileAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                var fullPath = GetFullPath(filePath);
                var semaphore = GetFileLock(filePath);

                await semaphore.WaitAsync();
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        result.Success = false;
                        result.ErrorMessage = "ファイルが存在しません";
                        return (result, null);
                    }

                    var metadata = await LoadMetadataAsync(filePath);
                    var content = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);

                    result.Success = true;
                    result.Version = metadata.Version;
                    return (result, content);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"ファイル読み込みエラー: {ex.Message}";
                return (result, null);
            }
        }

        /// <summary>
        /// 基本的なファイル保存
        /// </summary>
        public async Task<FileOperationResult> SaveFileAsync(
            string filePath,
            string content,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                var fullPath = GetFullPath(filePath);
                var semaphore = GetFileLock(filePath);

                await semaphore.WaitAsync();
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                    await File.WriteAllTextAsync(fullPath, content, Encoding.UTF8);

                    var newMetadata = new FileMetadata
                    {
                        Version = GenerateVersion(),
                        LastModified = DateTime.UtcNow,
                        LastModifiedBy = options.UserId,
                        Hash = ComputeHash(content),
                        IsLocked = false
                    };

                    await SaveMetadataAsync(filePath, newMetadata);

                    result.Success = true;
                    result.Version = newMetadata.Version;
                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"ファイル保存エラー: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// ファイルをロック
        /// </summary>
        public async Task<FileOperationResult> LockFileAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                var semaphore = GetFileLock(filePath);
                await semaphore.WaitAsync();

                try
                {
                    var metadata = await LoadMetadataAsync(filePath);

                    if (metadata.IsLocked &&
                        metadata.LockedBy != options.UserId &&
                        metadata.LockExpiry > DateTime.UtcNow)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"ファイルは既に{metadata.LockedBy}によってロックされています";
                        return result;
                    }

                    metadata.IsLocked = true;
                    metadata.LockedBy = options.UserId;
                    metadata.LockExpiry = DateTime.UtcNow.Add(options.LockTimeout);

                    await SaveMetadataAsync(filePath, metadata);

                    result.Success = true;
                    result.Version = metadata.Version;
                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"ファイルロックエラー: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// ファイルのロックを解除
        /// </summary>
        public async Task<FileOperationResult> UnlockFileAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new FileOperationResult();

            try
            {
                var semaphore = GetFileLock(filePath);
                await semaphore.WaitAsync();

                try
                {
                    var metadata = await LoadMetadataAsync(filePath);

                    if (metadata.IsLocked &&
                        metadata.LockedBy != options.UserId &&
                        metadata.LockExpiry > DateTime.UtcNow)
                    {
                        result.Success = false;
                        result.ErrorMessage = "他のユーザーのロックは解除できません";
                        return result;
                    }

                    metadata.IsLocked = false;
                    metadata.LockedBy = null;
                    metadata.LockExpiry = null;

                    await SaveMetadataAsync(filePath, metadata);

                    result.Success = true;
                    result.Version = metadata.Version;
                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"ファイルアンロックエラー: {ex.Message}";
                return result;
            }
        }

        #region Private Methods

        private async Task<MergeResult> PerformAutoMergeAsync(string baseContent, string currentContent, string incomingContent)
        {
            var result = new MergeResult();

            // 簡単な行ベースマージを実装
            var baseLines = baseContent.Split('\n');
            var currentLines = currentContent.Split('\n');
            var incomingLines = incomingContent.Split('\n');

            var mergedLines = new List<string>();
            var maxLength = Math.Max(Math.Max(baseLines.Length, currentLines.Length), incomingLines.Length);

            for (int i = 0; i < maxLength; i++)
            {
                var baseLine = i < baseLines.Length ? baseLines[i] : "";
                var currentLine = i < currentLines.Length ? currentLines[i] : "";
                var incomingLine = i < incomingLines.Length ? incomingLines[i] : "";

                if (baseLine == currentLine && baseLine == incomingLine)
                {
                    // 変更なし
                    mergedLines.Add(baseLine);
                }
                else if (baseLine == currentLine)
                {
                    // incoming側のみ変更
                    mergedLines.Add(incomingLine);
                }
                else if (baseLine == incomingLine)
                {
                    // current側のみ変更
                    mergedLines.Add(currentLine);
                }
                else if (currentLine == incomingLine)
                {
                    // 同じ変更
                    mergedLines.Add(currentLine);
                }
                else
                {
                    // 競合
                    result.HasConflicts = true;
                    result.ConflictMarkers.Add($"行 {i + 1}: 競合が検出されました");

                    // 競合マーカーを追加
                    mergedLines.Add("<<<<<<< Current");
                    mergedLines.Add(currentLine);
                    mergedLines.Add("=======");
                    mergedLines.Add(incomingLine);
                    mergedLines.Add(">>>>>>> Incoming");
                }
            }

            result.MergedContent = string.Join('\n', mergedLines);
            return result;
        }

        private async Task CreateConflictFileAsync(string filePath, string baseContent, string currentContent, string incomingContent, FileOperationOptions options)
        {
            var conflictDirectory = Path.Combine(_baseDirectory, ".conflicts");
            Directory.CreateDirectory(conflictDirectory);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var conflictFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_conflict_{timestamp}.txt";
            var conflictPath = Path.Combine(conflictDirectory, conflictFileName);

            var conflictContent = $@"=== ファイル競合レポート ===
ファイル: {filePath}
発生時刻: {DateTime.Now}
ユーザー: {options.UserId}

=== ベースバージョン ===
{baseContent}

=== 現在のバージョン ===
{currentContent}

=== 変更予定のバージョン ===
{incomingContent}

=== 手動マージの手順 ===
1. 上記の内容を確認してください
2. 必要な変更をマージしてください
3. マージ完了後、正しい内容でファイルを保存してください
";

            await File.WriteAllTextAsync(conflictPath, conflictContent, Encoding.UTF8);
        }

        private async Task SaveVersionAsync(string filePath, string content, string version)
        {
            var versionPath = Path.Combine(_versionsDirectory, $"{filePath.Replace('/', '_').Replace('\\', '_')}_{version}.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(versionPath)!);
            await File.WriteAllTextAsync(versionPath, content, Encoding.UTF8);
        }

        private string GetFullPath(string filePath)
        {
            var safePath = filePath.Replace("..", "").TrimStart('/', '\\');
            return Path.Combine(_baseDirectory, safePath);
        }

        private string GetMetadataPath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var directory = Path.GetDirectoryName(filePath) ?? "";
            var metadataFileName = $"{fileName}.meta";
            return Path.Combine(_metadataDirectory, directory, metadataFileName);
        }

        private SemaphoreSlim GetFileLock(string filePath)
        {
            lock (_fileLocks)
            {
                if (!_fileLocks.ContainsKey(filePath))
                {
                    _fileLocks[filePath] = new SemaphoreSlim(1, 1);
                }
                return _fileLocks[filePath];
            }
        }

        private async Task<FileMetadata> LoadMetadataAsync(string filePath)
        {
            var metadataPath = GetMetadataPath(filePath);

            if (!File.Exists(metadataPath))
            {
                return new FileMetadata
                {
                    Version = GenerateVersion(),
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = Environment.UserName
                };
            }

            try
            {
                var json = await File.ReadAllTextAsync(metadataPath, Encoding.UTF8);
                return JsonSerializer.Deserialize<FileMetadata>(json) ?? new FileMetadata();
            }
            catch
            {
                return new FileMetadata
                {
                    Version = GenerateVersion(),
                    LastModified = DateTime.UtcNow,
                    LastModifiedBy = Environment.UserName
                };
            }
        }

        private async Task SaveMetadataAsync(string filePath, FileMetadata metadata)
        {
            var metadataPath = GetMetadataPath(filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(metadataPath)!);

            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(metadataPath, json, Encoding.UTF8);
        }

        private string GenerateVersion()
        {
            return DateTime.UtcNow.Ticks.ToString("X");
        }

        private string ComputeHash(string content)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hash);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _globalLock?.Dispose();
            foreach (var semaphore in _fileLocks.Values)
            {
                semaphore?.Dispose();
            }
            _fileLocks.Clear();
        }

        #endregion
    }

    /// <summary>
    /// マージ結果
    /// </summary>
    internal class MergeResult
    {
        public string MergedContent { get; set; } = "";
        public bool HasConflicts { get; set; }
        public List<string> ConflictMarkers { get; set; } = new();
    }
}
