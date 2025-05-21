using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// アプリケーションコンテキストの基底クラス
    /// </summary>
    public abstract class AppContextBase<TAppSetting, TProject, TProjectContext, TProjectService>
        where TAppSetting : AppSettingBase
        where TProject : ProjectBase, new()
        where TProjectContext : ProjectContextBase<TProject>
        where TProjectService : ProjectServiceBase<TProject, TProjectContext>
    {
        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        public IServiceProvider ServiceProvider { get; protected set; }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        public TAppSetting Settings { get; protected set; }

        /// <summary>
        /// プロジェクトサービス
        /// </summary>
        public TProjectService ProjectService => ServiceProvider.GetRequiredService<TProjectService>();

        /// <summary>
        /// 現在アクティブなプロジェクトコンテキスト
        /// </summary>
        public TProjectContext ActiveProjectContext { get; protected set; }

        /// <summary>
        /// アプリケーションが初期化されたかどうか
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// コンテキストの初期化
        /// </summary>
        public abstract void Initialize(IServiceCollection services);

        /// <summary>
        /// アクティブプロジェクトを設定
        /// </summary>
        public virtual void SetActiveProject(TProjectContext projectContext)
        {
            ActiveProjectContext = projectContext;
        }

        /// <summary>
        /// 設定を保存
        /// </summary>
        public abstract Task SaveSettingsAsync();

        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public abstract Task LoadSettingsAsync();

        /// <summary>
        /// アプリケーションの終了処理
        /// </summary>
        public abstract Task ShutdownAsync();
    }
}
