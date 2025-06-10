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
        PermissionManager _permissionMng;
        DualListManager _dualListRolePermission;
        DualListManager _dualListUserRole;
        DualListManager _dualListUserAdditionalPermission;
        DualListManager _dualListDeptPermission;
        int _selectedRoleId = -1;
        string _selectedUserId = string.Empty;
        int _selectedDeptId = -1;
        (string, string) _selectedControl = (null, null);

        public FormPermission()
        {
            InitializeComponent();
            _permissionMng = new PermissionManager();
            _permissionMng.Load();
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            refreshAll();
        }

        private void refreshAll()
        {
            // 基本権限タブの更新
            var permissions = _permissionMng.GetAllPermissions();
            var permissionsTable = new DataTable();
            permissionsTable.Columns.Add("Id", typeof(int));
            permissionsTable.Columns.Add("Name", typeof(string));

            foreach (var permission in permissions)
            {
                permissionsTable.Rows.Add(permission.Id, permission.Name);
            }

            gridPermission.DataSource = permissionsTable;

            // ロール権限タブの更新
            var roles = _permissionMng.GetAllRoles();
            var rolesTable = new DataTable();
            rolesTable.Columns.Add("Id", typeof(int));
            rolesTable.Columns.Add("Name", typeof(string));

            foreach (var role in roles)
            {
                rolesTable.Rows.Add(role.Id, role.Name);
            }

            gridRole.DataSource = rolesTable;

            // ユーザー権限タブの更新
            var userRoles = _permissionMng.GetAllUserRoles();
            var userRolesTable = new DataTable();
            userRolesTable.Columns.Add("UserId", typeof(string));

            foreach (var userRole in userRoles)
            {
                userRolesTable.Rows.Add(userRole.UserId);
            }

            gridUser.DataSource = userRolesTable;

            // 部署権限タブの更新
            var departments = _permissionMng.GetAllDepartmentPermissions();
            var departmentsTable = new DataTable();
            departmentsTable.Columns.Add("Id", typeof(int));
            departmentsTable.Columns.Add("Name", typeof(string));

            foreach (var dept in departments)
            {
                departmentsTable.Rows.Add(dept.Id, dept.Name);
            }

            gridDept.DataSource = departmentsTable;

            // コントロール権限タブの更新
            var controlPermissions = _permissionMng.GetAllControlPermissions();
            var uniqueControls = controlPermissions
                .GroupBy(cp => new { cp.FormName, cp.ControlName })
                .Select(g => g.First())
                .ToList();

            var controlsTable = new DataTable();
            controlsTable.Columns.Add("FormName", typeof(string));
            controlsTable.Columns.Add("ControlName", typeof(string));

            foreach (var control in uniqueControls)
            {
                controlsTable.Rows.Add(control.FormName, control.ControlName);
            }

            gridControl.DataSource = controlsTable;
        }

        #region 権限タブ
        private void gridPermission_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 削除ボタン列がクリックされた場合
            if (e.ColumnIndex == gridPermission.Columns["ColumnPermDeleteButton"].Index && e.RowIndex >= 0)
            {
                int permissionId = Convert.ToInt32(gridPermission.Rows[e.RowIndex].Cells["ColumnPermId"].Value);

                if (MessageBox.Show($"権限ID: {permissionId} を削除しますか？", "権限の削除",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_permissionMng.RemovePermission(permissionId))
                    {
                        refreshAll();
                    }
                    else
                    {
                        MessageBox.Show("権限の削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            string permissionName = txtPermissionName.Text.Trim();

            if (string.IsNullOrEmpty(permissionName))
            {
                MessageBox.Show("権限名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _permissionMng.AddNewPermission(permissionName);
                txtPermissionName.Clear();
                refreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region ロールタブ
        private void btnAddRole_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show("ロール名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _permissionMng.AddNewRole(roleName);
                txtRoleName.Clear();
                refreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの追加に失敗しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridRole_SelectionChanged(object sender, EventArgs e)
        {
            if (gridRole.SelectedRows.Count > 0)
            {
                int roleId = Convert.ToInt32(gridRole.SelectedRows[0].Cells["ColumnRoleId"].Value);
                string roleName = gridRole.SelectedRows[0].Cells["ColumnRoleName"].Value.ToString();
                _selectedRoleId = roleId;
                lblSelectedRole.Text = $"選択中のロール: {roleName}";

                // 選択されたロールの権限を表示
                var role = _permissionMng.GetRole(roleId);
                if (role != null)
                {
                    // ロールに割り当てられた権限を取得
                    var selectedPermissions = new DataTable();
                    selectedPermissions.Columns.Add("Id", typeof(int));
                    selectedPermissions.Columns.Add("Name", typeof(string));

                    foreach (var permId in role.Permissions)
                    {
                        var perm = _permissionMng.GetPermission(permId);
                        if (perm != null)
                        {
                            selectedPermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridRoleSelected.DataSource = selectedPermissions;

                    // 利用可能な権限を表示
                    var allPermissions = _permissionMng.GetAllPermissions();
                    var availablePermissions = new DataTable();
                    availablePermissions.Columns.Add("Id", typeof(int));
                    availablePermissions.Columns.Add("Name", typeof(string));

                    foreach (var perm in allPermissions)
                    {
                        if (!role.Permissions.Contains(perm.Id))
                        {
                            availablePermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridRoleAvailable.DataSource = availablePermissions;
                }
            }
            else
            {
                _selectedRoleId = -1;
                lblSelectedRole.Text = "選択中のロール: なし";
                gridRoleSelected.DataSource = null;
                gridRoleAvailable.DataSource = null;
            }
        }

        private void btnDeSelectRole_Click(object sender, EventArgs e)
        {
            if (_selectedRoleId < 0)
            {
                MessageBox.Show("ロールを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridRoleSelected.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridRoleSelected.SelectedRows[0].Cells["ColumnRoleSelectedId"].Value);

                if (_permissionMng.RemovePermissionFromRole(_selectedRoleId, permId))
                {
                    gridRole_SelectionChanged(gridRole, EventArgs.Empty); // 表示を更新
                }
            }
        }

        private void btnSelectRole_Click(object sender, EventArgs e)
        {
            if (_selectedRoleId < 0)
            {
                MessageBox.Show("ロールを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridRoleAvailable.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridRoleAvailable.SelectedRows[0].Cells["ColumnRoleAvailableId"].Value);

                if (_permissionMng.AddPermissionToRole(_selectedRoleId, permId))
                {
                    gridRole_SelectionChanged(gridRole, EventArgs.Empty); // 表示を更新
                }
            }
        }
        #endregion

        #region ユーザーロールタブ
        private void gridUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 削除ボタン列がクリックされた場合
            if (e.ColumnIndex == gridUser.Columns["ColumnUserDeleteButton"].Index && e.RowIndex >= 0)
            {
                string userId = gridUser.Rows[e.RowIndex].Cells["ColumnUserId"].Value.ToString();

                if (MessageBox.Show($"ユーザーID: {userId} を削除しますか？", "ユーザーの削除",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_permissionMng.RemoveUserRole(userId))
                    {
                        refreshAll();
                    }
                    else
                    {
                        MessageBox.Show("ユーザーの削除に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void gridUser_SelectionChanged(object sender, EventArgs e)
        {
            if (gridUser.SelectedRows.Count > 0)
            {
                string userId = gridUser.SelectedRows[0].Cells["ColumnUserId"].Value.ToString();
                _selectedUserId = userId;
                lblSelectedUser.Text = $"選択中のユーザー: {userId}";

                // 選択されたユーザーのロールと追加権限を表示
                var userRole = _permissionMng.GetUserRole(userId);
                if (userRole != null)
                {
                    // ユーザーに割り当てられたロールを表示
                    var selectedRoles = new DataTable();
                    selectedRoles.Columns.Add("Id", typeof(int));
                    selectedRoles.Columns.Add("Name", typeof(string));

                    foreach (var roleId in userRole.Roles)
                    {
                        var role = _permissionMng.GetRole(roleId);
                        if (role != null)
                        {
                            selectedRoles.Rows.Add(role.Id, role.Name);
                        }
                    }

                    gridUserRoleSelected.DataSource = selectedRoles;

                    // 利用可能なロールを表示
                    var allRoles = _permissionMng.GetAllRoles();
                    var availableRoles = new DataTable();
                    availableRoles.Columns.Add("Id", typeof(int));
                    availableRoles.Columns.Add("Name", typeof(string));

                    foreach (var role in allRoles)
                    {
                        if (!userRole.Roles.Contains(role.Id))
                        {
                            availableRoles.Rows.Add(role.Id, role.Name);
                        }
                    }

                    gridUserRoleAvailable.DataSource = availableRoles;

                    // ユーザーの追加権限を表示
                    var selectedAddPermissions = new DataTable();
                    selectedAddPermissions.Columns.Add("Id", typeof(int));
                    selectedAddPermissions.Columns.Add("Name", typeof(string));

                    foreach (var permId in userRole.AdditionalPermissions)
                    {
                        var perm = _permissionMng.GetPermission(permId);
                        if (perm != null)
                        {
                            selectedAddPermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridUserAdditionalPermissionsSelected.DataSource = selectedAddPermissions;

                    // 利用可能な追加権限を表示
                    var allPermissions = _permissionMng.GetAllPermissions();
                    var availableAddPermissions = new DataTable();
                    availableAddPermissions.Columns.Add("Id", typeof(int));
                    availableAddPermissions.Columns.Add("Name", typeof(string));

                    foreach (var perm in allPermissions)
                    {
                        if (!userRole.AdditionalPermissions.Contains(perm.Id))
                        {
                            availableAddPermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridUserAdditionalPermissionsAvailable.DataSource = availableAddPermissions;

                    // ユーザーの全権限を表示（ロール経由 + 追加権限）
                    var permissionService = new PermissionService(_permissionMng);
                    var allUserPermIds = permissionService.GetUserPermissions(userId);
                    var allUserPermissions = new DataTable();
                    allUserPermissions.Columns.Add("Id", typeof(int));
                    allUserPermissions.Columns.Add("Name", typeof(string));

                    foreach (var permId in allUserPermIds)
                    {
                        var perm = _permissionMng.GetPermission(permId);
                        if (perm != null)
                        {
                            allUserPermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridUserAllPermissions.DataSource = allUserPermissions;
                }
            }
            else
            {
                _selectedUserId = string.Empty;
                lblSelectedUser.Text = "選択中のユーザー: なし";
                gridUserRoleSelected.DataSource = null;
                gridUserRoleAvailable.DataSource = null;
                gridUserAdditionalPermissionsSelected.DataSource = null;
                gridUserAdditionalPermissionsAvailable.DataSource = null;
                gridUserAllPermissions.DataSource = null;
            }
        }

        private void btnDeSelectUserRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridUserRoleSelected.SelectedRows.Count > 0)
            {
                int roleId = Convert.ToInt32(gridUserRoleSelected.SelectedRows[0].Cells["ColumnUserRoleSelectedId"].Value);

                if (_permissionMng.RemoveRoleFromUser(_selectedUserId, roleId))
                {
                    gridUser_SelectionChanged(gridUser, EventArgs.Empty); // 表示を更新
                }
            }
        }

        private void btnSelectUserRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridUserRoleAvailable.SelectedRows.Count > 0)
            {
                int roleId = Convert.ToInt32(gridUserRoleAvailable.SelectedRows[0].Cells["ColumnUserRoleAvailableId"].Value);

                if (_permissionMng.AddRoleToUser(_selectedUserId, roleId))
                {
                    gridUser_SelectionChanged(gridUser, EventArgs.Empty); // 表示を更新
                }
            }
        }

        private void btnDeSelectUserAdditionalPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridUserAdditionalPermissionsSelected.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridUserAdditionalPermissionsSelected.SelectedRows[0].Cells["ColumnUserAdditionalPermissionSelectedId"].Value);

                if (_permissionMng.RemoveAdditionalPermissionFromUser(_selectedUserId, permId))
                {
                    gridUser_SelectionChanged(gridUser, EventArgs.Empty); // 表示を更新
                }
            }
        }

        private void btnSelectUserAdditionalPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedUserId))
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridUserAdditionalPermissionsAvailable.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridUserAdditionalPermissionsAvailable.SelectedRows[0].Cells["ColumnUserAdditionalPermissionAvailableId"].Value);

                if (_permissionMng.AddAdditionalPermissionToUser(_selectedUserId, permId))
                {
                    gridUser_SelectionChanged(gridUser, EventArgs.Empty); // 表示を更新
                }
            }
        }
        #endregion

        #region 部署権限タブ
        private void gridDept_SelectionChanged(object sender, EventArgs e)
        {
            if (gridDept.SelectedRows.Count > 0)
            {
                int deptId = Convert.ToInt32(gridDept.SelectedRows[0].Cells["dataGridViewTextBoxColumn1"].Value);
                string deptName = gridDept.SelectedRows[0].Cells["dataGridViewTextBoxColumn2"].Value.ToString();
                _selectedDeptId = deptId;
                lblTitleSelectedDept.Text = $"選択中の部署: {deptName}";

                // 選択された部署の権限を表示
                var dept = _permissionMng.GetDepartmentPermission(deptId);
                if (dept != null)
                {
                    // 部署に割り当てられた権限を取得
                    var selectedPermissions = new DataTable();
                    selectedPermissions.Columns.Add("Id", typeof(int));
                    selectedPermissions.Columns.Add("Name", typeof(string));

                    foreach (var permId in dept.Permissions)
                    {
                        var perm = _permissionMng.GetPermission(permId);
                        if (perm != null)
                        {
                            selectedPermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridDeptPermSelected.DataSource = selectedPermissions;

                    // 利用可能な権限を表示
                    var allPermissions = _permissionMng.GetAllPermissions();
                    var availablePermissions = new DataTable();
                    availablePermissions.Columns.Add("Id", typeof(int));
                    availablePermissions.Columns.Add("Name", typeof(string));

                    foreach (var perm in allPermissions)
                    {
                        if (!dept.Permissions.Contains(perm.Id))
                        {
                            availablePermissions.Rows.Add(perm.Id, perm.Name);
                        }
                    }

                    gridDeptPermAvailable.DataSource = availablePermissions;
                }
            }
            else
            {
                _selectedDeptId = -1;
                lblTitleSelectedDept.Text = "選択中の部署: なし";
                gridDeptPermSelected.DataSource = null;
                gridDeptPermAvailable.DataSource = null;
            }
        }

        private void btnDeSelectDeptPerm_Click(object sender, EventArgs e)
        {
            if (_selectedDeptId < 0)
            {
                MessageBox.Show("部署を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridDeptPermSelected.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridDeptPermSelected.SelectedRows[0].Cells["ColumnDeptPermIdSelected"].Value);

                if (_permissionMng.RemovePermissionFromDepartment(_selectedDeptId, permId))
                {
                    gridDept_SelectionChanged(gridDept, EventArgs.Empty); // 表示を更新
                }
            }
        }

        private void btnSelectDeptPerm_Click(object sender, EventArgs e)
        {
            if (_selectedDeptId < 0)
            {
                MessageBox.Show("部署を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gridDeptPermAvailable.SelectedRows.Count > 0)
            {
                int permId = Convert.ToInt32(gridDeptPermAvailable.SelectedRows[0].Cells["ColumnDeptPermIdAvailable"].Value);

                if (_permissionMng.AddPermissionToDepartment(_selectedDeptId, permId))
                {
                    gridDept_SelectionChanged(gridDept, EventArgs.Empty); // 表示を更新
                }
            }
        }
        #endregion

        #region コントロール権限タブ
        private void btnAddControl_Click(object sender, EventArgs e)
        {
            string formName = txtControlFormName.Text.Trim();
            string controlName = txtControlControlName.Text.Trim();

            if (string.IsNullOrEmpty(formName) || string.IsNullOrEmpty(controlName))
            {
                MessageBox.Show("フォーム名とコントロール名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // コントロールの追加は、実際の権限設定は後で行うので、ダミーの値を使用
            try
            {
                // 既にコントロールが存在するか確認
                var existingControls = _permissionMng.GetControlPermissions(formName, controlName);
                if (existingControls.Count > 0)
                {
                    MessageBox.Show("このコントロールは既に登録されています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ダミー追加（実際には表示されない）これは後で実際のロールや部署が設定された時に使われる
                _permissionMng.AddControlPermission(formName, controlName, "Dummy", -1);

                txtControlFormName.Clear();
                txtControlControlName.Clear();
                refreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コントロールの追加に失敗しました。\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridControl_SelectionChanged(object sender, EventArgs e)
        {
            if (gridControl.SelectedRows.Count > 0)
            {
                string formName = gridControl.SelectedRows[0].Cells["ColumnControlFormName"].Value.ToString();
                string controlName = gridControl.SelectedRows[0].Cells["ColumnControlControlName"].Value.ToString();
                _selectedControl = (formName, controlName);
                lblTitleSelectedControl.Text = $"選択中のコントロール: {formName}.{controlName}";

                // ロールタブの権限設定を表示
                var roles = _permissionMng.GetAllRoles();
                var rolePermissions = new DataTable();
                rolePermissions.Columns.Add("Id", typeof(int));
                rolePermissions.Columns.Add("Name", typeof(string));
                rolePermissions.Columns.Add("Visible", typeof(bool));
                rolePermissions.Columns.Add("Enabled", typeof(bool));
                rolePermissions.Columns.Add("ReadOnly", typeof(bool));

                // コントロールの権限を取得
                var controlPerms = _permissionMng.GetControlPermissions(formName, controlName);

                foreach (var role in roles)
                {
                    // このロールに対する権限設定を検索
                    var rolePerm = controlPerms.FirstOrDefault(cp =>
                        cp.GroupCategory == ControlPermission.GroupCategory_Role && cp.Id == role.Id);

                    if (rolePerm != null)
                    {
                        rolePermissions.Rows.Add(role.Id, role.Name, rolePerm.ControlVisible,
                            rolePerm.ControlEnabled, rolePerm.ControlReadOnly);
                    }
                    else
                    {
                        // デフォルト値
                        rolePermissions.Rows.Add(role.Id, role.Name, true, true, false);
                    }
                }

                gridControlRole.DataSource = rolePermissions;

                // 部署タブの権限設定を表示
                var depts = _permissionMng.GetAllDepartmentPermissions();
                var deptPermissions = new DataTable();
                deptPermissions.Columns.Add("Id", typeof(int));
                deptPermissions.Columns.Add("Name", typeof(string));
                deptPermissions.Columns.Add("Visible", typeof(bool));
                deptPermissions.Columns.Add("Enabled", typeof(bool));
                deptPermissions.Columns.Add("ReadOnly", typeof(bool));

                foreach (var dept in depts)
                {
                    // この部署に対する権限設定を検索
                    var deptPerm = controlPerms.FirstOrDefault(cp =>
                        cp.GroupCategory == ControlPermission.GroupCategory_Dept && cp.Id == dept.Id);

                    if (deptPerm != null)
                    {
                        deptPermissions.Rows.Add(dept.Id, dept.Name, deptPerm.ControlVisible,
                            deptPerm.ControlEnabled, deptPerm.ControlReadOnly);
                    }
                    else
                    {
                        // デフォルト値
                        deptPermissions.Rows.Add(dept.Id, dept.Name, true, true, false);
                    }
                }

                gridControlDept.DataSource = deptPermissions;
            }
            else
            {
                _selectedControl = (null, null);
                lblTitleSelectedControl.Text = "選択中のコントロール: なし";
                gridControlRole.DataSource = null;
                gridControlDept.DataSource = null;
            }
        }

        private void gridControlRole_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _selectedControl.Item1 == null)
                return;

            // チェックボックスの列のみ処理
            if (e.ColumnIndex == gridControlRole.Columns["ColumnControlRoleVisible"].Index ||
                e.ColumnIndex == gridControlRole.Columns["ColumnControlRoleEnabled"].Index ||
                e.ColumnIndex == gridControlRole.Columns["ColumnControlRoleReadOnly"].Index)
            {
                int roleId = Convert.ToInt32(gridControlRole.Rows[e.RowIndex].Cells["ColumnControlRoleId"].Value);
                bool visible = Convert.ToBoolean(gridControlRole.Rows[e.RowIndex].Cells["ColumnControlRoleVisible"].Value);
                bool enabled = Convert.ToBoolean(gridControlRole.Rows[e.RowIndex].Cells["ColumnControlRoleEnabled"].Value);
                bool readOnly = Convert.ToBoolean(gridControlRole.Rows[e.RowIndex].Cells["ColumnControlRoleReadOnly"].Value);

                // コントロールの権限を取得
                var controlPerms = _permissionMng.GetControlPermissions(_selectedControl.Item1, _selectedControl.Item2);
                var rolePerm = controlPerms.FirstOrDefault(cp =>
                    cp.GroupCategory == ControlPermission.GroupCategory_Role && cp.Id == roleId);

                if (rolePerm != null)
                {
                    // 既存の権限設定を更新
                    _permissionMng.RemoveControlPermission(_selectedControl.Item1, _selectedControl.Item2,
                        ControlPermission.GroupCategory_Role, roleId);
                }

                // 新しい権限設定を追加
                _permissionMng.AddControlPermission(_selectedControl.Item1, _selectedControl.Item2,
                    ControlPermission.GroupCategory_Role, roleId, visible, enabled, readOnly);
            }
        }

        private void gridControlDept_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _selectedControl.Item1 == null)
                return;

            // チェックボックスの列のみ処理
            if (e.ColumnIndex == gridControlDept.Columns["ColumnControlDeptVisible"].Index ||
                e.ColumnIndex == gridControlDept.Columns["ColumnControlDeptEnabled"].Index ||
                e.ColumnIndex == gridControlDept.Columns["ColumnControlDeptReadOnly"].Index)
            {
                int deptId = Convert.ToInt32(gridControlDept.Rows[e.RowIndex].Cells["ColumnControlDeptId"].Value);
                bool visible = Convert.ToBoolean(gridControlDept.Rows[e.RowIndex].Cells["ColumnControlDeptVisible"].Value);
                bool enabled = Convert.ToBoolean(gridControlDept.Rows[e.RowIndex].Cells["ColumnControlDeptEnabled"].Value);
                bool readOnly = Convert.ToBoolean(gridControlDept.Rows[e.RowIndex].Cells["ColumnControlDeptReadOnly"].Value);

                // コントロールの権限を取得
                var controlPerms = _permissionMng.GetControlPermissions(_selectedControl.Item1, _selectedControl.Item2);
                var deptPerm = controlPerms.FirstOrDefault(cp =>
                    cp.GroupCategory == ControlPermission.GroupCategory_Dept && cp.Id == deptId);

                if (deptPerm != null)
                {
                    // 既存の権限設定を更新
                    _permissionMng.RemoveControlPermission(_selectedControl.Item1, _selectedControl.Item2,
                        ControlPermission.GroupCategory_Dept, deptId);
                }

                // 新しい権限設定を追加
                _permissionMng.AddControlPermission(_selectedControl.Item1, _selectedControl.Item2,
                    ControlPermission.GroupCategory_Dept, deptId, visible, enabled, readOnly);
            }
        }
        #endregion

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _permissionMng.Save();
                MessageBox.Show("権限情報を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限情報の保存に失敗しました。\n{ex.Message}", "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
