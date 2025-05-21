using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Projects
{
    /// <summary>
    /// プロジェクトデータモデル
    /// </summary>
    public class Project
    {
        /// <summary>
        /// プロジェクトID
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// プロジェクト名
        /// </summary>
        public string Name { get; set; } = "新規プロジェクト";

        /// <summary>
        /// 作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime LastModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// プロジェクトの説明
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// プロジェクトのファイルパス
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// プロジェクトデータ
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// プロジェクトが変更されたかどうか
        /// </summary>
        public bool IsDirty { get; set; } = false;

        /// <summary>
        /// プロジェクトが変更されたことをマーク
        /// </summary>
        public void MarkAsDirty()
        {
            IsDirty = true;
            LastModifiedAt = DateTime.Now;
        }
    }
}
