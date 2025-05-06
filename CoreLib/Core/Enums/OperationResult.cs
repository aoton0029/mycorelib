using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// 操作結果
    /// </summary>
    public enum OperationResult
    {
        [Description("成功")]
        Success = 0,

        [Description("失敗")]
        Failure = 1,

        [Description("部分的成功")]
        PartialSuccess = 2,

        [Description("アクセス拒否")]
        AccessDenied = 3,

        [Description("無効な入力")]
        InvalidInput = 4,

        [Description("リソースが見つかりません")]
        NotFound = 5,

        [Description("タイムアウト")]
        Timeout = 6,

        [Description("重複データ")]
        Duplicate = 7,

        [Description("処理中止")]
        Aborted = 8,

        [Description("システムエラー")]
        SystemError = 9
    }
}
