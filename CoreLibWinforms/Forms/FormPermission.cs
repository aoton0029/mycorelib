using CoreLibWinforms.Core;
using CoreLibWinforms.Core.Permissions;
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
        PermissionService _permissionService;
        private Dictionary<string, DualListManager> _dualListManagers = new();
        private string _selectedRoleId = string.Empty;
        private string _selectedUserId = string.Empty;
        private DataTable _permissionsTable = new DataTable();
        private DataTable _rolesTable = new DataTable();

        public FormPermission()
        {
            InitializeComponent();
            _permissionService = new PermissionService();
            _permissionService.Load();

            InitializeTables();
            RefreshPermissionsGrid();
            RefreshRolesGrid();
            RefreshUsersGrid();
            SetupControlMappingTab();
        }

        private void InitializeTables()
        {
            // 権限テーブルの初期化
            _permissionsTable.Columns.Add("Id", typeof(string));
            _permissionsTable.Columns.Add("Name", typeof(string));

            // ロールテーブルの初期化
            _rolesTable.Columns.Add("Id", typeof(string));
            _rolesTable.Columns.Add("Name", typeof(string));
        }

        #region 権限タブ
        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPermissionName.Text))
                return;

            var permissionId = txtPermissionId.Text;
            _permissionService.AddNewPermission(permissionId, txtPermissionName.Text);
            RefreshPermissionsGrid();
            txtPermissionName.Clear();
            txtPermissionId.Clear();
        }

        private void RefreshPermissionsGrid()
        {
            gridPermission.DataSource = _permissionService.GetAllPermissions();
        }


        private void gridPermission_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 削除ボタン列がクリックされた場合
            if (gridPermission.Columns[e.ColumnIndex].Name == "ColumnPermDeleteButton")
            {
                if (MessageBox.Show("この権限を削除しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string permissionId = gridPermission.Rows[e.RowIndex].Cells["ColumnPermId"].Value.ToString();
                    _permissionService.RemovePermission(permissionId);
                    RefreshPermissionsGrid();
                    RefreshRoleDualLists(); // 関連するロール権限リストも更新
                }
            }
        }
        #endregion

        #region ロールタブ
        private void btnAddRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text) || string.IsNullOrWhiteSpace(txtRoleId.Text))
                return;

            var roleId = txtRoleId.Text;
            _permissionService.AddNewRole(roleId, txtRoleName.Text);
            RefreshRolesGrid();
            txtRoleName.Clear();
            txtRoleId.Clear();
        }
        private void RefreshRolesGrid()
        {
            gridRole.DataSource = _permissionService.GetAllRoles();
        }
        private void btnDeSelectRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedRoleId) || gridRoleSelected.SelectedRows.Count == 0)
                return;

            string permissionId = gridRoleSelected.SelectedRows[0].Cells["ColumnRoleSelectedId"].Value.ToString();
            var role = _permissionService.GetRole(_selectedRoleId);
            if (role != null)
            {
                role.Permissions.Remove(permissionId);
                RefreshRoleDualLists();
            }
        }

        private void btnSelectRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedRoleId) || gridRoleAvailable.SelectedRows.Count == 0)
                return;

            string permissionId = gridRoleAvailable.SelectedRows[0].Cells["ColumnRoleAvailableId"].Value.ToString();
            var role = _permissionService.GetRole(_selectedRoleId);
            if (role != null)
            {
                role.Permissions.Add(permissionId);
                RefreshRoleDualLists();
            }
        }

        private void gridRoleSelected_SelectionChanged(object sender, EventArgs e)
        {
            btnDeSelectRole.Enabled = gridRoleSelected.SelectedRows.Count > 0;
        }

        private void gridRoleAvailable_SelectionChanged(object sender, EventArgs e)
        {
            btnSelectRole.Enabled = gridRoleAvailable.SelectedRows.Count > 0;
        }

        private void gridRole_SelectionChanged(object sender, EventArgs e)
        {
            if (gridRole.SelectedRows.Count > 0)
            {
                _selectedRoleId = gridRole.SelectedRows[0].Cells["ColumnRoleId"].Value.ToString();
                var role = _permissionService.GetRole(_selectedRoleId);
                if (role != null)
                {
                    lblSelectedRole.Text = $"選択中のロール: {role.Name}";
                    InitializeRoleDualLists();
                }
            }
            else
            {
                _selectedRoleId = string.Empty;
                lblSelectedRole.Text = "選択中のロール: なし";
                ClearRoleDualLists();
            }
        }

        private void InitializeRoleDualLists()
        {
            if (string.IsNullOrEmpty(_selectedRoleId))
                return;

            var role = _permissionService.GetRole(_selectedRoleId);
            if (role == null)
                return;

            // 権限テーブルのクリアと再構築
            _permissionsTable.Clear();
            foreach (var permission in _permissionService.GetAllPermissions())
            {
                _permissionsTable.Rows.Add(permission.Id, permission.Name);
            }

            // DualListManagerを作成または取得して反映
            string roleKey = $"role_{_selectedRoleId}";
            if (!_dualListManagers.TryGetValue(roleKey, out var dualListManager))
            {
                dualListManager = new DualListManager(_permissionsTable, "Id", "Name", role.Permissions);
                _dualListManagers[roleKey] = dualListManager;
            }

            // データソースの設定
            gridRoleSelected.DataSource = dualListManager.SelectedView;
            gridRoleAvailable.DataSource = dualListManager.AvailableView;
        }

        private void ClearRoleDualLists()
        {
            gridRoleSelected.DataSource = null;
            gridRoleAvailable.DataSource = null;
        }

        private void RefreshRoleDualLists()
        {
            if (string.IsNullOrEmpty(_selectedRoleId))
                return;

            var role = _permissionService.GetRole(_selectedRoleId);
            if (role == null)
                return;

            string roleKey = $"role_{_selectedRoleId}";
            if (_dualListManagers.TryGetValue(roleKey, out var dualListManager))
            {
                // 選択状態を更新
                foreach (DataRow row in _permissionsTable.Rows)
                {
                    string permId = row["Id"].ToString();
                    if (role.Permissions.Contains(permId))
                    {
                        dualListManager.Select(permId);
                    }
                    else
                    {
                        dualListManager.Deselect(permId);
                    }
                }
            }
            else
            {
                InitializeRoleDualLists();
            }
        }

        private void gridRole_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 削除ボタン列がクリックされた場合
            if (gridRole.Columns[e.ColumnIndex].Name == "ColumnRoleDeleteButton")
            {
                if (MessageBox.Show("このロールを削除しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string roleId = gridRole.Rows[e.RowIndex].Cells["ColumnRoleId"].Value.ToString();
                    _permissionService.RemoveRole(roleId);
                    RefreshRolesGrid();

                    // 削除したロールが現在選択中だった場合は選択を解除
                    if (_selectedRoleId == roleId)
                    {
                        _selectedRoleId = string.Empty;
                        lblSelectedRole.Text = "選択中のロール: なし";
                        ClearRoleDualLists();
                    }

                    RefreshUserRoleDualLists(); // 関連するユーザーロールリストも更新
                }
            }
        }
        #endregion

        #region ユーザー権限タブ
        private void btnAddUserId_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
                return;

            var userId = txtUserId.Text;
            if (_permissionService.GetUserRole(userId) == null)
            {
                // 新しいユーザーロールを作成（空の状態で）
                _permissionService.GetUserRole(userId); // 存在しない場合は新規作成される
                RefreshUsersGrid();
                txtUserId.Clear();
            }
            else
            {
                MessageBox.Show("そのユーザーIDは既に存在します。", "重複エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshUsersGrid()
        {
            gridUser.DataSource = _permissionService.GetAllUserRoles();
        }

        private void gridUser_SelectionChanged(object sender, EventArgs e)
        {
            if (gridUser.SelectedRows.Count > 0)
            {
                _selectedUserId = gridUser.SelectedRows[0].Cells["ColumnUserId"].Value.ToString();
                var userRole = _permissionService.GetUserRole(_selectedUserId);
                if (userRole != null)
                {
                    lblSelectedUser.Text = $"選択中のユーザー: {userRole.UserId}";
                    InitializeUserDualLists();
                    RefreshUserAllPermissions();
                }
            }
            else
            {
                _selectedUserId = string.Empty;
                lblSelectedUser.Text = "選択中のユーザー: なし";
                ClearUserDualLists();
                gridUserAllPermissions.DataSource = null;
            }
        }

        private void RefreshUserAllPermissions()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
                return;

            // ユーザーの全権限（ロールからの権限 + 追加権限）を表示
            var permissionList = new List<Permission>();

            // すべての権限を取得
            foreach (var permission in _permissionService.GetAllPermissions())
            {
                if (_permissionService.HasPermission(_selectedUserId, permission.Id))
                {
                    permissionList.Add(permission);
                }
            }

            // DataTableに変換して表示
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            foreach (var permission in permissionList)
            {
                dt.Rows.Add(permission.Id, permission.Name);
            }

            gridUserAllPermissions.DataSource = dt;
        }

        private void InitializeUserDualLists()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
                return;

            var userRole = _permissionService.GetUserRole(_selectedUserId);
            if (userRole == null)
                return;

            // ロールデュアルリスト初期化
            _rolesTable.Clear();
            foreach (var role in _permissionService.GetAllRoles())
            {
                _rolesTable.Rows.Add(role.Id, role.Name);
            }

            // ロールのDualListManagerを作成または取得して反映
            string userRolesKey = $"user_roles_{_selectedUserId}";
            if (!_dualListManagers.TryGetValue(userRolesKey, out var rolesDualListManager))
            {
                rolesDualListManager = new DualListManager(_rolesTable, "Id", "Name", userRole.Roles);
                _dualListManagers[userRolesKey] = rolesDualListManager;
            }

            // 追加権限用デュアルリスト初期化
            string userPermsKey = $"user_perms_{_selectedUserId}";
            if (!_dualListManagers.TryGetValue(userPermsKey, out var permsDualListManager))
            {
                permsDualListManager = new DualListManager(_permissionsTable, "Id", "Name", userRole.AdditionalPermissions);
                _dualListManagers[userPermsKey] = permsDualListManager;
            }

            // データソースの設定
            gridUserRoleSelected.DataSource = rolesDualListManager.SelectedView;
            gridUserRoleAvailable.DataSource = rolesDualListManager.AvailableView;

            gridUserAdditionalPermissionsSelected.DataSource = permsDualListManager.SelectedView;
            gridUserAdditionalPermissionsAvailable.DataSource = permsDualListManager.AvailableView;
        }

        private void ClearUserDualLists()
        {
            gridUserRoleSelected.DataSource = null;
            gridUserRoleAvailable.DataSource = null;
            gridUserAdditionalPermissionsSelected.DataSource = null;
            gridUserAdditionalPermissionsAvailable.DataSource = null;
        }

        private void RefreshUserRoleDualLists()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
                return;

            var userRole = _permissionService.GetUserRole(_selectedUserId);
            if (userRole == null)
                return;

            string userRolesKey = $"user_roles_{_selectedUserId}";
            if (_dualListManagers.TryGetValue(userRolesKey, out var dualListManager))
            {
                // 選択状態を更新
                foreach (DataRow row in _rolesTable.Rows)
                {
                    string roleId = row["Id"].ToString();
                    if (userRole.Roles.Contains(roleId))
                    {
                        dualListManager.Select(roleId);
                    }
                    else
                    {
                        dualListManager.Deselect(roleId);
                    }
                }
            }
            else
            {
                InitializeUserDualLists();
            }

            RefreshUserAllPermissions();
        }

        private void RefreshUserPermDualLists()
        {
            if (string.IsNullOrEmpty(_selectedUserId))
                return;

            var userRole = _permissionService.GetUserRole(_selectedUserId);
            if (userRole == null)
                return;

            string userPermsKey = $"user_perms_{_selectedUserId}";
            if (_dualListManagers.TryGetValue(userPermsKey, out var dualListManager))
            {
                // 選択状態を更新
                foreach (DataRow row in _permissionsTable.Rows)
                {
                    string permId = row["Id"].ToString();
                    if (userRole.AdditionalPermissions.Contains(permId))
                    {
                        dualListManager.Select(permId);
                    }
                    else
                    {
                        dualListManager.Deselect(permId);
                    }
                }
            }
            else
            {
                InitializeUserDualLists();
            }

            RefreshUserAllPermissions();
        }

        private void gridUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 削除ボタン列がクリックされた場合
            if (gridUser.Columns[e.ColumnIndex].Name == "ColumnUserDeleteButton")
            {
                if (MessageBox.Show("このユーザーを削除しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string userId = gridUser.Rows[e.RowIndex].Cells["ColumnUserId"].Value.ToString();
                    var userRoles = _permissionService.GetAllUserRoles();
                    var userToRemove = userRoles.FirstOrDefault(ur => ur.UserId == userId);
                    if (userToRemove != null)
                    {
                        userRoles.Remove(userToRemove);
                        RefreshUsersGrid();

                        // 削除したユーザーが現在選択中だった場合は選択を解除
                        if (_selectedUserId == userId)
                        {
                            _selectedUserId = string.Empty;
                            lblSelectedUser.Text = "選択中のユーザー: なし";
                            ClearUserDualLists();
                            gridUserAllPermissions.DataSource = null;
                        }
                    }
                }
            }
        }

        private void gridUserRoleSelected_SelectionChanged(object sender, EventArgs e)
        {
            btnDeSelectUserRole.Enabled = gridUserRoleSelected.SelectedRows.Count > 0;
        }

        private void btnDeSelectUserRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || gridUserRoleSelected.SelectedRows.Count == 0)
                return;

            string roleId = gridUserRoleSelected.SelectedRows[0].Cells["ColumnUserRoleSelectedId"].Value.ToString();
            _permissionService.RemoveRoleFromUser(_selectedUserId, roleId);
            RefreshUserRoleDualLists();
        }

        private void btnSelectUserRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || gridUserRoleAvailable.SelectedRows.Count == 0)
                return;

            string roleId = gridUserRoleAvailable.SelectedRows[0].Cells["ColumnUserRoleAvailableId"].Value.ToString();
            _permissionService.AssignRoleToUser(_selectedUserId, roleId);
            RefreshUserRoleDualLists();
        }

        private void gridUserRoleAvailable_SelectionChanged(object sender, EventArgs e)
        {
            btnSelectUserRole.Enabled = gridUserRoleAvailable.SelectedRows.Count > 0;
        }

        private void gridUserAdditionalPermissionsSelected_SelectionChanged(object sender, EventArgs e)
        {
            btnDeSelectUserAdditionalPermission.Enabled = gridUserAdditionalPermissionsSelected.SelectedRows.Count > 0;
        }

        private void btnDeSelectUserAdditionalPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || gridUserAdditionalPermissionsSelected.SelectedRows.Count == 0)
                return;

            string permissionId = gridUserAdditionalPermissionsSelected.SelectedRows[0].Cells["ColumnUserAdditionalPermissionSelectedId"].Value.ToString();
            _permissionService.RevokeAdditionalPermissionFromUser(_selectedUserId, permissionId);
            RefreshUserPermDualLists();
        }

        private void btnSelectUserAdditionalPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId) || gridUserAdditionalPermissionsAvailable.SelectedRows.Count == 0)
                return;

            string permissionId = gridUserAdditionalPermissionsAvailable.SelectedRows[0].Cells["ColumnUserAdditionalPermissionAvailableId"].Value.ToString();
            _permissionService.GrantAdditionalPermissionToUser(_selectedUserId, permissionId);
            RefreshUserPermDualLists();
        }

        private void gridUserAdditionalPermissionsAvailable_SelectionChanged(object sender, EventArgs e)
        {
            btnSelectUserAdditionalPermission.Enabled = gridUserAdditionalPermissionsAvailable.SelectedRows.Count > 0;
        }
        #endregion

        #region コントロール権限マッピングタブ
        private void SetupControlMappingTab()
        {
            // 権限コンボボックスの設定
            cmbPermission.DisplayMember = "Name";
            cmbPermission.ValueMember = "Id";
            cmbPermission.DataSource = _permissionService.GetAllPermissions();

            // 制限タイプコンボボックスの設定
            cmbRestrictType.DataSource = Enum.GetValues(typeof(RestrictType))
                .Cast<RestrictType>()
                .Select(r => new { Value = r, Name = GetRestrictTypeName(r) })
                .ToList();
            cmbRestrictType.DisplayMember = "Name";
            cmbRestrictType.ValueMember = "Value";

            // フォーム名に現在のフォーム名をデフォルト設定
            txtFormName.Text = this.Name;

            // マッピングデータ読み込み
            RefreshControlMappingGrid();
        }

        private string GetRestrictTypeName(RestrictType restrictType)
        {
            return restrictType switch
            {
                RestrictType.Visibility => "表示/非表示",
                RestrictType.Enabled => "有効/無効",
                RestrictType.ReadOnly => "読み取り専用/編集可能",
                _ => restrictType.ToString()
            };
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            string formName = txtFormName.Text.Trim();
            string controlName = txtControlName.Text.Trim();

            if (string.IsNullOrEmpty(formName) || string.IsNullOrEmpty(controlName))
            {
                MessageBox.Show("フォーム名とコントロール名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbPermission.SelectedItem == null)
            {
                MessageBox.Show("権限を選択してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRestrictType.SelectedItem == null)
            {
                MessageBox.Show("制限タイプを選択してください。", "選択エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 選択値の取得
            var permission = (Permission)cmbPermission.SelectedItem;
            var restrictType = (RestrictType)cmbRestrictType.SelectedValue;

            // マッピング追加
            _permissionService.AddControlMapping(formName, controlName, permission.Id, restrictType);

            // グリッド更新
            RefreshControlMappingGrid();

            // 入力フィールドのクリア
            txtControlName.Clear();
        }

        private void RefreshControlMappingGrid()
        {
            string formName = txtFormName.Text.Trim();
            if (string.IsNullOrEmpty(formName))
            {
                gridControlMapping.Rows.Clear();
                return;
            }

            var mappings = _permissionService.GetControlMappings(formName);
            gridControlMapping.Rows.Clear();

            foreach (var mapping in mappings)
            {
                // 権限名を取得
                string permissionName = "不明";
                var permission = _permissionService.GetPermission(mapping.PermissionId);
                if (permission != null)
                {
                    permissionName = permission.Name;
                }

                // 制限タイプ名を取得
                string restrictTypeName = GetRestrictTypeName(mapping.RestrictType);

                // 行の追加
                int rowIndex = gridControlMapping.Rows.Add(mapping.ControlName, permissionName, restrictTypeName);

                // 非表示データとして保存
                gridControlMapping.Rows[rowIndex].Tag = mapping;
            }
        }

        private void gridControlMapping_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 削除ボタン列がクリックされた場合
            if (gridControlMapping.Columns[e.ColumnIndex].Name == "columnDelete")
            {
                if (MessageBox.Show("このマッピングを削除しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var mapping = gridControlMapping.Rows[e.RowIndex].Tag as ControlPermissionMapping;
                    if (mapping != null)
                    {
                        _permissionService.RemoveControlMapping(mapping.FormName, mapping.ControlName, mapping.RestrictType);
                        RefreshControlMappingGrid();
                    }
                }
            }
        }

        private void txtFormName_TextChanged(object sender, EventArgs e)
        {
            RefreshControlMappingGrid();
        }
        #endregion


        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("変更を破棄しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _permissionService.Load();
                RefreshPermissionsGrid();
                RefreshRolesGrid();
                RefreshUsersGrid();
                RefreshControlMappingGrid();
                ClearRoleDualLists();
                ClearUserDualLists();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _permissionService.Save();
                MessageBox.Show("保存しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPermissionCategoryUnique_Click(object sender, EventArgs e)
        {
            txtPermissionId.Text += Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private void btnPermissionCategoryDelete_Click(object sender, EventArgs e)
        {
            txtPermissionId.Text += "delete.";
        }

        private void btnPermissionCategoryEdit_Click(object sender, EventArgs e)
        {
            txtPermissionId.Text += "edit.";
        }

        private void btnPermissionCategoryCreate_Click(object sender, EventArgs e)
        {
            txtPermissionId.Text += "create.";
        }

        private void btnPermissionCategoryView_Click(object sender, EventArgs e)
        {
            txtPermissionId.Text += "view.";
        }

        private void btnRoleAdmin_Click(object sender, EventArgs e)
        {
            txtRoleId.Text += "admin.";
        }

        private void btnRoleManager_Click(object sender, EventArgs e)
        {
            txtRoleId.Text += "manager.";
        }

        private void btnRoleUser_Click(object sender, EventArgs e)
        {
            txtRoleId.Text += "user.";
        }

        private void btnRoleUnique_Click(object sender, EventArgs e)
        {
            txtRoleId.Text += Guid.NewGuid().ToString("N").Substring(0, 8);
        }

    }
}
