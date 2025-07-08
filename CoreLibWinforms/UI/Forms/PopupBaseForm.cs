using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.UI.Forms
{
    public enum PopupPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        AboveControl,
        BelowControl,
        LeftOfControl,
        RightOfControl
    }

    public partial class PopupBaseForm : Form
    {
        private Control _ownerControl;
        private PopupPosition _position;
        private bool _closeWhenClickedOutside = true;

        public PopupBaseForm()
        {
            // フォームの基本設定
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;

            // 標準的なサイズ設定
            Size = new Size(300, 200);

            // 影をつける
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // フォームの外をクリックしたときの動作をフック
            if (_closeWhenClickedOutside)
            {
                Application.AddMessageFilter(new OutsideClickMessageFilter(this));
            }
        }

        /// <summary>
        /// ポップアップを指定された位置に表示します
        /// </summary>
        public void ShowPopup(Control ownerControl, PopupPosition position)
        {
            _ownerControl = ownerControl;
            _position = position;
            UpdatePosition();
            Show(ownerControl.FindForm());
        }

        /// <summary>
        /// フォームの外側をクリックしたら閉じるかどうかを設定
        /// </summary>
        public void SetCloseWhenClickedOutside(bool closeWhenClickedOutside)
        {
            _closeWhenClickedOutside = closeWhenClickedOutside;
        }

        /// <summary>
        /// ポジションを更新します
        /// </summary>
        private void UpdatePosition()
        {
            if (_ownerControl == null) return;

            Point ownerPoint = _ownerControl.PointToScreen(Point.Empty);
            Point newLocation = Point.Empty;

            // 親フォームの四隅またはコントロールに対する相対位置の計算
            switch (_position)
            {
                case PopupPosition.TopLeft:
                    Form parentForm = _ownerControl.FindForm();
                    if (parentForm != null)
                    {
                        newLocation = parentForm.PointToScreen(Point.Empty);
                    }
                    break;
                case PopupPosition.TopRight:
                    parentForm = _ownerControl.FindForm();
                    if (parentForm != null)
                    {
                        newLocation = parentForm.PointToScreen(new Point(parentForm.ClientSize.Width - Width, 0));
                    }
                    break;
                case PopupPosition.BottomLeft:
                    parentForm = _ownerControl.FindForm();
                    if (parentForm != null)
                    {
                        newLocation = parentForm.PointToScreen(new Point(0, parentForm.ClientSize.Height - Height));
                    }
                    break;
                case PopupPosition.BottomRight:
                    parentForm = _ownerControl.FindForm();
                    if (parentForm != null)
                    {
                        newLocation = parentForm.PointToScreen(new Point(parentForm.ClientSize.Width - Width, parentForm.ClientSize.Height - Height));
                    }
                    break;
                case PopupPosition.AboveControl:
                    newLocation = new Point(ownerPoint.X, ownerPoint.Y - Height);
                    break;
                case PopupPosition.BelowControl:
                    newLocation = new Point(ownerPoint.X, ownerPoint.Y + _ownerControl.Height);
                    break;
                case PopupPosition.LeftOfControl:
                    newLocation = new Point(ownerPoint.X - Width, ownerPoint.Y);
                    break;
                case PopupPosition.RightOfControl:
                    newLocation = new Point(ownerPoint.X + _ownerControl.Width, ownerPoint.Y);
                    break;
            }

            Location = newLocation;
        }

        /// <summary>
        /// フォーム外のクリックを検知するメッセージフィルター
        /// </summary>
        private class OutsideClickMessageFilter : IMessageFilter
        {
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_RBUTTONDOWN = 0x0204;
            private readonly PopupBaseForm _form;

            public OutsideClickMessageFilter(PopupBaseForm form)
            {
                _form = form;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if ((m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN) && _form.Visible)
                {
                    Point cursorPos = Cursor.Position;
                    if (!_form.Bounds.Contains(cursorPos))
                    {
                        _form.Close();
                        return true;
                    }
                }
                return false;
            }
        }
    }

    // 通知用ポップアップの例
    public class NotificationPopup : PopupBaseForm
    {
        private Label messageLabel;

        public NotificationPopup(string message)
        {
            InitializeComponents();
            SetMessage(message);
        }

        private void InitializeComponents()
        {
            this.Size = new Size(300, 100);
            this.BackColor = Color.FromArgb(230, 230, 250);

            messageLabel = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular)
            };

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(messageLabel);

            Button closeButton = new Button
            {
                Text = "×",
                Size = new Size(25, 25),
                Location = new Point(this.Width - 30, 5),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => Close();

            this.Controls.Add(mainPanel);
            this.Controls.Add(closeButton);
        }

        public void SetMessage(string message)
        {
            messageLabel.Text = message;
        }
    }

    // 選択肢を提示するポップアップの例
    public class SelectionPopup : PopupBaseForm
    {
        private ListBox optionsList;
        public event EventHandler<string> ItemSelected;

        public SelectionPopup()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(200, 250);
            this.BackColor = Color.White;

            optionsList = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.None
            };

            optionsList.SelectedIndexChanged += (s, e) =>
            {
                if (optionsList.SelectedItem != null)
                {
                    ItemSelected?.Invoke(this, optionsList.SelectedItem.ToString());
                    this.Close();
                }
            };

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(optionsList);

            this.Controls.Add(mainPanel);
        }

        public void SetOptions(string[] options)
        {
            optionsList.Items.Clear();
            optionsList.Items.AddRange(options);
        }
    }

    // ファイル内容表示用ポップアップの例
    public class FileContentPopup : PopupBaseForm
    {
        private RichTextBox contentTextBox;

        public FileContentPopup()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Size = new Size(500, 400);
            this.BackColor = Color.White;

            contentTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 9F, FontStyle.Regular),
                BorderStyle = BorderStyle.None
            };

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(contentTextBox);

            Button closeButton = new Button
            {
                Text = "閉じる",
                Size = new Size(80, 30),
                Location = new Point(this.Width - 90, this.Height - 40),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            closeButton.Click += (s, e) => Close();

            this.Controls.Add(mainPanel);
            this.Controls.Add(closeButton);
        }

        public void SetContent(string content, string title = null)
        {
            contentTextBox.Text = content;
            if (!string.IsNullOrEmpty(title))
            {
                this.Text = title;
            }
        }
    }
}
