Winformsで改善要望をJSONファイルで管理するためのライブラリを作成します。サーバー上のJSONファイルにアクセスし、複数のPCから同時にアクセスしても整合性を保てるような汎用的な設計にします。

以下のファイル構成でライブラリを作成します：

```csharp name=Models/ImprovementRequest.cs
using System;
using System.ComponentModel;

namespace ImprovementRequestManager.Models
{
    /// <summary>
    /// 改善要望を表すモデルクラス
    /// </summary>
    public class ImprovementRequest : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private string _description;
        private string _requester;
        private DateTime _requestDate;
        private Priority _priority;
        private Status _status;
        private string _assignee;
        private DateTime? _completedDate;
        private string _category;
        private string _comments;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Requester
        {
            get => _requester;
            set
            {
                _requester = value;
                OnPropertyChanged(nameof(Requester));
            }
        }

        public DateTime RequestDate
        {
            get => _requestDate;
            set
            {
                _requestDate = value;
                OnPropertyChanged(nameof(RequestDate));
            }
        }

        public Priority Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        public Status Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string Assignee
        {
            get => _assignee;
            set
            {
                _assignee = value;
                OnPropertyChanged(nameof(Assignee));
            }
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set
            {
                _completedDate = value;
                OnPropertyChanged(nameof(CompletedDate));
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public string Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                OnPropertyChanged(nameof(Comments));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImprovementRequest()
        {
            Id = Guid.NewGuid().ToString();
            RequestDate = DateTime.Now;
            Status = Status.Open;
            Priority = Priority.Medium;
        }
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum Status
    {
        Open,
        InProgress,
        UnderReview,
        Completed,
        Rejected,
        OnHold
    }
}
```

```csharp name=Services/ImprovementRequestService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using ImprovementRequestManager.Models;

namespace ImprovementRequestManager.Services
{
    /// <summary>
    /// 改善要望の管理を行うサービスクラス
    /// ファイルロックによる排他制御を実装
    /// </summary>
    public class ImprovementRequestService
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _lockTimeoutMs;

        public ImprovementRequestService(string filePath, int lockTimeoutMs = 30000)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _semaphore = new SemaphoreSlim(1, 1);
            _lockTimeoutMs = lockTimeoutMs;
        }

        /// <summary>
        /// すべての改善要望を取得
        /// </summary>
        public async Task<List<ImprovementRequest>> GetAllRequestsAsync()
        {
            if (!await _semaphore.WaitAsync(_lockTimeoutMs))
                throw new TimeoutException("ファイルアクセスがタイムアウトしました。");

            try
            {
                return await ReadFromFileAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 新しい改善要望を追加
        /// </summary>
        public async Task<bool> AddRequestAsync(ImprovementRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!await _semaphore.WaitAsync(_lockTimeoutMs))
                throw new TimeoutException("ファイルアクセスがタイムアウトしました。");

            try
            {
                var requests = await ReadFromFileAsync();
                
                // IDが重複していないかチェック
                if (requests.Exists(r => r.Id == request.Id))
                {
                    request.Id = Guid.NewGuid().ToString();
                }

                requests.Add(request);
                await WriteToFileAsync(requests);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 改善要望を更新
        /// </summary>
        public async Task<bool> UpdateRequestAsync(ImprovementRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (!await _semaphore.WaitAsync(_lockTimeoutMs))
                throw new TimeoutException("ファイルアクセスがタイムアウトしました。");

            try
            {
                var requests = await ReadFromFileAsync();
                var index = requests.FindIndex(r => r.Id == request.Id);
                
                if (index == -1) return false;

                requests[index] = request;
                await WriteToFileAsync(requests);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 改善要望を削除
        /// </summary>
        public async Task<bool> DeleteRequestAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            if (!await _semaphore.WaitAsync(_lockTimeoutMs))
                throw new TimeoutException("ファイルアクセスがタイムアウトしました。");

            try
            {
                var requests = await ReadFromFileAsync();
                var removed = requests.RemoveAll(r => r.Id == id) > 0;
                
                if (removed)
                {
                    await WriteToFileAsync(requests);
                }
                
                return removed;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// IDで改善要望を検索
        /// </summary>
        public async Task<ImprovementRequest> GetRequestByIdAsync(string id)
        {
            var requests = await GetAllRequestsAsync();
            return requests.Find(r => r.Id == id);
        }

        /// <summary>
        /// ファイルからデータを読み込み
        /// </summary>
        private async Task<List<ImprovementRequest>> ReadFromFileAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<ImprovementRequest>();
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fileStream))
            {
                var json = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<ImprovementRequest>();
                }

                try
                {
                    return JsonConvert.DeserializeObject<List<ImprovementRequest>>(json) ?? new List<ImprovementRequest>();
                }
                catch (JsonException)
                {
                    // JSONの形式が不正な場合は空のリストを返す
                    return new List<ImprovementRequest>();
                }
            }
        }

        /// <summary>
        /// ファイルにデータを書き込み
        /// </summary>
        private async Task WriteToFileAsync(List<ImprovementRequest> requests)
        {
            // ディレクトリが存在しない場合は作成
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(requests, Formatting.Indented);
            
            // 一時ファイルに書き込んでから元のファイルと置き換え（原子性を保証）
            var tempPath = _filePath + ".tmp";
            
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteAsync(json);
                await writer.FlushAsync();
            }

            // 元のファイルがあれば削除してから、一時ファイルをリネーム
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
            File.Move(tempPath, _filePath);
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
```

