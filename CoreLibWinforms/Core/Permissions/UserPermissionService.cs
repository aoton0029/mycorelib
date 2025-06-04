using CoreLibWinforms.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class UserPermissionService
    {
        private readonly Dictionary<string, UserPermissionProfile> _users = new Dictionary<string, UserPermissionProfile>();

        #region UserPermission
        public UserPermissionProfile RegisterNewUserPermission(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (_users.ContainsKey(userId))
                throw new InvalidOperationException($"User with ID {userId} already exists.");

            var permission = new UserPermissionProfile(userId);
            _users[userId] = permission;
            return permission;
        }

        public UserPermissionProfile GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            if (_users.TryGetValue(userId, out UserPermissionProfile user))
                return user;

            return null;
        }

        public void AssignRoleToUser(string userId, int roleId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.AssignRole(roleId);
        }

        public void UnassignRoleFromUser(string userId, int roleId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.UnassignRole(roleId);
        }

        public IEnumerable<UserPermissionProfile> GetAllUsers()
        {
            return _users.Values;
        }

        public void GrantAdditionalPermissionToUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.GrantAdditionalPermission(permissionId);
        }

        public void RevokeAdditionalPermissionFromUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.RevokeAdditionalPermission(permissionId);
        }

        public void DenyPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.DenyPermission(permissionId);
        }

        public void RemoveDeniedPermissionForUser(string userId, int permissionId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            user.RemoveDeniedPermission(permissionId);
        }

        public bool UserHasRole(string userId, int roleId)
        {
            var user = GetUser(userId);
            return user != null && user.HasRole(roleId);
        }

        public IEnumerable<int> GetUserRoleIds(string userId)
        {
            var user = GetUser(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
            return user.AssignedRoleIds;
        }
        #endregion

    }
}
