using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// エンティティの状態
    /// </summary>
    public enum EntityStatus
    {
        [Description("アクティブ")]
        Active = 0,

        [Description("非アクティブ")]
        Inactive = 1,

        [Description("削除済み")]
        Deleted = 2,

        [Description("アーカイブ済み")]
        Archived = 3,

        [Description("ドラフト")]
        Draft = 4,

        [Description("保留中")]
        Pending = 5,

        [Description("ロック済み")]
        Locked = 6
    }
}
}
