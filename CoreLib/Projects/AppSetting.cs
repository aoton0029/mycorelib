using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// アプリケーション設定を管理するクラス
    /// </summary>
    public class AppSetting
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string ApplicationName { get; set; } = "YourApplication";

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 最近使用したプロジェクトのパス
        /// </summary>
        public List<string> RecentProjects { get; set; } = new List<string>();

        /// <summary>
        /// テーマ設定
        /// </summary>
        public string Theme { get; set; } = "Light";

        /// <summary>
        /// 言語設定
        /// </summary>
        public string Language { get; set; } = "ja-JP";

        /// <summary>
        /// 自動保存間隔（分）
        /// </summary>
        public int AutoSaveIntervalMinutes { get; set; } = 5;

        /// <summary>
        /// デフォルトのプロジェクト保存ディレクトリ
        /// </summary>
        public string DefaultProjectDirectory { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "YourApplication/Projects");

        /// <summary>
        /// 設定が有効かどうかを検証
        /// </summary>
        public bool Validate()
        {
            // 自動保存間隔は1以上の値である必要がある
            if (AutoSaveIntervalMinutes <= 0)
                return false;

            // デフォルトプロジェクトディレクトリが空でないことを確認
            if (string.IsNullOrWhiteSpace(DefaultProjectDirectory))
                return false;

            return true;
        }
    }
}
