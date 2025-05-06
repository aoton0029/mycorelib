using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// データエクスポート機能を提供するインターフェース
    /// </summary>
    public interface IExport<T>
    {
        /// <summary>
        /// データをCSV形式でエクスポートする
        /// </summary>
        /// <param name="data">エクスポートするデータ</param>
        /// <param name="filePath">出力ファイルパス</param>
        /// <returns>エクスポート成功の場合はtrue</returns>
        Task<bool> ExportToCsvAsync(IEnumerable<T> data, string filePath);

        /// <summary>
        /// データをJSON形式でエクスポートする
        /// </summary>
        /// <param name="data">エクスポートするデータ</param>
        /// <param name="filePath">出力ファイルパス</param>
        /// <returns>エクスポート成功の場合はtrue</returns>
        Task<bool> ExportToJsonAsync(IEnumerable<T> data, string filePath);

        /// <summary>
        /// データをExcel形式でエクスポートする
        /// </summary>
        /// <param name="data">エクスポートするデータ</param>
        /// <param name="filePath">出力ファイルパス</param>
        /// <returns>エクスポート成功の場合はtrue</returns>
        Task<bool> ExportToExcelAsync(IEnumerable<T> data, string filePath);
    }
}
