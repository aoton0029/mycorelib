using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// ユーザー権限を統括管理するクラス
    /// </summary>
    public class PermissionSystem
    {
        private UserPermissionService _userPermissionManager;
        private PermissionRegistry _permissionMaster;
        private ControlPermissionMapper _controlPermissionMapManager;

        /// <summary>ユーザー権限管理</summary>
        public UserPermissionService UserPermissionManager => _userPermissionManager;

        /// <summary>権限マスター管理</summary>
        public PermissionRegistry PermissionMaster => _permissionMaster;

        /// <summary>コントロール権限マッピング管理</summary>
        public ControlPermissionMapper ControlPermissionMapManager => _controlPermissionMapManager;

        public PermissionSystem()
        {
            _permissionMaster = new PermissionRegistry();
            _userPermissionManager = new UserPermissionService();
            _controlPermissionMapManager = new ControlPermissionMapper();
        }

        /// <summary>
        /// ユーザーIDに基づいて持っている権限を取得し、その権限からコントロールの状態を変更するメソッド
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="control">対象のコントロール</param>
        public void ApplyUserPermissionsToControl(string userId, System.Windows.Forms.Control control)
        {
            if (string.IsNullOrEmpty(userId) || control == null)
                return;

            // ユーザー情報の取得
            var userProfile = _userPermissionManager.GetUser(userId);
            if (userProfile == null)
                return;

            // ユーザーが持つ全ての権限IDを取得
            var effectivePermissionIds = GetUserEffectivePermissions(userId);
            if (effectivePermissionIds.Count == 0)
                return;

            ApplyPermissionsToControl(effectivePermissionIds, control);
        }

        /// <summary>
        /// ユーザーの実効権限IDリストを取得
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>権限IDのリスト</returns>
        public List<int> GetUserEffectivePermissions(string userId)
        {
            var result = new HashSet<int>();

            // ユーザー情報の取得
            var userProfile = _userPermissionManager.GetUser(userId);
            if (userProfile == null)
                return new List<int>();

            // ユーザーに割り当てられているロールからの権限を取得
            foreach (var roleId in userProfile.AssignedRoleIds)
            {
                var role = _permissionMaster.GetRole(roleId);
                if (role != null)
                {
                    // ロールが持つ権限IDを追加
                    for (int i = 0; i < role.Permissions.Length; i++)
                    {
                        if (role.Permissions[i])
                            result.Add(i);
                    }
                }
            }

            // 追加権限を追加
            foreach (var permId in userProfile.AdditionalPermissionIds)
            {
                result.Add(permId);
            }

            // 拒否権限を削除
            foreach (var permId in userProfile.DeniedPermissionIds)
            {
                result.Remove(permId);
            }

            return result.ToList();
        }

        /// <summary>
        /// 権限IDリストに基づいてコントロールの状態を変更
        /// </summary>
        /// <param name="permissionIds">権限IDのリスト</param>
        /// <param name="control">対象のコントロール</param>
        public void ApplyPermissionsToControl(List<int> permissionIds, System.Windows.Forms.Control control)
        {
            if (permissionIds == null || permissionIds.Count == 0 || control == null)
                return;

            bool hasAnyVisibilityPermission = false;
            bool hasAnyEnabledPermission = false;
            bool shouldBeVisible = false;
            bool shouldBeEnabled = false;

            string controlName = control.Name;

            // 各権限に対して設定を確認
            foreach (var permId in permissionIds)
            {
                var settings = _controlPermissionMapManager.GetControlPermissionSettings(permId, controlName);
                if (settings != null)
                {
                    // 表示権限がある場合
                    if (settings.AffectVisibility)
                    {
                        hasAnyVisibilityPermission = true;
                        shouldBeVisible = true;
                    }

                    // 有効化権限がある場合
                    if (settings.AffectEnabled)
                    {
                        hasAnyEnabledPermission = true;
                        shouldBeEnabled = true;
                    }
                }
            }

            // 表示/非表示の制御
            if (hasAnyVisibilityPermission)
            {
                control.Visible = shouldBeVisible;
            }

            // 有効/無効の制御
            if (hasAnyEnabledPermission)
            {
                control.Enabled = shouldBeEnabled;
            }

            // 子コントロールにも適用（再帰的に処理）
            foreach (System.Windows.Forms.Control childControl in control.Controls)
            {
                ApplyPermissionsToControl(permissionIds, childControl);
            }
        }

        /// <summary>
        /// フォーム全体に権限設定を適用
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="form">対象のフォーム</param>
        public void ApplyUserPermissionsToForm(string userId, System.Windows.Forms.Form form)
        {
            if (string.IsNullOrEmpty(userId) || form == null)
                return;

            // ユーザーの効果的な権限を取得
            var effectivePermissionIds = GetUserEffectivePermissions(userId);
            if (effectivePermissionIds.Count == 0)
                return;

            // フォーム自体に権限を適用
            ApplyPermissionsToControl(effectivePermissionIds, form);
        }

        /// <summary>
        /// ユーザーが特定の権限を持っているかチェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionId">権限ID</param>
        /// <returns>権限を持っている場合true、そうでなければfalse</returns>
        public bool UserHasPermission(string userId, int permissionId)
        {
            // ユーザーの効果的な権限を取得
            var effectivePermissions = GetUserEffectivePermissions(userId);
            // 該当の権限IDが含まれるかチェック
            return effectivePermissions.Contains(permissionId);
        }

        public void Save()
        {
            // コントロール権限マッピングをJSONファイルに保存
            _controlPermissionMapManager.Save();
            // ユーザー権限情報をJSONファイルに保存
            _userPermissionManager.Save();
            // 権限マスター情報をJSONファイルに保存
            _permissionMaster.Save();
        }

        public void Load()
        {
            // コントロール権限マッピングをJSONファイルから読み込み
            _controlPermissionMapManager.Load();
            // ユーザー権限情報をJSONファイルから読み込み
            _userPermissionManager.Load();
            // 権限マスター情報をJSONファイルから読み込み
            _permissionMaster.Load();
        }
    }

}
