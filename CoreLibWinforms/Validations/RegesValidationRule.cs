using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// 正規表現バリデーションルール
    /// </summary>
    public class RegexValidationRule : ValidationRule
    {
        private readonly string _pattern;
        private readonly string _errorMessage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegexValidationRule(string pattern, string errorMessage)
        {
            _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        /// <summary>
        /// バリデーションを実行
        /// </summary>
        public override bool Validate(object? value, out string errorMessage)
        {
            if (value == null)
            {
                errorMessage = _errorMessage;
                return false;
            }

            string stringValue = value.ToString() ?? string.Empty;
            if (System.Text.RegularExpressions.Regex.IsMatch(stringValue, _pattern))
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = _errorMessage;
            return false;
        }
    }
}
