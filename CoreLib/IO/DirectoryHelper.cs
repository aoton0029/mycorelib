using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO
{
    /// <summary>
    /// ディレクトリ操作ユーティリティ
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 安全にディレクトリを作成
        /// </summary>
        public static bool SafeCreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全にディレクトリを削除
        /// </summary>
        public static bool SafeDeleteDirectory(string path, bool recursive = false)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 指定したパターンのファイルを再帰的に検索
        /// </summary>
        public static IEnumerable<string> FindFiles(string directory, string searchPattern, bool recursive = true)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                return Directory.EnumerateFiles(directory, searchPattern, searchOption);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// ディレクトリのサイズを計算
        /// </summary>
        public static long CalculateDirectorySize(string directory)
        {
            if (!Directory.Exists(directory))
                return 0;

            try
            {
                return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                    .Sum(file => new FileInfo(file).Length);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// ディレクトリ内のファイルを最終更新日時でソート
        /// </summary>
        public static IEnumerable<string> GetFilesSortedByDate(
            string directory,
            string searchPattern = "*",
            bool ascending = false,
            bool recursive = false)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                var files = Directory.EnumerateFiles(directory, searchPattern, searchOption)
                    .Select(file => new FileInfo(file))
                    .ToList();

                if (ascending)
                    return files.OrderBy(f => f.LastWriteTime).Select(f => f.FullName);
                else
                    return files.OrderByDescending(f => f.LastWriteTime).Select(f => f.FullName);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// 古いファイルを削除（指定日数より前のファイル）
        /// </summary>
        public static int CleanupOldFiles(
            string directory,
            int olderThanDays,
            string searchPattern = "*",
            bool recursive = false)
        {
            if (!Directory.Exists(directory))
                return 0;

            try
            {
                var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var cutoffDate = DateTime.Now.AddDays(-olderThanDays);
                var oldFiles = Directory.EnumerateFiles(directory, searchPattern, searchOption)
                    .Select(file => new FileInfo(file))
                    .Where(file => file.LastWriteTime < cutoffDate)
                    .ToList();

                foreach (var file in oldFiles)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                        // 個別のファイル削除エラーは無視して続行
                    }
                }

                return oldFiles.Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 空のディレクトリを削除
        /// </summary>
        public static int RemoveEmptyDirectories(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
                return 0;

            int count = 0;
            try
            {
                foreach (var directory in Directory.GetDirectories(rootDirectory))
                {
                    count += RemoveEmptyDirectories(directory);
                }

                if (!Directory.EnumerateFileSystemEntries(rootDirectory).Any())
                {
                    Directory.Delete(rootDirectory);
                    count++;
                }
            }
            catch
            {
                // エラーは無視して続行
            }

            return count;
        }
    }
}
