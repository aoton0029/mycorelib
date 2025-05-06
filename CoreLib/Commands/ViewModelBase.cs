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

    public class ExampleViewModel : ViewModelBase
    {
        private readonly ICommandBus _commandBus;
        private string _data;

        public string Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public IAsyncRelayCommand LoadDataCommand { get; }
        public IAsyncRelayCommand SaveDataCommand { get; }

        public ExampleViewModel(ICommandBus commandBus)
        {
            _commandBus = commandBus;

            LoadDataCommand = CreateAsyncCommand(LoadDataAsync);
            SaveDataCommand = CreateAsyncCommand(SaveDataAsync);
        }

        private async Task LoadDataAsync()
        {
            var query = new GetDataQuery { Id = 1 };
            Data = await _commandBus.QueryAsync(query);
        }

        private async Task SaveDataAsync()
        {
            var command = new SaveDataCommand { Data = Data };
            bool success = await _commandBus.SendAsync(command);

            if (success)
            {
                // 保存成功時の処理
            }
        }
    }
}
