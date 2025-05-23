

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

---

I'll help you implement validation in a WinForms DataGridView using INotifyDataErrorInfo, ValidationAttribute, INotifyPropertyChanged, and ErrorProvider to prevent users from leaving cells until validation errors are resolved.

Looking at your repository, I can see you have some validation components already implemented, but we'll need to create a specific implementation for DataGridView validation. Let's create a complete solution:

## Step 1: Create a Model Class with Validation

First, let's create a model class that implements INotifyPropertyChanged and INotifyDataErrorInfo:

```csharp
using CoreLib.Utilities.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace YourNamespace
{
    public class ValidatedItem : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly ValidationErrorProvider _errorProvider = new ValidationErrorProvider();
        
        // プロパティの例
        private string _name;
        
        [Required(ErrorMessage = "名前は必須です")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "名前は3〜50文字で入力してください")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ValidateProperty(value);
                    OnPropertyChanged();
                }
            }
        }
        
        private int _age;
        
        [Range(0, 120, ErrorMessage = "年齢は0〜120の間で入力してください")]
        public int Age
        {
            get => _age;
            set
            {
                if (_age != value)
                {
                    _age = value;
                    ValidateProperty(value);
                    OnPropertyChanged();
                }
            }
        }
        
        // コンストラクタで各プロパティの検証ルールを登録
        public ValidatedItem()
        {
            // ValidationAttributeの登録
            var nameProperty = GetType().GetProperty(nameof(Name));
            var ageProperty = GetType().GetProperty(nameof(Age));
            
            foreach (var attribute in nameProperty.GetCustomAttributes(typeof(ValidationAttribute), true))
            {
                _errorProvider.RegisterValidationRule(nameof(Name), (ValidationAttribute)attribute);
            }
            
            foreach (var attribute in ageProperty.GetCustomAttributes(typeof(ValidationAttribute), true))
            {
                _errorProvider.RegisterValidationRule(nameof(Age), (ValidationAttribute)attribute);
            }
        }

        // INotifyPropertyChanged実装
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        // INotifyDataErrorInfo実装
        public bool HasErrors => _errorProvider.HasErrors;
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => _errorProvider.ErrorsChanged += value;
            remove => _errorProvider.ErrorsChanged -= value;
        }
        
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return _errorProvider.GetErrors(propertyName);
        }
        
        // プロパティ検証メソッド
        private void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            _errorProvider.ValidateProperty(value, propertyName);
        }
        
        // 全てのプロパティを検証する
        public bool Validate()
        {
            return _errorProvider.Validate(this);
        }
    }
}
```

## Step 2: Create a Custom DataGridView with Error Validation

Next, let's create a custom DataGridView control that prevents users from leaving a cell if there's a validation error:

