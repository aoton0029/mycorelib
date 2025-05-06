using CoreLib.Core.Enums;
using CoreLib.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Entities

{
    /// <summary>
    /// 拡張された基本エンティティ - 監査情報およびソフトデリート機能を実装
    /// </summary>
    public abstract class EntityBase<TKey> :
        IEntity<TKey>,
        IAuditableEntity,
        ISoftDeletable
    {
        /// <summary>
        /// エンティティの一意識別子
        /// </summary>
        public TKey Id { get; set; } = default!;

        /// <summary>
        /// エンティティの作成日時
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// エンティティの作成者ID
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// エンティティの最終更新日時
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// エンティティの最終更新者ID
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// 論理削除フラグ
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 削除日時
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// 削除者ID
        /// </summary>
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// GUIDを識別子として使用する基本エンティティ
    /// </summary>
    public abstract class GuidEntityBase : EntityBase<Guid>
    {
        public GuidEntityBase()
        {
            Id = Guid.NewGuid();
        }
    }

    /// <summary>
    /// 整数を識別子として使用する基本エンティティ（通常、自動採番）
    /// </summary>
    public abstract class IntEntityBase : EntityBase<int>
    {
    }

    /// <summary>
    /// 文字列を識別子として使用する基本エンティティ
    /// </summary>
    public abstract class StringEntityBase : EntityBase<string>
    {
        protected StringEntityBase(string id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// 完全装備エンティティベース - すべての機能を実装
    /// </summary>
    public abstract class FullFeaturedEntityBase<TKey> :
        EntityBase<TKey>,
        IActivatable,
        ISortable,
        IConcurrencyCheckable
    {
        /// <summary>
        /// アクティブ状態
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// エンティティの状態
        /// </summary>
        public EntityStatus Status { get; set; } = EntityStatus.Active;

        /// <summary>
        /// 表示順
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 行バージョン（オプティミスティックロック用）
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
