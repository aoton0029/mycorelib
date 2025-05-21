using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// アプリケーションコンテキスト（シングルトン）
    /// </summary>
    public sealed class AppContext
    {
        // シングルトンインスタンス
        private static readonly Lazy<AppContext> _instance = new Lazy<AppContext>(() => new AppContext());

        /// <summary>
        /// シングルトンインスタンスの取得
        /// </summary>
        public static AppContext Instance => _instance.Value;

        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        public AppSetting Settings { get; private set; }

        /// <summary>
        /// プロジェクトサービス
        /// </summary>
        public ProjectService ProjectService => ServiceProvider.GetRequiredService<ProjectService>();

        /// <summary>
        /// 現在アクティブなプロジェクトコンテキスト
        /// </summary>
        public ProjectContext ActiveProjectContext { get; private set; }

        /// <summary>
        /// アプリケーションが初期化されたかどうか
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// コンストラクタ（private - シングルトン実装）
        /// </summary>
        private AppContext()
        {
            // 初期化はInitializeメソッドで行う
        }

        /// <summary>
        /// コンテキストの初期化
        /// </summary>
        public void Initialize(IServiceCollection services)
        {
            if (IsInitialized)
                return;

            // デフォルト設定の作成
            Settings = new AppSetting();

            // DI設定
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            IsInitialized = true;
        }

        /// <summary>
        /// DIサービスの設定
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            // AppSettingをシングルトンとして登録
            services.AddSingleton(Settings);

            // ロギングの設定
            services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });

            // ProjectServiceの登録
            services.AddSingleton<ProjectService>();
        }

        /// <summary>
        /// アクティブプロジェクトを設定
        /// </summary>
        public void SetActiveProject(ProjectContext projectContext)
        {
            ActiveProjectContext = projectContext;
        }

        /// <summary>
        /// 設定を保存
        /// </summary>
        public async Task SaveSettingsAsync()
        {
            // 設定を保存する実装
        }

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public async Task LoadSettingsAsync()
        {
            // 設定を読み込む実装
        }

        /// <summary>
        /// アプリケーションの終了処理
        /// </summary>
        public async Task ShutdownAsync()
        {
            // アクティブなプロジェクトがある場合は閉じる
            if (ActiveProjectContext != null)
            {
                await ProjectService.CloseProjectAsync(ActiveProjectContext);
            }

            // 設定を保存
            await SaveSettingsAsync();
        }
    }
}