```csharp
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace YourNamespace
{
    public class ValidatingDataGridView : DataGridView
    {
        private ErrorProvider _errorProvider;
        private bool _isCellValidating = false;

        public ValidatingDataGridView()
        {
            // ErrorProviderを初期化
            _errorProvider = new ErrorProvider();
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            
            // セル編集後のイベントハンドラを追加
            CellValidating += ValidatingDataGridView_CellValidating;
            CellEndEdit += ValidatingDataGridView_CellEndEdit;
            CellBeginEdit += ValidatingDataGridView_CellBeginEdit;
        }

        protected virtual void ValidatingDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // セルの編集開始時にエラー表示をクリア
            _errorProvider.SetError(this, string.Empty);
        }

        protected virtual void ValidatingDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // すでに検証中なら再帰を防止
            if (_isCellValidating) return;
            _isCellValidating = true;

            try
            {
                // 行インデックスが有効で、データソースが存在する場合
                if (e.RowIndex >= 0 && DataSource != null)
                {
                    var bindingSource = DataSource as BindingSource;
                    object dataItem = null;

                    // 現在の行のデータ項目を取得
                    if (bindingSource != null)
                    {
                        dataItem = bindingSource[e.RowIndex];
                    }
                    else if (DataSource is IList<object> list && e.RowIndex < list.Count)
                    {
                        dataItem = list[e.RowIndex];
                    }

                    // データ項目がINotifyDataErrorInfoを実装している場合
                    if (dataItem is INotifyDataErrorInfo errorInfo)
                    {
                        // 編集中のカラム名を取得
                        string propertyName = Columns[e.ColumnIndex].DataPropertyName;

                        // 現在の値をセット（型変換を行う）
                        var prop = dataItem.GetType().GetProperty(propertyName);
                        if (prop != null)
                        {
                            try
                            {
                                // 新しい値に変換して仮にセット
                                object convertedValue = Convert.ChangeType(e.FormattedValue, prop.PropertyType);
                                prop.SetValue(dataItem, convertedValue);
                            }
                            catch (Exception)
                            {
                                // 型変換に失敗した場合、エラーを表示してセルから出ないようにする
                                _errorProvider.SetError(this, "入力値の型が正しくありません");
                                e.Cancel = true;
                                return;
                            }

                            // プロパティに関連するエラーがあるか確認
                            var errors = errorInfo.GetErrors(propertyName);
                            
                            // エラーを列挙して最初のエラーメッセージを取得
                            string errorMessage = string.Empty;
                            foreach (object error in errors)
                            {
                                errorMessage = error.ToString();
                                if (!string.IsNullOrEmpty(errorMessage))
                                    break;
                            }

                            // エラーがあれば表示し、セルから出ないようにする
                            if (!string.IsNullOrEmpty(errorMessage))
                            {
                                _errorProvider.SetError(this, errorMessage);
                                e.Cancel = true;
                            }
                        }
                    }
                }
            }
            finally
            {
                _isCellValidating = false;
            }
        }

        protected virtual void ValidatingDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // セル編集完了時にデータが正常ならエラーをクリア
            var errorMessage = _errorProvider.GetError(this);
            if (string.IsNullOrEmpty(errorMessage))
            {
                _errorProvider.SetError(this, string.Empty);
            }
            
            // 編集が完了したら、変更された行の表示を更新
            InvalidateRow(e.RowIndex);
        }
    }
}
```

## Step 3: Create ObservableCollection and BindingSource

Now, let's create a method to set up the DataGridView with an ObservableCollection data source:

```csharp
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace YourNamespace
{
    public static class DataGridViewExtensions
    {
        public static BindingSource BindValidatedCollection<T>(this ValidatingDataGridView dataGridView, 
            ObservableCollection<T> collection) where T : INotifyPropertyChanged, INotifyDataErrorInfo
        {
            // BindingSourceを作成
            var bindingSource = new BindingSource();
            bindingSource.DataSource = collection;
            
            // DataGridViewにBindingSourceをセット
            dataGridView.DataSource = bindingSource;
            
            // 編集モードをプログラムモードに設定（セルから出る時の検証のため）
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            
            return bindingSource;
        }
    }
}
```

## Step 4: Usage Example (Form)

Here's how to use the above components in a form:

```csharp
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace YourNamespace
{
    public partial class MainForm : Form
    {
        private ObservableCollection<ValidatedItem> _items;
        private ValidatingDataGridView _dataGridView;
        private BindingSource _bindingSource;
        
        public MainForm()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadData();
        }
        
        private void SetupDataGridView()
        {
            _dataGridView = new ValidatingDataGridView();
            _dataGridView.Dock = DockStyle.Fill;
            _dataGridView.AllowUserToAddRows = true;
            _dataGridView.AllowUserToDeleteRows = true;
            _dataGridView.AutoGenerateColumns = true;
            
            // フォームにDataGridViewを追加
            Controls.Add(_dataGridView);
        }
        
        private void LoadData()
        {
            // サンプルデータの作成
            _items = new ObservableCollection<ValidatedItem>
            {
                new ValidatedItem { Name = "テストユーザー1", Age = 30 },
                new ValidatedItem { Name = "テストユーザー2", Age = 25 },
                new ValidatedItem { Name = "", Age = 150 }  // エラーあり
            };
            
            // DataGridViewにデータをバインド
            _bindingSource = _dataGridView.BindValidatedCollection(_items);
        }
    }
}
```

