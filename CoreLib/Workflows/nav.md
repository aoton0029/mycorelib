Workflowを使った画面遷移実装
画面フローとナビゲーション実装
.NET 8環境において、Workflowクラスを使った画面遷移処理を実装します。NavigationServiceを開発し、ワークフローと組み合わせることで、構造化された画面遷移フローを実現します。
1. NavigationServiceの実装
まず、画面遷移を担当するNavigationServiceを作成します：
```
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLib.Navigation
{
    /// <summary>
    /// 画面遷移サービスのインターフェース
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 指定した画面に遷移
        /// </summary>
        /// <typeparam name="TScreen">画面の型</typeparam>
        /// <param name="parameters">遷移時のパラメータ</param>
        Task NavigateToAsync<TScreen>(object parameters = null) where TScreen : class;
        
        /// <summary>
        /// 指定した画面IDに遷移
        /// </summary>
        /// <param name="screenId">画面ID</param>
        /// <param name="parameters">遷移時のパラメータ</param>
        Task NavigateToAsync(string screenId, object parameters = null);
        
        /// <summary>
        /// 前の画面に戻る
        /// </summary>
        /// <param name="result">前の画面に渡す結果</param>
        Task GoBackAsync(object result = null);
        
        /// <summary>
        /// 画面遷移履歴をクリア
        /// </summary>
        void ClearHistory();
        
        /// <summary>
        /// 現在の画面の取得
        /// </summary>
        object CurrentScreen { get; }
        
        /// <summary>
        /// 前の画面に戻れるかどうか
        /// </summary>
        bool CanGoBack { get; }
    }
    
    /// <summary>
    /// 画面ファクトリのインターフェース
    /// </summary>
    public interface IScreenFactory
    {
        /// <summary>
        /// 画面インスタンスの作成
        /// </summary>
        /// <param name="screenId">画面ID</param>
        /// <returns>画面インスタンス</returns>
        object CreateScreen(string screenId);
    }
    
    /// <summary>
    /// 画面遷移サービスの実装
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _navigationStack = new Stack<object>();
        private readonly Dictionary<string, Type> _screenRegistry = new Dictionary<string, Type>();
        private readonly IScreenFactory _screenFactory;
        private readonly IServiceProvider _serviceProvider;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NavigationService(IScreenFactory screenFactory, IServiceProvider serviceProvider)
        {
            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        /// <summary>
        /// 画面タイプの登録
        /// </summary>
        public void RegisterScreen<TScreen>(string screenId) where TScreen : class
        {
            _screenRegistry[screenId] = typeof(TScreen);
        }
        
        /// <summary>
        /// 現在の画面
        /// </summary>
        public object CurrentScreen => _navigationStack.Count > 0 ? _navigationStack.Peek() : null;
        
        /// <summary>
        /// 前の画面に戻れるかどうか
        /// </summary>
        public bool CanGoBack => _navigationStack.Count > 1;
        
        /// <summary>
        /// 指定した画面に遷移
        /// </summary>
        public async Task NavigateToAsync<TScreen>(object parameters = null) where TScreen : class
        {
            var screenId = typeof(TScreen).Name;
            await NavigateToAsync(screenId, parameters);
        }
        
        /// <summary>
        /// 指定した画面IDに遷移
        /// </summary>
        public async Task NavigateToAsync(string screenId, object parameters = null)
        {
            // 画面インスタンスの作成
            var screen = _screenFactory.CreateScreen(screenId);
            
            // 画面初期化処理（IInitializableが実装されている場合）
            if (screen is IInitializable initializable)
            {
                await initializable.InitializeAsync(parameters);
            }
            
            // 画面遷移スタックに追加
            _navigationStack.Push(screen);
            
            // 画面表示処理（IActivatableが実装されている場合）
            if (screen is IActivatable activatable)
            {
                await activatable.ActivateAsync();
            }
        }
        
        /// <summary>
        /// 前の画面に戻る
        /// </summary>
        public async Task GoBackAsync(object result = null)
        {
            if (!CanGoBack)
            {
                throw new InvalidOperationException("画面履歴がないため戻ることができません。");
            }
            
            // 現在の画面を閉じる
            var currentScreen = _navigationStack.Pop();
            if (currentScreen is IDeactivatable deactivatable)
            {
                await deactivatable.DeactivateAsync();
            }
            
            // 戻り先の画面を取得
            var previousScreen = _navigationStack.Peek();
            
            // 結果を前の画面に渡す（IResultReceiverが実装されている場合）
            if (previousScreen is IResultReceiver receiver && result != null)
            {
                await receiver.ReceiveResultAsync(result);
            }
            
            // 前の画面をアクティブ化
            if (previousScreen is IActivatable activatable)
            {
                await activatable.ActivateAsync();
            }
        }
        
        /// <summary>
        /// 画面遷移履歴をクリア
        /// </summary>
        public void ClearHistory()
        {
            if (_navigationStack.Count > 1)
            {
                var currentScreen = _navigationStack.Peek();
                _navigationStack.Clear();
                _navigationStack.Push(currentScreen);
            }
        }
    }
    
    /// <summary>
    /// 画面初期化インターフェース
    /// </summary>
    public interface IInitializable
    {
        Task InitializeAsync(object parameters);
    }
    
    /// <summary>
    /// 画面アクティブ化インターフェース
    /// </summary>
    public interface IActivatable
    {
        Task ActivateAsync();
    }
    
    /// <summary>
    /// 画面非アクティブ化インターフェース
    /// </summary>
    public interface IDeactivatable
    {
        Task DeactivateAsync();
    }
    
    /// <summary>
    /// 結果受信インターフェース
    /// </summary>
    public interface IResultReceiver
    {
        Task ReceiveResultAsync(object result);
    }
}

```

