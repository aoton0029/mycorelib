using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO
{
    /// <summary>
    /// ファイル操作ユーティリティ
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 安全にファイルをコピー（ディレクトリ自動作成）
        /// </summary>
        public static async Task<bool> SafeCopyAsync(string sourcePath, string destinationPath)
        {
            try
            {
                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
                await sourceStream.CopyToAsync(destStream);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルの拡張子を確認
        /// </summary>
        public static bool HasValidExtension(string filePath, params string[] allowedExtensions)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            return allowedExtensions.Any(ext => ext.ToLowerInvariant() == extension);
        }

        /// <summary>
        /// ファイルを安全に削除（例外をスローしない）
        /// </summary>
        public static bool SafeDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルを安全に移動（ディレクトリ自動作成）
        /// </summary>
        public static bool SafeMove(string sourcePath, string destinationPath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return false;

                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Move(sourcePath, destinationPath, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// テキストファイルを読み込む
        /// </summary>
        public static async Task<(bool Success, string Content, Exception? Error)> ReadTextAsync(
            string filePath, Encoding? encoding = null)
        {
            try
            {
                encoding ??= Encoding.UTF8;
                string content = await File.ReadAllTextAsync(filePath, encoding);
                return (true, content, null);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex);
            }
        }

        /// <summary>
        /// テキストファイルに書き込む
        /// </summary>
        public static async Task<(bool Success, Exception? Error)> WriteTextAsync(
            string filePath, string content, Encoding? encoding = null, bool createDirectory = true)
        {
            try
            {
                if (createDirectory)
                {
                    var directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                encoding ??= Encoding.UTF8;
                await File.WriteAllTextAsync(filePath, content, encoding);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }

        /// <summary>
        /// ファイルが使用中かどうかを確認
        /// </summary>
        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルのハッシュ値（SHA256）を計算
        /// </summary>
        public static async Task<string> CalculateHashAsync(string filePath)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// 指定されたサイズを超えるとファイルをローテート
        /// </summary>
        public static async Task<bool> RotateFileIfNeededAsync(string filePath, long maxSizeBytes, int maxBackups = 5)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < maxSizeBytes)
                    return false;

                // 既存のバックアップをシフト
                for (int i = maxBackups - 1; i >= 1; i--)
                {
                    string sourceBackup = $"{filePath}.{i}";
                    string destBackup = $"{filePath}.{i + 1}";

                    if (File.Exists(sourceBackup))
                    {
                        if (File.Exists(destBackup))
                            File.Delete(destBackup);

                        File.Move(sourceBackup, destBackup);
                    }
                }

                // 現在のファイルをバックアップ
                string firstBackup = $"{filePath}.1";
                if (File.Exists(firstBackup))
                    File.Delete(firstBackup);

                // 既存のファイルをコピーして削除
                await SafeCopyAsync(filePath, firstBackup);
                File.WriteAllText(filePath, string.Empty); // ファイルをクリア

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// フォルダ内のファイルに対して様々な操作を行う静的汎用クラス
    /// </summary>
    public static class FileOperations
    {
        /// <summary>
        /// 指定されたフォルダ内のファイルを検索します
        /// </summary>
        /// <param name="folderPath">検索対象のフォルダパス</param>
        /// <param name="searchPattern">検索パターン（例: "*.txt"）</param>
        /// <param name="searchOption">検索オプション（サブディレクトリを含めるかどうか）</param>
        /// <returns>ファイルパスのリスト</returns>
        public static IEnumerable<string> FindFiles(string folderPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"フォルダが見つかりません: {folderPath}");

            return Directory.GetFiles(folderPath, searchPattern, searchOption);
        }

        /// <summary>
        /// 指定されたフォルダ内のファイルを条件に基づいて検索します
        /// </summary>
        /// <param name="folderPath">検索対象のフォルダパス</param>
        /// <param name="predicate">ファイル選択条件</param>
        /// <param name="searchOption">検索オプション（サブディレクトリを含めるかどうか）</param>
        /// <returns>条件に一致するファイルパスのリスト</returns>
        public static IEnumerable<string> FindFiles(string folderPath, Func<FileInfo, bool> predicate, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"フォルダが見つかりません: {folderPath}");

            return Directory.GetFiles(folderPath, "*.*", searchOption)
                .Select(path => new FileInfo(path))
                .Where(predicate)
                .Select(fi => fi.FullName);
        }

        /// <summary>
        /// ファイルの内容をテキストとして読み込みます
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        /// <returns>ファイルの内容</returns>
        public static string ReadAllText(string filePath, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return File.ReadAllText(filePath, encoding);
        }

        /// <summary>
        /// テキストをファイルに書き込みます
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="content">書き込む内容</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        public static void WriteAllText(string filePath, string content, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            File.WriteAllText(filePath, content, encoding);
        }

        /// <summary>
        /// ファイルを非同期でテキストとして読み込みます
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        /// <returns>ファイルの内容</returns>
        public static async Task<string> ReadAllTextAsync(string filePath, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return await File.ReadAllTextAsync(filePath, encoding);
        }

        /// <summary>
        /// テキストをファイルに非同期で書き込みます
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="content">書き込む内容</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        public static async Task WriteAllTextAsync(string filePath, string content, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            await File.WriteAllTextAsync(filePath, content, encoding);
        }

        /// <summary>
        /// フォルダ内のファイルに対して指定された処理を実行します
        /// </summary>
        /// <typeparam name="T">処理結果の型</typeparam>
        /// <param name="folderPath">フォルダパス</param>
        /// <param name="action">各ファイルに対して実行する処理</param>
        /// <param name="searchPattern">検索パターン</param>
        /// <param name="searchOption">検索オプション</param>
        /// <returns>処理結果のリスト</returns>
        public static IEnumerable<T> ProcessFiles<T>(string folderPath, Func<string, T> action,
            string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var files = FindFiles(folderPath, searchPattern, searchOption);
            return files.Select(action);
        }

        /// <summary>
        /// フォルダ内のファイルに対して非同期で指定された処理を実行します
        /// </summary>
        /// <typeparam name="T">処理結果の型</typeparam>
        /// <param name="folderPath">フォルダパス</param>
        /// <param name="action">各ファイルに対して実行する非同期処理</param>
        /// <param name="searchPattern">検索パターン</param>
        /// <param name="searchOption">検索オプション</param>
        /// <returns>処理結果のリスト</returns>
        public static async Task<IEnumerable<T>> ProcessFilesAsync<T>(string folderPath, Func<string, Task<T>> action,
            string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var files = FindFiles(folderPath, searchPattern, searchOption);
            var tasks = files.Select(action);
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// ファイルをコピーします（上書き可能）
        /// </summary>
        /// <param name="sourcePath">コピー元ファイルパス</param>
        /// <param name="destinationPath">コピー先ファイルパス</param>
        /// <param name="overwrite">上書きするかどうか</param>
        public static void CopyFile(string sourcePath, string destinationPath, bool overwrite = true)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }

        /// <summary>
        /// ファイルを移動します
        /// </summary>
        /// <param name="sourcePath">移動元ファイルパス</param>
        /// <param name="destinationPath">移動先ファイルパス</param>
        /// <param name="overwrite">上書きするかどうか</param>
        public static void MoveFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            if (overwrite && File.Exists(destinationPath))
                File.Delete(destinationPath);

            File.Move(sourcePath, destinationPath);
        }

        /// <summary>
        /// ファイルを削除します
        /// </summary>
        /// <param name="filePath">削除するファイルパス</param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// フォルダ内のファイルを条件に基づいて検索し、一致したファイルの内容を操作します
        /// </summary>
        /// <param name="folderPath">検索対象のフォルダパス</param>
        /// <param name="fileContentPredicate">ファイル内容の条件</param>
        /// <param name="contentProcessor">ファイル内容を処理する関数</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        /// <param name="searchPattern">検索パターン</param>
        /// <param name="searchOption">検索オプション</param>
        /// <returns>処理されたファイルのパスリスト</returns>
        public static IEnumerable<string> FindAndProcessFileContent(
            string folderPath,
            Func<string, string, bool> fileContentPredicate,
            Func<string, string> contentProcessor,
            Encoding? encoding = null,
            string searchPattern = "*.*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"フォルダが見つかりません: {folderPath}");

            encoding ??= Encoding.UTF8;
            var processedFiles = new List<string>();

            foreach (var filePath in FindFiles(folderPath, searchPattern, searchOption))
            {
                try
                {
                    var content = File.ReadAllText(filePath, encoding);

                    // ファイル内容が条件に一致する場合、処理を実行
                    if (fileContentPredicate(filePath, content))
                    {
                        var newContent = contentProcessor(content);
                        File.WriteAllText(filePath, newContent, encoding);
                        processedFiles.Add(filePath);
                    }
                }
                catch (Exception)
                {
                    // エラーが発生したファイルはスキップ
                    continue;
                }
            }

            return processedFiles;
        }

        /// <summary>
        /// フォルダ内のファイルを条件に基づいて検索し、一致したファイルの内容を非同期で操作します
        /// </summary>
        /// <param name="folderPath">検索対象のフォルダパス</param>
        /// <param name="fileContentPredicate">ファイル内容の条件</param>
        /// <param name="contentProcessor">ファイル内容を処理する関数</param>
        /// <param name="encoding">エンコーディング（nullの場合はUTF-8）</param>
        /// <param name="searchPattern">検索パターン</param>
        /// <param name="searchOption">検索オプション</param>
        /// <returns>処理されたファイルのパスリスト</returns>
        public static async Task<IEnumerable<string>> FindAndProcessFileContentAsync(
            string folderPath,
            Func<string, string, bool> fileContentPredicate,
            Func<string, string> contentProcessor,
            Encoding? encoding = null,
            string searchPattern = "*.*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"フォルダが見つかりません: {folderPath}");

            encoding ??= Encoding.UTF8;
            var processedFiles = new List<string>();
            var files = FindFiles(folderPath, searchPattern, searchOption);

            foreach (var filePath in files)
            {
                try
                {
                    var content = await File.ReadAllTextAsync(filePath, encoding);

                    // ファイル内容が条件に一致する場合、処理を実行
                    if (fileContentPredicate(filePath, content))
                    {
                        var newContent = contentProcessor(content);
                        await File.WriteAllTextAsync(filePath, newContent, encoding);
                        processedFiles.Add(filePath);
                    }
                }
                catch (Exception)
                {
                    // エラーが発生したファイルはスキップ
                    continue;
                }
            }

            return processedFiles;
        }
    }
}