## Step 5: Enhancements for Better User Experience

Finally, let's add some enhancements to make the user experience better:

```csharp
// ValidatingDataGridView クラスに追加
public class ValidatingDataGridView : DataGridView
{
    // 既存のコードに追加
    
    // セルの背景色をエラー状態によって変更するメソッド
    protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
    {
        base.OnCellPainting(e);
        
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && DataSource != null)
        {
            var bindingSource = DataSource as BindingSource;
            if (bindingSource != null && e.RowIndex < bindingSource.Count)
            {
                var dataItem = bindingSource[e.RowIndex];
                
                if (dataItem is INotifyDataErrorInfo errorInfo)
                {
                    string propertyName = Columns[e.ColumnIndex].DataPropertyName;
                    var errors = errorInfo.GetErrors(propertyName);
                    
                    bool hasErrors = false;
                    foreach (var error in errors)
                    {
                        if (!string.IsNullOrEmpty(error?.ToString()))
                        {
                            hasErrors = true;
                            break;
                        }
                    }
                    
                    if (hasErrors)
                    {
                        // エラーがある場合、背景色を薄い赤色に
                        e.CellStyle.BackColor = Color.MistyRose;
                        // ツールチップをセット
                        var cell = Rows[e.RowIndex].Cells[e.ColumnIndex];
                        foreach (var error in errors)
                        {
                            if (!string.IsNullOrEmpty(error?.ToString()))
                            {
                                cell.ToolTipText = error.ToString();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    
    // グリッドの編集を開始するためのメソッドをオーバーライド
    protected override void OnCellClick(DataGridViewCellEventArgs e)
    {
        base.OnCellClick(e);
  


---

WinFormsのC1FlexGridでINotifyDataErrorInfoによるバリデーションを実装する際の問題について理解しました。ValidateEditでキャンセルするとPropertyChangedイベントが発火せず、プロパティが更新されない問題が発生しているようですね。

この問題は、ValidateEditイベントでキャンセル処理をすると、値の変更プロセスが中断されてしまうことが原因です。解決策を提案します。

## 解決策

1. **バリデーションとプロパティ更新の分離**:
   - ValidateEditでエラー検出だけを行い、エラー時もいったんプロパティ値は更新する
   - エラー状態を視覚的に表示し、後続の保存処理などでエラーがある場合は処理をブロックする

2. **具体的な実装例**:

```csharp
public class ValidationModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
    private readonly ValidationErrorProvider _errorProvider = new ValidationErrorProvider();
    private string _name;
    
    [Required(ErrorMessage = "名前は必須です")]
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                ValidateProperty(value, nameof(Name));
                OnPropertyChanged();
            }
        }
    }
    
    // INotifyPropertyChanged実装
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // INotifyDataErrorInfo実装
    public bool HasErrors => _errorProvider.HasErrors;
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
    {
        add => _errorProvider.ErrorsChanged += value;
        remove => _errorProvider.ErrorsChanged -= value;
    }
    
    public IEnumerable GetErrors(string propertyName)
    {
        return _errorProvider.GetErrors(propertyName);
    }
    
    // バリデーション実行
    private void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
    {
        _errorProvider.ClearErrors(propertyName);
        
        var context = new ValidationContext(this) { MemberName = propertyName };
        var results = new List<ValidationResult>();
        
        if (!Validator.TryValidateProperty(value, context, results))
        {
            foreach (var result in results)
            {
                _errorProvider.AddError(propertyName, result.ErrorMessage);
            }
        }
    }
}
```

3. **C1FlexGridの設定**:

```csharp
public partial class Form1 : Form
{
    private ObservableCollection<ValidationModel> _items;
    
