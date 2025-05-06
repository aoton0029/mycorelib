using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation.Validators
{
    /// <summary>
    /// 文字列の検証を行うバリデーター
    /// </summary>
    public class StringValidator : IValidator<string?>
    {
        private readonly Func<string?, bool> _predicate;
        private string _errorMessage = "文字列が検証条件を満たしていません。";
        private string _errorCode = "StringValidationFailed";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="predicate">検証条件</param>
        public StringValidator(Func<string?, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// 文字列の検証を実行
        /// </summary>
        public ValidationResult Validate(string? value)
        {
            if (_predicate(value))
            {
                return ValidationResult.Success();
            }
            else
            {
                var result = new ValidationResult();
                result.AddError(new ValidationError(_errorMessage, "Value", _errorCode));
                return result;
            }
        }

        /// <summary>
        /// 非同期で文字列の検証を実行
        /// </summary>
        public Task<ValidationResult> ValidateAsync(string? value)
        {
            return Task.FromResult(Validate(value));
        }

        /// <summary>
        /// エラーメッセージを設定
        /// </summary>
        public IValidator<string?> WithMessage(string errorMessage)
        {
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        public IValidator<string?> WithErrorCode(string errorCode)
        {
            _errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            return this;
        }

        #region Factory Methods

        /// <summary>
        /// 文字列が空でないことを検証
        /// </summary>
        public static StringValidator NotEmpty()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsNotEmpty)
                .WithMessage("値が空です。")
                .WithErrorCode("StringEmpty");
        }

        /// <summary>
        /// 文字列が空白文字を含まず空でないことを検証
        /// </summary>
        public static StringValidator NotWhiteSpace()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsNotWhiteSpace)
                .WithMessage("値が空白文字のみです。")
                .WithErrorCode("StringWhiteSpace");
        }

        /// <summary>
        /// 文字列の最大長を検証
        /// </summary>
        public static StringValidator MaxLength(int maxLength)
        {
            return (StringValidator)new StringValidator(value => ValidationHelper.HasMaxLength(value, maxLength))
                .WithMessage($"値の長さが{maxLength}文字を超えています。")
                .WithErrorCode("StringTooLong");
        }

        /// <summary>
        /// 文字列の最小長を検証
        /// </summary>
        public static StringValidator MinLength(int minLength)
        {
            return (StringValidator)new StringValidator(value => ValidationHelper.HasMinLength(value, minLength))
                .WithMessage($"値の長さが{minLength}文字未満です。")
                .WithErrorCode("StringTooShort");
        }

        /// <summary>
        /// 文字列が正規表現パターンに一致することを検証
        /// </summary>
        public static StringValidator Matches(string pattern)
        {
            return (StringValidator)new StringValidator(value => ValidationHelper.MatchesPattern(value, pattern))
                .WithMessage("値が指定されたパターンに一致しません。")
                .WithErrorCode("PatternMismatch");
        }

        /// <summary>
        /// 文字列が正規表現パターンに一致することを検証
        /// </summary>
        public static StringValidator Matches(Regex regex)
        {
            return (StringValidator)new StringValidator(value => value != null && regex.IsMatch(value))
                .WithMessage("値が指定されたパターンに一致しません。")
                .WithErrorCode("PatternMismatch");
        }

        /// <summary>
        /// メールアドレスの形式を検証
        /// </summary>
        public static StringValidator Email()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsValidEmail)
                .WithMessage("有効なメールアドレス形式ではありません。")
                .WithErrorCode("InvalidEmail");
        }

        /// <summary>
        /// 日本の郵便番号の形式を検証
        /// </summary>
        public static StringValidator JapanesePostalCode()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsValidJapanesePostalCode)
                .WithMessage("有効な郵便番号形式ではありません。")
                .WithErrorCode("InvalidPostalCode");
        }

        /// <summary>
        /// 日本の電話番号の形式を検証
        /// </summary>
        public static StringValidator JapanesePhoneNumber()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsValidJapanesePhoneNumber)
                .WithMessage("有効な電話番号形式ではありません。")
                .WithErrorCode("InvalidPhoneNumber");
        }

        /// <summary>
        /// URLの形式を検証
        /// </summary>
        public static StringValidator Url()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsValidUrl)
                .WithMessage("有効なURL形式ではありません。")
                .WithErrorCode("InvalidUrl");
        }

        /// <summary>
        /// クレジットカード番号の形式を検証
        /// </summary>
        public static StringValidator CreditCardNumber()
        {
            return (StringValidator)new StringValidator(ValidationHelper.IsValidCreditCardNumber)
                .WithMessage("有効なクレジットカード番号ではありません。")
                .WithErrorCode("InvalidCreditCardNumber");
        }

        #endregion
    }
}
