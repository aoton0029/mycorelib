using CoreLib.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Entities
{
    /// <summary>
    /// エンティティに関するユーティリティ拡張メソッド
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// エンティティを更新済みとしてマーク
        /// </summary>
        public static void MarkAsUpdated<T>(this T entity, string? updatedBy = null) where T : IAuditableEntity
        {
            entity.UpdatedAt = DateTime.UtcNow;

            if (updatedBy != null)
                entity.UpdatedBy = updatedBy;
        }

        /// <summary>
        /// エンティティを削除済みとしてマーク（論理削除）
        /// </summary>
        public static void MarkAsDeleted<T>(this T entity, string? deletedBy = null) where T : ISoftDeletable
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            if (deletedBy != null)
                entity.DeletedBy = deletedBy;
        }

        /// <summary>
        /// エンティティを非アクティブとしてマーク
        /// </summary>
        public static void Deactivate<T>(this T entity) where T : IActivatable
        {
            entity.IsActive = false;
        }

        /// <summary>
        /// エンティティをアクティブとしてマーク
        /// </summary>
        public static void Activate<T>(this T entity) where T : IActivatable
        {
            entity.IsActive = true;
        }

        /// <summary>
        /// エンティティを復元（論理削除を解除）
        /// </summary>
        public static void Restore<T>(this T entity) where T : ISoftDeletable
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedBy = null;
        }
    }
}
