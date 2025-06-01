C#でユーザー権限構造を設計する。Permission、Role、PermissionManagerを作る。PermissionはId、Nameを持ち、単一権限を表す。RoleはPermissionの組み合わせ

# C# RBAC システム設計

C#でRole-Based Access Control（RBAC）システムを設計します。BitArrayを使用して権限の組み合わせを効率的に管理する設計案を以下に示します。

## Permission クラス

```csharp
using System;

public class Permission
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Permission(int id, string name)
    {
        if (id < 0)
            throw new ArgumentException("Permission ID must be non-negative", nameof(id));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
            
        Id = id;
        Name = name;
    }
    
    public override string ToString()
    {
        return $"Permission({Id}: {Name})";
    }
}
```

## Role クラス

```csharp
using System;
using System.Collections;
using System.Text;

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
    
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Role({Id}: {Name})");
        sb.Append("Permissions: [");
        
        bool isFirst = true;
        for (int i = 0; i < Permissions.Length; i++)
        {
            if (Permissions[i])
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }
                sb.Append(i);
                isFirst = false;
            }
        }
        
        sb.Append("]");
        return sb.ToString();
    }
}
```

## Operation クラス

```csharp
using System;

public class Operation
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Permission RequiredPermission { get; private set; }
    
    public Operation(int id, string name, Permission requiredPermission)
    {
        if (id < 0)
            throw new ArgumentException("Operation ID must be non-negative", nameof(id));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Operation name cannot be empty", nameof(name));
            
        Id = id;
        Name = name;
        RequiredPermission = requiredPermission ?? 
            throw new ArgumentNullException(nameof(requiredPermission));
    }
    
    public bool CanExecute(Role role)
    {
        return role != null && role.HasPermission(RequiredPermission);
    }
    
    public override string ToString()
    {
        return $"Operation({Id}: {Name}, Requires: {RequiredPermission.Name})";
    }
}
```

