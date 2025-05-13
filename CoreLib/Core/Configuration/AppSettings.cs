using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Configuration
{
    /// <summary>
    /// 設定クラスの基底クラス
    /// </summary>
    public abstract class AppSettings : IValidatable
    {
        /// <summary>
        /// バージョン情報
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// 環境名
        /// </summary>
        public string Environment { get; set; } = "Development";

        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// 設定を検証
        /// </summary>
        public virtual ValidationResult Validate()
        {
            // 基本検証ロジック
            var result = new ValidationResult();

            // データアノテーション属性を使用した検証
            foreach (var property in GetType().GetProperties())
            {
                var validationAttributes = property.GetCustomAttributes<ValidationAttribute>(true);
                foreach (var attribute in validationAttributes)
                {
                    var value = property.GetValue(this);
                    var context = new ValidationContext(this) { MemberName = property.Name };

                    try
                    {
                        if (!attribute.IsValid(value))
                        {
                            result.AddError(
                                attribute.FormatErrorMessage(property.Name),
                                property.Name,
                                attribute.GetType().Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.AddError(
                            $"検証中にエラーが発生しました: {ex.Message}",
                            property.Name,
                            "ValidationError");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 設定が有効かどうかを検証（例外をスロー）
        /// </summary>
        public void ValidateAndThrow()
        {
            var result = Validate();
            result.ThrowIfInvalid();
        }

        /// <summary>
        /// カスタム検証ロジックを追加
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <param name="errorCode">エラーコード</param>
        /// <returns>検証結果</returns>
        protected ValidationResult ValidateProperty(
            bool isValid,
            string propertyName,
            string errorMessage,
            string errorCode = "ValidationError")
        {
            var result = new ValidationResult();

            if (!isValid)
            {
                result.AddError(errorMessage, propertyName, errorCode);
            }

            return result;
        }
    }

    /// <summary>
    /// アプリケーション全般の設定クラス
    /// </summary>
    public class ApplicationSettings : AppSettings
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        [Required(ErrorMessage = "アプリケーション名は必須です")]
        public string ApplicationName { get; set; } = "CoreLibアプリケーション";

        /// <summary>
        /// 会社名
        /// </summary>
        public string CompanyName { get; set; } = "";

        /// <summary>
        /// 管理者メールアドレス
        /// </summary>
        [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
        public string AdminEmail { get; set; } = "";

        /// <summary>
        /// セッションタイムアウト（分）
        /// </summary>
        [Range(5, 1440, ErrorMessage = "セッションタイムアウトは5～1440分の範囲で指定してください")]
        public int SessionTimeoutMinutes { get; set; } = 30;

        /// <summary>
        /// デバッグモードの有効化
        /// </summary>
        public bool EnableDebugMode { get; set; } = false;

        /// <summary>
        /// 接続文字列
        /// </summary>
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();

        /// <summary>
        /// 機能フラグ
        /// </summary>
        public FeatureFlags FeatureFlags { get; set; } = new FeatureFlags();

        /// <summary>
        /// 設定を検証
        /// </summary>
        public override ValidationResult Validate()
        {
            // 基本検証
            var result = base.Validate();

            // ConnectionStringsの検証
            if (ConnectionStrings != null)
            {
                var connectionStringsResult = ConnectionStrings.Validate();
                result.AddErrors(connectionStringsResult);
            }

            // カスタム検証
            if (EnableDebugMode && Environment != "Development")
            {
                result.AddError(
                    "本番環境ではデバッグモードを有効にすることはできません",
                    nameof(EnableDebugMode),
                    "InvalidDebugMode");
            }

            return result;
        }
    }


}
