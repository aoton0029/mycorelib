using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// データアノテーションを使用したバリデーションルール
    /// </summary>
    public class DataAnnotationValidationRule : ValidationRule
    {
        private readonly ValidationAttribute _validationAttribute;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataAnnotationValidationRule(ValidationAttribute validationAttribute)
        {
            _validationAttribute = validationAttribute ?? throw new ArgumentNullException(nameof(validationAttribute));
        }

        /// <summary>
        /// バリデーションを実行
        /// </summary>
        public override bool Validate(object? value, out string errorMessage)
        {
            var validationContext = new ValidationContext(new object())
            {
                MemberName = "Value"
            };

            var validationResult = _validationAttribute.GetValidationResult(value, validationContext);
            if (validationResult != ValidationResult.Success)
            {
                errorMessage = validationResult?.ErrorMessage ?? "不明なエラーが発生しました";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
