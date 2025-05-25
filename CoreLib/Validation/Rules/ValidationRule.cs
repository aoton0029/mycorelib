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

    /// <summary>
    /// プロパティの検証ルール
    /// </summary>
    public class PropertyValidationRule<T, TProperty> : ValidationRule<T>
    {
        private readonly Expression<Func<T, TProperty>> _propertySelector;
        private readonly Func<TProperty, bool> _predicate;
        private readonly Func<T, TProperty> _propertyGetter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="propertySelector">プロパティを選択する式</param>
        /// <param name="predicate">検証条件</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        public PropertyValidationRule(Expression<Func<T, TProperty>> propertySelector,
                                     Func<TProperty, bool> predicate,
                                     string errorMessage,
                                     string errorCode)
        {
            _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));

            // プロパティ名を抽出
            if (_propertySelector.Body is MemberExpression memberExpression)
            {
                PropertyName = memberExpression.Member.Name;
            }

            // プロパティの値を取得するためのコンパイル済み関数を作成
            _propertyGetter = _propertySelector.Compile();
        }

        /// <summary>
        /// オブジェクトを検証
        /// </summary>
        public override ValidationResult Validate(T value)
        {
            var result = new ValidationResult();

            if (value == null)
            {
                result.AddError("検証対象のオブジェクトがnullです。", "Object", "ObjectNull");
                return result;
            }

            // プロパティの値を取得
            var propertyValue = _propertyGetter(value);

            // 検証条件を適用
            if (!_predicate(propertyValue))
            {
                result.AddError(ErrorMessage, PropertyName, ErrorCode);
            }

            return result;
        }
    }

    /// <summary>
    /// カスタム検証ルール
    /// </summary>
    public class CustomValidationRule<T> : ValidationRule<T>
    {
        private readonly Func<T, bool> _predicate;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="predicate">検証条件</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        public CustomValidationRule(Func<T, bool> predicate, string errorMessage, string errorCode)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        }

        /// <summary>
        /// オブジェクトを検証
        /// </summary>
        public override ValidationResult Validate(T value)
        {
            var result = new ValidationResult();

            if (value == null)
            {
                result.AddError("検証対象のオブジェクトがnullです。", "Object", "ObjectNull");
                return result;
            }

            if (!_predicate(value))
            {
                result.AddError(ErrorMessage, PropertyName, ErrorCode);
            }

            return result;
        }
    }

}
