using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation.Rules
{
    /// <summary>
    /// 検証ルールの基底クラス
    /// </summary>
    /// <typeparam name="T">検証対象の型</typeparam>
    public abstract class ValidationRule<T>
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage { get; set; } = "検証に失敗しました。";

        /// <summary>
        /// エラーコード
        /// </summary>
        public string ErrorCode { get; set; } = "ValidationFailed";

        /// <summary>
        /// プロパティ名
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// 検証ロジック
        /// </summary>
        /// <param name="value">検証対象の値</param>
        /// <returns>検証結果</returns>
        public abstract ValidationResult Validate(T value);

        /// <summary>
        /// エラーメッセージを設定
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns>このルールインスタンス</returns>
        public ValidationRule<T> WithMessage(string message)
        {
            ErrorMessage = message;
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        /// <param name="code">エラーコード</param>
        /// <returns>このルールインスタンス</returns>
        public ValidationRule<T> WithErrorCode(string code)
        {
            ErrorCode = code;
            return this;
        }

        /// <summary>
        /// プロパティ名を設定
        /// </summary>
        /// <param name="name">プロパティ名</param>
        /// <returns>このルールインスタンス</returns>
        public ValidationRule<T> ForProperty(string name)
        {
            PropertyName = name;
            return this;
        }

        /// <summary>
        /// プロパティ名を式から設定
        /// </summary>
        /// <typeparam name="TProperty">プロパティの型</typeparam>
        /// <param name="propertyExpression">プロパティを表す式</param>
        /// <returns>このルールインスタンス</returns>
        public ValidationRule<T> ForProperty<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                PropertyName = memberExpression.Member.Name;
            }
            return this;
        }

        /// <summary>
        /// 検証エラーを作成
        /// </summary>
        /// <returns>検証エラー</returns>
        protected ValidationError CreateError()
        {
            return new ValidationError(ErrorMessage, PropertyName, ErrorCode);
        }
    }
}