2. 画面遷移用ワークフローステップの実装
次に、WorkflowとNavigationServiceを連携させる専用のステップクラスを作成します：
```
using System;
using System.Threading;
using System.Threading.Tasks;
using CoreLib.Navigation;
using CoreLib.Workflows;

namespace CoreLib.Navigation
{
    /// <summary>
    /// 画面遷移ワークフローステップ
    /// </summary>
    public class NavigationStep : WorkflowStepBase
    {
        private readonly INavigationService _navigationService;
        private readonly string _screenId;
        private readonly Func<WorkflowContext, object> _parametersGenerator;
        private readonly string _resultKey;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NavigationStep(
            INavigationService navigationService,
            string stepId,
            string description,
            string screenId,
            Func<WorkflowContext, object> parametersGenerator = null,
            string resultKey = null)
            : base(stepId, description)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _screenId = screenId ?? throw new ArgumentNullException(nameof(screenId));
            _parametersGenerator = parametersGenerator;
            _resultKey = resultKey;
        }
        
        /// <summary>
        /// ステップの内部実行処理
        /// </summary>
        protected override async Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken)
        {
            try
            {
                // パラメータの生成
                var parameters = _parametersGenerator?.Invoke(context);
                
                // 画面遷移実行
                await _navigationService.NavigateToAsync(_screenId, parameters);
                
                // 画面からの結果が必要な場合は、ここで待機する実装を追加
                // (本実装では簡略化のため、画面遷移成功のみを返す)
                
                // 結果キーが指定されている場合、画面遷移先からの戻り値をコンテキストに設定
                // この例では、仮のデータをセットする例
                if (!string.IsNullOrEmpty(_resultKey) && _navigationService.CurrentScreen is IScreenWithResult screenWithResult)
                {
                    var result = screenWithResult.GetResult();
                    context.SetData(_resultKey, result);
                }
                
                return StepResult.Success();
            }
            catch (Exception ex)
            {
                return StepResult.Failure($"画面 {_screenId} への遷移中にエラーが発生しました: {ex.Message}", ex);
            }
        }
    }
    
    /// <summary>
    /// 戻るボタン用ワークフローステップ
    /// </summary>
    public class GoBackStep : WorkflowStepBase
    {
        private readonly INavigationService _navigationService;
        private readonly Func<WorkflowContext, object> _resultGenerator;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GoBackStep(
            INavigationService navigationService,
            string stepId,
            string description,
            Func<WorkflowContext, object> resultGenerator = null)
            : base(stepId, description)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _resultGenerator = resultGenerator;
        }
        
        /// <summary>
        /// ステップの内部実行処理
        /// </summary>
        protected override async Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken)
        {
            try
            {
                if (!_navigationService.CanGoBack)
                {
                    return StepResult.Failure("画面履歴がないため戻ることができません。");
                }
                
                // 戻り値の生成
                var result = _resultGenerator?.Invoke(context);
                
                // 前の画面に戻る
                await _navigationService.GoBackAsync(result);
                
                return StepResult.Success();
            }
            catch (Exception ex)
            {
                return StepResult.Failure($"前の画面に戻る処理中にエラーが発生しました: {ex.Message}", ex);
            }
        }
    }
    
    /// <summary>
    /// 結果を提供する画面のインターフェース
    /// </summary>
    public interface IScreenWithResult
    {
        object GetResult();
    }
}

```