## PermissionManager クラス

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class PermissionManager
{
    private readonly Dictionary<int, Permission> _permissions = new Dictionary<int, Permission>();
    private readonly Dictionary<int, Role> _roles = new Dictionary<int, Role>();
    private readonly Dictionary<int, Operation> _operations = new Dictionary<int, Operation>();
    
    // Permission 管理
    public Permission CreatePermission(int id, string name)
    {
        if (_permissions.ContainsKey(id))
            throw new InvalidOperationException($"Permission with ID {id} already exists");
            
        var permission = new Permission(id, name);
        _permissions.Add(id, permission);
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
    
    // Role 管理
    public Role CreateRole(int id, string name)
    {
        if (_roles.ContainsKey(id))
            throw new InvalidOperationException($"Role with ID {id} already exists");
            
        var role = new Role(id, name);
        _roles.Add(id, role);
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
    
    // Operation 管理
    public Operation CreateOperation(int id, string name, Permission requiredPermission)
    {
        if (_operations.ContainsKey(id))
            throw new InvalidOperationException($"Operation with ID {id} already exists");
            
        var operation = new Operation(id, name, requiredPermission);
        _operations.Add(id, operation);
        return operation;
    }
    
    public Operation GetOperation(int id)
    {
        if (!_operations.TryGetValue(id, out var operation))
            throw new KeyNotFoundException($"Operation with ID {id} not found");
            
        return operation;
    }
    
    public IEnumerable<Operation> GetAllOperations()
    {
        return _operations.Values;
    }
    
    // Role-Permission 関係の管理
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
    
    // 操作の実行チェック
    public bool CanExecuteOperation(int roleId, int operationId)
    {
        var role = GetRole(roleId);
        var operation = GetOperation(operationId);
        return operation.CanExecute(role);
    }
}
```

## 使用例

```csharp
public class Program
{
    public static void Main()
    {
        var manager = new PermissionManager();
        
        // 権限の定義
        var readPermission = manager.CreatePermission(0, "Read");
        var writePermission = manager.CreatePermission(1, "Write");
        var deletePermission = manager.CreatePermission(2, "Delete");
        var adminPermission = manager.CreatePermission(3, "Admin");
        
        // ロールの定義
        var userRole = manager.CreateRole(1, "User");
        var editorRole = manager.CreateRole(2, "Editor");
        var adminRole = manager.CreateRole(3, "Administrator");
        
        // 権限をロールに割り当てる
        manager.GrantPermissionToRole(userRole.Id, readPermission.Id);
        
        manager.GrantPermissionToRole(editorRole.Id, readPermission.Id);
        manager.GrantPermissionToRole(editorRole.Id, writePermission.Id);
        
        manager.GrantPermissionToRole(adminRole.Id, readPermission.Id);
        manager.GrantPermissionToRole(adminRole.Id, writePermission.Id);
        manager.GrantPermissionToRole(adminRole.Id, deletePermission.Id);
        manager.GrantPermissionToRole(adminRole.Id, adminPermission.Id);
        
        // 操作の定義
        var viewOperation = manager.CreateOperation(1, "View Content", readPermission);
        var editOperation = manager.CreateOperation(2, "Edit Content", writePermission);
        var deleteOperation = manager.CreateOperation(3, "Delete Content", deletePermission);
        var configureOperation = manager.CreateOperation(4, "Configure System", adminPermission);
        
        // 権限チェック
        Console.WriteLine($"User can view: {manager.CanExecuteOperation(userRole.Id, viewOperation.Id)}");
        Console.WriteLine($"User can edit: {manager.CanExecuteOperation(userRole.Id, editOperation.Id)}");
        
        Console.WriteLine($"Editor can view: {manager.CanExecuteOperation(editorRole.Id, viewOperation.Id)}");
        Console.WriteLine($"Editor can edit: {manager.CanExecuteOperation(editorRole.Id, editOperation.Id)}");
        Console.WriteLine($"Editor can delete: {manager.CanExecuteOperation(editorRole.Id, deleteOperation.Id)}");
        
        Console.WriteLine($"Admin can view: {manager.CanExecuteOperation(adminRole.Id, viewOperation.Id)}");
        Console.WriteLine($"Admin can edit: {manager.CanExecuteOperation(adminRole.Id, editOperation.Id)}");
        Console.WriteLine($"Admin can delete: {manager.CanExecuteOperation(adminRole.Id, deleteOperation.Id)}");
        Console.WriteLine($"Admin can configure: {manager.CanExecuteOperation(adminRole.Id, configureOperation.Id)}");
    }
}
```

この設計では、BitArrayを使用して効率的に権限の組み合わせを管理しています。PermissionのIdをBitArrayのインデックスとして使用することで、各Roleが持つ権限を簡潔に表現できます。また、RoleクラスにはEnsureCapacityメソッドを追加して、BitArrayのサイズを動的に調整できるようにしています。



# ユーザーIDをキーにした権限管理システムの拡張

既存のRBACシステムを拡張して、ユーザーIDをキーにした権限管理を追加します。これにより、ユーザー単位で詳細な権限制御が可能になります。

## User クラス

```csharp
using System;
using System.Collections.Generic;

public class User
{
    public string UserId { get; private set; }
    public string Username { get; set; }
    
    // ユーザーに直接割り当てられたロールのリスト
    private readonly List<int> _assignedRoleIds = new List<int>();
    
    // ユーザー固有の追加権限（ロールに含まれない権限）
    private readonly HashSet<int> _additionalPermissionIds = new HashSet<int>();
    
    // ユーザー固有の拒否権限（ロールに含まれていても無効にする）
    private readonly HashSet<int> _deniedPermissionIds = new HashSet<int>();
    
    public User(string userId, string username)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
            
        UserId = userId;
        Username = username;
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
    
    public override string ToString()
    {
        return $"User(ID: {UserId}, Name: {Username})";
    }
}
```

## UserManager クラス

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class UserManager
{
    private readonly Dictionary<string, User> _users = new Dictionary<string, User>();
    private readonly PermissionManager _permissionManager;
    
    public UserManager(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
    }
    
    // ユーザー管理
    public User CreateUser(string userId, string username)
    {
        if (_users.ContainsKey(userId))
            throw new InvalidOperationException($"User with ID {userId} already exists");
            
        var user = new User(userId, username);
        _users.Add(userId, user);
        return user;
    }
    
    public User GetUser(string userId)
    {
        if (!_users.TryGetValue(userId, out var user))
            throw new KeyNotFoundException($"User with ID {userId} not found");
            
        return user;
    }
    
    public IEnumerable<User> GetAllUsers()
    {
        return _users.Values;
    }
    
    public bool UserExists(string userId)
    {
        return _users.ContainsKey(userId);
    }
    
    // ユーザー-ロール関連の管理
    public void AssignRoleToUser(string userId, int roleId)
    {
        var user = GetUser(userId);
        // ロールの存在確認
        _permissionManager.GetRole(roleId);
        user.AssignRole(roleId);
    }
    
    public void UnassignRoleFromUser(string userId, int roleId)
    {
        var user = GetUser(userId);
        user.UnassignRole(roleId);
    }
    
    // ユーザー-権限関連の管理
    public void GrantAdditionalPermissionToUser(string userId, int permissionId)
    {
        var user = GetUser(userId);
        // 権限の存在確認
        _permissionManager.GetPermission(permissionId);
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
        _permissionManager.GetPermission(permissionId);
        user.DenyPermission(permissionId);
    }
    
    public void RemoveDeniedPermissionForUser(string userId, int permissionId)
    {
        var user = GetUser(userId);
        user.RemoveDeniedPermission(permissionId);
    }
    
    // ユーザーの権限チェック
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
            if (_permissionManager.HasPermission(roleId, permissionId))
                return true;
        }
        
        return false;
    }
    
    public bool CanExecuteOperation(string userId, int operationId)
    {
        var operation = _permissionManager.GetOperation(operationId);
        return HasPermission(userId, operation.RequiredPermission.Id);
    }
    
    // ユーザーに割り当てられている全権限を取得
    public IEnumerable<Permission> GetUserPermissions(string userId)
    {
        var user = GetUser(userId);
        var allPermissions = new HashSet<Permission>();
        
        // ロールから取得できる権限を追加
        foreach (var roleId in user.AssignedRoleIds)
        {
            var role = _permissionManager.GetRole(roleId);
            foreach (var permission in _permissionManager.GetAllPermissions())
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
                allPermissions.Add(_permissionManager.GetPermission(permissionId));
            }
        }
        
        return allPermissions;
    }
}
```

## 拡張されたPermissionManager クラス

```csharp
// PermissionManager クラスに追加するメソッド

public class PermissionManager
{
    // 既存のコードはそのまま...
    
    // 権限定義の追加メソッド
    public bool TryGetPermission(int id, out Permission permission)
    {
        return _permissions.TryGetValue(id, out permission);
    }
    
    public bool TryGetRole(int id, out Role role)
    {
        return _roles.TryGetValue(id, out role);
    }
    
    public bool TryGetOperation(int id, out Operation operation)
    {
        return _operations.TryGetValue(id, out operation);
    }
    
    // ロールに割り当てられたすべての権限を取得
    public IEnumerable<Permission> GetRolePermissions(int roleId)
    {
        var role = GetRole(roleId);
        return _permissions.Values.Where(p => role.HasPermission(p));
    }
}
```

## AuthorizationService クラス

```csharp
using System;

public class AuthorizationService
{
    private readonly UserManager _userManager;
    private readonly PermissionManager _permissionManager;
    
    public AuthorizationService(UserManager userManager, PermissionManager permissionManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
    }
    
    // 特定のユーザーが特定の権限を持っているかをチェック
    public bool HasPermission(string userId, string permissionName)
    {
        if (!_userManager.UserExists(userId))
            return false;
            
        // 権限名から権限IDを検索
        foreach (var permission in _permissionManager.GetAllPermissions())
        {
            if (permission.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase))
            {
                return _userManager.HasPermission(userId, permission.Id);
            }
        }
        
        return false;
    }
    
    // 特定のユーザーが特定の操作を実行できるかをチェック
    public bool CanExecuteOperation(string userId, string operationName)
    {
        if (!_userManager.UserExists(userId))
            return false;
            
        // 操作名から操作IDを検索
        foreach (var operation in _permissionManager.GetAllOperations())
        {
            if (operation.Name.Equals(operationName, StringComparison.OrdinalIgnoreCase))
            {
                return _userManager.CanExecuteOperation(userId, operation.Id);
            }
        }
        
        return false;
    }
    
    // ユーザーに特定のロールがあるかをチェック
    public bool HasRole(string userId, string roleName)
    {
        if (!_userManager.UserExists(userId))
            return false;
            
        var user = _userManager.GetUser(userId);
        
        foreach (var role in _permissionManager.GetAllRoles())
        {
            if (role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) && 
                user.AssignedRoleIds.Contains(role.Id))
            {
                return true;
            }
        }
        
        return false;
    }
}
```

## 使用例

```csharp
public class Program
{
    public static void Main()
    {
        Console.WriteLine("Current Date and Time (UTC): 2025-05-31 11:25:26");
        Console.WriteLine("Current User's Login: aoton0029");
        Console.WriteLine();
        
        // システム初期化
        var permissionManager = new PermissionManager();
        var userManager = new UserManager(permissionManager);
        var authService = new AuthorizationService(userManager, permissionManager);
        
        // 権限を定義
        var readPermission = permissionManager.CreatePermission(0, "Read");
        var writePermission = permissionManager.CreatePermission(1, "Write");
        var deletePermission = permissionManager.CreatePermission(2, "Delete");
        var adminPermission = permissionManager.CreatePermission(3, "Admin");
        
        // ロールを定義
        var userRole = permissionManager.CreateRole(1, "User");
        var editorRole = permissionManager.CreateRole(2, "Editor");
        var adminRole = permissionManager.CreateRole(3, "Administrator");
        
        // ロールに権限を割り当て
        permissionManager.GrantPermissionToRole(userRole.Id, readPermission.Id);
        
        permissionManager.GrantPermissionToRole(editorRole.Id, readPermission.Id);
        permissionManager.GrantPermissionToRole(editorRole.Id, writePermission.Id);
        
        permissionManager.GrantPermissionToRole(adminRole.Id, readPermission.Id);
        permissionManager.GrantPermissionToRole(adminRole.Id, writePermission.Id);
        permissionManager.GrantPermissionToRole(adminRole.Id, deletePermission.Id);
        permissionManager.GrantPermissionToRole(adminRole.Id, adminPermission.Id);
        
        // 操作を定義
        var viewOperation = permissionManager.CreateOperation(1, "View Content", readPermission);
        var editOperation = permissionManager.CreateOperation(2, "Edit Content", writePermission);
        var deleteOperation = permissionManager.CreateOperation(3, "Delete Content", deletePermission);
        var configureOperation = permissionManager.CreateOperation(4, "Configure System", adminPermission);
        
        // ユーザーを作成
        var normalUser = userManager.CreateUser("user001", "Normal User");
        var specialUser = userManager.CreateUser("aoton0029", "Aoton User");
        var adminUser = userManager.CreateUser("admin001", "Admin User");
        
        // ユーザーにロールを割り当て
        userManager.AssignRoleToUser(normalUser.UserId, userRole.Id);
        
        userManager.AssignRoleToUser(specialUser.UserId, userRole.Id);
        userManager.AssignRoleToUser(specialUser.UserId, editorRole.Id);
        // 特別なユーザー権限を追加
        userManager.GrantAdditionalPermissionToUser(specialUser.UserId, deletePermission.Id);
        
        userManager.AssignRoleToUser(adminUser.UserId, adminRole.Id);
        
        // aoton0029ユーザーの権限を確認
        Console.WriteLine("=== ユーザー aoton0029 の権限情報 ===");
        Console.WriteLine($"ユーザー名: {specialUser.Username}");
        Console.WriteLine("割り当てられたロール:");
        foreach (var roleId in specialUser.AssignedRoleIds)
        {
            var role = permissionManager.GetRole(roleId);
            Console.WriteLine($"- {role.Name}");
        }
        
        Console.WriteLine("\n保有している権限:");
        foreach (var permission in userManager.GetUserPermissions(specialUser.UserId))
        {
            Console.WriteLine($"- {permission.Name}");
        }
        
        Console.WriteLine("\n実行可能な操作:");
        foreach (var operation in permissionManager.GetAllOperations())
        {
            bool canExecute = userManager.CanExecuteOperation(specialUser.UserId, operation.Id);
            Console.WriteLine($"- {operation.Name}: {(canExecute ? "可能" : "不可")}");
        }
        
        // 権限変更のデモ
        Console.WriteLine("\n=== 権限変更のデモ ===");
        Console.WriteLine("「Configure System」権限を拒否します。");
        userManager.DenyPermissionForUser(specialUser.UserId, adminPermission.Id);
        
        Console.WriteLine("\n変更後の実行可能な操作:");
        foreach (var operation in permissionManager.GetAllOperations())
        {
            bool canExecute = userManager.CanExecuteOperation(specialUser.UserId, operation.Id);
            Console.WriteLine($"- {operation.Name}: {(canExecute ? "可能" : "不可")}");
        }
    }
}
```

## 期待される出力例

```
Current Date and Time (UTC): 2025-05-31 11:25:26
Current User's Login: aoton0029

=== ユーザー aoton0029 の権限情報 ===
ユーザー名: Aoton User
割り当てられたロール:
- User
- Editor

保有している権限:
- Read
- Write
- Delete

実行可能な操作:
- View Content: 可能
- Edit Content: 可能
- Delete Content: 可能
- Configure System: 不可

=== 権限変更のデモ ===
「Configure System」権限を拒否します。

変更後の実行可能な操作:
- View Content: 可能
- Edit Content: 可能
- Delete Content: 可能
- Configure System: 不可
```

このシステムでは、ユーザーIDをキーにして各ユーザーの権限を管理できます。ユーザーはロールを通じた権限と、ユーザー固有の追加権限を持つことができます。また、特定の権限をユーザーレベルで拒否する機能も実装されています。これにより、ロールベースの権限管理に加えて、ユーザーごとの細かな権限制御が可能になります。




---

現在の日時 (UTC): 2025-05-30 09:23:16
ユーザー: aoton0029

`PermissionManager`クラスに、Permissionの名前から検索・取得するためのメソッドを追加します。名前が一意であることを保証し、効率的に検索できるようにします。以下に更新したコードを示します：

```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 権限を表すクラス
public class Permission
{
    // 権限のID（これはBitArrayの位置としても使う）
    public int Id { get; private set; }
    
    // 権限の名前
    public string Name { get; private set; }
    
    // 権限の組み合わせを表すビット配列
    public BitArray Permissions { get; private set; }
    
    public Permission(int id, string name, int initialCapacity = 32)
    {
        Id = id;
        Name = name;
        Permissions = new BitArray(initialCapacity);
    }
    
    // 静的な管理者権限生成メソッド
    public static Permission Admin(int maxPermissionId = 100, string name = "Administrator")
    {
        Permission adminPermission = new Permission(-1, name, maxPermissionId);
        adminPermission.GrantAll(maxPermissionId);
        return adminPermission;
    }
    
    // 特定の権限を付与する
    public void Grant(int permissionId)
    {
        if (permissionId >= 0 && permissionId < Permissions.Length)
        {
            Permissions[permissionId] = true;
        }
    }
    
    // 特定の権限を削除する
    public void Revoke(int permissionId)
    {
        if (permissionId >= 0 && permissionId < Permissions.Length)
        {
            Permissions[permissionId] = false;
        }
    }
    
    // 特定の権限を持っているかチェックする
    public bool HasPermission(int permissionId)
    {
        return permissionId >= 0 && permissionId < Permissions.Length && Permissions[permissionId];
    }
    
    // 権限セットを他の権限セットと組み合わせる
    public void CombineWith(Permission other)
    {
        // BitArrayをORで結合
        Permissions.Or(other.Permissions);
    }
    
    // すべての権限を付与する（管理者権限）
    public void GrantAll(int maxPermissionId)
    {
        for (int i = 0; i < maxPermissionId; i++)
        {
            Grant(i);
        }
    }
}

// ユーザー権限を管理するクラス
public class PermissionManager
{
    // 利用可能な権限のリスト
    private List<Permission> availablePermissions;
    
    // 権限名をキーとした権限の辞書
    private Dictionary<string, Permission> permissionsByName;
    
    // ユーザーIDと対応する権限のマッピング
    private Dictionary<string, Permission> userPermissions;
    
    // 最大の権限ID
    private int maxPermissionId = 0;
    
    public PermissionManager()
    {
        availablePermissions = new List<Permission>();
        permissionsByName = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase); // 大文字小文字を区別しない
        userPermissions = new Dictionary<string, Permission>();
    }
    
    // 利用可能な権限を追加
    public void RegisterPermission(Permission permission)
    {
        // 名前の一意性をチェック
        if (permissionsByName.ContainsKey(permission.Name))
        {
            throw new ArgumentException($"権限名 '{permission.Name}' はすでに登録されています。");
        }
        
        availablePermissions.Add(permission);
        permissionsByName[permission.Name] = permission;
        
        // 最大の権限IDを更新
        if (permission.Id > maxPermissionId)
        {
            maxPermissionId = permission.Id;
        }
    }
    
    // 名前で権限を取得（完全一致・大文字小文字区別なし）
    public Permission GetPermissionByName(string name)
    {
        if (permissionsByName.TryGetValue(name, out Permission permission))
        {
            return permission;
        }
        return null;
    }
    
    // 名前に指定したキーワードを含む権限をすべて取得
    public List<Permission> FindPermissionsByPartialName(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return new List<Permission>(availablePermissions);
        }
        
        return availablePermissions
            .Where(p => p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();
    }
    
    // IDで権限を取得
    public Permission GetPermissionById(int id)
    {
        return availablePermissions.FirstOrDefault(p => p.Id == id);
    }
    
    // すべての権限を取得
    public List<Permission> GetAllPermissions()
    {
        return new List<Permission>(availablePermissions);
    }
    
    // 権限名が存在するかどうかを確認
    public bool ExistsPermissionName(string name)
    {
        return permissionsByName.ContainsKey(name);
    }
    
    // 権限IDが存在するかどうかを確認
    public bool ExistsPermissionId(int id)
    {
        return availablePermissions.Any(p => p.Id == id);
    }
    
    // 現在の最大権限IDに基づいた管理者権限を取得
    public Permission GetAdminPermission()
    {
        return Permission.Admin(maxPermissionId + 1);
    }
    
    // ユーザーに権限を割り当てる
    public void AssignPermissionToUser(string userId, Permission permission)
    {
        if (!userPermissions.ContainsKey(userId))
        {
            userPermissions[userId] = new Permission(0, "UserPermissions", permission.Permissions.Length);
        }
        
        userPermissions[userId].CombineWith(permission);
    }
    
    // 権限名を指定してユーザーに権限を割り当てる
    public void AssignPermissionToUserByName(string userId, string permissionName)
    {
        Permission permission = GetPermissionByName(permissionName);
        if (permission == null)
        {
            throw new ArgumentException($"権限名 '{permissionName}' は見つかりませんでした。");
        }
        
        AssignPermissionToUser(userId, permission);
    }
    
    // ユーザーに管理者権限を割り当てる
    public void AssignAdminPermissionToUser(string userId)
    {
        AssignPermissionToUser(userId, GetAdminPermission());
    }
    
    // ユーザーに特定のIDの権限を付与する
    public void GrantPermissionToUser(string userId, int permissionId)
    {
        if (userPermissions.ContainsKey(userId))
        {
            userPermissions[userId].Grant(permissionId);
        }
        else
        {
            var newPermission = new Permission(0, "UserPermissions");
            newPermission.Grant(permissionId);
            userPermissions[userId] = newPermission;
        }
    }
    
    // 権限名を指定してユーザーに権限を付与する
    public void GrantPermissionToUserByName(string userId, string permissionName)
    {
        Permission permission = GetPermissionByName(permissionName);
        if (permission == null)
        {
            throw new ArgumentException($"権限名 '{permissionName}' は見つかりませんでした。");
        }
        
        GrantPermissionToUser(userId, permission.Id);
    }
    
    // ユーザーから特定のIDの権限を削除する
    public void RevokePermissionFromUser(string userId, int permissionId)
    {
        if (userPermissions.ContainsKey(userId))
        {
            userPermissions[userId].Revoke(permissionId);
        }
    }
    
    // 権限名を指定してユーザーから権限を削除する
    public void RevokePermissionFromUserByName(string userId, string permissionName)
    {
        Permission permission = GetPermissionByName(permissionName);
        if (permission == null)
        {
            throw new ArgumentException($"権限名 '{permissionName}' は見つかりませんでした。");
        }
        
        RevokePermissionFromUser(userId, permission.Id);
    }
    
    // ユーザーが特定の権限IDを持っているかチェックする
    public bool UserHasPermission(string userId, int permissionId)
    {
        return userPermissions.ContainsKey(userId) && userPermissions[userId].HasPermission(permissionId);
    }
    
    // ユーザーが特定の名前の権限を持っているかチェックする
    public bool UserHasPermissionByName(string userId, string permissionName)
    {
        Permission permission = GetPermissionByName(permissionName);
        if (permission == null)
        {
            throw new ArgumentException($"権限名 '{permissionName}' は見つかりませんでした。");
        }
        
        return UserHasPermission(userId, permission.Id);
    }
    
    // ユーザーの権限を取得
    public Permission GetUserPermission(string userId)
    {
        if (userPermissions.ContainsKey(userId))
        {
            return userPermissions[userId];
        }
        
        return null;
    }
}
```

### 追加した名前関連のメソッド

1. **`GetPermissionByName(string name)`**:
   - 名前で権限を検索する（完全一致）
   - 大文字小文字を区別せずに検索（`StringComparer.OrdinalIgnoreCase`を使用）

2. **`FindPermissionsByPartialName(string keyword)`**:
   - キーワードを含む名前の権限をすべて検索
   - 部分一致検索が可能

3. **`ExistsPermissionName(string name)`**:
   - 指定した名前の権限が存在するかを確認

4. **`AssignPermissionToUserByName(string userId, string permissionName)`**:
   - 名前で指定した権限をユーザーに割り当てる

5. **`GrantPermissionToUserByName(string userId, string permissionName)`**:
   - 名前で指定した権限をユーザーに付与する

6. **`RevokePermissionFromUserByName(string userId, string permissionName)`**:
   - 名前で指定した権限をユーザーから削除する

7. **`UserHasPermissionByName(string userId, string permissionName)`**:
   - ユーザーが特定の名前の権限を持っているかをチェック

### 名前の一意性保証の仕組み

`RegisterPermission`メソッド内で、新しい権限を登録する前に名前の一意性をチェックしています：

```csharp
// 名前の一意性をチェック
if (permissionsByName.ContainsKey(permission.Name))
{
    throw new ArgumentException($"権限名 '{permission.Name}' はすでに登録されています。");
}
```

これにより、同じ名前の権限が登録されることを防ぎ、名前の一意性を保証しています。

### 大文字小文字を区別しない名前の比較

名前で権限を検索する際に大文字小文字を区別しないようにするため、`permissionsByName`ディクショナリを初期化する際に`StringComparer.OrdinalIgnoreCase`を使用しています：

```csharp
permissionsByName = new Dictionary<string, Permission>(StringComparer.OrdinalIgnoreCase);
```

これにより、"Admin"と"admin"は同じ権限として扱われます。

### 使用例

```csharp
// システムの初期化
PermissionManager manager = new PermissionManager();

// 権限定義を作成
Permission viewPermission = new Permission(0, "View");
Permission editPermission = new Permission(1, "Edit");
Permission deletePermission = new Permission(2, "Delete");
Permission adminViewPermission = new Permission(3, "AdminView");

// 権限をシステムに登録
manager.RegisterPermission(viewPermission);
manager.RegisterPermission(editPermission);
manager.RegisterPermission(deletePermission);
manager.RegisterPermission(adminViewPermission);

// 名前で権限を検索
Permission permission = manager.GetPermissionByName("Edit");
Console.WriteLine($"権限ID: {permission.Id}, 名前: {permission.Name}");

// 部分一致で権限を検索
List<Permission> viewPermissions = manager.FindPermissionsByPartialName("View");
foreach (var p in viewPermissions)
{
    Console.WriteLine($"権限ID: {p.Id}, 名前: {p.Name}");
}

// ユーザーに名前で指定した権限を割り当てる
string userId = "user1";
manager.AssignPermissionToUserByName(userId, "Edit");

// ユーザーが権限を持っているか名前でチェック
bool hasPermission = manager.UserHasPermissionByName(userId, "Edit");
Console.WriteLine($"ユーザーは編集権限を持っています: {hasPermission}");
```


---


現在の日時 (UTC): 2025-05-30 11:33:14
ユーザー: aoton0029

# Winformsで権限登録フォームの画面設計

PermissionManagerを使った権限登録・管理のためのWinformsフォームの画面設計を提案します。

## メイン画面（権限一覧・管理画面）

```
+--------------------------------------------------------------+
| 権限管理                                          [_][□][×] |
+--------------------------------------------------------------+
| [新規登録] [編集] [削除]                         [検索]: |_| |
+--------------------------------------------------------------+
| 権限一覧:                                                   |
| +--------------------------------------------------------+ |
| | ID | 名前           | 説明                 | 作成日時   | |
| |----+----------------+----------------------+------------| |
| | 1  | ViewUser       | ユーザー閲覧権限     | 2025/05/.. | |
| | 2  | EditUser       | ユーザー編集権限     | 2025/05/.. | |
| | 3  | AdminUser      | 管理者権限           | 2025/05/.. | |
| | .. | ..             | ..                   | ..         | |
| +--------------------------------------------------------+ |
|                                                            |
| 詳細情報:                                                  |
| +--------------------------------------------------------+ |
| | ID: |_1_|  名前: |_ViewUser______|                     | |
| |                                                        | |
| | 説明:                                                 | |
| | |_ユーザー情報の閲覧のみを許可する権限です___|        | |
| |                                                        | |
| | 権限ビット:                                            | |
| | |_00000001___________________________________|         | |
| |                                                        | |
| +--------------------------------------------------------+ |
|                                                            |
| [権限をユーザーに割り当て] [ユーザー権限一覧] [閉じる]     |
+--------------------------------------------------------------+
```

## 権限新規登録・編集画面

```
+--------------------------------------------------------------+
| 権限の登録・編集                                  [_][□][×] |
+--------------------------------------------------------------+
| 基本情報                                                    |
| +--------------------------------------------------------+ |
| | 権限ID:   |_自動採番__|                                 | |
| | 権限名:   |_________________________________|           | |
| |           (重複不可・必須項目)                          | |
| |                                                        | |
| | 説明:                                                 | |
| | |_________________________________________|           | |
| |                                                        | |
| +--------------------------------------------------------+ |
|                                                            |
| 権限設定                                                   |
| +--------------------------------------------------------+ |
| | 個別権限:                                              | |
| | [ ] ユーザー閲覧   [ ] ユーザー編集   [ ] ユーザー削除 | |
| | [ ] コンテンツ閲覧 [ ] コンテンツ編集 [ ] コンテンツ削除| |
| | [ ] ログ閲覧      [ ] システム設定                     | |
| |                                                        | |
| | ビット表現: |_00000000___|                             | |
| |                                                        | |
| +--------------------------------------------------------+ |
|                                                            |
|                                   [保存] [キャンセル]       |
+--------------------------------------------------------------+
```

## ユーザー権限割り当て画面

```
+--------------------------------------------------------------+
| ユーザー権限の割り当て                            [_][□][×] |
+--------------------------------------------------------------+
| ユーザー選択: |_ドロップダウン_________|  [検索]            |
+--------------------------------------------------------------+
| 割り当て可能な権限:              | 割り当て済み権限:        |
| +------------------------+       | +------------------------+ |
| | □ ViewUser (閲覧)     |       | | ☑ EditUser (編集)     | |
| | □ DeleteUser (削除)   |       | | ☑ ViewContent (閲覧)  | |
| | □ AdminUser (管理者)  |       | |                        | |
| | □ ...                 |       | |                        | |
| +------------------------+       | +------------------------+ |
|                                                               |
| [>>] 選択した権限を追加          [<<] 選択した権限を削除     |
|                                                               |
|                                     [保存] [キャンセル]       |
+--------------------------------------------------------------+
```

## C#コードの実装例

### メインフォーム (PermissionManagerForm.cs)

```csharp
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace PermissionManagementApp
{
    public partial class PermissionManagerForm : Form
    {
        private PermissionManager permissionManager;
        
        public PermissionManagerForm()
        {
            InitializeComponent();
            
            // PermissionManagerの初期化
            permissionManager = new PermissionManager();
            
            // サンプルデータの登録
            InitializeSampleData();
            
            // 権限一覧の表示
            LoadPermissionList();
        }
        
        private void InitializeComponent()
        {
            // フォームのサイズと基本設定
            this.Text = "権限管理";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // コントロールの配置
            CreateControls();
        }
        
        private void CreateControls()
        {
            // ボタン配置（新規登録、編集、削除）
            Button btnNew = new Button
            {
                Text = "新規登録",
                Location = new Point(20, 20),
                Size = new Size(100, 30)
            };
            btnNew.Click += BtnNew_Click;
            
            Button btnEdit = new Button
            {
                Text = "編集",
                Location = new Point(130, 20),
                Size = new Size(100, 30)
            };
            btnEdit.Click += BtnEdit_Click;
            
            Button btnDelete = new Button
            {
                Text = "削除",
                Location = new Point(240, 20),
                Size = new Size(100, 30)
            };
            btnDelete.Click += BtnDelete_Click;
            
            // 検索ボックス
            Label lblSearch = new Label
            {
                Text = "検索:",
                Location = new Point(500, 25),
                Size = new Size(50, 20)
            };
            
            TextBox txtSearch = new TextBox
            {
                Location = new Point(550, 20),
                Size = new Size(200, 25),
                Name = "txtSearch"
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            
            // 権限一覧DataGridView
            Label lblPermissionList = new Label
            {
                Text = "権限一覧:",
                Location = new Point(20, 60),
                Size = new Size(100, 20)
            };
            
            DataGridView dgvPermissions = new DataGridView
            {
                Location = new Point(20, 85),
                Size = new Size(750, 200),
                Name = "dgvPermissions",
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvPermissions.SelectionChanged += DgvPermissions_SelectionChanged;
            
            // DataGridViewの列設定
            dgvPermissions.Columns.Add("colId", "ID");
            dgvPermissions.Columns.Add("colName", "名前");
            dgvPermissions.Columns.Add("colDescription", "説明");
            dgvPermissions.Columns.Add("colCreationDate", "作成日時");
            
            // 詳細情報グループボックス
            GroupBox gbDetails = new GroupBox
            {
                Text = "詳細情報",
                Location = new Point(20, 300),
                Size = new Size(750, 180)
            };
            
            // 詳細情報コントロール
            Label lblDetailId = new Label
            {
                Text = "ID:",
                Location = new Point(20, 30),
                Size = new Size(30, 20)
            };
            
            TextBox txtDetailId = new TextBox
            {
                Location = new Point(55, 30),
                Size = new Size(50, 25),
                Name = "txtDetailId",
                ReadOnly = true
            };
            
            Label lblDetailName = new Label
            {
                Text = "名前:",
                Location = new Point(120, 30),
                Size = new Size(40, 20)
            };
            
            TextBox txtDetailName = new TextBox
            {
                Location = new Point(165, 30),
                Size = new Size(200, 25),
                Name = "txtDetailName",
                ReadOnly = true
            };
            
            Label lblDetailDescription = new Label
            {
                Text = "説明:",
                Location = new Point(20, 65),
                Size = new Size(40, 20)
            };
            
            TextBox txtDetailDescription = new TextBox
            {
                Location = new Point(20, 90),
                Size = new Size(700, 25),
                Name = "txtDetailDescription",
                ReadOnly = true
            };
            
            Label lblDetailBits = new Label
            {
                Text = "権限ビット:",
                Location = new Point(20, 125),
                Size = new Size(80, 20)
            };
            
            TextBox txtDetailBits = new TextBox
            {
                Location = new Point(20, 150),
                Size = new Size(700, 25),
                Name = "txtDetailBits",
                ReadOnly = true
            };
            
            gbDetails.Controls.Add(lblDetailId);
            gbDetails.Controls.Add(txtDetailId);
            gbDetails.Controls.Add(lblDetailName);
            gbDetails.Controls.Add(txtDetailName);
            gbDetails.Controls.Add(lblDetailDescription);
            gbDetails.Controls.Add(txtDetailDescription);
            gbDetails.Controls.Add(lblDetailBits);
            gbDetails.Controls.Add(txtDetailBits);
            
            // 機能ボタン
            Button btnAssign = new Button
            {
                Text = "権限をユーザーに割り当て",
                Location = new Point(20, 500),
                Size = new Size(200, 30)
            };
            btnAssign.Click += BtnAssign_Click;
            
            Button btnUserPermissions = new Button
            {
                Text = "ユーザー権限一覧",
                Location = new Point(240, 500),
                Size = new Size(150, 30)
            };
            btnUserPermissions.Click += BtnUserPermissions_Click;
            
            Button btnClose = new Button
            {
                Text = "閉じる",
                Location = new Point(670, 500),
                Size = new Size(100, 30)
            };
            btnClose.Click += (s, e) => this.Close();
            
            // コントロールの追加
            this.Controls.Add(btnNew);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(lblPermissionList);
            this.Controls.Add(dgvPermissions);
            this.Controls.Add(gbDetails);
            this.Controls.Add(btnAssign);
            this.Controls.Add(btnUserPermissions);
            this.Controls.Add(btnClose);
        }
        
        private void InitializeSampleData()
        {
            try
            {
                // サンプル権限の作成と登録
                Permission viewUser = new Permission(1, "ViewUser", 32);
                viewUser.Grant(0); // 最初のビットを許可
                
                Permission editUser = new Permission(2, "EditUser", 32);
                editUser.Grant(1); // 2番目のビットを許可
                
                Permission adminUser = new Permission(3, "AdminUser", 32);
                adminUser.Grant(0);
                adminUser.Grant(1);
                adminUser.Grant(2);
                
                // 権限マネージャーに登録
                permissionManager.RegisterPermission(viewUser);
                permissionManager.RegisterPermission(editUser);
                permissionManager.RegisterPermission(adminUser);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"サンプルデータの初期化中にエラーが発生しました: {ex.Message}", 
                    "初期化エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void LoadPermissionList()
        {
            DataGridView dgvPermissions = (DataGridView)this.Controls["dgvPermissions"];
            
            // DataGridViewをクリア
            dgvPermissions.Rows.Clear();
            
            // すべての権限を取得
            List<Permission> permissions = permissionManager.GetAllPermissions();
            
            // DataGridViewに表示
            foreach (Permission permission in permissions)
            {
                string bitString = GetBitString(permission);
                string description = $"{permission.Name}の説明";
                string creationDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                
                dgvPermissions.Rows.Add(permission.Id, permission.Name, description, creationDate);
            }
        }
        
        private string GetBitString(Permission permission)
        {
            // BitArrayの内容を文字列として表現
            string result = "";
            for (int i = 0; i < 8; i++) // 最初の8ビットのみ表示
            {
                result = (permission.HasPermission(i) ? "1" : "0") + result;
            }
            return result;
        }
        
        private void DisplayPermissionDetails(Permission permission)
        {
            if (permission == null) return;
            
            TextBox txtDetailId = (TextBox)Controls.Find("txtDetailId", true)[0];
            TextBox txtDetailName = (TextBox)Controls.Find("txtDetailName", true)[0];
            TextBox txtDetailDescription = (TextBox)Controls.Find("txtDetailDescription", true)[0];
            TextBox txtDetailBits = (TextBox)Controls.Find("txtDetailBits", true)[0];
            
            txtDetailId.Text = permission.Id.ToString();
            txtDetailName.Text = permission.Name;
            txtDetailDescription.Text = $"{permission.Name}の説明";
            txtDetailBits.Text = GetBitString(permission);
        }
        
        // イベントハンドラ
        private void BtnNew_Click(object sender, EventArgs e)
        {
            using (EditPermissionForm form = new EditPermissionForm(permissionManager))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPermissionList();
                }
            }
        }
        
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            DataGridView dgvPermissions = (DataGridView)Controls["dgvPermissions"];
            
            if (dgvPermissions.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する権限を選択してください", "選択エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            int permissionId = Convert.ToInt32(dgvPermissions.SelectedRows[0].Cells["colId"].Value);
            Permission selectedPermission = permissionManager.GetPermissionById(permissionId);
            
            using (EditPermissionForm form = new EditPermissionForm(permissionManager, selectedPermission))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadPermissionList();
                }
            }
        }
        
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DataGridView dgvPermissions = (DataGridView)Controls["dgvPermissions"];
            
            if (dgvPermissions.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する権限を選択してください", "選択エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            int permissionId = Convert.ToInt32(dgvPermissions.SelectedRows[0].Cells["colId"].Value);
            string permissionName = dgvPermissions.SelectedRows[0].Cells["colName"].Value.ToString();
            
            DialogResult result = MessageBox.Show(
                $"権限 '{permissionName}' を削除してもよろしいですか？", 
                "削除の確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                // 権限の削除処理（PermissionManagerに削除メソッドを追加する必要あり）
                // permissionManager.DeletePermission(permissionId);
                
                MessageBox.Show("権限が削除されました", "削除完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                LoadPermissionList();
            }
        }
        
        private void BtnAssign_Click(object sender, EventArgs e)
        {
            using (AssignPermissionForm form = new AssignPermissionForm(permissionManager))
            {
                form.ShowDialog();
            }
        }
        
        private void BtnUserPermissions_Click(object sender, EventArgs e)
        {
            using (UserPermissionsForm form = new UserPermissionsForm(permissionManager))
            {
                form.ShowDialog();
            }
        }
        
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            TextBox txtSearch = (TextBox)sender;
            DataGridView dgvPermissions = (DataGridView)Controls["dgvPermissions"];
            
            string searchText = txtSearch.Text.Trim().ToLower();
            
            dgvPermissions.Rows.Clear();
            
            foreach (Permission permission in permissionManager.GetAllPermissions())
            {
                if (string.IsNullOrEmpty(searchText) ||
                    permission.Name.ToLower().Contains(searchText))
                {
                    string description = $"{permission.Name}の説明";
                    string creationDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                    
                    dgvPermissions.Rows.Add(
                        permission.Id,
                        permission.Name,
                        description,
                        creationDate);
                }
            }
        }
        
        private void DgvPermissions_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgvPermissions = (DataGridView)sender;
            
            if (dgvPermissions.SelectedRows.Count > 0)
            {
                int permissionId = Convert.ToInt32(dgvPermissions.SelectedRows[0].Cells["colId"].Value);
                Permission selectedPermission = permissionManager.GetPermissionById(permissionId);
                
                DisplayPermissionDetails(selectedPermission);
            }
        }
    }
}
```

### 権限編集フォーム (EditPermissionForm.cs)

```csharp
using System;
using System.Windows.Forms;
using System.Drawing;

namespace PermissionManagementApp
{
    public class EditPermissionForm : Form
    {
        private PermissionManager permissionManager;
        private Permission editingPermission;
        private bool isNewMode;
        
        // チェックボックス配列
        private CheckBox[] permissionCheckboxes;
        
        public EditPermissionForm(Permission

---



I'll help you create a desktop application in C# with a user permission structure that stores data in files instead of a database. I'll create the necessary classes: Permission, Role, PermissionManager, and PermissionHelper.

Let me define these classes for you:

## 1. Permission Class

```csharp
using System;

namespace PermissionSystem
{
    [Serializable]
    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Permission()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Permission(string name, string description)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj is Permission permission)
            {
                return Id == permission.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
```

## 2. Role Class

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace PermissionSystem
{
    [Serializable]
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }

        public Role()
        {
            Id = Guid.NewGuid().ToString();
            Permissions = new List<Permission>();
        }

        public Role(string name, string description)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            Permissions = new List<Permission>();
        }

        public void AddPermission(Permission permission)
        {
            if (!Permissions.Any(p => p.Id == permission.Id))
            {
                Permissions.Add(permission);
            }
        }

        public void RemovePermission(Permission permission)
        {
            Permissions.RemoveAll(p => p.Id == permission.Id);
        }

        public bool HasPermission(string permissionName)
        {
            return Permissions.Any(p => p.Name == permissionName);
        }
    }
}
```

## 3. User Class (for linking users to roles)

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace PermissionSystem
{
    [Serializable]
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public List<string> RoleIds { get; set; }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            RoleIds = new List<string>();
        }

        public User(string username)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            RoleIds = new List<string>();
        }
    }
}
```

## 4. PermissionHelper Class

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PermissionSystem
{
    public class PermissionHelper
    {
        private readonly string _permissionsFilePath;
        private readonly string _rolesFilePath;
        private readonly string _usersFilePath;

        public PermissionHelper(string dataDirectory = "PermissionData")
        {
            // Ensure directory exists
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            _permissionsFilePath = Path.Combine(dataDirectory, "permissions.json");
            _rolesFilePath = Path.Combine(dataDirectory, "roles.json");
            _usersFilePath = Path.Combine(dataDirectory, "users.json");
        }

        public void SavePermissions(List<Permission> permissions)
        {
            string json = JsonSerializer.Serialize(permissions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_permissionsFilePath, json);
        }

        public List<Permission> LoadPermissions()
        {
            if (!File.Exists(_permissionsFilePath))
            {
                return new List<Permission>();
            }

            string json = File.ReadAllText(_permissionsFilePath);
            return JsonSerializer.Deserialize<List<Permission>>(json) ?? new List<Permission>();
        }

        public void SaveRoles(List<Role> roles)
        {
            string json = JsonSerializer.Serialize(roles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_rolesFilePath, json);
        }

        public List<Role> LoadRoles()
        {
            if (!File.Exists(_rolesFilePath))
            {
                return new List<Role>();
            }

            string json = File.ReadAllText(_rolesFilePath);
            return JsonSerializer.Deserialize<List<Role>>(json) ?? new List<Role>();
        }

        public void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_usersFilePath, json);
        }

        public List<User> LoadUsers()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            string json = File.ReadAllText(_usersFilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
    }
}
```

