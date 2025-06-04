using CoreLibWinforms.Permissions;
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
        public static string PermissionsFilePath = "permissions.json";

        private UserPermissionService _userPermissionManager;
        private PermissionRegistry _permissionMaster;
        private ControlPermissionMapper _controlPermissionMapManager;

        /// <summary>
        /// ユーザー権限管理
        /// </summary>
        public UserPermissionService UserPermissionManager => _userPermissionManager;

        /// <summary>
        /// 権限マスター管理
        /// </summary>
        public PermissionRegistry PermissionMaster => _permissionMaster;

        /// <summary>
        /// コントロール権限マッピング管理
        /// </summary>
        public ControlPermissionMapper ControlPermissionMapManager => _controlPermissionMapManager;

        /// <summary>
        /// 現在のユーザーID
        /// </summary>
        public string CurrentUserId { get; private set; }

        public PermissionSystem()
        {
            _permissionMaster = new PermissionRegistry();
            _userPermissionManager = new UserPermissionService();
            _controlPermissionMapManager = new ControlPermissionMapper(_userPermissionManager, _permissionMaster);
        }

        /// <summary>
        /// 現在のユーザーを設定
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void SetCurrentUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var user = _userPermissionManager.GetUser(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found.");

            CurrentUserId = userId;
        }

        /// <summary>
        /// 現在のユーザーが特定の権限を持っているか確認
        /// </summary>
        /// <param name="permissionId">権限ID</param>
        /// <returns>権限があるかどうか</returns>
        public bool CurrentUserHasPermission(int permissionId)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return false;

            return UserHasPermission(CurrentUserId, permissionId);
        }

        /// <summary>
        /// 指定したユーザーが特定の権限を持っているか確認
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionId">権限ID</param>
        /// <returns>権限があるかどうか</returns>
        public bool UserHasPermission(string userId, int permissionId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = _userPermissionManager.GetUser(userId);
            if (user == null)
                return false;

            // 明示的に拒否された権限をチェック
            if (user.HasDeniedPermission(permissionId))
                return false;

            // 追加で付与された権限をチェック
            if (user.HasAdditionalPermission(permissionId))
                return true;

            // ユーザーのロールに基づいて権限をチェック
            foreach (var roleId in user.AssignedRoleIds)
            {
                if (_permissionMaster.HasPermissionRole(roleId, permissionId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// フォームに対して権限を適用
        /// </summary>
        /// <param name="form">適用対象のフォーム</param>
        public void ApplyPermissionsToForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (string.IsNullOrEmpty(CurrentUserId))
                throw new InvalidOperationException("Current user not set.");

            _controlPermissionMapManager.ApplyPermissionsToForm(form, CurrentUserId);
        }

        /// <summary>
        /// 権限設定を保存
        /// </summary>
        public async Task SavePermissionsAsync()
        {
            var data = new PermissionsData
            {
                Permissions = _permissionMaster.GetAllPermissions().ToList(),
                Roles = _permissionMaster.GetAllRoles().ToList(),
                Users = _userPermissionManager.GetAllUsers().ToList()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using (var fs = File.Create(PermissionsFilePath))
            {
                await JsonSerializer.SerializeAsync(fs, data, options);
            }
        }

        /// <summary>
        /// 権限設定を読み込み
        /// </summary>
        public async Task LoadPermissionsAsync()
        {
            if (!File.Exists(PermissionsFilePath))
                return;

            using (var fs = File.OpenRead(PermissionsFilePath))
            {
                var data = await JsonSerializer.DeserializeAsync<PermissionsData>(fs);
                if (data == null)
                    return;

                // データのインポート処理
                ImportPermissionsData(data);
            }
        }

        /// <summary>
        /// 権限設定データをインポート
        /// </summary>
        private void ImportPermissionsData(PermissionsData data)
        {
            // 現在の設定をクリア
            _permissionMaster = new PermissionRegistry();
            _userPermissionManager = new UserPermissionService();

            // 権限の復元
            foreach (var permission in data.Permissions)
            {
                var newPerm = _permissionMaster.RegisterNewPermission(permission.Name);
                // IDは自動生成されるため、上書きする必要がある場合は追加コードが必要
            }

            // ロールの復元
            foreach (var role in data.Roles)
            {
                var newRole = _permissionMaster.RegisterNewRole(role.Name);
                // 関連する権限を設定
                // この部分はRoleクラスの実装によって異なる
            }

            // ユーザーの復元
            foreach (var user in data.Users)
            {
                var newUser = _userPermissionManager.RegisterNewUserPermission(user.UserId);

                // ロールの割り当て
                foreach (var roleId in user.AssignedRoleIds)
                {
                    _userPermissionManager.AssignRoleToUser(user.UserId, roleId);
                }

                // 追加権限の割り当て
                foreach (var permId in user.AdditionalPermissionIds)
                {
                    _userPermissionManager.GrantAdditionalPermissionToUser(user.UserId, permId);
                }

                // 拒否権限の設定
                foreach (var permId in user.DeniedPermissionIds)
                {
                    _userPermissionManager.DenyPermissionForUser(user.UserId, permId);
                }
            }

            // 最後に新しいControlPermissionMapManagerを作成
            _controlPermissionMapManager = new ControlPermissionMapper(_userPermissionManager, _permissionMaster);
        }

        /// <summary>
        /// 権限データを保持するためのクラス
        /// </summary>
        private class PermissionsData
        {
            public List<Permission> Permissions { get; set; } = new List<Permission>();
            public List<Role> Roles { get; set; } = new List<Role>();
            public List<UserPermissionProfile> Users { get; set; } = new List<UserPermissionProfile>();
        }
    }

}
