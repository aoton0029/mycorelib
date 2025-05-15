using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// カスタムバリデーションルール
    /// </summary>
    public class CustomValidationRule : ValidationRule
    {
        private readonly Func<object?, bool> _validationFunc;
        private readonly string _errorMessage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomValidationRule(Func<object?, bool> validationFunc, string errorMessage)
        {
            _validationFunc = validationFunc ?? throw new ArgumentNullException(nameof(validationFunc));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        /// <summary>
        /// バリデーションを実行
        /// </summary>
        public override bool Validate(object? value, out string errorMessage)
        {
            if (_validationFunc(value))
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = _errorMessage;
            return false;
        }
    }
}