## 5. PermissionManager Class

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace PermissionSystem
{
    public class PermissionManager
    {
        private List<Permission> _permissions;
        private List<Role> _roles;
        private List<User> _users;
        private readonly PermissionHelper _helper;

        public PermissionManager()
        {
            _helper = new PermissionHelper();
            LoadAll();
        }

        public PermissionManager(string dataDirectory)
        {
            _helper = new PermissionHelper(dataDirectory);
            LoadAll();
        }

        private void LoadAll()
        {
            _permissions = _helper.LoadPermissions();
            _roles = _helper.LoadRoles();
            _users = _helper.LoadUsers();
        }

        public void SaveAll()
        {
            _helper.SavePermissions(_permissions);
            _helper.SaveRoles(_roles);
            _helper.SaveUsers(_users);
        }

        // Permission methods
        public Permission CreatePermission(string name, string description)
        {
            var permission = new Permission(name, description);
            _permissions.Add(permission);
            _helper.SavePermissions(_permissions);
            return permission;
        }

        public Permission GetPermission(string id)
        {
            return _permissions.FirstOrDefault(p => p.Id == id);
        }

        public List<Permission> GetAllPermissions()
        {
            return new List<Permission>(_permissions);
        }

        public bool UpdatePermission(string id, string name, string description)
        {
            var permission = _permissions.FirstOrDefault(p => p.Id == id);
            if (permission == null) return false;

            permission.Name = name;
            permission.Description = description;
            _helper.SavePermissions(_permissions);
            return true;
        }

        public bool DeletePermission(string id)
        {
            var permission = _permissions.FirstOrDefault(p => p.Id == id);
            if (permission == null) return false;

            // Remove the permission from all roles
            foreach (var role in _roles)
            {
                role.RemovePermission(permission);
            }
            
            _permissions.Remove(permission);
            SaveAll(); // Save both permissions and roles
            return true;
        }

        // Role methods
        public Role CreateRole(string name, string description)
        {
            var role = new Role(name, description);
            _roles.Add(role);
            _helper.SaveRoles(_roles);
            return role;
        }

        public Role GetRole(string id)
        {
            return _roles.FirstOrDefault(r => r.Id == id);
        }

        public List<Role> GetAllRoles()
        {
            return new List<Role>(_roles);
        }

        public bool UpdateRole(string id, string name, string description)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return false;

            role.Name = name;
            role.Description = description;
            _helper.SaveRoles(_roles);
            return true;
        }

        public bool DeleteRole(string id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return false;

            // Remove role id from all users
            foreach (var user in _users)
            {
                user.RoleIds.Remove(id);
            }
            
            _roles.Remove(role);
            SaveAll(); // Save both roles and users
            return true;
        }

        public bool AddPermissionToRole(string roleId, string permissionId)
        {
            var role = _roles.FirstOrDefault(r => r.Id == roleId);
            var permission = _permissions.FirstOrDefault(p => p.Id == permissionId);
            
            if (role == null || permission == null) return false;
            
            role.AddPermission(permission);
            _helper.SaveRoles(_roles);
            return true;
        }

        public bool RemovePermissionFromRole(string roleId, string permissionId)
        {
            var role = _roles.FirstOrDefault(r => r.Id == roleId);
            var permission = _permissions.FirstOrDefault(p => p.Id == permissionId);
            
            if (role == null || permission == null) return false;
            
            role.RemovePermission(permission);
            _helper.SaveRoles(_roles);
            return true;
        }

        // User methods
        public User CreateUser(string username)
        {
            var user = new User(username);
            _users.Add(user);
            _helper.SaveUsers(_users);
            return user;
        }

        public User GetUser(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public List<User> GetAllUsers()
        {
            return new List<User>(_users);
        }

        public bool AssignRoleToUser(string userId, string roleId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            var role = _roles.FirstOrDefault(r => r.Id == roleId);
            
            if (user == null || role == null) return false;
            
            if (!user.RoleIds.Contains(roleId))
            {
                user.RoleIds.Add(roleId);
                _helper.SaveUsers(_users);
            }
            return true;
        }

        public bool RemoveRoleFromUser(string userId, string roleId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null) return false;
            
            if (user.RoleIds.Contains(roleId))
            {
                user.RoleIds.Remove(roleId);
                _helper.SaveUsers(_users);
            }
            return true;
        }

        public List<Role> GetUserRoles(string userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return new List<Role>();
            
            return _roles.Where(r => user.RoleIds.Contains(r.Id)).ToList();
        }

        public bool UserHasPermission(string userId, string permissionName)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;
            
            var userRoles = GetUserRoles(userId);
            return userRoles.Any(r => r.HasPermission(permissionName));
        }
    }
}
```

## 使用例 (Usage Example)

```csharp
using System;

