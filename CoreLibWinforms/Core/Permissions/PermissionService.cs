using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class PermissionService
    {
        private PermissionManager _permissionManager;

        public PermissionService(PermissionManager permissionManager = null)
        {
            if (permissionManager == null)
            {
                _permissionManager = new PermissionManager();
                _permissionManager.Load(); // Load permissions from file if available
            }
            else
            {
                _permissionManager = permissionManager;
            }
        }

        #region 高度なユーザー権限チェック機能

        /// <summary>
        /// ユーザーが特定の権限を持っているかをチェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionId">チェックする権限ID</param>
        /// <returns>権限がある場合はtrue</returns>
        public bool HasPermission(string userId, int permissionId)
        {
            var userRole = _permissionManager.GetUserRole(userId);
            if (userRole == null) return false;

            // 追加権限でチェック
            if (userRole.AdditionalPermissions.Contains(permissionId))
                return true;

            // ロール経由の権限をチェック
            foreach (var roleId in userRole.Roles)
            {
                var role = _permissionManager.GetRole(roleId);
                if (role != null && role.Permissions.Contains(permissionId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// ユーザーが特定の名前の権限を持っているかをチェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionName">チェックする権限名</param>
        /// <returns>権限がある場合はtrue</returns>
        public bool HasPermission(string userId, string permissionName)
        {
            var permission = _permissionManager.GetAllPermissions()
                .FirstOrDefault(p => p.Name == permissionName);

            return permission != null && HasPermission(userId, permission.Id);
        }

        /// <summary>
        /// ユーザーが複数の権限のいずれかを持っているかをチェック（OR条件）
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionIds">チェックする権限IDのリスト</param>
        /// <returns>いずれかの権限がある場合はtrue</returns>
        public bool HasAnyPermission(string userId, IEnumerable<int> permissionIds)
        {
            return permissionIds.Any(permId => HasPermission(userId, permId));
        }

        /// <summary>
        /// ユーザーが複数の権限をすべて持っているかをチェック（AND条件）
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissionIds">チェックする権限IDのリスト</param>
        /// <returns>すべての権限がある場合はtrue</returns>
        public bool HasAllPermissions(string userId, IEnumerable<int> permissionIds)
        {
            return permissionIds.All(permId => HasPermission(userId, permId));
        }

        /// <summary>
        /// ユーザーに付与されているすべての権限IDを取得
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザーが持つすべての権限ID</returns>
        public HashSet<int> GetUserPermissions(string userId)
        {
            var userRole = _permissionManager.GetUserRole(userId);
            if (userRole == null) return new HashSet<int>();

            var permissions = new HashSet<int>(userRole.AdditionalPermissions);

            // ロールからの権限を追加
            foreach (var roleId in userRole.Roles)
            {
                var role = _permissionManager.GetRole(roleId);
                if (role != null)
                {
                    permissions.UnionWith(role.Permissions);
                }
            }

            return permissions;
        }

        /// <summary>
        /// ユーザーが特定のロールを持っているかをチェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="roleId">チェックするロールID</param>
        /// <returns>ロールがある場合はtrue</returns>
        public bool HasRole(string userId, int roleId)
        {
            var userRole = _permissionManager.GetUserRole(userId);
            return userRole != null && userRole.Roles.Contains(roleId);
        }

        /// <summary>
        /// ユーザーが特定の名前のロールを持っているかをチェック
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="roleName">チェックするロール名</param>
        /// <returns>ロールがある場合はtrue</returns>
        public bool HasRole(string userId, string roleName)
        {
            var role = _permissionManager.GetAllRoles()
                .FirstOrDefault(r => r.Name == roleName);

            return role != null && HasRole(userId, role.Id);
        }

        #endregion

        #region UI制御関連の機能

        /// <summary>
        /// フォームのコントロールに対してユーザーの権限に基づいて表示・操作制限を適用
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="formName">フォーム名</param>
        /// <param name="controlName">コントロール名</param>
        /// <returns>コントロールの状態（表示、有効、読み取り専用）</returns>
        public (bool Visible, bool Enabled, bool ReadOnly) GetControlRestrictions(string userId, string formName, string controlName)
        {
            bool visible = true;
            bool enabled = true;
            bool readOnly = false;

            var userRole = _permissionManager.GetUserRole(userId);
            if (userRole == null) return (visible, enabled, readOnly);

            var controlPermissions = _permissionManager.GetControlPermissions(formName, controlName);
            if (controlPermissions.Count == 0) return (visible, enabled, readOnly);

            // ユーザーのロールに基づく制限を適用
            foreach (var roleId in userRole.Roles)
            {
                var rolePermissions = controlPermissions
                    .Where(cp => cp.GroupCategory == ControlPermission.GroupCategory_Role && cp.Id == roleId)
                    .ToList();

                foreach (var perm in rolePermissions)
                {
                    visible &= perm.ControlVisible;
                    enabled &= perm.ControlEnabled;
                    readOnly |= perm.ControlReadOnly;
                }
            }

            // 部門ベースの権限は別のユースケースで考慮する必要がある場合は、
            // 部門IDを引数に追加して同様の処理を行う

            return (visible, enabled, readOnly);
        }

        /// <summary>
        /// 指定されたロールに対するすべてのコントロール制限を取得
        /// </summary>
        /// <param name="roleId">ロールID</param>
        /// <returns>コントロール制限のディクショナリ - キー: (フォーム名, コントロール名), 値: (表示, 有効, 読取専用)</returns>
        public Dictionary<(string FormName, string ControlName), (bool Visible, bool Enabled, bool ReadOnly)>
            GetAllControlRestrictionsForRole(int roleId)
        {
            var result = new Dictionary<(string, string), (bool, bool, bool)>();
            var rolePermissions = _permissionManager.GetControlPermissionsByRole(roleId);

            foreach (var perm in rolePermissions)
            {
                var key = (perm.FormName, perm.ControlName);
                result[key] = (perm.ControlVisible, perm.ControlEnabled, perm.ControlReadOnly);
            }

            return result;
        }

        /// <summary>
        /// フォーム内のすべてのコントロールに対してユーザーの権限に基づいて制限を適用
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="form">制限を適用するフォーム</param>
        /// <param name="departmentId">オプショナルな部門ID</param>
        /// <returns>適用されたコントロールの数</returns>
        public int ApplyControlPermissionsToForm(string userId, Form form, int? departmentId = null)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            var userRole = _permissionManager.GetUserRole(userId);
            if (userRole == null) return 0;

            var formName = form.Name;
            int appliedCount = 0;

            // フォーム内のすべてのコントロールを再帰的に処理
            return ApplyPermissionsToControls(form.Controls, formName, userId, departmentId, ref appliedCount);
        }

        private int ApplyPermissionsToControls(Control.ControlCollection controls, string formName,
            string userId, int? departmentId, ref int appliedCount)
        {
            foreach (Control control in controls)
            {
                // コントロールの権限を取得して適用
                var restrictions = GetControlRestrictionsWithDepartment(userId, formName, control.Name, departmentId);

                // コントロールに制限を適用
                ApplyRestrictionsToControl(control, restrictions);
                appliedCount++;

                // 子コントロールがある場合は再帰的に処理
                if (control.Controls.Count > 0)
                {
                    ApplyPermissionsToControls(control.Controls, formName, userId, departmentId, ref appliedCount);
                }
            }

            return appliedCount;
        }

        /// <summary>
        /// コントロールに権限制限を適用
        /// </summary>
        /// <param name="control">制限を適用するコントロール</param>
        /// <param name="restrictions">適用する制限（表示、有効、読み取り専用）</param>
        private void ApplyRestrictionsToControl(Control control, (bool Visible, bool Enabled, bool ReadOnly) restrictions)
        {
            // 表示と有効/無効の設定はすべてのコントロールに適用可能
            control.Visible = restrictions.Visible;
            control.Enabled = restrictions.Enabled;

            // 読み取り専用は特定のコントロールタイプにのみ適用
            if (restrictions.ReadOnly)
            {
                // TextBoxの場合
                if (control is TextBox textBox)
                {
                    textBox.ReadOnly = true;
                }
                // RichTextBoxの場合
                else if (control is RichTextBox richTextBox)
                {
                    richTextBox.ReadOnly = true;
                }
                // DataGridViewの場合
                else if (control is DataGridView dataGridView)
                {
                    dataGridView.ReadOnly = true;
                }
                // ComboBoxの場合（編集不可にする）
                else if (control is ComboBox comboBox)
                {
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                }
            }
        }

        /// <summary>
        /// 部門IDも考慮して、コントロールの制限を取得
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="formName">フォーム名</param>
        /// <param name="controlName">コントロール名</param>
        /// <param name="departmentId">部門ID（オプション）</param>
        /// <returns>コントロールの状態（表示、有効、読み取り専用）</returns>
        public (bool Visible, bool Enabled, bool ReadOnly) GetControlRestrictionsWithDepartment(
            string userId, string formName, string controlName, int? departmentId = null)
        {
            // 基本的な制限を取得（ロールベース）
            var restrictions = GetControlRestrictions(userId, formName, controlName);

            // 部門IDが指定されていれば、部門ベースの制限も考慮
            if (departmentId.HasValue)
            {
                var userRole = _permissionManager.GetUserRole(userId);
                if (userRole == null) return restrictions;

                var controlPermissions = _permissionManager.GetControlPermissions(formName, controlName);
                if (controlPermissions.Count == 0) return restrictions;

                // 部門ベースの制限を適用
                var deptPermissions = controlPermissions
                    .Where(cp => cp.GroupCategory == ControlPermission.GroupCategory_Dept && cp.Id == departmentId.Value)
                    .ToList();

                foreach (var perm in deptPermissions)
                {
                    restrictions.Visible &= perm.ControlVisible;
                    restrictions.Enabled &= perm.ControlEnabled;
                    restrictions.ReadOnly |= perm.ControlReadOnly;
                }
            }

            return restrictions;
        }

        /// <summary>
        /// フォーム内の特定のコントロールを名前で検索して、権限を適用
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="form">対象フォーム</param>
        /// <param name="controlName">コントロール名</param>
        /// <param name="departmentId">部門ID（オプション）</param>
        /// <returns>コントロールが見つかり権限が適用された場合はtrue</returns>
        public bool ApplyControlPermissionByName(string userId, Form form, string controlName, int? departmentId = null)
        {
            var controls = form.Controls.Find(controlName, true);
            if (controls.Length == 0) return false;

            var restrictions = GetControlRestrictionsWithDepartment(userId, form.Name, controlName, departmentId);

            foreach (var control in controls)
            {
                ApplyRestrictionsToControl(control, restrictions);
            }

            return true;
        }

        /// <summary>
        /// 指定したフォームの複数のコントロールに権限を適用
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="form">対象フォーム</param>
        /// <param name="controlNames">コントロール名のリスト</param>
        /// <param name="departmentId">部門ID（オプション）</param>
        /// <returns>適用したコントロールの数</returns>
        public int ApplyControlPermissionsToSpecificControls(string userId, Form form, IEnumerable<string> controlNames,
            int? departmentId = null)
        {
            int count = 0;
            foreach (var controlName in controlNames)
            {
                if (ApplyControlPermissionByName(userId, form, controlName, departmentId))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion

        #region 権限設定のエクスポート・インポート機能

        /// <summary>
        /// 現在の権限設定を指定されたファイルにエクスポート
        /// </summary>
        /// <param name="filePath">エクスポート先のファイルパス</param>
        public void ExportSettings(string filePath)
        {
            string originalPath = PermissionManager.FilePath;
            PermissionManager.FilePath = filePath;

            try
            {
                _permissionManager.Save();
            }
            finally
            {
                // 元のパスを復元
                PermissionManager.FilePath = originalPath;
            }
        }

        /// <summary>
        /// 指定されたファイルから権限設定をインポート
        /// </summary>
        /// <param name="filePath">インポート元のファイルパス</param>
        /// <returns>インポートが成功したかどうか</returns>
        public bool ImportSettings(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return false;

            string originalPath = PermissionManager.FilePath;
            PermissionManager.FilePath = filePath;

            try
            {
                _permissionManager.Load();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                // 元のパスを復元
                PermissionManager.FilePath = originalPath;
            }
        }

        #endregion
    }


    public class PermissionManager
    {
        public static string FilePath = "PermissionSetting.json";
        private Dictionary<int, Permission> _permissions;
        private Dictionary<int, Role> _roles;
        private Dictionary<string, UserRole> _userRoles;
        private Dictionary<int, DepartmentPermission> _departmentPermissions;
        private Dictionary<(string, string), List<ControlPermission>> _controlPermissions;

        public PermissionManager()
        {
            _permissions = new Dictionary<int, Permission>();
            _roles = new Dictionary<int, Role>();
            _userRoles = new Dictionary<string, UserRole>();
            _departmentPermissions = new Dictionary<int, DepartmentPermission>();
            _controlPermissions = new Dictionary<(string, string), List<ControlPermission>>();
        }

        #region Permission
        public Permission AddNewPermission(int id, string name)
        {
            if (_permissions.ContainsKey(id))
            {
                throw new ArgumentException($"Permission with ID {id} already exists.");
            }
            var permission = new Permission { Id = id, Name = name };
            _permissions[id] = permission;
            return permission;
        }

        public Permission AddNewPermission(string name)
        {
            int newId = _permissions.Count > 0 ? _permissions.Keys.Max() + 1 : 0;
            return AddNewPermission(newId, name);
        }

        public bool RemovePermission(int id)
        {
            return _permissions.Remove(id);
        }

        public Permission GetPermission(int id)
        {
            return _permissions.TryGetValue(id, out var permission) ? permission : null;
        }

        public List<Permission> GetAllPermissions()
        {
            return _permissions.Values.ToList();
        }
        #endregion

        #region Role
        public Role AddNewRole(int id, string name)
        {
            if (_roles.ContainsKey(id))
            {
                throw new ArgumentException($"Role with ID {id} already exists.");
            }
            var role = new Role { Id = id, Name = name };
            _roles[id] = role;
            return role;
        }

        public Role AddNewRole(string name)
        {
            int newId = _roles.Count > 0 ? _roles.Keys.Max() + 1 : 0;
            return AddNewRole(newId, name);
        }

        public bool RemoveRole(int id)
        {
            return _roles.Remove(id);
        }

        public Role GetRole(int id)
        {
            return _roles.TryGetValue(id, out var role) ? role : null;
        }

        public List<Role> GetAllRoles()
        {
            return _roles.Values.ToList();
        }

        public bool AddPermissionToRole(int roleId, int permissionId)
        {
            if (!_roles.TryGetValue(roleId, out var role))
                return false;

            if (!_permissions.ContainsKey(permissionId))
                return false;

            return role.Permissions.Add(permissionId);
        }

        public bool RemovePermissionFromRole(int roleId, int permissionId)
        {
            if (!_roles.TryGetValue(roleId, out var role))
                return false;

            return role.Permissions.Remove(permissionId);
        }
        #endregion

        #region UserRole
        public UserRole AddUserRole(string userId)
        {
            if (_userRoles.ContainsKey(userId))
            {
                throw new ArgumentException($"User role with ID {userId} already exists.");
            }
            var userRole = new UserRole { UserId = userId };
            _userRoles[userId] = userRole;
            return userRole;
        }

        public bool RemoveUserRole(string userId)
        {
            return _userRoles.Remove(userId);
        }

        public UserRole GetUserRole(string userId)
        {
            return _userRoles.TryGetValue(userId, out var userRole) ? userRole : null;
        }

        public List<UserRole> GetAllUserRoles()
        {
            return _userRoles.Values.ToList();
        }

        public bool AddRoleToUser(string userId, int roleId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            if (!_roles.ContainsKey(roleId))
                return false;

            return userRole.Roles.Add(roleId);
        }

        public bool RemoveRoleFromUser(string userId, int roleId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            return userRole.Roles.Remove(roleId);
        }

        public bool AddAdditionalPermissionToUser(string userId, int permissionId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            if (!_permissions.ContainsKey(permissionId))
                return false;

            return userRole.AdditionalPermissions.Add(permissionId);
        }

        public bool RemoveAdditionalPermissionFromUser(string userId, int permissionId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            return userRole.AdditionalPermissions.Remove(permissionId);
        }
        #endregion

        #region DepartmentPermission
        public DepartmentPermission AddNewDepartmentPermission(int id, string name)
        {
            if (_departmentPermissions.ContainsKey(id))
            {
                throw new ArgumentException($"Department permission with ID {id} already exists.");
            }
            var deptPermission = new DepartmentPermission { Id = id, Name = name };
            _departmentPermissions[id] = deptPermission;
            return deptPermission;
        }

        public DepartmentPermission AddNewDepartmentPermission(string name)
        {
            int newId = _departmentPermissions.Count > 0 ? _departmentPermissions.Keys.Max() + 1 : 0;
            return AddNewDepartmentPermission(newId, name);
        }

        public bool RemoveDepartmentPermission(int id)
        {
            return _departmentPermissions.Remove(id);
        }

        public DepartmentPermission GetDepartmentPermission(int id)
        {
            return _departmentPermissions.TryGetValue(id, out var deptPermission) ? deptPermission : null;
        }

        public List<DepartmentPermission> GetAllDepartmentPermissions()
        {
            return _departmentPermissions.Values.ToList();
        }

        public bool AddPermissionToDepartment(int departmentId, int permissionId)
        {
            if (!_departmentPermissions.TryGetValue(departmentId, out var deptPermission))
                return false;

            if (!_permissions.ContainsKey(permissionId))
                return false;

            return deptPermission.Permissions.Add(permissionId);
        }

        public bool RemovePermissionFromDepartment(int departmentId, int permissionId)
        {
            if (!_departmentPermissions.TryGetValue(departmentId, out var deptPermission))
                return false;

            return deptPermission.Permissions.Remove(permissionId);
        }
        #endregion

        #region ControlPermission
        public ControlPermission AddControlPermission(string formName, string controlName, string groupCategory, int id,
            bool visible = true, bool enabled = true, bool readOnly = false)
        {
            var key = (formName, controlName);

            if (!_controlPermissions.ContainsKey(key))
            {
                _controlPermissions[key] = new List<ControlPermission>();
            }

            // Check if a control permission with the same group category and ID already exists
            var existingPermission = _controlPermissions[key].FirstOrDefault(cp =>
                cp.GroupCategory == groupCategory && cp.Id == id);

            if (existingPermission != null)
            {
                throw new ArgumentException($"Control permission for {formName}.{controlName} with {groupCategory}={id} already exists.");
            }

            var controlPermission = new ControlPermission
            {
                FormName = formName,
                ControlName = controlName,
                GroupCategory = groupCategory,
                Id = id,
                ControlVisible = visible,
                ControlEnabled = enabled,
                ControlReadOnly = readOnly
            };

            _controlPermissions[key].Add(controlPermission);
            return controlPermission;
        }

        public bool RemoveControlPermission(string formName, string controlName, string groupCategory, int id)
        {
            var key = (formName, controlName);
            if (!_controlPermissions.TryGetValue(key, out var permissions))
                return false;

            var permissionToRemove = permissions.FirstOrDefault(cp =>
                cp.GroupCategory == groupCategory && cp.Id == id);

            if (permissionToRemove == null)
                return false;

            var result = permissions.Remove(permissionToRemove);

            // Remove the key if there are no more permissions for this control
            if (permissions.Count == 0)
                _controlPermissions.Remove(key);

            return result;
        }

        public List<ControlPermission> GetControlPermissions(string formName, string controlName)
        {
            var key = (formName, controlName);
            return _controlPermissions.TryGetValue(key, out var permissions)
                ? permissions
                : new List<ControlPermission>();
        }

        public List<ControlPermission> GetAllControlPermissions()
        {
            return _controlPermissions.Values.SelectMany(list => list).ToList();
        }

        public List<ControlPermission> GetControlPermissionsByRole(int roleId)
        {
            return GetAllControlPermissions()
                .Where(cp => cp.GroupCategory == ControlPermission.GroupCategory_Role && cp.Id == roleId)
                .ToList();
        }

        public List<ControlPermission> GetControlPermissionsByDepartment(int departmentId)
        {
            return GetAllControlPermissions()
                .Where(cp => cp.GroupCategory == ControlPermission.GroupCategory_Dept && cp.Id == departmentId)
                .ToList();
        }
        #endregion

        public void Save()
        {
            var data = new
            {
                Permissions = _permissions,
                Roles = _roles,
                UserRoles = _userRoles,
                DepartmentPermissions = _departmentPermissions,
                ControlPermissions = _controlPermissions.Values.SelectMany(list => list).ToList()
            };

            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(FilePath, jsonString);
        }

        public void Load()
        {
            if (!File.Exists(FilePath)) { return; }

            string jsonString = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<JsonData>(jsonString);

            if (data != null)
            {
                _permissions = data.Permissions;
                _roles = data.Roles;
                _userRoles = data.UserRoles;
                _departmentPermissions = data.DepartmentPermissions;

                // 再構築するために一時変数を作成
                _controlPermissions = new Dictionary<(string, string), List<ControlPermission>>();
                foreach (var controlPermission in data.ControlPermissions)
                {
                    var key = (controlPermission.FormName, controlPermission.ControlName);
                    if (!_controlPermissions.ContainsKey(key))
                    {
                        _controlPermissions[key] = new List<ControlPermission>();
                    }
                    _controlPermissions[key].Add(controlPermission);
                }
            }
        }

        private class JsonData
        {
            public Dictionary<int, Permission> Permissions { get; set; }
            public Dictionary<int, Role> Roles { get; set; }
            public Dictionary<string, UserRole> UserRoles { get; set; }
            public Dictionary<int, DepartmentPermission> DepartmentPermissions { get; set; }
            public List<ControlPermission> ControlPermissions { get; set; }
        }
    }

    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public HashSet<int> Permissions { get; set; } = new HashSet<int>();
    }

    public class UserRole
    {
        public string UserId { get; set; }
        public HashSet<int> Roles { get; set; } = new HashSet<int>();
        public HashSet<int> AdditionalPermissions { get; set; } = new HashSet<int>();
    }

    public class DepartmentPermission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public HashSet<int> Permissions { get; set; } = new HashSet<int>();
    }

    public class ControlPermission
    {
        public const string GroupCategory_Role = "Role";
        public const string GroupCategory_Dept = "Dept";

        public string FormName { get; set; } = string.Empty;
        public string ControlName { get; set; } = string.Empty;
        public string GroupCategory { get; set; } = string.Empty;
        public int Id { get; set; }
        public bool ControlVisible { get; set; }
        public bool ControlEnabled { get; set; }
        public bool ControlReadOnly { get; set; }
    }

    /// <summary>
    /// PermissionManager の拡張メソッドを提供するクラス
    /// </summary>
    public static class PermissionManagerExtensions
    {
        /// <summary>
        /// 列挙型から権限をまとめて登録する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="prefix">権限名の接頭辞（オプション）</param>
        /// <returns>登録された権限の数</returns>
        public static int RegisterPermissionsFromEnum<TEnum>(this PermissionManager manager, string prefix = "")
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var value in Enum.GetValues<TEnum>())
            {
                int id = Convert.ToInt32(value);
                string name = prefix + value.ToString();

                // 権限が既に存在する場合はスキップ
                if (manager.GetPermission(id) != null)
                    continue;

                manager.AddNewPermission(id, name);
                count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型から特定のロールに対して権限を付与する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="roleId">ロールID</param>
        /// <param name="permissions">付与する権限の配列</param>
        /// <returns>付与された権限の数</returns>
        public static int AddPermissionsToRole<TEnum>(this PermissionManager manager, int roleId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.AddPermissionToRole(roleId, permissionId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型から特定のユーザーに対して追加権限を付与する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissions">付与する権限の配列</param>
        /// <returns>付与された権限の数</returns>
        public static int AddPermissionsToUser<TEnum>(this PermissionManager manager, string userId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.AddAdditionalPermissionToUser(userId, permissionId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型から特定の部門に対して権限を付与する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="departmentId">部門ID</param>
        /// <param name="permissions">付与する権限の配列</param>
        /// <returns>付与された権限の数</returns>
        public static int AddPermissionsToDepartment<TEnum>(this PermissionManager manager, int departmentId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.AddPermissionToDepartment(departmentId, permissionId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型から権限を削除する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="permissions">削除する権限の配列</param>
        /// <returns>削除された権限の数</returns>
        public static int RemovePermissions<TEnum>(this PermissionManager manager, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.RemovePermission(permissionId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型からロールの権限を削除する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="roleId">ロールID</param>
        /// <param name="permissions">削除する権限の配列</param>
        /// <returns>削除された権限の数</returns>
        public static int RemovePermissionsFromRole<TEnum>(this PermissionManager manager, int roleId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.RemovePermissionFromRole(roleId, permissionId))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 列挙型からユーザーの追加権限を削除する
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="manager">PermissionManager インスタンス</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissions">削除する権限の配列</param>
        /// <returns>削除された権限の数</returns>
        public static int RemovePermissionsFromUser<TEnum>(this PermissionManager manager, string userId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            int count = 0;
            foreach (var permission in permissions)
            {
                int permissionId = Convert.ToInt32(permission);
                if (manager.RemoveAdditionalPermissionFromUser(userId, permissionId))
                    count++;
            }
            return count;
        }
    }

    /// <summary>
    /// PermissionService の拡張メソッドを提供するクラス
    /// </summary>
    public static class PermissionServiceExtensions
    {
        /// <summary>
        /// ユーザーが指定した列挙型の権限を持っているかをチェック
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="service">PermissionService インスタンス</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permission">チェックする権限</param>
        /// <returns>権限がある場合はtrue</returns>
        public static bool HasPermission<TEnum>(this PermissionService service, string userId, TEnum permission)
            where TEnum : struct, Enum
        {
            int permissionId = Convert.ToInt32(permission);
            return service.HasPermission(userId, permissionId);
        }

        /// <summary>
        /// ユーザーが列挙型で指定した複数の権限のいずれかを持っているかをチェック（OR条件）
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="service">PermissionService インスタンス</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissions">チェックする権限の配列</param>
        /// <returns>いずれかの権限がある場合はtrue</returns>
        public static bool HasAnyPermission<TEnum>(this PermissionService service, string userId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            var permissionIds = permissions.Select(p => Convert.ToInt32(p));
            return service.HasAnyPermission(userId, permissionIds);
        }

        /// <summary>
        /// ユーザーが列挙型で指定した複数の権限をすべて持っているかをチェック（AND条件）
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="service">PermissionService インスタンス</param>
        /// <param name="userId">ユーザーID</param>
        /// <param name="permissions">チェックする権限の配列</param>
        /// <returns>すべての権限がある場合はtrue</returns>
        public static bool HasAllPermissions<TEnum>(this PermissionService service, string userId, params TEnum[] permissions)
            where TEnum : struct, Enum
        {
            var permissionIds = permissions.Select(p => Convert.ToInt32(p));
            return service.HasAllPermissions(userId, permissionIds);
        }

        /// <summary>
        /// 指定したロールがenum型の権限を持っているかチェック
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <param name="service">PermissionService インスタンス</param>
        /// <param name="roleId">ロールID</param>
        /// <param name="permission">チェックする権限</param>
        /// <returns>ロールが権限を持っている場合はtrue</returns>
        public static bool RoleHasPermission<TEnum>(this PermissionService service, int roleId, TEnum permission)
            where TEnum : struct, Enum
        {
            int permissionId = Convert.ToInt32(permission);
            var role = service._permissionManager.GetRole(roleId);
            return role != null && role.Permissions.Contains(permissionId);
        }

        /// <summary>
        /// 列挙型で定義された全権限の配列を取得
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <returns>全権限IDの配列</returns>
        public static int[] GetAllPermissionIds<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>().Select(p => Convert.ToInt32(p)).ToArray();
        }

        /// <summary>
        /// 列挙型で定義された全権限の名前と値の辞書を取得
        /// </summary>
        /// <typeparam name="TEnum">権限IDを表す列挙型</typeparam>
        /// <returns>権限名と権限IDの辞書</returns>
        public static Dictionary<string, int> GetPermissionDictionary<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>().ToDictionary(p => p.ToString(), p => Convert.ToInt32(p));
        }
    }
}
