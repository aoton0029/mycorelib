using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Extensions.System
{
    /// <summary>
    /// ファイル操作の拡張メソッド
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// ファイルがロックされているかを確認
        /// </summary>
        public static bool IsFileLocked(this FileInfo file)
        {
            if (!file.Exists)
                return false;

            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        /// <summary>
        /// ファイルが存在するならばファイルの内容を読み込み、存在しなければデフォルト値を返す
        /// </summary>
        public static async Task<string> ReadFileIfExistsAsync(this string path, string defaultValue = "")
        {
            if (!File.Exists(path))
                return defaultValue;

            try
            {
                return await File.ReadAllTextAsync(path);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// ファイルを安全に書き込む（ディレクトリが存在しない場合は作成）
        /// </summary>
        public static async Task WriteFileSafelyAsync(this string path, string content, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(path, content, encoding);
        }

        /// <summary>
        /// ファイルを安全に削除（例外をスローしない）
        /// </summary>
        public static bool DeleteSafely(this FileInfo file)
        {
            if (!file.Exists)
                return true;

            try
            {
                file.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ファイルを一時的な場所にバックアップしてから操作を行い、完了後に元に戻す
        /// </summary>
        public static async Task<bool> WithBackupAsync(this FileInfo file, Func<Task> action)
        {
            if (!file.Exists)
                return false;

            var backupPath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(file.Name)}_backup_{Guid.NewGuid():N}{file.Extension}");

            try
            {
                File.Copy(file.FullName, backupPath, true);

                await action();

                // 操作が成功したらバックアップを削除
                if (File.Exists(backupPath))
                    File.Delete(backupPath);

                return true;
            }
            catch
            {
                // 操作が失敗したらバックアップから復元
                if (File.Exists(backupPath))
                {
                    if (file.Exists)
                        file.Delete();
                    File.Move(backupPath, file.FullName);
                }

                return false;
            }
        }

        /// <summary>
        /// ファイルのハッシュを計算
        /// </summary>
        public static async Task<string> CalculateHashAsync(this FileInfo file, System.Security.Cryptography.HashAlgorithm hashAlgorithm)
        {
            if (!file.Exists)
                throw new FileNotFoundException("ファイルが存在しません", file.FullName);

            using var stream = file.OpenRead();
            var hashBytes = await hashAlgorithm.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// ファイルをBase64文字列にエンコード
        /// </summary>
        public static async Task<string> ToBase64StringAsync(this FileInfo file)
        {
            if (!file.Exists)
                throw new FileNotFoundException("ファイルが存在しません", file.FullName);

            byte[] bytes = await File.ReadAllBytesAsync(file.FullName);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64文字列からファイルを作成
        /// </summary>
        public static async Task SaveBase64ToFileAsync(this string base64String, string filePath)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentException("Base64文字列が空です", nameof(base64String));

            byte[] bytes = Convert.FromBase64String(base64String);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(filePath, bytes);
        }
    }
}
