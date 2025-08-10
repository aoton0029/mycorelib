using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Merges
{

    public enum ConflictResolutionStrategy
    {
        Abort,               // 競合が発生した場合は保存を中止
        Overwrite,           // 現在のモデルを上書き
        AutoMerge,           // 可能な限り自動マージ
        CreateConflictFile   // 競合ファイルを作成
    }

    public enum ConflictType
    {
        PropertyConflict,    // プロパティレベルの競合
        ModelConflict,       // モデル全体の競合
        LockConflict         // ロック競合
    }

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
    /// JSON操作の結果
    /// </summary>
    /// <typeparam name="T">モデルの型</typeparam>
    public class JsonOperationResult<T> where T : class
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Model { get; set; }
        public string? Version { get; set; }
        public JsonConflictInfo<T>? ConflictInfo { get; set; }
        public List<MergeConflict> Conflicts { get; set; } = new List<MergeConflict>();
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
    /// JSON競合情報
    /// </summary>
    /// <typeparam name="T">モデルの型</typeparam>
    public class JsonConflictInfo<T> where T : class
    {
        public string CurrentVersion { get; set; } = "";
        public string ExpectedVersion { get; set; } = "";
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; } = "";
        public string ConflictReason { get; set; } = "";
        public T? CurrentModel { get; set; }
        public T? BaseModel { get; set; }
        public T? IncomingModel { get; set; }
    }

    public class MergeConflict
    {
        public string PropertyName { get; set; } = "";
        public ConflictType Type { get; set; }
        public object? BaseValue { get; set; }
        public object? LocalValue { get; set; }
        public object? RemoteValue { get; set; }
        public string Description { get; set; } = "";
    }

    public class MergeResult<T>
    {
        public bool IsSuccessful { get; set; }
        public T? MergedModel { get; set; }
        public string? ErrorMessage { get; set; }
        public List<MergeConflict> Conflicts { get; set; } = new List<MergeConflict>();
        public bool HasConflicts => Conflicts.Any();
    }

    /// <summary>
    /// JSON編集セッション
    /// </summary>
    /// <typeparam name="T">モデルの型</typeparam>
    public class JsonEditSession<T> : IDisposable where T : class, ICloneable
    {
        public string FilePath { get; }
        public string Version { get; }
        public T BaseModel { get; }
        public DateTime StartTime { get; }
        public string UserId { get; }
        private readonly JsonFileService<T> _jsonFileService;

        internal JsonEditSession(JsonFileService<T> jsonFileService, string filePath, string version, T baseModel, string userId)
        {
            _jsonFileService = jsonFileService;
            FilePath = filePath;
            Version = version;
            BaseModel = baseModel;
            StartTime = DateTime.UtcNow;
            UserId = userId;
        }

        /// <summary>
        /// 編集セッションを終了して保存
        /// </summary>
        public async Task<JsonOperationResult<T>> SaveAsync(T model, ConflictResolutionStrategy? strategy = null)
        {
            var options = new FileOperationOptions
            {
                ExpectedVersion = Version,
                BaseVersion = Version,
                UserId = UserId,
                ConflictStrategy = strategy ?? ConflictResolutionStrategy.Abort
            };

            return await _jsonFileService._SaveModelWithMergeAsync(FilePath, model, BaseModel, options);
        }

        public void Dispose()
        {
            // 編集セッション終了時にロックを解除
            _ = Task.Run(async () =>
            {
                var options = new FileOperationOptions { UserId = UserId };
                await _jsonFileService.UnlockFileAsync(FilePath, options);
            });
        }
    }

    /// <summary>
    /// JSONファイルサービス - モデルクラス用3-wayマージ対応
    /// </summary>
    /// <typeparam name="T">保存するモデルの型</typeparam>
    public class JsonFileService<T> : IDisposable where T : class, ICloneable
    {
        private string _baseDirectory = "../../設定";
        private readonly SemaphoreSlim _globalLock = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, SemaphoreSlim> _fileLocks = new Dictionary<string, SemaphoreSlim>();
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonFileService(string? baseDirectory = null)
        {
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                _baseDirectory = baseDirectory;
            }
            Directory.CreateDirectory(_baseDirectory);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        #region Public Methods
        /// <summary>
        /// 簡易保存メソッド - 編集セッションを自動管理してモデルを保存
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="model">保存するモデル</param>
        /// <param name="strategy">競合解決戦略（省略時は Abort）</param>
        /// <param name="options">ファイル操作オプション（省略時はデフォルト）</param>
        /// <returns>保存結果</returns>
        public async Task<JsonOperationResult<T>> SaveModelWithSessionAsync(
            string filePath,
            T model,
            ConflictResolutionStrategy? strategy = null,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            options.ConflictStrategy = strategy ?? ConflictResolutionStrategy.Abort;

            try
            {
                // ファイルが存在しない場合は直接保存
                var fullPath = GetFullPath(filePath);
                if (!File.Exists(fullPath))
                {
                    return await SaveModelAsync(filePath, model, options);
                }

                // 編集セッションを開始
                var (sessionResult, session) = await StartEditSessionAsync(filePath, options);

                if (!sessionResult.Success || session == null)
                {
                    return sessionResult;
                }

                // セッションを使用して保存
                using (session)
                {
                    return await session.SaveAsync(model, strategy);
                }
            }
            catch (Exception ex)
            {
                return new JsonOperationResult<T>
                {
                    Success = false,
                    ErrorMessage = $"簡易保存エラー: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 簡易保存メソッド（オーバーロード） - ユーザーIDと競合解決戦略を指定
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="model">保存するモデル</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="strategy">競合解決戦略（省略時は Abort）</param>
        /// <returns>保存結果</returns>
        public async Task<JsonOperationResult<T>> SaveModelWithSessionAsync(
            string filePath,
            T model,
            string userId,
            ConflictResolutionStrategy? strategy = null)
        {
            var options = new FileOperationOptions
            {
                UserId = userId,
                ConflictStrategy = strategy ?? ConflictResolutionStrategy.Abort
            };

            return await SaveModelWithSessionAsync(filePath, model, strategy, options);
        }

        /// <summary>
        /// 簡易保存メソッド（オーバーロード） - フルオプション指定
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="model">保存するモデル</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="strategy">競合解決戦略</param>
        /// <param name="enableAutoLock">自動ロックを有効にするか</param>
        /// <param name="lockTimeout">ロックタイムアウト</param>
        /// <returns>保存結果</returns>
        public async Task<JsonOperationResult<T>> SaveModelWithSessionAsync(
            string filePath,
            T model,
            string userId,
            ConflictResolutionStrategy strategy,
            bool enableAutoLock = true,
            TimeSpan? lockTimeout = null)
        {
            var options = new FileOperationOptions
            {
                UserId = userId,
                ConflictStrategy = strategy,
                EnableAutoLock = enableAutoLock,
                LockTimeout = lockTimeout ?? TimeSpan.FromMinutes(5)
            };

            return await SaveModelWithSessionAsync(filePath, model, strategy, options);
        }

        /// <summary>
        /// モデル編集セッションを開始
        /// </summary>
        public async Task<(JsonOperationResult<T> Result, JsonEditSession<T>? Session)> StartEditSessionAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

            try
            {
                // モデルを読み込み
                var (readResult, model) = await ReadModelAsync(filePath, options);
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

                var session = new JsonEditSession<T>(this, filePath, readResult.Version!, model!, options.UserId);
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
        /// 3-wayマージ機能付きモデル保存
        /// </summary>
        public async Task<JsonOperationResult<T>> _SaveModelWithMergeAsync(
            string filePath,
            T model,
            T baseModel,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

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
                        return await SaveModelAsync(filePath, model, options);
                    }

                    var currentMetadata = await LoadMetadataAsync(filePath);
                    var (_, currentModel) = await ReadModelInternalAsync(filePath);

                    // バージョンチェック（競合検出）
                    if (!string.IsNullOrEmpty(options.ExpectedVersion) &&
                        currentMetadata.Version != options.ExpectedVersion)
                    {
                        // 競合が発生
                        result.ConflictInfo = new JsonConflictInfo<T>
                        {
                            CurrentVersion = currentMetadata.Version,
                            ExpectedVersion = options.ExpectedVersion,
                            LastModified = currentMetadata.LastModified,
                            LastModifiedBy = currentMetadata.LastModifiedBy,
                            ConflictReason = "ファイルが他のユーザーによって変更されています",
                            CurrentModel = currentModel,
                            BaseModel = baseModel,
                            IncomingModel = model
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
                                var mergeResult = PerformThreeWayMerge(baseModel, model, currentModel!);
                                if (mergeResult.IsSuccessful)
                                {
                                    model = mergeResult.MergedModel;
                                    result.Conflicts = mergeResult.Conflicts;
                                }
                                else
                                {
                                    result.Success = false;
                                    result.ErrorMessage = mergeResult.ErrorMessage;
                                    result.Conflicts = mergeResult.Conflicts;
                                    return result;
                                }
                                break;

                            case ConflictResolutionStrategy.CreateConflictFile:
                                await CreateConflictFileAsync(filePath, baseModel, model, currentModel!, options);
                                result.Success = false;
                                result.ErrorMessage = "競合ファイルが作成されました";
                                return result;
                        }
                    }

                    // モデル保存
                    var json = JsonSerializer.Serialize(model, _jsonOptions);
                    await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);

                    // メタデータ更新
                    var newMetadata = new FileMetadata
                    {
                        Version = GenerateVersion(),
                        LastModified = DateTime.UtcNow,
                        LastModifiedBy = options.UserId,
                        Hash = ComputeHash(json),
                        IsLocked = false,
                        BaseVersion = options.BaseVersion
                    };

                    await SaveMetadataAsync(filePath, newMetadata);

                    result.Success = true;
                    result.Model = model;
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
                result.ErrorMessage = $"モデル保存エラー: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// モデルを読み込み
        /// </summary>
        public async Task<(JsonOperationResult<T> Result, T? Model)> ReadModelAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

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
                    var (success, model) = await ReadModelInternalAsync(filePath);

                    if (!success || model == null)
                    {
                        result.Success = false;
                        result.ErrorMessage = "モデルの読み込みに失敗しました";
                        return (result, null);
                    }

                    result.Success = true;
                    result.Model = model;
                    result.Version = metadata.Version;
                    return (result, model);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"モデル読み込みエラー: {ex.Message}";
                return (result, null);
            }
        }

        /// <summary>
        /// 基本的なモデル保存
        /// </summary>
        public async Task<JsonOperationResult<T>> SaveModelAsync(
            string filePath,
            T model,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

            try
            {
                var fullPath = GetFullPath(filePath);
                var semaphore = GetFileLock(filePath);

                await semaphore.WaitAsync();
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                    var json = JsonSerializer.Serialize(model, _jsonOptions);
                    await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);

                    var newMetadata = new FileMetadata
                    {
                        Version = GenerateVersion(),
                        LastModified = DateTime.UtcNow,
                        LastModifiedBy = options.UserId,
                        Hash = ComputeHash(json),
                        IsLocked = false
                    };

                    await SaveMetadataAsync(filePath, newMetadata);

                    result.Success = true;
                    result.Model = model;
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
                result.ErrorMessage = $"モデル保存エラー: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// ファイルをロック
        /// </summary>
        public async Task<JsonOperationResult<T>> LockFileAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

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
        public async Task<JsonOperationResult<T>> UnlockFileAsync(
            string filePath,
            FileOperationOptions? options = null)
        {
            options ??= new FileOperationOptions();
            var result = new JsonOperationResult<T>();

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
        #endregion

        #region Private Methods - 3-way Merge Implementation

        /// <summary>
        /// 3wayマージを実行（ThreeWayMergerの機能を独自実装）
        /// </summary>
        private MergeResult<T> PerformThreeWayMerge(T baseModel, T localModel, T remoteModel)
        {
            var result = new MergeResult<T>();

            try
            {
                // 入力値検証
                if (!ValidateMergeInputs(baseModel, localModel, remoteModel, result))
                {
                    return result;
                }

                // 自動マージ可能な場合の処理
                if (CanAutoMerge(baseModel, localModel, remoteModel))
                {
                    result.MergedModel = PerformAutoMerge(baseModel, localModel, remoteModel);
                    result.IsSuccessful = true;
                    return result;
                }

                // 競合を検出してマージを試行
                result.MergedModel = (T)localModel.Clone();
                DetectAndResolveConflicts(baseModel, localModel, remoteModel, result);

                result.IsSuccessful = !result.HasConflicts;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = $"マージ中にエラーが発生しました: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 入力値の検証
        /// </summary>
        private bool ValidateMergeInputs(T baseModel, T localModel, T remoteModel, MergeResult<T> result)
        {
            if (baseModel == null || localModel == null || remoteModel == null)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = "すべてのモデルインスタンスが必要です";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 自動マージが可能かどうかを判定
        /// </summary>
        private bool CanAutoMerge(T baseModel, T localModel, T remoteModel)
        {
            var baseJson = JsonSerializer.Serialize(baseModel, _jsonOptions);
            var localJson = JsonSerializer.Serialize(localModel, _jsonOptions);
            var remoteJson = JsonSerializer.Serialize(remoteModel, _jsonOptions);

            // ローカルが変更されていない場合、リモートを採用
            if (baseJson.Equals(localJson))
            {
                return true;
            }

            // リモートが変更されていない場合、ローカルを採用
            if (baseJson.Equals(remoteJson))
            {
                return true;
            }

            // 両方が同じ変更をしている場合
            if (localJson.Equals(remoteJson))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 自動マージを実行
        /// </summary>
        private T PerformAutoMerge(T baseModel, T localModel, T remoteModel)
        {
            var baseJson = JsonSerializer.Serialize(baseModel, _jsonOptions);
            var localJson = JsonSerializer.Serialize(localModel, _jsonOptions);
            var remoteJson = JsonSerializer.Serialize(remoteModel, _jsonOptions);

            if (baseJson.Equals(localJson))
            {
                return (T)remoteModel.Clone();
            }

            if (baseJson.Equals(remoteJson))
            {
                return (T)localModel.Clone();
            }

            // 両方が同じ変更
            return (T)localModel.Clone();
        }

        /// <summary>
        /// 競合を検出して解決を試行
        /// </summary>
        private void DetectAndResolveConflicts(T baseModel, T localModel, T remoteModel, MergeResult<T> result)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .ToList();

            foreach (var property in properties)
            {
                var baseValue = property.GetValue(baseModel);
                var localValue = property.GetValue(localModel);
                var remoteValue = property.GetValue(remoteModel);

                var conflictResult = ResolvePropertyConflict(property, baseValue, localValue, remoteValue);

                if (conflictResult.HasConflict)
                {
                    result.Conflicts.Add(new MergeConflict
                    {
                        PropertyName = property.Name,
                        Type = ConflictType.PropertyConflict,
                        BaseValue = baseValue,
                        LocalValue = localValue,
                        RemoteValue = remoteValue,
                        Description = $"プロパティ '{property.Name}' で競合が発生しました"
                    });
                }
                else
                {
                    // 競合がない場合、解決された値を設定
                    property.SetValue(result.MergedModel, conflictResult.ResolvedValue);
                }
            }
        }

        /// <summary>
        /// プロパティレベルの競合解決
        /// </summary>
        private PropertyConflictResult ResolvePropertyConflict(PropertyInfo property, object baseValue, object localValue, object remoteValue)
        {
            var result = new PropertyConflictResult();

            // 値が同じ場合は競合なし
            if (Equals(localValue, remoteValue))
            {
                result.ResolvedValue = localValue;
                return result;
            }

            // ローカルが変更されていない場合、リモートを採用
            if (Equals(baseValue, localValue))
            {
                result.ResolvedValue = remoteValue;
                return result;
            }

            // リモートが変更されていない場合、ローカルを採用
            if (Equals(baseValue, remoteValue))
            {
                result.ResolvedValue = localValue;
                return result;
            }

            // 特定の型に対する自動解決ルール
            if (TryAutoResolveByType(property, baseValue, localValue, remoteValue, out var autoResolvedValue))
            {
                result.ResolvedValue = autoResolvedValue;
                return result;
            }

            // 競合発生
            result.HasConflict = true;
            result.ResolvedValue = localValue; // デフォルトはローカル値
            return result;
        }

        /// <summary>
        /// 型に基づく自動解決を試行
        /// </summary>
        private bool TryAutoResolveByType(PropertyInfo property, object baseValue, object localValue, object remoteValue, out object resolvedValue)
        {
            resolvedValue = null!;

            // 数値型の場合、より大きい値を採用
            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                var localInt = (int?)localValue;
                var remoteInt = (int?)remoteValue;
                resolvedValue = Math.Max(localInt ?? 0, remoteInt ?? 0);
                return true;
            }

            // DateTime型の場合、より新しい日時を採用
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                var localDate = (DateTime?)localValue;
                var remoteDate = (DateTime?)remoteValue;
                resolvedValue = localDate > remoteDate ? localDate : remoteDate;
                return true;
            }

            // 文字列の場合、より長い文字列を採用（簡単な例）
            if (property.PropertyType == typeof(string))
            {
                var localStr = localValue as string ?? "";
                var remoteStr = remoteValue as string ?? "";
                resolvedValue = localStr.Length >= remoteStr.Length ? localStr : remoteStr;
                return true;
            }

            return false;
        }

        /// <summary>
        /// プロパティ競合解決結果
        /// </summary>
        private class PropertyConflictResult
        {
            public bool HasConflict { get; set; }
            public object ResolvedValue { get; set; } = null!;
        }

        #endregion

        #region Private Methods - File Operations

        private async Task<(bool Success, T? Model)> ReadModelInternalAsync(string filePath)
        {
            try
            {
                var fullPath = GetFullPath(filePath);
                var json = await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
                var model = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                return (true, model);
            }
            catch
            {
                return (false, null);
            }
        }

        private async Task CreateConflictFileAsync(string filePath, T baseModel, T localModel, T remoteModel, FileOperationOptions options)
        {
            var conflictInfo = new
            {
                ConflictTime = DateTime.UtcNow,
                BaseModel = baseModel,
                LocalModel = localModel,
                RemoteModel = remoteModel,
                UserId = options.UserId
            };

            var conflictJson = JsonSerializer.Serialize(conflictInfo, _jsonOptions);
            var conflictFilePath = $"{filePath}.conflict.{DateTime.UtcNow:yyyyMMddHHmmss}.json";
            var fullConflictPath = GetFullPath(conflictFilePath);

            await File.WriteAllTextAsync(fullConflictPath, conflictJson, Encoding.UTF8);
        }

        private string GetFullPath(string filePath)
        {
            var safePath = filePath.Replace("..", "").TrimStart('/', '\\');
            return Path.Combine(_baseDirectory, safePath);
        }

        private string GetMetadataPath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var metadataFileName = $"{fileName}.meta";
            return Path.Combine(_baseDirectory, metadataFileName);
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
}
