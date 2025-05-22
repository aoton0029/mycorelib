

すでに`CoreLib/Utilities/Validation`ディレクトリには複数のバリデーション関連のクラスが存在しています。既存のバリデーションの仕組みを理解した上で、新たにクラスのバリデーションを作成しましょう。

クラス全体を検証するためのクラスバリデーターを以下のように作成します：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// クラスのバリデーションを行うクラス
    /// </summary>
    /// <typeparam name="T">検証対象クラスの型</typeparam>
    public class ClassValidator<T> : IValidator<T> where T : class
    {
        private readonly List<IPropertyValidator<T>> _propertyValidators = new List<IPropertyValidator<T>>();
        private string _errorMessage = "クラスの検証に失敗しました。";
        private string _errorCode = "ClassValidationFailed";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClassValidator()
        {
        }

        /// <summary>
        /// プロパティのバリデーションルールを追加
        /// </summary>
        /// <typeparam name="TProperty">プロパティの型</typeparam>
        /// <param name="propertySelector">検証対象のプロパティを選択する式</param>
        /// <returns>プロパティバリデータ</returns>
        public PropertyValidator<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            var validator = new PropertyValidator<T, TProperty>(propertySelector);
            _propertyValidators.Add(validator);
            return validator;
        }

        /// <summary>
        /// インスタンスの検証を実行
        /// </summary>
        /// <param name="value">検証対象インスタンス</param>
        /// <returns>検証結果</returns>
        public ValidationResult Validate(T value)
        {
            if (value == null)
            {
                return ValidationResult.Error("検証対象のインスタンスがnullです。");
            }

            var result = new ValidationResult();

            foreach (var validator in _propertyValidators)
            {
                var propertyResult = validator.Validate(value);
                if (!propertyResult.IsValid)
                {
                    result.AddErrors(propertyResult);
                }
            }

            return result;
        }

        /// <summary>
        /// 非同期でインスタンスの検証を実行
        /// </summary>
        /// <param name="value">検証対象インスタンス</param>
        /// <returns>検証結果を含むタスク</returns>
        public async Task<ValidationResult> ValidateAsync(T value)
        {
            if (value == null)
            {
                return ValidationResult.Error("検証対象のインスタンスがnullです。");
            }

            var result = new ValidationResult();
            var tasks = new List<Task<ValidationResult>>();

            foreach (var validator in _propertyValidators)
            {
                tasks.Add(validator.ValidateAsync(value));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var propertyResult in results)
            {
                if (!propertyResult.IsValid)
                {
                    result.AddErrors(propertyResult);
                }
            }

            return result;
        }

        /// <summary>
        /// エラーメッセージを設定
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>このバリデーターインスタンス</returns>
        public IValidator<T> WithMessage(string errorMessage)
        {
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <returns>このバリデーターインスタンス</returns>
        public IValidator<T> WithErrorCode(string errorCode)
        {
            _errorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            return this;
        }
    }

    /// <summary>
    /// プロパティバリデーションのインターフェース
    /// </summary>
    /// <typeparam name="T">検証対象クラスの型</typeparam>
    public interface IPropertyValidator<T> where T : class
    {
        ValidationResult Validate(T value);
        Task<ValidationResult> ValidateAsync(T value);
    }

    /// <summary>
    /// プロパティバリデーション実装クラス
    /// </summary>
    /// <typeparam name="T">検証対象クラスの型</typeparam>
    /// <typeparam name="TProperty">プロパティの型</typeparam>
    public class PropertyValidator<T, TProperty> : IPropertyValidator<T> where T : class
    {
        private readonly Expression<Func<T, TProperty>> _propertySelector;
        private readonly List<Func<TProperty, ValidationResult>> _validators = new List<Func<TProperty, ValidationResult>>();
        private string _propertyName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="propertySelector">プロパティセレクタ</param>
        public PropertyValidator(Expression<Func<T, TProperty>> propertySelector)
        {
            _propertySelector = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));
            _propertyName = GetPropertyName(propertySelector);
        }

        /// <summary>
        /// プロパティが空でないことを検証
        /// </summary>
        public PropertyValidator<T, TProperty> NotNull()
        {
            _validators.Add(value =>
            {
                if (value == null)
                {
                    var result = new ValidationResult();
                    result.AddError($"'{_propertyName}' は必須項目です。", _propertyName, "PropertyRequired");
                    return result;
                }
                return ValidationResult.Success();
            });
            return this;
        }

        /// <summary>
        /// カスタム検証を追加
        /// </summary>
        /// <param name="validator">検証関数</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        public PropertyValidator<T, TProperty> Must(Func<TProperty, bool> validator, string errorMessage, string errorCode = "ValidationFailed")
        {
            _validators.Add(value =>
            {
                if (!validator(value))
                {
                    var result = new ValidationResult();
                    result.AddError(errorMessage, _propertyName, errorCode);
                    return result;
                }
                return ValidationResult.Success();
            });
            return this;
        }

        /// <summary>
        /// バリデータを追加
        /// </summary>
        public PropertyValidator<T, TProperty> SetValidator(IValidator<TProperty> validator)
        {
            _validators.Add(value =>
            {
                var result = validator.Validate(value);
                if (!result.IsValid && result.Errors.Any())
                {
                    // プロパティ名を更新
                    foreach (var error in result.Errors)
                    {
                        if (string.IsNullOrEmpty(error.PropertyName))
                        {
                            error.PropertyName = _propertyName;
                        }
                    }
                }
                return result;
            });
            return this;
        }

        /// <summary>
        /// 検証を実行
        /// </summary>
        public ValidationResult Validate(T value)
        {
            if (value == null)
            {
                var result = new ValidationResult();
                result.AddError("検証対象のインスタンスがnullです。", _propertyName);
                return result;
            }

            var propertyValue = _propertySelector.Compile()(value);
            var validationResult = new ValidationResult();

            foreach (var validator in _validators)
            {
                var result = validator(propertyValue);
                if (!result.IsValid)
                {
                    validationResult.AddErrors(result);
                }
            }

            return validationResult;
        }

        /// <summary>
        /// 非同期で検証を実行
        /// </summary>
        public async Task<ValidationResult> ValidateAsync(T value)
        {
            // 同期実装を使用
            return await Task.FromResult(Validate(value));
        }

        /// <summary>
        /// プロパティ名を取得
        /// </summary>
        private string GetPropertyName(Expression<Func<T, TProperty>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            return string.Empty;
        }
    }
}
```

また、実際に使用するためのクラスバリデーターファクトリクラスも作成します：

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// クラスバリデーターのファクトリクラス
    /// </summary>
    public static class ClassValidatorFactory
    {
        private static Dictionary<Type, object> _validators = new Dictionary<Type, object>();

        /// <summary>
        /// 型に対応するバリデーターを取得または作成
        /// </summary>
        /// <typeparam name="T">検証対象の型</typeparam>
        /// <returns>クラスバリデーター</returns>
        public static ClassValidator<T> GetValidator<T>() where T : class
        {
            Type type = typeof(T);
            
            if (_validators.TryGetValue(type, out object? validator))
            {
                return (ClassValidator<T>)validator;
            }

            var newValidator = new ClassValidator<T>();
            _validators[type] = newValidator;
            return newValidator;
        }

        /// <summary>
        /// 型に対応するバリデーターを登録
        /// </summary>
        /// <typeparam name="T">検証対象の型</typeparam>
        /// <param name="validator">バリデーター</param>
        public static void RegisterValidator<T>(ClassValidator<T> validator) where T : class
        {
            _validators[typeof(T)] = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// 指定された型のインスタンスを検証
        /// </summary>
        /// <typeparam name="T">検証対象の型</typeparam>
        /// <param name="value">検証対象インスタンス</param>
        /// <returns>検証結果</returns>
        public static ValidationResult Validate<T>(T value) where T : class
        {
            if (value == null)
            {
                return ValidationResult.Error("検証対象のインスタンスがnullです。");
            }

            var validator = GetValidator<T>();
            return validator.Validate(value);
        }

        /// <summary>
        /// 非同期で指定された型のインスタンスを検証
        /// </summary>
        /// <typeparam name="T">検証対象の型</typeparam>
        /// <param name="value">検証対象インスタンス</param>
        /// <returns>検証結果を含むタスク</returns>
        public static Task<ValidationResult> ValidateAsync<T>(T value) where T : class
        {
            if (value == null)
            {
                return Task.FromResult(ValidationResult.Error("検証対象のインスタンスがnullです。"));
            }

            var validator = GetValidator<T>();
            return validator.ValidateAsync(value);
        }
    }
}
```

