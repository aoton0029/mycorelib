using CoreLib.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Services
{
    public class FileService<T> : IDisposable where T : class, new ()
    {
        private readonly ILogger<FileService<T>> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private RetrySettings _retrySettings;
        public string FilePath;

        public FileService(RetrySettings retrySettings = null)
        {
            
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentException("ファイルパスが指定されていません");

            _retrySettings = retrySettings ?? new RetrySettings()
            {
                Timeout = TimeSpan.FromSeconds(30),
                Interval = TimeSpan.FromMilliseconds(100),
                ThrowOnTimeout = true,
                IgnoreException = true,
                TimeoutMessage = "JSONファイル操作がタイムアウトしました"
            };
        }

        /// <summary>
        /// JSON ファイルを排他的に読み込み（Retry機能付き）
        /// </summary>
        public async Task<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                _logger.LogDebug("JSONファイル読み込み開始: {FilePath}", FilePath);

                var result = Retry.WhileException(
                    () => ReadFileInternal(),
                    _retrySettings.Timeout,
                    _retrySettings.Interval,
                    _retrySettings.ThrowOnTimeout,
                    _retrySettings.TimeoutMessage
                );

                if (result.HadException && _retrySettings.ThrowOnTimeout)
                {
                    _logger.LogError(result.LastException, "JSONファイル読み込みでリトライ上限に達しました: {FilePath}", FilePath);
                    throw new InvalidOperationException($"JSONファイルの読み込みに失敗しました: {FilePath}", result.LastException);
                }

                _logger.LogDebug("JSONファイル読み込み完了: {FilePath}, リトライ回数: {RetryCount}", FilePath, result.RetryCount);
                return result.Result ?? new T();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// JSON ファイルを排他的かつアトミックに書き込み（Retry機能付き）
        /// </summary>
        public async Task WriteAsync(T data, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(data);

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                _logger.LogDebug("JSONファイル書き込み開始: {FilePath}", FilePath);

                var result = Retry.WhileException(
                    () => WriteFileInternal(data),
                    _retrySettings.Timeout,
                    _retrySettings.Interval,
                    _retrySettings.ThrowOnTimeout,
                    _retrySettings.TimeoutMessage
                );

                if (result.HadException && _retrySettings.ThrowOnTimeout)
                {
                    _logger.LogError(result.LastException, "JSONファイル書き込みでリトライ上限に達しました: {FilePath}", FilePath);
                    throw new InvalidOperationException($"JSONファイルの書き込みに失敗しました: {FilePath}", result.LastException);
                }

                _logger.LogDebug("JSONファイル書き込み完了: {FilePath}, リトライ回数: {RetryCount}", FilePath, result.RetryCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// ファイルの存在確認
        /// </summary>
        public bool Exists() => File.Exists(FilePath);

        /// <summary>
        /// ファイルサイズを取得
        /// </summary>
        public long GetFileSize()
        {
            return File.Exists(FilePath) ? new FileInfo(FilePath).Length : 0;
        }

        /// <summary>
        /// ファイル読み込みの内部実装
        /// </summary>
        private T ReadFileInternal()
        {
            if (!File.Exists(FilePath))
            {
                _logger.LogDebug("ファイルが存在しないため新しいインスタンスを返します: {FilePath}", FilePath);
                return new T();
            }

            using var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var result = JsonSerializer.Deserialize<T>(fileStream);
            return result ?? new T();
        }

        /// <summary>
        /// ファイル書き込みの内部実装
        /// </summary>
        private void WriteFileInternal(T data)
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempFilePath = FilePath + ".tmp";
            var backupFilePath = FilePath + ".bak";

            // 一時ファイルに排他ロックで書き込み
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                JsonSerializer.Serialize(fileStream, data);
                fileStream.Flush();
            }

            // アトミックな操作でファイルを置き換え
            if (File.Exists(FilePath))
            {
                File.Replace(tempFilePath, FilePath, backupFilePath, ignoreMetadataErrors: true);
                if (File.Exists(backupFilePath))
                {
                    File.Delete(backupFilePath);
                }
            }
            else
            {
                File.Move(tempFilePath, FilePath);
            }
        }

        /// <summary>
        /// ファイルロック状態を確認
        /// </summary>
        public bool IsFileLocked()
        {
            if (!File.Exists(FilePath))
                return false;

            try
            {
                using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