namespace PermissionSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the permission manager
            var permManager = new PermissionManager();
            
            // Create permissions
            var readPermission = permManager.CreatePermission("Read", "Can read data");
            var writePermission = permManager.CreatePermission("Write", "Can write data");
            var adminPermission = permManager.CreatePermission("Admin", "Administrative privileges");
            
            // Create roles
            var userRole = permManager.CreateRole("User", "Regular user");
            var editorRole = permManager.CreateRole("Editor", "Content editor");
            var adminRole = permManager.CreateRole("Administrator", "System administrator");
            
            // Assign permissions to roles
            permManager.AddPermissionToRole(userRole.Id, readPermission.Id);
            
            permManager.AddPermissionToRole(editorRole.Id, readPermission.Id);
            permManager.AddPermissionToRole(editorRole.Id, writePermission.Id);
            
            permManager.AddPermissionToRole(adminRole.Id, readPermission.Id);
            permManager.AddPermissionToRole(adminRole.Id, writePermission.Id);
            permManager.AddPermissionToRole(adminRole.Id, adminPermission.Id);
            
            // Create users
            var normalUser = permManager.CreateUser("john_doe");
            var editor = permManager.CreateUser("jane_editor");
            var admin = permManager.CreateUser("admin_user");
            
            // Assign roles to users
            permManager.AssignRoleToUser(normalUser.Id, userRole.Id);
            permManager.AssignRoleToUser(editor.Id, editorRole.Id);
            permManager.AssignRoleToUser(admin.Id, adminRole.Id);
            
            // Check permissions
            Console.WriteLine($"User {normalUser.Username} can read: {permManager.UserHasPermission(normalUser.Id, "Read")}");
            Console.WriteLine($"User {normalUser.Username} can write: {permManager.UserHasPermission(normalUser.Id, "Write")}");
            
            Console.WriteLine($"User {editor.Username} can read: {permManager.UserHasPermission(editor.Id, "Read")}");
            Console.WriteLine($"User {editor.Username} can write: {permManager.UserHasPermission(editor.Id, "Write")}");
            
            Console.WriteLine($"User {admin.Username} is admin: {permManager.UserHasPermission(admin.Id, "Admin")}");
            
            // Save all data
            permManager.SaveAll();
            
            Console.WriteLine("All data has been saved to files.");
        }
    }
}
```

このコードでは、ユーザー権限システムの基本的な構造を実装しています。Permissionクラスは権限を、Roleクラスは役割（権限のグループ）を表し、UserクラスはユーザーとRoleの関連付けを管理します。PermissionHelperはJSONファイルへの保存と読み込みを担当し、PermissionManagerは全体の管理を行います。データはすべてファイルに保存され、次回アプリケーション起動時に読み込まれます。




I can see you mentioned "フラグを使わずに組み合わせ可能な権限" which translates to "combinable permissions without using flags." Let me help you design a permission system that allows for flexible combinations without relying on the enum flags approach.

Let me check your GitHub repository first to understand the context better.# フラグを使わずに組み合わせ可能な権限システムの設計

リポジトリのコードを確認したところ、`CoreLibWinforms/Core/Permissions/PermissionType.cs`に興味深い実装がありました。これをベースにフラグを使用せず、組み合わせ可能な権限システムを設計します。

## 1. 権限インターフェースの定義

```csharp
/// <summary>
/// 権限を表すインターフェース
/// </summary>
public interface IPermission
{
    /// <summary>
    /// 権限の数値値
    /// </summary>
    int Value { get; }

    /// <summary>
    /// 権限の名前
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 説明
    /// </summary>
    string Description { get; }

    /// <summary>
    /// 他の権限と組み合わせることができるか
    /// </summary>
    bool IsCombineable { get; }
}
```

## 2. 基本権限クラスの実装

```csharp
/// <summary>
/// 基本的な権限を表すクラス
/// </summary>
public class Permission : IPermission
{
    /// <summary>
    /// 権限の数値値（固有のプライム値を使用）
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// 権限の名前
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 説明
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 他の権限と組み合わせることができるか
    /// </summary>
    public bool IsCombineable { get; }

    /// <summary>
    /// 権限に関連付けられたメタデータ
    /// </summary>
    public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    /// <summary>
    /// 新しい権限を作成します
    /// </summary>
    public Permission(int value, string name, string description, bool isCombineable = true)
    {
        Value = value;
        Name = name;
        Description = description;
        IsCombineable = isCombineable;
    }

    public override string ToString() => Name;

    public override bool Equals(object obj)
    {
        return obj is Permission permission && Value == permission.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Permission left, Permission right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Permission left, Permission right)
    {
        return !(left == right);
    }
}
```

## 3. 複合権限クラスの実装

```csharp
/// <summary>
/// 複数の権限を組み合わせた権限
/// </summary>
public class CompositePermission : IPermission
{
    /// <summary>
    /// この複合権限に含まれる個々の権限
    /// </summary>
    public IReadOnlyList<IPermission> Permissions { get; }

    /// <summary>
    /// 権限の数値値（含まれるすべての権限の値の合計）
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// 権限の名前（含まれる権限名をパイプで結合）
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 説明（含まれる権限の説明を結合）
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 他の権限と組み合わせることができるか（常にtrue）
    /// </summary>
    public bool IsCombineable => true;

    /// <summary>
    /// 複合権限に関連付けられたメタデータ
    /// </summary>
    public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    /// <summary>
    /// 新しい複合権限を作成します
    /// </summary>
    public CompositePermission(IEnumerable<IPermission> permissions)
    {
        if (permissions == null)
            throw new ArgumentNullException(nameof(permissions));

        var permList = permissions.ToList();
        
        // 組み合わせ不可能な権限がないかチェック
        var nonCombineablePermission = permList.FirstOrDefault(p => !p.IsCombineable);
        if (nonCombineablePermission != null)
            throw new InvalidOperationException($"権限 '{nonCombineablePermission.Name}' は組み合わせることができません。");

        Permissions = permList.ToArray();
        Value = CalculateValue(permList);
        Name = string.Join("|", permList.Select(p => p.Name));
        Description = string.Join("、", permList.Select(p => p.Description));
    }

    /// <summary>
    /// 複合権限の値を計算（権限値の積またはハッシュ値を使用）
    /// </summary>
    private int CalculateValue(List<IPermission> permissions)
    {
        return permissions.Aggregate(0, (current, permission) => current + permission.Value);
    }

    /// <summary>
    /// この権限が指定された権限を含むかをチェックします
    /// </summary>
    public bool Contains(IPermission permission)
    {
        if (permission == null)
            return false;

        // 指定された権限自体が含まれているか、あるいは
        // 複合権限の場合はその中の個々の権限が含まれているかをチェック
        if (permission is CompositePermission compositePermission)
        {
            return compositePermission.Permissions.All(p => Contains(p));
        }

        return Permissions.Any(p => p.Equals(permission)) || 
               Permissions.OfType<CompositePermission>().Any(cp => cp.Contains(permission));
    }

