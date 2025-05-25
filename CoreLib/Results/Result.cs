using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core
{
    /// <summary>
    /// 汎用的な結果オブジェクト
    /// </summary>
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Exception? Exception { get; private set; }

        public static Result<T> Ok(T data) => new Result<T> { Success = true, Data = data };
        public static Result<T> Fail(string message) => new Result<T> { Success = false, ErrorMessage = message };
        public static Result<T> Fail(Exception ex) => new Result<T> { Success = false, ErrorMessage = ex.Message, Exception = ex };
    }
}
