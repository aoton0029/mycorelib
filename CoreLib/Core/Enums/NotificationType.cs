using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// 通知タイプ
    /// </summary>
    public enum NotificationType
    {
        [Description("情報")]
        Information = 0,

        [Description("成功")]
        Success = 1,

        [Description("警告")]
        Warning = 2,

        [Description("エラー")]
        Error = 3,

        [Description("システム")]
        System = 4
    }
}
