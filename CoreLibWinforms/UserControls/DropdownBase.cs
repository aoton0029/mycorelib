using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.UserControls
{
    public partial class DropdownBase : UserControl
    {
        #region イベント

        /// <summary>
        /// OKボタンがクリックされた時に発生するイベント
        /// </summary>
        public event EventHandler OKClicked;

        /// <summary>
        /// キャンセルボタンがクリックされた時に発生するイベント
        /// </summary>
        public event EventHandler CancelClicked;

        /// <summary>
        /// コントロールが閉じる前に発生するイベント
        /// </summary>
        public event CancelEventHandler Closing;

        #endregion

        #region プロパティ

        /// <summary>
        /// コントロール外をクリックした時に閉じるかどうか
        /// </summary>
        [DefaultValue(true)]
        [Description("コントロール外をクリックした時に閉じるかどうか")]
        [Category("動作")]
        public bool CloseOnOutsideClick { get; set; } = true;

        /// <summary>
        /// キャンセル時に自動的に閉じるかどうか
        /// </summary>
        [DefaultValue(true)]
        [Description("キャンセル時に自動的に閉じるかどうか")]
        [Category("動作")]
        public bool CloseOnCancel { get; set; } = true;

        /// <summary>
        /// OK時に自動的に閉じるかどうか
        /// </summary>
        [DefaultValue(true)]
        [Description("OK時に自動的に閉じるかどうか")]
        [Category("動作")]
        public bool CloseOnOK { get; set; } = true;

        /// <summary>
        /// ESCキーでキャンセル扱いにするかどうか
        /// </summary>
        [DefaultValue(true)]
        [Description("ESCキーでキャンセル扱いにするかどうか")]
        [Category("動作")]
        public bool CancelOnEscKey { get; set; } = true;

        /// <summary>
        /// Enterキーでキャンセル扱いにするかどうか
        /// </summary>
        [DefaultValue(false)]
        [Description("EnterキーでOK扱いにするかどうか")]
        [Category("動作")]
        public bool OKOnEnterKey { get; set; } = false;

        #endregion

        #region プライベート変数

        private Form _host;
        private Control _parentControl;
        private bool _isShown = false;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DropdownBase()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Visible = false;
        }

        #region パブリックメソッド

        /// <summary>
        /// 指定したコントロールの下に表示します
        /// </summary>
        /// <param name="targetControl">基準となるコントロール</param>
        public void ShowBelowControl(Control targetControl)
        {
            Point location = targetControl.PointToScreen(new Point(0, targetControl.Height));
            ShowAtLocation(targetControl, location);
        }

        /// <summary>
        /// 指定したコントロールの横に表示します
        /// </summary>
        /// <param name="targetControl">基準となるコントロール</param>
        public void ShowNextToControl(Control targetControl)
        {
            Point location = targetControl.PointToScreen(new Point(targetControl.Width, 0));
            ShowAtLocation(targetControl, location);
        }

        /// <summary>
        /// コントロールの上にドロップダウンを表示します
        /// </summary>
        /// <param name="dropdown">ドロップダウンコントロール</param>
        /// <param name="targetControl">基準となるコントロール</param>
        private void ShowAboveControl(Control targetControl)
        {
            Point location = targetControl.PointToScreen(new Point(0, -Height));
            ShowAtLocation(targetControl, location);
        }

        /// <summary>
        /// コントロールの左側にドロップダウンを表示します
        /// </summary>
        /// <param name="dropdown">ドロップダウンコントロール</param>
        /// <param name="targetControl">基準となるコントロール</param>
        private void ShowLeftOfControl(Control targetControl)
        {
            Point location = targetControl.PointToScreen(new Point(-this.Width, 0));
            ShowAtLocation(targetControl, location);
        }

        /// <summary>
        /// コントロールの中央にドロップダウンを表示します
        /// </summary>
        /// <param name="dropdown">ドロップダウンコントロール</param>
        /// <param name="targetControl">基準となるコントロール</param>
        private void ShowCenteredOnControl(Control targetControl)
        {
            int x = (targetControl.Width - Width) / 2;
            int y = (targetControl.Height - Height) / 2;
            Point location = targetControl.PointToScreen(new Point(x, y));
            ShowAtLocation(targetControl, location);
        }

        /// <summary>
        /// マウスカーソルの位置にドロップダウンを表示します
        /// </summary>
        /// <typeparam name="T">DropdownBaseを継承したコントロールの型</typeparam>
        /// <param name="dropdown">ドロップダウンコントロール</param>
        /// <param name="parentControl">親コントロール</param>
        public void ShowAtMousePosition<T>(Control parentControl)
            where T : DropdownBase
        {
            Point mousePos = Control.MousePosition;
            ShowAtLocation(parentControl, mousePos);
        }

        /// <summary>
        /// 親コントロールの指定位置に表示します
        /// </summary>
        /// <param name="parentControl">親となるコントロール</param>
        /// <param name="location">表示位置（スクリーン座標）</param>
        public void ShowAtLocation(Control parentControl, Point location)
        {
            if (_isShown)
                return;

            _parentControl = parentControl;

            // フォームを作成
            _host = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                Location = location,
                Size = this.Size,
                TopMost = true
            };

            // 画面外にはみ出さないように位置を調整
            Rectangle screenBounds = Screen.FromControl(parentControl).WorkingArea;
            if (_host.Right > screenBounds.Right)
            {
                _host.Left = screenBounds.Right - _host.Width;
            }
            if (_host.Bottom > screenBounds.Bottom)
            {
                _host.Top = screenBounds.Bottom - _host.Height;
            }
            if (_host.Left < screenBounds.Left)
            {
                _host.Left = screenBounds.Left;
            }
            if (_host.Top < screenBounds.Top)
            {
                _host.Top = screenBounds.Top;
            }

            // コントロールをフォームに追加
            _host.Controls.Add(this);
            this.Dock = DockStyle.Fill;
            this.Visible = true;

            // フォームのDeactivatedイベントを処理
            _host.Deactivate += (s, e) =>
            {
                if (CloseOnOutsideClick)
                {
                    Close(true);
                }
            };

            // キー入力処理
            _host.KeyPreview = true;
            _host.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && CancelOnEscKey)
                {
                    OnCancelClicked();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && OKOnEnterKey)
                {
                    OnOKClicked();
                    e.Handled = true;
                }
            };

            // フォームを表示
            _isShown = true;
            _host.Show();
            this.Focus();
        }

        /// <summary>
        /// コントロールを閉じます
        /// </summary>
        /// <param name="isCancel">キャンセル扱いかどうか</param>
        public void Close(bool isCancel = false)
        {
            if (!_isShown)
                return;

            // Closingイベント発生
            CancelEventArgs args = new CancelEventArgs();
            OnClosing(args);

            if (args.Cancel)
            {
                return;
            }

            _isShown = false;

            if (_host != null && !_host.IsDisposed)
            {
                this.Visible = false;
                _host.Controls.Remove(this);
                _host.Close();
                _host.Dispose();
                _host = null;
            }
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// OKボタンがクリックされた時の処理
        /// </summary>
        protected virtual void OnOKClicked()
        {
            OKClicked?.Invoke(this, EventArgs.Empty);

            if (CloseOnOK)
            {
                Close();
            }
        }

        /// <summary>
        /// キャンセルボタンがクリックされた時の処理
        /// </summary>
        protected virtual void OnCancelClicked()
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);

            if (CloseOnCancel)
            {
                Close(true);
            }
        }

        /// <summary>
        /// コントロールが閉じる前の処理
        /// </summary>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        #endregion
    }
}
