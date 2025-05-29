using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public static  class PermissionExtensions
    {
        /// <summary>
        /// コントロールに権限を適用します。
        /// 権限がない場合、コントロールは無効化されます。
        /// </summary>
        /// <param name="control">対象となるコントロール</param>
        /// <param name="permissionId">必要な権限ID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>変更されたコントロール</returns>
        public static T ApplyPermission<T>(this T control, int permissionId, string userId) where T : Control
        {
            bool hasPermission = PermissionHelper.HasPermission(userId, permissionId);
            control.Enabled = hasPermission;
            return control;
        }

        /// <summary>
        /// コントロールとその子コントロールすべてに権限を適用します。
        /// </summary>
        /// <param name="control">ルートコントロール</param>
        /// <param name="permissionId">必要な権限ID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>変更されたコントロール</returns>
        public static T ApplyPermissionToAll<T>(this T control, int permissionId, string userId) where T : Control
        {
            bool hasPermission = PermissionHelper.HasPermission(userId, permissionId);
            control.Enabled = hasPermission;

            foreach (Control childControl in control.Controls)
            {
                ApplyPermissionToAll(childControl, permissionId, userId);
            }

            return control;
        }

        /// <summary>
        /// 特定のタグを持つコントロールに権限を適用します。
        /// </summary>
        /// <param name="control">対象となるコントロール</param>
        /// <param name="permissionId">必要な権限ID</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="tag">対象とするタグ</param>
        /// <returns>変更されたコントロール</returns>
        public static T ApplyPermissionByTag<T>(this T control, int permissionId, string userId, object tag) where T : Control
        {
            if (control.Tag != null && control.Tag.Equals(tag))
            {
                bool hasPermission = PermissionHelper.HasPermission(userId, permissionId);
                control.Enabled = hasPermission;
            }

            return control;
        }

        /// <summary>
        /// ユーザーの全権限をコントロールに自動適用します。
        /// コントロールのTagにPermissionId（整数）が設定されている場合、
        /// そのPermissionIdに対応する権限をチェックします。
        /// </summary>
        /// <param name="container">権限を適用するコンテナ</param>
        /// <param name="userId">ユーザーID</param>
        public static void AutoApplyPermissions(this Control container, string userId)
        {
            var userPermissions = PermissionHelper.GetUserPermissions(userId);
            var permissionIds = userPermissions.Select(p => p.Id).ToHashSet();

            ApplyPermissionsRecursively(container, permissionIds);
        }

        // 再帰的に権限を適用するヘルパーメソッド
        private static void ApplyPermissionsRecursively(Control control, HashSet<int> permissionIds)
        {
            if (control.Tag is int permissionId)
            {
                control.Enabled = permissionIds.Contains(permissionId);
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyPermissionsRecursively(childControl, permissionIds);
            }
        }

        /// <summary>
        /// タグに基づいて複数のコントロールにまとめて権限を適用します。
        /// </summary>
        /// <param name="container">コントロールを含むコンテナ</param>
        /// <param name="permissionId">必要な権限ID</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="tag">対象とするタグ</param>
        public static void ApplyPermissionsByTag(this Control container, int permissionId, string userId, object tag)
        {
            bool hasPermission = PermissionHelper.HasPermission(userId, permissionId);
            ApplyPermissionsByTagRecursively(container, tag, hasPermission);
        }

        // タグに基づいて再帰的に権限を適用するヘルパーメソッド
        private static void ApplyPermissionsByTagRecursively(Control control, object tag, bool hasPermission)
        {
            if (control.Tag != null && control.Tag.Equals(tag))
            {
                control.Enabled = hasPermission;
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyPermissionsByTagRecursively(childControl, tag, hasPermission);
            }
        }

        /// <summary>
        /// Enum値を整数のパーミッションIDに変換します。
        /// </summary>
        /// <typeparam name="TEnum">変換するEnum型</typeparam>
        /// <param name="enumValue">Enum値</param>
        /// <returns>整数化されたパーミッションID</returns>
        public static int ToPermissionId<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            return Convert.ToInt32(enumValue);
        }

        /// <summary>
        /// 複数のEnum値をビットマスクとしてOR演算で結合し、一つの整数値に変換します。
        /// </summary>
        /// <typeparam name="TEnum">変換するEnum型</typeparam>
        /// <param name="enumValues">Enum値の配列</param>
        /// <returns>ビット演算でOR結合された整数値</returns>
        public static int CombinePermissions<TEnum>(params TEnum[] enumValues) where TEnum : Enum
        {
            int result = 0;
            foreach (var value in enumValues)
            {
                result |= Convert.ToInt32(value);
            }
            return result;
        }

        /// <summary>
        /// ビットフラグとしてタグに設定されている権限をチェックします。
        /// </summary>
        /// <param name="control">対象となるコントロール</param>
        /// <param name="requiredPermission">必要な権限のビットフラグ</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>変更されたコントロール</returns>
        public static T ApplyBitPermission<T>(this T control, int requiredPermission, string userId) where T : Control
        {
            var userPermissions = PermissionHelper.GetUserPermissions(userId);
            int userPermissionFlags = 0;

            // ユーザーの持つ全権限をOR演算で結合
            foreach (var permission in userPermissions)
            {
                userPermissionFlags |= permission.Id;
            }

            // ビット演算でパーミッションをチェック（必要な権限がすべて含まれているか）
            bool hasPermission = (userPermissionFlags & requiredPermission) == requiredPermission;
            control.Enabled = hasPermission;

            return control;
        }

        /// <summary>
        /// コントロールのタグに設定されたビットフラグ権限をチェックします。
        /// タグには整数型のビットフラグが設定されていることを前提とします。
        /// </summary>
        /// <param name="control">対象となるコントロール</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>変更されたコントロール</returns>
        public static T ApplyTagBitPermission<T>(this T control, string userId) where T : Control
        {
            if (control.Tag is int permissionBitFlag)
            {
                var userPermissions = PermissionHelper.GetUserPermissions(userId);
                int userPermissionFlags = 0;

                // ユーザーの持つ全権限をOR演算で結合
                foreach (var permission in userPermissions)
                {
                    userPermissionFlags |= permission.Id;
                }

                // ビット演算でパーミッションをチェック
                bool hasPermission = (userPermissionFlags & permissionBitFlag) == permissionBitFlag;
                control.Enabled = hasPermission;
            }

            return control;
        }

        /// <summary>
        /// コンテナ内のすべてのコントロールに対し、タグに設定されたビットフラグ権限をチェックします。
        /// </summary>
        /// <param name="container">権限を適用するコンテナ</param>
        /// <param name="userId">ユーザーID</param>
        public static void AutoApplyBitPermissions(this Control container, string userId)
        {
            var userPermissions = PermissionHelper.GetUserPermissions(userId);
            int userPermissionFlags = 0;

            // ユーザーの持つ全権限をOR演算で結合
            foreach (var permission in userPermissions)
            {
                userPermissionFlags |= permission.Id;
            }

            ApplyBitPermissionsRecursively(container, userPermissionFlags);
        }

        // 再帰的にビット権限を適用するヘルパーメソッド
        private static void ApplyBitPermissionsRecursively(Control control, int userPermissionFlags)
        {
            if (control.Tag is int permissionBitFlag)
            {
                // ビット演算でパーミッションをチェック
                control.Enabled = (userPermissionFlags & permissionBitFlag) == permissionBitFlag;
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyBitPermissionsRecursively(childControl, userPermissionFlags);
            }
        }
    }
}