```csharp name=Forms/ImprovementRequestForm.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImprovementRequestManager.Models;
using ImprovementRequestManager.Services;

namespace ImprovementRequestManager.Forms
{
    /// <summary>
    /// 改善要望管理のメインフォーム
    /// </summary>
    public partial class ImprovementRequestForm : Form
    {
        private ImprovementRequestService _service;
        private List<ImprovementRequest> _requests;
        private ImprovementRequest _selectedRequest;

        public ImprovementRequestForm(string jsonFilePath)
        {
            InitializeComponent();
            _service = new ImprovementRequestService(jsonFilePath);
            _requests = new List<ImprovementRequest>();
            InitializeEvents();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridViewRequests = new DataGridView();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonDelete = new Button();
            this.buttonRefresh = new Button();
            this.groupBoxDetails = new GroupBox();
            this.textBoxTitle = new TextBox();
            this.textBoxDescription = new TextBox();
            this.textBoxRequester = new TextBox();
            this.comboBoxPriority = new ComboBox();
            this.comboBoxStatus = new ComboBox();
            this.textBoxAssignee = new TextBox();
            this.textBoxCategory = new TextBox();
            this.textBoxComments = new TextBox();
            this.dateTimePickerRequest = new DateTimePicker();
            this.dateTimePickerCompleted = new DateTimePicker();
            this.checkBoxCompletedDate = new CheckBox();
            this.labelTitle = new Label();
            this.labelDescription = new Label();
            this.labelRequester = new Label();
            this.labelPriority = new Label();
            this.labelStatus = new Label();
            this.labelAssignee = new Label();
            this.labelCategory = new Label();
            this.labelComments = new Label();
            this.labelRequestDate = new Label();
            this.labelCompletedDate = new Label();
            this.buttonSave = new Button();
            this.buttonCancel = new Button();
            this.statusStrip = new StatusStrip();
            this.toolStripStatusLabel = new ToolStripStatusLabel();

            this.SuspendLayout();

            // Form
            this.Text = "改善要望管理システム";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView
            this.dataGridViewRequests.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewRequests.Size = new System.Drawing.Size(600, 400);
            this.dataGridViewRequests.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            this.dataGridViewRequests.AllowUserToAddRows = false;
            this.dataGridViewRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRequests.MultiSelect = false;
            this.dataGridViewRequests.ReadOnly = true;

            // Buttons
            this.buttonAdd.Location = new System.Drawing.Point(12, 420);
            this.buttonAdd.Size = new System.Drawing.Size(75, 30);
            this.buttonAdd.Text = "追加";
            this.buttonAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            this.buttonEdit.Location = new System.Drawing.Point(93, 420);
            this.buttonEdit.Size = new System.Drawing.Size(75, 30);
            this.buttonEdit.Text = "編集";
            this.buttonEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            this.buttonDelete.Location = new System.Drawing.Point(174, 420);
            this.buttonDelete.Size = new System.Drawing.Size(75, 30);
            this.buttonDelete.Text = "削除";
            this.buttonDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            this.buttonRefresh.Location = new System.Drawing.Point(255, 420);
            this.buttonRefresh.Size = new System.Drawing.Size(75, 30);
            this.buttonRefresh.Text = "更新";
            this.buttonRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            // GroupBox Details
            this.groupBoxDetails.Location = new System.Drawing.Point(630, 12);
            this.groupBoxDetails.Size = new System.Drawing.Size(540, 500);
            this.groupBoxDetails.Text = "詳細情報";
            this.groupBoxDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            // Detail Controls
            int yPos = 25;
            int spacing = 30;

            this.labelTitle.Location = new System.Drawing.Point(10, yPos);
            this.labelTitle.Size = new System.Drawing.Size(80, 20);
            this.labelTitle.Text = "タイトル:";

            this.textBoxTitle.Location = new System.Drawing.Point(100, yPos);
            this.textBoxTitle.Size = new System.Drawing.Size(420, 20);
            yPos += spacing;

            this.labelDescription.Location = new System.Drawing.Point(10, yPos);
            this.labelDescription.Size = new System.Drawing.Size(80, 20);
            this.labelDescription.Text = "説明:";

            this.textBoxDescription.Location = new System.Drawing.Point(100, yPos);
            this.textBoxDescription.Size = new System.Drawing.Size(420, 60);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.ScrollBars = ScrollBars.Vertical;
            yPos += 70;

            this.labelRequester.Location = new System.Drawing.Point(10, yPos);
            this.labelRequester.Size = new System.Drawing.Size(80, 20);
            this.labelRequester.Text = "要求者:";

            this.textBoxRequester.Location = new System.Drawing.Point(100, yPos);
            this.textBoxRequester.Size = new System.Drawing.Size(200, 20);
            yPos += spacing;

            this.labelPriority.Location = new System.Drawing.Point(10, yPos);
            this.labelPriority.Size = new System.Drawing.Size(80, 20);
            this.labelPriority.Text = "優先度:";

            this.comboBoxPriority.Location = new System.Drawing.Point(100, yPos);
            this.comboBoxPriority.Size = new System.Drawing.Size(150, 20);
            this.comboBoxPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += spacing;

            this.labelStatus.Location = new System.Drawing.Point(10, yPos);
            this.labelStatus.Size = new System.Drawing.Size(80, 20);
            this.labelStatus.Text = "ステータス:";

            this.comboBoxStatus.Location = new System.Drawing.Point(100, yPos);
            this.comboBoxStatus.Size = new System.Drawing.Size(150, 20);
            this.comboBoxStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += spacing;

            this.labelAssignee.Location = new System.Drawing.Point(10, yPos);
            this.labelAssignee.Size = new System.Drawing.Size(80, 20);
            this.labelAssignee.Text = "担当者:";

            this.textBoxAssignee.Location = new System.Drawing.Point(100, yPos);
            this.textBoxAssignee.Size = new System.Drawing.Size(200, 20);
            yPos += spacing;

            this.labelCategory.Location = new System.Drawing.Point(10, yPos);
            this.labelCategory.Size = new System.Drawing.Size(80, 20);
            this.labelCategory.Text = "カテゴリ:";

            this.textBoxCategory.Location = new System.Drawing.Point(100, yPos);
            this.textBoxCategory.Size = new System.Drawing.Size(200, 20);
            yPos += spacing;

            this.labelRequestDate.Location = new System.Drawing.Point(10, yPos);
            this.labelRequestDate.Size = new System.Drawing.Size(80, 20);
            this.labelRequestDate.Text = "要求日:";

            this.dateTimePickerRequest.Location = new System.Drawing.Point(100, yPos);
            this.dateTimePickerRequest.Size = new System.Drawing.Size(200, 20);
            yPos += spacing;

            this.checkBoxCompletedDate.Location = new System.Drawing.Point(10, yPos);
            this.checkBoxCompletedDate.Size = new System.Drawing.Size(80, 20);
            this.checkBoxCompletedDate.Text = "完了日:";

            this.dateTimePickerCompleted.Location = new System.Drawing.Point(100, yPos);
            this.dateTimePickerCompleted.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerCompleted.Enabled = false;
            yPos += spacing;

            this.labelComments.Location = new System.Drawing.Point(10, yPos);
            this.labelComments.Size = new System.Drawing.Size(80, 20);
            this.labelComments.Text = "コメント:";

            this.textBoxComments.Location = new System.Drawing.Point(100, yPos);
            this.textBoxComments.Size = new System.Drawing.Size(420, 60);
            this.textBoxComments.Multiline = true;
            this.textBoxComments.ScrollBars = ScrollBars.Vertical;
            yPos += 70;

            // Save/Cancel buttons
            this.buttonSave.Location = new System.Drawing.Point(360, yPos);
            this.buttonSave.Size = new System.Drawing.Size(75, 30);
            this.buttonSave.Text = "保存";
            this.buttonSave.Enabled = false;

            this.buttonCancel.Location = new System.Drawing.Point(445, yPos);
            this.buttonCancel.Size = new System.Drawing.Size(75, 30);
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.Enabled = false;

            // StatusStrip
            this.statusStrip.Items.Add(this.toolStripStatusLabel);
            this.toolStripStatusLabel.Text = "準備完了";

            // Add controls to GroupBox
            this.groupBoxDetails.Controls.AddRange(new Control[] {
                this.labelTitle, this.textBoxTitle,
                this.labelDescription, this.textBoxDescription,
                this.labelRequester, this.textBoxRequester,
                this.labelPriority, this.comboBoxPriority,
                this.labelStatus, this.comboBoxStatus,
                this.labelAssignee, this.textBoxAssignee,
                this.labelCategory, this.textBoxCategory,
                this.labelRequestDate, this.dateTimePickerRequest,
                this.checkBoxCompletedDate, this.dateTimePickerCompleted,
                this.labelComments, this.textBoxComments,
                this.buttonSave, this.buttonCancel
            });

            // Add controls to Form
            this.Controls.AddRange(new Control[] {
                this.dataGridViewRequests,
                this.buttonAdd, this.buttonEdit, this.buttonDelete, this.buttonRefresh,
                this.groupBoxDetails,
                this.statusStrip
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeEvents()
        {
            this.Load += ImprovementRequestForm_Load;
            this.buttonAdd.Click += ButtonAdd_Click;
            this.buttonEdit.Click += ButtonEdit_Click;
            this.buttonDelete.Click += ButtonDelete_Click;
            this.buttonRefresh.Click += ButtonRefresh_Click;
            this.buttonSave.Click += ButtonSave_Click;
            this.buttonCancel.Click += ButtonCancel_Click;
            this.dataGridViewRequests.SelectionChanged += DataGridViewRequests_SelectionChanged;
            this.checkBoxCompletedDate.CheckedChanged += CheckBoxCompletedDate_CheckedChanged;

            // コントロールの変更を監視
            this.textBoxTitle.TextChanged += Control_Changed;
            this.textBoxDescription.TextChanged += Control_Changed;
            this.textBoxRequester.TextChanged += Control_Changed;
            this.comboBoxPriority.SelectedIndexChanged += Control_Changed;
            this.comboBoxStatus.SelectedIndexChanged += Control_Changed;
            this.textBoxAssignee.TextChanged += Control_Changed;
            this.textBoxCategory.TextChanged += Control_Changed;
            this.textBoxComments.TextChanged += Control_Changed;
            this.dateTimePickerRequest.ValueChanged += Control_Changed;
            this.dateTimePickerCompleted.ValueChanged += Control_Changed;
        }

        private async void ImprovementRequestForm_Load(object sender, EventArgs e)
        {
            InitializeComboBoxes();
            await LoadDataAsync();
            ClearDetailControls();
        }

        private void InitializeComboBoxes()
        {
            // Priority ComboBox
            this.comboBoxPriority.Items.Clear();
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                this.comboBoxPriority.Items.Add(priority);
            }

            // Status ComboBox
            this.comboBoxStatus.Items.Clear();
            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                this.comboBoxStatus.Items.Add(status);
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                this.toolStripStatusLabel.Text = "データを読み込み中...";
                this.Cursor = Cursors.WaitCursor;

                _requests = await _service.GetAllRequestsAsync();
                UpdateDataGridView();

                this.toolStripStatusLabel.Text = $"{_requests.Count}件の改善要望を読み込みました";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.toolStripStatusLabel.Text = "データの読み込みに失敗";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void UpdateDataGridView()
        {
            this.dataGridViewRequests.DataSource = null;
            this.dataGridViewRequests.DataSource = _requests.Select(r => new
            {
                ID = r.Id,
                タイトル = r.Title,
                要求者 = r.Requester,
                優先度 = r.Priority,
                ステータス = r.Status,
                担当者 = r.Assignee,
                カテゴリ = r.Category,
                要求日 = r.RequestDate.ToString("yyyy/MM/dd"),
                完了日 = r.CompletedDate?.ToString("yyyy/MM/dd") ?? ""
            }).ToList();

            // ID列を非表示にする
            if (this.dataGridViewRequests.Columns["ID"] != null)
            {
                this.dataGridViewRequests.Columns["ID"].Visible = false;
            }
        }

        private async void ButtonAdd_Click(object sender, EventArgs e)
        {
            var editForm = new ImprovementRequestEditForm();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.toolStripStatusLabel.Text = "新しい改善要望を保存中...";
                    await _service.AddRequestAsync(editForm.Request);
                    await LoadDataAsync();
                    this.toolStripStatusLabel.Text = "新しい改善要望を追加しました";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"改善要望の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.toolStripStatusLabel.Text = "改善要望の追加に失敗";
                }
            }
        }

        private async void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("編集する改善要望を選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedId = this.dataGridViewRequests.SelectedRows[0].Cells["ID"].Value.ToString();
            var request = _requests.FirstOrDefault(r => r.Id == selectedId);
            
            if (request == null) return;

            var editForm = new ImprovementRequestEditForm(request);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.toolStripStatusLabel.Text = "改善要望を更新中...";
                    await _service.UpdateRequestAsync(editForm.Request);
                    await LoadDataAsync();
                    this.toolStripStatusLabel.Text = "改善要望を更新しました";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"改善要望の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.toolStripStatusLabel.Text = "改善要望の更新に失敗";
                }
            }
        }

        private async void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("削除する改善要望を選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("選択した改善要望を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var selectedId = this.dataGridViewRequests.SelectedRows[0].Cells["ID"].Value.ToString();

            try
            {
                this.toolStripStatusLabel.Text = "改善要望を削除中...";
                await _service.DeleteRequestAsync(selectedId);
                await LoadDataAsync();
                ClearDetailControls();
                this.toolStripStatusLabel.Text = "改善要望を削除しました";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"改善要望の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.toolStripStatusLabel.Text = "改善要望の削除に失敗";
            }
        }

        private async void ButtonRefresh_Click(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private void DataGridViewRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dataGridViewRequests.SelectedRows.Count > 0)
            {
                var selectedId = this.dataGridViewRequests.SelectedRows[0].Cells["ID"].Value.ToString();
                _selectedRequest = _requests.FirstOrDefault(r => r.Id == selectedId);
                DisplayRequestDetails(_selectedRequest);
            }
            else
            {
                ClearDetailControls();
                _selectedRequest = null;
            }
        }

        private void DisplayRequestDetails(ImprovementRequest request)
        {
            if (request == null)
            {
                ClearDetailControls();
                return;
            }

            this.textBoxTitle.Text = request.Title ?? "";
            this.textBoxDescription.Text = request.Description ?? "";
            this.textBoxRequester.Text = request.Requester ?? "";
            this.comboBoxPriority.SelectedItem = request.Priority;
            this.comboBoxStatus.SelectedItem = request.Status;
            this.textBoxAssignee.Text = request.Assignee ?? "";
            this.textBoxCategory.Text = request.Category ?? "";
            this.textBoxComments.Text = request.Comments ?? "";
            this.dateTimePickerRequest.Value = request.RequestDate;
            
            if (request.CompletedDate.HasValue)
            {
                this.checkBoxCompletedDate.Checked = true;
                this.dateTimePickerCompleted.Value = request.CompletedDate.Value;
                this.dateTimePickerCompleted.Enabled = true;
            }
            else
            {
                this.checkBoxCompletedDate.Checked = false;
                this.dateTimePickerCompleted.Enabled = false;
            }

            SetControlsReadOnly(true);
        }

        private void ClearDetailControls()
        {
            this.textBoxTitle.Clear();
            this.textBoxDescription.Clear();
            this.textBoxRequester.Clear();
            this.comboBoxPriority.SelectedIndex = -1;
            this.comboBoxStatus.SelectedIndex = -1;
            this.textBoxAssignee.Clear();
            this.textBoxCategory.Clear();
            this.textBoxComments.Clear();
            this.dateTimePickerRequest.Value = DateTime.Now;
            this.checkBoxCompletedDate.Checked = false;
            this.dateTimePickerCompleted.Enabled = false;

            SetControlsReadOnly(true);
        }

        private void SetControlsReadOnly(bool readOnly)
        {
            this.textBoxTitle.ReadOnly = readOnly;
            this.textBoxDescription.ReadOnly = readOnly;
            this.textBoxRequester.ReadOnly = readOnly;
            this.comboBoxPriority.Enabled = !readOnly;
            this.comboBoxStatus.Enabled = !readOnly;
            this.textBoxAssignee.ReadOnly = readOnly;
            this.textBoxCategory.ReadOnly = readOnly;
            this.textBoxComments.ReadOnly = readOnly;
            this.dateTimePickerRequest.Enabled = !readOnly;
            this.checkBoxCompletedDate.Enabled = !readOnly;
            this.dateTimePickerCompleted.Enabled = !readOnly && this.checkBoxCompletedDate.Checked;

            this.buttonSave.Enabled = !readOnly;
            this.buttonCancel.Enabled = !readOnly;
        }

        private void CheckBoxCompletedDate_CheckedChanged(object sender, EventArgs e)
        {
            this.dateTimePickerCompleted.Enabled = this.checkBoxCompletedDate.Checked && !this.textBoxTitle.ReadOnly;
        }

        private void Control_Changed(object sender, EventArgs e)
        {
            // コントロールが変更された時の処理（必要に応じて実装）
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            if (_selectedRequest == null) return;

            try
            {
                // 入力値の検証
                if (string.IsNullOrWhiteSpace(this.textBoxTitle.Text))
                {
                    MessageBox.Show("タイトルは必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // データを更新
                _selectedRequest.Title = this.textBoxTitle.Text.Trim();
                _selectedRequest.Description = this.textBoxDescription.Text.Trim();
                _selectedRequest.Requester = this.textBoxRequester.Text.Trim();
                _selectedRequest.Priority = (Priority)this.comboBoxPriority.SelectedItem;
                _selectedRequest.Status = (Status)this.comboBoxStatus.SelectedItem;
                _selectedRequest.Assignee = this.textBoxAssignee.Text.Trim();
                _selectedRequest.Category = this.textBoxCategory.Text.Trim();
                _selectedRequest.Comments = this.textBoxComments.Text.Trim();
                _selectedRequest.RequestDate = this.dateTimePickerRequest.Value;
                _selectedRequest.CompletedDate = this.checkBoxCompletedDate.Checked ? this.dateTimePickerCompleted.Value : (DateTime?)null;

                this.toolStripStatusLabel.Text = "改善要望を保存中...";
                await _service.UpdateRequestAsync(_selectedRequest);
                await LoadDataAsync();
                SetControlsReadOnly(true);
                this.toolStripStatusLabel.Text = "改善要望を保存しました";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"改善要望の保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.toolStripStatusLabel.Text = "改善要望の保存に失敗";
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DisplayRequestDetails(_selectedRequest);
            SetControlsReadOnly(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private System.ComponentModel.IContainer components = null;
        private DataGridView dataGridViewRequests;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonRefresh;
        private GroupBox groupBoxDetails;
        private TextBox textBoxTitle;
        private TextBox textBoxDescription;
        private TextBox textBoxRequester;
        private ComboBox comboBoxPriority;
        private ComboBox comboBoxStatus;
        private TextBox textBoxAssignee;
        private TextBox textBoxCategory;
        private TextBox textBoxComments;
        private DateTimePicker dateTimePickerRequest;
        private DateTimePicker dateTimePickerCompleted;
        private CheckBox checkBoxCompletedDate;
        private Label labelTitle;
        private Label labelDescription;
        private Label labelRequester;
        private Label labelPriority;
        private Label labelStatus;
        private Label labelAssignee;
        private Label labelCategory;
        private Label labelComments;
        private Label labelRequestDate;
        private Label labelCompletedDate;
        private Button buttonSave;
        private Button buttonCancel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        #endregion
    }
}
```

