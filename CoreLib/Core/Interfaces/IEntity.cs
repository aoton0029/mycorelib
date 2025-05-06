using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// 基本エンティティインターフェース - 識別子を持つエンティティ
    /// </summary>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// エンティティの一意識別子
        /// </summary>
        TKey Id { get; set; }
    }

    /// <summary>
    /// 監査可能なエンティティのインターフェース - 作成・更新日時情報を持つ
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// エンティティの作成日時
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// エンティティの作成者ID
        /// </summary>
        string? CreatedBy { get; set; }

        /// <summary>
        /// エンティティの最終更新日時
        /// </summary>
        DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// エンティティの最終更新者ID
        /// </summary>
        string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// ソフトデリート可能なエンティティのインターフェース - 論理削除を実装
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// 論理削除フラグ
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// 削除日時
        /// </summary>
        DateTime? DeletedAt { get; set; }

        /// <summary>
        /// 削除者ID
        /// </summary>
        string? DeletedBy { get; set; }
    }

    /// <summary>
    /// アクティブ状態を持つエンティティのインターフェース
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        /// アクティブ状態
        /// </summary>
        bool IsActive { get; set; }
    }

    /// <summary>
    /// 並び順を持つエンティティのインターフェース
    /// </summary>
    public interface ISortable
    {
        /// <summary>
        /// 表示順
        /// </summary>
        int DisplayOrder { get; set; }
    }

    /// <summary>
    /// コンカレンシー制御用インターフェース
    /// </summary>
    public interface IConcurrencyCheckable
    {
        /// <summary>
        /// 行バージョン（オプティミスティックロック用）
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
