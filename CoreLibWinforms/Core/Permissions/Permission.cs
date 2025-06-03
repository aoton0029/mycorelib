using CoreLibWinforms.Core.Permissions;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Permissions
{
    public class Permission
    {
        // 権限のID（これはBitArrayの位置としても使う）
        public int Id { get; private set; }
        // 権限の名前
        public string Name { get; private set; }

        /// <summary>
        /// 基本権限コンストラクタ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Permission(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BitArray Permissions { get; private set; }
        public bool IsAdministratorRole { get; private set; }

        public Role(string name, int initialCapacity = 32)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            Name = name;
            Permissions = new BitArray(initialCapacity);
        }

        public void GrantPermission(Permission permission)
        {
            EnsureCapacity(permission.Id + 1);
            Permissions[permission.Id] = true;
        }

        public void RevokePermission(Permission permission)
        {
            if (permission.Id < Permissions.Length)
            {
                Permissions[permission.Id] = false;
            }
        }

        public bool HasPermission(Permission permission)
        {
            return permission.Id < Permissions.Length && Permissions[permission.Id];
        }

        private void EnsureCapacity(int requiredLength)
        {
            if (requiredLength > Permissions.Length)
            {
                // 必要なサイズの2倍のサイズに拡張
                int newSize = Math.Max(requiredLength, Permissions.Length * 2);
                var newPermissions = new BitArray(newSize);

                // 既存の権限をコピー
                for (int i = 0; i < Permissions.Length; i++)
                {
                    newPermissions[i] = Permissions[i];
                }

                Permissions = newPermissions;
            }
        }

        public static Role Admin() =>
            new Role("Administrator", 32)
            {
                IsAdministratorRole = true
            };
    }

    // ユーザー権限を管理するクラス
    public class PermissionManager
    {
        public static int MaxPermissionId = 32;
        public static string PermissionsFilePath = "permissions.json";
        public static string UserRoleFilePath = "user_roles.json";

        // 管理者ロールの名前を定義
        public const string AdministratorRoleName = "Administrator";

        private readonly Dictionary<int, Permission> _permissions = new Dictionary<int, Permission>();
        private readonly Dictionary<string, Role> _roles = new Dictionary<string, Role>();
        private readonly Dictionary<string, UserRole> _users = new Dictionary<string, UserRole>();

        #region Permission
        public Permission CreatePermission(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Permission name cannot be empty", nameof(name));
            int newId = _permissions.Count;
            var permission = new Permission(newId, name);
            _permissions.Add(newId, permission);
            return permission;
        }

        public Permission GetPermission(int id)
        {
            if (!_permissions.TryGetValue(id, out var permission))
                throw new KeyNotFoundException($"Permission with ID {id} not found");

            return permission;
        }

        public IEnumerable<Permission> GetAllPermissions()
        {
            return _permissions.Values;
        }
        #endregion

        #region Role
        // 管理者ロールを作成
        public Role CreateAdministratorRole()
        {
            if (_roles.ContainsKey(AdministratorRoleName))
                return _roles[AdministratorRoleName];

            // 管理者ロールを作成
            var role = Role.Admin();

            // すべての権限を付与
            foreach (var permission in _permissions.Values)
            {
                role.GrantPermission(permission);
            }

            _roles.Add(AdministratorRoleName, role);
            return role;
        }

        // Role関連メソッドの修正
        public Role CreateRole(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            if (_roles.ContainsKey(name))
                throw new ArgumentException($"Role with name '{name}' already exists", nameof(name));

            var role = new Role(name);
            _roles.Add(name, role);
            return role;
        }

        public Role GetRole(string name)
        {
            if (!_roles.TryGetValue(name, out var role))
                throw new KeyNotFoundException($"Role with name '{name}' not found");

            return role;
        }

        /// <summary>
        /// 指定した名前のロールがあるか確認します
        /// </summary>
        /// <param name="name">ロール名</param>
        /// <returns>存在する場合はtrue</returns>
        public bool RoleExists(string name)
        {
            return _roles.ContainsKey(name);
        }


        /// <summary>
        /// ロールの名前を変更します
        /// </summary>
        /// <param name="currentName">現在のロール名</param>
        /// <param name="newName">新しいロール名</param>
        /// <exception cref="KeyNotFoundException">指定されたロールが存在しない場合</exception>
        /// <exception cref="ArgumentException">新しい名前が既に存在する場合</exception>
        public void RenameRole(string currentName, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New role name cannot be empty", nameof(newName));

            if (!_roles.TryGetValue(currentName, out var role))
                throw new KeyNotFoundException($"Role with name '{currentName}' not found");

            if (_roles.ContainsKey(newName))
                throw new ArgumentException($"Role with name '{newName}' already exists", nameof(newName));

            // ロールを削除して新しい名前で追加
            _roles.Remove(currentName);
            role.Name = newName; // Nameプロパティはprivate setなので、実際には内部でsetterを公開するか、リフレクションを使う必要があるかも
            _roles.Add(newName, role);

            // ユーザーのロール参照を更新
            foreach (var user in _users.Values)
            {
                if (user.AssignedRoleNames.Contains(currentName))
                {
                    user.UnassignRole(currentName);
                    user.AssignRole(newName);
                }
            }
        }

        /// <summary>
        /// ロールを削除します
        /// </summary>
        /// <param name="name">削除するロール名</param>
        /// <exception cref="KeyNotFoundException">指定されたロールが存在しない場合</exception>
        public void DeleteRole(string name)
        {
            if (!_roles.TryGetValue(name, out var _))
                throw new KeyNotFoundException($"Role with name '{name}' not found");

            // ロールを削除
            _roles.Remove(name);

            // ユーザーからそのロールの参照を削除
            foreach (var user in _users.Values)
            {
                if (user.AssignedRoleNames.Contains(name))
                {
                    user.UnassignRole(name);
                }
            }
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _roles.Values;
        }
        #endregion

        #region Role-Permission
        public void GrantPermissionToRole(string roleName, int permissionId)
        {
            var role = GetRole(roleName);
            var permission = GetPermission(permissionId);
            role.GrantPermission(permission);
        }

        public void RevokePermissionFromRole(string roleName, int permissionId)
        {
            var role = GetRole(roleName);
            var permission = GetPermission(permissionId);
            role.RevokePermission(permission);
        }

        public bool HasPermissionRole(string roleName, int permissionId)
        {
            var role = GetRole(roleName);
            var permission = GetPermission(permissionId);
            return role.HasPermission(permission);
        }
        #endregion

        #region UserRole
        /// <summary>
        /// ユーザーに管理者権限を付与します
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void GrantAdministratorRights(string userId)
        {
            // 管理者ロールが存在しない場合は作成
            if (!RoleExists(AdministratorRoleName))
                CreateAdministratorRole();

            // ユーザーに管理者ロールを割り当て
            if (!UserHasRole(userId, AdministratorRoleName))
                AssignRoleToUser(userId, AdministratorRoleName);
        }

        public UserRole CreateUser(string userId, Role role)
        {
            if (_users.ContainsKey(userId))
                throw new InvalidOperationException($"User with ID {userId} already exists");

            var user = new UserRole(userId);
            _users.Add(userId, user);

            // もしroleが指定されていれば、そのロールをユーザーに割り当て
            if (role != null)
                user.AssignRole(role.Name);

            return user;
        }

        public UserRole GetUser(string userId)
        {
            if (!_users.TryGetValue(userId, out var user))
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return user;
        }

        public void AssignRoleToUser(string userId, string roleName)
        {
            var user = GetUser(userId);
            // ロールの存在確認
            GetRole(roleName);
            user.AssignRole(roleName);
        }

        public void UnassignRoleFromUser(string userId, string roleName)
        {
            var user = GetUser(userId);
            user.UnassignRole(roleName);
        }

        public IEnumerable<UserRole> GetAllUsers()
        {
            return _users.Values;
        }

        public void GrantAdditionalPermissionToUser(string userId, int permissionId)
        {
            var user = GetUser(userId);
            // 権限の存在確認
            GetPermission(permissionId);
            user.GrantAdditionalPermission(permissionId);
        }

        public void RevokeAdditionalPermissionFromUser(string userId, int permissionId)
        {
            var user = GetUser(userId);
            user.RevokeAdditionalPermission(permissionId);
        }

        public void DenyPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId);
            // 権限の存在確認
            GetPermission(permissionId);
            user.DenyPermission(permissionId);
        }

        public void RemoveDeniedPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId);
            user.RemoveDeniedPermission(permissionId);
        }

        /// <summary>
        /// ユーザーが特定のロールを持っているかチェックします
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="roleName">確認するロール名</param>
        /// <returns>ロールを持っていればtrue</returns>
        public bool UserHasRole(string userId, string roleName)
        {
            var user = GetUser(userId);
            return user.AssignedRoleNames.Contains(roleName);
        }

        /// <summary>
        /// ユーザーに割り当てられているすべてのロールを取得します
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ロールのコレクション</returns>
        public IEnumerable<Role> GetUserRoles(string userId)
        {
            var user = GetUser(userId);
            return user.AssignedRoleNames.Select(name => GetRole(name));
        }

        /// <summary>
        /// ユーザーに管理者ロールが割り当てられているかをチェックします
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>管理者ロールを持っていればtrue</returns>
        public bool UserIsAdministrator(string userId)
        {
            if (!UserHasRole(userId, AdministratorRoleName))
            {
                // 管理者ロールを持っていない場合は、他に管理者ロールとしてマークされたロールがあるかチェック
                var userRoles = GetUserRoles(userId);
                return userRoles.Any(r => r.IsAdministratorRole);
            }
            return true;
        }

        // ユーザー権限チェックの修正
        public bool HasPermission(string userId, int permissionId)
        {
            var user = GetUser(userId);

            // 拒否リストに含まれている場合は常にfalse
            if (user.DeniedPermissionIds.Contains(permissionId))
                return false;

            // 追加権限リストに含まれている場合はtrue
            if (user.AdditionalPermissionIds.Contains(permissionId))
                return true;

            // ユーザーに割り当てられたロールをチェック
            foreach (var roleName in user.AssignedRoleNames)
            {
                if (HasPermissionRole(roleName, permissionId))
                    return true;
            }

            return false;
        }

        // ユーザーに割り当てられている全権限を取得
        public IEnumerable<Permission> GetUserPermissions(string userId)
        {
            var user = GetUser(userId);
            var allPermissions = new HashSet<Permission>();

            // ロールから取得できる権限を追加
            foreach (var roleName in user.AssignedRoleNames)
            {
                var role = GetRole(roleName);
                foreach (var permission in GetAllPermissions())
                {
                    if (role.HasPermission(permission) && !user.DeniedPermissionIds.Contains(permission.Id))
                    {
                        allPermissions.Add(permission);
                    }
                }
            }

            // 追加権限を追加
            foreach (var permissionId in user.AdditionalPermissionIds)
            {
                if (!user.DeniedPermissionIds.Contains(permissionId))
                {
                    allPermissions.Add(GetPermission(permissionId));
                }
            }

            return allPermissions;
        }
        #endregion

        //#region Department
        //// PermissionManagerクラス内に追加
        //private readonly DepartmentManager _departmentManager = new DepartmentManager();

        //// 部署マネージャーへのアクセスプロパティ
        //public DepartmentManager Departments => _departmentManager;

        ///// <summary>
        ///// 部署内のすべてのユーザーに対してロールを割り当てる
        ///// </summary>
        ///// <param name="departmentId">部署ID</param>
        ///// <param name="roleName">ロール名</param>
        //public void AssignRoleToDepartment(string departmentId, string roleName)
        //{
        //    var department = _departmentManager.GetDepartment(departmentId);
        //    var role = GetRole(roleName);

        //    foreach (var userId in department.MemberUserIds)
        //    {
        //        // ユーザーが存在する場合のみロールを割り当て
        //        if (_users.ContainsKey(userId))
        //        {
        //            AssignRoleToUser(userId, roleName);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 部署階層を上に遡り、ユーザーが所属するすべての部署の権限をチェック
        ///// </summary>
        ///// <param name="userId">ユーザーID</param>
        ///// <param name="permissionId">権限ID</param>
        ///// <returns>権限があればtrue</returns>
        //public bool HasDepartmentalPermission(string userId, int permissionId)
        //{
        //    var userDepartments = _departmentManager.GetUserDepartments(userId);

        //    foreach (var department in userDepartments)
        //    {
        //        // この部署に割り当てられた特殊な権限を確認する処理を追加
        //        // （現状の実装では部署に直接権限は割り当てられないが、将来的に拡張可能）

        //        // 親部署を遡って確認（組織階層を利用した権限チェック）
        //        string? currentDeptId = department.Id;
        //        while (!string.IsNullOrWhiteSpace(currentDeptId))
        //        {
        //            // 現在のところは何もしない
        //            // （部署に直接権限を割り当てる機能を追加する場合はここにロジックを追加）

        //            // 親部署を取得
        //            if (_departmentManager.DepartmentExists(currentDeptId))
        //            {
        //                currentDeptId = _departmentManager.GetDepartment(currentDeptId).ParentDepartmentId;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    // 通常のユーザー権限チェックを実行
        //    return HasPermission(userId, permissionId);
        //}
        //#endregion
    }

    public class UserRole
    {
        public string UserId { get; private set; }

        // ユーザーに直接割り当てられたロールのリスト（Int→String）
        private readonly List<string> _assignedRoleNames = new List<string>();

        // その他の既存プロパティはそのまま
        private readonly HashSet<int> _additionalPermissionIds = new HashSet<int>();
        private readonly HashSet<int> _deniedPermissionIds = new HashSet<int>();

        public UserRole(string userId)
        {
            UserId = userId;
        }

        // プロパティ名の変更
        public IReadOnlyList<string> AssignedRoleNames => _assignedRoleNames.AsReadOnly();
        public IReadOnlyCollection<int> AdditionalPermissionIds => _additionalPermissionIds;
        public IReadOnlyCollection<int> DeniedPermissionIds => _deniedPermissionIds;

        // メソッド名の更新
        public void AssignRole(string roleName)
        {
            if (!_assignedRoleNames.Contains(roleName))
            {
                _assignedRoleNames.Add(roleName);
            }
        }

        public void UnassignRole(string roleName)
        {
            _assignedRoleNames.Remove(roleName);
        }

        // その他のメソッドは変更なし
        public void GrantAdditionalPermission(int permissionId)
        {
            _additionalPermissionIds.Add(permissionId);
            _deniedPermissionIds.Remove(permissionId);
        }

        public void RevokeAdditionalPermission(int permissionId)
        {
            _additionalPermissionIds.Remove(permissionId);
        }

        public void DenyPermission(int permissionId)
        {
            _deniedPermissionIds.Add(permissionId);
            _additionalPermissionIds.Remove(permissionId);
        }

        public void RemoveDeniedPermission(int permissionId)
        {
            _deniedPermissionIds.Remove(permissionId);
        }

    }


    /// <summary>
    /// BitArrayクラスの拡張メソッドを提供します。
    /// </summary>
    public static class BitArrayExtensions
    {

        /// <summary>
        /// BitArray内に少なくとも1つ以上のtrueビットがあるかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>1つでもtrueビットがあればtrue、そうでなければfalse</returns>
        public static bool Any(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// BitArray内のすべてのビットがtrueかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがtrueならtrue、そうでなければfalse</returns>
        public static bool All(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (!bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 指定した位置のビットを反転させます。
        /// </summary>
        /// <param name="bitArray">操作対象のBitArray</param>
        /// <param name="index">反転するビットのインデックス</param>
        /// <returns>操作後のBitArray（元のインスタンスと同じ）</returns>
        public static BitArray Flip(this BitArray bitArray, int index)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (index < 0 || index >= bitArray.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            bitArray[index] = !bitArray[index];
            return bitArray;
        }

        /// <summary>
        /// BitArray内のすべてのビットがfalseかどうかを判定します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがfalseならtrue</returns>
        public static bool IsEmpty(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 2つのBitArrayが等しいかどうかを比較します。
        /// </summary>
        /// <param name="bitArray">比較元のBitArray</param>
        /// <param name="other">比較対象のBitArray</param>
        /// <returns>等しい場合はtrue、そうでなければfalse</returns>
        public static bool SequenceEqual(this BitArray bitArray, BitArray other)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (bitArray.Length != other.Length)
                return false;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i] != other[i])
                    return false;
            }
            return true;
        }
    }
}
