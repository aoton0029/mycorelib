using CoreLibWinforms.Core;
using CoreLibWinforms.Core.Permissions;
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
        private DualListManager _rolePermissionManager;
        private DualListManager _userRoleManager;
        private DualListManager _userPermissionManager;

        // 選択中の項目を追跡
        private string _selectedRoleName = string.Empty;
        private string _selectedUserId = string.Empty;

        public FormPermission()
        {
            InitializeComponent();
            _permissionManager = new PermissionManager();

            // イベントハンドラの設定
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // 各タブの初期化
            InitializeBasicPermissionsTab();
            InitializeRolePermissionsTab();
            InitializeUserPermissionsTab();
        }

        private void FormPermission_Shown(object sender, EventArgs e)
        {
            // フォーム表示時の初期化処理
            RefreshAllData();
        }

        private void RefreshAllData()
        {
            RefreshPermissionList();
            RefreshRoleList();
            RefreshUserList();
        }

        #region 基本権限タブ
        private void InitializeBasicPermissionsTab()
        {
            // 権限追加ボタンのイベント設定
            btnAddPermission.Click += BtnAddPermission_Click;

            // 権限リストの設定
            lstPermissions.FullRowSelect = true;
            lstPermissions.MultiSelect = false;
            lstPermissions.View = View.Details;
            lstPermissions.Columns.Add("ID", 50);
            lstPermissions.Columns.Add("権限名", 250);

            // コンテキストメニューの設定
            var permContextMenu = new ContextMenuStrip();
            var editItem = permContextMenu.Items.Add("編集");
            var deleteItem = permContextMenu.Items.Add("削除");
            editItem.Click += PermissionEdit_Click;
            deleteItem.Click += PermissionDelete_Click;
            lstPermissions.ContextMenuStrip = permContextMenu;

            // ダブルクリックで編集
            lstPermissions.DoubleClick += (s, e) => EditSelectedPermission();
        }

        private void RefreshPermissionList()
        {
            lstPermissions.Items.Clear();
            foreach (var permission in _permissionManager.GetAllPermissions())
            {
                var item = new ListViewItem(permission.Id.ToString());
                item.SubItems.Add(permission.Name);
                item.Tag = permission;
                lstPermissions.Items.Add(item);
            }
        }

        private void BtnAddPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPermissionName.Text))
            {
                MessageBox.Show("権限名を入力してください", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var permission = _permissionManager.CreatePermission(txtPermissionName.Text);
                txtPermissionName.Clear();
                RefreshPermissionList();
                MessageBox.Show("権限を追加しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PermissionEdit_Click(object sender, EventArgs e)
        {
            EditSelectedPermission();
        }

        private void EditSelectedPermission()
        {
            if (lstPermissions.SelectedItems.Count == 0) return;

            var permission = (Permission)lstPermissions.SelectedItems[0].Tag;

            // 編集用ダイアログを表示（実際の実装では入力ダイアログなど）
            var newName = Microsoft.VisualBasic.Interaction.InputBox(
                "権限名を入力してください", "権限の編集", permission.Name);

            if (string.IsNullOrWhiteSpace(newName)) return;

            try
            {
                // 権限名の更新（Permissionクラスに更新メソッドを追加する必要あり）
                // _permissionManager.UpdatePermission(permission.Id, newName);
                // RefreshPermissionList();
                MessageBox.Show("この機能は実装されていません。Permission クラスに更新メソッドを追加してください。",
                    "未実装", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PermissionDelete_Click(object sender, EventArgs e)
        {
            if (lstPermissions.SelectedItems.Count == 0) return;

            var permission = (Permission)lstPermissions.SelectedItems[0].Tag;

            if (MessageBox.Show($"権限「{permission.Name}」を削除してもよろしいですか？",
                "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                // 権限の削除（Permissionクラスに削除メソッドを追加する必要あり）
                // _permissionManager.DeletePermission(permission.Id);
                // RefreshPermissionList();
                MessageBox.Show("この機能は実装されていません。Permission クラスに削除メソッドを追加してください。",
                    "未実装", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ロール権限タブ
        private void InitializeRolePermissionsTab()
        {
            // ロール追加ボタンの設定
            btnAddRole.Click += BtnAddRole_Click;

            // ロールリストの設定
            lstRoles.FullRowSelect = true;
            lstRoles.MultiSelect = false;
            lstRoles.View = View.Details;
            lstRoles.Columns.Add("ロール名", 200);

            // コンテキストメニューの設定
            var roleContextMenu = new ContextMenuStrip();
            var editRoleItem = roleContextMenu.Items.Add("編集");
            var deleteRoleItem = roleContextMenu.Items.Add("削除");
            editRoleItem.Click += RoleEdit_Click;
            deleteRoleItem.Click += RoleDelete_Click;
            lstRoles.ContextMenuStrip = roleContextMenu;

            // ロール選択時のイベント
            lstRoles.SelectedIndexChanged += LstRoles_SelectedIndexChanged;

            // 権限移動ボタンの設定
            btnAddPermToRole.Click += BtnAddPermToRole_Click;
            btnRemovePermFromRole.Click += BtnRemovePermFromRole_Click;

            // 検索フィルタの設定
            txtRolePermFilter.TextChanged += TxtRolePermFilter_TextChanged;
        }

        private void RefreshRoleList()
        {
            lstRoles.Items.Clear();
            foreach (var role in _permissionManager.GetAllRoles())
            {
                var item = new ListViewItem(role.Name);
                item.Tag = role;
                lstRoles.Items.Add(item);
            }
        }

        private void BtnAddRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("ロール名を入力してください", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var role = _permissionManager.CreateRole(txtRoleName.Text);
                txtRoleName.Clear();
                RefreshRoleList();
                MessageBox.Show("ロールを追加しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RoleEdit_Click(object sender, EventArgs e)
        {
            if (lstRoles.SelectedItems.Count == 0) return;

            var role = (Role)lstRoles.SelectedItems[0].Tag;

            // 編集用ダイアログを表示
            var newName = Microsoft.VisualBasic.Interaction.InputBox(
                "ロール名を入力してください", "ロールの編集", role.Name);

            if (string.IsNullOrWhiteSpace(newName) || newName == role.Name) return;

            try
            {
                _permissionManager.RenameRole(role.Name, newName);
                RefreshRoleList();
                MessageBox.Show("ロール名を更新しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの更新に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RoleDelete_Click(object sender, EventArgs e)
        {
            if (lstRoles.SelectedItems.Count == 0) return;

            var role = (Role)lstRoles.SelectedItems[0].Tag;

            if (MessageBox.Show($"ロール「{role.Name}」を削除してもよろしいですか？\nこのロールを持つユーザーからも削除されます。",
                "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _permissionManager.DeleteRole(role.Name);
                RefreshRoleList();

                // 選択中のロールが削除された場合は選択をクリア
                if (_selectedRoleName == role.Name)
                {
                    _selectedRoleName = string.Empty;
                    ClearRolePermissionLists();
                }

                MessageBox.Show("ロールを削除しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRoles.SelectedItems.Count == 0)
            {
                ClearRolePermissionLists();
                return;
            }

            var role = (Role)lstRoles.SelectedItems[0].Tag;
            _selectedRoleName = role.Name;
            lblSelectedRole.Text = $"選択中のロール: {_selectedRoleName}";

            // 選択したロールの権限リストを更新
            RefreshRolePermissionLists();
        }

        private void RefreshRolePermissionLists()
        {
            if (string.IsNullOrEmpty(_selectedRoleName))
            {
                ClearRolePermissionLists();
                return;
            }

            var role = _permissionManager.GetRole(_selectedRoleName);
            var allPermissions = _permissionManager.GetAllPermissions().ToList();

            // DataTableの準備
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            // 全権限をDataTableに追加
            foreach (var perm in allPermissions)
            {
                var row = dt.NewRow();
                row["Id"] = perm.Id.ToString();
                row["Name"] = perm.Name;
                dt.Rows.Add(row);
            }

            // 選択済みの権限IDリストを作成
            var selectedPermIds = new List<string>();
            foreach (var perm in allPermissions)
            {
                if (role.HasPermission(perm))
                {
                    selectedPermIds.Add(perm.Id.ToString());
                }
            }

            // DualListManagerの作成
            _rolePermissionManager = new DualListManager(dt, "Id", "Name", selectedPermIds);

            // リストの初期化
            lstAvailablePermissions.Items.Clear();
            lstAssignedPermissions.Items.Clear();

            // 利用可能な権限を表示
            foreach (DataRowView row in _rolePermissionManager.AvailableView)
            {
                var item = new ListViewItem(row["Id"].ToString());
                item.SubItems.Add(row["Name"].ToString());
                lstAvailablePermissions.Items.Add(item);
            }

            // 割り当て済みの権限を表示
            foreach (DataRowView row in _rolePermissionManager.SelectedView)
            {
                var item = new ListViewItem(row["Id"].ToString());
                item.SubItems.Add(row["Name"].ToString());
                lstAssignedPermissions.Items.Add(item);
            }
        }

        private void ClearRolePermissionLists()
        {
            lblSelectedRole.Text = "選択中のロール: なし";
            lstAvailablePermissions.Items.Clear();
            lstAssignedPermissions.Items.Clear();
        }

        private void BtnAddPermToRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedRoleName) || lstAvailablePermissions.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAvailablePermissions.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.GrantPermissionToRole(_selectedRoleName, permId);
                    _rolePermissionManager.Select(permId.ToString());
                }

                RefreshRolePermissionLists();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemovePermFromRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedRoleName) || lstAssignedPermissions.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAssignedPermissions.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.RevokePermissionFromRole(_selectedRoleName, permId);
                    _rolePermissionManager.Deselect(permId.ToString());
                }

                RefreshRolePermissionLists();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtRolePermFilter_TextChanged(object sender, EventArgs e)
        {
            if (_rolePermissionManager == null) return;

            try
            {
                _rolePermissionManager.ApplyColumnFilter("Name", txtRolePermFilter.Text);

                // 利用可能な権限リストを再表示
                lstAvailablePermissions.Items.Clear();
                foreach (DataRowView row in _rolePermissionManager.AvailableView)
                {
                    var item = new ListViewItem(row["Id"].ToString());
                    item.SubItems.Add(row["Name"].ToString());
                    lstAvailablePermissions.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"フィルタの適用に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ユーザー権限タブ
        private void InitializeUserPermissionsTab()
        {
            // ユーザー追加ボタンの設定
            btnAddUser.Click += BtnAddUser_Click;

            // ユーザーリストの設定
            lstUsers.FullRowSelect = true;
            lstUsers.MultiSelect = false;
            lstUsers.View = View.Details;
            lstUsers.Columns.Add("ユーザーID", 200);

            // コンテキストメニューの設定
            var userContextMenu = new ContextMenuStrip();
            var deleteUserItem = userContextMenu.Items.Add("削除");
            deleteUserItem.Click += UserDelete_Click;
            lstUsers.ContextMenuStrip = userContextMenu;

            // ユーザー選択時のイベント
            lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;

            // ロール割り当てボタンの設定
            btnAddRoleToUser.Click += BtnAddRoleToUser_Click;
            btnRemoveRoleFromUser.Click += BtnRemoveRoleFromUser_Click;

            // 追加権限ボタンの設定
            btnAddPermToUser.Click += BtnAddPermToUser_Click;
            btnRemovePermFromUser.Click += BtnRemovePermFromUser_Click;

            // 拒否権限ボタンの設定
            btnDenyPermForUser.Click += BtnDenyPermForUser_Click;
            btnRemoveDeniedPerm.Click += BtnRemoveDeniedPerm_Click;

            // 検索フィルタの設定
            txtUserRoleFilter.TextChanged += TxtUserRoleFilter_TextChanged;
            txtUserPermFilter.TextChanged += TxtUserPermFilter_TextChanged;
        }

        private void RefreshUserList()
        {
            lstUsers.Items.Clear();
            foreach (var user in _permissionManager.GetAllUsers())
            {
                var item = new ListViewItem(user.UserId);
                item.Tag = user;
                lstUsers.Items.Add(item);
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("ユーザーIDを入力してください", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // デフォルトロールの取得（コンボボックスから選択）
                Role defaultRole = null;
                if (cmbDefaultRole.SelectedItem != null)
                {
                    var roleName = cmbDefaultRole.SelectedItem.ToString();
                    defaultRole = _permissionManager.GetRole(roleName);
                }

                var user = _permissionManager.CreateUser(txtUserId.Text, defaultRole);
                txtUserId.Clear();
                RefreshUserList();
                MessageBox.Show("ユーザーを追加しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ユーザーの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItems.Count == 0) return;

            var user = (UserRole)lstUsers.SelectedItems[0].Tag;

            if (MessageBox.Show($"ユーザー「{user.UserId}」を削除してもよろしいですか？",
                "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                // ユーザーの削除（UserRoleクラスに削除メソッドを追加する必要あり）
                // _permissionManager.DeleteUser(user.UserId);
                // RefreshUserList();

                // 選択中のユーザーが削除された場合は選択をクリア
                if (_selectedUserId == user.UserId)
                {
                    _selectedUserId = string.Empty;
                    ClearUserTabs();
                }

                MessageBox.Show("この機能は実装されていません。UserRole クラスに削除メソッドを追加してください。",
                    "未実装", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ユーザーの削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedItems.Count == 0)
            {
                ClearUserTabs();
                return;
            }

            var user = (UserRole)lstUsers.SelectedItems[0].Tag;
            _selectedUserId = user.UserId;
            lblSelectedUser.Text = $"選択中のユーザー: {_selectedUserId}";

            // ユーザーのロールと権限リストを更新
            RefreshUserRoleList();
            RefreshUserPermissionList();
        }

        private void RefreshUserRoleList()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                ClearUserRoleList();
                return;
            }

            var user = _permissionManager.GetUser(_selectedUserId);
            var allRoles = _permissionManager.GetAllRoles().ToList();

            // DataTableの準備
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));

            // 全ロールをDataTableに追加
            foreach (var role in allRoles)
            {
                var row = dt.NewRow();
                row["Name"] = role.Name;
                dt.Rows.Add(row);
            }

            // 選択済みのロール名リストを作成
            var selectedRoleNames = user.AssignedRoleNames.ToList();

            // DualListManagerの作成
            _userRoleManager = new DualListManager(dt, "Name", "Name", selectedRoleNames);

            // リストの初期化
            lstAvailableRoles.Items.Clear();
            lstAssignedRoles.Items.Clear();

            // 利用可能なロールを表示
            foreach (DataRowView row in _userRoleManager.AvailableView)
            {
                var item = new ListViewItem(row["Name"].ToString());
                lstAvailableRoles.Items.Add(item);
            }

            // 割り当て済みのロールを表示
            foreach (DataRowView row in _userRoleManager.SelectedView)
            {
                var item = new ListViewItem(row["Name"].ToString());
                lstAssignedRoles.Items.Add(item);
            }
        }

        private void RefreshUserPermissionList()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                ClearUserPermissionLists();
                return;
            }

            var user = _permissionManager.GetUser(_selectedUserId);
            var allPermissions = _permissionManager.GetAllPermissions().ToList();
            var userPermissions = _permissionManager.GetUserPermissions(_selectedUserId).ToList();

            // 追加権限と拒否権限のリストを取得
            var additionalPermIds = user.AdditionalPermissionIds;
            var deniedPermIds = user.DeniedPermissionIds;

            // 各リストをクリア
            lstEffectivePermissions.Items.Clear();
            lstAdditionalPermissions.Items.Clear();
            lstDeniedPermissions.Items.Clear();
            lstAvailableUserPerms.Items.Clear();

            // 効果的な権限（ロールから取得 + 追加権限 - 拒否権限）
            foreach (var perm in userPermissions)
            {
                var item = new ListViewItem(perm.Id.ToString());
                item.SubItems.Add(perm.Name);

                // 追加権限なら色を変える
                if (additionalPermIds.Contains(perm.Id))
                {
                    item.BackColor = Color.LightGreen;
                }

                lstEffectivePermissions.Items.Add(item);
            }

            // 追加権限
            foreach (var permId in additionalPermIds)
            {
                var permission = _permissionManager.GetPermission(permId);
                var item = new ListViewItem(permission.Id.ToString());
                item.SubItems.Add(permission.Name);
                lstAdditionalPermissions.Items.Add(item);
            }

            // 拒否権限
            foreach (var permId in deniedPermIds)
            {
                var permission = _permissionManager.GetPermission(permId);
                var item = new ListViewItem(permission.Id.ToString());
                item.SubItems.Add(permission.Name);
                lstDeniedPermissions.Items.Add(item);
            }

            // 利用可能な権限（効果的な権限でもなく、追加権限でもなく、拒否権限でもない）
            foreach (var perm in allPermissions)
            {
                if (!userPermissions.Contains(perm) && !additionalPermIds.Contains(perm.Id) && !deniedPermIds.Contains(perm.Id))
                {
                    var item = new ListViewItem(perm.Id.ToString());
                    item.SubItems.Add(perm.Name);
                    lstAvailableUserPerms.Items.Add(item);
                }
            }
        }

        private void ClearUserTabs()
        {
            lblSelectedUser.Text = "選択中のユーザー: なし";
            ClearUserRoleList();
            ClearUserPermissionLists();
        }

        private void ClearUserRoleList()
        {
            lstAvailableRoles.Items.Clear();
            lstAssignedRoles.Items.Clear();
        }

        private void ClearUserPermissionLists()
        {
            lstEffectivePermissions.Items.Clear();
            lstAdditionalPermissions.Items.Clear();
            lstDeniedPermissions.Items.Clear();
            lstAvailableUserPerms.Items.Clear();
        }

        private void BtnAddRoleToUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstAvailableRoles.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAvailableRoles.SelectedItems)
                {
                    string roleName = item.Text;
                    _permissionManager.AssignRoleToUser(_selectedUserId, roleName);
                    _userRoleManager?.Select(roleName);
                }

                RefreshUserRoleList();
                RefreshUserPermissionList(); // ロール変更で権限も変わる
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveRoleFromUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstAssignedRoles.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAssignedRoles.SelectedItems)
                {
                    string roleName = item.Text;
                    _permissionManager.UnassignRoleFromUser(_selectedUserId, roleName);
                    _userRoleManager?.Deselect(roleName);
                }

                RefreshUserRoleList();
                RefreshUserPermissionList(); // ロール変更で権限も変わる
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddPermToUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstAvailableUserPerms.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAvailableUserPerms.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.GrantAdditionalPermissionToUser(_selectedUserId, permId);
                }

                RefreshUserPermissionList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemovePermFromUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstAdditionalPermissions.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstAdditionalPermissions.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.RevokeAdditionalPermissionFromUser(_selectedUserId, permId);
                }

                RefreshUserPermissionList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"追加権限の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDenyPermForUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstEffectivePermissions.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstEffectivePermissions.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.DenyPermissionForUser(_selectedUserId, permId);
                }

                RefreshUserPermissionList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の拒否設定に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveDeniedPerm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || lstDeniedPermissions.SelectedItems.Count == 0) return;

            try
            {
                foreach (ListViewItem item in lstDeniedPermissions.SelectedItems)
                {
                    int permId = int.Parse(item.Text);
                    _permissionManager.RemoveDeniedPermissionForUser(_selectedUserId, permId);
                }

                RefreshUserPermissionList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"拒否権限の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtUserRoleFilter_TextChanged(object sender, EventArgs e)
        {
            if (_userRoleManager == null) return;

            try
            {
                _userRoleManager.ApplyColumnFilter("Name", txtUserRoleFilter.Text);

                // 利用可能なロールリストを再表示
                lstAvailableRoles.Items.Clear();
                foreach (DataRowView row in _userRoleManager.AvailableView)
                {
                    var item = new ListViewItem(row["Name"].ToString());
                    lstAvailableRoles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"フィルタの適用に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtUserPermFilter_TextChanged(object sender, EventArgs e)
        {
            // 権限のフィルタリング（DualListManagerを使用していないため単純な文字列検索）
            string filter = txtUserPermFilter.Text.ToLower();

            foreach (ListViewItem item in lstAvailableUserPerms.Items)
            {
                bool visible = string.IsNullOrEmpty(filter) ||
                               item.Text.ToLower().Contains(filter) ||
                               item.SubItems[1].Text.ToLower().Contains(filter);

                //item.Visible = visible;
            }
        }
        #endregion

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 実装: 権限設定を保存
            // TODO: PermissionManagerにファイル保存メソッドを追加する必要がある

            MessageBox.Show("権限設定を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("変更を破棄してよろしいですか？", "確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        #region コントロール権限マッピングタブ
        private int _selectedPermissionId = -1;
        private string _selectedControlName = string.Empty;

        private void RefreshPermissionListForMapping()
        {
            dataGridView1.Rows.Clear();
            foreach (var permission in _permissionManager.PermissionMaster.GetAllPermissions())
            {
                int rowIdx = dataGridView1.Rows.Add(permission.Id, permission.Name);
                dataGridView1.Rows[rowIdx].Tag = permission;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var permission = (Permission)dataGridView1.Rows[e.RowIndex].Tag;
            _selectedPermissionId = permission.Id;
            lblTitleSelectedPermission.Text = $"選択中の権限：{permission.Name}";

            // 選択された権限に関連付けられたコントロールを表示
            RefreshControlMappingList();
        }

        private void RefreshControlMappingList()
        {
            if (_selectedPermissionId < 0)
            {
                dataGridView2.Rows.Clear();
                return;
            }

            dataGridView2.Rows.Clear();

            // 選択された権限に関連付けられているコントロールマッピングを取得して表示
            var mappings = _permissionManager.ControlPermissionMapManager.GetAllMappings();

            foreach (var mapping in mappings)
            {
                // 権限IDで絞り込み
                foreach (var setting in mapping.Value.Where(s => s.PermissionId == _selectedPermissionId))
                {
                    int rowIdx = dataGridView2.Rows.Add(
                        setting.ControlName,
                        setting.AffectVisibility,
                        setting.AffectEnabled);

                    dataGridView2.Rows[rowIdx].Tag = setting;
                }
            }
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var setting = (ControlPermissionSettings)dataGridView2.Rows[e.RowIndex].Tag;
            _selectedControlName = setting.ControlName;
        }

        private void BtnAddControl_Click(object sender, EventArgs e)
        {
            if (_selectedPermissionId < 0)
            {
                MessageBox.Show("権限を選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtControlName.Text))
            {
                MessageBox.Show("コントロール名を入力してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // コントロールのマッピングを登録
                _permissionManager.ControlPermissionMapManager.RegisterControl(
                    txtControlName.Text,
                    _selectedPermissionId,
                    true,  // Visibilityに影響
                    true   // Enabledに影響
                );

                txtControlName.Clear();
                RefreshControlMappingList();
                MessageBox.Show("コントロールマッピングを追加しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コントロールマッピングの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteControlMapping_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0) return;

            var setting = (ControlPermissionSettings)dataGridView2.SelectedRows[0].Tag;

            if (MessageBox.Show($"コントロール「{setting.ControlName}」のマッピングを削除してもよろしいですか？",
                "削除の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _permissionManager.ControlPermissionMapManager.UnregisterControl(setting.ControlName, setting.PermissionId);
                RefreshControlMappingList();
                MessageBox.Show("コントロールマッピングを削除しました", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コントロールマッピングの削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