    public Form1()
    {
        InitializeComponent();
        
        _items = new ObservableCollection<ValidationModel>
        {
            new ValidationModel { Name = "テスト1" },
            new ValidationModel { Name = "テスト2" }
        };
        
        c1FlexGrid1.DataSource = _items;
        
        // バリデーションエラーを視覚的に表示するためのスタイルを設定
        c1FlexGrid1.EditError += C1FlexGrid1_EditError;
        c1FlexGrid1.AfterEdit += C1FlexGrid1_AfterEdit;
        c1FlexGrid1.ValidateEdit += C1FlexGrid1_ValidateEdit;
    }
    
    private void C1FlexGrid1_ValidateEdit(object sender, C1.Win.FlexGrid.ValidateEditEventArgs e)
    {
        // 現在のインデックスからモデルを取得
        var item = _items[c1FlexGrid1.Row - c1FlexGrid1.Rows.Fixed];
        
        // プロパティを直接更新して、バリデーションを実行する
        // この時点ではキャンセルせず、値の更新とバリデーションの実行を許可
        string propertyName = c1FlexGrid1.Cols[e.Col].Name;
        
        // プロパティに値を設定すると自動でバリデーションが実行される
        typeof(ValidationModel).GetProperty(propertyName)?.SetValue(item, e.Value);
        
        // エラーがある場合は視覚的なフィードバックを提供するが、編集はキャンセルしない
        // バリデーションエラーはPropertyChangedイベント後に確認する
    }
    
    private void C1FlexGrid1_AfterEdit(object sender, RowColEventArgs e)
    {
        // 編集完了後にエラー状態を確認し、視覚的なフィードバックを提供
        var item = _items[c1FlexGrid1.Row - c1FlexGrid1.Rows.Fixed];
        string propertyName = c1FlexGrid1.Cols[e.Col].Name;
        
        if (item is INotifyDataErrorInfo model)
        {
            var errors = model.GetErrors(propertyName);
            if (errors is IEnumerable enumerable && enumerable.Cast<object>().Any())
            {
                // エラー表示のためのスタイルを適用
                c1FlexGrid1[e.Row, e.Col] = new CellStyle
                {
                    BackColor = Color.MistyRose,
                    ForeColor = Color.Red
                };
                
                // エラーメッセージをツールチップなどで表示
                var errorMessages = string.Join(Environment.NewLine, 
                    enumerable.Cast<string>().ToArray());
                    
                // ツールチップなどにエラーを表示
            }
            else
            {
                // エラースタイルをクリア
                c1FlexGrid1[e.Row, e.Col] = null;
            }
        }
    }
    
    private void C1FlexGrid1_EditError(object sender, EventArgs e)
    {
        // 編集エラー処理（必要に応じて）
    }
}
```

## キーポイント

1. **ValidateEditでの処理**:
   - プロパティ値の更新とバリデーションの実行は許可する
   - エラーがあってもe.Cancel = trueとしない

2. **AfterEditでのUI更新**:
   - 編集完了後にエラー状態を確認
   - 視覚的なフィードバックを提供（セルの背景色変更、エラーアイコン表示など）
   - 必要に応じてエラーメッセージをツールチップなどで表示

3. **ユーザーエクスペリエンス**:
   - セルからは出られるが、エラー状態が視覚的に明確
   - 保存ボタンなどのアクション実行前にHasErrorsをチェックして、エラーがあれば実行をブロック

この方法により、PropertyChangedイベントが正常に発火してプロパティが更新されつつ、バリデーションも実行されます。ユーザーはエラーのある入力をいったん行えますが、視覚的なフィードバックを受け取り、最終的なアクションはブロックされる形になります。

追加の対応として、移動先のセルを制限したい場合は、AfterEditのタイミングで次のセルへの移動をハンドリングする方法もあります。      