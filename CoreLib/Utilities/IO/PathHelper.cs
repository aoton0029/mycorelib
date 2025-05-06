using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO
{
    /// <summary>
    /// パス操作ユーティリティ
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// ファイル名から無効な文字を削除
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            // パスに使用できない文字を削除または置換
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        }

        /// <summary>
        /// パスから無効な文字を削除
        /// </summary>
        public static string SanitizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // パスに使用できない文字を削除または置換
            var invalidChars = Path.GetInvalidPathChars();
            return new string(path.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        }

        /// <summary>
        /// パスを結合（複数パーツ対応）
        /// </summary>
        public static string CombinePath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            string result = paths[0];
            for (int i = 1; i < paths.Length; i++)
            {
                result = Path.Combine(result, paths[i]);
            }

            return result;
        }

        /// <summary>
        /// ファイル名が重複する場合に連番を付与
        /// </summary>
        public static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFilePath;

            do
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExt}({counter}){extension}");
                counter++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }

        /// <summary>
        /// パスを標準化（相対パスの解決など）
        /// </summary>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            try
            {
                // 相対パスの解決
                var normalizedPath = Path.GetFullPath(path);

                // パスの区切り文字を統一
                normalizedPath = normalizedPath.Replace('\\', Path.DirectorySeparatorChar);

                return normalizedPath;
            }
            catch
            {
                return path;
            }
        }

        /// <summary>
        /// 拡張子を変更（例: .txtを.bakに変更）
        /// </summary>
        public static string ChangeExtension(string filePath, string newExtension)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            return Path.ChangeExtension(filePath, newExtension);
        }

        /// <summary>
        /// タイムスタンプをファイル名に付加
        /// </summary>
        public static string AddTimestampToFileName(string filePath, string format = "yyyyMMdd_HHmmss")
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string timestamp = DateTime.Now.ToString(format);

            return Path.Combine(directory, $"{fileNameWithoutExt}_{timestamp}{extension}");
        }
    }
}
