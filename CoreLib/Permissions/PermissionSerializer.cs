using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreLib.Permissions
{
    public class PermissionSerializer
    {
        // ロールの保存
        public static void SaveRoles(IEnumerable<Role> roles, string filePath)
        {
            var roleDataList = new List<RoleData>();

            foreach (var role in roles)
            {
                var permissionDataList = new List<PermissionData>();
                foreach (var permission in role.Permissions)
                {
                    permissionDataList.Add(new PermissionData
                    {
                        Resource = permission.Resource,
                        Permissions = permission.Permissions
                    });
                }

                roleDataList.Add(new RoleData
                {
                    Name = role.Name,
                    Description = role.Description,
                    Permissions = permissionDataList
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(roleDataList, options);
            File.WriteAllText(filePath, jsonString);
        }

        // ロールの読み込み
        public static List<Role> LoadRoles(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Role>();

            string jsonString = File.ReadAllText(filePath);
            var roleDataList = JsonSerializer.Deserialize<List<RoleData>>(jsonString);

            var roles = new List<Role>();
            foreach (var roleData in roleDataList)
            {
                var role = new Role(roleData.Name, roleData.Description);

                foreach (var permissionData in roleData.Permissions)
                {
                    role.AddPermission(new Permission(
                        permissionData.Resource,
                        permissionData.Permissions));
                }

                roles.Add(role);
            }

            return roles;
        }

        // ユーザーの保存
        public static void SaveUsers(IEnumerable<User> users, string filePath)
        {
            var userDataList = new List<UserData>();

            foreach (var user in users)
            {
                var roleNames = new List<string>();
                foreach (var role in user.Roles)
                {
                    roleNames.Add(role.Name);
                }

                var directPermissions = new List<PermissionData>();
                foreach (var permission in user.DirectPermissions)
                {
                    directPermissions.Add(new PermissionData
                    {
                        Resource = permission.Resource,
                        Permissions = permission.Permissions
                    });
                }

                userDataList.Add(new UserData
                {
                    Id = user.Id,
                    Username = user.Username,
                    RoleNames = roleNames,
                    DirectPermissions = directPermissions
                });
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(userDataList, options);
            File.WriteAllText(filePath, jsonString);
        }

        // ユーザーの読み込み
        public static List<User> LoadUsers(string filePath, Dictionary<string, Role> roles)
        {
            if (!File.Exists(filePath))
                return new List<User>();

            string jsonString = File.ReadAllText(filePath);
            var userDataList = JsonSerializer.Deserialize<List<UserData>>(jsonString);

            var users = new List<User>();
            foreach (var userData in userDataList)
            {
                var user = new User(userData.Id, userData.Username);

                // ロールの割り当て
                foreach (var roleName in userData.RoleNames)
                {
                    if (roles.TryGetValue(roleName, out var role))
                    {
                        user.AddRole(role);
                    }
                }

                // 直接権限の割り当て
                foreach (var permissionData in userData.DirectPermissions)
                {
                    user.AddDirectPermission(new Permission(
                        permissionData.Resource,
                        permissionData.Permissions));
                }

                users.Add(user);
            }

            return users;
        }

        // JSONシリアライズ用のデータクラス
        private class RoleData
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<PermissionData> Permissions { get; set; }
        }

        private class UserData
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public List<string> RoleNames { get; set; }
            public List<PermissionData> DirectPermissions { get; set; }
        }

        private class PermissionData
        {
            public ResourceType Resource { get; set; }
            public PermissionType Permissions { get; set; }
        }
    }
}