```csharp name=Forms/ImprovementRequestEditForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using ImprovementRequestManager.Models;

namespace ImprovementRequestManager.Forms
{
    /// <summary>
    /// 改善要望の追加・編集用フォーム
    /// </summary>
    public partial class ImprovementRequestEditForm : Form
    {
        public ImprovementRequest Request { get; set; }
        private bool _isEditMode;

        public ImprovementRequestEditForm()
        {
            InitializeComponent();
            _isEditMode = false;
            Request = new ImprovementRequest();
            InitializeForm();
        }

        public ImprovementRequestEditForm(ImprovementRequest request)
        {
            InitializeComponent();
            _isEditMode = true;
            Request = new ImprovementRequest
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Requester = request.Requester,
                RequestDate = request.RequestDate,
                Priority = request.Priority,
                Status = request.Status,
                Assignee = request.Assignee,
                CompletedDate = request.CompletedDate,
                Category = request.Category,
                Comments = request.Comments
            };
            InitializeForm();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelTitle = new Label();
            this.textBoxTitle = new TextBox();
            this.labelDescription = new Label();
            this.textBoxDescription = new TextBox();
            this.labelRequester = new Label();
            this.textBoxRequester = new TextBox();
            this.labelPriority = new Label();
            this.comboBoxPriority = new ComboBox();
            this.labelStatus = new Label();
            this.comboBoxStatus = new ComboBox();
            this.labelAssignee = new Label();
            this.textBoxAssignee = new TextBox();
            this.labelCategory = new Label();
            this.textBoxCategory = new TextBox();
            this.labelRequestDate = new Label();
            this.dateTimePickerRequest = new DateTimePicker();
            this.checkBoxCompletedDate = new CheckBox();
            this.dateTimePickerCompleted = new DateTimePicker();
            this.labelComments = new Label();
            this.textBoxComments = new TextBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();

            this.SuspendLayout();

            // Form
            this.Text = "改善要望の編集";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;
            int spacing = 35;
            int labelWidth = 80;
            int controlWidth = 350;

            // Title
            this.labelTitle.Location = new Point(20, yPos);
            this.labelTitle.Size = new Size(labelWidth, 20);
            this.labelTitle.Text = "タイトル*:";

            this.textBoxTitle.Location = new Point(110, yPos);
            this.textBoxTitle.Size = new Size(controlWidth, 20);
            yPos += spacing;

            // Description
            this.labelDescription.Location = new Point(20, yPos);
            this.labelDescription.Size = new Size(labelWidth, 20);
            this.labelDescription.Text = "説明:";

            this.textBoxDescription.Location = new Point(110, yPos);
            this.textBoxDescription.Size = new Size(controlWidth, 60);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.ScrollBars = ScrollBars.Vertical;
            yPos += 70;

            // Requester
            this.labelRequester.Location = new Point(20, yPos);
            this.labelRequester.Size = new Size(labelWidth, 20);
            this.labelRequester.Text = "要求者*:";

            this.textBoxRequester.Location = new Point(110, yPos);
            this.textBoxRequester.Size = new Size(200, 20);
            yPos += spacing;

            // Priority
            this.labelPriority.Location = new Point(20, yPos);
            this.labelPriority.Size = new Size(labelWidth, 20);
            this.labelPriority.Text = "優先度:";

            this.comboBoxPriority.Location = new Point(110, yPos);
            this.comboBoxPriority.Size = new Size(150, 20);
            this.comboBoxPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += spacing;

            // Status
            this.labelStatus.Location = new Point(20, yPos);
            this.labelStatus.Size = new Size(labelWidth, 20);
            this.labelStatus.Text = "ステータス:";

            this.comboBoxStatus.Location = new Point(110, yPos);
            this.comboBoxStatus.Size = new Size(150, 20);
            this.comboBoxStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += spacing;

            // Assignee
            this.labelAssignee.Location = new Point(20, yPos);
            this.labelAssignee.Size = new Size(labelWidth, 20);
            this.labelAssignee.Text = "担当者:";

            this.textBoxAssignee.Location = new Point(110, yPos);
            this.textBoxAssignee.Size = new Size(200, 20);
            yPos += spacing;

            // Category
            this.labelCategory.Location = new Point(20, yPos);
            this.labelCategory.Size = new Size(labelWidth, 20);
            this.labelCategory.Text = "カテゴリ:";

            this.textBoxCategory.Location = new Point(110, yPos);
            this.textBoxCategory.Size = new Size(200, 20);
            yPos += spacing;

            // Request Date
            this.labelRequestDate.Location = new Point(20, yPos);
            this.labelRequestDate.Size = new Size(labelWidth, 20);
            this.labelRequestDate.Text = "要求日:";

            this.dateTimePickerRequest.Location = new Point(110, yPos);
            this.dateTimePickerRequest.Size = new Size(200, 20);
            yPos += spacing;

            // Completed Date
            this.checkBoxCompletedDate.Location = new Point(20, yPos);
            this.checkBoxCompletedDate.Size = new Size(80, 20);
            this.checkBoxCompletedDate.Text = "完了日:";

            this.dateTimePickerCompleted.Location = new Point(110, yPos);
            this.dateTimePickerCompleted.Size = new Size(200, 20);
            this.dateTimePickerCompleted.Enabled = false;
            yPos += spacing;

            // Comments
            this.labelComments.Location = new Point(20, yPos);
            this.labelComments.Size = new Size(labelWidth, 20);
            this.labelComments.Text = "コメント:";

            this.textBoxComments.Location = new Point(110, yPos);
            this.textBoxComments.Size = new Size(controlWidth, 60);
            this.textBoxComments.Multiline = true;
            this.textBoxComments.ScrollBars = ScrollBars.Vertical;
            yPos += 70;

            // Buttons
            this.buttonOK.Location = new Point(300, yPos);
            this.buttonOK.Size = new Size(75, 30);
            this.buttonOK.Text = "OK";
            this.buttonOK.DialogResult = DialogResult.OK;

            this.buttonCancel.Location = new Point(385, yPos);
            this.buttonCancel.Size = new Size(75, 30);
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.DialogResult = DialogResult.Cancel;

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                this.labelTitle, this.textBoxTitle,
                this.labelDescription, this.textBoxDescription,
                this.labelRequester, this.textBoxRequester,
                this.labelPriority, this.comboBoxPriority,
                this.labelStatus, this.comboBoxStatus,
                this.labelAssignee, this.textBoxAssignee,
                this.labelCategory, this.textBoxCategory,
                this.labelRequestDate, this.dateTimePickerRequest,
                this.checkBoxCompletedDate, this.dateTimePickerCompleted,
                this.labelComments, this.textBoxComments,
                this.buttonOK, this.buttonCancel
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void InitializeForm()
        {
            InitializeComboBoxes();
            InitializeEvents();
            LoadRequestData();
            
            this.Text = _isEditMode ? "改善要望の編集" : "新しい改善要望";
        }

        private void InitializeComboBoxes()
        {
            // Priority ComboBox
            this.comboBoxPriority.Items.Clear();
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                this.comboBoxPriority.Items.Add(priority);
            }

            // Status ComboBox
            this.comboBoxStatus.Items.Clear();
            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                this.comboBoxStatus.Items.Add(status);
            }
        }

        private void InitializeEvents()
        {
            this.checkBoxCompletedDate.CheckedChanged += CheckBoxCompletedDate_CheckedChanged;
            this.buttonOK.Click += ButtonOK_Click;
        }

        private void LoadRequestData()
        {
            if (Request == null) return;

            this.textBoxTitle.Text = Request.Title ?? "";
            this.textBoxDescription.Text = Request.Description ?? "";
            this.textBoxRequester.Text = Request.Requester ?? "";
            this.comboBoxPriority.SelectedItem = Request.Priority;
            this.comboBoxStatus.SelectedItem = Request.Status;
            this.textBoxAssignee.Text = Request.Assignee ?? "";
            this.textBoxCategory.Text = Request.Category ?? "";
            this.textBoxComments.Text = Request.Comments ?? "";
            this.dateTimePickerRequest.Value = Request.RequestDate;

            if (Request.CompletedDate.HasValue)
            {
                this.checkBoxCompletedDate.Checked = true;
                this.dateTimePickerCompleted.Value = Request.CompletedDate.Value;
                this.dateTimePickerCompleted.Enabled = true;
            }
            else
            {
                this.checkBoxCompletedDate.Checked = false;
                this.dateTimePickerCompleted.Enabled = false;
            }
        }

        private void CheckBoxCompletedDate_CheckedChanged(object sender, EventArgs e)
        {
            this.dateTimePickerCompleted.Enabled = this.checkBoxCompletedDate.Checked;
            if (!this.checkBoxCompletedDate.Checked)
            {
                this.dateTimePickerCompleted.Value = DateTime.Now;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            SaveRequestData();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(this.textBoxTitle.Text))
            {
                MessageBox.Show("タイトルは必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.textBoxRequester.Text))
            {
                MessageBox.Show("要求者は必須です。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxRequester.Focus();
                return false;
            }

            if (this.comboBoxPriority.SelectedItem == null)
            {
                MessageBox.Show("優先度を選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.comboBoxPriority.Focus();
                return false;
            }

            if (this.comboBoxStatus.SelectedItem == null)
            {
                MessageBox.Show("ステータスを選択してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.comboBoxStatus.Focus();
                return false;
            }

            return true;
        }

        private void SaveRequestData()
        {
            Request.Title = this.textBoxTitle.Text.Trim();
            Request.Description = this.textBoxDescription.Text.Trim();
            Request.Requester = this.textBoxRequester.Text.Trim();
            Request.Priority = (Priority)this.comboBoxPriority.SelectedItem;
            Request.Status = (Status)this.comboBoxStatus.SelectedItem;
            Request.Assignee = this.textBoxAssignee.Text.Trim();
            Request.Category = this.textBoxCategory.Text.Trim();
            Request.Comments = this.textBoxComments.Text.Trim();
            Request.RequestDate = this.dateTimePickerRequest.Value;
            Request.CompletedDate = this.checkBoxCompletedDate.Checked ? this.dateTimePickerCompleted.Value : (DateTime?)null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private System.ComponentModel.IContainer components = null;
        private Label labelTitle;
        private TextBox textBoxTitle;
        private Label labelDescription;
        private TextBox textBoxDescription;
        private Label labelRequester;
        private TextBox textBoxRequester;
        private Label labelPriority;
        private ComboBox comboBoxPriority;
        private Label labelStatus;
        private ComboBox comboBoxStatus;
        private Label labelAssignee;
        private TextBox textBoxAssignee;
        private Label labelCategory;
        private TextBox textBoxCategory;
        private Label labelRequestDate;
        private DateTimePicker dateTimePickerRequest;
        private CheckBox checkBoxCompletedDate;
        private DateTimePicker dateTimePickerCompleted;
        private Label labelComments;
        private TextBox textBoxComments;
        private Button buttonOK;
        private Button buttonCancel;
        #endregion
    }
}
```

