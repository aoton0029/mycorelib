using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// プロジェクト操作を提供するサービス
    /// </summary>
    public class ProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly AppSetting _appSetting;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProjectService(ILogger<ProjectService> logger, AppSetting appSetting)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSetting = appSetting ?? throw new ArgumentNullException(nameof(appSetting));
        }

        /// <summary>
        /// 新しいプロジェクトを作成
        /// </summary>
        public async Task<ProjectContext> CreateProjectAsync(string name, string description = "")
        {
            _logger.LogInformation($"新規プロジェクト '{name}' を作成します");

            var project = new Project
            {
                Name = name,
                Description = description
            };

            // デフォルトのプロジェクトディレクトリが存在しない場合は作成
            if (!Directory.Exists(_appSetting.DefaultProjectDirectory))
            {
                Directory.CreateDirectory(_appSetting.DefaultProjectDirectory);
            }

            // プロジェクトコンテキストを作成
            var context = new ProjectContext(
                project,
                AppContext.Instance.ServiceProvider.GetRequiredService<ILogger<ProjectContext>>());

            return context;
        }

        /// <summary>
        /// プロジェクトを開く
        /// </summary>
        public async Task<ProjectContext> OpenProjectAsync(string filePath)
        {
            _logger.LogInformation($"プロジェクト '{filePath}' を開きます");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"プロジェクトファイルが見つかりません: {filePath}");
            }

            // ここでプロジェクトファイルを読み込む実装

            // デモ実装として、空のプロジェクトを返す
            var project = new Project
            {
                Name = Path.GetFileNameWithoutExtension(filePath),
                FilePath = filePath,
                IsDirty = false
            };

            var context = new ProjectContext(
                project,
                AppContext.Instance.ServiceProvider.GetRequiredService<ILogger<ProjectContext>>());

            // 最近使用したプロジェクトに追加
            if (!_appSetting.RecentProjects.Contains(filePath))
            {
                _appSetting.RecentProjects.Insert(0, filePath);
                if (_appSetting.RecentProjects.Count > 10) // 最大10個まで保持
                {
                    _appSetting.RecentProjects.RemoveAt(_appSetting.RecentProjects.Count - 1);
                }
            }

            return context;
        }

        /// <summary>
        /// プロジェクトを保存
        /// </summary>
        public async Task SaveProjectAsync(ProjectContext projectContext, string filePath = null)
        {
            if (projectContext == null)
                throw new ArgumentNullException(nameof(projectContext));

            var project = projectContext.Project;

            // ファイルパスが指定されていない場合、プロジェクトの既存のパスを使用
            filePath = filePath ?? project.FilePath;

            // ファイルパスが空の場合、デフォルトのパスを生成
            if (string.IsNullOrEmpty(filePath))
            {
                string fileName = $"{project.Name}_{project.Id}.project";
                filePath = Path.Combine(_appSetting.DefaultProjectDirectory, fileName);
            }

            _logger.LogInformation($"プロジェクト '{project.Name}' を '{filePath}' に保存します");

            // ここでプロジェクトを保存する実装

            // プロジェクトのパスと状態を更新
            project.FilePath = filePath;
            project.IsDirty = false;
            project.LastModifiedAt = DateTime.Now;

            // 最近使用したプロジェクトに追加
            if (!_appSetting.RecentProjects.Contains(filePath))
            {
                _appSetting.RecentProjects.Insert(0, filePath);
                if (_appSetting.RecentProjects.Count > 10) // 最大10個まで保持
                {
                    _appSetting.RecentProjects.RemoveAt(_appSetting.RecentProjects.Count - 1);
                }
            }
        }

        /// <summary>
        /// プロジェクトを閉じる
        /// </summary>
        public async Task CloseProjectAsync(ProjectContext projectContext)
        {
            if (projectContext == null)
                throw new ArgumentNullException(nameof(projectContext));

            _logger.LogInformation($"プロジェクト '{projectContext.Project.Name}' を閉じます");

            // 必要なクリーンアップ処理
            projectContext.Dispose();
        }
    }
}
