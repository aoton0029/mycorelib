リポジトリの構造を確認しました。`mycorelib`は既に権限管理やフォーム機能を含む汎用的なWinFormsライブラリとして開発されていますね。改善要望管理システムの実装例を以下に示します：

## 🔧 改善要望管理ライブラリ - コード例

### 1. 改善要望データモデル

```csharp
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoreLib.ImprovementRequest
{
    /// <summary>
    /// 改善要望データモデル
    /// </summary>
    public class ImprovementRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum RequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Rejected
    }
}
```

### 2. JSONファイル管理サービス（ファイルロック対応）

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace CoreLib.ImprovementRequest
{
    /// <summary>
    /// 改善要望のJSONファイル管理（排他制御対応）
    /// </summary>
    public class ImprovementRequestManager
    {
        private readonly string _filePath;
        private readonly string _lockFilePath;
        private readonly object _lockObject = new object();
        private static readonly Dictionary<string, Mutex> _fileMutexes = new Dictionary<string, Mutex>();

        public ImprovementRequestManager(string filePath = "improvement_requests.json")
        {
            _filePath = filePath;
            _lockFilePath = _filePath + ".lock";
            
            // ファイル別のMutexを取得または作成
            if (!_fileMutexes.ContainsKey(_filePath))
            {
                _fileMutexes[_filePath] = new Mutex(false, $"ImprovementRequest_{Path.GetFileName(_filePath)}");
            }
        }

        /// <summary>
        /// 全ての改善要望を取得
        /// </summary>
        public List<ImprovementRequest> GetAll()
        {
            return ExecuteWithLock(() =>
            {
                if (!File.Exists(_filePath))
                    return new List<ImprovementRequest>();

                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<ImprovementRequest>>(json) ?? new List<ImprovementRequest>();
            });
        }

        /// <summary>
        /// 改善要望を追加
        /// </summary>
        public void Add(ImprovementRequest request)
        {
            ExecuteWithLock(() =>
            {
                var requests = GetAllInternal();
                request.Id = requests.Any() ? requests.Max(r => r.Id) + 1 : 1;
                request.RequestDate = DateTime.Now;
                requests.Add(request);
                SaveInternal(requests);
            });
        }

        /// <summary>
        /// 改善要望を更新
        /// </summary>
        public bool Update(ImprovementRequest request)
        {
            return ExecuteWithLock(() =>
            {
                var requests = GetAllInternal();
                var existingIndex = requests.FindIndex(r => r.Id == request.Id);
                
                if (existingIndex == -1)
                    return false;

                requests[existingIndex] = request;
                SaveInternal(requests);
                return true;
            });
        }

        /// <summary>
        /// 改善要望を削除
        /// </summary>
        public bool Delete(int id)
        {
            return ExecuteWithLock(() =>
            {
                var requests = GetAllInternal();
                var request = requests.FirstOrDefault(r => r.Id == id);
                
                if (request == null)
                    return false;

                requests.Remove(request);
                SaveInternal(requests);
                return true;
            });
        }

        /// <summary>
        /// IDで改善要望を取得
        /// </summary>
        public ImprovementRequest GetById(int id)
        {
            return ExecuteWithLock(() =>
            {
                var requests = GetAllInternal();
                return requests.FirstOrDefault(r => r.Id == id);
            });
        }

        /// <summary>
        /// 条件で検索
        /// </summary>
        public List<ImprovementRequest> Search(string keyword = null, Priority? priority = null, RequestStatus? status = null)
        {
            return ExecuteWithLock(() =>
            {
                var requests = GetAllInternal();
                
                if (!string.IsNullOrEmpty(keyword))
                {
                    requests = requests.Where(r => 
                        r.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        r.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (priority.HasValue)
                {
                    requests = requests.Where(r => r.Priority == priority.Value).ToList();
                }

                if (status.HasValue)
                {
                    requests = requests.Where(r => r.Status == status.Value).ToList();
                }

                return requests;
            });
        }

        private T ExecuteWithLock<T>(Func<T> action)
        {
            var mutex = _fileMutexes[_filePath];
            mutex.WaitOne();
            
            try
            {
                return action();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ExecuteWithLock(Action action)
        {
            ExecuteWithLock(() => { action(); return true; });
        }

        private List<ImprovementRequest> GetAllInternal()
        {
            if (!File.Exists(_filePath))
                return new List<ImprovementRequest>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<ImprovementRequest>>(json) ?? new List<ImprovementRequest>();
        }

        private void SaveInternal(List<ImprovementRequest> requests)
        {
            // バックアップ作成
            if (File.Exists(_filePath))
            {
                File.Copy(_filePath, _filePath + ".bak", true);
            }

            var json = JsonConvert.SerializeObject(requests, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
```

### 3. WinFormsユーザーインターフェース

```csharp
using System;
using System.Linq;
using System.Windows.Forms;
using CoreLib.ImprovementRequest;

namespace CoreLib.Forms
{
    /// <summary>
    /// 改善要望管理フォーム
    /// </summary>
    public partial class FormImprovementRequestManager : Form
    {
        private readonly ImprovementRequestManager _manager;
        private DataGridView dgvRequests;
        private TextBox txtSearch;
        private ComboBox cbPriority, cbStatus;
        private Button btnAdd, btnEdit, btnDelete, btnSearch;

        public FormImprovementRequestManager()
        {
            _manager = new ImprovementRequestManager();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(1000, 600);
            this.Text = "改善要望管理システム";

            // 検索パネル
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 60 };
            
            txtSearch = new TextBox { Location = new System.Drawing.Point(10, 15), Width = 200 };
            cbPriority = new ComboBox { Location = new System.Drawing.Point(220, 15), Width = 100 };
            cbStatus = new ComboBox { Location = new System.Drawing.Point(330, 15), Width = 100 };
            btnSearch = new Button { Location = new System.Drawing.Point(440, 13), Text = "検索", Width = 60 };

            cbPriority.Items.AddRange(new[] { "全て", "低", "中", "高", "緊急" });
            cbPriority.SelectedIndex = 0;
            cbStatus.Items.AddRange(new[] { "全て", "保留中", "進行中", "完了", "却下" });
            cbStatus.SelectedIndex = 0;

            pnlSearch.Controls.AddRange(new Control[] { txtSearch, cbPriority, cbStatus, btnSearch });

            // ボタンパネル
            var pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 50 };
            
            btnAdd = new Button { Location = new System.Drawing.Point(10, 10), Text = "新規追加", Width = 80 };
            btnEdit = new Button { Location = new System.Drawing.Point(100, 10), Text = "編集", Width = 80 };
            btnDelete = new Button { Location = new System.Drawing.Point(190, 10), Text = "削除", Width = 80 };

            pnlButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // データグリッド
            dgvRequests = new DataGridView 
            { 
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            // イベント設定
            btnSearch.Click += BtnSearch_Click;
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            dgvRequests.CellDoubleClick += DgvRequests_CellDoubleClick;

            this.Controls.AddRange(new Control[] { dgvRequests, pnlSearch, pnlButtons });
        }

        private void LoadData()
        {
            var requests = _manager.GetAll();
            dgvRequests.DataSource = requests.Select(r => new
            {
                ID = r.Id,
                タイトル = r.Title,
                説明 = r.Description,
                優先度 = GetPriorityText(r.Priority),
                ステータス = GetStatusText(r.Status),
                要求者 = r.RequestedBy,
                要求日 = r.RequestDate.ToString("yyyy/MM/dd"),
                カテゴリ = r.Category
            }).ToList();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            Priority? priority = cbPriority.SelectedIndex > 0 ? (Priority)(cbPriority.SelectedIndex) : null;
            RequestStatus? status = cbStatus.SelectedIndex > 0 ? (RequestStatus)(cbStatus.SelectedIndex - 1) : null;
            
            var results = _manager.Search(txtSearch.Text, priority, status);
            dgvRequests.DataSource = results.Select(r => new
            {
                ID = r.Id,
                タイトル = r.Title,
                説明 = r.Description,
                優先度 = GetPriorityText(r.Priority),
                ステータス = GetStatusText(r.Status),
                要求者 = r.RequestedBy,
                要求日 = r.RequestDate.ToString("yyyy/MM/dd"),
                カテゴリ = r.Category
            }).ToList();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormImprovementRequestEdit())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _manager.Add(form.Request);
                    LoadData();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0) return;
            
            var id = (int)dgvRequests.SelectedRows[0].Cells["ID"].Value;
            var request = _manager.GetById(id);
            
            using (var form = new FormImprovementRequestEdit(request))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _manager.Update(form.Request);
                    LoadData();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0) return;
            
            var id = (int)dgvRequests.SelectedRows[0].Cells["ID"].Value;
            
            if (MessageBox.Show("選択した改善要望を削除しますか？", "確認", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _manager.Delete(id);
                LoadData();
            }
        }

        private void DgvRequests_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private string GetPriorityText(Priority priority)
        {
            return priority switch
            {
                Priority.Low => "低",
                Priority.Medium => "中",
                Priority.High => "高",
                Priority.Critical => "緊急",
                _ => "不明"
            };
        }

        private string GetStatusText(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "保留中",
                RequestStatus.InProgress => "進行中",
                RequestStatus.Completed => "完了",
                RequestStatus.Rejected => "却下",
                _ => "不明"
            };
        }
    }
}
```

### 4. 使用例

```csharp
// ライブラリの使用例
using CoreLib.ImprovementRequest;
using CoreLib.Forms;

// 改善要望管理システムの起動
var manager = new ImprovementRequestManager("requests.json");

// 新しい改善要望を追加
var newRequest = new ImprovementRequest
{
    Title = "ログイン画面の改善",
    Description = "パスワード表示切り替えボタンを追加してほしい",
    Priority = Priority.Medium,
    Status = RequestStatus.Pending,
    RequestedBy = "田中太郎",
    Category = "UI改善"
};

manager.Add(newRequest);

// フォームを表示
var form = new FormImprovementRequestManager();
Application.Run(form);
```

## 🔑 主な特徴

- **ファイルロック機能**：複数アプリケーションからの同時アクセスを安全に処理
- **CRUD操作**：作成・読み込み・更新・削除の完全サポート
- **検索・フィルタリング**：キーワード、優先度、ステータスでの絞り込み
- **バックアップ機能**：データ保存時の自動バックアップ
- **汎用設計**：他のアプリケーションから簡単に組み込み可能

**注意**: 結果は最新の30件のコミットに限定されています。完全な履歴を確認するには、[GitHub上でコミット履歴](https://github.com/aoton0029/mycorelib/commits)をご覧ください。

---

Winformsで改善要望をJSONファイルで管理するライブラリを作成いたします。汎用的な設計で、各アプリから簡単に呼び出せるように実装します。

以下のような構成でライブラリを作成します：

## 1. データモデル (ImprovementRequest.cs)

```csharp
using System;
using System.ComponentModel;

namespace ImprovementRequestLibrary.Models
{
    public class ImprovementRequest : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _description;
        private Priority _priority;
        private Status _status;
        private string _category;
        private string _requester;
        private DateTime _requestDate;
        private DateTime? _completedDate;
        private string _notes;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public Priority Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(nameof(Priority)); }
        }

        public Status Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public string Requester
        {
            get => _requester;
            set { _requester = value; OnPropertyChanged(nameof(Requester)); }
        }

        public DateTime RequestDate
        {
            get => _requestDate;
            set { _requestDate = value; OnPropertyChanged(nameof(RequestDate)); }
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set { _completedDate = value; OnPropertyChanged(nameof(CompletedDate)); }
        }

        public string Notes
        {
            get => _notes;
            set { _notes = value; OnPropertyChanged(nameof(Notes)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Priority
    {
        低 = 1,
        中 = 2,
        高 = 3,
        緊急 = 4
    }

    public enum Status
    {
        未対応,
        対応中,
        完了,
        却下,
        保留
    }
}
```

## 2. JSONデータアクセス層 (JsonDataService.cs)

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ImprovementRequestLibrary.Models;

namespace ImprovementRequestLibrary.Services
{
    public class JsonDataService
    {
        private readonly string _dataFilePath;
        private List<ImprovementRequest> _requests;

        public JsonDataService(string dataFilePath = "improvement_requests.json")
        {
            _dataFilePath = dataFilePath;
            _requests = new List<ImprovementRequest>();
            LoadData();
        }

        public List<ImprovementRequest> GetAllRequests()
        {
            return _requests.ToList();
        }

        public ImprovementRequest GetRequestById(int id)
        {
            return _requests.FirstOrDefault(r => r.Id == id);
        }

        public void AddRequest(ImprovementRequest request)
        {
            request.Id = GetNextId();
            request.RequestDate = DateTime.Now;
            _requests.Add(request);
            SaveData();
        }

        public void UpdateRequest(ImprovementRequest request)
        {
            var existingRequest = _requests.FirstOrDefault(r => r.Id == request.Id);
            if (existingRequest != null)
            {
                var index = _requests.IndexOf(existingRequest);
                _requests[index] = request;
                SaveData();
            }
        }

        public void DeleteRequest(int id)
        {
            var request = _requests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                _requests.Remove(request);
                SaveData();
            }
        }

        public List<ImprovementRequest> SearchRequests(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _requests.ToList();

            return _requests.Where(r =>
                r.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                r.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                r.Category?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                r.Requester?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    var json = File.ReadAllText(_dataFilePath);
                    _requests = JsonConvert.DeserializeObject<List<ImprovementRequest>>(json) 
                               ?? new List<ImprovementRequest>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"データファイルの読み込みに失敗しました: {ex.Message}");
            }
        }

        private void SaveData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_requests, Formatting.Indented);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"データファイルの保存に失敗しました: {ex.Message}");
            }
        }

        private int GetNextId()
        {
            return _requests.Any() ? _requests.Max(r => r.Id) + 1 : 1;
        }
    }
}
```

## 3. メインフォーム (ImprovementRequestForm.cs)

```csharp
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ImprovementRequestLibrary.Models;
using ImprovementRequestLibrary.Services;

namespace ImprovementRequestLibrary.Forms
{
    public partial class ImprovementRequestForm : Form
    {
        private readonly JsonDataService _dataService;
        private DataGridView _dataGridView;
        private TextBox _searchTextBox;
        private ComboBox _statusFilterComboBox;
        private ComboBox _priorityFilterComboBox;
        private Button _addButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _refreshButton;

        public ImprovementRequestForm(string dataFilePath = null)
        {
            _dataService = new JsonDataService(dataFilePath ?? "improvement_requests.json");
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "改善要望管理システム";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 検索・フィルター部分
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            _searchTextBox = new TextBox
            {
                PlaceholderText = "検索...",
                Location = new Point(10, 15),
                Width = 200
            };
            _searchTextBox.TextChanged += SearchTextBox_TextChanged;

            _statusFilterComboBox = new ComboBox
            {
                Location = new Point(220, 15),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _statusFilterComboBox.Items.Add("全ステータス");
            _statusFilterComboBox.Items.AddRange(Enum.GetNames(typeof(Status)));
            _statusFilterComboBox.SelectedIndex = 0;
            _statusFilterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;

            _priorityFilterComboBox = new ComboBox
            {
                Location = new Point(350, 15),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _priorityFilterComboBox.Items.Add("全優先度");
            _priorityFilterComboBox.Items.AddRange(Enum.GetNames(typeof(Priority)));
            _priorityFilterComboBox.SelectedIndex = 0;
            _priorityFilterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;

            _refreshButton = new Button
            {
                Text = "更新",
                Location = new Point(480, 14),
                Width = 60
            };
            _refreshButton.Click += RefreshButton_Click;

            searchPanel.Controls.AddRange(new Control[] 
            { 
                _searchTextBox, _statusFilterComboBox, _priorityFilterComboBox, _refreshButton 
            });

            // ボタンパネル
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            _addButton = new Button
            {
                Text = "新規追加",
                Location = new Point(10, 10),
                Width = 80
            };
            _addButton.Click += AddButton_Click;

            _editButton = new Button
            {
                Text = "編集",
                Location = new Point(100, 10),
                Width = 80
            };
            _editButton.Click += EditButton_Click;

            _deleteButton = new Button
            {
                Text = "削除",
                Location = new Point(190, 10),
                Width = 80
            };
            _deleteButton.Click += DeleteButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _deleteButton });

            // データグリッド
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            SetupDataGridViewColumns();
            _dataGridView.DoubleClick += DataGridView_DoubleClick;

            this.Controls.AddRange(new Control[] { _dataGridView, buttonPanel, searchPanel });
        }

        private void SetupDataGridViewColumns()
        {
            _dataGridView.Columns.Clear();

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 50
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Title",
                HeaderText = "タイトル",
                DataPropertyName = "Title",
                Width = 200
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Priority",
                HeaderText = "優先度",
                DataPropertyName = "Priority",
                Width = 80
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "ステータス",
                DataPropertyName = "Status",
                Width = 100
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "カテゴリ",
                DataPropertyName = "Category",
                Width = 120
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Requester",
                HeaderText = "要望者",
                DataPropertyName = "Requester",
                Width = 100
            });

            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RequestDate",
                HeaderText = "要望日",
                DataPropertyName = "RequestDate",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy/MM/dd" }
            });
        }

        private void LoadData()
        {
            try
            {
                var requests = _dataService.GetAllRequests();
                ApplyFilters(requests);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters(System.Collections.Generic.List<ImprovementRequest> requests)
        {
            var filteredRequests = requests;

            // 検索フィルター
            if (!string.IsNullOrWhiteSpace(_searchTextBox.Text))
            {
                filteredRequests = _dataService.SearchRequests(_searchTextBox.Text);
            }

            // ステータスフィルター
            if (_statusFilterComboBox.SelectedIndex > 0)
            {
                var selectedStatus = (Status)Enum.Parse(typeof(Status), _statusFilterComboBox.SelectedItem.ToString());
                filteredRequests = filteredRequests.Where(r => r.Status == selectedStatus).ToList();
            }

            // 優先度フィルター
            if (_priorityFilterComboBox.SelectedIndex > 0)
            {
                var selectedPriority = (Priority)Enum.Parse(typeof(Priority), _priorityFilterComboBox.SelectedItem.ToString());
                filteredRequests = filteredRequests.Where(r => r.Priority == selectedPriority).ToList();
            }

            _dataGridView.DataSource = filteredRequests.OrderByDescending(r => r.Id).ToList();
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var editForm = new ImprovementRequestEditForm())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _dataService.AddRequest(editForm.ImprovementRequest);
                        LoadData();
                        MessageBox.Show("改善要望を追加しました。", "成功", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"追加に失敗しました: {ex.Message}", "エラー", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する項目を選択してください。", "警告", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRequest = (ImprovementRequest)_dataGridView.SelectedRows[0].DataBoundItem;
            using (var editForm = new ImprovementRequestEditForm(selectedRequest))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _dataService.UpdateRequest(editForm.ImprovementRequest);
                        LoadData();
                        MessageBox.Show("改善要望を更新しました。", "成功", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"更新に失敗しました: {ex.Message}", "エラー", 
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する項目を選択してください。", "警告", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRequest = (ImprovementRequest)_dataGridView.SelectedRows[0].DataBoundItem;
            var result = MessageBox.Show($"「{selectedRequest.Title}」を削除しますか？", "確認", 
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _dataService.DeleteRequest(selectedRequest.Id);
                    LoadData();
                    MessageBox.Show("改善要望を削除しました。", "成功", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"削除に失敗しました: {ex.Message}", "エラー", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataGridView_DoubleClick(object sender, EventArgs e)
        {
            EditButton_Click(sender, e);
        }
    }
}
```

## 4. 編集フォーム (ImprovementRequestEditForm.cs)

```csharp
using System;
using System.Drawing;
using System.Windows.Forms;
using ImprovementRequestLibrary.Models;

namespace ImprovementRequestLibrary.Forms
{
    public partial class ImprovementRequestEditForm : Form
    {
        private TextBox _titleTextBox;
        private TextBox _descriptionTextBox;
        private ComboBox _priorityComboBox;
        private ComboBox _statusComboBox;
        private TextBox _categoryTextBox;
        private TextBox _requesterTextBox;
        private DateTimePicker _requestDatePicker;
        private DateTimePicker _completedDatePicker;
        private CheckBox _completedDateCheckBox;
        private TextBox _notesTextBox;
        private Button _okButton;
        private Button _cancelButton;

        public ImprovementRequest ImprovementRequest { get; private set; }

        public ImprovementRequestEditForm(ImprovementRequest existingRequest = null)
        {
            InitializeComponent();
            
            if (existingRequest != null)
            {
                ImprovementRequest = existingRequest;
                LoadRequestData();
                this.Text = "改善要望編集";
            }
            else
            {
                ImprovementRequest = new ImprovementRequest
                {
                    RequestDate = DateTime.Now,
                    Status = Status.未対応,
                    Priority = Priority.中
                };
                this.Text = "改善要望新規作成";
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Paddi

---

VBAでSQL Serverに接続するには、**ADODB**ライブラリを使用する方法が一般的です。  
以下は基本的な接続例です。

---

### 1. 参照設定

VBAエディタで「ツール」→「参照設定」→「Microsoft ActiveX Data Objects x.x Library」にチェックを入れます。  
（例：Microsoft ActiveX Data Objects 6.1 Library）

---

### 2. コード例

```vba
Sub ConnectToSQLServer()
    Dim conn As Object
    Dim rs As Object
    Dim connStr As String

    ' 接続文字列の作成
    connStr = "Provider=SQLOLEDB;" & _
              "Data Source=サーバ名;" & _
              "Initial Catalog=データベース名;" & _
              "User ID=ユーザー名;" & _
              "Password=パスワード;"

    Set conn = CreateObject("ADODB.Connection")
    Set rs = CreateObject("ADODB.Recordset")

    On Error GoTo ErrHandler

    conn.Open connStr

    ' SQLを実行
    rs.Open "SELECT * FROM テーブル名", conn

    ' データの取得例
    Do Until rs.EOF
        Debug.Print rs.Fields(0).Value
        rs.MoveNext
    Loop

    rs.Close
    conn.Close

    Set rs = Nothing
    Set conn = Nothing

    Exit Sub

ErrHandler:
    MsgBox "接続エラー: " & Err.Description
End Sub
```

---

### 3. 注意点・補足

- **サーバ名**や**データベース名**、**ユーザー名**、**パスワード**は実環境に合わせて変更してください。
- Windows認証を使う場合は、User IDとPasswordを省略し、`Integrated Security=SSPI;`を追加します。
    ```vba
    connStr = "Provider=SQLOLEDB;" & _
              "Data Source=サーバ名;" & _
              "Initial Catalog=データベース名;" & _
              "Integrated Security=SSPI;"
    ```
- SQL Server Native Client（`Provider=SQLNCLI11` など）を使う場合は、Providerの部分を変更します。

---

ご要望に合わせてサンプルや応用例もご案内できます。  
必要であれば、接続文字列やSQLの内容を具体的に教えてください。


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