3. 画面フロービルダーの実装
画面フローを簡単に構築するためのビルダークラスを作成します：
```
using System;
using CoreLib.Logging;
using CoreLib.Navigation;
using CoreLib.Workflows;

namespace CoreLib.Navigation
{
    /// <summary>
    /// 画面フロービルダー
    /// </summary>
    public class ScreenFlowBuilder
    {
        private readonly WorkflowBuilder _workflowBuilder;
        private readonly INavigationService _navigationService;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScreenFlowBuilder(
            string flowId,
            string description,
            INavigationService navigationService,
            IAppLogger logger = null)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _workflowBuilder = new WorkflowBuilder(flowId, description, logger);
        }
        
        /// <summary>
        /// 画面遷移ステップを追加
        /// </summary>
        public ScreenFlowBuilder NavigateTo(
            string stepId,
            string description,
            string screenId,
            Func<WorkflowContext, object> parametersGenerator = null,
            string resultKey = null)
        {
            var step = new NavigationStep(
                _navigationService,
                stepId,
                description,
                screenId,
                parametersGenerator,
                resultKey);
                
            _workflowBuilder.AddStep(step);
            return this;
        }
        
        /// <summary>
        /// 戻るステップを追加
        /// </summary>
        public ScreenFlowBuilder GoBack(
            string stepId,
            string description,
            Func<WorkflowContext, object> resultGenerator = null)
        {
            var step = new GoBackStep(
                _navigationService,
                stepId,
                description,
                resultGenerator);
                
            _workflowBuilder.AddStep(step);
            return this;
        }
        
        /// <summary>
        /// 条件分岐ステップを追加
        /// </summary>
        public ScreenFlowBuilder AddCondition(
            string stepId,
            string description,
            Func<WorkflowContext, bool> condition,
            Action<ScreenFlowBuilder> configureTrueFlow,
            Action<ScreenFlowBuilder> configureFalseFlow)
        {
            // 真の場合のフロービルダー
            var trueBuilder = new ScreenFlowBuilder(
                $"{stepId}_True",
                $"{description} (True フロー)",
                _navigationService);
            configureTrueFlow(trueBuilder);
            var trueWorkflow = trueBuilder.BuildWorkflow();
            
            // 偽の場合のフロービルダー
            var falseBuilder = new ScreenFlowBuilder(
                $"{stepId}_False",
                $"{description} (False フロー)",
                _navigationService);
            configureFalseFlow(falseBuilder);
            var falseWorkflow = falseBuilder.BuildWorkflow();
            
            // 各ワークフローの最初のステップを取得
            var trueStep = trueWorkflow.Steps.Count > 0 ? trueWorkflow.Steps[0] : 
                new FunctionStep($"{stepId}_TrueNoOp", "何もしない", (_, __) => Task.FromResult(StepResult.Success()));
                
            var falseStep = falseWorkflow.Steps.Count > 0 ? falseWorkflow.Steps[0] : 
                new FunctionStep($"{stepId}_FalseNoOp", "何もしない", (_, __) => Task.FromResult(StepResult.Success()));
            
            _workflowBuilder.AddConditionalStep(stepId, description, condition, trueStep, falseStep);
            return this;
        }
        
        /// <summary>
        /// 関数ステップを追加
        /// </summary>
        public ScreenFlowBuilder AddFunction(
            string stepId,
            string description,
            Func<WorkflowContext, CancellationToken, Task<StepResult>> function)
        {
            _workflowBuilder.AddFunctionStep(stepId, description, function);
            return this;
        }
        
        /// <summary>
        /// ワークフローをビルド
        /// </summary>
        public Workflow BuildWorkflow()
        {
            return _workflowBuilder.Build();
        }
    }
}

```

