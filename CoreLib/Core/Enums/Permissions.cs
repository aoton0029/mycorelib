using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// ユーザーロール
    /// </summary>
    public enum UserRole
    {
        User = 0,
        Manager = 1,
        Administrator = 2,
        SystemAdmin = 3
    }

    /// <summary>
    /// ユーザー権限
    /// </summary>
    [Flags]
    public enum Permissions
    {
        [Description("なし")]
        None = 0,

        [Description("閲覧")]
        Read = 1,

        [Description("作成")]
        Create = 2,

        [Description("更新")]
        Update = 4,

        [Description("削除")]
        Delete = 8,

        [Description("エクスポート")]
        Export = 16,

        [Description("インポート")]
        Import = 32,

        [Description("管理")]
        Manage = 64,

        // 組み合わせ例
        [Description("基本権限")]
        Basic = Read | Create | Update,

        [Description("フル権限")]
        Full = Read | Create | Update | Delete | Export | Import | Manage
    }
}
