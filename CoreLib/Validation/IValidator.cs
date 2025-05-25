using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// 検証機能を提供するインターフェース
    /// </summary>
    public interface IValidator<T>
    {
        /// <summary>
        /// 指定された値が有効かどうか検証する
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns>検証結果</returns>
        ValidationResult Validate(T value);

        /// <summary>
        /// 非同期で値の検証を行う
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns>検証結果を含むタスク</returns>
        Task<ValidationResult> ValidateAsync(T value);

        /// <summary>
        /// 検証エラー時のメッセージを設定
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>このバリデーターインスタンス</returns>
        IValidator<T> WithMessage(string errorMessage);

        /// <summary>
        /// 検証エラー時のエラーコードを設定
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <returns>このバリデーターインスタンス</returns>
        IValidator<T> WithErrorCode(string errorCode);
    }
}