4. 依存関係の登録
DIコンテナに上記のサービスを登録します：
```
using CoreLib.Logging;
using CoreLib.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace CoreLib.Configuration
{
    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// ナビゲーションサービスを登録
        /// </summary>
        public static IServiceCollection AddNavigationServices(this IServiceCollection services)
        {
            // スクリーンファクトリーを登録（アプリケーション固有の実装が必要）
            services.AddSingleton<IScreenFactory, AppScreenFactory>();
            
            // ナビゲーションサービスをシングルトンで登録
            services.AddSingleton<INavigationService, NavigationService>();
            
            return services;
        }
    }
}

```

5. 使用例
画面遷移フローの定義
```
// 依存関係の注入
IServiceProvider serviceProvider = /* DIコンテナー */;
var navigationService = serviceProvider.GetRequiredService<INavigationService>();
var logger = serviceProvider.GetRequiredService<IAppLogger>();

// 画面フローの定義
var loginFlow = new ScreenFlowBuilder(
    "LoginFlow", 
    "ログインとユーザー登録の画面フロー",
    navigationService,
    logger)
    .NavigateTo(
        "ShowWelcome",
        "ようこそ画面を表示",
        "WelcomeScreen")
    .AddCondition(
        "CheckUserExists",
        "ユーザーが既に存在するか確認",
        ctx => ctx.GetData<bool>("UserExists"),
        // ユーザーが存在する場合のフロー
        trueFlow => trueFlow
            .NavigateTo(
                "ShowLogin",
                "ログイン画面を表示",
                "LoginScreen",
                ctx => new { ReturnUrl = ctx.GetData<string>("ReturnUrl") },
                "LoginResult"),
        // ユーザーが存在しない場合のフロー
        falseFlow => falseFlow
            .NavigateTo(
                "ShowRegistration", 
                "ユーザー登録画面を表示",
                "RegistrationScreen",
                null,
                "RegistrationResult")
            .AddCondition(
                "CheckRegistrationSuccess",
                "登録成功を確認",
                ctx => ctx.GetData<bool>("RegistrationResult"),
                // 登録成功の場合
                successFlow => successFlow
                    .NavigateTo(
                        "ShowLogin", 
                        "ログイン画面を表示",
                        "LoginScreen"),
                // 登録失敗の場合
                failFlow => failFlow
                    .GoBack(
                        "ReturnToWelcome", 
                        "ようこそ画面に戻る"))
    )
    .AddCondition(
        "CheckLoginSuccess",
        "ログイン成功を確認",
        ctx => ctx.GetData<bool>("LoginResult"),
        // ログイン成功の場合
        successFlow => successFlow
            .NavigateTo(
                "ShowDashboard",
                "ダッシュボード画面を表示",
                "DashboardScreen",
                ctx => new { UserName = ctx.GetData<string>("UserName") }),
        // ログイン失敗の場合
        failFlow => failFlow
            .NavigateTo(
                "ShowLoginError",
                "ログインエラー画面を表示",
                "LoginErrorScreen")
            .GoBack(
                "ReturnToLogin",
                "ログイン画面に戻る")
    )
    .BuildWorkflow();

// 画面フローの実行開始
await loginFlow.ExecuteAsync();

```

