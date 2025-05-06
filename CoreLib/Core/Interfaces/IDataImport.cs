using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// データインポート機能を提供するインターフェース
    /// </summary>
    public interface IDataImport<T>
    {
        /// <summary>
        /// CSVからデータをインポート
        /// </summary>
        Task<Result<IEnumerable<T>>> ImportFromCsvAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// JSONからデータをインポート
        /// </summary>
        Task<Result<IEnumerable<T>>> ImportFromJsonAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Excelからデータをインポート
        /// </summary>
        Task<Result<IEnumerable<T>>> ImportFromExcelAsync(string filePath, string sheetName = "Sheet1", CancellationToken cancellationToken = default);
    }
}
