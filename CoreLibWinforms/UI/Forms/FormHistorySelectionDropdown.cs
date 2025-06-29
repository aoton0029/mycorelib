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
    public partial class FormHistorySelectionDropdown : FormDropdownBase
    {
        private ListBox _listHistory;
        private Button _btnClear;

        /// <summary>
        /// 履歴アイテムが選択された時に発生するイベント
        /// </summary>
        public event EventHandler<HistoryItemSelectedEventArgs> ItemSelected;

        /// <summary>
        /// 履歴がクリアされた時に発生するイベント
        /// </summary>
        public event EventHandler HistoryCleared;

        /// <summary>
        /// 履歴リストの最大表示項目数
        /// </summary>
        [DefaultValue(10)]
        [Description("履歴リストの最大表示項目数")]
        [Category("表示")]
        public int MaxVisibleItems { get; set; } = 10;

        /// <summary>
        /// 履歴アイテムのソース
        /// </summary>
        [Browsable(false)]
        public IList<object> HistoryItems
        {
            get => _historyItems;
            set
            {
                _historyItems = value;
                UpdateHistoryList();
            }
        }
        private IList<object> _historyItems = new List<object>();

        public FormHistorySelectionDropdown()
        {
            InitializeComponent();
            InitializeHistoryDropdown();
        }

        private void InitializeHistoryDropdown()
        {
            // ListBoxの初期化
            _listHistory = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                IntegralHeight = true,
                SelectionMode = SelectionMode.One
            };
            _listHistory.MouseDoubleClick += ListHistory_MouseDoubleClick;
            _listHistory.KeyDown += ListHistory_KeyDown;

            // クリアボタンの初期化
            _btnClear = new Button
            {
                Text = "履歴をクリア",
                Dock = DockStyle.Bottom,
                Height = 30,
                FlatStyle = FlatStyle.Flat
            };
            _btnClear.Click += BtnClear_Click;

            // パネルの作成とコントロールの配置
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(1)
            };

            panel.Controls.Add(_listHistory);
            panel.Controls.Add(_btnClear);

            this.Controls.Add(panel);
            this.Size = new Size(250, 300);

            // 親クラスのプロパティ設定
            this.CloseOnOutsideClick = true;
            this.OKOnEnterKey = true;
        }

        /// <summary>
        /// 履歴リストを更新します
        /// </summary>
        private void UpdateHistoryList()
        {
            _listHistory.Items.Clear();

            if (_historyItems == null || _historyItems.Count == 0)
            {
                _listHistory.Items.Add("(履歴はありません)");
                _btnClear.Enabled = false;
            }
            else
            {
                foreach (var item in _historyItems)
                {
                    _listHistory.Items.Add(item);
                }
                _btnClear.Enabled = true;

                // リストボックスの高さを調整
                int itemHeight = _listHistory.ItemHeight;
                int visibleItems = Math.Min(MaxVisibleItems, _historyItems.Count);
                int listHeight = itemHeight * visibleItems;

                // 20pxはスクロールバー用の追加スペース
                this.Height = listHeight + _btnClear.Height + 20;
            }
        }

        private void ListHistory_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = _listHistory.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches && _historyItems.Count > 0)
            {
                SelectItem(index);
            }
        }

        private void ListHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _listHistory.SelectedIndex >= 0 && _historyItems.Count > 0)
            {
                SelectItem(_listHistory.SelectedIndex);
                e.Handled = true;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _historyItems.Clear();
            UpdateHistoryList();
            HistoryCleared?.Invoke(this, EventArgs.Empty);
        }

        private void SelectItem(int index)
        {
            if (index >= 0 && index < _historyItems.Count)
            {
                var selectedItem = _historyItems[index];
                ItemSelected?.Invoke(this, new HistoryItemSelectedEventArgs(selectedItem, index));
                OnOKClicked();
            }
        }

        /// <summary>
        /// 指定したコントロールの下に履歴選択ドロップダウンを表示します
        /// </summary>
        /// <param name="position">表示位置</param>
        /// <param name="targetControl">基準となるコントロール</param>
        /// <param name="historyItems">履歴アイテムのリスト</param>
        public void ShowHistory(Position position, Control targetControl, IList<object> historyItems)
        {
            HistoryItems = historyItems;
            Show(position, targetControl);
        }
    }

    /// <summary>
    /// 履歴アイテム選択イベントの引数
    /// </summary>
    public class HistoryItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// 選択されたアイテム
        /// </summary>
        public object SelectedItem { get; private set; }

        /// <summary>
        /// 選択されたアイテムのインデックス
        /// </summary>
        public int SelectedIndex { get; private set; }

        public HistoryItemSelectedEventArgs(object selectedItem, int selectedIndex)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
        }
    }
}