WinFormsでの実装例
WinFormsアプリケーションで画面ファクトリを実装する例：
```
using CoreLib.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YourApp.Forms
{
    public class WinFormsScreenFactory : IScreenFactory
    {
        private readonly Dictionary<string, Func<Form>> _formFactories = new();
        private readonly IServiceProvider _serviceProvider;
        
        public WinFormsScreenFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // 画面登録
            RegisterScreen("WelcomeScreen", () => new WelcomeForm(_serviceProvider));
            RegisterScreen("LoginScreen", () => new LoginForm(_serviceProvider));
            RegisterScreen("RegistrationScreen", () => new RegistrationForm(_serviceProvider));
            RegisterScreen("DashboardScreen", () => new DashboardForm(_serviceProvider));
            RegisterScreen("LoginErrorScreen", () => new LoginErrorForm(_serviceProvider));
        }
        
        public void RegisterScreen(string screenId, Func<Form> factory)
        {
            _formFactories[screenId] = factory;
        }
        
        public object CreateScreen(string screenId)
        {
            if (_formFactories.TryGetValue(screenId, out var factory))
            {
                return factory();
            }
            
            throw new ArgumentException($"画面ID '{screenId}' は登録されていません。");
        }
    }
    
    public class WinFormsNavigationService : NavigationService
    {
        private Form _currentForm;
        
        public WinFormsNavigationService(IScreenFactory screenFactory, IServiceProvider serviceProvider)
            : base(screenFactory, serviceProvider)
        {
        }
        
        public override async Task NavigateToAsync(string screenId, object parameters = null)
        {
            // 画面インスタンスを作成
            var screen = _screenFactory.CreateScreen(screenId);
            
            if (!(screen is Form form))
            {
                throw new InvalidOperationException($"画面 '{screenId}' はForm型ではありません。");
            }
            
            // 画面初期化
            if (form is IInitializable initializable)
            {
                await initializable.InitializeAsync(parameters);
            }
            
            // 現在の画面を非表示
            _currentForm?.Hide();
            
            // 新しい画面を表示
            _currentForm = form;
            form.Show();
            
            // 画面遷移スタックに追加
            base.NavigateToAsync(screenId, parameters);
        }
        
        public override async Task GoBackAsync(object result = null)
        {
            if (!CanGoBack)
            {
                throw new InvalidOperationException("画面履歴がないため戻ることができません。");
            }
            
            // 現在の画面を閉じる
            var oldForm = _currentForm;
            await base.GoBackAsync(result);
            
            // 前の画面を表示
            var previousScreen = CurrentScreen as Form;
            _currentForm = previousScreen;
            previousScreen?.Show();
            
            // 古い画面を破棄
            oldForm?.Dispose();
        }
    }
}
```
6. 解説
このコード実装では、以下の特長を持つ画面遷移フレームワークを実現しています：
1.	宣言的な画面フロー定義
•	ScreenFlowBuilderを使って、可読性の高い宣言的な画面フローを定義
2.	柔軟な画面遷移ロジック
•	条件分岐、データの受け渡し、エラーハンドリングをサポート
3.	UIフレームワーク非依存
•	基本設計はUIフレームワークに依存しないため、WinForms、WPF、MAUIなど様々な環境で利用可能
4.	テスト容易性
•	画面遷移ロジックがワークフローとして分離されているため、単体テストが容易
5.	拡張性
•	新しい画面遷移パターンや特殊な振る舞いを追加しやすい設計

---

