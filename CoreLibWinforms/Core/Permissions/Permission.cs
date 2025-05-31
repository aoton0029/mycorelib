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
        public int Id { get; private set; }
        public string Name { get; private set; }
        public BitArray Permissions { get; private set; }

        public Role(int id, string name, int initialCapacity = 32)
        {
            if (id < 0)
                throw new ArgumentException("Role ID must be non-negative", nameof(id));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));

            Id = id;
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
    }

    // ユーザー権限を管理するクラス
    public class PermissionManager
    {
        public static int MaxPermissionId = 32;
        public static string PermissionsFilePath = "permissions.json";
        public static string UserRoleFilePath = "user_roles.json";

        private readonly Dictionary<int, Permission> _permissions = new Dictionary<int, Permission>();
        private readonly Dictionary<int, Role> _roles = new Dictionary<int, Role>();
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
        public Role CreateRole(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty", nameof(name));
            int newId = _roles.Count;
            var role = new Role(newId, name);
            _roles.Add(newId, role);
            return role;
        }

        public Role GetRole(int id)
        {
            if (!_roles.TryGetValue(id, out var role))
                throw new KeyNotFoundException($"Role with ID {id} not found");

            return role;
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _roles.Values;
        }
        #endregion

        #region Role-Permission
        public void GrantPermissionToRole(int roleId, int permissionId)
        {
            var role = GetRole(roleId);
            var permission = GetPermission(permissionId);
            role.GrantPermission(permission);
        }

        public void RevokePermissionFromRole(int roleId, int permissionId)
        {
            var role = GetRole(roleId);
            var permission = GetPermission(permissionId);
            role.RevokePermission(permission);
        }

        public bool HasPermission(int roleId, int permissionId)
        {
            var role = GetRole(roleId);
            var permission = GetPermission(permissionId);
            return role.HasPermission(permission);
        }
        #endregion

        #region UserRole
        public UserRole CreateUser(string userId, Role role)
        {
            if (_users.ContainsKey(userId))
                throw new InvalidOperationException($"User with ID {userId} already exists");

            var user = new UserRole(userId);
            _users.Add(userId, user);
            return user;
        }

        public UserRole GetUser(string userId)
        {
            if (!_users.TryGetValue(userId, out var user))
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return user;
        }

        public IEnumerable<UserRole> GetAllUsers()
        {
            return _users.Values;
        }

        public void AssignRoleToUser(string userId, int roleId)
        {
            var user = GetUser(userId);
            // ロールの存在確認
            GetRole(roleId);
            user.AssignRole(roleId);
        }

        public void UnassignRoleFromUser(string userId, int roleId)
        {
            var user = GetUser(userId);
            user.UnassignRole(roleId);
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
            foreach (var roleId in user.AssignedRoleIds)
            {
                if (HasPermission(roleId, permissionId))
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
            foreach (var roleId in user.AssignedRoleIds)
            {
                var role = GetRole(roleId);
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
    }

    public class UserRole
    {
        public string UserId { get; private set; }

        // ユーザーに直接割り当てられたロールのリスト
        private readonly List<int> _assignedRoleIds = new List<int>();

        // ユーザー固有の追加権限（ロールに含まれない権限）
        private readonly HashSet<int> _additionalPermissionIds = new HashSet<int>();

        // ユーザー固有の拒否権限（ロールに含まれていても無効にする）
        private readonly HashSet<int> _deniedPermissionIds = new HashSet<int>();

        public UserRole(string userId)
        {
            UserId = userId;
        }

        public IReadOnlyList<int> AssignedRoleIds => _assignedRoleIds.AsReadOnly();
        public IReadOnlyCollection<int> AdditionalPermissionIds => _additionalPermissionIds;
        public IReadOnlyCollection<int> DeniedPermissionIds => _deniedPermissionIds;

        public void AssignRole(int roleId)
        {
            if (!_assignedRoleIds.Contains(roleId))
            {
                _assignedRoleIds.Add(roleId);
            }
        }

        public void UnassignRole(int roleId)
        {
            _assignedRoleIds.Remove(roleId);
        }

        public void GrantAdditionalPermission(int permissionId)
        {
            _additionalPermissionIds.Add(permissionId);
            _deniedPermissionIds.Remove(permissionId); // 同時に拒否リストから削除
        }

        public void RevokeAdditionalPermission(int permissionId)
        {
            _additionalPermissionIds.Remove(permissionId);
        }

        public void DenyPermission(int permissionId)
        {
            _deniedPermissionIds.Add(permissionId);
            _additionalPermissionIds.Remove(permissionId); // 同時に追加リストから削除
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
