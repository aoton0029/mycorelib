using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// ソート方向
    /// </summary>
    public enum SortDirection
    {
        [Description("昇順")]
        Ascending = 0,

        [Description("降順")]
        Descending = 1
    }
}
