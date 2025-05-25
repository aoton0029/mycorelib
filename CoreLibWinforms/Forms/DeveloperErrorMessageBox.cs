using CoreLib.Diagnoctics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLib.Resources;

namespace CoreLibWinforms.Forms
{
    /// <summary>
    /// 開発者向けエラー情報を表示するメッセージボックス
    /// </summary>
    public partial class DeveloperErrorMessageBox : Form
    {
        private static readonly ResourceManager _resourceManager; //.= new ResourceManager(typeof(ErrorMessages));

        private readonly ErrorInfo _errorInfo;
        private bool _detailsVisible = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="errorInfo">エラー情報</param>
        public DeveloperErrorMessageBox(ErrorInfo errorInfo)
        {
            InitializeComponent();
            _errorInfo = errorInfo ?? throw new ArgumentNullException(nameof(errorInfo));

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = true;
            this.ShowInTaskbar = true;
            this.Icon = SystemIcons.Error;

            // フォームの設定
            SetupForm();
        }

        /// <summary>
        /// フォームの表示内容を設定
        /// </summary>
        private void SetupForm()
        {
            // タイトル設定
            this.Text = GetLocalizedMessage("DeveloperErrorTitle");

            // ユーザー向けエラーメッセージを設定
            this.lblUserMessage.Text = _errorInfo.UserMessage;

            // 発生日時を設定
            this.lblTimestamp.Text = string.Format(
                GetLocalizedMessage("ErrorOccurredAt"),
                _errorInfo.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

            // エラーコードがあれば表示
            if (!string.IsNullOrEmpty(_errorInfo.ErrorCode))
            {
                this.lblErrorCode.Text = string.Format(
                    GetLocalizedMessage("ErrorCode"),
                    _errorInfo.ErrorCode);
                this.lblErrorCode.Visible = true;
            }
            else
            {
                this.lblErrorCode.Visible = false;
            }

            // 詳細情報を設定
            this.txtDetails.Text = _errorInfo.GetDeveloperDetails();

            // 最初は詳細を隠す
            this.pnlDetails.Visible = false;
            this.btnToggleDetails.Text = GetLocalizedMessage("ShowDetails");

            // エラーアイコンを設定
            switch (_errorInfo.Severity)
            {
                case ErrorSeverity.Information:
                    this.picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;
                case ErrorSeverity.Warning:
                    this.picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case ErrorSeverity.Critical:
                case ErrorSeverity.Error:
                default:
                    this.picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;
            }

            // ボタンテキストを設定
            this.btnCopy.Text = GetLocalizedMessage("CopyToClipboard");
            this.btnOK.Text = "OK";
        }

        /// <summary>
        /// 詳細表示ボタンのクリックイベント
        /// </summary>
        private void btnToggleDetails_Click(object sender, EventArgs e)
        {
            _detailsVisible = !_detailsVisible;

            // 詳細パネルの表示/非表示を切り替え
            this.pnlDetails.Visible = _detailsVisible;

            // ボタンテキストを変更
            this.btnToggleDetails.Text = _detailsVisible
                ? GetLocalizedMessage("HideDetails")
                : GetLocalizedMessage("ShowDetails");

            // フォームのサイズを調整
            AdjustFormSize();
        }

        /// <summary>
        /// フォームサイズを調整
        /// </summary>
        private void AdjustFormSize()
        {
            if (_detailsVisible)
            {
                // 詳細表示時は高さを拡大
                this.ClientSize = new Size(this.ClientSize.Width, this.pnlDetails.Bottom + this.btnOK.Height + 20);
            }
            else
            {
                // 詳細非表示時は通常サイズ
                this.ClientSize = new Size(this.ClientSize.Width, this.btnToggleDetails.Bottom + this.btnOK.Height + 20);
            }

            // ボタンの位置を調整
            this.btnOK.Location = new Point(
                this.ClientSize.Width / 2 - this.btnOK.Width / 2,
                this.ClientSize.Height - this.btnOK.Height - 10);
        }

        /// <summary>
        /// コピーボタンのクリックイベント
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                // 詳細情報をクリップボードにコピー
                Clipboard.SetText(_errorInfo.GetDeveloperDetails());
                MessageBox.Show(
                    GetLocalizedMessage("CopiedToClipboard"),
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                // クリップボード操作に失敗した場合は何もしない
            }
        }

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
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
