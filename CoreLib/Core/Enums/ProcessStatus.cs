using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// 処理状態
    /// </summary>
    public enum ProcessStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}