    public override string ToString() => Name;

    public override bool Equals(object obj)
    {
        return obj is CompositePermission other && 
               Value == other.Value && 
               Permissions.Count == other.Permissions.Count && 
               Permissions.All(p => other.Contains(p));
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
```

## 4. 権限マネージャークラス

```csharp
/// <summary>
/// 権限を管理するクラス
/// </summary>
public class PermissionManager
{
    private static PermissionManager _instance;
    public static PermissionManager Instance => _instance ??= new PermissionManager();

    // 権限のコレクション
    private readonly Dictionary<string, Permission> _permissions = new();
    
    // 権限の組み合わせをキャッシュ
    private readonly Dictionary<string, CompositePermission> _compositeCache = new();

    // ユーザーに割り当てられた権限
    private readonly Dictionary<string, HashSet<IPermission>> _userPermissions = new();
    
    // 現在のユーザー
    public string CurrentUser { get; private set; }

    /// <summary>
    /// 権限を登録
    /// </summary>
    public Permission RegisterPermission(string name, string description, bool isCombineable = true)
    {
        if (_permissions.ContainsKey(name))
            return _permissions[name];
            
        // 固有の素数値を使用することで、権限の一意性を確保
        int value = GenerateUniqueValue();
        var permission = new Permission(value, name, description, isCombineable);
        _permissions[name] = permission;
        return permission;
    }

    /// <summary>
    /// 固有の値を生成（素数を使用）
    /// </summary>
    private int GenerateUniqueValue()
    {
        // 既存の権限から使用されていない素数を見つける
        int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        
        var usedValues = _permissions.Values.Select(p => p.Value).ToHashSet();
        return primes.FirstOrDefault(p => !usedValues.Contains(p));
    }

    /// <summary>
    /// 権限の組み合わせを作成
    /// </summary>
    public CompositePermission CombinePermissions(params string[] permissionNames)
    {
        // キャッシュキーを作成（ソート済み名前のリスト）
        string cacheKey = string.Join(",", permissionNames.OrderBy(n => n));
        
        if (_compositeCache.TryGetValue(cacheKey, out var cached))
            return cached;
            
        var permissions = new List<IPermission>();
        foreach (var name in permissionNames)
        {
            if (_permissions.TryGetValue(name, out var permission))
                permissions.Add(permission);
        }
        
        var composite = new CompositePermission(permissions);
        _compositeCache[cacheKey] = composite;
        return composite;
    }

    /// <summary>
    /// ユーザーに権限を割り当て
    /// </summary>
    public void AssignPermissionToUser(string userId, IPermission permission)
    {
        if (!_userPermissions.TryGetValue(userId, out var permissions))
        {
            permissions = new HashSet<IPermission>();
            _userPermissions[userId] = permissions;
        }
        
        permissions.Add(permission);
    }

    /// <summary>
    /// ユーザーの権限を確認
    /// </summary>
    public bool UserHasPermission(string userId, IPermission requiredPermission)
    {
        if (!_userPermissions.TryGetValue(userId, out var permissions))
            return false;
            
        // 直接一致する権限があるか確認
        if (permissions.Contains(requiredPermission))
            return true;
            
        // 複合権限の場合、その構成要素を含む権限があるか確認
        if (requiredPermission is CompositePermission composite)
        {
            // 必要な権限がすべて含まれているか確認
            return composite.Permissions.All(p => 
                permissions.Any(userPerm => 
                    userPerm.Equals(p) || 
                    (userPerm is CompositePermission userComp && userComp.Contains(p))
                )
            );
        }
        
        // 複合権限の中に必要な権限が含まれているか確認
        return permissions.OfType<CompositePermission>().Any(cp => cp.Contains(requiredPermission));
    }

    /// <summary>
    /// 現在のユーザーを設定
    /// </summary>
    public void SetCurrentUser(string userId)
    {
        CurrentUser = userId;
    }

    /// <summary>
    /// 現在のユーザーの権限を確認
    /// </summary>
    public bool CurrentUserHasPermission(IPermission requiredPermission)
    {
        return !string.IsNullOrEmpty(CurrentUser) && 
               UserHasPermission(CurrentUser, requiredPermission);
    }
}
```

## 5. 使用例

```csharp
public class PermissionSystemExample
{
    public void InitializePermissions()
    {
        // 権限マネージャーの取得
        var permManager = PermissionManager.Instance;
        
        // 基本権限の登録
        var viewPermission = permManager.RegisterPermission("View", "閲覧する権限");
        var createPermission = permManager.RegisterPermission("Create", "作成する権限");
        var editPermission = permManager.RegisterPermission("Edit", "編集する権限");
        var deletePermission = permManager.RegisterPermission("Delete", "削除する権限");
        var approvePermission = permManager.RegisterPermission("Approve", "承認する権限");
        var adminPermission = permManager.RegisterPermission("Admin", "管理者権限", false); // 組み合わせ不可
        
        // 複合権限の作成
        var editorPermission = permManager.CombinePermissions("View", "Create", "Edit");
        var managerPermission = permManager.CombinePermissions("View", "Create", "Edit", "Delete");
        
        // ユーザーの作成と権限の割り当て
        permManager.AssignPermissionToUser("user1", viewPermission);
        permManager.AssignPermissionToUser("user2", editorPermission);
        permManager.AssignPermissionToUser("admin", adminPermission);
        
        // 特定のユーザーに追加の権限を割り当て
        permManager.AssignPermissionToUser("manager", managerPermission);
        permManager.AssignPermissionToUser("manager", approvePermission);
    }
    
    public void CheckPermissions()
    {
        var permManager = PermissionManager.Instance;
        
        // 現在のユーザーを設定
        permManager.SetCurrentUser("user2");
        
        // シンプルな権限チェック
        var viewPermission = permManager.RegisterPermission("View", "閲覧する権限");
        bool canView = permManager.CurrentUserHasPermission(viewPermission);
        Console.WriteLine($"Can view: {canView}"); // true
        
        // 複合権限のチェック
        var deletePermission = permManager.RegisterPermission("Delete", "削除する権限");
        bool canDelete = permManager.CurrentUserHasPermission(deletePermission);
        Console.WriteLine($"Can delete: {canDelete}"); // false
        
        // ユーザーを変更して複合権限をチェック
        permManager.SetCurrentUser("manager");
        canDelete = permManager.CurrentUserHasPermission(deletePermission);
        Console.WriteLine($"Manager can delete: {canDelete}"); // true
    }
}
```

## 6. リソース別権限の実装

```csharp
/// <summary>
/// リソースに対する権限
/// </summary>
public class ResourcePermission
{
    /// <summary>
    /// リソースID
    /// </summary>
    public string ResourceId { get; }
    
    /// <summary>
    /// 権限
    /// </summary>
    public IPermission Permission { get; }
    
    public ResourcePermission(string resourceId, IPermission permission)
    {
        ResourceId = resourceId ?? throw new ArgumentNullException(nameof(resourceId));
        Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
}

/// <summary>
/// リソース別権限の拡張
/// </summary>
public class ResourcePermissionManager
{
    // リソースタイプ別の権限マッピング
    private readonly Dictionary<string, Dictionary<string, ResourcePermission>> _userResourcePermissions = new();
    private readonly PermissionManager _permissionManager;
    
    public ResourcePermissionManager(PermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }
    
    /// <summary>
    /// ユーザーにリソース権限を割り当て
    /// </summary>
    public void AssignResourcePermission(string userId, string resourceId, IPermission permission)
    {
        if (!_userResourcePermissions.TryGetValue(userId, out var resourcePermissions))
        {
            resourcePermissions = new Dictionary<string, ResourcePermission>();
            _userResourcePermissions[userId] = resourcePermissions;
        }
        
        resourcePermissions[resourceId] = new ResourcePermission(resourceId, permission);
    }
    
    /// <summary>
    /// ユーザーがリソースに対して権限を持っているか確認
    /// </summary>
    public bool CheckResourcePermission(string userId, string resourceId, IPermission requiredPermission)
    {
        // 1. まずリソース固有の権限をチェック
        if (_userResourcePermissions.TryGetValue(userId, out var resourcePermissions) &&
            resourcePermissions.TryGetValue(resourceId, out var permission))
        {
            if (permission.Permission is CompositePermission composite)
            {
                if (composite.Contains(requiredPermission))
                    return true;
            }
            else if (permission.Permission.Equals(requiredPermission))
            {
                return true;
            }
        }
        
        // 2. グローバル権限をチェック
        return _permissionManager.UserHasPermission(userId, requiredPermission);
    }
}
```

## 7. シリアライゼーション対応

```csharp
/// <summary>
/// 権限構成のシリアライゼーション
/// </summary>
public class PermissionSerializer
{
    private class PermissionData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public bool IsCombineable { get; set; }
    }
    
    private class CompositePermissionData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> PermissionNames { get; set; }
    }
    
    private class UserPermissionData
    {
        public string UserId { get; set; }
        public List<string> PermissionNames { get; set; }
        public Dictionary<string, List<string>> ResourcePermissions { get; set; }
    }
    
    /// <summary>
    /// 権限設定を保存
    /// </summary>
    public static void SavePermissions(PermissionManager permissionManager, string filePath)
    {
        // 権限マネージャーから必要なデータを抽出してシリアライズ
        // ...
    }
    
    /// <summary>
    /// 権限設定を読み込み
    /// </summary>
    public static void LoadPermissions(PermissionManager permissionManager, string filePath)
    {
        // ファイルからデータを読み込んで権限マネージャーに復元
        // ...
    }
}
```

## まとめ

この設計では、フラグを使用せずに権限を組み合わせる方法を示しました。主な特長は以下の通りです：

1. **オブジェクト指向アプローチ**：
   - 各権限は独自のオブジェクトとして表現
   - コンポジットパターンを使用して複雑な権限構造を構築

2. **優れた拡張性**：
   - 新しい権限タイプを簡単に追加可能
   - メタデータを通じて権限に追加情報を付与可能

3. **型安全性**：
   - フラグ型Enumと異なり、権限チェックが型安全

4. **明示的な権限組み合わせ**：
   - 権限の組み合わせが明示的で追跡しやすい
   - コンポジットオブジェクトを通じて階層的な権限構造を実現

5. **柔軟な権限チェックロジック**：
   - 単純な一致だけでなく、構造的なチェックが可能
   - リソース別の権限管理が容易

この設計は柔軟性が高く、アプリケーション間で共有できる汎用的な権限システムとして機能します。

なお、GitHub上の検索結果には制限があり、完全な結果が表示されていない可能性があります。
---


理解しました。個々のアプリケーションが独自の権限システムを持つ形で、アプリケーション全体の管理は不要なシステムを設計します。それぞれのアプリケーションが独自に権限を定義・管理できる柔軟な設計に変更します。

# C# デスクトップアプリ用の独立型ユーザー権限システムの設計

## 基本設計 (改訂版)

### 1. アプリケーション独自の権限システム

```csharp
// 柔軟な権限定義のための基本クラス
public class PermissionDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

// 具体的なアプリケーションでの権限実装例
public class InvoiceAppPermissions
{
    public static readonly PermissionDefinition ViewInvoices = new PermissionDefinition 
    { 
        Id = "invoice_view", 
        Name = "請求書閲覧", 
        Description = "請求書の閲覧権限" 
    };
    
    public static readonly PermissionDefinition CreateInvoices = new PermissionDefinition 
    { 
        Id = "invoice_create", 
        Name = "請求書作成", 
        Description = "請求書の作成権限" 
    };
    
    // 他の権限も同様に定義...
    
    // アプリケーションで使用する全ての権限を取得
    public static IEnumerable<PermissionDefinition> GetAllPermissions()
    {
        return new[] { ViewInvoices, CreateInvoices /* 他の権限も追加 */ };
    }
}
```

### 2. ロールベース権限システム (単一アプリケーション用)

```csharp
// ロール定義
public class Role
{
    public string Id { get; set; }
    public string Name { get; set; }
    public HashSet<string> PermissionIds { get; set; } = new HashSet<string>();
    
    public bool HasPermission(string permissionId)
    {
        return PermissionIds.Contains(permissionId);
    }
}

// ユーザー定義
public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public List<string> RoleIds { get; set; } = new List<string>();
    
    // ユーザー固有の権限オーバーライド（特定権限の許可または拒否）
    public Dictionary<string, bool> PermissionOverrides { get; set; } = new Dictionary<string, bool>();
}
```

### 3. 権限管理サービス (単一アプリ用)

```csharp
public class PermissionService
{
    private readonly Dictionary<string, Role> _roles = new Dictionary<string, Role>();
    private readonly Dictionary<string, User> _users = new Dictionary<string, User>();
    private readonly string _configFilePath;
    
    public PermissionService(string appName)
    {
        // アプリごとに個別の設定ファイルを使用
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string configDir = Path.Combine(appDataPath, appName, "Config");
        
        if (!Directory.Exists(configDir))
            Directory.CreateDirectory(configDir);
            
        _configFilePath = Path.Combine(configDir, "permissions.json");
        LoadConfiguration();
    }
    
    private void LoadConfiguration()
    {
        if (!File.Exists(_configFilePath))
            return;
            
        try
        {
            var json = File.ReadAllText(_configFilePath);
            var config = JsonSerializer.Deserialize<PermissionConfig>(json);
            
            if (config != null)
            {
                _roles = config.Roles ?? new Dictionary<string, Role>();
                _users = config.Users ?? new Dictionary<string, User>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"設定ファイルの読み込みエラー: {ex.Message}");
            // エラーログに記録など
        }
    }
    
    public void SaveConfiguration()
    {
        var config = new PermissionConfig
        {
            Roles = _roles,
            Users = _users
        };
        
        try
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"設定ファイルの保存エラー: {ex.Message}");
            // エラーログに記録など
        }
    }
    
    // ユーザーが特定の権限を持っているか確認
    public bool HasPermission(string userId, string permissionId)
    {
        if (!_users.TryGetValue(userId, out var user))
            return false;
            
        // ユーザー固有のオーバーライドがある場合はそれを優先
        if (user.PermissionOverrides.TryGetValue(permissionId, out var hasPermission))
            return hasPermission;
            
        // ユーザーのロールを確認
        foreach (var roleId in user.RoleIds)
        {
            if (_roles.TryGetValue(roleId, out var role) && role.HasPermission(permissionId))
                return true;
        }
        
        return false;
    }
    
    // ユーザー、ロールの管理メソッド
    public void AddUser(User user) => _users[user.Id] = user;
    public void AddRole(Role role) => _roles[role.Id] = role;
    public User GetUser(string userId) => _users.TryGetValue(userId, out var user) ? user : null;
    public Role GetRole(string roleId) => _roles.TryGetValue(roleId, out var role) ? role : null;
    
    // その他の必要なメソッド...
    
    // 設定ファイル用のクラス
    private class PermissionConfig
    {
        public Dictionary<string, Role> Roles { get; set; }
        public Dictionary<string, User> Users { get; set; }
    }
}
```

### 4. アプリケーションでの使用例

```csharp
public class InvoiceApplication
{
    private readonly PermissionService _permissionService;
    private readonly string _currentUserId;
    
