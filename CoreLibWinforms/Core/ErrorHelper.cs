using CoreLib.Diagnoctics;
using CoreLibWinforms.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    /// <summary>
    /// エラー表示・処理のためのヘルパークラス
    /// </summary>
    public static class ErrorHelper
    {
        // ログディレクトリのパス
        private static readonly string LogDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "YourAppName", "Logs");

        /// <summary>
        /// エラー情報をファイルに記録
        /// </summary>
        /// <param name="errorInfo">エラー情報</param>
        /// <returns>ログファイルのパス</returns>
        public static string LogToFile(ErrorInfo errorInfo)
        {
            if (errorInfo == null)
                throw new ArgumentNullException(nameof(errorInfo));

            try
            {
                // ログディレクトリが存在しない場合は作成
                if (!Directory.Exists(LogDirectory))
                    Directory.CreateDirectory(LogDirectory);

                // ファイル名を日時とGUIDで生成
                var timestamp = DateTime.Now;
                string fileName = $"Error_{timestamp:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.log";
                string filePath = Path.Combine(LogDirectory, fileName);

                // ファイルに書き込み
                File.WriteAllText(filePath, errorInfo.GetDeveloperDetails());

                return filePath;
            }
            catch
            {
                // ファイル書き込みに失敗した場合はnullを返す
                return null;
            }
        }

        /// <summary>
        /// ユーザー向けエラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowError(string message)
        {
            return MessageBoxExHelper.ShowError(message);
        }

        /// <summary>
        /// 開発者向けエラー情報を表示
        /// </summary>
        /// <param name="errorInfo">エラー情報</param>
        public static void ShowDeveloperError(ErrorInfo errorInfo)
        {
            if (errorInfo == null)
                throw new ArgumentNullException(nameof(errorInfo));

            //using (var form = new DeveloperErrorMessageBox(errorInfo))
            //{
            //    form.ShowDialog();
            //}
        }

        /// <summary>
        /// エラー情報を表示し、ファイルに記録
        /// </summary>
        /// <param name="errorInfo">エラー情報</param>
        /// <param name="showUserDialog">ユーザー向けダイアログを表示するかどうか</param>
        /// <param name="showDeveloperDialog">開発者向けダイアログを表示するかどうか</param>
        /// <param name="logger">ロガー（省略可）</param>
        public static void HandleError(
            ErrorInfo errorInfo,
            bool showUserDialog = true,
            bool showDeveloperDialog = false,
            ILogger logger = null)
        {
            if (errorInfo == null)
                throw new ArgumentNullException(nameof(errorInfo));

            // ログにエラーを記録
            string logFilePath = LogToFile(errorInfo);

            if (logFilePath != null)
            {
                errorInfo.WithData("LogFile", logFilePath);
            }

            // ロガーが提供されている場合はログに記録
            if (logger != null)
            {
                LogLevel level = LogLevel.Error;
                switch (errorInfo.Severity)
                {
                    case ErrorSeverity.Information:
                        level = LogLevel.Information;
                        break;
                    case ErrorSeverity.Warning:
                        level = LogLevel.Warning;
                        break;
                    case ErrorSeverity.Critical:
                        level = LogLevel.Critical;
                        break;
                }

                if (errorInfo.Exception != null)
                {
                    logger.Log(level, errorInfo.Exception, errorInfo.UserMessage);
                }
                else
                {
                    logger.Log(level, errorInfo.UserMessage);
                }
            }

            // ユーザー向けダイアログを表示
            if (showUserDialog && !string.IsNullOrEmpty(errorInfo.UserMessage))
            {
                ShowError(errorInfo.UserMessage);
            }

            // 開発者向けダイアログを表示
            if (showDeveloperDialog)
            {
                ShowDeveloperError(errorInfo);
            }
        }

        /// <summary>
        /// 例外を処理し、エラーメッセージを表示
        /// </summary>
        /// <param name="ex">例外</param>
        /// <param name="userMessage">ユーザー向けメッセージ</param>
        /// <param name="showUserDialog">ユーザー向けダイアログを表示するかどうか</param>
        /// <param name="showDeveloperDialog">開発者向けダイアログを表示するかどうか</param>
        /// <param name="logger">ロガー（省略可）</param>
        public static void HandleException(
            Exception ex,
            string userMessage,
            bool showUserDialog = true,
            bool showDeveloperDialog = false,
            ILogger logger = null)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            var errorInfo = new ErrorInfo(userMessage, ex);
            HandleError(errorInfo, showUserDialog, showDeveloperDialog, logger);
        }

        /// <summary>
        /// try-catch内でエラーをハンドリングする
        /// </summary>
        /// <param name="action">実行するアクション</param>
        /// <param name="userMessage">エラー時のユーザー向けメッセージ</param>
        /// <param name="showUserDialog">ユーザー向けダイアログを表示するかどうか</param>
        /// <param name="showDeveloperDialog">開発者向けダイアログを表示するかどうか</param>
        /// <param name="logger">ロガー（省略可）</param>
        /// <returns>アクションが成功したかどうか</returns>
        public static bool TryCatch(
            Action action,
            string userMessage,
            bool showUserDialog = true,
            bool showDeveloperDialog = false,
            ILogger logger = null)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, userMessage, showUserDialog, showDeveloperDialog, logger);
                return false;
            }
        }

        /// <summary>
        /// 非同期処理内でエラーをハンドリングする
        /// </summary>
        /// <param name="asyncAction">実行する非同期アクション</param>
        /// <param name="userMessage">エラー時のユーザー向けメッセージ</param>
        /// <param name="showUserDialog">ユーザー向けダイアログを表示するかどうか</param>
        /// <param name="showDeveloperDialog">開発者向けダイアログを表示するかどうか</param>
        /// <param name="logger">ロガー（省略可）</param>
        /// <returns>アクションが成功したかどうかを示す非同期タスク</returns>
        public static async Task<bool> TryCatchAsync(
            Func<Task> asyncAction,
            string userMessage,
            bool showUserDialog = true,
            bool showDeveloperDialog = false,
            ILogger logger = null)
        {
            try
            {
                await asyncAction();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, userMessage, showUserDialog, showDeveloperDialog, logger);
                return false;
            }
        }
    }
}