```csharp name=ImprovementRequestManager.cs
using System;
using System.Windows.Forms;
using ImprovementRequestManager.Forms;

namespace ImprovementRequestManager
{
    /// <summary>
    /// 改善要望管理ライブラリのメインクラス
    /// 各アプリケーションから呼び出すためのエントリーポイント
    /// </summary>
    public static class ImprovementRequestManager
    {
        /// <summary>
        /// 改善要望管理フォームを表示する
        /// </summary>
        /// <param name="jsonFilePath">JSONファイルのパス（サーバー上のパスを指定）</param>
        /// <param name="owner">親ウィンドウ（オプション）</param>
        /// <returns>フォームの表示結果</returns>
        public static DialogResult ShowManager(string jsonFilePath, IWin32Window owner = null)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentException("JSONファイルのパスが指定されていません。", nameof(jsonFilePath));

            try
            {
                using (var form = new ImprovementRequestForm(jsonFilePath))
                {
                    return owner != null ? form.ShowDialog(owner) : form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"改善要望管理システムを開くことができませんでした。\n\n詳細: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return DialogResult.Abort;
            }
        }

        /// <summary>
        /// 改善要望管理フォームをモードレスで表示する
        /// </summary>
        /// <param name="jsonFilePath">JSONファイルのパス（サーバー上のパスを指定）</param>
        /// <returns>作成されたフォームインスタンス</returns>
        public static Form ShowManagerModeless(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentException("JSONファイルのパスが指定されていません。", nameof(jsonFilePath));

            try
            {
                var form = new ImprovementRequestForm(jsonFilePath);
                form.Show();
                return form;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"改善要望管理システムを開くことができませんでした。\n\n詳細: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
```

