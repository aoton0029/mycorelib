using CoreLib.Utilities.Validation;
using CoreLib.Utilities.Validation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Validation.Validators
{
    /// <summary>
    /// モデルクラスに対するバリデーションを提供するクラス
    /// </summary>
    /// <typeparam name="T">検証対象のモデル型</typeparam>
    public class ModelValidator<T> where T : class
    {
        private readonly List<ValidationRule<T>> _rules = new List<ValidationRule<T>>();

        /// <summary>
        /// バリデーションルールを追加
        /// </summary>
        /// <param name="rule">追加するルール</param>
        /// <returns>このバリデーターインスタンス</returns>
        public ModelValidator<T> AddRule(ValidationRule<T> rule)
        {
            _rules.Add(rule ?? throw new ArgumentNullException(nameof(rule)));
            return this;
        }

        /// <summary>
        /// 文字列プロパティに対するバリデーションルールを追加
        /// </summary>
        /// <param name="propertySelector">プロパティを選択する式</param>
        /// <returns>バリデーションルールビルダー</returns>
        public StringPropertyRuleBuilder<T> RuleForString<TProperty>(Expression<Func<T, string?>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return new StringPropertyRuleBuilder<T>(this, propertyName, propertySelector);
        }

        /// <summary>
        /// 数値プロパティに対するバリデーションルールを追加
        /// </summary>
        /// <param name="propertySelector">プロパティを選択する式</param>
        /// <returns>バリデーションルールビルダー</returns>
        public NumberPropertyRuleBuilder<T, TProperty> RuleForNumber<TProperty>(Expression<Func<T, TProperty>> propertySelector)
            where TProperty : struct, IComparable<TProperty>
        {
            var propertyName = GetPropertyName(propertySelector);
            return new NumberPropertyRuleBuilder<T, TProperty>(this, propertyName, propertySelector);
        }

        ///// <summary>
        ///// 日付プロパティに対するバリデーションルールを追加
        ///// </summary>
        ///// <param name="propertySelector">プロパティを選択する式</param>
        ///// <returns>バリデーションルールビルダー</returns>
        //public DateTimePropertyRuleBuilder<T> RuleForDateTime(Expression<Func<T, DateTime>> propertySelector)
        //{
        //    var propertyName = GetPropertyName(propertySelector);
        //    return new DateTimePropertyRuleBuilder<T>(this, propertyName, propertySelector);
        //}

        ///// <summary>
        ///// Nullable日付プロパティに対するバリデーションルールを追加
        ///// </summary>
        ///// <param name="propertySelector">プロパティを選択する式</param>
        ///// <returns>バリデーションルールビルダー</returns>
        //public NullableDateTimePropertyRuleBuilder<T> RuleForNullableDateTime(Expression<Func<T, DateTime?>> propertySelector)
        //{
        //    var propertyName = GetPropertyName(propertySelector);
        //    return new NullableDateTimePropertyRuleBuilder<T>(this, propertyName, propertySelector);
        //}

        /// <summary>
        /// カスタムルールを追加
        /// </summary>
        /// <param name="predicate">検証条件</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        /// <returns>このバリデーターインスタンス</returns>
        public ModelValidator<T> AddCustomRule(Func<T, bool> predicate, string errorMessage, string errorCode = "ValidationFailed")
        {
            _rules.Add(new CustomValidationRule<T>(predicate, errorMessage, errorCode));
            return this;
        }

        /// <summary>
        /// モデルを検証
        /// </summary>
        /// <param name="model">検証対象のモデル</param>
        /// <returns>検証結果</returns>
        public ValidationResult Validate(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var result = new ValidationResult();

            foreach (var rule in _rules)
            {
                var ruleResult = rule.Validate(model);
                result.AddErrors(ruleResult);
            }

            return result;
        }

        /// <summary>
        /// プロパティ名を取得
        /// </summary>
        private static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("式はプロパティを指定している必要があります", nameof(propertySelector));
        }
    }

    /// <summary>
    /// 文字列プロパティに対するバリデーションルールビルダー
    /// </summary>
    /// <typeparam name="T">モデルの型</typeparam>
    public class StringPropertyRuleBuilder<T> where T : class
    {
        private readonly ModelValidator<T> _validator;
        private readonly string _propertyName;
        private readonly Expression<Func<T, string?>> _propertySelector;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StringPropertyRuleBuilder(ModelValidator<T> validator, string propertyName, Expression<Func<T, string?>> propertySelector)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
        }

        /// <summary>
        /// 空でないことを検証
        /// </summary>
        public StringPropertyRuleBuilder<T> NotEmpty(string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.IsNotEmpty(value),
                errorMessage ?? $"{_propertyName}は必須です。",
                "Required"));
            return this;
        }

        /// <summary>
        /// 空白文字でないことを検証
        /// </summary>
        public StringPropertyRuleBuilder<T> NotWhiteSpace(string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.IsNotWhiteSpace(value),
                errorMessage ?? $"{_propertyName}は空白文字のみではいけません。",
                "WhiteSpace"));
            return this;
        }

        /// <summary>
        /// 最大長を検証
        /// </summary>
        public StringPropertyRuleBuilder<T> MaxLength(int maxLength, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.HasMaxLength(value, maxLength),
                errorMessage ?? $"{_propertyName}は{maxLength}文字以内である必要があります。",
                "MaxLength"));
            return this;
        }

        /// <summary>
        /// 最小長を検証
        /// </summary>
        public StringPropertyRuleBuilder<T> MinLength(int minLength, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.HasMinLength(value, minLength),
                errorMessage ?? $"{_propertyName}は{minLength}文字以上である必要があります。",
                "MinLength"));
            return this;
        }

        /// <summary>
        /// メールアドレス形式を検証
        /// </summary>
        public StringPropertyRuleBuilder<T> EmailAddress(string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.IsValidEmail(value),
                errorMessage ?? $"{_propertyName}は有効なメールアドレス形式である必要があります。",
                "InvalidEmail"));
            return this;
        }

        /// <summary>
        /// 郵便番号形式を検証
        /// </summary>
        public StringPropertyRuleBuilder<T> JapanesePostalCode(string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.IsValidJapanesePostalCode(value),
                errorMessage ?? $"{_propertyName}は有効な郵便番号形式である必要があります。",
                "InvalidPostalCode"));
            return this;
        }

        /// <summary>
        /// 電話番号形式を検証
        /// </summary>
        public StringPropertyRuleBuilder<T> JapanesePhoneNumber(string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.IsValidJapanesePhoneNumber(value),
                errorMessage ?? $"{_propertyName}は有効な電話番号形式である必要があります。",
                "InvalidPhoneNumber"));
            return this;
        }

        /// <summary>
        /// 正規表現パターンに一致することを検証
        /// </summary>
        public StringPropertyRuleBuilder<T> Matches(string pattern, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                value => ValidationHelper.MatchesPattern(value, pattern),
                errorMessage ?? $"{_propertyName}は指定されたパターンに一致する必要があります。",
                "PatternMismatch"));
            return this;
        }

        /// <summary>
        /// カスタム検証を追加
        /// </summary>
        public StringPropertyRuleBuilder<T> Custom(Func<string?, bool> predicate, string errorMessage, string errorCode = "ValidationFailed")
        {
            _validator.AddRule(new PropertyValidationRule<T, string?>(
                _propertySelector,
                predicate,
                errorMessage,
                errorCode));
            return this;
        }
    }

    /// <summary>
    /// 数値プロパティに対するバリデーションルールビルダー
    /// </summary>
    public class NumberPropertyRuleBuilder<T, TProperty> where T : class where TProperty : struct, IComparable<TProperty>
    {
        private readonly ModelValidator<T> _validator;
        private readonly string _propertyName;
        private readonly Expression<Func<T, TProperty>> _propertySelector;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NumberPropertyRuleBuilder(ModelValidator<T> validator, string propertyName, Expression<Func<T, TProperty>> propertySelector)
        {
            _validator = validator;
            _propertyName = propertyName;
            _propertySelector = propertySelector;
        }

        /// <summary>
        /// 最小値を検証
        /// </summary>
        public NumberPropertyRuleBuilder<T, TProperty> GreaterThanOrEqual(TProperty minValue, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, TProperty>(
                _propertySelector,
                value => value.CompareTo(minValue) >= 0,
                errorMessage ?? $"{_propertyName}は{minValue}以上である必要があります。",
                "MinValue"));
            return this;
        }

        /// <summary>
        /// 最大値を検証
        /// </summary>
        public NumberPropertyRuleBuilder<T, TProperty> LessThanOrEqual(TProperty maxValue, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, TProperty>(
                _propertySelector,
                value => value.CompareTo(maxValue) <= 0,
                errorMessage ?? $"{_propertyName}は{maxValue}以下である必要があります。",
                "MaxValue"));
            return this;
        }

        /// <summary>
        /// 範囲を検証
        /// </summary>
        public NumberPropertyRuleBuilder<T, TProperty> Between(TProperty minValue, TProperty maxValue, string? errorMessage = null)
        {
            _validator.AddRule(new PropertyValidationRule<T, TProperty>(
                _propertySelector,
                value => value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0,
                errorMessage ?? $"{_propertyName}は{minValue}から{maxValue}の間である必要があります。",
                "Range"));
            return this;
        }

        /// <summary>
        /// カスタム検証を追加
        /// </summary>
        public NumberPropertyRuleBuilder<T, TProperty> Custom(Func<TProperty, bool> predicate, string errorMessage, string errorCode = "ValidationFailed")
        {
            _validator.AddRule(new PropertyValidationRule<T, TProperty>(
                _propertySelector,
                predicate,
                errorMessage,
                errorCode));
            return this;
        }
    }


    public class _Sample
    {
        // モデルクラスの例
        public class UserModel
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public int Age { get; set; }
            public DateTime? BirthDate { get; set; }
        }

        // バリデータークラスの例
        public class UserValidator
        {
            private readonly ModelValidator<UserModel> _validator;

            public UserValidator()
            {
                _validator = new ModelValidator<UserModel>();

                // 文字列プロパティのバリデーション
                _validator.RuleForString<UserModel>(u => u.Username)
                    .NotEmpty("ユーザー名は必須です。")
                    .MinLength(3, "ユーザー名は3文字以上である必要があります。")
                    .MaxLength(50, "ユーザー名は50文字以内である必要があります。");

                _validator.RuleForString<UserModel>(u => u.Email)
                    .NotEmpty("メールアドレスは必須です。")
                    .EmailAddress("有効なメールアドレス形式である必要があります。");

                _validator.RuleForString<UserModel>(u => u.PhoneNumber)
                    .JapanesePhoneNumber("有効な電話番号形式である必要があります。");

                // 数値プロパティのバリデーション
                _validator.RuleForNumber(u => u.Age)
                    .GreaterThanOrEqual(0, "年齢は0以上である必要があります。")
                    .LessThanOrEqual(120, "年齢は120以下である必要があります。");

                // カスタムバリデーション
                _validator.AddCustomRule(
                    u => u.Username != "admin" || u.Email!.EndsWith("@example.com"),
                    "admin ユーザーは example.com のメールアドレスを使用する必要があります。",
                    "AdminEmailRestriction");
            }

            public ValidationResult Validate(UserModel user)
            {
                return _validator.Validate(user);
            }
        }
    }
}