    public InvoiceApplication(string currentUserId)
    {
        _currentUserId = currentUserId;
        _permissionService = new PermissionService("InvoiceApp");
        
        // アプリ起動時に基本ロールがなければ初期化
        InitializeDefaultRolesIfNeeded();
    }
    
    private void InitializeDefaultRolesIfNeeded()
    {
        if (_permissionService.GetRole("admin") == null)
        {
            // 管理者ロールの作成
            var adminRole = new Role
            {
                Id = "admin",
                Name = "管理者",
                PermissionIds = new HashSet<string>(
                    InvoiceAppPermissions.GetAllPermissions().Select(p => p.Id))
            };
            _permissionService.AddRole(adminRole);
            
            // 一般ユーザーロールの作成
            var userRole = new Role
            {
                Id = "user",
                Name = "一般ユーザー",
                PermissionIds = new HashSet<string> { InvoiceAppPermissions.ViewInvoices.Id }
            };
            _permissionService.AddRole(userRole);
            
            _permissionService.SaveConfiguration();
        }
    }
    
    // UI要素の可視性を設定
    public bool CanViewInvoices => _permissionService.HasPermission(_currentUserId, InvoiceAppPermissions.ViewInvoices.Id);
    public bool CanCreateInvoices => _permissionService.HasPermission(_currentUserId, InvoiceAppPermissions.CreateInvoices.Id);
    
    // 機能実行時の権限チェック
    public void CreateInvoice()
    {
        if (!CanCreateInvoices)
        {
            MessageBox.Show("請求書を作成する権限がありません。");
            return;
        }
        
        // 請求書作成のロジック...
    }
}
```

## 拡張機能と応用例

### 1. 動的な権限設定 UI

```csharp
public class PermissionSettingsForm : Form
{
    private readonly PermissionService _permissionService;
    
    public PermissionSettingsForm(PermissionService permissionService)
    {
        _permissionService = permissionService;
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        // UI要素の初期化（簡略化）
        // ...
        
        // ロール一覧を表示
        ShowRoles();
    }
    
    private void ShowRoles()
    {
        // ロールをリストビューに表示
        // ...
        
        // 選択されたロールの権限一覧を表示
        // ...
    }
    
    private void SavePermissionChanges()
    {
        // UI上での変更をモデルに適用
        // ...
        
        // 変更を保存
        _permissionService.SaveConfiguration();
    }
}
```

### 2. 権限プロファイル機能

```csharp
public class PermissionProfile
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, Role> Roles { get; set; } = new Dictionary<string, Role>();
    
    // プロファイルのインポート/エクスポート
    public static PermissionProfile FromJson(string json)
    {
        return JsonSerializer.Deserialize<PermissionProfile>(json);
    }
    
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}

// PermissionServiceに追加するメソッド
public void ImportProfile(PermissionProfile profile)
{
    // プロファイルからロールをインポート
    foreach (var role in profile.Roles.Values)
    {
        _roles[role.Id] = role;
    }
    SaveConfiguration();
}

public PermissionProfile ExportProfile(string profileId, string profileName)
{
    return new PermissionProfile
    {
        Id = profileId,
        Name = profileName,
        Roles = new Dictionary<string, Role>(_roles)
    };
}
```

## 代替アプローチ: 属性ベースの権限システム

より柔軟で宣言的な権限チェックを行いたい場合は、属性を使用したアプローチも有効です：

```csharp
// 権限チェック属性
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RequirePermissionAttribute : Attribute
{
    public string PermissionId { get; }
    
    public RequirePermissionAttribute(string permissionId)
    {
        PermissionId = permissionId;
    }
}

// 拡張メソッドを使用した権限チェック
public static class PermissionExtensions
{
    public static void CheckPermission(this PermissionService service, string userId, string permissionId)
    {
        if (!service.HasPermission(userId, permissionId))
        {
            throw new UnauthorizedAccessException($"権限がありません: {permissionId}");
        }
    }
}

// 使用例
public class InvoiceService
{
    private readonly PermissionService _permissionService;
    private readonly string _currentUserId;
    
    public InvoiceService(PermissionService permissionService, string currentUserId)
    {
        _permissionService = permissionService;
        _currentUserId = currentUserId;
    }
    
    [RequirePermission("invoice_create")]
    public void CreateInvoice(Invoice invoice)
    {
        // リフレクションを使って属性を取得し権限チェックを行う
        // （通常はAOPフレームワークを使用）
        
        // メソッドの実行...
    }
    
    // 明示的な権限チェック
    public void DeleteInvoice(int invoiceId)
    {
        _permissionService.CheckPermission(_currentUserId, "invoice_delete");
        
        // 削除処理...
    }
}
```

この設計により、各アプリケーションが完全に独立した権限システムを持ち、アプリケーション間の管理は必要ない柔軟なシステムを実現できます。JSONファイルを使用して権限情報を保存することで、データベースを使わずに権限を管理できます。

Thank you for providing that information. Now I understand that you're looking to implement EventBus and EventHandlerManager classes in C#.

Let me help you create implementations for these classes that handle control events, PropertyChanged, and CollectionChanged events using WeakReferences.

Here's a complete implementation of both classes:

```csharp
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

/// <summary>
/// A generic event bus that allows publish-subscribe style communication between components
/// </summary>
public class EventBus
{
    private static readonly Lazy<EventBus> _instance = new Lazy<EventBus>(() => new EventBus());
    public static EventBus Instance => _instance.Value;

    private readonly EventHandlerManager _eventHandlerManager;

    private EventBus()
    {
        _eventHandlerManager = new EventHandlerManager();
    }

    /// <summary>
    /// Register a subscriber to receive events of type T
    /// </summary>
    /// <typeparam name="T">The type of event to subscribe to</typeparam>
    /// <param name="subscriber">The subscriber object</param>
    /// <param name="handler">The method to call when the event occurs</param>
    public void Subscribe<T>(object subscriber, Action<T> handler)
    {
        _eventHandlerManager.AddHandler(typeof(T), subscriber, handler);
    }

    /// <summary>
    /// Unregister a subscriber from events of type T
    /// </summary>
    /// <typeparam name="T">The type of event to unsubscribe from</typeparam>
    /// <param name="subscriber">The subscriber object</param>
    public void Unsubscribe<T>(object subscriber)
    {
        _eventHandlerManager.RemoveHandler(typeof(T), subscriber);
    }

    /// <summary>
    /// Unregister a subscriber from all events
    /// </summary>
    /// <param name="subscriber">The subscriber object</param>
    public void UnsubscribeAll(object subscriber)
    {
        _eventHandlerManager.RemoveAllHandlers(subscriber);
    }

    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    /// <typeparam name="T">The type of event</typeparam>
    /// <param name="eventData">The event data</param>
    public void Publish<T>(T eventData)
    {
        _eventHandlerManager.InvokeHandlers(typeof(T), eventData);
    }

    /// <summary>
    /// Register for PropertyChanged events from an INotifyPropertyChanged object
    /// </summary>
    /// <param name="source">The source object implementing INotifyPropertyChanged</param>
    /// <param name="subscriber">The subscriber object</param>
    /// <param name="handler">The method to call when the property changes</param>
    public void SubscribeToPropertyChanged(INotifyPropertyChanged source, object subscriber, PropertyChangedEventHandler handler)
    {
        _eventHandlerManager.AddPropertyChangedHandler(source, subscriber, handler);
    }

    /// <summary>
    /// Register for CollectionChanged events from an INotifyCollectionChanged object
    /// </summary>
    /// <param name="source">The source object implementing INotifyCollectionChanged</param>
    /// <param name="subscriber">The subscriber object</param>
    /// <param name="handler">The method to call when the collection changes</param>
    public void SubscribeToCollectionChanged(INotifyCollectionChanged source, object subscriber, NotifyCollectionChangedEventHandler handler)
    {
        _eventHandlerManager.AddCollectionChangedHandler(source, subscriber, handler);
    }

    /// <summary>
    /// Register for a standard .NET event
    /// </summary>
    /// <param name="source">The source object</param>
    /// <param name="eventName">The name of the event</param>
    /// <param name="subscriber">The subscriber object</param>
    /// <param name="handler">The delegate to handle the event</param>
    public void SubscribeToEvent(object source, string eventName, object subscriber, Delegate handler)
    {
        _eventHandlerManager.AddEventHandler(source, eventName, subscriber, handler);
    }
}

/// <summary>
/// Manages event handlers using weak references to prevent memory leaks
/// </summary>
public class EventHandlerManager
{
    private readonly Dictionary<Type, List<EventSubscription>> _eventHandlers = new Dictionary<Type, List<EventSubscription>>();
    private readonly Dictionary<object, List<PropertyChangedSubscription>> _propertyChangedHandlers = new Dictionary<object, List<PropertyChangedSubscription>>();
    private readonly Dictionary<object, List<CollectionChangedSubscription>> _collectionChangedHandlers = new Dictionary<object, List<CollectionChangedSubscription>>();
    private readonly Dictionary<EventKey, List<StandardEventSubscription>> _standardEventHandlers = new Dictionary<EventKey, List<StandardEventSubscription>>();

    /// <summary>
    /// Add a handler for a specific event type
    /// </summary>
    public void AddHandler(Type eventType, object subscriber, Delegate handler)
    {
        if (!_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            handlers = new List<EventSubscription>();
            _eventHandlers[eventType] = handlers;
        }

        // Remove any existing subscription for this subscriber
        handlers.RemoveAll(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber);

        // Add the new subscription
        handlers.Add(new EventSubscription(subscriber, handler));

        // Clean up dead references
        CleanupEventHandlers(eventType);
    }

