using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Permissions
{
    public class PermissionSystemUsage
    {
        public void InitializePermissions()
        {
            // 管理者ロールの作成
            var adminRole = new Role("Administrator", "システム管理者");
            adminRole.AddPermission(new Permission(ResourceType.User, PermissionType.View | PermissionType.Create | PermissionType.Edit | PermissionType.Delete | PermissionType.Admin));
            adminRole.AddPermission(new Permission(ResourceType.Setting, PermissionType.View | PermissionType.Edit | PermissionType.Admin));

            // 一般ユーザーロールの作成
            var userRole = new Role("StandardUser", "一般ユーザー");
            userRole.AddPermission(new Permission(ResourceType.Customer, PermissionType.View | PermissionType.Create | PermissionType.Edit));
            userRole.AddPermission(new Permission(ResourceType.Order, PermissionType.View | PermissionType.Create));
            userRole.AddPermission(new Permission(ResourceType.Product, PermissionType.View));

            // 閲覧者ロールの作成
            var viewerRole = new Role("Viewer", "閲覧者");
            viewerRole.AddPermission(new Permission(ResourceType.Customer, PermissionType.View));
            viewerRole.AddPermission(new Permission(ResourceType.Order, PermissionType.View));
            viewerRole.AddPermission(new Permission(ResourceType.Product, PermissionType.View));

            // 権限管理システムの初期化
            var permissionManager = PermissionManager.Instance;
            permissionManager.AddRole(adminRole);
            permissionManager.AddRole(userRole);
            permissionManager.AddRole(viewerRole);

            // ユーザーの作成
            var adminUser = new User("1", "admin");
            adminUser.AddRole(adminRole);

            var standardUser = new User("2", "user1");
            standardUser.AddRole(userRole);

            var viewerUser = new User("3", "viewer1");
            viewerUser.AddRole(viewerRole);

            // 特別な権限を持つユーザー
            var specialUser = new User("4", "special_user");
            specialUser.AddRole(userRole);
            // 直接の特別な権限を追加
            specialUser.AddDirectPermission(new Permission(ResourceType.Report, PermissionType.Export));

            // ユーザーをシステムに登録
            permissionManager.AddUser(adminUser);
            permissionManager.AddUser(standardUser);
            permissionManager.AddUser(viewerUser);
            permissionManager.AddUser(specialUser);
        }

        public void CheckPermissionExample()
        {
            var permissionManager = PermissionManager.Instance;

            // 現在のユーザーを設定
            var user = permissionManager.GetUser("2"); // 一般ユーザー
            permissionManager.SetCurrentUser(user);

            // 権限チェック
            bool canViewCustomers = permissionManager.CheckPermission(ResourceType.Customer, PermissionType.View);
            Console.WriteLine($"Can view customers: {canViewCustomers}"); // true

            bool canDeleteCustomers = permissionManager.CheckPermission(ResourceType.Customer, PermissionType.Delete);
            Console.WriteLine($"Can delete customers: {canDeleteCustomers}"); // false

            bool canEditSettings = permissionManager.CheckPermission(ResourceType.Setting, PermissionType.Edit);
            Console.WriteLine($"Can edit settings: {canEditSettings}"); // false
        }

        // WPFでの権限チェック例（コードビハインド）
        public void CheckPermissionInUI()
        {
            // ボタンの可視性を権限に基づいて設定
            // <Button x:Name="btnDeleteUser" Content="Delete User" 
            //         local:PermissionBehavior.ResourceType="User" 
            //         local:PermissionBehavior.RequiredPermission="Delete" />

            // 現在のユーザーが変更された場合の更新
            User newUser = PermissionManager.Instance.GetUser("1"); // 管理者
            PermissionManager.Instance.SetCurrentUser(newUser);

            // UIの権限を更新（MainWindowがUIのルート要素と仮定）
            // WPFIntegration.PermissionBehavior.UpdatePermissions(MainWindow);
        }
    }
    

}
