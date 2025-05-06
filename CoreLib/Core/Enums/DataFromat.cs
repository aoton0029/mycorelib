using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Enums
{
    /// <summary>
    /// データフォーマット
    /// </summary>
    public enum DataFormat
    {
        [Description("CSV")]
        Csv = 0,

        [Description("JSON")]
        Json = 1,

        [Description("XML")]
        Xml = 2,

        [Description("Excel")]
        Excel = 3,

        [Description("PDF")]
        Pdf = 4,

        [Description("テキスト")]
        Text = 5,

        [Description("バイナリ")]
        Binary = 6
    }
}
