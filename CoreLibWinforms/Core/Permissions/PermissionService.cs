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
        private readonly string _permissionsFilePath = "permissions.json";
        private readonly string _rolesFilePath = "roles.json";
        private readonly string _userRolesFilePath = "user_roles.json";
        private readonly string _controlMappingsFilePath = "controlmapping.json";

        private readonly Dictionary<string, Permission> _permissions;
        private readonly Dictionary<string, Role> _roles;
        private readonly Dictionary<string, UserRole> _userRoles;
        private readonly Dictionary<string, List<ControlPermissionMapping>> _controlMappings;

        public PermissionService()
        {
            _permissions = new Dictionary<string, Permission>();
            _roles = new Dictionary<string, Role>();
            _userRoles = new Dictionary<string, UserRole>();
            _controlMappings = new Dictionary<string, List<ControlPermissionMapping>>();
        }

        #region 権限
        public List<Permission> GetAllPermissions() => _permissions.Values.ToList();

        public void AddNewPermission(string id, string name)
        {
            var permission = new Permission
            {
                Id = id,
                Name = name
            };
            _permissions[id] = permission;
        }

        public void RemovePermission(string id)
        {
            _permissions.Remove(id);

            // 関連するロールから権限を削除
            foreach (var role in _roles.Values)
            {
                role.Permissions.Remove(id);
            }

            // ユーザーの追加権限からも削除
            foreach (var userRole in _userRoles.Values)
            {
                userRole.AdditionalPermissions.Remove(id);
            }
        }

        public Permission GetPermission(string id)
        {
            return _permissions.TryGetValue(id, out var permission) ? permission : null;
        }

        // 権限に依存関係を追加
        public void AddDependencyToPermission(string permissionId, string dependsOnId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                permission.DependsOn.Add(dependsOnId);
            }
        }

        // 権限から依存関係を削除
        public void RemoveDependencyFromPermission(string permissionId, string dependsOnId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                permission.DependsOn.Remove(dependsOnId);
            }
        }

        // 特定の権限の依存関係をすべて取得
        public HashSet<string> GetPermissionDependencies(string permissionId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                return new HashSet<string>(permission.DependsOn);
            }
            return new HashSet<string>();
        }
        #endregion

        #region ロール
        public List<Role> GetAllRoles() => _roles.Values.ToList();

        public void AddNewRole(string roleId, string name)
        {
            var role = new Role
            {
                Id = roleId,
                Name = name
            };
            _roles[roleId] = role;
        }

        public void RemoveRole(string roleId)
        {
            _roles.Remove(roleId);

            // ユーザーロールからこのロールを削除
            foreach (var userRole in _userRoles.Values)
            {
                userRole.Roles.Remove(roleId);
            }
        }

        public Role GetRole(string roleId)
        {
            return _roles.TryGetValue(roleId, out var role) ? role : null;
        }

        // ロールに権限を追加
        public void AddPermissionToRole(string roleId, string permissionId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                role.Permissions.Add(permissionId);
            }
        }

        // ロールから権限を削除
        public void RemovePermissionFromRole(string roleId, string permissionId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                role.Permissions.Remove(permissionId);
            }
        }

        // 複数の権限をロールに一括追加
        public void AddPermissionsToRole(string roleId, IEnumerable<string> permissionIds)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                foreach (var permissionId in permissionIds)
                {
                    role.Permissions.Add(permissionId);
                }
            }
        }

        // ロールに割り当てられた権限の一覧を取得
        public HashSet<string> GetRolePermissions(string roleId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                return new HashSet<string>(role.Permissions);
            }
            return new HashSet<string>();
        }
        #endregion

        #region ユーザーロール
        public List<UserRole> GetAllUserRoles() => _userRoles.Values.ToList();

        public void AssignRoleToUser(string userId, string roleId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            if (!userRole.Roles.Contains(roleId))
            {
                userRole.Roles.Add(roleId);
            }
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole.Roles.Remove(roleId);
            }
        }

        public void GrantAdditionalPermissionToUser(string userId, string permissionId)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            if (!userRole.AdditionalPermissions.Contains(permissionId))
            {
                userRole.AdditionalPermissions.Add(permissionId);
            }
        }

        public void RevokeAdditionalPermissionFromUser(string userId, string permissionId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole.AdditionalPermissions.Remove(permissionId);
            }
        }

        public UserRole GetUserRole(string userId)
        {
            return _userRoles.TryGetValue(userId, out var userRole) ? userRole : null;
        }

        // 複数のロールをユーザーに一括割り当て
        public void AssignRolesToUser(string userId, IEnumerable<string> roleIds)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            foreach (var roleId in roleIds)
            {
                userRole.Roles.Add(roleId);
            }
        }

        // 複数の追加権限をユーザーに一括付与
        public void GrantAdditionalPermissionsToUser(string userId, IEnumerable<string> permissionIds)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            foreach (var permissionId in permissionIds)
            {
                userRole.AdditionalPermissions.Add(permissionId);
            }
        }

        // ユーザーに割り当てられたロールの一覧を取得
        public HashSet<string> GetUserRoles(string userId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                return new HashSet<string>(userRole.Roles);
            }
            return new HashSet<string>();
        }

        // ユーザーに付与された追加権限の一覧を取得
        public HashSet<string> GetUserAdditionalPermissions(string userId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                return new HashSet<string>(userRole.AdditionalPermissions);
            }
            return new HashSet<string>();
        }

        // ユーザーが持つすべての権限（ロール由来と追加権限）を取得
        public HashSet<string> GetAllUserPermissions(string userId)
        {
            var allPermissions = new HashSet<string>();

            if (!_userRoles.TryGetValue(userId, out var userRole))
                return allPermissions;

            // 追加権限を追加
            foreach (var permissionId in userRole.AdditionalPermissions)
            {
                allPermissions.Add(permissionId);

                // 依存権限も追加
                if (_permissions.TryGetValue(permissionId, out var permission))
                {
                    foreach (var dependsOnId in permission.DependsOn)
                    {
                        allPermissions.Add(dependsOnId);
                    }
                }
            }

            // ロール由来の権限を追加
            foreach (var roleId in userRole.Roles)
            {
                if (_roles.TryGetValue(roleId, out var role))
                {
                    foreach (var permissionId in role.Permissions)
                    {
                        allPermissions.Add(permissionId);

                        // 依存権限も追加
                        if (_permissions.TryGetValue(permissionId, out var permission))
                        {
                            foreach (var dependsOnId in permission.DependsOn)
                            {
                                allPermissions.Add(dependsOnId);
                            }
                        }
                    }
                }
            }

            return allPermissions;
        }
        #endregion

        #region コントロールマッピング
        public List<ControlPermissionMapping> GetControlMappings(string formName)
        {
            if (_controlMappings.TryGetValue(formName, out var mappings))
            {
                return mappings;
            }
            return new List<ControlPermissionMapping>();
        }

        public List<string> GetAllFormNames()
        {
            return _controlMappings.Keys.ToList();
        }

        public void AddControlMapping(string formName, string controlName, string permissionId, RestrictType restrictType)
        {
            if (!_controlMappings.TryGetValue(formName, out var formMappings))
            {
                formMappings = new List<ControlPermissionMapping>();
                _controlMappings[formName] = formMappings;
            }

            // 既存のマッピングを確認し、あれば更新
            var existingMapping = formMappings.FirstOrDefault(m =>
                m.ControlName == controlName && m.RestrictType == restrictType);

            if (existingMapping != null)
            {
                existingMapping.PermissionId = permissionId;
            }
            else
            {
                // 新しいマッピングを追加（FormNameプロパティも設定）
                formMappings.Add(new ControlPermissionMapping
                {
                    FormName = formName,  // FormNameプロパティを設定
                    ControlName = controlName,
                    PermissionId = permissionId,
                    RestrictType = restrictType
                });
            }
        }

        public void RemoveControlMapping(string formName, string controlName, RestrictType restrictType)
        {
            if (_controlMappings.TryGetValue(formName, out var formMappings))
            {
                formMappings.RemoveAll(m => m.ControlName == controlName && m.RestrictType == restrictType);

                // マッピングが空になった場合はフォームエントリ自体を削除
                if (formMappings.Count == 0)
                {
                    _controlMappings.Remove(formName);
                }
            }
        }

        public bool IsControlRestricted(string formName, string controlName, RestrictType restrictType, string userId)
        {
            // 指定されたフォームのマッピングを取得
            if (!_controlMappings.TryGetValue(formName, out var formMappings))
            {
                return false; // マッピングがなければ制限なし
            }

            // 指定されたコントロールと制限タイプに一致するマッピングを検索
            var mapping = formMappings.FirstOrDefault(m =>
                m.ControlName == controlName && m.RestrictType == restrictType);

            if (mapping == null)
            {
                return false; // マッピングがなければ制限なし
            }

            // ユーザーが必要な権限を持っているか確認
            // 持っていなければ制限あり（true）、持っていれば制限なし（false）
            return !HasPermission(userId, mapping.PermissionId);
        }

        // 特定のフォームのすべてのコントロール制限情報を取得
        public Dictionary<string, List<ControlPermissionMapping>> GetControlRestrictionsForUser(string formName, string userId)
        {
            var result = new Dictionary<string, List<ControlPermissionMapping>>();

            if (!_controlMappings.TryGetValue(formName, out var formMappings))
            {
                return result; // マッピングがなければ空のディクショナリを返す
            }

            // コントロール名でグループ化
            foreach (var controlGroup in formMappings.GroupBy(m => m.ControlName))
            {
                var controlName = controlGroup.Key;
                var restrictedMappings = new List<ControlPermissionMapping>();

                foreach (var mapping in controlGroup)
                {
                    // ユーザーが権限を持っていなければ、制限対象としてリストに追加
                    if (!HasPermission(userId, mapping.PermissionId))
                    {
                        restrictedMappings.Add(mapping);
                    }
                }

                if (restrictedMappings.Any())
                {
                    result[controlName] = restrictedMappings;
                }
            }

            return result;
        }
        #endregion

        // ユーザーが特定の権限を持っているか確認するメソッド
        public bool HasPermission(string userId, string permissionId)
        {
            // ユーザーロールが存在しない場合は権限なし
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            // 追加権限に直接含まれているか確認
            if (userRole.AdditionalPermissions.Contains(permissionId))
                return true;

            // ユーザーのロールに関連付けられた権限を確認
            foreach (var roleId in userRole.Roles)
            {
                if (_roles.TryGetValue(roleId, out var role) && role.Permissions.Contains(permissionId))
                    return true;

                // 権限の依存関係も確認
                if (role != null)
                {
                    foreach (var permission in role.Permissions)
                    {
                        if (permission == permissionId ||
                            (_permissions.TryGetValue(permission, out var perm) &&
                             perm.DependsOn.Contains(permissionId)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #region UIコントロール制限の適用
        /// <summary>
        /// ユーザーIDに基づいてフォームのUIコントロール状態を変更します
        /// </summary>
        /// <param name="form">制限を適用するフォーム</param>
        /// <param name="userId">ユーザーID</param>
        public void ApplyUIRestrictions(Form form, string userId)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ユーザーIDが指定されていません", nameof(userId));

            // フォーム名を取得
            string formName = form.Name;

            // このフォームに対するすべての制限マッピングを取得
            var restrictions = GetControlRestrictionsForUser(formName, userId);

            // 各コントロールに対して制限を適用
            foreach (var controlEntry in restrictions)
            {
                string controlName = controlEntry.Key;
                var mappings = controlEntry.Value;

                // コントロール名でコントロールを検索
                var control = FindControlRecursive(form, controlName);
                if (control != null)
                {
                    // 制限タイプ別に状態を変更
                    foreach (var mapping in mappings)
                    {
                        switch (mapping.RestrictType)
                        {
                            case RestrictType.Visibility:
                                control.Visible = false;
                                break;
                            case RestrictType.Enabled:
                                control.Enabled = false;
                                break;
                            case RestrictType.ReadOnly:
                                SetControlReadOnly(control, true);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// フォーム内のコントロールを再帰的に検索します
        /// </summary>
        private Control FindControlRecursive(Control parent, string controlName)
        {
            // 直接の子コントロールを検索
            Control foundControl = parent.Controls[controlName];
            if (foundControl != null)
                return foundControl;

            // 再帰的に子コントロールを検索
            foreach (Control control in parent.Controls)
            {
                foundControl = FindControlRecursive(control, controlName);
                if (foundControl != null)
                    return foundControl;
            }

            return null;
        }

        /// <summary>
        /// コントロールを読み取り専用に設定します
        /// </summary>
        private void SetControlReadOnly(Control control, bool readOnly)
        {
            // コントロールタイプに応じて適切なプロパティを設定
            if (control is TextBox textBox)
            {
                textBox.ReadOnly = readOnly;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.Enabled = !readOnly;
            }
            else if (control is DataGridView dataGridView)
            {
                dataGridView.ReadOnly = readOnly;
            }
            else if (control is CheckBox checkBox ||
                     control is RadioButton radioButton ||
                     control is Button button)
            {
                control.Enabled = !readOnly;
            }
            // 他のコントロールタイプも必要に応じて追加
        }
        #endregion
        public void Save()
        {
            PermissionFileLoader.SavePermissions(_permissionsFilePath, _permissions.Values.ToList());
            PermissionFileLoader.SaveRoles(_rolesFilePath, _roles.Values.ToList());
            PermissionFileLoader.SaveUserRoles(_userRolesFilePath, _userRoles.Values.ToList());
            var allMappings = _controlMappings.Values.SelectMany(list => list).ToList();
            PermissionFileLoader.SaveControlMappings(_controlMappingsFilePath, allMappings);
        }

        public void Load()
        {
            // 権限をロード
            var permissions = PermissionFileLoader.LoadPermissions(_permissionsFilePath);
            _permissions.Clear();
            foreach (var permission in permissions)
            {
                _permissions.TryAdd(permission.Id, permission);
            }

            // ロールをロード
            var roles = PermissionFileLoader.LoadRoles(_rolesFilePath);
            _roles.Clear();
            foreach (var role in roles)
            {
                _roles.TryAdd(role.Id, role);
            }

            // ユーザーロールをロード
            var userRoles = PermissionFileLoader.LoadUserRoles(_userRolesFilePath);
            _userRoles.Clear();
            foreach (var userRole in userRoles)
            {
                _userRoles.TryAdd(userRole.UserId, userRole);
            }

            // コントロールマッピングをロード
            var mappings = PermissionFileLoader.LoadControlMappings(_controlMappingsFilePath);
            _controlMappings.Clear();

            // フォーム名でグループ化してコントロールマッピングを追加
            foreach (var grouping in mappings.GroupBy(m => m.FormName))
            {
                _controlMappings.TryAdd(grouping.Key, grouping.ToList());
            }
        }

    }

    public class PermissionFileLoader
    {
        public static List<Permission> LoadPermissions(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Permission>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Permission>>(json) ?? new List<Permission>();
        }

        public static List<Role> LoadRoles(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Role>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Role>>(json) ?? new List<Role>();
        }

        public static List<UserRole> LoadUserRoles(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<UserRole>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<UserRole>>(json) ?? new List<UserRole>();
        }

        public static List<ControlPermissionMapping> LoadControlMappings(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<ControlPermissionMapping>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<ControlPermissionMapping>>(json) ?? new List<ControlPermissionMapping>();
        }

        public static void SavePermissions(string filePath, List<Permission> permissions)
        {
            string json = JsonSerializer.Serialize(permissions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static void SaveRoles(string filePath, List<Role> roles)
        {
            string json = JsonSerializer.Serialize(roles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static void SaveUserRoles(string filePath, List<UserRole> userRoles)
        {
            string json = JsonSerializer.Serialize(userRoles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static void SaveControlMappings(string filePath, List<ControlPermissionMapping> mappings)
        {
            string json = JsonSerializer.Serialize(mappings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }

    public class Permission
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public HashSet<string> DependsOn { get; set; } = new HashSet<string>();
    }

    public class Role
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public HashSet<string> Permissions { get; set; } = new HashSet<string>();
    }

    public class UserRole
    {
        public string UserId { get; set; } = string.Empty;
        public HashSet<string> Roles { get; set; } = new HashSet<string>();
        public HashSet<string> AdditionalPermissions { get; set; } = new HashSet<string>();
    }

    // 追加: コントロールと権限のマッピングを定義するクラス
    public class ControlPermissionMapping
    {
        public string FormName { get; set; } = string.Empty;
        public string ControlName { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public RestrictType RestrictType { get; set; } = RestrictType.Enabled;
    }

    // 追加: 制限タイプの列挙型
    public enum RestrictType
    {
        Visibility, // 表示/非表示
        Enabled,    // 有効/無効
        ReadOnly    // 読み取り専用/編集可能
    }
}
