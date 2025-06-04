using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// コントロールの権限設定を保持するクラス
    /// </summary>
    public class ControlPermissionSettings
    {
        /// <summary>
        /// コントロールの名前
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 権限ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 表示/非表示を制御するか
        /// </summary>
        public bool AffectVisibility { get; set; } = true;

        /// <summary>
        /// 有効/無効を制御するか
        /// </summary>
        public bool AffectEnabled { get; set; } = true;
    }

    public class ControlPermissionMapper
    {
        private readonly Dictionary<string, List<ControlPermissionSettings>> _controlPermissionMaps;
        private UserPermissionService _userPermissionManager;
        private PermissionRegistry _permissionMaster;

        public ControlPermissionMapper(UserPermissionService userPermissionManager, PermissionRegistry permissionMaster)
        {
            _controlPermissionMaps = new Dictionary<string, List<ControlPermissionSettings>>();
            _userPermissionManager = userPermissionManager ?? throw new ArgumentNullException(nameof(userPermissionManager));
            _permissionMaster = permissionMaster ?? throw new ArgumentNullException(nameof(permissionMaster));
        }

        /// <summary>
        /// コントロールに権限を関連付ける
        /// </summary>
        public void RegisterControl(string controlName, int permissionId, bool affectVisibility = true, bool affectEnabled = true)
        {
            if (!_controlPermissionMaps.ContainsKey(controlName))
            {
                _controlPermissionMaps[controlName] = new List<ControlPermissionSettings>();
            }

            // 同じ権限IDの設定がすでに存在する場合は更新
            var existingSetting = _controlPermissionMaps[controlName]
                .FirstOrDefault(s => s.PermissionId == permissionId);

            if (existingSetting != null)
            {
                existingSetting.AffectVisibility = affectVisibility;
                existingSetting.AffectEnabled = affectEnabled;
            }
            else
            {
                _controlPermissionMaps[controlName].Add(
                    new ControlPermissionSettings
                    {
                        ControlName = controlName,
                        PermissionId = permissionId,
                        AffectVisibility = affectVisibility,
                        AffectEnabled = affectEnabled
                    });
            }
        }

        public void UnregisterControl(string controlName, int permissionId)
        {
            if (_controlPermissionMaps.TryGetValue(controlName, out var settings))
            {
                settings.RemoveAll(x => x.PermissionId == permissionId);

                // 設定が空になったらキー自体を削除
                if (settings.Count == 0)
                {
                    _controlPermissionMaps.Remove(controlName);
                }
            }
        }

        /// <summary>
        /// ユーザーの権限に基づいてコントロールに権限を適用
        /// </summary>
        /// <param name="controlName">コントロール名</param>
        /// <param name="control">適用対象のコントロール</param>
        /// <param name="userId">ユーザーID (省略時は現在のユーザー)</param>
        public void ApplyPermissionsToControl(string controlName, Control control, string userId = null)
        {
            if (string.IsNullOrEmpty(controlName))
                throw new ArgumentNullException(nameof(controlName));

            if (control == null)
                throw new ArgumentNullException(nameof(control));

            string effectiveUserId = userId;
            if (string.IsNullOrEmpty(effectiveUserId))
                throw new InvalidOperationException("User ID is not specified or set.");

            var user = _userPermissionManager.GetUser(effectiveUserId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {effectiveUserId} not found.");

            if (!_controlPermissionMaps.TryGetValue(controlName, out var settings) || settings.Count == 0)
                return; // 権限設定がない場合は何もしない

            bool hasAnyPermission = false;

            foreach (var setting in settings)
            {
                bool hasPerm = UserHasPermission(effectiveUserId, setting.PermissionId);

                if (hasPerm)
                {
                    hasAnyPermission = true;
                    break;
                }
            }

            // 権限がない場合の処理
            if (!hasAnyPermission)
            {
                // すべての権限設定を確認し、AffectVisibilityとAffectEnabledフラグに応じて設定
                bool shouldBeVisible = true;
                bool shouldBeEnabled = true;

                foreach (var setting in settings)
                {
                    if (setting.AffectVisibility)
                        shouldBeVisible = false;

                    if (setting.AffectEnabled)
                        shouldBeEnabled = false;
                }

                if (!shouldBeVisible)
                    control.Visible = false;

                if (!shouldBeEnabled)
                    control.Enabled = false;
            }
        }
        /// <summary>
        /// ユーザーの権限に基づいてフォーム上のすべてのコントロールに権限を適用
        /// </summary>
        /// <param name="form">適用対象のフォーム</param>
        /// <param name="userId">ユーザーID (省略時は現在のユーザー)</param>
        public void ApplyPermissionsToForm(Form form, string userId = null)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // フォーム上のすべてのコントロールを再帰的に処理
            ApplyPermissionsToControlsRecursive(form.Controls, userId);
        }

        /// <summary>
        /// コントロールコレクションに対して再帰的に権限を適用
        /// </summary>
        private void ApplyPermissionsToControlsRecursive(Control.ControlCollection controls, string userId)
        {
            foreach (Control control in controls)
            {
                // このコントロールに権限設定があれば適用
                if (_controlPermissionMaps.ContainsKey(control.Name))
                {
                    ApplyPermissionsToControl(control.Name, control, userId);
                }

                // 子コントロールがあれば再帰的に適用
                if (control.Controls.Count > 0)
                {
                    ApplyPermissionsToControlsRecursive(control.Controls, userId);
                }
            }
        }

        /// <summary>
        /// 特定のフォーム用のマッピングをクリア
        /// </summary>
        public void ClearFormMapping(string formName)
        {
            if (string.IsNullOrEmpty(formName))
                throw new ArgumentNullException(nameof(formName));

            var keysToRemove = _controlPermissionMaps.Keys
                .Where(key => key.StartsWith(formName + "."))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _controlPermissionMaps.Remove(key);
            }
        }

        /// <summary>
        /// すべてのマッピングをクリア
        /// </summary>
        public void ClearAllMappings()
        {
            _controlPermissionMaps.Clear();
        }

        /// <summary>
        /// 指定したユーザーが権限を持っているかどうかを判定
        /// </summary>
        private bool UserHasPermission(string userId, int permissionId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = _userPermissionManager.GetUser(userId);
            if (user == null)
                return false;

            // 明示的に拒否された権限はチェック
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
        /// 現在のマッピング情報を取得
        /// </summary>
        public IEnumerable<KeyValuePair<string, List<ControlPermissionSettings>>> GetAllMappings()
        {
            return _controlPermissionMaps;
        }

    }
}
