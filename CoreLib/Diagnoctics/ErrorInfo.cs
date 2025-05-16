using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Diagnoctics
{
    /// <summary>
    /// エラーの深刻度
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>
        /// 情報
        /// </summary>
        Information,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// エラー
        /// </summary>
        Error,

        /// <summary>
        /// 重大なエラー
        /// </summary>
        Critical
    }

    /// <summary>
    /// エラー情報を格納するクラス
    /// </summary>
    public class ErrorInfo
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(typeof(Resources.ErrorMessages));

        /// <summary>
        /// ユーザー向けエラーメッセージ
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// エラーコード
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// エラーの発生日時
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// エラーの原因となった例外
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// エラーの発生源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 追加情報（キーと値のペア）
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// エラーの深刻度
        /// </summary>
        public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ErrorInfo() { }

        /// <summary>
        /// メッセージと例外からエラー情報を初期化するコンストラクタ
        /// </summary>
        public ErrorInfo(string userMessage, Exception ex = null)
        {
            UserMessage = userMessage;
            Exception = ex;
            if (ex != null)
            {
                Source = ex.Source;
            }
        }

        /// <summary>
        /// 追加データを設定
        /// </summary>
        public ErrorInfo WithData(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            AdditionalData[key] = value;
            return this;
        }

        /// <summary>
        /// エラーコードを設定
        /// </summary>
        public ErrorInfo WithErrorCode(string errorCode)
        {
            ErrorCode = errorCode;
            return this;
        }

        /// <summary>
        /// エラーの発生源を設定
        /// </summary>
        public ErrorInfo WithSource(string source)
        {
            Source = source;
            return this;
        }

        /// <summary>
        /// エラーの深刻度を設定
        /// </summary>
        public ErrorInfo WithSeverity(ErrorSeverity severity)
        {
            Severity = severity;
            return this;
        }

        /// <summary>
        /// 開発者向けの詳細情報を取得
        /// </summary>
        public string GetDeveloperDetails()
        {
            var sb = new StringBuilder();

            // 共通のエラー情報
            sb.AppendLine(GetLocalizedMessage("ErrorReport"));
            sb.AppendLine("".PadLeft(50, '='));
            sb.AppendLine(string.Format(GetLocalizedMessage("ErrorOccurredAt"), Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")));

            if (!string.IsNullOrEmpty(UserMessage))
                sb.AppendLine($"Message: {UserMessage}");

            if (!string.IsNullOrEmpty(ErrorCode))
                sb.AppendLine(string.Format(GetLocalizedMessage("ErrorCode"), ErrorCode));

            if (!string.IsNullOrEmpty(Source))
                sb.AppendLine(string.Format(GetLocalizedMessage("ErrorSource"), Source));

            sb.AppendLine($"Severity: {Severity}");

            // 例外情報
            if (Exception != null)
            {
                sb.AppendLine("".PadLeft(50, '-'));
                sb.AppendLine($"Exception Type: {Exception.GetType().FullName}");
                sb.AppendLine($"Exception Message: {Exception.Message}");

                if (Exception.InnerException != null)
                    sb.AppendLine($"Inner Exception: {Exception.InnerException.Message}");

                if (Exception.StackTrace != null)
                {
                    sb.AppendLine("".PadLeft(50, '-'));
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(Exception.StackTrace);
                }
            }

            // 追加データ
            if (AdditionalData.Count > 0)
            {
                sb.AppendLine("".PadLeft(50, '-'));
                sb.AppendLine("Additional Data:");
                foreach (var kvp in AdditionalData)
                {
                    sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// リソースからローカライズされたメッセージを取得
        /// </summary>
        private static string GetLocalizedMessage(string key)
        {
            string message = _resourceManager.GetString(key, CultureInfo.CurrentUICulture);
            return message ?? key;
        }
    }
}
