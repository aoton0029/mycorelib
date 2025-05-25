using CoreLib.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Storage
{
    /// <summary>
    /// ファイルストレージサービスのインターフェース
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// ファイルを保存
        /// </summary>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string? subDirectory = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// ファイルデータを保存
        /// </summary>
        Task<string> SaveFileAsync(byte[] fileData, string fileName, string? subDirectory = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// ファイルを取得
        /// </summary>
        Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// ファイルを削除
        /// </summary>
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// ファイルの存在確認
        /// </summary>
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// ディレクトリ内のファイル一覧を取得
        /// </summary>
        Task<IEnumerable<string>> ListFilesAsync(string? subDirectory = null, string? searchPattern = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ローカルファイルシステムを使用したストレージサービス
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _baseDirectory;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="baseDirectory">ファイル保存の基本ディレクトリ</param>
        public LocalFileStorageService(string baseDirectory)
        {
            _baseDirectory = baseDirectory ?? throw new ArgumentNullException(nameof(baseDirectory));

            // ベースディレクトリが存在しない場合は作成
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
        }

        /// <summary>
        /// ファイルを保存
        /// </summary>
        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string? subDirectory = null, CancellationToken cancellationToken = default)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("ファイル名は必須です", nameof(fileName));

            string directory = GetFullDirectoryPath(subDirectory);
            string uniqueFileName = GetUniqueFileName(fileName);
            string fullPath = Path.Combine(directory, uniqueFileName);

            try
            {
                // ディレクトリがなければ作成
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream2 = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await fileStream.CopyToAsync(fileStream2, cancellationToken);
                }

                return GetRelativePath(fullPath);
            }
            catch (Exception ex)
            {
                throw new AppException("FileStorage", $"ファイルの保存中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// ファイルデータを保存
        /// </summary>
        public async Task<string> SaveFileAsync(byte[] fileData, string fileName, string? subDirectory = null, CancellationToken cancellationToken = default)
        {
            if (fileData == null || fileData.Length == 0)
                throw new ArgumentException("ファイルデータは必須です", nameof(fileData));

            using var stream = new MemoryStream(fileData);
            return await SaveFileAsync(stream, fileName, subDirectory, cancellationToken);
        }

        /// <summary>
        /// ファイルを取得
        /// </summary>
        public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string fullPath = GetFullPath(filePath);

            if (!File.Exists(fullPath))
                throw new AppException("FileNotFound", $"指定されたファイルが見つかりません: {filePath}");

            try
            {
                // MemoryStreamにファイルを読み込む
                var memoryStream = new MemoryStream();
                using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await fileStream.CopyToAsync(memoryStream, cancellationToken);
                }

                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new AppException("FileStorage", $"ファイルの読み込み中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// ファイルを削除
        /// </summary>
        public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string fullPath = GetFullPath(filePath);

            if (!File.Exists(fullPath))
                return Task.CompletedTask;

            try
            {
                File.Delete(fullPath);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new AppException("FileStorage", $"ファイルの削除中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// ファイルの存在確認
        /// </summary>
        public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
        {
            string fullPath = GetFullPath(filePath);
            return Task.FromResult(File.Exists(fullPath));
        }

        /// <summary>
        /// ディレクトリ内のファイル一覧を取得
        /// </summary>
        public Task<IEnumerable<string>> ListFilesAsync(string? subDirectory = null, string? searchPattern = null, CancellationToken cancellationToken = default)
        {
            string directory = GetFullDirectoryPath(subDirectory);

            if (!Directory.Exists(directory))
                return Task.FromResult(Enumerable.Empty<string>());

            try
            {
                searchPattern ??= "*";
                var files = Directory.GetFiles(directory, searchPattern, SearchOption.TopDirectoryOnly)
                    .Select(GetRelativePath);

                return Task.FromResult(files);
            }
            catch (Exception ex)
            {
                throw new AppException("FileStorage", $"ファイル一覧の取得中にエラーが発生しました: {ex.Message}");
            }
        }

        #region Helper Methods

        private string GetFullDirectoryPath(string? subDirectory)
        {
            if (string.IsNullOrEmpty(subDirectory))
                return _baseDirectory;

            // サブディレクトリのパスを安全に結合
            string safePath = subDirectory.Replace("..", "").TrimStart('/', '\\');
            return Path.Combine(_baseDirectory, safePath);
        }

        private string GetFullPath(string relativePath)
        {
            // 相対パスから絶対パスに変換（パストラバーサル攻撃の防止）
            string safePath = relativePath.Replace("..", "").TrimStart('/', '\\');
            return Path.Combine(_baseDirectory, safePath);
        }

        private string GetRelativePath(string fullPath)
        {
            // 絶対パスから相対パスに変換
            if (fullPath.StartsWith(_baseDirectory))
            {
                return fullPath.Substring(_baseDirectory.Length).TrimStart('/', '\\');
            }
            return fullPath;
        }

        private string GetUniqueFileName(string fileName)
        {
            // ファイル名が重複しないようにGUID接頭辞を追加
            string extension = Path.GetExtension(fileName);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string guid = Guid.NewGuid().ToString("N").Substring(0, 6);

            return $"{nameWithoutExtension}_{timestamp}_{guid}{extension}";
        }

        #endregion
    }
}
