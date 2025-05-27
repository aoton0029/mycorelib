using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Permissions
{
    /// <summary>
    /// 権限を表す列挙型
    /// </summary>
    [Flags]
    public enum PermissionType
    {
        None = 0,
        View = 1 << 0,
        Create = 1 << 1,
        Edit = 1 << 2,
        Delete = 1 << 3,
        Export = 1 << 4,
        Import = 1 << 5,
        Admin = 1 << 6,
        // 必要に応じて追加
    }

    /// <summary>
    /// リソースタイプを表す列挙型
    /// </summary>
    public enum ResourceType
    {
        None = 0,
        User,
        Customer,
        Order,
        Product,
        Report,
        Setting,
        // 必要に応じて追加
    }

    /// <summary>
    /// 特定のリソースに対する権限を表すクラス
    /// </summary>
    public class Permission
    {
        public ResourceType Resource { get; private set; }
        public PermissionType Permissions { get; private set; }

        public Permission(ResourceType resource, PermissionType permissions)
        {
            Resource = resource;
            Permissions = permissions;
        }

        public bool HasPermission(PermissionType permission)
        {
            return (Permissions & permission) == permission;
        }
    }

    /// <summary>
    /// ロール（役割）を表すクラス
    /// </summary>
    public class Role
    {
        public string Name { get; private set; }
        public string Description { get; set; }
        private readonly List<Permission> _permissions = new List<Permission>();

        public Role(string name, string description = "")
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
        }

        public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

        public void AddPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            // 既存の同じリソースの権限を更新または追加
            var existingPermission = _permissions.FirstOrDefault(p => p.Resource == permission.Resource);
            if (existingPermission != null)
            {
                _permissions.Remove(existingPermission);
            }

            _permissions.Add(permission);
        }

        public void RemovePermission(ResourceType resource)
        {
            var permission = _permissions.FirstOrDefault(p => p.Resource == resource);
            if (permission != null)
            {
                _permissions.Remove(permission);
            }
        }

        public bool HasPermission(ResourceType resource, PermissionType permission)
        {
            var resourcePermission = _permissions.FirstOrDefault(p => p.Resource == resource);
            return resourcePermission != null && resourcePermission.HasPermission(permission);
        }
    }

    /// <summary>
    /// システムユーザーを表すクラス
    /// </summary>
    public class User
    {
        public string Id { get; private set; }
        public string Username { get; set; }
        private readonly List<Role> _roles = new List<Role>();
        private readonly List<Permission> _directPermissions = new List<Permission>();

        public User(string id, string username)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }

        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
        public IReadOnlyList<Permission> DirectPermissions => _directPermissions.AsReadOnly();

        public void AddRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (!_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void RemoveRole(Role role)
        {
            _roles.Remove(role);
        }

        public void AddDirectPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            // 既存の同じリソースの権限を更新または追加
            var existingPermission = _directPermissions.FirstOrDefault(p => p.Resource == permission.Resource);
            if (existingPermission != null)
            {
                _directPermissions.Remove(existingPermission);
            }

            _directPermissions.Add(permission);
        }

        public void RemoveDirectPermission(ResourceType resource)
        {
            var permission = _directPermissions.FirstOrDefault(p => p.Resource == resource);
            if (permission != null)
            {
                _directPermissions.Remove(permission);
            }
        }

        public bool HasPermission(ResourceType resource, PermissionType permission)
        {
            // 直接付与された権限をチェック
            var directPermission = _directPermissions.FirstOrDefault(p => p.Resource == resource);
            if (directPermission != null && directPermission.HasPermission(permission))
            {
                return true;
            }

            // ロールに基づく権限をチェック
            return _roles.Any(role => role.HasPermission(resource, permission));
        }
    }

    /// <summary>
    /// 権限管理を担当するシングルトンクラス
    /// </summary>
    public class PermissionManager
    {
        private static readonly Lazy<PermissionManager> _instance = new Lazy<PermissionManager>(() => new PermissionManager());
        public static PermissionManager Instance => _instance.Value;

        private readonly Dictionary<string, User> _users = new Dictionary<string, User>();
        private readonly Dictionary<string, Role> _roles = new Dictionary<string, Role>();
        private User _currentUser;

        private PermissionManager() { }

        public void Initialize(IEnumerable<Role> predefinedRoles = null)
        {
            // 定義済みロールの登録
            if (predefinedRoles != null)
            {
                foreach (var role in predefinedRoles)
                {
                    AddRole(role);
                }
            }
        }

        public void SetCurrentUser(User user)
        {
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _users[user.Id] = user;
        }

        public User GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            return _users.TryGetValue(userId, out var user) ? user : null;
        }

        public void AddRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            _roles[role.Name] = role;
        }

        public Role GetRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            return _roles.TryGetValue(roleName, out var role) ? role : null;
        }

        public bool CheckPermission(ResourceType resource, PermissionType permission)
        {
            if (_currentUser == null)
                return false;

            return _currentUser.HasPermission(resource, permission);
        }

        public bool CheckPermission(string userId, ResourceType resource, PermissionType permission)
        {
            var user = GetUser(userId);
            return user?.HasPermission(resource, permission) ?? false;
        }
    }
}
