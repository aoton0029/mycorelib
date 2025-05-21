using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// プロジェクトコンテキストの基底クラス
    /// </summary>
    public abstract class ProjectContextBase<TProject> : IDisposable
        where TProject : ProjectBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// 現在のプロジェクト
        /// </summary>
        public TProject Project { get; protected set; }

        /// <summary>
        /// プロジェクトが変更されたイベント
        /// </summary>
        public event EventHandler<TProject> ProjectChanged;

        /// <summary>
        /// 破棄済みフラグ
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ProjectContextBase(TProject project, ILogger logger)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // プロジェクトの変更を監視
            Project.PropertyChanged += (s, e) => NotifyProjectChanged();

            _logger.LogInformation($"プロジェクトコンテキストを作成しました: {project.Name}");
        }

        /// <summary>
        /// プロジェクトの変更を通知
        /// </summary>
        public virtual void NotifyProjectChanged()
        {
            ProjectChanged?.Invoke(this, Project);
            _logger.LogDebug($"プロジェクト '{Project.Name}' が変更されました");
        }

        /// <summary>
        /// オブジェクトの破棄
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _logger.LogInformation($"プロジェクトコンテキストを破棄します: {Project.Name}");
                // マネージドリソースの解放
            }

            _disposed = true;
        }
    }
}
