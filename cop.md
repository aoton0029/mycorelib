以下に、TextBoxの編集時にPopupBaseFormを使用して履歴選択ができるようなコードの実装例を示します。テキスト入力履歴をドロップダウンのように表示し、過去の入力を選択できる機能を実装します。

```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CustomPopupForm
{
    /// <summary>
    /// 履歴選択用のポップアップ
    /// </summary>
    public class HistorySelectionPopup : PopupBaseForm
    {
        private ListBox historyListBox;
        private Button clearHistoryButton;
        private Panel mainPanel;
        
        // 履歴選択イベント
        public event EventHandler<string> HistorySelected;

        private const int MaxHistoryItems = 10; // 最大履歴数

        public HistorySelectionPopup()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // フォームの設定
            this.Size = new Size(250, 200);
            this.BackColor = Color.White;

            // リストボックスの設定
            historyListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Yu Gothic UI", 9F),
                BorderStyle = BorderStyle.None,
                IntegralHeight = false // 高さを自由に調整できるようにする
            };

            historyListBox.MouseDoubleClick += (s, e) => SelectCurrentItem();
            historyListBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SelectCurrentItem();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Close();
                    e.Handled = true;
                }
            };

            // 履歴クリアボタン
            clearHistoryButton = new Button
            {
                Text = "履歴をクリア",
                Dock = DockStyle.Bottom,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Yu Gothic UI", 9F),
                Cursor = Cursors.Hand
            };
            clearHistoryButton.FlatAppearance.BorderSize = 0;
            clearHistoryButton.Click += (s, e) =>
            {
                if (MessageBox.Show("入力履歴をクリアしますか？", "確認",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ClearHistory();
                }
            };

            // パネルの設定
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(1)
            };

            // コントロールを追加
            mainPanel.Controls.Add(historyListBox);
            this.Controls.Add(mainPanel);
            this.Controls.Add(clearHistoryButton);
        }

        /// <summary>
        /// 現在選択されている項目を選択して閉じる
        /// </summary>
        private void SelectCurrentItem()
        {
            if (historyListBox.SelectedItem != null)
            {
                string selected = historyListBox.SelectedItem.ToString();
                HistorySelected?.Invoke(this, selected);
                Close();
            }
        }

        /// <summary>
        /// 履歴をロードして表示
        /// </summary>
        public void LoadHistory(string historyCategory)
        {
            List<string> history = LoadHistoryFromStorage(historyCategory);
            DisplayHistory(history);
        }

        /// <summary>
        /// 履歴を表示
        /// </summary>
        private void DisplayHistory(List<string> history)
        {
            historyListBox.Items.Clear();
            foreach (string item in history)
            {
                historyListBox.Items.Add(item);
            }

            // 履歴がない場合はメッセージを表示
            if (historyListBox.Items.Count == 0)
            {
                historyListBox.Items.Add("(履歴はありません)");
                historyListBox.Enabled = false;
                clearHistoryButton.Enabled = false;
            }
            else
            {
                historyListBox.Enabled = true;
                clearHistoryButton.Enabled = true;
                historyListBox.SelectedIndex = 0; // 最初の項目を選択
            }

            // リストボックスの高さを調整
            AdjustSize();
        }

        /// <summary>
        /// 履歴をストレージからロード
        /// </summary>
        private List<string> LoadHistoryFromStorage(string category)
        {
            string fileName = GetHistoryFileName(category);
            if (File.Exists(fileName))
            {
                try
                {
                    return File.ReadAllLines(fileName).ToList();
                }
                catch
                {
                    // ファイル読み込みに失敗した場合は空のリストを返す
                    return new List<string>();
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// 履歴をストレージに保存
        /// </summary>
        private void SaveHistoryToStorage(List<string> history, string category)
        {
            string fileName = GetHistoryFileName(category);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllLines(fileName, history);
            }
            catch
            {
                // 保存に失敗しても続行する
            }
        }

        /// <summary>
        /// 履歴ファイル名を取得
        /// </summary>
        private string GetHistoryFileName(string category)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string historyDir = Path.Combine(appDataPath, "YourAppName", "History");
            return Path.Combine(historyDir, $"{category}_history.txt");
        }

        /// <summary>
        /// 履歴を追加
        /// </summary>
        public void AddHistory(string item, string category)
        {
            if (string.IsNullOrWhiteSpace(item)) return;

            List<string> history = LoadHistoryFromStorage(category);

            // 既に同じ項目が存在する場合は削除（重複を防ぐため）
            history.RemoveAll(x => x == item);

            // 履歴の先頭に追加
            history.Insert(0, item);

            // 最大数を超える場合は古いものを削除
            if (history.Count > MaxHistoryItems)
            {
                history = history.Take(MaxHistoryItems).ToList();
            }

            // 保存して表示を更新
            SaveHistoryToStorage(history, category);
            DisplayHistory(history);
        }

        /// <summary>
        /// 履歴をクリア
        /// </summary>
        public void ClearHistory(string category = null)
        {
            if (category != null)
            {
                SaveHistoryToStorage(new List<string>(), category);
            }
            historyListBox.Items.Clear();
            historyListBox.Items.Add("(履歴はありません)");
            historyListBox.Enabled = false;
            clearHistoryButton.Enabled = false;
        }

        /// <summary>
        /// リストボックスのサイズを調整
        /// </summary>
        private void AdjustSize()
        {
            int itemCount = Math.Min(historyListBox.Items.Count, 10); // 最大10項目まで表示
            int itemHeight = historyListBox.ItemHeight;
            
            // リストボックスの高さを計算
            int listHeight = itemCount * itemHeight + 4;
            
            // フォームの高さを計算（余白やボタンの高さを考慮）
            this.Height = listHeight + clearHistoryButton.Height + 10;
        }

        /// <summary>
        /// フィルター処理
        /// </summary>
        public void FilterHistory(string filter, string category)
        {
            List<string> allHistory = LoadHistoryFromStorage(category);
            
            // フィルターが空の場合は全表示
            if (string.IsNullOrWhiteSpace(filter))
            {
                DisplayHistory(allHistory);
                return;
            }

            // フィルターに一致する項目を表示
            List<string> filtered = allHistory.Where(item => 
                item.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            
            DisplayHistory(filtered);
        }
    }

    /// <summary>
    /// 履歴機能付きテキストボックス
    /// </summary>
    public class HistoryTextBox : TextBox
    {
        private HistorySelectionPopup historyPopup;
        private string historyCategory;
        private bool isHistoryVisible = false;

        public HistoryTextBox() : base()
        {
            this.historyCategory = this.Name; // デフォルトはコントロール名を使用
            
            // イベントハンドラの設定
            this.Click += HistoryTextBox_Click;
            this.KeyDown += HistoryTextBox_KeyDown;
            this.TextChanged += HistoryTextBox_TextChanged;
            this.LostFocus += HistoryTextBox_LostFocus;
            
            // 履歴ポップアップの初期化
            InitializeHistoryPopup();
        }

        /// <summary>
        /// 履歴カテゴリを設定
        /// </summary>
        public void SetHistoryCategory(string category)
        {
            this.historyCategory = category;
        }

        /// <summary>
        /// 履歴ポップアップの初期化
        /// </summary>
        private void InitializeHistoryPopup()
        {
            historyPopup = new HistorySelectionPopup();
            historyPopup.HistorySelected += (s, text) =>
            {
                this.Text = text;
                this.SelectionStart = this.Text.Length;
                this.Focus();
            };
            
            // クリック時に自動的に閉じる設定
            historyPopup.SetCloseWhenClickedOutside(true);
        }

        /// <summary>
        /// クリックイベント - 履歴表示
        /// </summary>
        private void HistoryTextBox_Click(object sender, EventArgs e)
        {
            ShowHistoryPopup();
        }

        /// <summary>
        /// キーダウンイベント - 特殊キー処理
        /// </summary>
        private void HistoryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (!isHistoryVisible)
                    {
                        ShowHistoryPopup();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Escape:
                    if (isHistoryVisible)
                    {
                        HideHistoryPopup();
                        e.Handled = true;
                    }
                    break;
                    
                case Keys.Enter:
                    if (isHistoryVisible)
                    {
                        HideHistoryPopup();
                        AddCurrentTextToHistory();
                        e.Handled = true;
                    }
                    else
                    {
                        AddCurrentTextToHistory();
                    }
                    break;
            }
        }

        /// <summary>
        /// テキスト変更イベント - フィルタリング
        /// </summary>
        private void HistoryTextBox_TextChanged(object sender, EventArgs e)
        {
            if (isHistoryVisible)
            {
                // テキスト入力に合わせて履歴をフィルタリング
                historyPopup.FilterHistory(this.Text, historyCategory);
            }
        }

        /// <summary>
        /// フォーカス喪失イベント - ポップアップ非表示
        /// </summary>
        private void HistoryTextBox_LostFocus(object sender, EventArgs e)
        {
            // フォーカスがポップアップに移った場合は閉じない
            if (historyPopup.Focused) return;
            
            // 少し遅延させて閉じる（ポップアップ内クリックの処理を許可するため）
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, args) =>
            {
                HideHistoryPopup();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        /// <summary>
        /// 履歴ポップアップを表示
        /// </summary>
        public void ShowHistoryPopup()
        {
            if (isHistoryVisible) return;
            
            historyPopup.LoadHistory(historyCategory);
            historyPopup.ShowPopup(this, PopupPosition.BelowControl);
            isHistoryVisible = true;
        }

        /// <summary>
        /// 履歴ポップアップを非表示
        /// </summary>
        public void HideHistoryPopup()
        {
            if (!isHistoryVisible) return;
            
            historyPopup.Hide();
            isHistoryVisible = false;
        }

        /// <summary>
        /// 現在のテキストを履歴に追加
        /// </summary>
        public void AddCurrentTextToHistory()
        {
            if (!string.IsNullOrWhiteSpace(this.Text))
            {
                historyPopup.AddHistory(this.Text, historyCategory);
            }
        }
    }
}
```

