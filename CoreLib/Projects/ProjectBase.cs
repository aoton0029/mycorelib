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
    /// プロジェクトの基底クラス
    /// </summary>
    public abstract class ProjectBase : INotifyPropertyChanged
    {
        private string _name = "新規プロジェクト";
        private string _description = string.Empty;
        private bool _isDirty = false;

        /// <summary>
        /// プロパティ変更通知イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロジェクトID
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// プロジェクトの説明
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime LastModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// プロジェクトのファイルパス
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// プロジェクトが変更されたかどうか
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        /// <summary>
        /// プロジェクトが変更されたことをマーク
        /// </summary>
        public virtual void MarkAsDirty()
        {
            IsDirty = true;
            LastModifiedAt = DateTime.Now;
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // 自動的にdirtyフラグを設定 (ただしIsDirtyプロパティ自体の変更は除く)
            if (propertyName != nameof(IsDirty) && propertyName != nameof(LastModifiedAt))
            {
                MarkAsDirty();
            }
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
