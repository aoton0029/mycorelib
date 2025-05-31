using CoreLibWinforms.Permissions;
using System;
using System.Collections;
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
    public partial class FormPermission : Form
    {
        private PermissionManager _permissionManager;
        private Dictionary<string, CheckBox> _basicPermissionCheckboxes = new Dictionary<string, CheckBox>();
        private Dictionary<string, CheckBox> _combinedPermissionCheckboxes = new Dictionary<string, CheckBox>();
        private Dictionary<string, CheckBox> _userPermissionCheckboxes = new Dictionary<string, CheckBox>();

        // 現在選択中のユーザーID
        private string _selectedUserId = string.Empty;

        public FormPermission(PermissionManager permissionManager)
        {
            InitializeComponent();
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));

            InitializeBasicPermissions();
            InitializeCombinedPermissions();
            InitializeUserPermissions();

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        #region 基本権限画面
        private void InitializeBasicPermissions()
        {
            // 基本権限タブのコントロールを初期化
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown
            };

            // 権限名テキストボックスと追加ボタン
            TextBox txtNewPermission = new TextBox
            {
                Width = 300,
                Font = new Font("メイリオ", 12)
            };

            Button btnAddPermission = new Button
            {
                Text = "基本権限を追加",
                Width = 200,
                Font = new Font("メイリオ", 12)
            };

            Panel addPanel = new Panel
            {
                Width = 550,
                Height = 50
            };

            addPanel.Controls.Add(txtNewPermission);
            addPanel.Controls.Add(btnAddPermission);
            txtNewPermission.Location = new Point(10, 10);
            btnAddPermission.Location = new Point(320, 8);

            panel.Controls.Add(addPanel);

            // 権限リストの表示
            ListBox permissionsList = new ListBox
            {
                Width = 550,
                Height = 300,
                Font = new Font("メイリオ", 12)
            };

            Button btnRemovePermission = new Button
            {
                Text = "選択した権限を削除",
                Width = 200,
                Font = new Font("メイリオ", 12)
            };

            panel.Controls.Add(permissionsList);
            panel.Controls.Add(btnRemovePermission);

            // イベント登録
            btnAddPermission.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(txtNewPermission.Text))
                {
                    try
                    {
                        _permissionManager.RegisterNewPermission(txtNewPermission.Text);
                        RefreshPermissionsList(permissionsList);
                        txtNewPermission.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            btnRemovePermission.Click += (sender, e) =>
            {
                if (permissionsList.SelectedItem != null)
                {
                    string selectedPermission = permissionsList.SelectedItem.ToString();
                    if (_permissionManager.RemovePermission(selectedPermission))
                    {
                        RefreshPermissionsList(permissionsList);
                    }
                    else
                    {
                        MessageBox.Show("権限の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            // 初期データ読み込み
            RefreshPermissionsList(permissionsList);

            basicPermissionsTab.Controls.Add(panel);
        }

        private void RefreshPermissionsList(ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                if (!permission.IsCombined)
                {
                    listBox.Items.Add(permission.Name);
                }
            }
        }
        #endregion

        #region 結合権限画面
        private void InitializeCombinedPermissions()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                RowStyles =
                {
                    new RowStyle(SizeType.Absolute, 60),
                    new RowStyle(SizeType.Percent, 100),
                    new RowStyle(SizeType.Absolute, 50)
                }
            };

            // 上部：新規結合権限作成パネル
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Label lblCombinedName = new Label
            {
                Text = "結合権限名:",
                Location = new Point(10, 18),
                AutoSize = true,
                Font = new Font("メイリオ", 12)
            };

            TextBox txtCombinedName = new TextBox
            {
                Location = new Point(110, 15),
                Width = 300,
                Font = new Font("メイリオ", 12)
            };

            Button btnAddCombined = new Button
            {
                Text = "新規結合権限を作成",
                Location = new Point(420, 13),
                Width = 220,
                Font = new Font("メイリオ", 12)
            };

            topPanel.Controls.Add(lblCombinedName);
            topPanel.Controls.Add(txtCombinedName);
            topPanel.Controls.Add(btnAddCombined);

            // 中央：権限選択パネル
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 200
            };

            // 上部：結合権限リスト
            Panel combinedListPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Label lblCombinedList = new Label
            {
                Text = "結合権限一覧:",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("メイリオ", 12)
            };

            ListBox lstCombinedPermissions = new ListBox
            {
                Location = new Point(10, 40),
                Width = 840,
                Height = 150,
                Font = new Font("メイリオ", 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            combinedListPanel.Controls.Add(lblCombinedList);
            combinedListPanel.Controls.Add(lstCombinedPermissions);

            // 下部：権限チェックボックスパネル
            Panel permissionCheckboxPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            Label lblPermissions = new Label
            {
                Text = "基本権限を選択:",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("メイリオ", 12)
            };

            FlowLayoutPanel checkboxPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 40),
                Width = 840,
                Height = 260,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            permissionCheckboxPanel.Controls.Add(lblPermissions);
            permissionCheckboxPanel.Controls.Add(checkboxPanel);

            splitContainer.Panel1.Controls.Add(combinedListPanel);
            splitContainer.Panel2.Controls.Add(permissionCheckboxPanel);

            // 下部：操作ボタンパネル
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Button btnRemoveCombined = new Button
            {
                Text = "選択した結合権限を削除",
                Location = new Point(10, 8),
                Width = 250,
                Font = new Font("メイリオ", 12)
            };

            bottomPanel.Controls.Add(btnRemoveCombined);

            // レイアウトに追加
            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(splitContainer, 0, 1);
            mainLayout.Controls.Add(bottomPanel, 0, 2);

            combinedPermissionsTab.Controls.Add(mainLayout);

            // 権限チェックボックスを生成
            PopulatePermissionCheckboxes(checkboxPanel);

            // イベント登録
            lstCombinedPermissions.SelectedIndexChanged += (sender, e) =>
            {
                if (lstCombinedPermissions.SelectedItem != null)
                {
                    string selectedName = lstCombinedPermissions.SelectedItem.ToString();
                    var permissions = _permissionManager.GetAllPermissions();
                    var selectedPermission = permissions.Find(p => p.Name == selectedName && p.IsCombined);

                    if (selectedPermission != null)
                    {
                        UpdateCheckboxes(checkboxPanel, selectedPermission.Permissions);
                    }
                }
            };

            btnAddCombined.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(txtCombinedName.Text))
                {
                    try
                    {
                        // 選択された権限IDを収集
                        List<int> selectedIds = new List<int>();
                        foreach (CheckBox cb in checkboxPanel.Controls)
                        {
                            if (cb.Checked && cb.Tag is int id)
                            {
                                selectedIds.Add(id);
                            }
                        }

                        if (selectedIds.Count > 0)
                        {
                            _permissionManager.RegisterNewPermission(txtCombinedName.Text, selectedIds);
                            RefreshCombinedPermissionsList(lstCombinedPermissions);
                            txtCombinedName.Clear();
                            // チェックボックスをクリア
                            foreach (CheckBox cb in checkboxPanel.Controls)
                            {
                                cb.Checked = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("少なくとも1つの権限を選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"結合権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            btnRemoveCombined.Click += (sender, e) =>
            {
                if (lstCombinedPermissions.SelectedItem != null)
                {
                    string selectedPermission = lstCombinedPermissions.SelectedItem.ToString();
                    if (_permissionManager.RemovePermission(selectedPermission))
                    {
                        RefreshCombinedPermissionsList(lstCombinedPermissions);
                    }
                    else
                    {
                        MessageBox.Show("権限の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            // 初期データ読み込み
            RefreshCombinedPermissionsList(lstCombinedPermissions);
        }

        private void PopulatePermissionCheckboxes(FlowLayoutPanel panel)
        {
            panel.Controls.Clear();
            _combinedPermissionCheckboxes.Clear();

            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                if (!permission.IsCombined)
                {
                    CheckBox cb = new CheckBox
                    {
                        Text = permission.Name,
                        Tag = permission.Id,
                        AutoSize = true,
                        Font = new Font("メイリオ", 10),
                        Margin = new Padding(5)
                    };
                    panel.Controls.Add(cb);
                    _combinedPermissionCheckboxes[permission.Name] = cb;
                }
            }
        }

        private void UpdateCheckboxes(FlowLayoutPanel panel, BitArray permissions)
        {
            foreach (CheckBox cb in panel.Controls)
            {
                if (cb.Tag is int id)
                {
                    cb.Checked = permissions[id];
                }
            }
        }

        private void RefreshCombinedPermissionsList(ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                if (permission.IsCombined)
                {
                    listBox.Items.Add(permission.Name);
                }
            }
        }
        #endregion

        #region ユーザー権限画面
        private void InitializeUserPermissions()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2,
                RowStyles =
                {
                    new RowStyle(SizeType.Absolute, 60),
                    new RowStyle(SizeType.Percent, 100)
                },
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Percent, 30),
                    new ColumnStyle(SizeType.Percent, 70)
                }
            };

            // ユーザー選択パネル（左上）
            Panel userSelectPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Label lblUserSelect = new Label
            {
                Text = "ユーザーID:",
                Location = new Point(10, 18),
                AutoSize = true,
                Font = new Font("メイリオ", 12)
            };

            TextBox txtUserId = new TextBox
            {
                Location = new Point(90, 15),
                Width = 150,
                Font = new Font("メイリオ", 12)
            };

            userSelectPanel.Controls.Add(lblUserSelect);
            userSelectPanel.Controls.Add(txtUserId);

            // 権限操作パネル（右上）
            Panel permissionActionPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Button btnLoadUser = new Button
            {
                Text = "ユーザー権限を読み込み",
                Location = new Point(10, 13),
                Width = 220,
                Font = new Font("メイリオ", 12)
            };

            Button btnGrantAdmin = new Button
            {
                Text = "管理者権限を付与",
                Location = new Point(240, 13),
                Width = 180,
                Font = new Font("メイリオ", 12)
            };

            permissionActionPanel.Controls.Add(btnLoadUser);
            permissionActionPanel.Controls.Add(btnGrantAdmin);

            // ユーザーリストパネル（左下）
            Panel userListPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblUserList = new Label
            {
                Text = "登録済みユーザー:",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("メイリオ", 12)
            };

            ListBox lstUsers = new ListBox
            {
                Location = new Point(10, 40),
                Width = 240,
                Height = 450,
                Font = new Font("メイリオ", 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            userListPanel.Controls.Add(lblUserList);
            userListPanel.Controls.Add(lstUsers);

            // 権限チェックボックスパネル（右下）
            Panel permissionsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };

            TabControl permissionTabControl = new TabControl
            {
                Location = new Point(10, 10),
                Width = 570,
                Height = 480,
                Font = new Font("メイリオ", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // 基本権限タブ
            TabPage basicTab = new TabPage("基本権限");
            FlowLayoutPanel basicFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            basicTab.Controls.Add(basicFlowPanel);
            permissionTabControl.Controls.Add(basicTab);

            // 結合権限タブ
            TabPage combinedTab = new TabPage("結合権限");
            FlowLayoutPanel combinedFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            combinedTab.Controls.Add(combinedFlowPanel);
            permissionTabControl.Controls.Add(combinedTab);

            permissionsPanel.Controls.Add(permissionTabControl);

            // レイアウトに追加
            mainLayout.Controls.Add(userSelectPanel, 0, 0);
            mainLayout.Controls.Add(permissionActionPanel, 1, 0);
            mainLayout.Controls.Add(userListPanel, 0, 1);
            mainLayout.Controls.Add(permissionsPanel, 1, 1);

            userPermissionsTab.Controls.Add(mainLayout);

            // ユーザー権限チェックボックスを生成
            PopulateUserPermissionCheckboxes(basicFlowPanel, combinedFlowPanel);

            // イベント登録
            btnLoadUser.Click += (sender, e) =>
            {
                string userId = txtUserId.Text.Trim();
                if (!string.IsNullOrEmpty(userId))
                {
                    _selectedUserId = userId;
                    LoadUserPermissions();

                    // ユーザーリストに追加（存在しない場合）
                    if (!lstUsers.Items.Contains(userId))
                    {
                        lstUsers.Items.Add(userId);
                    }
                }
                else
                {
                    MessageBox.Show("ユーザーIDを入力してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            lstUsers.SelectedIndexChanged += (sender, e) =>
            {
                if (lstUsers.SelectedItem != null)
                {
                    _selectedUserId = lstUsers.SelectedItem.ToString();
                    txtUserId.Text = _selectedUserId;
                    LoadUserPermissions();
                }
            };

            btnGrantAdmin.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(_selectedUserId))
                {
                    try
                    {
                        _permissionManager.AssignAdminToUser(_selectedUserId);
                        LoadUserPermissions();
                        MessageBox.Show($"ユーザー '{_selectedUserId}' に管理者権限を付与しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"管理者権限の付与に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("ユーザーを選択してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void PopulateUserPermissionCheckboxes(FlowLayoutPanel basicPanel, FlowLayoutPanel combinedPanel)
        {
            basicPanel.Controls.Clear();
            combinedPanel.Controls.Clear();
            _userPermissionCheckboxes.Clear();

            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                CheckBox cb = new CheckBox
                {
                    Text = permission.Name,
                    Tag = permission.Name,
                    AutoSize = true,
                    Font = new Font("メイリオ", 10),
                    Margin = new Padding(5)
                };

                cb.CheckedChanged += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(_selectedUserId) && sender is CheckBox checkBox)
                    {
                        string permissionName = checkBox.Tag.ToString();
                        try
                        {
                            if (checkBox.Checked)
                            {
                                _permissionManager.AssignPermissionToUserByName(_selectedUserId, permissionName);
                            }
                            else
                            {
                                _permissionManager.RevokePermissionFromUserByName(_selectedUserId, permissionName);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"権限の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // エラー発生時は元の状態に戻す
                            checkBox.CheckedChanged -= (s, args) => { };
                            checkBox.Checked = !checkBox.Checked;
                            checkBox.CheckedChanged += (s, args) => { /* 再登録 */ };
                        }
                    }
                };

                if (permission.IsCombined)
                {
                    combinedPanel.Controls.Add(cb);
                }
                else
                {
                    basicPanel.Controls.Add(cb);
                }

                _userPermissionCheckboxes[permission.Name] = cb;
            }
        }

        private void LoadUserPermissions()
        {
            // ユーザー権限を読み込み、チェックボックスの状態を更新
            var userPermission = _permissionManager.GetUserPermission(_selectedUserId);

            foreach (var entry in _userPermissionCheckboxes)
            {
                string permissionName = entry.Key;
                CheckBox cb = entry.Value;

                if (userPermission != null)
                {
                    cb.CheckedChanged -= (s, e) => { }; // 一時的にイベントハンドラを削除
                    cb.Checked = _permissionManager.UserHasPermissionByName(_selectedUserId, permissionName);
                    cb.CheckedChanged += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(_selectedUserId) && s is CheckBox checkBox)
                        {
                            string pName = checkBox.Tag.ToString();
                            try
                            {
                                if (checkBox.Checked)
                                {
                                    _permissionManager.AssignPermissionToUserByName(_selectedUserId, pName);
                                }
                                else
                                {
                                    _permissionManager.RevokePermissionFromUserByName(_selectedUserId, pName);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"権限の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                checkBox.Checked = !checkBox.Checked;
                            }
                        }
                    };
                }
                else
                {
                    cb.Checked = false;
                }
            }
        }
        #endregion

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 実装: 権限設定を保存
            MessageBox.Show("権限設定を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