1. ConditionalStepを使った条件分岐
CoreLib.Workflows名前空間に含まれるConditionalStepクラスを使うと、条件によって異なる画面遷移パスを実行できます。
```
/// <summary>
/// 条件分岐ステップ
/// </summary>
public class ConditionalStep : WorkflowStepBase
{
    private readonly Func<WorkflowContext, bool> _condition;
    private readonly IWorkflowStep _trueStep;
    private readonly IWorkflowStep _falseStep;

    // コンストラクタ省略

    /// <summary>
    /// ステップの内部実行処理
    /// </summary>
    protected override async Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken)
    {
        var result = _condition(context);

        return result
            ? await _trueStep.ExecuteAsync(context, cancellationToken)
            : await _falseStep.ExecuteAsync(context, cancellationToken);
    }
}

```
2. 実装サンプル
2.1 画面遷移ステップの作成
まず、画面遷移を行うためのカスタムNavigationStepを作成します：
```
using CoreLib.Workflows;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YourApp.Navigation
{
    /// <summary>
    /// 画面遷移ステップ
    /// </summary>
    public class NavigationStep : WorkflowStepBase
    {
        private readonly IScreenNavigator _navigator;
        private readonly string _screenName;
        private readonly Func<WorkflowContext, object> _parameterFactory;

        public NavigationStep(
            string stepId,
            string description,
            IScreenNavigator navigator,
            string screenName,
            Func<WorkflowContext, object> parameterFactory = null)
            : base(stepId, description)
        {
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _screenName = screenName ?? throw new ArgumentNullException(nameof(screenName));
            _parameterFactory = parameterFactory;
        }

        protected override async Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken)
        {
            try
            {
                var parameters = _parameterFactory?.Invoke(context);
                await _navigator.NavigateToAsync(_screenName, parameters);
                return StepResult.Success();
            }
            catch (Exception ex)
            {
                return StepResult.Failure($"画面 {_screenName} への遷移に失敗しました: {ex.Message}", ex);
            }
        }
    }
}

```

