using CoreLib.Utilities.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation.Extensions
{
    /// <summary>
    /// 検証に関する拡張メソッド
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// 文字列が空でないことを検証
        /// </summary>
        public static ValidationResult ValidateNotEmpty(this string? value, string? propertyName = null, string? errorMessage = null)
        {
            var validator = StringValidator.NotEmpty();

            if (!string.IsNullOrEmpty(errorMessage))
                validator.WithMessage(errorMessage);

            var result = validator.Validate(value);

            if (!string.IsNullOrEmpty(propertyName) && !result.IsValid)
                result.Errors[0] = new ValidationError(result.Errors[0].Message, propertyName, result.Errors[0].ErrorCode);

            return result;
        }

        /// <summary>
        /// 文字列が有効なメールアドレス形式であることを検証
        /// </summary>
        public static ValidationResult ValidateEmail(this string? value, string? propertyName = null, string? errorMessage = null)
        {
            var validator = StringValidator.Email();

            if (!string.IsNullOrEmpty(errorMessage))
                validator.WithMessage(errorMessage);

            var result = validator.Validate(value);

            if (!string.IsNullOrEmpty(propertyName) && !result.IsValid)
                result.Errors[0] = new ValidationError(result.Errors[0].Message, propertyName, result.Errors[0].ErrorCode);

            return result;
        }

        /// <summary>
        /// 文字列が有効な日本の郵便番号形式であることを検証
        /// </summary>
        public static ValidationResult ValidateJapanesePostalCode(this string? value, string? propertyName = null, string? errorMessage = null)
        {
            var validator = StringValidator.JapanesePostalCode();

            if (!string.IsNullOrEmpty(errorMessage))
                validator.WithMessage(errorMessage);

            var result = validator.Validate(value);

            if (!string.IsNullOrEmpty(propertyName) && !result.IsValid)
                result.Errors[0] = new ValidationError(result.Errors[0].Message, propertyName, result.Errors[0].ErrorCode);

            return result;
        }

        /// <summary>
        /// 文字列が有効な日本の電話番号形式であることを検証
        /// </summary>
        public static ValidationResult ValidateJapanesePhoneNumber(this string? value, string? propertyName = null, string? errorMessage = null)
        {
            var validator = StringValidator.JapanesePhoneNumber();

            if (!string.IsNullOrEmpty(errorMessage))
                validator.WithMessage(errorMessage);

            var result = validator.Validate(value);

            if (!string.IsNullOrEmpty(propertyName) && !result.IsValid)
                result.Errors[0] = new ValidationError(result.Errors[0].Message, propertyName, result.Errors[0].ErrorCode);

            return result;
        }

        /// <summary>
        /// 数値が指定された範囲内であることを検証
        /// </summary>
        public static ValidationResult ValidateInRange<T>(this T value, T min, T max, string? propertyName = null, string? errorMessage = null)
            where T : IComparable<T>
        {
            var validator = NumberValidator<T>.InRange(min, max);

            if (!string.IsNullOrEmpty(errorMessage))
                validator.WithMessage(errorMessage);

            var result = validator.Validate(value);

            if (!string.IsNullOrEmpty(propertyName) && !result.IsValid)
                result.Errors[0] = new ValidationError(result.Errors[0].Message, propertyName, result.Errors[0].ErrorCode);

            return result;
        }

        /// <summary>
        /// コレクションが空でないことを検証
        /// </summary>
        public static ValidationResult ValidateNotEmpty<T>(this IEnumerable<T>? collection, string? propertyName = null, string? errorMessage = null)
        {
            var result = new ValidationResult();

            if (collection == null || !collection.Any())
            {
                var message = errorMessage ?? "コレクションが空です。";
                var propertyNameToUse = propertyName ?? "Collection";
                result.AddError(message, propertyNameToUse, "CollectionEmpty");
            }

            return result;
        }

        /// <summary>
        /// オブジェクトがnullでないことを検証
        /// </summary>
        public static ValidationResult ValidateNotNull<T>(this T? value, string? propertyName = null, string? errorMessage = null)
            where T : class
        {
            var result = new ValidationResult();

            if (value == null)
            {
                var message = errorMessage ?? "値がnullです。";
                var propertyNameToUse = propertyName ?? "Value";
                result.AddError(message, propertyNameToUse, "ValueIsNull");
            }

            return result;
        }
    }
}
