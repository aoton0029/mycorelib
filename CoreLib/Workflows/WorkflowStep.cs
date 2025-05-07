using CoreLib.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Workflows
{
    /// <summary>
    /// ワークフローの状態
    /// </summary>
    public enum WorkflowStatus
    {
        /// <summary>
        /// 初期状態
        /// </summary>
        NotStarted,

        /// <summary>
        /// 実行中
        /// </summary>
        Running,

        /// <summary>
        /// 完了
        /// </summary>
        Completed,

        /// <summary>
        /// 失敗
        /// </summary>
        Failed,

        /// <summary>
        /// キャンセル
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// ワークフローステップの結果
    /// </summary>
    public class StepResult
    {
        /// <summary>
        /// 成功したかどうか
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// 出力値
        /// </summary>
        public object? Output { get; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// 発生した例外
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// 成功結果
        /// </summary>
        /// <param name="output">ステップの出力値</param>
        /// <returns>成功結果</returns>
        public static StepResult Success(object? output = null) => new(true, output, null, null);

        /// <summary>
        /// 失敗結果
        /// </summary>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="exception">例外</param>
        /// <returns>失敗結果</returns>
        public static StepResult Failure(string errorMessage, Exception? exception = null) =>
            new(false, null, errorMessage, exception);

        private StepResult(bool isSuccess, object? output, string? errorMessage, Exception? exception)
        {
            IsSuccess = isSuccess;
            Output = output;
            ErrorMessage = errorMessage;
            Exception = exception;
        }
    }

    /// <summary>
    /// ワークフローのコンテキスト
    /// </summary>
    public class WorkflowContext
    {
        private readonly Dictionary<string, object> _data = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// コンテキストデータの取得
        /// </summary>
        /// <typeparam name="T">データの型</typeparam>
        /// <param name="key">キー</param>
        /// <returns>取得した値</returns>
        public T? GetData<T>(string key)
        {
            if (_data.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return default;
        }

        /// <summary>
        /// コンテキストデータの設定
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public void SetData(string key, object value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// コンテキストデータの存在確認
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>存在するかどうか</returns>
        public bool HasData(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// コンテキストデータの削除
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>削除できたかどうか</returns>
        public bool RemoveData(string key)
        {
            return _data.Remove(key);
        }

        /// <summary>
        /// コンテキストデータのクリア
        /// </summary>
        public void ClearData()
        {
            _data.Clear();
        }

        /// <summary>
        /// 全てのキーの取得
        /// </summary>
        /// <returns>キーの列挙</returns>
        public IEnumerable<string> GetAllKeys()
        {
            return _data.Keys.ToList();
        }
    }

    /// <summary>
    /// ワークフローステップのインターフェース
    /// </summary>
    public interface IWorkflowStep
    {
        /// <summary>
        /// ステップID
        /// </summary>
        string StepId { get; }

        /// <summary>
        /// ステップの説明
        /// </summary>
        string Description { get; }

        /// <summary>
        /// ステップの実行
        /// </summary>
        /// <param name="context">ワークフローコンテキスト</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        Task<StepResult> ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ワークフローステップの基底クラス
    /// </summary>
    public abstract class WorkflowStepBase : IWorkflowStep
    {
        /// <summary>
        /// ステップID
        /// </summary>
        public string StepId { get; }

        /// <summary>
        /// ステップの説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected WorkflowStepBase(string stepId, string description)
        {
            StepId = stepId ?? throw new ArgumentNullException(nameof(stepId));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        /// <summary>
        /// ステップの実行
        /// </summary>
        public virtual async Task<StepResult> ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return await ExecuteInternalAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                return StepResult.Failure($"ステップ {StepId} の実行中にエラーが発生しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ステップ処理の実装
        /// </summary>
        protected abstract Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 関数ベースのワークフローステップ
    /// </summary>
    public class FunctionStep : WorkflowStepBase
    {
        private readonly Func<WorkflowContext, CancellationToken, Task<StepResult>> _function;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FunctionStep(
            string stepId,
            string description,
            Func<WorkflowContext, CancellationToken, Task<StepResult>> function)
            : base(stepId, description)
        {
            _function = function ?? throw new ArgumentNullException(nameof(function));
        }

        /// <summary>
        /// ステップの内部実行処理
        /// </summary>
        protected override async Task<StepResult> ExecuteInternalAsync(WorkflowContext context, CancellationToken cancellationToken)
        {
            return await _function(context, cancellationToken);
        }
    }

    /// <summary>
    /// 条件分岐ステップ
    /// </summary>
    public class ConditionalStep : WorkflowStepBase
    {
        private readonly Func<WorkflowContext, bool> _condition;
        private readonly IWorkflowStep _trueStep;
        private readonly IWorkflowStep _falseStep;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConditionalStep(
            string stepId,
            string description,
            Func<WorkflowContext, bool> condition,
            IWorkflowStep trueStep,
            IWorkflowStep falseStep)
            : base(stepId, description)
        {
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _trueStep = trueStep ?? throw new ArgumentNullException(nameof(trueStep));
            _falseStep = falseStep ?? throw new ArgumentNullException(nameof(falseStep));
        }

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

    /// <summary>
    /// ワークフロー定義
    /// </summary>
    public class Workflow
    {
        /// <summary>
        /// ワークフローID
        /// </summary>
        public string WorkflowId { get; }

        /// <summary>
        /// ワークフローの説明
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// ワークフローステップのリスト
        /// </summary>
        public IReadOnlyList<IWorkflowStep> Steps => _steps.AsReadOnly();

        /// <summary>
        /// ワークフローの状態
        /// </summary>
        public WorkflowStatus Status { get; private set; } = WorkflowStatus.NotStarted;

        /// <summary>
        /// ワークフローコンテキスト
        /// </summary>
        public WorkflowContext Context { get; }

        /// <summary>
        /// 現在実行中のステップのインデックス
        /// </summary>
        public int CurrentStepIndex { get; private set; } = -1;

        /// <summary>
        /// 現在のステップ結果
        /// </summary>
        public StepResult? CurrentResult { get; private set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string? ErrorMessage { get; private set; }

        private readonly List<IWorkflowStep> _steps = new();
        private readonly IAppLogger? _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Workflow(string workflowId, string description, IAppLogger? logger = null)
        {
            WorkflowId = workflowId ?? throw new ArgumentNullException(nameof(workflowId));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Context = new WorkflowContext();
            _logger = logger;
        }

        /// <summary>
        /// ステップを追加
        /// </summary>
        public Workflow AddStep(IWorkflowStep step)
        {
            if (Status != WorkflowStatus.NotStarted)
                throw new InvalidOperationException("ワークフロー実行開始後にステップを追加することはできません");

            _steps.Add(step);
            return this;
        }

        /// <summary>
        /// ステップをクリア
        /// </summary>
        public Workflow ClearSteps()
        {
            if (Status != WorkflowStatus.NotStarted)
                throw new InvalidOperationException("ワークフロー実行開始後にステップをクリアすることはできません");

            _steps.Clear();
            return this;
        }

        /// <summary>
        /// ワークフローの実行
        /// </summary>
        public async Task<bool> ExecuteAsync(
            CancellationToken cancellationToken = default,
            bool continueOnError = false)
        {
            if (_steps.Count == 0)
            {
                _logger?.LogWarning($"ワークフロー {WorkflowId} には実行するステップがありません");
                Status = WorkflowStatus.Completed;
                return true;
            }

            Status = WorkflowStatus.Running;
            CurrentStepIndex = -1;
            ErrorMessage = null;

            _logger?.LogInformation($"ワークフロー {WorkflowId} - {Description} を開始");

            try
            {
                for (int i = 0; i < _steps.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Status = WorkflowStatus.Cancelled;
                        _logger?.LogInformation($"ワークフロー {WorkflowId} はキャンセルされました");
                        return false;
                    }

                    CurrentStepIndex = i;
                    var step = _steps[i];

                    _logger?.LogInformation($"ステップ {i + 1}/{_steps.Count} [{step.StepId}] - {step.Description} を実行");

                    CurrentResult = await step.ExecuteAsync(Context, cancellationToken);

                    if (!CurrentResult.IsSuccess)
                    {
                        ErrorMessage = CurrentResult.ErrorMessage;
                        _logger?.LogError(CurrentResult.Exception, $"ステップ {step.StepId} は失敗しました: {ErrorMessage}");

                        if (!continueOnError)
                        {
                            Status = WorkflowStatus.Failed;
                            return false;
                        }

                        _logger?.LogWarning($"continueOnError=true のため、次のステップに進みます");
                    }
                    else
                    {
                        _logger?.LogInformation($"ステップ {step.StepId} は成功しました");
                    }
                }

                Status = WorkflowStatus.Completed;
                _logger?.LogInformation($"ワークフロー {WorkflowId} は正常に完了しました");
                return true;
            }
            catch (Exception ex)
            {
                Status = WorkflowStatus.Failed;
                ErrorMessage = $"ワークフロー実行中に予期しないエラーが発生しました: {ex.Message}";
                _logger?.LogError(ex, ErrorMessage);
                return false;
            }
        }

        /// <summary>
        /// ワークフローのリセット
        /// </summary>
        public void Reset()
        {
            Status = WorkflowStatus.NotStarted;
            CurrentStepIndex = -1;
            CurrentResult = null;
            ErrorMessage = null;
            Context.ClearData();
        }
    }

    /// <summary>
    /// ワークフロービルダー
    /// </summary>
    public class WorkflowBuilder
    {
        private readonly Workflow _workflow;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkflowBuilder(string workflowId, string description, IAppLogger? logger = null)
        {
            _workflow = new Workflow(workflowId, description, logger);
        }

        /// <summary>
        /// ステップを追加
        /// </summary>
        public WorkflowBuilder AddStep(IWorkflowStep step)
        {
            _workflow.AddStep(step);
            return this;
        }

        /// <summary>
        /// 関数ステップを追加
        /// </summary>
        public WorkflowBuilder AddFunctionStep(
            string stepId,
            string description,
            Func<WorkflowContext, CancellationToken, Task<StepResult>> function)
        {
            var step = new FunctionStep(stepId, description, function);
            return AddStep(step);
        }

        /// <summary>
        /// 条件分岐ステップを追加
        /// </summary>
        public WorkflowBuilder AddConditionalStep(
            string stepId,
            string description,
            Func<WorkflowContext, bool> condition,
            IWorkflowStep trueStep,
            IWorkflowStep falseStep)
        {
            var step = new ConditionalStep(stepId, description, condition, trueStep, falseStep);
            return AddStep(step);
        }

        /// <summary>
        /// ワークフローをビルド
        /// </summary>
        public Workflow Build()
        {
            return _workflow;
        }
    }
}
