using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands
{
    /// <summary>
    /// ViewModelの基底クラス
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        private bool _isBusy;
        private string _statusMessage = string.Empty;
        private bool _hasErrors;

        /// <summary>
        /// ビジー状態を示すフラグ
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            protected set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// ステータスメッセージ
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            protected set => SetProperty(ref _statusMessage, value);
        }

        /// <summary>
        /// エラーの有無
        /// </summary>
        public bool HasErrors
        {
            get => _hasErrors;
            protected set => SetProperty(ref _hasErrors, value);
        }

        /// <summary>
        /// RelayCommand を作成
        /// </summary>
        protected IRelayCommand CreateCommand(Action execute, Func<bool> canExecute = null)
        {
            return canExecute == null
                ? new RelayCommand(execute)
                : new RelayCommand(execute, canExecute);
        }

        /// <summary>
        /// パラメータ付き RelayCommand を作成
        /// </summary>
        protected IRelayCommand<T> CreateCommand<T>(Action<T> execute, Predicate<T> canExecute = null)
        {
            return canExecute == null
                ? new RelayCommand<T>(execute)
                : new RelayCommand<T>(execute, canExecute);
        }

        /// <summary>
        /// 非同期 RelayCommand を作成
        /// </summary>
        protected IAsyncRelayCommand CreateAsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            return canExecute == null
                ? new AsyncRelayCommand(execute)
                : new AsyncRelayCommand(execute, canExecute);
        }

        /// <summary>
        /// パラメータ付き非同期 RelayCommand を作成
        /// </summary>
        protected IAsyncRelayCommand<T> CreateAsyncCommand<T>(Func<T, Task> execute, Predicate<T> canExecute = null)
        {
            return canExecute == null
                ? new AsyncRelayCommand<T>(execute)
                : new AsyncRelayCommand<T>(execute, canExecute);
        }

        /// <summary>
        /// 安全な非同期コマンド実行をラップ（例外処理とビジー状態管理付き）
        /// </summary>
        protected async Task ExecuteSafelyAsync(Func<Task> action, string successMessage = null)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                HasErrors = false;
                StatusMessage = "処理中...";

                await action();

                if (!string.IsNullOrEmpty(successMessage))
                {
                    StatusMessage = successMessage;
                }
                else
                {
                    StatusMessage = "処理が完了しました";
                }
            }
            catch (Exception ex)
            {
                HasErrors = true;
                StatusMessage = $"エラー: {ex.Message}";
                HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// エラーハンドリングの既定の実装
        /// </summary>
        protected virtual void HandleError(Exception ex)
        {
            // 派生クラスでオーバーライドして独自のエラー処理を実装可能
            System.Diagnostics.Debug.WriteLine($"ViewModel Error: {ex}");
        }

        /// <summary>
        /// プロパティ値をリセット
        /// </summary>
        protected void ResetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetSetMethod(true).IsPublic);

            foreach (var property in properties)
            {
                if (property.PropertyType.IsValueType)
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                }
                else
                {
                    property.SetValue(this, null);
                }
            }
        }
    }

    /// <summary>
    /// サービスロケーター機能を持つViewModel基底クラス
    /// </summary>
    public abstract class ServicedViewModelBase : ViewModelBase
    {
        protected readonly IServiceProvider ServiceProvider;

        protected ServicedViewModelBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// サービスを取得
        /// </summary>
        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>() ??
                throw new InvalidOperationException($"サービス '{typeof(T).Name}' が登録されていません。");
        }

        /// <summary>
        /// オプションのサービスを取得（存在しない場合はnull）
        /// </summary>
        protected T GetOptionalService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }
    }

    /// <summary>
    /// CQRSパターンを用いたViewModel基底クラス
    /// </summary>
    public abstract class CqrsViewModelBase : ViewModelBase
    {
        protected readonly ICommandBus CommandBus;

        protected CqrsViewModelBase(ICommandBus commandBus)
        {
            CommandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
        }

        /// <summary>
        /// コマンドを安全に送信（結果なし）
        /// </summary>
        protected async Task SendCommandSafelyAsync(ICommand command, string successMessage = null)
        {
            await ExecuteSafelyAsync(async () =>
            {
                await CommandBus.SendAsync(command);
            }, successMessage);
        }

        /// <summary>
        /// コマンドを安全に送信（結果あり）
        /// </summary>
        protected async Task<TResult> SendCommandSafelyAsync<TResult>(ICommand<TResult> command, string successMessage = null)
        {
            TResult result = default;

            await ExecuteSafelyAsync(async () =>
            {
                result = await CommandBus.SendAsync(command);
            }, successMessage);

            return result;
        }

        /// <summary>
        /// クエリを安全に送信
        /// </summary>
        protected async Task<TResult> QuerySafelyAsync<TResult>(IQuery<TResult> query, string successMessage = null)
        {
            TResult result = default;

            await ExecuteSafelyAsync(async () =>
            {
                result = await CommandBus.QueryAsync(query);
            }, successMessage);

            return result;
        }
    }

    /// <summary>
    /// CQRSパターンの依存関係登録拡張
    /// </summary>
    public static class CqrsExtensions
    {
        /// <summary>
        /// CQRSパターンの実装を登録
        /// </summary>
        public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
        {
            // MediatRの登録
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(assemblies));

            // コマンドバスの登録
            services.AddScoped<ICommandBus, CommandBus>();

            return services;
        }
    }

    // サンプルのコマンドとクエリ
    public class GetDataQuery : IQuery<string>
    {
        public int Id { get; set; }
    }

    public class SaveDataCommand : ICommand<bool>
    {
        public string Data { get; set; }
    }

    // 拡張ViewModelの使用例
    public class EnhancedExampleViewModel : CqrsViewModelBase
    {
        private string _data;

        public string Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }
        public IAsyncRelayCommand SaveDataCommand { get; }
        public IRelayCommand ResetCommand { get; }

        public EnhancedExampleViewModel(ICommandBus commandBus)
            : base(commandBus)
        {
            LoadDataCommand = CreateAsyncCommand(LoadDataAsync, () => !IsBusy);
            SaveDataCommand = CreateAsyncCommand(SaveDataAsync, () => !IsBusy && !string.IsNullOrEmpty(Data));
            ResetCommand = CreateCommand(Reset);
        }

        private async Task LoadDataAsync()
        {
            var query = new GetDataQuery { Id = 1 };
            Data = await QuerySafelyAsync(query, "データを読み込みました");
        }

        private async Task SaveDataAsync()
        {
            var command = new SaveDataCommand { Data = Data };
            bool success = await SendCommandSafelyAsync(command, "データを保存しました");

            if (success)
            {
                // 保存成功時の追加処理があれば実装
            }
        }

        private void Reset()
        {
            Data = string.Empty;
            StatusMessage = "リセットしました";
            HasErrors = false;
        }

        protected override void HandleError(Exception ex)
        {
            base.HandleError(ex);
            // 特定のエラーに対するカスタム処理
            if (ex is InvalidOperationException)
            {
                StatusMessage = "操作が無効です。正しい入力を確認してください。";
            }
        }
    }
}
