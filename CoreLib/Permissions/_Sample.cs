using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Permissions
{
    internal class _Sample
    {
        // カスタムリソースタイプの拡張
        public enum ExtendedResourceType
        {
            // 基本リソースタイプからの継承（値が被らないよう注意）
            User = ResourceType.User,
            Customer = ResourceType.Customer,
            Order = ResourceType.Order,

            // 拡張リソース（100から開始）
            Invoice = 100,
            Contract = 101,
            Project = 102,
            Task = 103
        }

        // カスタム権限タイプの拡張
        [Flags]
        public enum ExtendedPermissionType
        {
            // 基本権限タイプからの継承
            None = PermissionType.None,
            View = PermissionType.View,
            Create = PermissionType.Create,
            Edit = PermissionType.Edit,
            Delete = PermissionType.Delete,

            // 拡張権限（高いビット位置から開始）
            Approve = 1 << 10,
            Reject = 1 << 11,
            Sign = 1 << 12,
            Assign = 1 << 13
        }

        // 拡張ロールの例
        public class CustomRoleProvider
        {
            public static Role[] GetPredefinedRoles()
            {
                var roles = new Role[]
                {
                    // プロジェクトマネージャーロール
                    new Role("ProjectManager", "プロジェクト管理者")
                    {
                        // 権限追加
                    }.AddPermission(new Permission((ResourceType)ExtendedResourceType.Project,
                        (PermissionType)(ExtendedPermissionType.View | ExtendedPermissionType.Create |
                                         ExtendedPermissionType.Edit | ExtendedPermissionType.Assign))),
                
                    // 契約管理者ロール
                    new Role("ContractManager", "契約管理者")
                    {
                        // 権限追加
                    }.AddPermission(new Permission((ResourceType)ExtendedResourceType.Contract,
                        (PermissionType)(ExtendedPermissionType.View | ExtendedPermissionType.Create |
                                         ExtendedPermissionType.Edit | ExtendedPermissionType.Sign)))
                    };

                return roles;
            }
        }
    }
}
