using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions2
{
    public class PermissionService2
    {
        private readonly PermissionRoleManager _permissionRoleManager;
        private readonly UserRoleManager _userRoleManager;
        private readonly ControlMappingManager _controlMappingManager;
        private readonly DepartmentManager _departmentManager;
        public PermissionRoleManager PermissionRoleManager => _permissionRoleManager;
        public UserRoleManager UserRoleManager => _userRoleManager;
        public ControlMappingManager ControlMappingManager => _controlMappingManager;
        public DepartmentManager DepartmentManager => _departmentManager;
        public PermissionService2()
        {
            _permissionRoleManager = new PermissionRoleManager();
            _userRoleManager = new UserRoleManager(_permissionRoleManager);
            _controlMappingManager = new ControlMappingManager(_userRoleManager, _permissionRoleManager);
            _departmentManager = new DepartmentManager(_permissionRoleManager);
        }
        #region PermissionRoleManagerの委譲メソッド

        /// <summary>
        /// すべての権限を取得します
        /// </summary>
        public List<Permission> GetAllPermissions() => _permissionRoleManager.GetAllPermissions();

        /// <summary>
        /// 新しい権限を追加します
        /// </summary>
        public void AddNewPermission(string id, string name) => _permissionRoleManager.AddNewPermission(id, name);

        /// <summary>
        /// 権限を削除します
        /// </summary>
        public void RemovePermission(string id) => _permissionRoleManager.RemovePermission(id);

        /// <summary>
        /// IDを指定して権限を取得します
        /// </summary>
        public Permission GetPermission(string id) => _permissionRoleManager.GetPermission(id);

        /// <summary>
        /// すべてのロールを取得します
        /// </summary>
        public List<Role> GetAllRoles() => _permissionRoleManager.GetAllRoles();

        /// <summary>
        /// 新しいロールを追加します
        /// </summary>
        public void AddNewRole(string id, string name) => _permissionRoleManager.AddNewRole(id, name);

        /// <summary>
        /// ロールを削除します
        /// </summary>
        public void RemoveRole(string id) => _permissionRoleManager.RemoveRole(id);

        /// <summary>
        /// IDを指定してロールを取得します
        /// </summary>
        public Role GetRole(string id) => _permissionRoleManager.GetRole(id);

        /// <summary>
        /// ロールに権限を追加します
        /// </summary>
        public void AddPermissionToRole(string roleId, string permissionId)
            => _permissionRoleManager.AddPermissionToRole(roleId, permissionId);

        /// <summary>
        /// ロールから権限を削除します
        /// </summary>
        public void RemovePermissionFromRole(string roleId, string permissionId)
            => _permissionRoleManager.RemovePermissionFromRole(roleId, permissionId);
        #endregion

        #region UserRoleManagerの委譲メソッド

        /// <summary>
        /// すべてのユーザーロールを取得します
        /// </summary>
        public List<UserRole> GetAllUserRoles() => _userRoleManager.GetAllUserRoles();

        /// <summary>
        /// ユーザーにロールを割り当てます
        /// </summary>
        public void AssignRoleToUser(string userId, string roleId)
            => _userRoleManager.AssignRoleToUser(userId, roleId);

        /// <summary>
        /// ユーザーからロールを削除します
        /// </summary>
        public void RemoveRoleFromUser(string userId, string roleId)
            => _userRoleManager.RemoveRoleFromUser(userId, roleId);

        /// <summary>
        /// ユーザーに追加の権限を付与します
        /// </summary>
        public void GrantAdditionalPermissionToUser(string userId, string permissionId)
            => _userRoleManager.GrantAdditionalPermissionToUser(userId, permissionId);

        /// <summary>
        /// ユーザーから追加の権限を削除します
        /// </summary>
        public void RevokeAdditionalPermissionFromUser(string userId, string permissionId)
            => _userRoleManager.RevokeAdditionalPermissionFromUser(userId, permissionId);

        /// <summary>
        /// ユーザーが特定の権限を持っているかチェックします
        /// </summary>
        public bool HasPermission(string userId, string permissionId)
            => _userRoleManager.HasPermission(userId, permissionId);

        /// <summary>
        /// ユーザーが持つすべての権限を取得します
        /// </summary>
        public HashSet<string> GetAllUserPermissions(string userId)
            => _userRoleManager.GetAllUserPermissions(userId);
        #endregion

        #region ControlMappingManagerの委譲メソッド

        /// <summary>
        /// コントロールの制限状態を確認します
        /// </summary>
        public bool IsControlRestricted(string formName, string controlName, RestrictType restrictType, string userId, string permissionId = null)
            => _controlMappingManager.IsControlRestricted(formName, controlName, restrictType, userId, permissionId);

        /// <summary>
        /// ロールベースのコントロールマッピングを追加します
        /// </summary>
        public void AddRoleControlMapping(string formName, string controlName, string roleId, RestrictType restrictType)
            => _controlMappingManager.AddRoleControlMapping(formName, controlName, roleId, restrictType);

        /// <summary>
        /// ロールベースのコントロールマッピングを削除します
        /// </summary>
        public void RemoveRoleControlMapping(string formName, string controlName, RestrictType restrictType)
            => _controlMappingManager.RemoveRoleControlMapping(formName, controlName, restrictType);

        /// <summary>
        /// ユーザーに対する特定フォームの全コントロールの制約状態を取得します
        /// </summary>
        public Dictionary<string, Dictionary<RestrictType, bool>> GetUserFormControlRestrictions(string formName, string userId)
            => _controlMappingManager.GetUserFormControlRestrictions(formName, userId);
        #endregion

        #region DepartmentManagerの委譲メソッド

        /// <summary>
        /// 新しい部署を追加します
        /// </summary>
        public void AddDepartment(string id, string name, string parentId = null)
            => _departmentManager.AddDepartment(id, name, parentId);

        /// <summary>
        /// 部署を削除します
        /// </summary>
        public void RemoveDepartment(string id) => _departmentManager.RemoveDepartment(id);

        /// <summary>
        /// 部署情報を更新します
        /// </summary>
        public void UpdateDepartment(string id, string name = null, string parentId = null)
            => _departmentManager.UpdateDepartment(id, name, parentId);

        /// <summary>
        /// すべての部署を取得します
        /// </summary>
        public List<Department> GetAllDepartments() => _departmentManager.GetAllDepartments();

        /// <summary>
        /// ユーザーを部署に割り当てます
        /// </summary>
        public void AssignUserToDepartment(string userId, string departmentId)
            => _departmentManager.AssignUserToDepartment(userId, departmentId);

        /// <summary>
        /// ユーザーを部署から削除します
        /// </summary>
        public void RemoveUserFromDepartment(string userId, string departmentId)
            => _departmentManager.RemoveUserFromDepartment(userId, departmentId);

        /// <summary>
        /// ユーザーの主部署を設定します
        /// </summary>
        public void SetUserPrimaryDepartment(string userId, string departmentId)
            => _departmentManager.SetUserPrimaryDepartment(userId, departmentId);

        /// <summary>
        /// 部署にロールを割り当てます
        /// </summary>
        public void AssignRoleToDepartment(string departmentId, string roleId)
            => _departmentManager.AssignRoleToDepartment(departmentId, roleId);

        /// <summary>
        /// ユーザーの部署に基づくロール一覧を取得します
        /// </summary>
        public HashSet<string> GetUserDepartmentRoles(string userId)
            => _departmentManager.GetUserDepartmentRoles(userId);
        #endregion

        #region 統合機能

        /// <summary>
        /// ユーザーがもつすべてのロールを取得します（直接割り当てられたものと部署経由で継承されたもの）
        /// </summary>
        public HashSet<string> GetAllUserRolesIncludingDepartments(string userId)
        {
            // ユーザーに直接割り当てられたロール
            var roles = new HashSet<string>(_userRoleManager.GetUserRoles(userId));

            // 部署経由で割り当てられたロール
            var departmentRoles = _departmentManager.GetUserDepartmentRoles(userId);
            foreach (var role in departmentRoles)
            {
                roles.Add(role);
            }

            return roles;
        }

        /// <summary>
        /// ユーザーがもつすべての権限を取得します（直接割り当てられたものと部署経由で継承されたロール由来のもの）
        /// </summary>
        public HashSet<string> GetAllUserPermissionsIncludingDepartments(string userId)
        {
            // ユーザーに直接割り当てられた権限
            var permissions = new HashSet<string>(_userRoleManager.GetAllUserPermissions(userId));

            // 部署経由で割り当てられたロールの権限
            var departmentRoles = _departmentManager.GetUserDepartmentRoles(userId);
            foreach (var roleId in departmentRoles)
            {
                var rolePermissions = _permissionRoleManager.GetRolePermissions(roleId);
                foreach (var permissionId in rolePermissions)
                {
                    permissions.Add(permissionId);

                    // 依存権限も追加
                    var permission = _permissionRoleManager.GetPermission(permissionId);
                    if (permission != null)
                    {
                        foreach (var dependsOnId in permission.DependsOn)
                        {
                            permissions.Add(dependsOnId);
                        }
                    }
                }
            }

            return permissions;
        }

        /// <summary>
        /// 部署の階層を考慮したユーザーの権限チェック
        /// </summary>
        public bool HasPermissionIncludingDepartments(string userId, string permissionId)
        {
            // ユーザー自身の権限をチェック
            if (_userRoleManager.HasPermission(userId, permissionId))
                return true;

            // 部署経由のロールから権限をチェック
            var departmentRoles = _departmentManager.GetUserDepartmentRoles(userId);
            foreach (var roleId in departmentRoles)
            {
                var role = _permissionRoleManager.GetRole(roleId);
                if (role != null)
                {
                    // ロールが直接権限を持っているか確認
                    if (role.Permissions.Contains(permissionId))
                        return true;

                    // 権限の依存関係も確認
                    foreach (var permission in role.Permissions)
                    {
                        var perm = _permissionRoleManager.GetPermission(permission);
                        if (perm != null && perm.DependsOn.Contains(permissionId))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 部署の階層構造を考慮したコントロールの制限状態チェック
        /// </summary>
        public bool IsControlRestrictedConsideringDepartments(string formName, string controlName, RestrictType restrictType, string userId)
        {
            // 通常のロールベースの制限チェック
            bool baseRestriction = _controlMappingManager.IsControlRestrictedByRole(formName, controlName, restrictType, userId);

            // 制限がない場合はさらにチェックする必要なし
            if (!baseRestriction)
                return false;

            // 部署経由のロールをチェック
            var departmentRoles = _departmentManager.GetUserDepartmentRoles(userId);

            // フォームのマッピングを取得
            var mappings = _controlMappingManager.GetRoleControlMappings(formName);

            // 指定されたコントロールと制限タイプに一致するマッピングを検索
            var relevantMappings = mappings.Where(m =>
                m.ControlName == controlName && m.RestrictType == restrictType).ToList();

            if (!relevantMappings.Any())
                return false; // マッピングがなければ制限なし

            // 部署経由のロールがマッピングに含まれるか確認
            foreach (var mapping in relevantMappings)
            {
                if (departmentRoles.Contains(mapping.RoleId))
                {
                    return false; // 少なくとも1つのロールと一致すれば制限なし
                }
            }

            // どのロールにも一致しなかった場合は制限あり
            return true;
        }
        #endregion

        #region データの保存/読み込み
        /// <summary>
        /// すべての権限関連データを保存します
        /// </summary>
        public void Save()
        {
            try
            {
                _permissionRoleManager.Save();
                _userRoleManager.Save();
                _controlMappingManager.Save();
                _departmentManager.Save();
            }
            catch (Exception ex)
            {
                // エラーログを残すか、例外を再スローするかは用途によって決める
                Console.WriteLine($"Permission data save failed: {ex.Message}");
                throw; // アプリケーション層で処理するため再スロー
            }
        }

        /// <summary>
        /// すべての権限関連データを読み込みます
        /// </summary>
        public void Load()
        {
            try
            {
                // マネージャーの依存関係を考慮して、正しい順序で読み込む
                _permissionRoleManager.Load();
                _userRoleManager.Load();
                _controlMappingManager.Load();
                _departmentManager.Load();
            }
            catch (Exception ex)
            {
                // エラーログを残すか、例外を再スローするかは用途によって決める
                Console.WriteLine($"Permission data load failed: {ex.Message}");
                throw; // アプリケーション層で処理するため再スロー
            }
        }
        #endregion
    }

    public class PermissionRoleManager
    {
        public static string FilePath = "PermissionRoleManager.json";
        private readonly Dictionary<string, Permission> _permissions;
        private readonly Dictionary<string, Role> _roles;

        public PermissionRoleManager()
        {
            _permissions = new Dictionary<string, Permission>();
            _roles = new Dictionary<string, Role>();
        }

        #region 権限管理
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

            // ユーザーの追加権限からも削除は、UserRoleManagerで行う
            // この情報はUserRoleManagerに通知する必要がある
            PermissionRemoved?.Invoke(this, id);
        }

        public event EventHandler<string> PermissionRemoved;

        public Permission GetPermission(string id)
        {
            return _permissions.TryGetValue(id, out var permission) ? permission : null;
        }

        public void AddDependencyToPermission(string permissionId, string dependsOnId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                permission.DependsOn.Add(dependsOnId);
            }
        }

        public void RemoveDependencyFromPermission(string permissionId, string dependsOnId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                permission.DependsOn.Remove(dependsOnId);
            }
        }

        public HashSet<string> GetPermissionDependencies(string permissionId)
        {
            if (_permissions.TryGetValue(permissionId, out var permission))
            {
                return new HashSet<string>(permission.DependsOn);
            }
            return new HashSet<string>();
        }
        #endregion

        #region ロール管理
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

            // ユーザーロールからこのロールを削除は、UserRoleManagerで行う
            // この情報はUserRoleManagerに通知する必要がある
            RoleRemoved?.Invoke(this, roleId);
        }

        public event EventHandler<string> RoleRemoved;

        public Role GetRole(string roleId)
        {
            return _roles.TryGetValue(roleId, out var role) ? role : null;
        }

        public void AddPermissionToRole(string roleId, string permissionId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                role.Permissions.Add(permissionId);
            }
        }

        public void RemovePermissionFromRole(string roleId, string permissionId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                role.Permissions.Remove(permissionId);
            }
        }

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

        public HashSet<string> GetRolePermissions(string roleId)
        {
            if (_roles.TryGetValue(roleId, out var role))
            {
                return new HashSet<string>(role.Permissions);
            }
            return new HashSet<string>();
        }
        #endregion

        public void Save()
        {

        }

        public void Load()
        {

        }
    }

    public class UserRoleManager
    {
        public static string FilePath = "UserRoleManager.json";
        private readonly Dictionary<string, UserRole> _userRoles;
        private readonly PermissionRoleManager _permissionRoleManager;

        public UserRoleManager(PermissionRoleManager permissionRoleManager = null)
        {
            _userRoles = new Dictionary<string, UserRole>();
            _permissionRoleManager = permissionRoleManager;

            // PermissionRoleManagerとの連携が可能な場合、イベント登録する
            if (_permissionRoleManager != null)
            {
                _permissionRoleManager.RoleRemoved += OnRoleRemoved;
                _permissionRoleManager.PermissionRemoved += OnPermissionRemoved;
            }
        }

        #region イベントハンドラ
        private void OnRoleRemoved(object sender, string roleId)
        {
            // ロールが削除された場合、すべてのユーザーからそのロールを削除
            foreach (var userRole in _userRoles.Values)
            {
                userRole.Roles.Remove(roleId);
            }
        }

        private void OnPermissionRemoved(object sender, string permissionId)
        {
            // 権限が削除された場合、すべてのユーザーからその追加権限を削除
            foreach (var userRole in _userRoles.Values)
            {
                userRole.AdditionalPermissions.Remove(permissionId);
            }
        }
        #endregion

        #region ユーザーロール管理
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

        public void AssignRolesToUser(string userId, IEnumerable<string> roleIds)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            foreach (var roleId in roleIds)
            {
                if (!userRole.Roles.Contains(roleId))
                {
                    userRole.Roles.Add(roleId);
                }
            }
        }

        public void GrantAdditionalPermissionsToUser(string userId, IEnumerable<string> permissionIds)
        {
            if (!_userRoles.TryGetValue(userId, out var userRole))
            {
                userRole = new UserRole { UserId = userId };
                _userRoles[userId] = userRole;
            }

            foreach (var permissionId in permissionIds)
            {
                if (!userRole.AdditionalPermissions.Contains(permissionId))
                {
                    userRole.AdditionalPermissions.Add(permissionId);
                }
            }
        }

        public HashSet<string> GetUserRoles(string userId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                return new HashSet<string>(userRole.Roles);
            }
            return new HashSet<string>();
        }

        public HashSet<string> GetUserAdditionalPermissions(string userId)
        {
            if (_userRoles.TryGetValue(userId, out var userRole))
            {
                return new HashSet<string>(userRole.AdditionalPermissions);
            }
            return new HashSet<string>();
        }
        #endregion

        #region 権限チェック
        public HashSet<string> GetAllUserPermissions(string userId)
        {
            var allPermissions = new HashSet<string>();

            if (!_userRoles.TryGetValue(userId, out var userRole))
                return allPermissions;

            // PermissionRoleManagerが設定されていない場合は追加権限のみ返す
            if (_permissionRoleManager == null)
            {
                return new HashSet<string>(userRole.AdditionalPermissions);
            }

            // 追加権限を追加
            foreach (var permissionId in userRole.AdditionalPermissions)
            {
                allPermissions.Add(permissionId);

                // 依存権限も追加
                var permission = _permissionRoleManager.GetPermission(permissionId);
                if (permission != null)
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
                var rolePermissions = _permissionRoleManager.GetRolePermissions(roleId);
                foreach (var permissionId in rolePermissions)
                {
                    allPermissions.Add(permissionId);

                    // 依存権限も追加
                    var permission = _permissionRoleManager.GetPermission(permissionId);
                    if (permission != null)
                    {
                        foreach (var dependsOnId in permission.DependsOn)
                        {
                            allPermissions.Add(dependsOnId);
                        }
                    }
                }
            }

            return allPermissions;
        }

        public bool HasPermission(string userId, string permissionId)
        {
            // ユーザーロールが存在しない場合は権限なし
            if (!_userRoles.TryGetValue(userId, out var userRole))
                return false;

            // 追加権限に直接含まれているか確認
            if (userRole.AdditionalPermissions.Contains(permissionId))
                return true;

            // PermissionRoleManagerが設定されていない場合は権限なし
            if (_permissionRoleManager == null)
                return false;

            // ユーザーのロールに関連付けられた権限を確認
            foreach (var roleId in userRole.Roles)
            {
                var role = _permissionRoleManager.GetRole(roleId);
                if (role != null)
                {
                    // ロールが直接権限を持っているか確認
                    if (role.Permissions.Contains(permissionId))
                        return true;

                    // 権限の依存関係も確認
                    foreach (var permission in role.Permissions)
                    {
                        var perm = _permissionRoleManager.GetPermission(permission);
                        if (perm != null && perm.DependsOn.Contains(permissionId))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
        public void Save()
        {

        }

        public void Load()
        {

        }
    }

    public class ControlMappingManager
    {
        public static string FilePath = "ControlMappingManager.json";
        private readonly Dictionary<string, List<RoleControlMapping>> _roleControlMappings;
        private readonly UserRoleManager _userRoleManager;
        private readonly PermissionRoleManager _permissionRoleManager;

        public ControlMappingManager(UserRoleManager userRoleManager = null, PermissionRoleManager permissionRoleManager = null)
        {
            _roleControlMappings = new Dictionary<string, List<RoleControlMapping>>();
            _userRoleManager = userRoleManager;
            _permissionRoleManager = permissionRoleManager;

            // PermissionRoleManagerとの連携が可能な場合、イベントを設定
            if (_permissionRoleManager != null)
            {
                _permissionRoleManager.RoleRemoved += OnRoleRemoved;
            }
        }

        #region イベントハンドラ
        private void OnRoleRemoved(object sender, string roleId)
        {
            // ロールが削除された場合、関連するマッピングを削除
            foreach (var formMappings in _roleControlMappings.Values)
            {
                formMappings.RemoveAll(m => m.RoleId == roleId);
            }

            // 空になったフォームエントリを削除
            var emptyForms = _roleControlMappings
                .Where(kvp => kvp.Value.Count == 0)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var formName in emptyForms)
            {
                _roleControlMappings.Remove(formName);
            }
        }
        #endregion

        #region コントロールマッピング管理
        // ロールベースのコントロールマッピングを取得
        public List<RoleControlMapping> GetRoleControlMappings(string formName)
        {
            if (_roleControlMappings.TryGetValue(formName, out var mappings))
            {
                return mappings;
            }
            return new List<RoleControlMapping>();
        }

        // すべてのマッピングを取得
        public List<RoleControlMapping> GetAllMappings()
        {
            return _roleControlMappings.Values.SelectMany(list => list).ToList();
        }

        // フォーム単位でマッピングを取得
        public Dictionary<string, List<RoleControlMapping>> GetMappingsByForm()
        {
            return new Dictionary<string, List<RoleControlMapping>>(_roleControlMappings);
        }

        // ロールベースのコントロールマッピングを追加
        public void AddRoleControlMapping(string formName, string controlName, string roleId, RestrictType restrictType)
        {
            if (!_roleControlMappings.TryGetValue(formName, out var formMappings))
            {
                formMappings = new List<RoleControlMapping>();
                _roleControlMappings[formName] = formMappings;
            }

            // 既存のマッピングを確認し、あれば更新
            var existingMapping = formMappings.FirstOrDefault(m =>
                m.ControlName == controlName && m.RestrictType == restrictType);

            if (existingMapping != null)
            {
                existingMapping.RoleId = roleId;
            }
            else
            {
                // 新しいマッピングを追加
                formMappings.Add(new RoleControlMapping
                {
                    FormName = formName,
                    ControlName = controlName,
                    RoleId = roleId,
                    RestrictType = restrictType
                });
            }
        }

        // ロールベースのコントロールマッピングを削除
        public void RemoveRoleControlMapping(string formName, string controlName, RestrictType restrictType)
        {
            if (_roleControlMappings.TryGetValue(formName, out var formMappings))
            {
                formMappings.RemoveAll(m => m.ControlName == controlName && m.RestrictType == restrictType);

                // マッピングが空になった場合はフォームエントリ自体を削除
                if (formMappings.Count == 0)
                {
                    _roleControlMappings.Remove(formName);
                }
            }
        }

        // フォーム単位でマッピングを削除
        public void RemoveFormMappings(string formName)
        {
            _roleControlMappings.Remove(formName);
        }
        #endregion

        #region アクセス制御
        // ロールベースのアクセス制御をチェック
        public bool IsControlRestrictedByRole(string formName, string controlName, RestrictType restrictType, string userId)
        {
            // UserRoleManagerが設定されていない場合は制限なし
            if (_userRoleManager == null)
                return false;

            // 指定されたフォームのマッピングを取得
            if (!_roleControlMappings.TryGetValue(formName, out var formMappings))
            {
                return false; // マッピングがなければ制限なし
            }

            // 指定されたコントロールと制限タイプに一致するマッピングを検索
            var mappings = formMappings.Where(m =>
                m.ControlName == controlName && m.RestrictType == restrictType).ToList();

            if (!mappings.Any())
            {
                return false; // マッピングがなければ制限なし
            }

            // ユーザーロールを取得
            var userRoles = _userRoleManager.GetUserRoles(userId);

            // ユーザーが必要なロールを持っているか確認
            // どれか1つのロールが一致すれば制限なし（false）
            foreach (var mapping in mappings)
            {
                if (userRoles.Contains(mapping.RoleId))
                {
                    return false; // 少なくとも1つのロールと一致すれば制限なし
                }
            }

            // どのロールにも一致しなかった場合は制限あり
            return true;
        }

        // 権限ベースのアクセス制御をチェック
        public bool IsControlRestrictedByPermission(string formName, string controlName, RestrictType restrictType, string userId, string permissionId)
        {
            // UserRoleManagerが設定されていない場合は制限なし
            if (_userRoleManager == null)
                return false;

            // ユーザーが必要な権限を持っているか確認
            // 持っていなければ制限あり（true）、持っていれば制限なし（false）
            return !_userRoleManager.HasPermission(userId, permissionId);
        }

        // ロールベースとパーミッションベースの両方を考慮した統合チェック
        public bool IsControlRestricted(string formName, string controlName, RestrictType restrictType, string userId, string permissionId = null)
        {
            // ロールベースのチェック
            bool restrictedByRole = IsControlRestrictedByRole(formName, controlName, restrictType, userId);

            // 権限IDが指定されている場合は権限ベースのチェックも行う
            bool restrictedByPermission = permissionId != null
                ? IsControlRestrictedByPermission(formName, controlName, restrictType, userId, permissionId)
                : false;

            // 両方の制限をチェック - どちらか一方でも制限あり（true）の場合はアクセス制限
            return restrictedByRole || restrictedByPermission;
        }
        #endregion

        #region ファイル操作
        public void Save()
        {
            var allMappings = _roleControlMappings.Values.SelectMany(list => list).ToList();
            string json = JsonSerializer.Serialize(allMappings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
                return;

            try
            {
                string json = File.ReadAllText(FilePath);
                var mappings = JsonSerializer.Deserialize<List<RoleControlMapping>>(json);

                if (mappings != null)
                {
                    _roleControlMappings.Clear();

                    // フォーム名でグループ化してマッピングを追加
                    foreach (var grouping in mappings.GroupBy(m => m.FormName))
                    {
                        _roleControlMappings[grouping.Key] = grouping.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // ロード失敗時のエラー処理
                Console.WriteLine($"ControlMappingManager.Load failed: {ex.Message}");
            }
        }
        #endregion

        #region 拡張機能
        // 特定のフォームに適用されるすべてのコントロール制約を取得
        public Dictionary<string, Dictionary<RestrictType, List<string>>> GetFormControlRestrictions(string formName)
        {
            var result = new Dictionary<string, Dictionary<RestrictType, List<string>>>();

            if (!_roleControlMappings.TryGetValue(formName, out var formMappings))
            {
                return result;
            }

            foreach (var mapping in formMappings)
            {
                if (!result.TryGetValue(mapping.ControlName, out var restrictionMap))
                {
                    restrictionMap = new Dictionary<RestrictType, List<string>>();
                    result[mapping.ControlName] = restrictionMap;
                }

                if (!restrictionMap.TryGetValue(mapping.RestrictType, out var roleList))
                {
                    roleList = new List<string>();
                    restrictionMap[mapping.RestrictType] = roleList;
                }

                roleList.Add(mapping.RoleId);
            }

            return result;
        }

        // ユーザーに対する特定フォームの全コントロールの制約状態を取得
        public Dictionary<string, Dictionary<RestrictType, bool>> GetUserFormControlRestrictions(string formName, string userId)
        {
            var result = new Dictionary<string, Dictionary<RestrictType, bool>>();

            if (!_roleControlMappings.TryGetValue(formName, out var formMappings))
            {
                return result;
            }

            foreach (var mapping in formMappings)
            {
                if (!result.TryGetValue(mapping.ControlName, out var restrictionMap))
                {
                    restrictionMap = new Dictionary<RestrictType, bool>();
                    result[mapping.ControlName] = restrictionMap;
                }

                if (!restrictionMap.ContainsKey(mapping.RestrictType))
                {
                    bool isRestricted = IsControlRestrictedByRole(formName, mapping.ControlName, mapping.RestrictType, userId);
                    restrictionMap[mapping.RestrictType] = isRestricted;
                }
            }

            return result;
        }
        #endregion
    }

    public class DepartmentManager
    {
        public static string FilePath = "DepartmentManager.json";
        private readonly Dictionary<string, Department> _departments;
        private readonly Dictionary<string, UserDepartment> _userDepartments;
        private readonly PermissionRoleManager _permissionRoleManager;

        public DepartmentManager(PermissionRoleManager permissionRoleManager = null)
        {
            _departments = new Dictionary<string, Department>();
            _userDepartments = new Dictionary<string, UserDepartment>();
            _permissionRoleManager = permissionRoleManager;

            // PermissionRoleManagerとの連携が可能な場合、イベント登録する
            if (_permissionRoleManager != null)
            {
                _permissionRoleManager.RoleRemoved += OnRoleRemoved;
            }
        }

        #region イベントハンドラ
        private void OnRoleRemoved(object sender, string roleId)
        {
            // ロールが削除された場合、関連する部署のロールマッピングを削除
            foreach (var department in _departments.Values)
            {
                department.AssignedRoles.Remove(roleId);
            }
        }
        #endregion

        #region 部署管理
        // 部署の追加
        public void AddDepartment(string id, string name, string parentId = null)
        {
            var department = new Department
            {
                Id = id,
                Name = name,
                ParentId = parentId
            };
            _departments[id] = department;
        }

        // 部署の削除
        public void RemoveDepartment(string id)
        {
            if (_departments.ContainsKey(id))
            {
                // 部署を削除
                _departments.Remove(id);

                // 削除した部署をユーザーから削除
                foreach (var userDept in _userDepartments.Values)
                {
                    userDept.DepartmentIds.Remove(id);
                }

                // 削除した部署を親部署とする部署の親部署IDをクリア
                foreach (var dept in _departments.Values)
                {
                    if (dept.ParentId == id)
                    {
                        dept.ParentId = null;
                    }
                }

                // 部署削除イベントを発行
                DepartmentRemoved?.Invoke(this, id);
            }
        }

        public event EventHandler<string> DepartmentRemoved;

        // 部署の取得
        public Department GetDepartment(string id)
        {
            return _departments.TryGetValue(id, out var department) ? department : null;
        }

        // すべての部署を取得
        public List<Department> GetAllDepartments() => _departments.Values.ToList();

        // 子部署を取得
        public List<Department> GetChildDepartments(string parentId)
        {
            return _departments.Values.Where(d => d.ParentId == parentId).ToList();
        }

        // 部署階層を再帰的に取得
        public List<Department> GetDepartmentHierarchy(string rootDepartmentId)
        {
            var result = new List<Department>();
            if (!_departments.TryGetValue(rootDepartmentId, out var rootDept))
                return result;

            result.Add(rootDept);
            AddChildDepartmentsRecursive(rootDepartmentId, result);
            return result;
        }

        private void AddChildDepartmentsRecursive(string parentId, List<Department> result)
        {
            var children = GetChildDepartments(parentId);
            foreach (var child in children)
            {
                result.Add(child);
                AddChildDepartmentsRecursive(child.Id, result);
            }
        }

        // 部署情報を更新
        public void UpdateDepartment(string id, string name = null, string parentId = null)
        {
            if (_departments.TryGetValue(id, out var department))
            {
                if (name != null)
                    department.Name = name;

                if (parentId != null)
                {
                    // 循環参照をチェック
                    if (WouldCreateCycle(id, parentId))
                    {
                        throw new InvalidOperationException("部署階層に循環参照が発生します。");
                    }
                    department.ParentId = parentId;
                }
            }
        }

        // 循環参照のチェック
        private bool WouldCreateCycle(string departmentId, string newParentId)
        {
            if (departmentId == newParentId)
                return true;

            var currentParentId = newParentId;
            while (!string.IsNullOrEmpty(currentParentId))
            {
                if (currentParentId == departmentId)
                    return true;

                if (!_departments.TryGetValue(currentParentId, out var parentDept))
                    break;

                currentParentId = parentDept.ParentId;
            }

            return false;
        }

        // 部署にロールを割り当て
        public void AssignRoleToDepartment(string departmentId, string roleId)
        {
            if (_departments.TryGetValue(departmentId, out var department))
            {
                department.AssignedRoles.Add(roleId);
            }
        }

        // 部署からロールを削除
        public void RemoveRoleFromDepartment(string departmentId, string roleId)
        {
            if (_departments.TryGetValue(departmentId, out var department))
            {
                department.AssignedRoles.Remove(roleId);
            }
        }

        // 部署に割り当てられたロール取得
        public HashSet<string> GetDepartmentRoles(string departmentId)
        {
            if (_departments.TryGetValue(departmentId, out var department))
            {
                return new HashSet<string>(department.AssignedRoles);
            }
            return new HashSet<string>();
        }

        // 部署階層の全てのロールを取得（親部署のロールを含む）
        public HashSet<string> GetDepartmentAndParentRoles(string departmentId)
        {
            var roles = new HashSet<string>();
            var currentDeptId = departmentId;

            while (!string.IsNullOrEmpty(currentDeptId))
            {
                if (_departments.TryGetValue(currentDeptId, out var department))
                {
                    foreach (var roleId in department.AssignedRoles)
                    {
                        roles.Add(roleId);
                    }
                    currentDeptId = department.ParentId;
                }
                else
                {
                    break;
                }
            }

            return roles;
        }
        #endregion

        #region ユーザー部署関連
        // ユーザーを部署に割り当て
        public void AssignUserToDepartment(string userId, string departmentId)
        {
            if (!_departments.ContainsKey(departmentId))
                return;

            if (!_userDepartments.TryGetValue(userId, out var userDepartment))
            {
                userDepartment = new UserDepartment { UserId = userId };
                _userDepartments[userId] = userDepartment;
            }

            userDepartment.DepartmentIds.Add(departmentId);
        }

        // ユーザーを部署から削除
        public void RemoveUserFromDepartment(string userId, string departmentId)
        {
            if (_userDepartments.TryGetValue(userId, out var userDepartment))
            {
                userDepartment.DepartmentIds.Remove(departmentId);

                // 部署がなくなった場合はユーザーの部署情報自体を削除
                if (userDepartment.DepartmentIds.Count == 0)
                {
                    _userDepartments.Remove(userId);
                }
            }
        }

        // ユーザーに主部署を設定
        public void SetUserPrimaryDepartment(string userId, string departmentId)
        {
            if (!_departments.ContainsKey(departmentId))
                return;

            if (!_userDepartments.TryGetValue(userId, out var userDepartment))
            {
                userDepartment = new UserDepartment { UserId = userId };
                _userDepartments[userId] = userDepartment;
            }

            // 主部署を設定する前に所属部署に追加する
            userDepartment.DepartmentIds.Add(departmentId);
            userDepartment.PrimaryDepartmentId = departmentId;
        }

        // ユーザーの部署情報を取得
        public UserDepartment GetUserDepartment(string userId)
        {
            return _userDepartments.TryGetValue(userId, out var userDepartment) ? userDepartment : null;
        }

        // ユーザーの所属部署一覧を取得
        public List<Department> GetUserDepartments(string userId)
        {
            var result = new List<Department>();
            if (_userDepartments.TryGetValue(userId, out var userDepartment))
            {
                foreach (var deptId in userDepartment.DepartmentIds)
                {
                    if (_departments.TryGetValue(deptId, out var department))
                    {
                        result.Add(department);
                    }
                }
            }
            return result;
        }

        // ユーザーの主部署を取得
        public Department GetUserPrimaryDepartment(string userId)
        {
            if (_userDepartments.TryGetValue(userId, out var userDepartment) &&
                !string.IsNullOrEmpty(userDepartment.PrimaryDepartmentId) &&
                _departments.TryGetValue(userDepartment.PrimaryDepartmentId, out var department))
            {
                return department;
            }
            return null;
        }

        // 部署に所属するユーザー一覧を取得
        public List<string> GetDepartmentUsers(string departmentId)
        {
            return _userDepartments.Values
                .Where(ud => ud.DepartmentIds.Contains(departmentId))
                .Select(ud => ud.UserId)
                .ToList();
        }

        // 部署階層に所属するユーザー一覧を取得（子部署のユーザーを含む）
        public List<string> GetDepartmentHierarchyUsers(string departmentId)
        {
            var deptHierarchy = GetDepartmentHierarchy(departmentId);
            var deptIds = deptHierarchy.Select(d => d.Id).ToHashSet();

            return _userDepartments.Values
                .Where(ud => ud.DepartmentIds.Any(deptIds.Contains))
                .Select(ud => ud.UserId)
                .ToList();
        }
        #endregion

        #region 権限チェック
        // ユーザーの部署に基づくロール一覧を取得
        public HashSet<string> GetUserDepartmentRoles(string userId)
        {
            var roles = new HashSet<string>();
            if (_userDepartments.TryGetValue(userId, out var userDepartment))
            {
                foreach (var departmentId in userDepartment.DepartmentIds)
                {
                    var deptRoles = GetDepartmentAndParentRoles(departmentId);
                    foreach (var role in deptRoles)
                    {
                        roles.Add(role);
                    }
                }
            }
            return roles;
        }
        #endregion

        public void Save()
        {

        }

        public void Load()
        {

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

    public class RoleControlMapping
    {
        public string FormName { get; set; } = string.Empty;
        public string ControlName { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public RestrictType RestrictType { get; set; } = RestrictType.Enabled;
    }

    // 追加: 制限タイプの列挙型
    public enum RestrictType
    {
        Visibility, // 表示/非表示
        Enabled,    // 有効/無効
        ReadOnly    // 読み取り専用/編集可能
    }

    // 部署情報クラス
    public class Department
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public HashSet<string> AssignedRoles { get; set; } = new HashSet<string>();
    }

    // ユーザーの部署所属情報クラス
    public class UserDepartment
    {
        public string UserId { get; set; } = string.Empty;
        public HashSet<string> DepartmentIds { get; set; } = new HashSet<string>();
        public string PrimaryDepartmentId { get; set; } = string.Empty;
    }
}
