using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// プロジェクトサービスの基底クラス
    /// </summary>
    public abstract class ProjectServiceBase<TProject, TProjectContext>
        where TProject : ProjectBase, new()
        where TProjectContext : ProjectContextBase<TProject>
    {
        /// <summary>
        /// ロガー
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        protected readonly AppSettingBase _appSetting;

        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        protected readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ProjectServiceBase(
            ILogger logger,
            AppSettingBase appSetting,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSetting = appSetting ?? throw new ArgumentNullException(nameof(appSetting));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// 新しいプロジェクトを作成
        /// </summary>
        public abstract Task<TProjectContext> CreateProjectAsync(string name, string description = "");

        /// <summary>
        /// プロジェクトを開く
        /// </summary>
        public abstract Task<TProjectContext> OpenProjectAsync(string filePath);

        /// <summary>
        /// プロジェクトを保存
        /// </summary>
        public abstract Task SaveProjectAsync(TProjectContext projectContext, string filePath = null);

        /// <summary>
        /// プロジェクトを閉じる
        /// </summary>
        public abstract Task CloseProjectAsync(TProjectContext projectContext);

        /// <summary>
        /// プロジェクトコンテキストを作成
        /// </summary>
        protected abstract TProjectContext CreateProjectContext(TProject project);
    }
}
