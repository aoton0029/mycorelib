using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation.Validators
{
    /// <summary>
    /// 数値の検証を行うバリデーター
    /// </summary>
    /// <typeparam name="T">数値型</typeparam>
    public class NumberValidator<T> : IValidator<T> where T : IComparable<T>
    {
        private readonly Func<T, bool> _predicate;
        private string _errorMessage = "数値が検証条件を満たしていません。";
        private string _errorCode = "NumberValidationFailed";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="predicate">検証条件</param>
        public NumberValidator(Func<T, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// 数値の検証を実行
        /// </summary>
        public ValidationResult Validate(T value)
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
        /// 非同期で数値の検証を実行
        /// </summary>
        public Task<ValidationResult> ValidateAsync(T value)
        {
            return Task.FromResult(Validate(value));
        }

        /// <summary>
        /// エラーメッセージを設定
        /// </summary>
        public IValidator<T> WithMessage(string errorMessage)
        {
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        public IValidator<T> WithErrorCode(string errorCode)
        {
            _errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            return this;
        }

        #region Factory Methods

        /// <summary>
        /// 値が指定された範囲内であることを検証
        /// </summary>
        public static NumberValidator<T> InRange(T min, T max)
        {
            return (NumberValidator<T>)new NumberValidator<T>(value => ValidationHelper.IsInRange(value, min, max))
                .WithMessage($"値が範囲外です。{min}から{max}の間である必要があります。")
                .WithErrorCode("NumberOutOfRange");
        }

        /// <summary>
        /// 値が指定された値より大きいことを検証
        /// </summary>
        public static NumberValidator<T> GreaterThan(T min)
        {
            return (NumberValidator<T>)new NumberValidator<T>(value => ValidationHelper.IsGreaterThan(value, min))
                .WithMessage($"値が{min}より大きい必要があります。")
                .WithErrorCode("NumberTooSmall");
        }

        /// <summary>
        /// 値が指定された値以上であることを検証
        /// </summary>
        public static NumberValidator<T> GreaterThanOrEqual(T min)
        {
            return (NumberValidator<T>)new NumberValidator<T>(value => ValidationHelper.IsGreaterThanOrEqual(value, min))
                .WithMessage($"値が{min}以上である必要があります。")
                .WithErrorCode("NumberTooSmall");
        }

        /// <summary>
        /// 値が指定された値より小さいことを検証
        /// </summary>
        public static NumberValidator<T> LessThan(T max)
        {
            return (NumberValidator<T>)new NumberValidator<T>(value => ValidationHelper.IsLessThan(value, max))
                .WithMessage($"値が{max}より小さい必要があります。")
                .WithErrorCode("NumberTooLarge");
        }

        /// <summary>
        /// 値が指定された値以下であることを検証
        /// </summary>
        public static NumberValidator<T> LessThanOrEqual(T max)
        {
            return (NumberValidator<T>)new NumberValidator<T>(value => ValidationHelper.IsLessThanOrEqual(value, max))
                .WithMessage($"値が{max}以下である必要があります。")
                .WithErrorCode("NumberTooLarge");
        }

        #endregion
    }
}