## 使用方法

以下に、この履歴機能付きテキストボックスを実際のフォームで使用する例を示します：

```csharp
public partial class MainForm : Form
{
    private HistoryTextBox searchTextBox;
    private HistoryTextBox addressTextBox;
    private Button searchButton;

    public MainForm()
    {
        InitializeComponent();
        InitializeCustomControls();
    }

    private void InitializeCustomControls()
    {
        // 検索用テキストボックス
        searchTextBox = new HistoryTextBox
        {
            Location = new Point(20, 30),
            Width = 300,
            Font = new Font("Yu Gothic UI", 9F)
        };
        searchTextBox.SetHistoryCategory("search"); // 検索履歴用カテゴリ

        // 住所入力用テキストボックス
        addressTextBox = new HistoryTextBox
        {
            Location = new Point(20, 80),
            Width = 300,
            Font = new Font("Yu Gothic UI", 9F)
        };
        addressTextBox.SetHistoryCategory("address"); // 住所履歴用カテゴリ

        // 検索ボタン
        searchButton = new Button
        {
            Text = "検索",
            Location = new Point(330, 30),
            Size = new Size(80, 28)
        };
        searchButton.Click += (s, e) =>
        {
            // 検索実行時に履歴に追加
            searchTextBox.AddCurrentTextToHistory();
            MessageBox.Show($"「{searchTextBox.Text}」を検索しました");
        };

        // ラベル
        Label searchLabel = new Label
        {
            Text = "検索:",
            AutoSize = true,
            Location = new Point(20, 10)
        };

        Label addressLabel = new Label
        {
            Text = "住所:",
            AutoSize = true,
            Location = new Point(20, 60)
        };

        // フォームに追加
        this.Controls.Add(searchLabel);
        this.Controls.Add(searchTextBox);
        this.Controls.Add(searchButton);
        this.Controls.Add(addressLabel);
        this.Controls.Add(addressTextBox);
    }
}
```

## 機能の詳細説明

1. **HistorySelectionPopup クラス**:
   - PopupBaseFormを継承して履歴選択用のポップアップを実装
   - 履歴の表示、フィルタリング、クリア機能を提供
   - 履歴はカテゴリごとにファイルに保存
   - 履歴選択時にイベントで通知

2. **HistoryTextBox クラス**:
   - 標準のTextBoxを拡張して履歴機能を追加
   - クリックまたは下矢印キーで履歴ポップアップを表示
   - Enterキーでテキストを履歴に追加
   - 入力テキストに基づいて履歴をフィルタリング

3. **主な特徴**:
   - テキストボックスごとに異なる履歴カテゴリを設定可能
   - 最大履歴数の制限（デフォルト10件）
   - 重複エントリの自動排除（同じ内容は最新のものだけが保持される）
   - 入力テキストに応じた動的フィルタリング

この実装により、ユーザーは過去の入力内容を簡単に再利用でき、入力作業の効率化が図れます。また、TextBoxごとに異なる履歴カテゴリを持たせることで、検索履歴、住所履歴など用途に応じた履歴管理が可能になります。