```xml name=ImprovementRequestManager.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <GenerateAssemblyInfo>true</GenerateAssemblyInfo>
    <AssemblyTitle>ImprovementRequestManager</AssemblyTitle>
    <AssemblyDescription>改善要望管理ライブラリ</AssemblyDescription>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>

</Project>
```

````markdown name=README.md
# 改善要望管理ライブラリ

## 概要
このライブラリは、Winformsアプリケーション用の改善要望管理システムです。JSONファイルでデータを管理し、複数のPCから同時にアクセスしても整合性を保つことができる汎用的な設計になっています。

## 特徴
- **JSONファイルベース**: サーバー上のJSONファイルでデータを管理
- **排他制御**: ファイルロックによる整合性保証
- **汎用設計**: 各アプリケーションから簡単に組み込み可能
- **非同期処理**: UIをブロックしない非同期操作
- **入力検証**: データの整合性を保つ入力チェック

## 使用方法

### 基本的な使用方法
```csharp
// モーダルダイアログとして表示
string jsonPath = @"\\server\shared\improvement_requests.json";
var result = ImprovementRequestManager.ShowManager(jsonPath, this);

// モードレスウィンドウとして表示
var form = ImprovementRequestManager.ShowManagerModeless(jsonPath);
```

### アプリケーションへの組み込み例
```csharp
private void button1_Click(object sender, EventArgs e)
{
    try
    {
        string serverPath = @"\\fileserver\shared\requests\improvement_requests.json";
        ImprovementRequestManager.ShowManager(serverPath, this);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"エラー: {ex.Message}");
    }
}
```

