using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class PermissionRegistry
    {
        public static string FilePath = "Permissions.json";
        private readonly Dictionary<int, Permission> _permissions = new Dictionary<int, Permission>();
        private readonly Dictionary<int, Role> _roles = new Dictionary<int, Role>();

        #region Permission
        public Permission RegisterNewPermission(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            // 新しいIDを作成（既存のIDの最大値 + 1）
            int newId = _permissions.Count > 0 ? _permissions.Keys.Max() + 1 : 0;
            var permission = new Permission(newId, name);
            _permissions.Add(newId, permission);

            return permission;
        }

        public Permission GetPermission(int id)
        {
            return _permissions.TryGetValue(id, out var permission) ? permission : null;
        }

        public bool ExistsPermission(int id)
        {
            return _permissions.ContainsKey(id);
        }

        public void RenamePermission(int id, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException(nameof(newName));

            if (_permissions.TryGetValue(id, out var permission))
            {
                permission.Name = newName;
            }
            else
            {
                throw new KeyNotFoundException($"Permission with ID {id} not found.");
            }
        }

        public void DeletePermission(int id)
        {
            if (_permissions.ContainsKey(id))
            {
                _permissions.Remove(id);
            }
        }

        public IEnumerable<Permission> GetAllPermissions()
        {
            return _permissions.Values;
        }
        #endregion

        #region Role
        public Role RegisterNewRole(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            // 重複チェック
            if (_roles.Values.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Role with name '{name}' already exists.");

            // 新しいIDを作成（既存のIDの最大値 + 1）
            int newId = _roles.Count > 0 ? _roles.Keys.Max() + 1 : 0;
            var role = new Role(name);
            role.Id = newId;
            _roles.Add(newId, role);

            return role;
        }

        public Role? GetRole(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return _roles.Values.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool ExistsRole(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _roles.Values.Any(r =>
                r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void RenameRole(int roleId, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException(nameof(newName));

            if (_roles.TryGetValue(roleId, out var role))
            {
                role.Name = newName;
            }
            else
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }
        }

        public void DeleteRole(string name)
        {
            var role = GetRole(name);
            if (role != null)
            {
                _roles.Remove(role.Id);
            }
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _roles.Values;
        }
        #endregion

        #region Role-Permission
        public void GrantPermissionToRole(int roleId, int permissionId)
        {
            var role = _roles.TryGetValue(roleId, out var r) ? r : null;
            var permission = _permissions.TryGetValue(permissionId, out var p) ? p : null;

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {permissionId} not found.");

            role.GrantPermission(permission);
        }

        public void RevokePermissionFromRole(int roleId, int permissionId)
        {
            var role = _roles.TryGetValue(roleId, out var r) ? r : null;
            var permission = _permissions.TryGetValue(permissionId, out var p) ? p : null;

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {permissionId} not found.");

            role.RevokePermission(permission);
        }

        public bool HasPermissionRole(int roleId, int permissionId)
        {
            var role = _roles.TryGetValue(roleId, out var r) ? r : null;
            var permission = _permissions.TryGetValue(permissionId, out var p) ? p : null;

            if (role == null || permission == null)
                return false;

            return role.HasPermission(permission);
        }
        #endregion

        public void Save()
        {
            // 権限リストの変換
            var permissionDataList = _permissions.Values.Select(p =>
                new PermissionData(p.Id, p.Name)).ToList();

            // ロールリストの変換
            var roleDataList = new List<RoleData>();
            foreach (var role in _roles.Values)
            {
                var roleData = new RoleData(role.Id, role.Name);

                // BitArrayからtrueになっているインデックスのリストを取得
                roleData.Permissions = role.Permissions.GetTrueIndices();

                roleDataList.Add(roleData);
            }

            // MasterDataオブジェクトの作成
            var masterData = new MasterData(permissionDataList, roleDataList);

            try
            {
                // JSONシリアライズ
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(masterData, options);

                // ファイルに保存
                File.WriteAllText(FilePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
                throw;
            }
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            try
            {
                // ファイルからJSONを読み込み
                string jsonString = File.ReadAllText(FilePath);

                // JSONデシリアライズ
                var masterData = JsonSerializer.Deserialize<MasterData>(jsonString);

                if (masterData == null)
                {
                    return;
                }

                // 現在のデータをクリア
                _permissions.Clear();
                _roles.Clear();

                // 権限の読み込み
                foreach (var permData in masterData.Permissions)
                {
                    var permission = new Permission(permData.Id, permData.Name);
                    _permissions[permission.Id] = permission;
                }

                // ロールの読み込み
                foreach (var roleData in masterData.Roles)
                {
                    var role = new Role(roleData.Name);
                    role.Id = roleData.Id;

                    // 権限の設定
                    if (roleData.Permissions != null)
                    {
                        foreach (var permId in roleData.Permissions)
                        {
                            if (_permissions.TryGetValue(permId, out Permission permission))
                            {
                                role.GrantPermission(permission);
                            }
                        }
                    }

                    _roles[role.Id] = role;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
                throw;
            }
        }
    }

    public class MasterData
    {
        public List<PermissionData> Permissions { get; set; } = new List<PermissionData>();
        public List<RoleData> Roles { get; set; } = new List<RoleData>();
        public MasterData() { }
        public MasterData(List<PermissionData> permissions, List<RoleData> roles)
        {
            Permissions = permissions;
            Roles = roles;
        }
    }
    public class PermissionData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PermissionData(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class RoleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Permissions { get; set; } = new List<int>();
        public RoleData(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Permission
    {
        // 権限のID（これはBitArrayの位置としても使う）
        public int Id { get; set; }
        // 権限の名前
        public string Name { get; set; }

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
    }
}
