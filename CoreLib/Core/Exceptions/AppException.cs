using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Exceptions
{
    /// <summary>
    /// アプリケーション基本例外
    /// </summary>
    public class AppException : Exception
    {
        public string Code { get; }

        public AppException(string message) : base(message)
        {
            Code = "General";
        }

        public AppException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