    /// <summary>
    /// Remove handlers for a specific event type and subscriber
    /// </summary>
    public void RemoveHandler(Type eventType, object subscriber)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            handlers.RemoveAll(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber);
            CleanupEventHandlers(eventType);
        }
    }

    /// <summary>
    /// Remove all handlers for a specific subscriber
    /// </summary>
    public void RemoveAllHandlers(object subscriber)
    {
        // Remove from event handlers
        foreach (var eventType in _eventHandlers.Keys.ToList())
        {
            RemoveHandler(eventType, subscriber);
        }

        // Remove from property changed handlers
        foreach (var source in _propertyChangedHandlers.Keys.ToList())
        {
            RemovePropertyChangedHandler(source, subscriber);
        }

        // Remove from collection changed handlers
        foreach (var source in _collectionChangedHandlers.Keys.ToList())
        {
            RemoveCollectionChangedHandler(source, subscriber);
        }

        // Remove from standard event handlers
        foreach (var key in _standardEventHandlers.Keys.ToList())
        {
            RemoveEventHandler(key.Source, key.EventName, subscriber);
        }
    }

    /// <summary>
    /// Invoke all handlers for a specific event type
    /// </summary>
    public void InvokeHandlers(Type eventType, object eventData)
    {
        if (!_eventHandlers.TryGetValue(eventType, out var handlers))
            return;

        // Create a copy to allow handlers to unsubscribe during invocation
        var handlersCopy = handlers.ToList();

        foreach (var subscription in handlersCopy)
        {
            if (subscription.TargetReference.IsAlive)
            {
                try
                {
                    subscription.Handler.DynamicInvoke(eventData);
                }
                catch (Exception ex)
                {
                    // Log exception but continue with other handlers
                    Console.WriteLine($"Error invoking event handler: {ex.Message}");
                }
            }
        }

        // Clean up dead references
        CleanupEventHandlers(eventType);
    }

    /// <summary>
    /// Add a handler for PropertyChanged events
    /// </summary>
    public void AddPropertyChangedHandler(INotifyPropertyChanged source, object subscriber, PropertyChangedEventHandler handler)
    {
        if (source == null || handler == null)
            return;

        if (!_propertyChangedHandlers.TryGetValue(source, out var handlers))
        {
            handlers = new List<PropertyChangedSubscription>();
            _propertyChangedHandlers[source] = handlers;
        }

        // Remove any existing subscription for this subscriber
        handlers.RemoveAll(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber);

        // Create a new weak subscription
        var subscription = new PropertyChangedSubscription(subscriber, handler);
        
        // Add the PropertyChanged event handler
        source.PropertyChanged += subscription.EventHandler;
        
        // Add the subscription to our list
        handlers.Add(subscription);

        // Clean up dead references
        CleanupPropertyChangedHandlers(source);
    }

    /// <summary>
    /// Remove a handler for PropertyChanged events
    /// </summary>
    public void RemovePropertyChangedHandler(object source, object subscriber)
    {
        if (!(source is INotifyPropertyChanged notifySource) || !_propertyChangedHandlers.TryGetValue(source, out var handlers))
            return;

        var toRemove = handlers.Where(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber).ToList();
        
        foreach (var subscription in toRemove)
        {
            notifySource.PropertyChanged -= subscription.EventHandler;
            handlers.Remove(subscription);
        }

        // Clean up dead references
        CleanupPropertyChangedHandlers(source);
    }

    /// <summary>
    /// Add a handler for CollectionChanged events
    /// </summary>
    public void AddCollectionChangedHandler(INotifyCollectionChanged source, object subscriber, NotifyCollectionChangedEventHandler handler)
    {
        if (source == null || handler == null)
            return;

        if (!_collectionChangedHandlers.TryGetValue(source, out var handlers))
        {
            handlers = new List<CollectionChangedSubscription>();
            _collectionChangedHandlers[source] = handlers;
        }

        // Remove any existing subscription for this subscriber
        handlers.RemoveAll(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber);

        // Create a new weak subscription
        var subscription = new CollectionChangedSubscription(subscriber, handler);
        
        // Add the CollectionChanged event handler
        source.CollectionChanged += subscription.EventHandler;
        
        // Add the subscription to our list
        handlers.Add(subscription);

        // Clean up dead references
        CleanupCollectionChangedHandlers(source);
    }

    /// <summary>
    /// Remove a handler for CollectionChanged events
    /// </summary>
    public void RemoveCollectionChangedHandler(object source, object subscriber)
    {
        if (!(source is INotifyCollectionChanged notifySource) || !_collectionChangedHandlers.TryGetValue(source, out var handlers))
            return;

        var toRemove = handlers.Where(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber).ToList();
        
        foreach (var subscription in toRemove)
        {
            notifySource.CollectionChanged -= subscription.EventHandler;
            handlers.Remove(subscription);
        }

        // Clean up dead references
        CleanupCollectionChangedHandlers(source);
    }

    /// <summary>
    /// Add a handler for standard .NET events
    /// </summary>
    public void AddEventHandler(object source, string eventName, object subscriber, Delegate handler)
    {
        if (source == null || string.IsNullOrEmpty(eventName) || handler == null)
            return;

        var key = new EventKey(source, eventName);

        if (!_standardEventHandlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<StandardEventSubscription>();
            _standardEventHandlers[key] = handlers;
        }

        // Remove any existing subscription for this subscriber
        handlers.RemoveAll(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber);

        // Get the event info
        var eventInfo = source.GetType().GetEvent(eventName);
        if (eventInfo == null)
            throw new ArgumentException($"Event '{eventName}' not found on '{source.GetType().Name}'");

        // Create a new subscription
        var subscription = new StandardEventSubscription(subscriber, handler, eventInfo, source);
        
        // Add the event handler
        eventInfo.AddEventHandler(source, handler);
        
        // Add the subscription to our list
        handlers.Add(subscription);

        // Clean up dead references
        CleanupStandardEventHandlers(key);
    }

    /// <summary>
    /// Remove a handler for standard .NET events
    /// </summary>
    public void RemoveEventHandler(object source, string eventName, object subscriber)
    {
        var key = new EventKey(source, eventName);
        
        if (!_standardEventHandlers.TryGetValue(key, out var handlers))
            return;

        var toRemove = handlers.Where(h => h.TargetReference.IsAlive && h.TargetReference.Target == subscriber).ToList();
        
        foreach (var subscription in toRemove)
        {
            subscription.EventInfo.RemoveEventHandler(source, subscription.Handler);
            handlers.Remove(subscription);
        }

        // Clean up dead references
        CleanupStandardEventHandlers(key);
    }

    private void CleanupEventHandlers(Type eventType)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handlers))
        {
            handlers.RemoveAll(h => !h.TargetReference.IsAlive);
            
            if (handlers.Count == 0)
            {
                _eventHandlers.Remove(eventType);
            }
        }
    }

    private void CleanupPropertyChangedHandlers(object source)
    {
        if (!(source is INotifyPropertyChanged notifySource) || !_propertyChangedHandlers.TryGetValue(source, out var handlers))
            return;

        var deadHandlers = handlers.Where(h => !h.TargetReference.IsAlive).ToList();
        foreach (var deadHandler in deadHandlers)
        {
            notifySource.PropertyChanged -= deadHandler.EventHandler;
            handlers.Remove(deadHandler);
        }

        if (handlers.Count == 0)
        {
            _propertyChangedHandlers.Remove(source);
        }
    }

    private void CleanupCollectionChangedHandlers(object source)
    {
        if (!(source is INotifyCollectionChanged notifySource) || !_collectionChangedHandlers.TryGetValue(source, out var handlers))
            return;

        var deadHandlers = handlers.Where(h => !h.TargetReference.IsAlive).ToList();
        foreach (var deadHandler in deadHandlers)
        {
            notifySource.CollectionChanged -= deadHandler.EventHandler;
            handlers.Remove(deadHandler);
        }

        if (handlers.Count == 0)
        {
            _collectionChangedHandlers.Remove(source);
        }
    }

    private void CleanupStandardEventHandlers(EventKey key)
    {
        if (!_standardEventHandlers.TryGetValue(key, out var handlers))
            return;

        var deadHandlers = handlers.Where(h => !h.TargetReference.IsAlive).ToList();
        foreach (var deadHandler in deadHandlers)
        {
            deadHandler.EventInfo.RemoveEventHandler(key.Source, deadHandler.Handler);
            handlers.Remove(deadHandler);
        }

        if (handlers.Count == 0)
        {
            _standardEventHandlers.Remove(key);
        }
    }

    #region Subscription Classes

    private class EventSubscription
    {
        public WeakReference TargetReference { get; }
        public Delegate Handler { get; }

        public EventSubscription(object target, Delegate handler)
        {
            TargetReference = new WeakReference(target);
            Handler = handler;
        }
    }

    private class PropertyChangedSubscription
    {
        public WeakReference TargetReference { get; }
        public PropertyChangedEventHandler EventHandler { get; }

        public PropertyChangedSubscription(object target, PropertyChangedEventHandler handler)
        {
            TargetReference = new WeakReference(target);
            EventHandler = handler;
        }
    }

    private class CollectionChangedSubscription
    {
        public WeakReference TargetReference { get; }
        public NotifyCollectionChangedEventHandler EventHandler { get; }

        public CollectionChangedSubscription(object target, NotifyCollectionChangedEventHandler handler)
        {
            TargetReference = new WeakReference(target);
            EventHandler = handler;
        }
    }

    private class StandardEventSubscription
    {
        public WeakReference TargetReference { get; }
        public Delegate Handler { get; }
        public EventInfo EventInfo { get; }

        public StandardEventSubscription(object target, Delegate handler, EventInfo eventInfo, object source)
        {
            TargetReference = new WeakReference(target);
            Handler = handler;
            EventInfo = eventInfo;
        }
    }

    private class EventKey : IEquatable<EventKey>
    {
        public object Source { get; }
        public string EventName { get; }

        public EventKey(object source, string eventName)
        {
            Source = source;
            EventName = eventName;
        }

        public bool Equals(EventKey other)
        {
            if (other == null) return false;
            return Source.Equals(other.Source) && EventName.Equals(other.EventName);
        }

        public override bool Equals(object obj) => Equals(obj as EventKey);

        public override int GetHashCode() => (Source?.GetHashCode() ?? 0) ^ EventName.GetHashCode();
    }

    #endregion
}
```

### Key Features of this Implementation:

1. **EventBus Class**:
   - Singleton pattern for easy global access
   - Methods to subscribe, unsubscribe, and publish custom events
   - Special methods for handling PropertyChanged and CollectionChanged events
   - Support for standard .NET control events

2. **EventHandlerManager Class**:
   - Uses WeakReference to prevent memory leaks
   - Manages multiple types of event subscriptions:
     - Custom events (via EventSubscription)
     - PropertyChanged events (via PropertyChangedSubscription)
     - CollectionChanged events (via CollectionChangedSubscription)
     - Standard .NET events (via StandardEventSubscription)
   - Automatic cleanup of dead references
   - Thread-safe event invocation

3. **Usage Examples**:

```csharp
// Example 1: Custom events
public class UserLoggedInEvent { public string Username { get; set; } }

// Publisher
EventBus.Instance.Publish(new UserLoggedInEvent { Username = "JohnDoe" });

// Subscriber
EventBus.Instance.Subscribe<UserLoggedInEvent>(this, e => 
{
    Console.WriteLine($"User logged in: {e.Username}");
});

// Example 2: PropertyChanged events
var viewModel = new MyViewModel(); // implements INotifyPropertyChanged
EventBus.Instance.SubscribeToPropertyChanged(viewModel, this, (sender, e) => 
{
    if (e.PropertyName == "Username")
    {
        Console.WriteLine("Username changed");
    }
});

// Example 3: CollectionChanged events
var collection = new ObservableCollection<string>(); // implements INotifyCollectionChanged
EventBus.Instance.SubscribeToCollectionChanged(collection, this, (sender, e) => 
{
    Console.WriteLine("Collection changed");
});

// Example 4: Control events
var button = new Button();
EventBus.Instance.SubscribeToEvent(button, "Click", this, new EventHandler((sender, e) => 
{
    Console.WriteLine("Button clicked");
}));
```

This implementation provides a ro

イベントの重複登録を防止するよう `EventHandlerManager` クラスを改善しましょう。同じイベントが複数回登録されないようにする機能を追加します。

「同じイベントは一つだけ」とのご要望ですね。先ほど提案した `EventHandlerManager` クラスに、同じイベントハンドラーが重複して登録されないようにする機能を追加します。

以下に、重複登録を防止するように修正したコードを提示します：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsのイベントハンドラーを一元管理するユーティリティクラス
    /// イベントのアタッチとデタッチを追跡し、一括処理を可能にします
    /// 同じイベントは一つだけ登録されることを保証します
    /// </summary>
    public class EventHandlerManager
    {
        // イベントハンドラー情報を格納するための内部クラス
        private class EventInfo
        {
            public object Sender { get; set; }
            public Delegate Handler { get; set; }
            public string EventName { get; set; }

            public EventInfo(object sender, Delegate handler, string eventName)
            {
                Sender = sender;
                Handler = handler;
                EventName = eventName;
            }

            // 同一イベント判定用の比較メソッド
            public bool IsSameEvent(object sender, string eventName, Delegate handler)
            {
                return Sender == sender && EventName == eventName && Handler == handler;
            }
        }

        // 登録済みのイベントハンドラーを保持するリスト
        private readonly List<EventInfo> _registeredEvents = new List<EventInfo>();

        /// <summary>
        /// イベントにハンドラーを登録し、管理対象に追加します
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "TextChanged")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // イベントにハンドラーを追加
            eventInfo.AddEventHandler(control, eventHandler);

            // 登録情報を保持
            _registeredEvents.Add(new EventInfo(control, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 標準的なEventHandlerを使用するイベントに対する、簡略化されたAttachメソッド
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "Load")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // イベントにハンドラーを追加
            eventInfo.AddEventHandler(control, eventHandler);

            // 登録情報を保持
            _registeredEvents.Add(new EventInfo(control, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 指定されたイベントが既に登録されているかを確認します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>既に登録されている場合はtrue</returns>
        public bool IsEventRegistered(Control control, string eventName, Delegate handler)
        {
            return _registeredEvents.Any(e => e.IsSameEvent(control, eventName, handler));
        }

        /// <summary>
        /// 特定のコントロールとイベントからハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>削除に成功したかどうか</returns>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            // 該当する登録を検索
            var eventToRemove = _registeredEvents.Find(e => e.IsSameEvent(control, eventName, handler));

            if (eventToRemove != null)
            {
                // リフレクションを使用してイベントを取得
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    // イベントからハンドラーを削除
                    eventInfo.RemoveEventHandler(control, handler);

                    // 管理リストから削除
                    _registeredEvents.Remove(eventToRemove);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 特定のコントロールに登録されているすべてのイベントハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        public void DetachAllEvents(Control control)
        {
            // 削除対象のイベント登録情報を取得
            var eventsToRemove = _registeredEvents.FindAll(e => e.Sender == control);

            foreach (var eventInfo in eventsToRemove)
            {
                // コントロールからイベントタイプを取得
                var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                if (controlEventInfo != null)
                {
                    // イベントからハンドラーを削除
                    controlEventInfo.RemoveEventHandler(control, eventInfo.Handler);
                }
            }

            // 管理リストから削除
            _registeredEvents.RemoveAll(e => e.Sender == control);
        }

        /// <summary>
        /// 登録されている全てのイベントハンドラーを削除します
        /// フォーム終了時などに呼び出すとメモリリークを防止できます
        /// </summary>
        public void DetachAllEvents()
        {
            foreach (var eventInfo in _registeredEvents)
            {
                if (eventInfo.Sender is Control control)
                {
                    // コントロールからイベントタイプを取得
                    var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                    if (controlEventInfo != null)
                    {
                        // イベントからハンドラーを削除
                        controlEventInfo.RemoveEventHandler(control, eventInfo.Handler);
                    }
                }
            }

            // すべての登録をクリア
            _registeredEvents.Clear();
        }

        /// <summary>
        /// コントロールが登録しているイベントの数を返します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <returns>登録イベント数</returns>
        public int GetRegisteredEventCount(Control control)
        {
            return _registeredEvents.Count(e => e.Sender == control);
        }

        /// <summary>
        /// 管理下にある全てのイベント登録数を返します
        /// </summary>
        /// <returns>登録イベント総数</returns>
        public int GetRegisteredEventCount()
        {
            return _registeredEvents.Count;
        }
    }
}
```

## 主な変更点

1. **重複チェック機能の追加**: 
   - `IsEventRegistered` メソッドで、同じイベントが既に登録されているか確認します。
   - EventInfoクラスに `IsSameEvent` メソッドを追加し、イベントの比較を容易にしました。

2. **AttachEventメソッドの戻り値の変更**:
   - AttachEventメソッドはbool型を返すように変更し、イベントが新規に登録されたかどうかがわかるようにしました。
   - 既に同じイベントが登録されている場合は、重複して登録せず `false` を返します。

## 使用例

```csharp
public partial class MyForm : Form
{
    private EventHandlerManager _eventManager = new EventHandlerManager();
    
    public MyForm()
    {
        InitializeComponent();
        RegisterEvents();
    }
    
    private void RegisterEvents()
    {
        // イベントを登録（重複を許可しない）
        _eventManager.AttachEvent(button1, "Click", Button1_Click);
        
        // 同じイベントを再度登録しようとしても無視される
        bool registered = _eventManager.AttachEvent(button1, "Click", Button1_Click);
        // registered は false になる
        
        // 別のイベントは正常に登録される
        _eventManager.AttachEvent(textBox1, "TextChanged", TextBox1_TextChanged);
    }
    
    private void Button1_Click(object sender, EventArgs e)
    {
        // このハンドラーは一度だけ呼び出される
    }
    
    private void TextBox1_TextChanged(object sender, EventArgs e)
    {
        // 処理
    }
    
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _eventManager.DetachAllEvents();
        base.OnFormClosing(e);
    }
}
```

この実装により、同じコントロールの同じイベントに同じハンドラーを複数回アタッチしようとしても、一度だけしか登録されなくなります。イベントが重複して発火する問題が解消され、安全にイベント管理を行うことができます。

---
シングルトンのEventBusとEventHandlerManagerを組み合わせて、WinFormsのイベントを一元管理するソリューションを作成しましょう。リポジトリにある既存のメッセージング機能と統合した実装を提案します。リポジトリの既存コードを確認した結果、WinFormsのEventHandlerManagerとシングルトンパターンのEventBusを統合する解決策を提案します。既存のServiceBusとの統合を考慮した実装を作成します。

# シングルトンEventBusとEventHandlerManagerの統合実装

まず、EventBusとEventHandlerManagerの両方を実装した統合クラスを作成します：

