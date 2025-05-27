using CoreLib.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// 権限の設定及び確認を担当するクラス
    /// </summary>
    public class PermissionManager
    {
        private static PermissionManager _instance;
        private readonly Dictionary<string, Dictionary<IUserRole, IPermission>> _featurePermissions = new();

        /// <summary>
        /// シングルトンインスタンスの取得
        /// </summary>
        public static PermissionManager Instance => _instance ??= new PermissionManager();

        /// <summary>
        /// 現在のユーザーロール
        /// </summary>
        public IUserRole CurrentUserRole { get; private set; }

        /// <summary>
        /// 現在のユーザーロールを設定
        /// </summary>
        /// <param name="role">ユーザーロール</param>
        public void SetCurrentUserRole(IUserRole role)
        {
            CurrentUserRole = role;
        }

        /// <summary>
        /// 機能に対する権限を設定
        /// </summary>
        /// <param name="featureId">機能ID</param>
        /// <param name="role">ユーザーロール</param>
        /// <param name="permission">権限</param>
        public void SetPermission(string featureId, IUserRole role, IPermission permission)
        {
            if (!_featurePermissions.ContainsKey(featureId))
            {
                _featurePermissions[featureId] = new Dictionary<IUserRole, IPermission>();
            }

            _featurePermissions[featureId][role] = permission;
        }

        /// <summary>
        /// 機能に対する権限を取得
        /// </summary>
        /// <param name="featureId">機能ID</param>
        /// <param name="role">ユーザーロール</param>
        /// <returns>権限</returns>
        public IPermission GetPermission(string featureId, IUserRole role)
        {
            if (_featurePermissions.TryGetValue(featureId, out var rolePermissions) &&
                rolePermissions.TryGetValue(role, out var permission))
            {
                return permission;
            }

            return null;
        }

        /// <summary>
        /// 現在のユーザーが指定した機能の特定の権限を持っているか確認
        /// </summary>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">確認したい権限</param>
        /// <returns>権限を持っているか</returns>
        public bool HasPermission(string featureId, IPermission requiredPermission)
        {
            if (CurrentUserRole == null || requiredPermission == null)
                return false;

            var userPermissions = GetPermission(featureId, CurrentUserRole);
            if (userPermissions == null)
                return false;

            // ApplicationPermissionの場合は専用のチェックロジックを使用
            if (userPermissions is ApplicationPermission appPermission &&
                requiredPermission is ApplicationPermission requiredAppPermission)
            {
                return appPermission.HasPermission(requiredAppPermission);
            }

            // それ以外の場合は数値値で比較
            return (userPermissions.Value & requiredPermission.Value) == requiredPermission.Value;
        }

        /// <summary>
        /// 特定のユーザーロールが指定した機能の特定の権限を持っているか確認
        /// </summary>
        /// <param name="featureId">機能ID</param>
        /// <param name="role">ユーザーロール</param>
        /// <param name="requiredPermission">確認したい権限</param>
        /// <returns>権限を持っているか</returns>
        public bool HasPermission(string featureId, IUserRole role, IPermission requiredPermission)
        {
            if (role == null || requiredPermission == null)
                return false;

            var userPermissions = GetPermission(featureId, role);
            if (userPermissions == null)
                return false;

            // ApplicationPermissionの場合は専用のチェックロジックを使用
            if (userPermissions is ApplicationPermission appPermission &&
                requiredPermission is ApplicationPermission requiredAppPermission)
            {
                return appPermission.HasPermission(requiredAppPermission);
            }

            // それ以外の場合は数値値で比較
            return (userPermissions.Value & requiredPermission.Value) == requiredPermission.Value;
        }

        /// <summary>
        /// ボタンの可視性を権限に基づいて判定
        /// </summary>
        /// <param name="buttonId">ボタンID</param>
        /// <param name="requiredPermission">必要な権限</param>
        /// <returns>表示すべきかどうか</returns>
        public bool ShouldShowButton(string buttonId, IPermission requiredPermission)
        {
            return HasPermission(buttonId, requiredPermission);
        }

        /// <summary>
        /// 指定したユーザーロールに基づいてUIを制御するためのヘルパーメソッド
        /// </summary>
        /// <typeparam name="T">コントロールの型</typeparam>
        /// <param name="control">制御対象のコントロール</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        public void ApplyPermissionToControl<T>(T control, string featureId, IPermission requiredPermission) where T : class
        {
            bool hasPermission = HasPermission(featureId, requiredPermission);

            // WinFormsのButtonやMenuItemなど、様々なコントロールタイプに対応
            if (control is System.Windows.Forms.Control winControl)
            {
                winControl.Visible = hasPermission;
                winControl.Enabled = hasPermission;
            }
            else if (control is System.Windows.Forms.ToolStripItem toolItem)
            {
                toolItem.Visible = hasPermission;
                toolItem.Enabled = hasPermission;
            }
            // 他のUIフレームワークのコントロールにも必要に応じて拡張可能
        }
    }


}
