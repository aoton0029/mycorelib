using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Serialization
{
    /// <summary>
    /// シリアライザーのインターフェース
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// サポートするシリアライズフォーマット
        /// </summary>
        CoreLib.Core.Enums.DataFormat Format { get; }

        /// <summary>
        /// オブジェクトを文字列にシリアライズ
        /// </summary>
        string Serialize<T>(T obj);

        /// <summary>
        /// 文字列をオブジェクトにデシリアライズ
        /// </summary>
        T? Deserialize<T>(string data);

        /// <summary>
        /// オブジェクトをストリームにシリアライズ
        /// </summary>
        Task SerializeToStreamAsync<T>(T obj, Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// ストリームからオブジェクトにデシリアライズ
        /// </summary>
        Task<T?> DeserializeFromStreamAsync<T>(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// オブジェクトをファイルにシリアライズ
        /// </summary>
        Task SerializeToFileAsync<T>(T obj, string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// ファイルからオブジェクトにデシリアライズ
        /// </summary>
        Task<T?> DeserializeFromFileAsync<T>(string filePath, CancellationToken cancellationToken = default);
    }
}
