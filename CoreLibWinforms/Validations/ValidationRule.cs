using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// バリデーションルールのインターフェース
    /// </summary>
    public interface IValidationRule
    {
        bool Validate(object? value, out string errorMessage);
    }

    /// <summary>
    /// バリデーションルールの抽象基底クラス
    /// </summary>
    public abstract class ValidationRule : IValidationRule
    {
        /// <summary>
        /// バリデーションを実行
        /// </summary>
        public abstract bool Validate(object? value, out string errorMessage);
    }
}
