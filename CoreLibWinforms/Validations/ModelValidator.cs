using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Validations
{
    /// <summary>
    /// モデルバインディングによるバリデーションをサポートするクラス
    /// </summary>
    public class ModelValidator<TModel> where TModel : class
    {
        private readonly TModel _model;
        private readonly EnhancedErrorProvider _errorProvider;
        private readonly Dictionary<string, Control> _boundControls = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelValidator(TModel model, EnhancedErrorProvider errorProvider)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _errorProvider = errorProvider ?? throw new ArgumentNullException(nameof(errorProvider));
        }

        /// <summary>
        /// モデルプロパティをコントロールにバインド
        /// </summary>
        public void BindProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, Control control)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (control == null)
                throw new ArgumentNullException(nameof(control));

            // プロパティ名を取得
            string propertyName = GetPropertyName(propertyExpression);
            PropertyInfo? propertyInfo = typeof(TModel).GetProperty(propertyName);

            if (propertyInfo == null)
                throw new ArgumentException($"プロパティ '{propertyName}' が見つかりません。", nameof(propertyExpression));

            // コントロールの種類に応じたイベントハンドラを設定
            SetControlValueChangedHandler(control, propertyInfo);

            // プロパティの値をコントロールに設定
            SetControlValue(control, propertyInfo.GetValue(_model));

            // プロパティに対するバリデーション属性を取得して適用
            var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
            foreach (var attribute in validationAttributes)
            {
                _errorProvider.AddValidationRule(control, new DataAnnotationValidationRule(attribute));
            }

            _boundControls[propertyName] = control;
        }

        /// <summary>
        /// モデルプロパティをテキストボックスにバインド（簡易版）
        /// </summary>
        public void BindTextBox<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, TextBox textBox)
        {
            BindProperty(propertyExpression, textBox);
        }

        /// <summary>
        /// モデルプロパティをコンボボックスにバインド（簡易版）
        /// </summary>
        public void BindComboBox<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, ComboBox comboBox)
        {
            BindProperty(propertyExpression, comboBox);
        }

        /// <summary>
        /// 全ての関連コントロールのバリデーションを実行
        /// </summary>
        public bool ValidateAll()
        {
            bool isValid = true;
            foreach (var control in _boundControls.Values)
            {
                if (!_errorProvider.ValidateControl(control))
                    isValid = false;
            }
            return isValid;
        }

        /// <summary>
        /// モデルの値をコントロールに反映
        /// </summary>
        public void UpdateControls()
        {
            foreach (var pair in _boundControls)
            {
                PropertyInfo? property = typeof(TModel).GetProperty(pair.Key);
                if (property != null)
                {
                    var value = property.GetValue(_model);
                    SetControlValue(pair.Value, value);
                }
            }
        }

        /// <summary>
        /// コントロールの値をモデルに反映
        /// </summary>
        public void UpdateModel()
        {
            foreach (var pair in _boundControls)
            {
                PropertyInfo? property = typeof(TModel).GetProperty(pair.Key);
                if (property != null)
                {
                    var value = GetControlValue(pair.Value);
                    try
                    {
                        // 必要な型変換を行う
                        object? convertedValue = ConvertValue(value, property.PropertyType);
                        property.SetValue(_model, convertedValue);
                    }
                    catch (Exception)
                    {
                        // 型変換エラーは無視（バリデーションで捕捉される）
                    }
                }
            }
        }

        /// <summary>
        /// プロパティ名を取得
        /// </summary>
        private string GetPropertyName<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException("式はプロパティを表していません", nameof(propertyExpression));
        }

        /// <summary>
        /// コントロールの値変更イベントのハンドラを設定
        /// </summary>
        private void SetControlValueChangedHandler(Control control, PropertyInfo propertyInfo)
        {
            if (control is TextBox textBox)
            {
                textBox.TextChanged += (s, e) => UpdateModelProperty(propertyInfo, textBox);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.SelectedIndexChanged += (s, e) => UpdateModelProperty(propertyInfo, comboBox);
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.CheckedChanged += (s, e) => UpdateModelProperty(propertyInfo, checkBox);
            }
            else if (control is RadioButton radioButton)
            {
                radioButton.CheckedChanged += (s, e) => UpdateModelProperty(propertyInfo, radioButton);
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                dateTimePicker.ValueChanged += (s, e) => UpdateModelProperty(propertyInfo, dateTimePicker);
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.ValueChanged += (s, e) => UpdateModelProperty(propertyInfo, numericUpDown);
            }
        }

        /// <summary>
        /// モデルプロパティを更新
        /// </summary>
        private void UpdateModelProperty(PropertyInfo property, Control control)
        {
            try
            {
                var value = GetControlValue(control);
                object? convertedValue = ConvertValue(value, property.PropertyType);
                property.SetValue(_model, convertedValue);

                // バリデーションを実行
                _errorProvider.ValidateControl(control);
            }
            catch
            {
                // 変換エラーはバリデーションで処理
            }
        }

        /// <summary>
        /// コントロールに値を設定
        /// </summary>
        private void SetControlValue(Control control, object? value)
        {
            if (value == null)
                return;

            if (control is TextBox textBox)
            {
                textBox.Text = value.ToString();
            }
            else if (control is ComboBox comboBox)
            {
                if (comboBox.Items.Count > 0)
                {
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (comboBox.Items[i]?.Equals(value) == true)
                        {
                            comboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    comboBox.Text = value.ToString();
                }
            }
            else if (control is CheckBox checkBox)
            {
                if (value is bool boolValue)
                {
                    checkBox.Checked = boolValue;
                }
            }
            else if (control is RadioButton radioButton)
            {
                if (value is bool boolValue)
                {
                    radioButton.Checked = boolValue;
                }
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                if (value is DateTime dateTime)
                {
                    dateTimePicker.Value = dateTime;
                }
            }
            else if (control is NumericUpDown numericUpDown)
            {
                if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                {
                    numericUpDown.Value = Math.Max(numericUpDown.Minimum,
                        Math.Min(numericUpDown.Maximum, decimalValue));
                }
            }
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
            else
                return control.Text;
        }

        /// <summary>
        /// 値を目的の型に変換
        /// </summary>
        private object? ConvertValue(object? value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType == typeof(string))
                return value.ToString();

            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                if (int.TryParse(value.ToString(), out int intValue))
                    return intValue;
                return null;
            }

            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                    return decimalValue;
                return null;
            }

            if (targetType == typeof(double) || targetType == typeof(double?))
            {
                if (double.TryParse(value.ToString(), out double doubleValue))
                    return doubleValue;
                return null;
            }

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                if (value is DateTime dateTime)
                    return dateTime;
                if (DateTime.TryParse(value.ToString(), out DateTime dateTimeValue))
                    return dateTimeValue;
                return null;
            }

            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                if (value is bool boolValue)
                    return boolValue;
                if (bool.TryParse(value.ToString(), out bool booleanValue))
                    return booleanValue;
                return null;
            }

            // 列挙型の場合
            if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, value.ToString() ?? string.Empty);
                }
                catch
                {
                    return null;
                }
            }

            // 変換できない場合はnullを返す
            return null;
        }
    }

    /// <summary>
    /// モデルバインディングのためのフォーム拡張メソッド
    /// </summary>
    public static class ModelBindingExtensions
    {
        /// <summary>
        /// フォームにモデルをバインド
        /// </summary>
        public static ModelValidator<TModel> BindModel<TModel>(this Form form, TModel model) where TModel : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var errorProvider = form.GetErrorProvider();
            return new ModelValidator<TModel>(model, errorProvider);
        }

        /// <summary>
        /// フォームからEnhancedErrorProviderを取得または作成
        /// </summary>
        public static EnhancedErrorProvider GetErrorProvider(this Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // フォームのコンポーネントからEnhancedErrorProviderを探す
            foreach (var component in form.Components)
            {
                if (component is EnhancedErrorProvider errorProvider)
                    return errorProvider;
            }

            // 見つからない場合は新しく作成して追加
            var newErrorProvider = new EnhancedErrorProvider(form.Container)
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            return newErrorProvider;
        }
    }
}
