using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// 標準ErrorProviderを拡張した機能強化版ErrorProvider
    /// </summary>
    public class EnhancedErrorProvider : ErrorProvider
    {
        private readonly Dictionary<Control, List<ValidationRule>> _validationRules = new();
        private readonly Dictionary<Control, object> _controlToDataSourceMap = new();
        private bool _showAllErrors = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnhancedErrorProvider() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnhancedErrorProvider(IContainer container) : base(container)
        {
        }

        /// <summary>
        /// 全てのエラーを表示するかどうか
        /// </summary>
        public bool ShowAllErrors
        {
            get => _showAllErrors;
            set => _showAllErrors = value;
        }

        /// <summary>
        /// コントロールにバリデーションルールを追加
        /// </summary>
        /// <param name="control">対象コントロール</param>
        /// <param name="validationRule">バリデーションルール</param>
        public void AddValidationRule(Control control, ValidationRule validationRule)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (validationRule == null)
                throw new ArgumentNullException(nameof(validationRule));

            if (!_validationRules.ContainsKey(control))
                _validationRules[control] = new List<ValidationRule>();

            _validationRules[control].Add(validationRule);
        }

        /// <summary>
        /// データアノテーションを使用してバリデーションルールを追加
        /// </summary>
        /// <param name="control">対象コントロール</param>
        /// <param name="validationAttributes">バリデーション属性配列</param>
        public void AddValidationRules(Control control, params ValidationAttribute[] validationAttributes)
        {
            foreach (var attribute in validationAttributes)
            {
                AddValidationRule(control, new DataAnnotationValidationRule(attribute));
            }
        }

        /// <summary>
        /// オブジェクトのプロパティからデータアノテーションを取得して自動的にバリデーションルールを構築
        /// </summary>
        /// <param name="control">対象コントロール</param>
        /// <param name="dataSource">データソースオブジェクト</param>
        /// <param name="propertyName">バインドするプロパティ名</param>
        public void BindToProperty(Control control, object dataSource, string propertyName)
        {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            PropertyInfo? property = dataSource.GetType().GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"プロパティ '{propertyName}' が見つかりません。", nameof(propertyName));

            // データソースを記録
            _controlToDataSourceMap[control] = dataSource;

            // データアノテーションを取得
            var validationAttributes = property.GetCustomAttributes<ValidationAttribute>(true);
            foreach (var attribute in validationAttributes)
            {
                AddValidationRule(control, new DataAnnotationValidationRule(attribute));
            }

            // コントロールのイベントハンドラを設定（典型的なコントロールの例）
            if (control is TextBox textBox)
            {
                textBox.TextChanged += (sender, e) => ValidateControl(textBox);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.SelectedIndexChanged += (sender, e) => ValidateControl(comboBox);
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                dateTimePicker.ValueChanged += (sender, e) => ValidateControl(dateTimePicker);
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.CheckedChanged += (sender, e) => ValidateControl(checkBox);
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.ValueChanged += (sender, e) => ValidateControl(numericUpDown);
            }
        }

        /// <summary>
        /// コントロールに関連付けられたバリデーションルールを全て削除
        /// </summary>
        public void ClearValidationRules(Control control)
        {
            if (_validationRules.ContainsKey(control))
            {
                _validationRules.Remove(control);
            }
        }

        /// <summary>
        /// コントロールのバリデーションを実行
        /// </summary>
        /// <returns>バリデーション成功の場合はtrue</returns>
        public bool ValidateControl(Control control)
        {
            // エラーメッセージをクリア
            SetError(control, string.Empty);

            if (!_validationRules.ContainsKey(control) || _validationRules[control].Count == 0)
                return true;

            var value = GetControlValue(control);
            var errors = new List<string>();

            // 全てのルールを検証
            foreach (var rule in _validationRules[control])
            {
                if (!rule.Validate(value, out string errorMessage))
                {
                    errors.Add(errorMessage);
                    if (!_showAllErrors)
                        break;
                }
            }

            // エラーがあればセット
            if (errors.Count > 0)
            {
                SetError(control, string.Join(Environment.NewLine, errors));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 全てのコントロールのバリデーションを実行
        /// </summary>
        /// <returns>全て成功した場合はtrue</returns>
        public bool ValidateAll()
        {
            bool isValid = true;

            foreach (var control in _validationRules.Keys)
            {
                if (!ValidateControl(control))
                    isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// コントロールから値を取得
        /// </summary>
        private object? GetControlValue(Control control)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            else if (control is ComboBox comboBox)
                return comboBox.SelectedItem ?? comboBox.Text;
            else if (control is CheckBox checkBox)
                return checkBox.Checked;
            else if (control is RadioButton radioButton)
                return radioButton.Checked;
            else if (control is DateTimePicker dateTimePicker)
                return dateTimePicker.Value;
            else if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value;
            else if (control is ListBox listBox)
                return listBox.SelectedItem;
            else if (control is TrackBar trackBar)
                return trackBar.Value;
            else
                return control.Text;
        }
    }
    
}