## データモデル

### ImprovementRequest
改善要望を表すメインのデータモデルです。

- **Id**: 一意識別子（自動生成）
- **Title**: タイトル（必須）
- **Description**: 詳細説明
- **Requester**: 要求者（必須）
- **RequestDate**: 要求日
- **Priority**: 優先度（Low, Medium, High, Critical）
- **Status**: ステータス（Open, InProgress, UnderReview, Completed, Rejected, OnHold）
- **Assignee**: 担当者
- **CompletedDate**: 完了日
- **Category**: カテゴリ
- **Comments**: コメント

## アーキテクチャ

### ファイル構成
```
ImprovementRequestManager/
├── Models/
│   └── ImprovementRequest.cs      # データモデル
├── Services/
│   └── ImprovementRequestService.cs # データアクセス層
├── Forms/
│   ├── ImprovementRequestForm.cs     # メインフォーム
│   └── ImprovementRequestEditForm.cs # 編集フォーム
├── ImprovementRequestManager.cs      # エントリーポイント
└── ImprovementRequestManager.csproj  # プロジェクトファイル
```

### 排他制御
- `SemaphoreSlim`を使用したスレッドレベルの排他制御
- ファイルアクセス時の一時ファイル利用による原子性保証
- タイムアウト機能により無限待機を防止

