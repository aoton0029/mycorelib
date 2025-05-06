using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.IO
{
    /// <summary>
    /// ストリーム操作ユーティリティ
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// ストリームをバイト配列に変換
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// バイト配列からMemoryStreamを作成
        /// </summary>
        public static MemoryStream ToMemoryStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// ストリームをテキストとして読み込み
        /// </summary>
        public static async Task<string> ReadToEndAsync(this Stream stream, Encoding? encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            encoding ??= Encoding.UTF8;

            using var reader = new StreamReader(stream, encoding, true, -1, true);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// ストリームにテキストを書き込み
        /// </summary>
        public static async Task WriteTextAsync(this Stream stream, string text, Encoding? encoding = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrEmpty(text))
                return;

            encoding ??= Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// ストリームのコピー（進捗報告付き）
        /// </summary>
        public static async Task CopyToAsync(
            this Stream source,
            Stream destination,
            int bufferSize = 81920,
            IProgress<long>? progress = null,
            System.Threading.CancellationToken cancellationToken = default)
        {
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        /// <summary>
        /// BOMからエンコーディングを検出
        /// </summary>
        public static Encoding? DetectEncoding(this Stream stream)
        {
            if (!stream.CanSeek || !stream.CanRead)
                return null;

            // 現在の位置を保存
            long originalPosition = stream.Position;

            try
            {
                stream.Position = 0;

                // BOMを読み取る
                byte[] bom = new byte[4];
                int read = stream.Read(bom, 0, 4);

                // エンコーディングを判定
                if (read >= 2 && bom[0] == 0xFE && bom[1] == 0xFF)
                    return Encoding.BigEndianUnicode;

                if (read >= 2 && bom[0] == 0xFF && bom[1] == 0xFE)
                {
                    if (read >= 4 && bom[2] == 0 && bom[3] == 0)
                        return Encoding.UTF32;

                    return Encoding.Unicode;
                }

                if (read >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return Encoding.UTF8;

                if (read >= 4 && bom[0] == 0 && bom[1] == 0 && bom[2] == 0xFE && bom[3] == 0xFF)
                    return Encoding.UTF32;

                return null; // BOMが検出できない場合
            }
            finally
            {
                // 元の位置に戻す
                stream.Position = originalPosition;
            }
        }
    }
}
