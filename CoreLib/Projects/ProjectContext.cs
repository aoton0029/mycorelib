using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// プロジェクトコンテキスト - 特定のプロジェクトに関連する操作と状態
    /// </summary>
    public class ProjectContext : IDisposable
    {
        /// <summary>
        /// 現在のプロジェクト
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// プロジェクトが変更されたイベント
        /// </summary>
        public event EventHandler<Project> ProjectChanged;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger<ProjectContext> _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProjectContext(Project project, ILogger<ProjectContext> logger)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation($"プロジェクトコンテキストを作成しました: {project.Name}");
        }

        /// <summary>
        /// プロジェクトの変更を通知
        /// </summary>
        public void NotifyProjectChanged()
        {
            Project.MarkAsDirty();
            ProjectChanged?.Invoke(this, Project);
            _logger.LogDebug($"プロジェクト '{Project.Name}' が変更されました");
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            _logger.LogInformation($"プロジェクトコンテキストを破棄します: {Project.Name}");
        }
    }
}
