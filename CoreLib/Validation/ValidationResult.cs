using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// 検証結果を表すクラス
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// 検証エラーのコレクション
        /// </summary>
        public List<ValidationError> Errors { get; } = new List<ValidationError>();

        /// <summary>
        /// 検証が成功したかどうか
        /// </summary>
        public bool IsValid => !Errors.Any();

        /// <summary>
        /// 検証エラーを追加
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public void AddError(string message)
        {
            Errors.Add(new ValidationError(message));
        }

        /// <summary>
        /// 検証エラーを追加
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="propertyName">エラーが発生したプロパティ名</param>
        public void AddError(string message, string propertyName)
        {
            Errors.Add(new ValidationError(message, propertyName));
        }

        /// <summary>
        /// 検証エラーを追加
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="propertyName">エラーが発生したプロパティ名</param>
        /// <param name="errorCode">エラーコード</param>
        public void AddError(string message, string propertyName, string errorCode)
        {
            Errors.Add(new ValidationError(message, propertyName, errorCode));
        }

        /// <summary>
        /// 検証エラーを追加
        /// </summary>
        /// <param name="error">追加する検証エラー</param>
        public void AddError(ValidationError error)
        {
            Errors.Add(error);
        }

        /// <summary>
        /// 別の検証結果からエラーを追加
        /// </summary>
        /// <param name="result">追加元の検証結果</param>
        public void AddErrors(ValidationResult result)
        {
            if (result == null || result.IsValid)
                return;

            Errors.AddRange(result.Errors);
        }

        /// <summary>
        /// 検証が失敗した場合に例外をスロー
        /// </summary>
        public void ThrowIfInvalid()
        {
            if (!IsValid)
            {
                throw new ValidationException(
                    "検証エラーが発生しました。",
                    this.Errors.Select(e => e.Message).ToArray());
            }
        }

        /// <summary>
        /// 新しい成功検証結果を作成
        /// </summary>
        public static ValidationResult Success() => new ValidationResult();

        /// <summary>
        /// 一つのエラーを含む検証結果を作成
        /// </summary>
        public static ValidationResult Error(string message)
        {
            var result = new ValidationResult();
            result.AddError(message);
            return result;
        }

        /// <summary>
        /// エラーリストから検証結果を作成
        /// </summary>
        public static ValidationResult FromErrors(IEnumerable<string> errorMessages)
        {
            var result = new ValidationResult();
            foreach (var message in errorMessages)
            {
                result.AddError(message);
            }
            return result;
        }
    }

    /// <summary>
    /// 検証エラーを表すクラス
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// エラーが発生したプロパティ名
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// エラーコード
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public ValidationError(string message)
            : this(message, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="propertyName">プロパティ名</param>
        public ValidationError(string message, string propertyName)
            : this(message, propertyName, string.Empty)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="errorCode">エラーコード</param>
        public ValidationError(string message, string propertyName, string errorCode)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            PropertyName = propertyName ?? string.Empty;
            ErrorCode = errorCode ?? string.Empty;
        }
    }

    /// <summary>
    /// 検証例外
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// 検証エラーメッセージのコレクション
        /// </summary>
        public string[] Errors { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">例外メッセージ</param>
        /// <param name="errors">検証エラーメッセージの配列</param>
        public ValidationException(string message, string[] errors)
            : base(message)
        {
            Errors = errors ?? Array.Empty<string>();
        }
    }
}
