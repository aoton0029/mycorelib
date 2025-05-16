using CoreLibWinforms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    /// <summary>
    /// 汎用メッセージボックスのヘルパークラス
    /// </summary>
    public static class MessageBoxExHelper
    {
        /// <summary>
        /// 情報メッセージを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowInfo(string message, string title = "情報")
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, MessageBoxExButtons.Ok, MessageBoxExType.Information);
            }
        }

        /// <summary>
        /// 警告メッセージを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowWarning(string message, string title = "警告")
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, MessageBoxExButtons.Ok, MessageBoxExType.Warning);
            }
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowError(string message, string title = "エラー")
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, MessageBoxExButtons.Ok, MessageBoxExType.Error);
            }
        }

        /// <summary>
        /// 成功メッセージを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowSuccess(string message, string title = "成功")
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, MessageBoxExButtons.Ok, MessageBoxExType.Success);
            }
        }

        /// <summary>
        /// 質問メッセージを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttons">表示するボタン</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowQuestion(string message, string title = "確認", MessageBoxExButtons buttons = MessageBoxExButtons.YesNo)
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, buttons, MessageBoxExType.Question);
            }
        }

        /// <summary>
        /// カスタムメッセージボックスを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttons">表示するボタン</param>
        /// <param name="type">メッセージボックスのタイプ</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult Show(string message, string title, MessageBoxExButtons buttons, MessageBoxExType type)
        {
            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, buttons, type);
            }
        }

        /// <summary>
        /// Yes/Noダイアログを表示して結果を論理値で返す
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="title">タイトル</param>
        /// <returns>はい=true、いいえ=false</returns>
        public static bool Confirm(string message, string title = "確認")
        {
            using (var messageBox = new MessageBoxEx())
            {
                var result = messageBox.ShowDialog(message, title, MessageBoxExButtons.YesNo, MessageBoxExType.Question);
                return result == MessageBoxExResult.Yes;
            }
        }

        /// <summary>
        /// 例外メッセージを表示
        /// </summary>
        /// <param name="ex">表示する例外</param>
        /// <param name="title">タイトル</param>
        /// <param name="showStackTrace">スタックトレースを表示するかどうか</param>
        /// <returns>ダイアログの結果</returns>
        public static MessageBoxExResult ShowException(Exception ex, string title = "エラー", bool showStackTrace = false)
        {
            string message = ex.Message;

            if (showStackTrace && ex.StackTrace != null)
            {
                message += Environment.NewLine + Environment.NewLine + "詳細情報:" + Environment.NewLine + ex.StackTrace;
            }

            using (var messageBox = new MessageBoxEx())
            {
                return messageBox.ShowDialog(message, title, MessageBoxExButtons.Ok, MessageBoxExType.Error);
            }
        }

        /// <summary>
        /// 処理待ちダイアログを表示
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="action">実行するアクション</param>
        /// <param name="canCancel">キャンセル可能かどうか</param>
        /// <returns>キャンセルされたかどうか</returns>
        public static bool ShowWithProgress(string message, Action<ProgressReporter> action, bool canCancel = true)
        {
            bool cancelled = false;

            using (var form = new FormLoading(message, canCancel))
            {
                var reporter = new ProgressReporter(form);

                Task.Run(() => {
                    try
                    {
                        action(reporter);
                    }
                    catch (Exception ex)
                    {
                        form.Invoke((MethodInvoker)delegate {
                            form.Close();
                            ShowException(ex);
                        });
                    }
                    finally
                    {
                        form.Invoke((MethodInvoker)delegate {
                            form.Close();
                        });
                    }
                });

                form.ShowDialog();
                cancelled = form._cts.IsCancellationRequested;
            }

            return cancelled;
        }
    }
}