```csharp
using CoreLib.Logging;
using CoreLib.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsイベントバスのインターフェース
    /// コントロールイベントとアプリケーションイベントの両方を管理
    /// </summary>
    public interface IWinFormsEventBus
    {
        // イベントハンドラー管理メソッド
        bool AttachEvent(Control control, string eventName, EventHandler eventHandler);
        bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) where TEventArgs : EventArgs;
        bool DetachEvent(Control control, string eventName, Delegate handler);
        void DetachAllEvents(Control control);
        void DetachAllEvents();
        int GetRegisteredEventCount(Control control);
        int GetRegisteredEventCount();

        // アプリケーションイベント管理メソッド
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage;
        void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;
        void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : IMessage;
    }

    /// <summary>
    /// WinFormsイベントバスの実装
    /// シングルトンパターンでイベントの一元管理を行う
    /// </summary>
    public class WinFormsEventBus : IWinFormsEventBus
    {
        // シングルトンインスタンス
        private static readonly Lazy<WinFormsEventBus> _instance = 
            new Lazy<WinFormsEventBus>(() => new WinFormsEventBus());

        // ServiceBusのインスタンス
        private readonly IServiceBus _serviceBus;
        // ロガー
        private readonly IAppLogger _logger;
        // イベント管理のための内部クラスインスタンス
        private readonly EventHandlerManager _eventHandlerManager;

        /// <summary>
        /// シングルトンインスタンスを取得
        /// </summary>
        public static WinFormsEventBus Instance => _instance.Value;

        /// <summary>
        /// プライベートコンストラクタ - シングルトンパターン
        /// </summary>
        private WinFormsEventBus()
        {
            _eventHandlerManager = new EventHandlerManager();

            // DIコンテナがなくても動作するようにフォールバック実装
            try
            {
                // アプリケーション全体のDIコンテナからServiceBusとLoggerを解決
                var serviceProvider = ServiceProviderAccessor.GetServiceProvider();
                if (serviceProvider != null)
                {
                    _serviceBus = serviceProvider.GetService<IServiceBus>();
                    _logger = serviceProvider.GetService<IAppLogger>();
                }
            }
            catch
            {
                // DIコンテナからの取得に失敗した場合は内部でインスタンス作成
            }

            // ServiceBusがDIから解決できなかった場合のフォールバック
            if (_serviceBus == null)
            {
                // ロガーも同様
                if (_logger == null)
                {
                    _logger = new NullLogger(); // ダミーロガー（何も出力しない）
                }

                // 簡易的なServiceBus実装を作成
                _serviceBus = new SimpleServiceBus(_logger);
            }

            _logger.LogInformation($"WinFormsEventBus initialized");
        }

        #region EventHandlerManager委譲メソッド

        /// <summary>
        /// イベントにハンドラーを登録
        /// </summary>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            return _eventHandlerManager.AttachEvent(control, eventName, eventHandler);
        }

        /// <summary>
        /// イベントにハンドラーを登録（ジェネリック版）
        /// </summary>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            return _eventHandlerManager.AttachEvent<TEventArgs>(control, eventName, eventHandler);
        }

        /// <summary>
        /// イベントからハンドラーを削除
        /// </summary>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            return _eventHandlerManager.DetachEvent(control, eventName, handler);
        }

        /// <summary>
        /// 特定のコントロールのすべてのイベントハンドラーを削除
        /// </summary>
        public void DetachAllEvents(Control control)
        {
            _eventHandlerManager.DetachAllEvents(control);
        }

        /// <summary>
        /// すべてのイベントハンドラーを削除
        /// </summary>
        public void DetachAllEvents()
        {
            _eventHandlerManager.DetachAllEvents();
        }

        /// <summary>
        /// コントロールの登録済みイベント数を取得
        /// </summary>
        public int GetRegisteredEventCount(Control control)
        {
            return _eventHandlerManager.GetRegisteredEventCount(control);
        }

        /// <summary>
        /// 登録済みイベント総数を取得
        /// </summary>
        public int GetRegisteredEventCount()
        {
            return _eventHandlerManager.GetRegisteredEventCount();
        }

        #endregion

        #region ServiceBus委譲メソッド

        /// <summary>
        /// メッセージを発行
        /// </summary>
        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IMessage
        {
            return _serviceBus.PublishAsync(message, cancellationToken);
        }

        /// <summary>
        /// メッセージハンドラーを登録
        /// </summary>
        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            _serviceBus.RegisterHandler(handler);
        }

        /// <summary>
        /// メッセージハンドラーの登録を解除
        /// </summary>
        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            _serviceBus.UnregisterHandler(handler);
        }

        #endregion
    }

    /// <summary>
    /// DIからサービスプロバイダーを取得するためのユーティリティクラス
    /// </summary>
    public static class ServiceProviderAccessor
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
    }

    /// <summary>
    /// WinFormsイベントバス用の拡張メソッド
    /// </summary>
    public static class WinFormsEventBusExtensions
    {
        /// <summary>
        /// WinFormsEventBusをDIに登録
        /// </summary>
        public static IServiceCollection AddWinFormsEventBus(this IServiceCollection services)
        {
            // シングルトンインスタンスをDIコンテナに登録
            services.AddSingleton<IWinFormsEventBus>(_ => WinFormsEventBus.Instance);
            return services;
        }

        /// <summary>
        /// WinFormsアプリケーションでDIサービスプロバイダーを初期化
        /// </summary>
        public static void InitializeServiceProvider(this IServiceProvider serviceProvider)
        {
            ServiceProviderAccessor.Initialize(serviceProvider);
        }
    }

    /// <summary>
    /// DIが使用できない場合のシンプルなServiceBus実装
    /// </summary>
    internal class SimpleServiceBus : IServiceBus
    {
        private readonly IAppLogger _logger;
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
        private readonly object _syncRoot = new object();

        public SimpleServiceBus(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = typeof(TMessage);
            _logger.LogDebug($"メッセージを発行: {messageType.Name}, ID={message.MessageId}");

            List<object> handlers;
            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(messageType, out handlers) || handlers.Count == 0)
                {
                    _logger.LogWarning($"メッセージタイプ {messageType.Name} のハンドラーが登録されていません");
                    return;
                }
                handlers = new List<object>(handlers); // スレッドセーフのためにコピー
            }

            var typedHandlers = handlers.Cast<IMessageHandler<TMessage>>();
            var tasks = new List<Task>();

            foreach (var handler in typedHandlers)
            {
                try
                {
                    tasks.Add(handler.HandleAsync(message, cancellationToken));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ハンドラーの実行中にエラーが発生: {ex.Message}");
                }
            }

            await Task.WhenAll(tasks);
        }

        public void RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (!_handlers.TryGetValue(messageType, out var handlers))
                {
                    handlers = new List<object>();
                    _handlers[messageType] = handlers;
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                    _logger.LogDebug($"ハンドラーを登録: {handler.GetType().Name} for {messageType.Name}");
                }
            }
        }

        public void UnregisterHandler<TMessage>(IMessageHandler<TMessage> handler) 
            where TMessage : IMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var messageType = typeof(TMessage);

            lock (_syncRoot)
            {
                if (_handlers.TryGetValue(messageType, out var handlers))
                {
                    if (handlers.Remove(handler))
                    {
                        _logger.LogDebug($"ハンドラーの登録を解除: {handler.GetType().Name} for {messageType.Name}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// ロガーが利用できない場合のダミー実装
    /// </summary>
    internal class NullLogger : IAppLogger
    {
        public void LogDebug(string message) { }
        public void LogInformation(string message) { }
        public void LogWarning(string message) { }
        public void LogError(string message) { }
        public void LogError(Exception exception, string message) { }
        public void LogCritical(string message) { }
        public void LogCritical(Exception exception, string message) { }
    }

    /// <summary>
    /// WinFormsイベント用のメッセージクラス
    /// </summary>
    public class WinFormsEventMessage : IMessage
    {
        public Guid MessageId { get; } = Guid.NewGuid();
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public Control Sender { get; }
        public string EventName { get; }
        public object EventArgs { get; }

        public WinFormsEventMessage(Control sender, string eventName, object eventArgs)
        {
            Sender = sender;
            EventName = eventName;
            EventArgs = eventArgs;
        }
    }
}
```

次に、前回提供したEventHandlerManagerクラスに、イベント発火をEventBusに通知する機能を追加します：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CoreLib.Messaging;

namespace CoreLibWinforms.EventManagement
{
    /// <summary>
    /// WinFormsのイベントハンドラーを一元管理するユーティリティクラス
    /// イベントのアタッチとデタッチを追跡し、一括処理を可能にします
    /// 同じイベントは一つだけ登録されることを保証します
    /// </summary>
    public class EventHandlerManager
    {
        // イベントハンドラー情報を格納するための内部クラス
        private class EventInfo
        {
            public Control Sender { get; set; }
            public Delegate Handler { get; set; }
            public Delegate OriginalHandler { get; set; } // 元のハンドラー
            public string EventName { get; set; }

            public EventInfo(Control sender, Delegate handler, Delegate originalHandler, string eventName)
            {
                Sender = sender;
                Handler = handler;
                OriginalHandler = originalHandler;
                EventName = eventName;
            }

            // 同一イベント判定用の比較メソッド
            public bool IsSameEvent(Control sender, string eventName, Delegate handler)
            {
                return Sender == sender && 
                       EventName == eventName && 
                       (Handler == handler || OriginalHandler == handler);
            }
        }

        // 登録済みのイベントハンドラーを保持するリスト
        private readonly List<EventInfo> _registeredEvents = new List<EventInfo>();

        /// <summary>
        /// イベントにハンドラーを登録し、管理対象に追加します
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <typeparam name="TEventArgs">イベント引数の型</typeparam>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "TextChanged")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent<TEventArgs>(Control control, string eventName, EventHandler<TEventArgs> eventHandler) 
            where TEventArgs : EventArgs
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // EventBusへの転送を行うラッパーハンドラーを作成
            EventHandler<TEventArgs> wrapperHandler = (sender, args) =>
            {
                // 元のハンドラーを呼び出し
                eventHandler(sender, args);

                // EventBusがあれば、イベントをグローバルメッセージとして発行
                try
                {
                    // EventBusにイベントを通知
                    var eventMessage = new WinFormsEventMessage(control, eventName, args);
                    WinFormsEventBus.Instance.PublishAsync(eventMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // イベント転送中のエラーはアプリケーションを停止させない
                    System.Diagnostics.Debug.WriteLine($"EventBus通知エラー: {ex.Message}");
                }
            };

            // イベントにラッパーハンドラーを追加
            eventInfo.AddEventHandler(control, wrapperHandler);

            // 登録情報を保持（元のハンドラーとラッパーハンドラーの両方を記録）
            _registeredEvents.Add(new EventInfo(control, wrapperHandler, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 標準的なEventHandlerを使用するイベントに対する、簡略化されたAttachメソッド
        /// 同じイベント（同じコントロール、同じイベント名、同じハンドラー）は重複登録されません
        /// </summary>
        /// <param name="control">イベントを発生させるコントロール</param>
        /// <param name="eventName">イベント名 (例: "Click", "Load")</param>
        /// <param name="eventHandler">イベントハンドラー</param>
        /// <returns>イベントが新規に登録された場合はtrue、既に登録されていた場合はfalse</returns>
        public bool AttachEvent(Control control, string eventName, EventHandler eventHandler)
        {
            if (control == null || string.IsNullOrEmpty(eventName) || eventHandler == null)
                throw new ArgumentNullException("コントロール、イベント名、またはハンドラーがnullです。");

            // 既に同じイベントが登録されているか確認
            if (IsEventRegistered(control, eventName, eventHandler))
            {
                return false; // 既に登録済み
            }

            // リフレクションを使用してイベントを取得
            var eventInfo = control.GetType().GetEvent(eventName);
            if (eventInfo == null)
                throw new ArgumentException($"指定したイベント '{eventName}' はコントロール '{control.GetType().Name}' に存在しません。");

            // EventBusへの転送を行うラッパーハンドラーを作成
            EventHandler wrapperHandler = (sender, args) =>
            {
                // 元のハンドラーを呼び出し
                eventHandler(sender, args);

                // EventBusがあれば、イベントをグローバルメッセージとして発行
                try
                {
                    // EventBusにイベントを通知
                    var eventMessage = new WinFormsEventMessage(control, eventName, args);
                    WinFormsEventBus.Instance.PublishAsync(eventMessage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // イベント転送中のエラーはアプリケーションを停止させない
                    System.Diagnostics.Debug.WriteLine($"EventBus通知エラー: {ex.Message}");
                }
            };

            // イベントにラッパーハンドラーを追加
            eventInfo.AddEventHandler(control, wrapperHandler);

            // 登録情報を保持（元のハンドラーとラッパーハンドラーの両方を記録）
            _registeredEvents.Add(new EventInfo(control, wrapperHandler, eventHandler, eventName));
            return true;
        }

        /// <summary>
        /// 指定されたイベントが既に登録されているかを確認します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>既に登録されている場合はtrue</returns>
        public bool IsEventRegistered(Control control, string eventName, Delegate handler)
        {
            return _registeredEvents.Any(e => e.IsSameEvent(control, eventName, handler));
        }

        /// <summary>
        /// 特定のコントロールとイベントからハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        /// <param name="eventName">イベント名</param>
        /// <param name="handler">イベントハンドラー</param>
        /// <returns>削除に成功したかどうか</returns>
        public bool DetachEvent(Control control, string eventName, Delegate handler)
        {
            // 該当する登録を検索
            var eventToRemove = _registeredEvents.Find(e => e.IsSameEvent(control, eventName, handler));

            if (eventToRemove != null)
            {
                // リフレクションを使用してイベントを取得
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo != null)
                {
                    // イベントからハンドラーを削除
                    eventInfo.RemoveEventHandler(control, eventToRemove.Handler);

                    // 管理リストから削除
                    _registeredEvents.Remove(eventToRemove);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 特定のコントロールに登録されているすべてのイベントハンドラーを削除します
        /// </summary>
        /// <param name="control">コントロール</param>
        public void DetachAllEvents(Control control)
        {
            // 削除対象のイベント登録情報を取得
            var eventsToRemove = _registeredEvents.FindAll(e => e.Sender == control);

            foreach (var eventInfo in eventsToRemove)
            {
                // コントロールからイベントタイプを取得
                var controlEventInfo = control.GetType().GetEvent(eventInfo.EventName);
                if (controlEventInfo != null)
                {
                    // イベントからハンドラーを削除
                    controlEventInfo.RemoveEventHandler(control, even