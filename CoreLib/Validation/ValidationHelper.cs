using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// 一般的な検証機能を提供するヘルパークラス
    /// </summary>
    public static class ValidationHelper
    {
        // メールアドレス検証の正規表現
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
            RegexOptions.Compiled);

        // 日本の郵便番号検証の正規表現（例: 123-4567）
        private static readonly Regex JapanesePostalCodeRegex = new Regex(
            @"^\d{3}-?\d{4}$",
            RegexOptions.Compiled);

        // 日本の電話番号検証の正規表現
        private static readonly Regex JapanesePhoneNumberRegex = new Regex(
            @"^0\d{1,4}-?\d{1,4}-?\d{4}$",
            RegexOptions.Compiled);

        // クレジットカード番号検証の正規表現
        private static readonly Regex CreditCardNumberRegex = new Regex(
            @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})$",
            RegexOptions.Compiled);

        #region 文字列検証

        /// <summary>
        /// 文字列が空でないか検証
        /// </summary>
        public static bool IsNotEmpty(string? value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 文字列が空白を含まず空でないか検証
        /// </summary>
        public static bool IsNotWhiteSpace(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 文字列が指定された長さ以内か検証
        /// </summary>
        public static bool HasMaxLength(string? value, int maxLength)
        {
            if (value == null)
                return true;

            return value.Length <= maxLength;
        }

        /// <summary>
        /// 文字列が指定された長さ以上か検証
        /// </summary>
        public static bool HasMinLength(string? value, int minLength)
        {
            if (value == null)
                return minLength == 0;

            return value.Length >= minLength;
        }

        /// <summary>
        /// 文字列が正規表現パターンに一致するか検証
        /// </summary>
        public static bool MatchesPattern(string? value, string pattern)
        {
            if (value == null)
                return false;

            return Regex.IsMatch(value, pattern);
        }

        #endregion

        #region 数値検証

        /// <summary>
        /// 数値が指定された範囲内にあるか検証
        /// </summary>
        public static bool IsInRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        /// <summary>
        /// 値が指定された値より大きいか検証
        /// </summary>
        public static bool IsGreaterThan<T>(T value, T min) where T : IComparable<T>
        {
            return value.CompareTo(min) > 0;
        }

        /// <summary>
        /// 値が指定された値以上か検証
        /// </summary>
        public static bool IsGreaterThanOrEqual<T>(T value, T min) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0;
        }

        /// <summary>
        /// 値が指定された値より小さいか検証
        /// </summary>
        public static bool IsLessThan<T>(T value, T max) where T : IComparable<T>
        {
            return value.CompareTo(max) < 0;
        }

        /// <summary>
        /// 値が指定された値以下か検証
        /// </summary>
        public static bool IsLessThanOrEqual<T>(T value, T max) where T : IComparable<T>
        {
            return value.CompareTo(max) <= 0;
        }

        #endregion

        #region その他の検証

        /// <summary>
        /// 値がnullでないか検証
        /// </summary>
        public static bool IsNotNull<T>(T? value) where T : class
        {
            return value != null;
        }

        /// <summary>
        /// コレクションが空でないか検証
        /// </summary>
        public static bool IsNotEmpty<T>(IEnumerable<T>? collection)
        {
            return collection != null && collection.Any();
        }

        /// <summary>
        /// メールアドレスの形式が正しいか検証
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// 日本の郵便番号の形式が正しいか検証
        /// </summary>
        public static bool IsValidJapanesePostalCode(string? postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;

            return JapanesePostalCodeRegex.IsMatch(postalCode);
        }

        /// <summary>
        /// 日本の電話番号の形式が正しいか検証
        /// </summary>
        public static bool IsValidJapanesePhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            return JapanesePhoneNumberRegex.IsMatch(phoneNumber);
        }

        /// <summary>
        /// クレジットカード番号の形式が正しいか検証（Luhnアルゴリズム）
        /// </summary>
        public static bool IsValidCreditCardNumber(string? cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            // 数字以外の文字を削除
            cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

            if (!CreditCardNumberRegex.IsMatch(cardNumber))
                return false;

            // Luhnアルゴリズムでチェックサムを検証
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n = (n % 10) + 1;
                    }
                }

                sum += n;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// 日付が過去の日付かどうか検証
        /// </summary>
        public static bool IsPastDate(DateTime date)
        {
            return date < DateTime.Now;
        }

        /// <summary>
        /// 日付が未来の日付かどうか検証
        /// </summary>
        public static bool IsFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        /// <summary>
        /// URLが有効な形式かどうか検証
        /// </summary>
        public static bool IsValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        #endregion

        #region 検証結果の組み合わせ

        /// <summary>
        /// 複数の検証結果を結合
        /// </summary>
        public static ValidationResult Combine(params ValidationResult[] results)
        {
            var combined = new ValidationResult();
            foreach (var result in results)
            {
                if (result != null && !result.IsValid)
                {
                    combined.AddErrors(result);
                }
            }
            return combined;
        }

        /// <summary>
        /// すべての条件が満たされている場合のみ成功する検証結果を返す
        /// </summary>
        public static ValidationResult All(params Func<ValidationResult>[] validators)
        {
            var result = new ValidationResult();
            foreach (var validator in validators)
            {
                var validationResult = validator();
                if (!validationResult.IsValid)
                {
                    result.AddErrors(validationResult);
                }
            }
            return result;
        }

        /// <summary>
        /// いずれかの条件が満たされている場合に成功する検証結果を返す
        /// </summary>
        public static ValidationResult Any(params Func<ValidationResult>[] validators)
        {
            foreach (var validator in validators)
            {
                var result = validator();
                if (result.IsValid)
                {
                    return result;
                }
            }

            var errorResult = new ValidationResult();
            errorResult.AddError("いずれの条件も満たされていません。");
            return errorResult;
        }

        #endregion
    }
}
