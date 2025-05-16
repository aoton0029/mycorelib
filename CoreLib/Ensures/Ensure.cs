using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Resources;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Ensures
{
    /// <summary>
    /// パラメータ検証のための汎用的なEnsureクラス
    /// </summary>
    public static class Ensure
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(typeof(Resources.ErrorMessages));

        /// <summary>
        /// 値がnullでないことを確認
        /// </summary>
        public static T NotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                string message = GetLocalizedMessage("NotNull");
                throw new ArgumentNullException(paramName, message);
            }
            return value;
        }

        /// <summary>
        /// 文字列がnullまたは空でないことを確認
        /// </summary>
        public static string NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                string message = GetLocalizedMessage("NotNullOrEmpty");
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 文字列がnull、空、または空白文字のみでないことを確認
        /// </summary>
        public static string NotNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                string message = GetLocalizedMessage("NotNullOrWhiteSpace");
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 値が指定された値より大きいことを確認
        /// </summary>
        public static T GreaterThan<T>(T value, T minValue, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) <= 0)
            {
                string message = string.Format(GetLocalizedMessage("GreaterThan"), minValue);
                throw new ArgumentOutOfRangeException(paramName, value, message);
            }
            return value;
        }

        /// <summary>
        /// 値が指定された値より小さいことを確認
        /// </summary>
        public static T LessThan<T>(T value, T maxValue, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(maxValue) >= 0)
            {
                string message = string.Format(GetLocalizedMessage("LessThan"), maxValue);
                throw new ArgumentOutOfRangeException(paramName, value, message);
            }
            return value;
        }

        /// <summary>
        /// 値が指定された範囲内にあることを確認
        /// </summary>
        public static T Between<T>(T value, T minValue, T maxValue, string paramName) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
            {
                string message = string.Format(GetLocalizedMessage("Between"), minValue, maxValue);
                throw new ArgumentOutOfRangeException(paramName, value, message);
            }
            return value;
        }

        /// <summary>
        /// 値が有効なメールアドレスであることを確認
        /// </summary>
        public static string IsValidEmail(string value, string paramName)
        {
            NotNullOrEmpty(value, paramName);

            // メールアドレス検証用の正規表現（簡易版）
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(value, pattern))
            {
                string message = GetLocalizedMessage("EmailFormat");
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 現在の文化に基づいてローカライズされたメッセージを取得
        /// </summary>
        private static string GetLocalizedMessage(string key)
        {
            // 現在のUI文化に基づいてメッセージを取得
            string message = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);

            // メッセージが見つからない場合はキーをそのまま返す
            return message ?? key;
        }

        /// <summary>
        /// コレクションが空でないことを確認
        /// </summary>
        public static IEnumerable<T> NotEmpty<T>(IEnumerable<T> collection, string paramName)
        {
            NotNull(collection, paramName);

            if (!collection.Any())
            {
                string message = GetLocalizedMessage("CollectionEmpty");
                throw new ArgumentException(message, paramName);
            }
            return collection;
        }

        /// <summary>
        /// コレクションの要素数が指定された範囲内にあることを確認
        /// </summary>
        public static IEnumerable<T> CountBetween<T>(IEnumerable<T> collection, int minCount, int maxCount, string paramName)
        {
            NotNull(collection, paramName);

            int count = collection.Count();
            if (count < minCount || count > maxCount)
            {
                string message = string.Format(GetLocalizedMessage("CollectionCountBetween"), minCount, maxCount, count);
                throw new ArgumentException(message, paramName);
            }
            return collection;
        }

        /// <summary>
        /// GUIDが空でないことを確認
        /// </summary>
        public static Guid NotEmpty(Guid value, string paramName)
        {
            if (value == Guid.Empty)
            {
                string message = GetLocalizedMessage("GuidEmpty");
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 文字列が有効なURLであることを確認
        /// </summary>
        public static string IsValidUrl(string value, string paramName)
        {
            NotNullOrEmpty(value, paramName);

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                string message = GetLocalizedMessage("UrlFormat");
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 値が正数であることを確認
        /// </summary>
        public static T Positive<T>(T value, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) <= 0)
            {
                string message = GetLocalizedMessage("MustBePositive");
                throw new ArgumentOutOfRangeException(paramName, value, message);
            }
            return value;
        }

        /// <summary>
        /// 値が負数でないことを確認
        /// </summary>
        public static T NonNegative<T>(T value, string paramName) where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) < 0)
            {
                string message = GetLocalizedMessage("MustBeNonNegative");
                throw new ArgumentOutOfRangeException(paramName, value, message);
            }
            return value;
        }

        /// <summary>
        /// 文字列が指定された正規表現パターンに一致することを確認
        /// </summary>
        public static string MatchesPattern(string value, string pattern, string paramName)
        {
            NotNullOrEmpty(value, paramName);
            NotNullOrEmpty(pattern, nameof(pattern));

            if (!Regex.IsMatch(value, pattern))
            {
                string message = string.Format(GetLocalizedMessage("PatternMismatch"), pattern);
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 文字列が指定された長さの範囲内にあることを確認
        /// </summary>
        public static string LengthBetween(string value, int minLength, int maxLength, string paramName)
        {
            NotNull(value, paramName);

            if (value.Length < minLength || value.Length > maxLength)
            {
                string message = string.Format(GetLocalizedMessage("StringLengthBetween"), minLength, maxLength);
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 値が列挙型に定義された値であることを確認
        /// </summary>
        public static T IsDefinedEnum<T>(T value, string paramName) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                string message = string.Format(GetLocalizedMessage("InvalidEnumValue"), typeof(T).Name);
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// 条件が真であることを確認
        /// </summary>
        public static void IsTrue(bool condition, string paramName, string? customMessage = null)
        {
            if (!condition)
            {
                string message = customMessage ?? GetLocalizedMessage("ConditionFalse");
                throw new ArgumentException(message, paramName);
            }
        }

        #region ファイル操作
        /// <summary>
        /// ファイルが存在することを確認
        /// </summary>
        public static string FileExists(string filePath, string paramName)
        {
            NotNullOrEmpty(filePath, paramName);

            if (!File.Exists(filePath))
            {
                string message = string.Format(GetLocalizedMessage("FileNotFound"), filePath);
                throw new FileNotFoundException(message, filePath);
            }
            return filePath;
        }

        /// <summary>
        /// ファイルが書き込み可能であることを確認
        /// </summary>
        public static string FileWritable(string filePath, string paramName)
        {
            FileExists(filePath, paramName);

            try
            {
                using var fs = File.OpenWrite(filePath);
                // ファイルが開けたら書き込み可能
            }
            catch (UnauthorizedAccessException)
            {
                string message = string.Format(GetLocalizedMessage("FileNotWritable"), filePath);
                throw new UnauthorizedAccessException(message);
            }

            return filePath;
        }

        /// <summary>
        /// ディレクトリが存在することを確認
        /// </summary>
        public static string DirectoryExists(string directoryPath, string paramName)
        {
            NotNullOrEmpty(directoryPath, paramName);

            if (!Directory.Exists(directoryPath))
            {
                string message = string.Format(GetLocalizedMessage("DirectoryNotFound"), directoryPath);
                throw new DirectoryNotFoundException(message);
            }
            return directoryPath;
        }

        /// <summary>
        /// ディレクトリが書き込み可能であることを確認
        /// </summary>
        public static string DirectoryWritable(string directoryPath, string paramName)
        {
            DirectoryExists(directoryPath, paramName);

            try
            {
                // 一時ファイルを作成して書き込み権限を確認
                string tempFile = Path.Combine(directoryPath, $"write_test_{Guid.NewGuid()}.tmp");
                using (File.Create(tempFile)) { }
                File.Delete(tempFile);
            }
            catch (UnauthorizedAccessException)
            {
                string message = string.Format(GetLocalizedMessage("DirectoryNotWritable"), directoryPath);
                throw new UnauthorizedAccessException(message);
            }

            return directoryPath;
        }

        /// <summary>
        /// ファイルサイズが上限を超えていないことを確認
        /// </summary>
        public static FileInfo FileSizeNotExceeding(string filePath, long maxSizeInBytes, string paramName)
        {
            FileExists(filePath, paramName);

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > maxSizeInBytes)
            {
                string message = string.Format(GetLocalizedMessage("FileSizeExceeded"),
                    fileInfo.Length, maxSizeInBytes, FormatFileSize(maxSizeInBytes));
                throw new ArgumentException(message, paramName);
            }
            return fileInfo;
        }

        /// <summary>
        /// ストリームの長さが上限を超えていないことを確認
        /// </summary>
        public static Stream StreamSizeNotExceeding(Stream stream, long maxSizeInBytes, string paramName)
        {
            NotNull(stream, paramName);

            if (stream.Length > maxSizeInBytes)
            {
                string message = string.Format(GetLocalizedMessage("StreamSizeExceeded"),
                    stream.Length, maxSizeInBytes, FormatFileSize(maxSizeInBytes));
                throw new ArgumentException(message, paramName);
            }
            return stream;
        }

        /// <summary>
        /// バイト配列のサイズが上限を超えていないことを確認
        /// </summary>
        public static byte[] ByteArraySizeNotExceeding(byte[] data, int maxSizeInBytes, string paramName)
        {
            NotNull(data, paramName);

            if (data.Length > maxSizeInBytes)
            {
                string message = string.Format(GetLocalizedMessage("DataSizeExceeded"),
                    data.Length, maxSizeInBytes, FormatFileSize(maxSizeInBytes));
                throw new ArgumentException(message, paramName);
            }
            return data;
        }

        /// <summary>
        /// コレクションのサイズが上限を超えていないことを確認
        /// </summary>
        public static ICollection<T> CollectionSizeNotExceeding<T>(ICollection<T> collection, int maxCount, string paramName)
        {
            NotNull(collection, paramName);

            if (collection.Count > maxCount)
            {
                string message = string.Format(GetLocalizedMessage("CollectionSizeExceeded"), collection.Count, maxCount);
                throw new ArgumentException(message, paramName);
            }
            return collection;
        }

        /// <summary>
        /// 文字列の長さが上限を超えていないことを確認
        /// </summary>
        public static string StringLengthNotExceeding(string value, int maxLength, string paramName)
        {
            NotNull(value, paramName);

            if (value.Length > maxLength)
            {
                string message = string.Format(GetLocalizedMessage("StringLengthExceeded"), value.Length, maxLength);
                throw new ArgumentException(message, paramName);
            }
            return value;
        }

        /// <summary>
        /// ファイルの拡張子が許可されたものであることを確認
        /// </summary>
        public static string FileExtensionIn(string filePath, IEnumerable<string> allowedExtensions, string paramName)
        {
            NotNullOrEmpty(filePath, paramName);

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) ||
                !allowedExtensions.Select(ext => ext.StartsWith(".") ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant())
                                 .Contains(extension))
            {
                string message = string.Format(GetLocalizedMessage("FileExtensionNotAllowed"),
                    extension, string.Join(", ", allowedExtensions));
                throw new ArgumentException(message, paramName);
            }
            return filePath;
        }

        /// <summary>
        /// ファイル拡張子がブラックリストに含まれていないことを確認
        /// </summary>
        public static string FileExtensionNotIn(string filePath, IEnumerable<string> blockedExtensions, string paramName)
        {
            NotNullOrEmpty(filePath, paramName);

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (!string.IsNullOrEmpty(extension) &&
                blockedExtensions.Select(ext => ext.StartsWith(".") ? ext.ToLowerInvariant() : "." + ext.ToLowerInvariant())
                                 .Contains(extension))
            {
                string message = string.Format(GetLocalizedMessage("FileExtensionBlocked"),
                    extension, string.Join(", ", blockedExtensions));
                throw new ArgumentException(message, paramName);
            }
            return filePath;
        }

        /// <summary>
        /// ファイル名が有効であることを確認（無効な文字が含まれていない）
        /// </summary>
        public static string ValidFileName(string fileName, string paramName)
        {
            NotNullOrEmpty(fileName, paramName);

            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0)
            {
                string message = string.Format(GetLocalizedMessage("InvalidFileName"),
                    fileName, string.Join(" ", invalidChars.Select(c => $"'{c}'")));
                throw new ArgumentException(message, paramName);
            }
            return fileName;
        }

        /// <summary>
        /// パスが有効であることを確認（無効な文字が含まれていない）
        /// </summary>
        public static string ValidPath(string path, string paramName)
        {
            NotNullOrEmpty(path, paramName);

            char[] invalidChars = Path.GetInvalidPathChars();
            if (path.IndexOfAny(invalidChars) >= 0)
            {
                string message = string.Format(GetLocalizedMessage("InvalidPath"),
                    path, string.Join(" ", invalidChars.Select(c => $"'{c}'")));
                throw new ArgumentException(message, paramName);
            }
            return path;
        }

        /// <summary>
        /// ストリームが読み取り可能であることを確認
        /// </summary>
        public static Stream CanRead(Stream stream, string paramName)
        {
            NotNull(stream, paramName);

            if (!stream.CanRead)
            {
                string message = GetLocalizedMessage("StreamNotReadable");
                throw new ArgumentException(message, paramName);
            }
            return stream;
        }

        /// <summary>
        /// ストリームが書き込み可能であることを確認
        /// </summary>
        public static Stream CanWrite(Stream stream, string paramName)
        {
            NotNull(stream, paramName);

            if (!stream.CanWrite)
            {
                string message = GetLocalizedMessage("StreamNotWritable");
                throw new ArgumentException(message, paramName);
            }
            return stream;
        }

        /// <summary>
        /// ファイルサイズを読みやすい形式に変換するヘルパーメソッド
        /// </summary>
        private static string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return $"{number:n2} {suffixes[counter]}";
        }
        #endregion

        #region プロジェクト・アプリ操作

        /// <summary>
        /// プロセスが実行中であることを確認
        /// </summary>
        public static Process ProcessIsRunning(int processId, string paramName)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                if (process.HasExited)
                {
                    string message = string.Format(GetLocalizedMessage("ProcessExited"), processId);
                    throw new InvalidOperationException(message);
                }
                return process;
            }
            catch (ArgumentException)
            {
                string message = string.Format(GetLocalizedMessage("ProcessNotFound"), processId);
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// プロセスが実行中でないことを確認
        /// </summary>
        public static int ProcessNotRunning(string processName, string paramName)
        {
            NotNullOrEmpty(processName, nameof(processName));

            var processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                string message = string.Format(GetLocalizedMessage("ProcessAlreadyRunning"), processName);
                throw new InvalidOperationException(message);
            }
            return processes.Length;
        }

        /// <summary>
        /// コンポーネントが初期化済みであることを確認
        /// </summary>
        public static T ComponentInitialized<T>(T component, string paramName) where T : Component
        {
            NotNull(component, paramName);

            if (component.Site == null || component.Container == null)
            {
                string message = string.Format(GetLocalizedMessage("ComponentNotInitialized"), typeof(T).Name);
                throw new InvalidOperationException(message);
            }
            return component;
        }

        /// <summary>
        /// アプリケーションが管理者権限で実行されていることを確認
        /// </summary>
        public static bool IsAdministrator(string paramName)
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
            {
                string message = GetLocalizedMessage("RequiresAdministrator");
                throw new UnauthorizedAccessException(message);
            }
            return true;
        }

        /// <summary>
        /// 特定のWindowsバージョン以上であることを確認
        /// </summary>
        public static Version WindowsVersionAtLeast(Version requiredVersion, string paramName)
        {
            var currentVersion = Environment.OSVersion.Version;
            if (currentVersion < requiredVersion)
            {
                string message = string.Format(GetLocalizedMessage("WindowsVersionRequired"),
                    requiredVersion, currentVersion);
                throw new PlatformNotSupportedException(message);
            }
            return currentVersion;
        }

        ///// <summary>
        ///// コントロールがUIスレッドで実行されていることを確認
        ///// </summary>
        //public static Control OnUIThread(Control control, string paramName)
        //{
        //    NotNull(control, paramName);

        //    if (control.InvokeRequired)
        //    {
        //        string message = GetLocalizedMessage("RequiresUIThread");
        //        throw new InvalidOperationException(message);
        //    }
        //    return control;
        //}

        /// <summary>
        /// ネットワーク接続が利用可能であることを確認
        /// </summary>
        public static bool NetworkAvailable(string paramName)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                string message = GetLocalizedMessage("NetworkNotAvailable");
                throw new InvalidOperationException(message);
            }
            return true;
        }

        /// <summary>
        /// 指定されたポートが利用可能であることを確認
        /// </summary>
        public static int PortAvailable(int port, string paramName)
        {
            if (port < 0 || port > 65535)
            {
                string message = string.Format(GetLocalizedMessage("InvalidPortRange"), port);
                throw new ArgumentOutOfRangeException(paramName, port, message);
            }

            try
            {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnections = ipGlobalProperties.GetActiveTcpListeners();

                if (tcpConnections.Any(endpoint => endpoint.Port == port))
                {
                    string message = string.Format(GetLocalizedMessage("PortInUse"), port);
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format(GetLocalizedMessage("PortCheckFailed"), port, ex.Message);
                throw new InvalidOperationException(message, ex);
            }

            return port;
        }

        ///// <summary>
        ///// 必要なサービスが実行中であることを確認
        ///// </summary>
        //public static ServiceController ServiceRunning(string serviceName, string paramName)
        //{
        //    NotNullOrEmpty(serviceName, nameof(serviceName));

        //    try
        //    {
        //        var service = new ServiceController(serviceName);
        //        if (service.Status != ServiceControllerStatus.Running)
        //        {
        //            string message = string.Format(GetLocalizedMessage("ServiceNotRunning"), serviceName, service.Status);
        //            throw new InvalidOperationException(message);
        //        }
        //        return service;
        //    }
        //    catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist"))
        //    {
        //        string message = string.Format(GetLocalizedMessage("ServiceNotFound"), serviceName);
        //        throw new ArgumentException(message, paramName);
        //    }
        //}

        /// <summary>
        /// アプリケーション設定が有効であることを確認
        /// </summary>
        public static T ValidConfiguration<T>(T configuration, Action<T> validator, string paramName) where T : class
        {
            NotNull(configuration, paramName);
            NotNull(validator, nameof(validator));

            try
            {
                validator(configuration);
                return configuration;
            }
            catch (Exception ex)
            {
                string message = string.Format(GetLocalizedMessage("InvalidConfiguration"), typeof(T).Name, ex.Message);
                throw new ArgumentException(message, paramName, ex);
            }
        }

        #endregion

        #region 汎用例外メッセージ

        /// <summary>
        /// オブジェクトがDispose済みでないことを確認
        /// </summary>
        public static T NotDisposed<T>(T obj, bool isDisposed, string paramName) where T : class
        {
            NotNull(obj, paramName);

            if (isDisposed)
            {
                string message = string.Format(GetLocalizedMessage("ObjectDisposed"), typeof(T).Name);
                throw new ObjectDisposedException(paramName, message);
            }
            return obj;
        }

        /// <summary>
        /// メソッドが特定の状態でのみ呼び出されることを確認
        /// </summary>
        public static void ValidState(bool validState, string stateName, string methodName)
        {
            if (!validState)
            {
                string message = string.Format(GetLocalizedMessage("InvalidMethodCallForState"), methodName, stateName);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// 操作がキャンセルされていないことを確認
        /// </summary>
        public static void NotCanceled(CancellationToken cancellationToken, string operationName)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                string message = string.Format(GetLocalizedMessage("OperationCanceled"), operationName);
                throw new OperationCanceledException(message, cancellationToken);
            }
        }

        /// <summary>
        /// 指定されたタイプに変換可能であることを確認
        /// </summary>
        public static T CanCastTo<T>(object obj, string paramName) where T : class
        {
            NotNull(obj, paramName);

            if (obj is not T result)
            {
                string message = string.Format(GetLocalizedMessage("CannotCastToType"), obj.GetType().Name, typeof(T).Name);
                throw new InvalidCastException(message);
            }
            return result;
        }

        /// <summary>
        /// 実装が完了していない機能の呼び出しを防止
        /// </summary>
        public static void NotImplemented(string featureName)
        {
            string message = string.Format(GetLocalizedMessage("FeatureNotImplemented"), featureName);
            throw new NotImplementedException(message);
        }

        /// <summary>
        /// 非同期操作がタイムアウトしていないことを確認
        /// </summary>
        public static void NotTimedOut(bool isTimedOut, TimeSpan timeout, string operationName)
        {
            if (isTimedOut)
            {
                string message = string.Format(GetLocalizedMessage("OperationTimedOut"), operationName, timeout.TotalSeconds);
                throw new TimeoutException(message);
            }
        }

        /// <summary>
        /// インスタンスが同一スレッドで作成・アクセスされていることを確認
        /// </summary>
        public static void SameThread(int creationThreadId, string objectName)
        {
            int currentThreadId = Environment.CurrentManagedThreadId;
            if (currentThreadId != creationThreadId)
            {
                string message = string.Format(GetLocalizedMessage("CrossThreadOperation"),
                    objectName, creationThreadId, currentThreadId);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// 処理結果が成功であることを確認
        /// </summary>
        public static T SuccessfulOperation<T>(T result, Func<T, bool> isSuccessful, string operationName)
        {
            if (!isSuccessful(result))
            {
                string message = string.Format(GetLocalizedMessage("OperationFailed"), operationName);
                throw new InvalidOperationException(message);
            }
            return result;
        }

        /// <summary>
        /// 指定されたタスクが完了していることを確認
        /// </summary>
        public static Task TaskCompleted(Task task, string taskName)
        {
            NotNull(task, nameof(task));

            if (!task.IsCompleted)
            {
                string message = string.Format(GetLocalizedMessage("TaskNotCompleted"), taskName);
                throw new InvalidOperationException(message);
            }

            if (task.IsFaulted)
            {
                string message = string.Format(GetLocalizedMessage("TaskFaulted"),
                    taskName, task.Exception?.InnerException?.Message ?? "Unknown error");
                throw new AggregateException(message, task.Exception);
            }

            if (task.IsCanceled)
            {
                string message = string.Format(GetLocalizedMessage("TaskCanceled"), taskName);
                throw new OperationCanceledException(message);
            }

            return task;
        }

        /// <summary>
        /// ライセンスが有効であることを確認
        /// </summary>
        public static bool ValidLicense(bool isValid, string featureName)
        {
            if (!isValid)
            {
                string message = string.Format(GetLocalizedMessage("InvalidLicense"), featureName);
                throw new UnauthorizedAccessException(message);
            }
            return true;
        }

        /// <summary>
        /// オブジェクトの状態が有効であることを確認
        /// </summary>
        public static T ValidObjectState<T>(T obj, Func<T, bool> validator, string objectName) where T : class
        {
            NotNull(obj, nameof(obj));
            NotNull(validator, nameof(validator));

            if (!validator(obj))
            {
                string message = string.Format(GetLocalizedMessage("InvalidObjectState"), objectName);
                throw new InvalidOperationException(message);
            }
            return obj;
        }

        #endregion
    }
}