### 非同期処理
すべてのファイルI/O操作は非同期で実行され、UIの応答性を保ちます。

## インストール要件
- .NET 6.0 Windows Forms
- Newtonsoft.Json パッケージ

## セットアップ

1. プロジェクトをビルドしてDLLを生成
2. 参照を追加して使用するアプリケーションに組み込み
3. サーバー上に共有フォルダを作成してJSONファイルのパスを指定

## 注意事項
- JSONファイルのパスは書き込み権限のあるネットワークパスを指定してください
- 大量のデータを扱う場合は、パフォーマンスを考慮してデータベースの使用を検討してください
- ネットワーク障害に対する適切なエラーハンドリングを実装してください

## ライセンス
このライブラリは汎用的な設計により、様々なアプリケーションで再利用可能です。
````

この改善要望管理ライブラリの主な特徴：

1. **排他制御**: SemaphoreSlimとファイルロックで同時アクセス時の整合性を保証
2. **汎用設計**: 静的メソッドで簡単に各アプリから呼び出し可能
3. **非同期処理**: UIをブロックしない非同期ファイルI/O
4. **原子性保証**: 一時ファイルを使った安全な書き込み処理
5. **入力検証**: データの整合性を保つ検証機能
6. **エラーハンドリング**: 適切なエラー処理とユーザーへの通知

各アプリケーションからは以下のように簡単に呼び出せます：

```csharp
// 使用例
ImprovementRequestManager.ShowManager(@"\\server\shared\requests.json", this);
```

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
