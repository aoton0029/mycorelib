using CoreLibWinforms.Permissions;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class PermissionRegistry
    {
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

    }
}
