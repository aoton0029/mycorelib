using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    public class AppPermissions
    {
        // アプリケーション固有のユーザーロール
        public static readonly ApplicationRole User = new ApplicationRole(0, "User", "一般ユーザー");
        public static readonly ApplicationRole Manager = new ApplicationRole(1, "Manager", "マネージャー");
        public static readonly ApplicationRole Administrator = new ApplicationRole(2, "Administrator", "管理者");
        public static readonly ApplicationRole SystemAdmin = new ApplicationRole(3, "SystemAdmin", "システム管理者");

        // すべてのロールをリストで取得
        public static List<IUserRole> AllRoles => new List<IUserRole> { User, Manager, Administrator, SystemAdmin };

        // アプリケーション固有の権限
        public static readonly ApplicationPermission None = new ApplicationPermission(0, "None", "なし", false);
        public static readonly ApplicationPermission Read = new ApplicationPermission(1, "Read", "閲覧");
        public static readonly ApplicationPermission Create = new ApplicationPermission(2, "Create", "作成");
        public static readonly ApplicationPermission Update = new ApplicationPermission(4, "Update", "更新");
        public static readonly ApplicationPermission Delete = new ApplicationPermission(8, "Delete", "削除");
        public static readonly ApplicationPermission Export = new ApplicationPermission(16, "Export", "エクスポート");
        public static readonly ApplicationPermission Import = new ApplicationPermission(32, "Import", "インポート");
        public static readonly ApplicationPermission Manage = new ApplicationPermission(64, "Manage", "管理");

        // 組み合わせ権限
        public static readonly ApplicationPermission Basic = ApplicationPermission.Combine(Read, Create, Update);
        public static readonly ApplicationPermission Full = ApplicationPermission.Combine(Read, Create, Update, Delete, Export, Import, Manage);

        // 権限名から権限オブジェクトを解決するメソッド
        public static ApplicationPermission ResolvePermission(string permissionName)
        {
            return permissionName.ToLower() switch
            {
                "none" => None,
                "read" => Read,
                "create" => Create,
                "update" => Update,
                "delete" => Delete,
                "export" => Export,
                "import" => Import,
                "manage" => Manage,
                "basic" => Basic,
                "full" => Full,
                _ => null
            };
        }

        // アプリケーションの権限初期化
        public static void InitializePermissions()
        {
            var permManager = PermissionManager.Instance;

            // 顧客管理機能の権限設定例
            permManager.SetPermission("CustomerManagement", User, Read);
            permManager.SetPermission("CustomerManagement", Manager, Basic);
            permManager.SetPermission("CustomerManagement", Administrator, Full);
            permManager.SetPermission("CustomerManagement", SystemAdmin, Full);

            // 製品管理機能の権限設定例
            permManager.SetPermission("ProductManagement", User, Read);
            permManager.SetPermission("ProductManagement", Manager, ApplicationPermission.Combine(Basic, Delete));
            permManager.SetPermission("ProductManagement", Administrator, Full);
            permManager.SetPermission("ProductManagement", SystemAdmin, Full);

            // システム設定の権限設定例
            permManager.SetPermission("SystemSettings", User, None);
            permManager.SetPermission("SystemSettings", Manager, Read);
            permManager.SetPermission("SystemSettings", Administrator, ApplicationPermission.Combine(Basic, Manage));
            permManager.SetPermission("SystemSettings", SystemAdmin, Full);
        }
    }
}
