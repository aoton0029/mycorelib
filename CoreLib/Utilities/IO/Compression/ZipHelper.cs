using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO.Compression
{
    /// <summary>
    /// ZIPアーカイブ操作ユーティリティ
    /// </summary>
    public static class ZipHelper
    {
        /// <summary>
        /// ファイルまたはディレクトリをZIP化
        /// </summary>
        public static async Task<bool> CreateZipFromDirectoryAsync(
            string sourceDirectoryPath,
            string destinationZipFilePath,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            bool includeBaseDirectory = false,
            Encoding? entryNameEncoding = null)
        {
            try
            {
                // 出力先ディレクトリ確保
                var destDir = Path.GetDirectoryName(destinationZipFilePath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // ZIP圧縮
                ZipFile.CreateFromDirectory(
                    sourceDirectoryPath,
                    destinationZipFilePath,
                    compressionLevel,
                    includeBaseDirectory,
                    entryNameEncoding ?? Encoding.UTF8);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ZIPファイルを解凍
        /// </summary>
        public static bool ExtractZipToDirectory(
            string zipFilePath,
            string destinationDirectoryPath,
            bool overwriteExistingFiles = false,
            Encoding? entryNameEncoding = null)
        {
            try
            {
                // 出力先ディレクトリ確保
                if (!Directory.Exists(destinationDirectoryPath))
                {
                    Directory.CreateDirectory(destinationDirectoryPath);
                }

                // ZIP解凍
                ZipFile.ExtractToDirectory(
                    zipFilePath,
                    destinationDirectoryPath,
                    entryNameEncoding ?? Encoding.UTF8,
                    overwriteExistingFiles);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 指定したファイルをZIPに追加
        /// </summary>
        public static async Task<bool> AddFilesToZipAsync(
            string zipFilePath,
            IEnumerable<string> filesToAdd,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            string? entryDirectoryPathInZip = null)
        {
            try
            {
                using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);

                foreach (var filePath in filesToAdd)
                {
                    if (!File.Exists(filePath))
                        continue;

                    string entryName = Path.GetFileName(filePath);
                    if (!string.IsNullOrEmpty(entryDirectoryPathInZip))
                    {
                        entryName = Path.Combine(entryDirectoryPathInZip, entryName)
                            .Replace('\\', '/'); // ZIPエントリのパス区切りは常に/
                    }

                    var entry = zipArchive.CreateEntry(entryName, compressionLevel);

                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(filePath);
                    await fileStream.CopyToAsync(entryStream);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ZIPファイルの内容一覧を取得
        /// </summary>
        public static List<(string FileName, long UncompressedSize, DateTime LastModified)> ListZipContents(
            string zipFilePath,
            Encoding? entryNameEncoding = null)
        {
            var result = new List<(string, long, DateTime)>();

            try
            {
                using var zipArchive = ZipFile.OpenRead(zipFilePath);
                foreach (var entry in zipArchive.Entries)
                {
                    result.Add((entry.FullName, entry.Length, entry.LastWriteTime.DateTime));
                }

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// ZIPファイルから特定のファイルを抽出
        /// </summary>
        public static async Task<bool> ExtractSingleFileAsync(
            string zipFilePath,
            string entryPath,
            string destinationPath,
            bool overwriteExisting = false,
            Encoding? entryNameEncoding = null)
        {
            try
            {
                using var zipArchive = ZipFile.OpenRead(zipFilePath);
                var entry = zipArchive.GetEntry(entryPath.Replace('\\', '/'));

                if (entry == null)
                    return false;

                // 出力先ディレクトリ確保
                var destDir = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                if (File.Exists(destinationPath) && !overwriteExisting)
                    return false;

                using var entryStream = entry.Open();
                using var fileStream = File.Create(destinationPath);
                await entryStream.CopyToAsync(fileStream);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
