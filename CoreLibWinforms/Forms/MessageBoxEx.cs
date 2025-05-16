using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.Forms
{
    /// <summary>
    /// メッセージボックスの種類
    /// </summary>
    public enum MessageBoxExType
    {
        Information,
        Warning,
        Error,
        Question,
        Success
    }

    /// <summary>
    /// メッセージボックスの結果
    /// </summary>
    public enum MessageBoxExResult
    {
        Ok,
        Cancel,
        Yes,
        No,
        Retry,
        Abort,
        Ignore,
        Continue
    }

    /// <summary>
    /// メッセージボックスのボタン構成
    /// </summary>
    public enum MessageBoxExButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        RetryCancel,
        AbortRetryIgnore,
        ContinueRetryCancel
    }

    /// <summary>
    /// カスタムメッセージボックス
    /// </summary>
    public partial class MessageBoxEx : Form
    {
        private MessageBoxExResult _result = MessageBoxExResult.Cancel;

        /// <summary>
        /// メッセージボックスの結果
        /// </summary>
        public MessageBoxExResult Result => _result;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MessageBoxEx()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ControlBox = true;
        }

        /// <summary>
        /// メッセージボックスを設定して表示
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttons">ボタン構成</param>
        /// <param name="type">メッセージボックスタイプ</param>
        public MessageBoxExResult ShowDialog(string message, string title, MessageBoxExButtons buttons, MessageBoxExType type)
        {
            // タイトルの設定
            this.Text = title;

            // メッセージの設定
            this.lblMessage.Text = message;

            // アイコンの設定
            SetIconByType(type);

            // ボタンの設定
            SetupButtons(buttons);

            // フォームサイズの自動調整
            AdjustFormSize();

            // モーダルダイアログとして表示
            base.ShowDialog();

            // 結果を返す
            return _result;
        }

        /// <summary>
        /// アイコン画像を種類に応じて設定
        /// </summary>
        private void SetIconByType(MessageBoxExType type)
        {
            switch (type)
            {
                case MessageBoxExType.Information:
                    this.picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxExType.Warning:
                    this.picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxExType.Error:
                    this.picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;
                case MessageBoxExType.Question:
                    this.picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;
                case MessageBoxExType.Success:
                    // 成功アイコンはSystemIconsにないため、情報アイコンを代用
                    this.picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;
            }
        }

        /// <summary>
        /// ボタン構成に応じてボタンを設定
        /// </summary>
        private void SetupButtons(MessageBoxExButtons buttons)
        {
            // ボタンをすべて非表示にする
            this.btnOk.Visible = false;
            this.btnCancel.Visible = false;
            this.btnYes.Visible = false;
            this.btnNo.Visible = false;
            this.btnRetry.Visible = false;
            this.btnAbort.Visible = false;
            this.btnIgnore.Visible = false;
            this.btnContinue.Visible = false;

            // ボタン構成に応じてボタンを表示
            switch (buttons)
            {
                case MessageBoxExButtons.Ok:
                    this.btnOk.Visible = true;
                    this.AcceptButton = this.btnOk;
                    this.CancelButton = this.btnOk;
                    break;

                case MessageBoxExButtons.OkCancel:
                    this.btnOk.Visible = true;
                    this.btnCancel.Visible = true;
                    this.AcceptButton = this.btnOk;
                    this.CancelButton = this.btnCancel;
                    break;

                case MessageBoxExButtons.YesNo:
                    this.btnYes.Visible = true;
                    this.btnNo.Visible = true;
                    this.AcceptButton = this.btnYes;
                    this.CancelButton = this.btnNo;
                    break;

                case MessageBoxExButtons.YesNoCancel:
                    this.btnYes.Visible = true;
                    this.btnNo.Visible = true;
                    this.btnCancel.Visible = true;
                    this.AcceptButton = this.btnYes;
                    this.CancelButton = this.btnCancel;
                    break;

                case MessageBoxExButtons.RetryCancel:
                    this.btnRetry.Visible = true;
                    this.btnCancel.Visible = true;
                    this.AcceptButton = this.btnRetry;
                    this.CancelButton = this.btnCancel;
                    break;

                case MessageBoxExButtons.AbortRetryIgnore:
                    this.btnAbort.Visible = true;
                    this.btnRetry.Visible = true;
                    this.btnIgnore.Visible = true;
                    this.AcceptButton = this.btnRetry;
                    this.CancelButton = this.btnAbort;
                    break;

                case MessageBoxExButtons.ContinueRetryCancel:
                    this.btnContinue.Visible = true;
                    this.btnRetry.Visible = true;
                    this.btnCancel.Visible = true;
                    this.AcceptButton = this.btnContinue;
                    this.CancelButton = this.btnCancel;
                    break;
            }

            // ボタンの位置を調整
            RepositionButtons();
        }

        /// <summary>
        /// ボタンの位置を再配置
        /// </summary>
        private void RepositionButtons()
        {
            // 表示されるボタンをリストアップ
            var visibleButtons = new List<Button>();

            foreach (Control c in this.panelButtons.Controls)
            {
                if (c is Button button && button.Visible)
                {
                    visibleButtons.Add(button);
                }
            }

            // ボタンがなければ何もしない
            if (visibleButtons.Count == 0)
                return;

            // ボタン間の間隔
            const int buttonSpacing = 10;

            // ボタンの合計幅
            int totalWidth = visibleButtons.Sum(b => b.Width) + buttonSpacing * (visibleButtons.Count - 1);

            // 開始X位置（パネル中央からボタン群の半分の位置）
            int startX = (this.panelButtons.Width - totalWidth) / 2;

            // ボタンの配置
            for (int i = 0; i < visibleButtons.Count; i++)
            {
                var button = visibleButtons[i];
                button.Location = new Point(startX, button.Location.Y);
                startX += button.Width + buttonSpacing;
            }
        }

        /// <summary>
        /// フォームのサイズを内容に合わせて自動調整
        /// </summary>
        private void AdjustFormSize()
        {
            // メッセージのサイズに合わせてラベルの高さを調整
            Size textSize = TextRenderer.MeasureText(this.lblMessage.Text, this.lblMessage.Font,
                new Size(this.lblMessage.Width, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            this.lblMessage.Height = textSize.Height + 10; // 若干の余裕を持たせる

            // ラベルの位置を調整
            this.lblMessage.Location = new Point(this.lblMessage.Location.X,
                this.picIcon.Location.Y + (this.picIcon.Height - this.lblMessage.Height) / 2);

            // フォームの高さを調整
            int contentHeight = Math.Max(this.picIcon.Bottom, this.lblMessage.Bottom) + 20; // 上部コンテンツの最大高さ + マージン

            // ボタンパネルの位置調整
            this.panelButtons.Location = new Point(this.panelButtons.Location.X, contentHeight);

            // フォーム全体の高さ調整
            this.ClientSize = new Size(this.ClientSize.Width, contentHeight + this.panelButtons.Height);

            // フォームの幅が十分でない場合は調整
            int minWidth = this.lblMessage.Right + 20;
            if (this.ClientSize.Width < minWidth)
            {
                this.ClientSize = new Size(minWidth, this.ClientSize.Height);
            }
        }

        // 各ボタンクリック時の処理
        private void btnOk_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Ok;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Cancel;
            this.Close();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.No;
            this.Close();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Retry;
            this.Close();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Abort;
            this.Close();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Ignore;
            this.Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            this._result = MessageBoxExResult.Continue;
            this.Close();
        }
    }
}
