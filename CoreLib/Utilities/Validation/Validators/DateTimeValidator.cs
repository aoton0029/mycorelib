using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation.Validators
{
    /// <summary>
    /// 日付・時刻の検証クラス
    /// </summary>
    public class DateTimeValidator : IValidator<DateTime>
    {
        private readonly Func<DateTime, bool> _validationFunc;
        private string _message = "日付が無効です。";
        private string _errorCode = "InvalidDate";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DateTimeValidator(Func<DateTime, bool> validationFunc)
        {
            _validationFunc = validationFunc ?? throw new ArgumentNullException(nameof(validationFunc));
        }

        /// <summary>
        /// 日付を検証
        /// </summary>
        public ValidationResult Validate(DateTime value)
        {
            var result = new ValidationResult();

            if (!_validationFunc(value))
            {
                result.AddError(_message, "Value", _errorCode);
            }

            return result;
        }

        /// <summary>
        /// Nullable型の日付を検証
        /// </summary>
        public ValidationResult Validate(DateTime? value)
        {
            var result = new ValidationResult();

            if (value == null)
            {
                result.AddError("日付が指定されていません。", "Value", "DateRequired");
                return result;
            }

            return Validate(value.Value);
        }

        /// <summary>
        /// 文字列形式の日付を検証
        /// </summary>
        public ValidationResult Validate(string? dateString, string format = "yyyy/MM/dd")
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(dateString))
            {
                result.AddError("日付が指定されていません。", "Value", "DateRequired");
                return result;
            }

            if (!DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parsedDate))
            {
                result.AddError($"日付の形式が無効です。形式: {format}", "Value", "InvalidDateFormat");
                return result;
            }

            return Validate(parsedDate);
        }

        /// <summary>
        /// エラーメッセージを設定
        /// </summary>
        public DateTimeValidator WithMessage(string message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        public DateTimeValidator WithErrorCode(string errorCode)
        {
            _errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            return this;
        }

        #region Factory Methods

        /// <summary>
        /// 過去日であることを検証
        /// </summary>
        public static DateTimeValidator InPast(DateTime? compareDate = null)
        {
            var reference = compareDate ?? DateTime.Now;
            return new DateTimeValidator(date => date < reference)
                .WithMessage($"{reference.ToString("yyyy/MM/dd")} より前の日付である必要があります。")
                .WithErrorCode("DateNotInPast");
        }

        /// <summary>
        /// 未来日であることを検証
        /// </summary>
        public static DateTimeValidator InFuture(DateTime? compareDate = null)
        {
            var reference = compareDate ?? DateTime.Now;
            return new DateTimeValidator(date => date > reference)
                .WithMessage($"{reference.ToString("yyyy/MM/dd")} より後の日付である必要があります。")
                .WithErrorCode("DateNotInFuture");
        }

        /// <summary>
        /// 指定された日付範囲内であることを検証
        /// </summary>
        public static DateTimeValidator Between(DateTime minDate, DateTime maxDate)
        {
            return new DateTimeValidator(date => date >= minDate && date <= maxDate)
                .WithMessage($"日付は {minDate.ToString("yyyy/MM/dd")} から {maxDate.ToString("yyyy/MM/dd")} の間である必要があります。")
                .WithErrorCode("DateOutOfRange");
        }

        /// <summary>
        /// 特定の曜日であることを検証
        /// </summary>
        public static DateTimeValidator OnDayOfWeek(DayOfWeek dayOfWeek)
        {
            return new DateTimeValidator(date => date.DayOfWeek == dayOfWeek)
                .WithMessage($"日付は {dayOfWeek} である必要があります。")
                .WithErrorCode("InvalidDayOfWeek");
        }

        /// <summary>
        /// 平日であることを検証
        /// </summary>
        public static DateTimeValidator OnWeekday()
        {
            return new DateTimeValidator(date =>
                date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday)
                .WithMessage("日付は平日である必要があります。")
                .WithErrorCode("NotWeekday");
        }

        /// <summary>
        /// 週末であることを検証
        /// </summary>
        public static DateTimeValidator OnWeekend()
        {
            return new DateTimeValidator(date =>
                date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday)
                .WithMessage("日付は週末である必要があります。")
                .WithErrorCode("NotWeekend");
        }

        /// <summary>
        /// 年齢が指定された範囲内であることを検証
        /// </summary>
        public static DateTimeValidator AgeInRange(int minAge, int maxAge, DateTime? referenceDate = null)
        {
            var reference = referenceDate ?? DateTime.Today;
            return new DateTimeValidator(birthDate =>
            {
                int age = reference.Year - birthDate.Year;
                if (birthDate.Date > reference.AddYears(-age))
                    age--;

                return age >= minAge && age <= maxAge;
            })
            .WithMessage($"年齢は {minAge} 歳から {maxAge} 歳の間である必要があります。")
            .WithErrorCode("AgeOutOfRange");
        }

        /// <summary>
        /// 有効な日付であることを検証
        /// </summary>
        public static DateTimeValidator IsValid()
        {
            return new DateTimeValidator(_ => true)
                .WithMessage("有効な日付である必要があります。")
                .WithErrorCode("InvalidDate");
        }

        #endregion
    }
}