2.2 条件付き画面遷移の実装例
```
using CoreLib.Workflows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using YourApp.Navigation;
using YourApp.Screens;
using YourApp.Services;

namespace YourApp
{
    public class UserRegistrationFlow
    {
        private readonly IScreenNavigator _navigator;
        private readonly IUserService _userService;
        private readonly IAppLogger _logger;

        public UserRegistrationFlow(
            IScreenNavigator navigator,
            IUserService userService,
            IAppLogger logger)
        {
            _navigator = navigator;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync()
        {
            // ワークフローを構築
            var workflow = new WorkflowBuilder("UserRegistration", "ユーザー登録フロー", _logger)
                // 1. 最初にウェルカム画面を表示
                .AddStep(new NavigationStep(
                    "ShowWelcome", 
                    "ウェルカム画面表示", 
                    _navigator, 
                    "WelcomeScreen"))
                
                // 2. ユーザータイプの選択結果を取得する関数ステップ
                .AddFunctionStep(
                    "GetUserType",
                    "ユーザータイプの取得",
                    async (context, token) => {
                        // 画面からの結果を取得（この例では簡略化）
                        var userType = await _userService.GetSelectedUserTypeAsync();
                        context.SetData("UserType", userType);
                        return StepResult.Success();
                    })
                
                // 3. ユーザータイプに基づく条件分岐
                .AddConditionalStep(
                    "CheckUserType",
                    "ユーザータイプ確認",
                    context => context.GetData<string>("UserType") == "Corporate",
                    // 法人ユーザー向け登録フォーム
                    new NavigationStep(
                        "CorporateRegistration",
                        "法人登録画面表示",
                        _navigator,
                        "CorporateRegistrationScreen"),
                    // 個人ユーザー向け登録フォーム
                    new NavigationStep(
                        "PersonalRegistration",
                        "個人登録画面表示",
                        _navigator,
                        "PersonalRegistrationScreen"))
                
                // 4. 登録情報の検証
                .AddFunctionStep(
                    "ValidateRegistration",
                    "登録情報検証",
                    async (context, token) => {
                        var userType = context.GetData<string>("UserType");
                        bool isValid;
                        
                        if (userType == "Corporate") {
                            // 法人情報の検証
                            var corporateInfo = await _navigator.GetScreenDataAsync<CorporateInfo>();
                            isValid = _userService.ValidateCorporateInfo(corporateInfo);
                            context.SetData("RegistrationData", corporateInfo);
                        } else {
                            // 個人情報の検証
                            var personalInfo = await _navigator.GetScreenDataAsync<PersonalInfo>();
                            isValid = _userService.ValidatePersonalInfo(personalInfo);
                            context.SetData("RegistrationData", personalInfo);
                        }
                        
                        context.SetData("IsValid", isValid);
                        return StepResult.Success();
                    })
                
                // 5. 検証結果に基づく条件分岐
                .AddConditionalStep(
                    "CheckValidation",
                    "検証結果確認",
                    context => context.GetData<bool>("IsValid"),
                    // 検証成功：確認画面へ
                    new NavigationStep(
                        "ShowConfirmation",
                        "確認画面表示",
                        _navigator,
                        "RegistrationConfirmationScreen",
                        ctx => ctx.GetData<object>("RegistrationData")),
                    // 検証失敗：エラー画面へ
                    new NavigationStep(
                        "ShowError",
                        "エラー画面表示",
                        _navigator,
                        "ValidationErrorScreen"))
                
                // 6. 登録完了処理
                .AddFunctionStep(
                    "CompleteRegistration",
                    "登録完了処理",
                    async (context, token) => {
                        // 登録処理の実行
                        var data = context.GetData<object>("RegistrationData");
                        var userType = context.GetData<string>("UserType");
                        
                        bool success = await _userService.RegisterUserAsync(data, userType);
                        context.SetData("RegistrationSuccess", success);
                        
                        return StepResult.Success();
                    })
                
                // 7. 登録結果に基づく条件分岐
                .AddConditionalStep(
                    "CheckRegistrationResult",
                    "登録結果確認",
                    context => context.GetData<bool>("RegistrationSuccess"),
                    // 登録成功：完了画面へ
                    new NavigationStep(
                        "ShowCompletion",
                        "完了画面表示",
                        _navigator,
                        "RegistrationCompletionScreen"),
                    // 登録失敗：エラー画面へ
                    new NavigationStep(
                        "ShowRegistrationError",
                        "登録エラー画面表示",
                        _navigator,
                        "RegistrationErrorScreen"))
                
                .Build();

            // ワークフローを実行
            return await workflow.ExecuteAsync();
        }
    }
}

```

