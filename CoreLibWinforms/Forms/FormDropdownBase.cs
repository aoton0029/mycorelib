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
    public enum Position
    {
        Below,
        Above,
        Right,
        Left,
        Centered
    }
    
    public partial class FormDropdownBase : Form
    {
        private Control _targetControl;

        #region イベント

        /// <summary>
        /// OKボタンがクリックされた時に発生するイベント
        /// </summary>
        public event EventHandler OKClicked;

        /// <summary>
        /// キャンセルボタンがクリックされた時に発生するイベント
        /// </summary>
        public event EventHandler CancelClicked;
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
        
        public FormDropdownBase()
        {
            InitializeComponent();
            
        }

        #region パブリックメソッド

        public void Show(Position position, Control targetControl)
        {
            _targetControl = targetControl;
            Form hostForm = targetControl.FindForm();
            hostForm.SizeChanged += (s, e) => reLocate(targetControl);
            hostForm.LocationChanged += (s, e) => reLocate(targetControl);
            switch (position)
            {
                case Position.Below:
                    ShowBelowControl(targetControl);
                    break;
                case Position.Above:
                    ShowAboveControl(targetControl);
                    break;
                case Position.Right:
                    ShowNextToControl(targetControl);
                    break;
                case Position.Left:
                    ShowLeftOfControl(targetControl);
                    break;
                case Position.Centered:
                    ShowCenteredOnControl(targetControl);
                    break;
            }
            reLocate(targetControl);
            Show();
        }
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
        public void ShowAtMousePosition()
        {
            Point mousePos = Control.MousePosition;
            ShowAtLocation(null, mousePos);
        }

        /// <summary>
        /// 親コントロールの指定位置に表示します
        /// </summary>
        /// <param name="parentControl">親となるコントロール</param>
        /// <param name="location">表示位置（スクリーン座標）</param>
        public void ShowAtLocation(Control targetControl, Point location)
        {
            _targetControl = targetControl;

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "";

            Location = location;
            Size = this.Size;
            TopMost = true;

        }

        private void reLocate(Control targetControl)
        {
            // 画面外にはみ出さないように位置を調整
            Rectangle screenBounds = Screen.FromControl(targetControl).WorkingArea;
            if (this.Right > screenBounds.Right)
            {
                this.Left = screenBounds.Right - this.Width;
            }
            if (this.Bottom > screenBounds.Bottom)
            {
                this.Top = screenBounds.Bottom - this.Height;
            }
            if (this.Left < screenBounds.Left)
            {
                this.Left = screenBounds.Left;
            }
            if (this.Top < screenBounds.Top)
            {
                this.Top = screenBounds.Top;
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
                Close();
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

        private void FormDropdownBase_KeyDown(object sender, KeyEventArgs e)
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
        }

        private void FormDropdownBase_Deactivate(object sender, EventArgs e)
        {
            Close();
        }
    }
}