このクラスバリデーションは以下の特徴があります：

1. `ClassValidator<T>`：クラス全体の検証を行うメインクラス
2. `PropertyValidator<T, TProperty>`：クラスの特定のプロパティに対する検証ルールを定義
3. `ClassValidatorFactory`：バリデーターの管理と取得を容易にするユーティリティクラス

使用例：

```csharp
// ユーザークラス
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// バリデーターの定義
public static void ConfigureValidators()
{
    var userValidator = new ClassValidator<User>();
    
    userValidator.RuleFor(u => u.Name)
        .NotNull()
        .Must(name => !string.IsNullOrWhiteSpace(name), "名前は空白であってはなりません。", "NameRequired");
    
    userValidator.RuleFor(u => u.Email)
        .NotNull()
        .SetValidator(StringValidator.Email());
    
    userValidator.RuleFor(u => u.Age)
        .Must(age => age >= 18, "年齢は18歳以上である必要があります。", "AgeTooYoung");
    
    ClassValidatorFactory.RegisterValidator(userValidator);
}

// 検証の使用
public void ValidateUser(User user)
{
    var result = ClassValidatorFactory.Validate(user);
    
    if (!result.IsValid)
    {
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"{error.PropertyName}: {error.Message}");
        }
    }
}
```

これらのファイルを`CoreLib/Utilities/Validation`ディレクトリに保存してください。必要に応じて既存のバリデーション機能と統合して使用できます。

なお、検索結果が10件に制限されているため、すべての既存ファイルを確認できていない可能性があります。完全な結果を見るには、GitHubリポジトリで直接確認することをお勧めします。