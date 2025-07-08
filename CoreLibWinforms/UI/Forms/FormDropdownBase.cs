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
            //Closing?.Invoke(this, e);
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


ありがとうございます。要件をより理解できました。WinFormsでドロップダウンとトースト通知の両方の特性を持つベースフォームを作成します。以下の機能を実装します：

1. コントロールの上下左右に接して表示できる
2. フォームの四隅に表示できる
3. ドロップダウン以外をクリックしたら消える
4. 明示的にクローズするまで表示される
5. 通知、選択肢の提示、ファイルの内容の表示などに使用可能

それでは、C#でのコード実装例を示します：

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomPopupForm
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

    public class PopupBaseForm : Form
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
```

## 使用例

このベースフォームを使用する方法の例を以下に示します：

```csharp
// フォームのボタンクリックイベントなどで使用
private void showNotificationButton_Click(object sender, EventArgs e)
{
    // 通知ポップアップの表示
    NotificationPopup notification = new NotificationPopup("これは通知メッセージです。");
    notification.ShowPopup(showNotificationButton, PopupPosition.BelowControl);
}

private void showSelectionButton_Click(object sender, EventArgs e)
{
    // 選択肢ポップアップの表示
    SelectionPopup selection = new SelectionPopup();
    selection.SetOptions(new string[] { "選択肢1", "選択肢2", "選択肢3" });
    selection.ItemSelected += (s, selected) => MessageBox.Show($"選択: {selected}");
    selection.ShowPopup(showSelectionButton, PopupPosition.BelowControl);
}

private void showFileContentButton_Click(object sender, EventArgs e)
{
    // ファイル内容ポップアップの表示
    try
    {
        string content = File.ReadAllText(@"C:\example\sample.txt");
        FileContentPopup filePopup = new FileContentPopup();
        filePopup.SetContent(content, "サンプルファイル");
        filePopup.ShowPopup(showFileContentButton, PopupPosition.RightOfControl);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"エラー: {ex.Message}");
    }
}
```

## 機能の詳細説明

1. **PopupBaseForm**: すべてのポップアップの基本となるクラスです。
   - さまざまな位置（コントロールの上下左右、フォームの四隅）に表示可能
   - フォーム外をクリックすると閉じる機能
   - 表示位置を動的に調整

2. **通知用ポップアップ(NotificationPopup)**: 
   - シンプルなメッセージを表示
   - 閉じるボタン付き
   - カスタマイズ可能なデザイン

3. **選択肢提示用ポップアップ(SelectionPopup)**:
   - リスト形式で選択肢を表示
   - 選択項目をイベント経由で取得可能
   - ドロップダウンメニューのような動作

4. **ファイル内容表示用ポップアップ(FileContentPopup)**:
   - テキストファイルの内容を表示
   - スクロール可能なテキストエリア
   - タイトル設定機能付き

これらのクラスは必要に応じて拡張でき、デザインもカスタマイズ可能です。背景色、フォント、サイズなどは自由に変更できます。また、アニメーション効果を追加したい場合は、Timer コントロールを使用してフォームのサイズや位置を徐々に変化させることで実装できます。