3. ネストされた条件分岐
より複雑な条件分岐が必要な場合、ネストされたConditionalStepを使用できます：
```
// ユーザー状態に基づく3つのパスを持つ条件分岐
var userStatusStep = new ConditionalStep(
    "CheckUserStatus",
    "ユーザー状態確認",
    context => context.GetData<string>("UserStatus") == "Active",
    // アクティブユーザー向けパス
    new NavigationStep("ShowDashboard", "ダッシュボード表示", _navigator, "DashboardScreen"),
    // 非アクティブユーザー向けの追加分岐
    new ConditionalStep(
        "CheckInactiveReason",
        "非アクティブ理由確認",
        context => context.GetData<string>("InactiveReason") == "PendingVerification",
        // 認証待ちユーザー向けパス
        new NavigationStep("ShowVerification", "認証画面表示", _navigator, "VerificationScreen"),
        // その他の非アクティブユーザー向けパス
        new NavigationStep("ShowReactivation", "再有効化画面表示", _navigator, "ReactivationScreen")
    )
);

```
4. WorkflowContextを活用したデータ共有
条件分岐で使用するデータをWorkflowContextに格納することで、画面間でのデータ共有が容易になります：
```
// ログイン処理ステップ
.AddFunctionStep(
    "ProcessLogin",
    "ログイン処理",
    async (context, token) => {
        // ログイン画面からのデータ取得
        var credentials = await _navigator.GetScreenDataAsync<LoginCredentials>();
        
        // ログイン処理実行
        var result = await _authService.LoginAsync(credentials.Username, credentials.Password);
        
        // コンテキストにデータを格納（条件分岐で使用）
        context.SetData("LoginSuccess", result.IsSuccess);
        context.SetData("UserRole", result.UserRole);
        context.SetData("RequiresMfa", result.RequiresMfa);
        
        return StepResult.Success();
    })

// 複数条件に基づく分岐
.AddConditionalStep(
    "CheckLoginResult",
    "ログイン結果確認",
    context => context.GetData<bool>("LoginSuccess"),
    // ログイン成功時の追加分岐
    new ConditionalStep(
        "CheckMfaRequired",
        "多要素認証必要性確認",
        context => context.GetData<bool>("RequiresMfa"),
        // 多要素認証が必要
        new NavigationStep("ShowMfa", "MFA画面表示", _navigator, "MfaScreen"),
        // 多要素認証が不要、ロールに基づく分岐
        new ConditionalStep(
            "CheckUserRole",
            "ユーザーロール確認",
            context => context.GetData<string>("UserRole") == "Admin",
            // 管理者向け
            new NavigationStep("ShowAdminPanel", "管理者画面表示", _navigator, "AdminPanelScreen"),
            // 一般ユーザー向け
            new NavigationStep("ShowUserDashboard", "ユーザーダッシュボード表示", _navigator, "UserDashboardScreen")
        )
    ),
    // ログイン失敗
    new NavigationStep("ShowLoginError", "ログインエラー表示", _navigator, "LoginErrorScreen")
)

```

5. 動的条件生成
画面遷移の条件を動的に生成する場合は、ファクトリーパターンと組み合わせて使用できます：
```
public class ScreenConditionFactory
{
    private readonly IFeatureService _featureService;
    
    public ScreenConditionFactory(IFeatureService featureService)
    {
        _featureService = featureService;
    }
    
    public Func<WorkflowContext, bool> CreateCondition(string conditionType)
    {
        return conditionType switch
        {
            "UserIsAdmin" => context => context.GetData<string>("UserRole") == "Admin",
            "UserHasFeature" => context => _featureService.HasFeature(
                context.GetData<string>("UserId"), 
                context.GetData<string>("FeatureName")),
            "UserIsVerified" => context => context.GetData<bool>("IsVerified"),
            _ => _ => false
        };
    }
}

// 使用例
var conditionFactory = new ScreenConditionFactory(_featureService);
var workflow = new WorkflowBuilder("DynamicFlow", "動的条件フロー")
    .AddConditionalStep(
        "DynamicCondition",
        "動的条件確認",
        conditionFactory.CreateCondition("UserHasFeature"),
        new NavigationStep("ShowPremium", "プレミアム機能表示", _navigator, "PremiumFeatureScreen"),
        new NavigationStep("ShowUpgrade", "アップグレード画面表示", _navigator, "UpgradeScreen")
    )
    .Build();

```
まとめ
.NET 8環境でワークフローを使用した条件付き画面遷移の実装方法を紹介しました。主なポイントは：
1.	ConditionalStepを使って、条件に基づいて異なる画面遷移パスを定義できる
2.	WorkflowContextを使って画面間でデータを共有し、条件分岐の判断に利用できる
3.	ネストされたConditionalStepで複雑な条件ロジックを表現できる
4.	動的に条件を生成するファクトリーパターンと組み合わせて使用可能
5.	画面遷移ロジックとビジネスロジックを分離できる
この実装アプローチは、複雑なマルチステップの画面フローや、ユーザーの状態や権限に基づいた条件付き画面遷移に特に有効です。
