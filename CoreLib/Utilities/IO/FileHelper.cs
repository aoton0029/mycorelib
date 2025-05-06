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
}
