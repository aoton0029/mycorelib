using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Utilities.Validation
{
    /// <summary>
    /// ビューモデルにおけるエラー通知機能を提供するエラープロバイダー拡張
    /// </summary>
    public class ValidationErrorProvider : INotifyDataErrorInfo
    {
        private readonly ConcurrentDictionary<string, List<string>> _errors = new();
        private readonly ConcurrentDictionary<string, List<ValidationAttribute>> _validationRules = new();

        /// <summary>
        /// エラーが発生した時に呼び出されるイベント
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// エラーが存在するかどうかを示すフラグ
        /// </summary>
        public bool HasErrors => _errors.Any(kv => kv.Value.Count > 0);

        /// <summary>
        /// 指定されたプロパティに関連するエラーを取得
        /// </summary>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _errors[propertyName];
        }

        /// <summary>
        /// 指定されたプロパティに関連するエラーを文字列として取得
        /// </summary>
        public string GetErrorMessage(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return string.Empty;

            return string.Join(Environment.NewLine, _errors[propertyName]);
        }

        /// <summary>
        /// エラーが発生したことを通知
        /// </summary>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// エラーを追加
        /// </summary>
        public void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// 指定したプロパティのエラーを全てクリア
        /// </summary>
        public void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Any())
            {
                _errors[propertyName].Clear();
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// 全てのエラーをクリア
        /// </summary>
        public void ClearAllErrors()
        {
            var propertiesToClear = _errors.Keys.ToList();
            foreach (var property in propertiesToClear)
            {
                ClearErrors(property);
            }
        }

        /// <summary>
        /// バリデーションルールを登録
        /// </summary>
        public void RegisterValidationRule(string propertyName, ValidationAttribute validationRule)
        {
            if (!_validationRules.ContainsKey(propertyName))
                _validationRules[propertyName] = new List<ValidationAttribute>();

            _validationRules[propertyName].Add(validationRule);
        }

        /// <summary>
        /// バリデーションルールの登録を解除
        /// </summary>
        public void UnregisterValidationRule(string propertyName, ValidationAttribute validationRule)
        {
            if (_validationRules.ContainsKey(propertyName))
            {
                _validationRules[propertyName].Remove(validationRule);
            }
        }

        /// <summary>
        /// 指定したプロパティのバリデーションを実行
        /// </summary>
        public bool ValidateProperty(object value, [CallerMemberName] string propertyName = "")
        {
            ClearErrors(propertyName);

            if (_validationRules.ContainsKey(propertyName))
            {
                foreach (var rule in _validationRules[propertyName])
                {
                    var result = rule.GetValidationResult(value, new ValidationContext(this) { MemberName = propertyName });
                    //if (result != ValidationResult.Success)
                    //{
                    //    AddError(propertyName, result?.ErrorMessage ?? "不明なエラーが発生しました");
                    //}
                }
            }

            return !GetErrors(propertyName).Cast<string>().Any();
        }

        /// <summary>
        /// 全プロパティのバリデーションを実行
        /// </summary>
        public bool Validate(object instance)
        {
            bool isValid = true;
            foreach (var kv in _validationRules)
            {
                var propertyInfo = instance.GetType().GetProperty(kv.Key);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(instance);
                    if (!ValidateProperty(value, kv.Key))
                        isValid = false;
                }
            }
            return isValid;
        }
    }
}
