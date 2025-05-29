using CoreLibWinforms.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public static  class PermissionHelper
    {
        // ユーザーが特定の権限を持っているかチェック
        public static bool HasPermission(string userId, int permissionId)
        {
            var manager = PermissionManager.Instance;
            var userRole = manager.GetUserRole(userId);

            if (userRole == null)
                return false;

            foreach (var roleId in userRole.RoleIds)
            {
                var role = manager.GetRole(roleId);
                if (role != null && role.PermissionIds.Contains(permissionId))
                    return true;
            }

            return false;
        }

        // ユーザーの全権限を取得
        public static List<Permission> GetUserPermissions(string userId)
        {
            var manager = PermissionManager.Instance;
            var userRole = manager.GetUserRole(userId);
            var result = new HashSet<int>();

            if (userRole == null)
                return new List<Permission>();

            foreach (var roleId in userRole.RoleIds)
            {
                var role = manager.GetRole(roleId);
                if (role != null)
                {
                    foreach (var permissionId in role.PermissionIds)
                    {
                        result.Add(permissionId);
                    }
                }
            }

            return manager.GetAllPermissions()
                .Where(p => result.Contains(p.Id))
                .ToList();
        }

        // ユーザーの全ロールを取得
        public static List<Role> GetUserRoles(string userId)
        {
            var manager = PermissionManager.Instance;
            var userRole = manager.GetUserRole(userId);

            if (userRole == null)
                return new List<Role>();

            return manager.GetAllRoles()
                .Where(r => userRole.RoleIds.Contains(r.Id))
                .ToList();
        }
    }
}
