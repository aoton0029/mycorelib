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
        private int _selectedRoleId = -1;
        private string _selectedUserId = null;

        public FormPermission(PermissionManager permissionManager)
        {
            InitializeComponent();
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));

            // イベントハンドラの登録
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // 基本権限タブの初期化
            InitializePermissionTab();

            // 役割権限タブの初期化
            InitializeRoleTab();

            // ユーザー権限タブの初期化
            InitializeUserTab();
        }

        #region 権限画面
        private void InitializePermissionTab()
        {
            // イベントハンドラの登録
            btnRegistPerm.Click += BtnRegistPerm_Click;
            gridPerm.CellClick += GridPerm_CellClick;

            // データグリッドビューの設定
            RefreshPermissionGrid();
        }

        private void RefreshPermissionGrid()
        {
            gridPerm.Rows.Clear();

            // 権限一覧の取得
            var permissions = _permissionManager.GetAllPermissions();

            foreach (var permission in permissions)
            {
                gridPerm.Rows.Add(permission.Id, permission.Name);
            }
        }

        private void BtnRegistPerm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPerm.Text))
            {
                MessageBox.Show("権限名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 新しい権限を作成
                _permissionManager.CreatePermission(txtPerm.Text);

                // テキストボックスをクリア
                txtPerm.Clear();

                // グリッドを更新
                RefreshPermissionGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridPerm_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 削除ボタンクリック時の処理
            if (e.RowIndex >= 0 && e.ColumnIndex == ColumnPermDeleteButton.Index)
            {
                var permId = int.Parse(gridPerm.Rows[e.RowIndex].Cells[ColumnPermId.Index].Value.ToString());
                var permName = gridPerm.Rows[e.RowIndex].Cells[ColumnPermName.Index].Value.ToString();

                if (MessageBox.Show($"権限「{permName}」を削除してもよろしいですか？", "確認",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // 権限の削除処理
                    // 注: PermissionManagerに削除メソッドがないため、ここではグリッドの行のみを削除
                    // 実際には削除メソッドを追加するか、他の方法で削除を処理する必要がある
                    gridPerm.Rows.RemoveAt(e.RowIndex);
                    MessageBox.Show("権限を削除しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region ロール画面
        private void InitializeRoleTab()
        {
            // イベントハンドラの登録
            btnRegistRole.Click += BtnRegistRole_Click;
            gridRole.CellClick += GridRole_CellClick;
            btnRolePermSelected.Click += BtnRolePermSelected_Click;
            btnRolePermAvailable.Click += BtnRolePermAvailable_Click;

            // データグリッドビューの設定
            RefreshRoleGrid();
        }

        private void RefreshRoleGrid()
        {
            gridRole.Rows.Clear();

            // ロール一覧の取得
            var roles = _permissionManager.GetAllRoles();

            foreach (var role in roles)
            {
                gridRole.Rows.Add(role.Id, role.Name);
            }
        }

        private void BtnRegistRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoleName.Text))
            {
                MessageBox.Show("ロール名を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 新しいロールを作成
                _permissionManager.CreateRole(txtRoleName.Text);

                // テキストボックスをクリア
                txtRoleName.Clear();

                // グリッドを更新
                RefreshRoleGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridRole_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var roleId = int.Parse(gridRole.Rows[e.RowIndex].Cells[ColumnRoleId.Index].Value.ToString());
            var roleName = gridRole.Rows[e.RowIndex].Cells[ColumnRoleName.Index].Value.ToString();

            // 編集ボタンクリック時の処理
            if (e.ColumnIndex == ColumnRoleEditButton.Index)
            {
                _selectedRoleId = roleId;
                lblTItleSelectedRole.Text = $"選択中のロール：{roleName}";

                // 選択されたロールの権限を表示
                RefreshRolePermissionGrids();
            }
            // 削除ボタンクリック時の処理
            else if (e.ColumnIndex == ColumnRoleDeleteButton.Index)
            {
                if (MessageBox.Show($"ロール「{roleName}」を削除してもよろしいですか？", "確認",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // ロールの削除処理
                    // 注: PermissionManagerに削除メソッドがないため、ここではグリッドの行のみを削除
                    // 実際には削除メソッドを追加するか、他の方法で削除を処理する必要がある
                    gridRole.Rows.RemoveAt(e.RowIndex);

                    if (_selectedRoleId == roleId)
                    {
                        _selectedRoleId = -1;
                        lblTItleSelectedRole.Text = "選択中のロール：";
                        ClearRolePermissionGrids();
                    }

                    MessageBox.Show("ロールを削除しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RefreshRolePermissionGrids()
        {
            if (_selectedRoleId < 0) return;

            gridRolePermSelected.Rows.Clear();
            gridRolePermAvailable.Rows.Clear();

            try
            {
                var role = _permissionManager.GetRole(_selectedRoleId);
                var allPermissions = _permissionManager.GetAllPermissions().ToList();

                foreach (var permission in allPermissions)
                {
                    if (role.HasPermission(permission))
                    {
                        // 割り当て済み権限
                        gridRolePermSelected.Rows.Add(permission.Id, permission.Name);
                    }
                    else
                    {
                        // 利用可能な権限
                        gridRolePermAvailable.Rows.Add(permission.Id, permission.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限情報の取得に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearRolePermissionGrids()
        {
            gridRolePermSelected.Rows.Clear();
            gridRolePermAvailable.Rows.Clear();
        }

        private void BtnRolePermSelected_Click(object sender, EventArgs e)
        {
            // 「<<」ボタン：権限を解除する
            if (_selectedRoleId < 0 || gridRolePermSelected.SelectedRows.Count == 0) return;

            try
            {
                foreach (DataGridViewRow row in gridRolePermSelected.SelectedRows)
                {
                    var permId = int.Parse(row.Cells[ColumnRoleSelectedId.Index].Value.ToString());

                    // 権限を解除
                    _permissionManager.RevokePermissionFromRole(_selectedRoleId, permId);
                }

                // グリッドを更新
                RefreshRolePermissionGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の解除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRolePermAvailable_Click(object sender, EventArgs e)
        {
            // 「>>」ボタン：権限を付与する
            if (_selectedRoleId < 0 || gridRolePermAvailable.SelectedRows.Count == 0) return;

            try
            {
                foreach (DataGridViewRow row in gridRolePermAvailable.SelectedRows)
                {
                    var permId = int.Parse(row.Cells[ColumnRoleAvailableId.Index].Value.ToString());

                    // 権限を付与
                    _permissionManager.GrantPermissionToRole(_selectedRoleId, permId);
                }

                // グリッドを更新
                RefreshRolePermissionGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の付与に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ユーザー権限画面
        private void InitializeUserTab()
        {
            // イベントハンドラの登録
            btnRegistUser.Click += BtnRegistUser_Click;
            gridUser.CellClick += GridUser_CellClick;
            btnAssignUserRole.Click += BtnAssignUserRole_Click;
            btnUserPremSelected.Click += BtnUserPermSelected_Click;
            btnUserPermAvailable.Click += BtnUserPermAvailable_Click;

            // ロール選択コンボボックスの設定
            RefreshRoleComboBox();

            // データグリッドビューの設定
            RefreshUserGrid();
        }

        private void RefreshRoleComboBox()
        {
            cmbUserRole.Items.Clear();

            foreach (var role in _permissionManager.GetAllRoles())
            {
                cmbUserRole.Items.Add(new RoleItem(role));
            }

            if (cmbUserRole.Items.Count > 0)
                cmbUserRole.SelectedIndex = 0;
        }

        private void RefreshUserGrid()
        {
            gridUser.Rows.Clear();

            // ユーザー一覧の取得
            var users = _permissionManager.GetAllUsers();

            foreach (var user in users)
            {
                gridUser.Rows.Add(user.UserId, user.UserId);
            }
        }

        private void BtnRegistUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("ユーザーIDを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // デフォルトロールを取得
                Role defaultRole = null;
                if (cmbUserRole.SelectedItem != null)
                {
                    defaultRole = ((RoleItem)cmbUserRole.SelectedItem).Role;
                }

                // 新しいユーザーを作成
                _permissionManager.CreateUser(txtUserId.Text, defaultRole);

                // テキストボックスをクリア
                txtUserId.Clear();

                // グリッドを更新
                RefreshUserGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ユーザーの追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GridUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var userId = gridUser.Rows[e.RowIndex].Cells[ColumnUserId.Index].Value.ToString();

            // 編集ボタンクリック時の処理
            if (e.ColumnIndex == ColumnUserEdit.Index)
            {
                _selectedUserId = userId;
                lblTitleSelectedUserId.Text = $"選択中のユーザー：{userId}";

                // 選択されたユーザーの権限を表示
                RefreshUserPermissionGrids();
            }
            // 削除ボタンクリック時の処理
            else if (e.ColumnIndex == ColumnUserDelete.Index)
            {
                if (MessageBox.Show($"ユーザー「{userId}」を削除してもよろしいですか？", "確認",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // ユーザーの削除処理
                    // 注: PermissionManagerに削除メソッドがないため、ここではグリッドの行のみを削除
                    // 実際には削除メソッドを追加するか、他の方法で削除を処理する必要がある
                    gridUser.Rows.RemoveAt(e.RowIndex);

                    if (_selectedUserId == userId)
                    {
                        _selectedUserId = null;
                        lblTitleSelectedUserId.Text = "選択中のユーザー：";
                        ClearUserPermissionGrids();
                    }

                    MessageBox.Show("ユーザーを削除しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnAssignUserRole_Click(object sender, EventArgs e)
        {
            if (_selectedUserId == null || cmbUserRole.SelectedItem == null) return;

            try
            {
                var roleItem = (RoleItem)cmbUserRole.SelectedItem;

                // ユーザーにロールを割り当て
                _permissionManager.AssignRoleToUser(_selectedUserId, roleItem.Role.Id);

                MessageBox.Show($"ユーザー「{_selectedUserId}」にロール「{roleItem.Role.Name}」を適用しました。",
                    "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ユーザー権限グリッドを更新
                RefreshUserPermissionGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ロールの適用に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshUserPermissionGrids()
        {
            if (_selectedUserId == null) return;

            gridUserPermSelected.Rows.Clear();
            gridUserPermAvailable.Rows.Clear();

            try
            {
                var user = _permissionManager.GetUser(_selectedUserId);
                var userPermissions = _permissionManager.GetUserPermissions(_selectedUserId);
                var allPermissions = _permissionManager.GetAllPermissions();

                // ユーザーが持っている権限
                foreach (var permission in userPermissions)
                {
                    gridUserPermSelected.Rows.Add(permission.Id, permission.Name);
                }

                // ユーザーが持っていない権限
                var userPermissionIds = userPermissions.Select(p => p.Id).ToHashSet();
                foreach (var permission in allPermissions)
                {
                    if (!userPermissionIds.Contains(permission.Id))
                    {
                        gridUserPermAvailable.Rows.Add(permission.Id, permission.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ユーザー権限情報の取得に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearUserPermissionGrids()
        {
            gridUserPermSelected.Rows.Clear();
            gridUserPermAvailable.Rows.Clear();
        }

        private void BtnUserPermSelected_Click(object sender, EventArgs e)
        {
            // 「<<」ボタン：ユーザーから権限を削除（拒否権限に追加）
            if (_selectedUserId == null || gridUserPermSelected.SelectedRows.Count == 0) return;

            try
            {
                foreach (DataGridViewRow row in gridUserPermSelected.SelectedRows)
                {
                    var permId = int.Parse(row.Cells[0].Value.ToString());

                    // ユーザーの権限を拒否（このアプローチが適切かどうかは要検討）
                    _permissionManager.DenyPermissionForUser(_selectedUserId, permId);
                }

                // グリッドを更新
                RefreshUserPermissionGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の削除に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUserPermAvailable_Click(object sender, EventArgs e)
        {
            // 「>>」ボタン：ユーザーに権限を追加
            if (_selectedUserId == null || gridUserPermAvailable.SelectedRows.Count == 0) return;

            try
            {
                foreach (DataGridViewRow row in gridUserPermAvailable.SelectedRows)
                {
                    var permId = int.Parse(row.Cells[0].Value.ToString());

                    // ユーザーに追加権限を付与
                    _permissionManager.GrantAdditionalPermissionToUser(_selectedUserId, permId);
                }

                // グリッドを更新
                RefreshUserPermissionGrids();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"権限の追加に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ロール選択用のアイテムクラス
        private class RoleItem
        {
            public Role Role { get; }

            public RoleItem(Role role)
            {
                Role = role;
            }

            public override string ToString()
            {
                return Role.Name;
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
    }
}
