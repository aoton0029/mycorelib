using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// アプリケーション設定の基底クラス
    /// </summary>
    public abstract class AppSettingBase : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public virtual string ApplicationName { get; set; } = "App";

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
        public virtual string Version { get; set; } = "1.0.0";

        /// <summary>
        /// テーマ設定
        /// </summary>
        public virtual string Theme { get; set; } = "Light";

        /// <summary>
        /// 言語設定
        /// </summary>
        public virtual string Language { get; set; } = "ja-JP";

        /// <summary>
        /// 設定ファイルのパス
        /// </summary>
        public virtual string SettingsFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AppFramework/settings.json");

        /// <summary>
        /// 設定の検証
        /// </summary>
        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(ApplicationName) && !string.IsNullOrEmpty(Version);
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティ値を設定し、変更があれば通知
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
