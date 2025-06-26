using CoreLibWinforms.Core.Permissions;
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
    public partial class FormPermission : Form
    {
        readonly PermissionManager _permissionManager;

        public FormPermission(PermissionManager permissionManager = null)
        {
            InitializeComponent();

            _permissionManager = permissionManager ?? new PermissionManager();

        }
        private void FormPermission_Load(object sender, EventArgs e)
        {
            // 初期データの読み込み
            RefreshAllViews();
        }

        #region Common Methods

        private void RefreshAllViews()
        {
            RefreshPermissionsView();
            RefreshUsersView();
            RefreshLocationsView();
            RefreshControlsView();
        }

        private void PopulateAvailablePermissionsList(ListBox listBox, IEnumerable<string> excludePermissions)
        {
            listBox.Items.Clear();
            foreach (var permission in _permissionManager.AvailablePermissions)
            {
                if (!excludePermissions.Contains(permission.Id))
                {
                    listBox.Items.Add(permission.Id);
                }
            }
        }

        #endregion

        #region Permission Tab Methods

        private void RefreshPermissionsView()
        {
            listViewPermissions.Items.Clear();
            foreach (var permission in _permissionManager.AvailablePermissions)
            {
                var item = new ListViewItem(permission.Id);
                item.SubItems.Add(permission.Description);
                item.Tag = permission;
                listViewPermissions.Items.Add(item);
            }

            // テキストボックスをクリア
            txtPermissionId.Clear();
            txtPermissionDescription.Clear();
        }

        private void listViewPermissions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPermissions.SelectedItems.Count > 0)
            {
                var permission = (Permission)listViewPermissions.SelectedItems[0].Tag;
                txtPermissionId.Text = permission.Id;
                txtPermissionDescription.Text = permission.Description;
            }
        }

        private void btnAddPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPermissionId.Text))
            {
                MessageBox.Show("権限IDを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _permissionManager.AddPermission(txtPermissionId.Text, txtPermissionDescription.Text);
            RefreshPermissionsView();
        }

        private void btnRemovePermission_Click(object sender, EventArgs e)
        {
            if (listViewPermissions.SelectedItems.Count > 0)
            {
                var permission = (Permission)listViewPermissions.SelectedItems[0].Tag;
                _permissionManager.RemovePermission(permission.Id);
                RefreshPermissionsView();
            }
            else
            {
                MessageBox.Show("削除する権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region User Permissions Tab Methods

        private void RefreshUsersView()
        {
            // コンボボックスの更新
            comboBoxUsers.Items.Clear();
            foreach (var user in _permissionManager.UserPermissions)
            {
                comboBoxUsers.Items.Add($"{user.UserId} - {user.UserName}");
            }

            if (comboBoxUsers.Items.Count > 0)
            {
                comboBoxUsers.SelectedIndex = 0;
            }
            else
            {
                // ユーザーデータがない場合、テキストボックスとリストをクリア
                txtUserId.Clear();
                txtUserName.Clear();
                listBoxUserPermissions.Items.Clear();
                listBoxAvailablePermissionsUser.Items.Clear();
            }
        }

        private void comboBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedIndex >= 0)
            {
                var selectedUser = _permissionManager.UserPermissions[comboBoxUsers.SelectedIndex];

                txtUserId.Text = selectedUser.UserId;
                txtUserName.Text = selectedUser.UserName;

                // 付与済み権限リストの更新
                listBoxUserPermissions.Items.Clear();
                foreach (var permId in selectedUser.GrantedPermissions)
                {
                    listBoxUserPermissions.Items.Add(permId);
                }

                // 利用可能権限リストの更新
                PopulateAvailablePermissionsList(listBoxAvailablePermissionsUser, selectedUser.GrantedPermissions);
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserId.Text))
            {
                MessageBox.Show("ユーザーIDを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 既存のユーザーIDをチェック
            if (_permissionManager.UserPermissions.Any(u => u.UserId == txtUserId.Text))
            {
                MessageBox.Show("同じIDのユーザーが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _permissionManager.AddUserPermission(txtUserId.Text, txtUserName.Text);
            RefreshUsersView();

            // 新規追加したユーザーを選択
            int newIndex = _permissionManager.UserPermissions.FindIndex(u => u.UserId == txtUserId.Text);
            if (newIndex >= 0)
            {
                comboBoxUsers.SelectedIndex = newIndex;
            }
        }

        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedIndex >= 0)
            {
                var selectedUser = _permissionManager.UserPermissions[comboBoxUsers.SelectedIndex];
                _permissionManager.RemoveUserPermission(selectedUser.UserId);
                RefreshUsersView();
            }
            else
            {
                MessageBox.Show("削除するユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGrantUserPermission_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedIndex < 0)
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listBoxAvailablePermissionsUser.SelectedIndex < 0)
            {
                MessageBox.Show("付与する権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedUser = _permissionManager.UserPermissions[comboBoxUsers.SelectedIndex];
            var permissionId = listBoxAvailablePermissionsUser.SelectedItem.ToString();

            _permissionManager.GrantPermissionToUser(selectedUser.UserId, permissionId);

            // リスト表示を更新
            listBoxUserPermissions.Items.Add(permissionId);
            listBoxAvailablePermissionsUser.Items.RemoveAt(listBoxAvailablePermissionsUser.SelectedIndex);
        }

        private void btnRevokeUserPermission_Click(object sender, EventArgs e)
        {
            if (comboBoxUsers.SelectedIndex < 0)
            {
                MessageBox.Show("ユーザーを選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listBoxUserPermissions.SelectedIndex < 0)
            {
                MessageBox.Show("削除する権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedUser = _permissionManager.UserPermissions[comboBoxUsers.SelectedIndex];
            var permissionId = listBoxUserPermissions.SelectedItem.ToString();

            _permissionManager.RevokePermissionFromUser(selectedUser.UserId, permissionId);

            // リスト表示を更新
            listBoxAvailablePermissionsUser.Items.Add(permissionId);
            listBoxUserPermissions.Items.RemoveAt(listBoxUserPermissions.SelectedIndex);
        }

        #endregion

        #region Location Permissions Tab Methods

        private void RefreshLocationsView()
        {
            // コンボボックスの更新
            comboBoxLocations.Items.Clear();
            foreach (var location in _permissionManager.LocationPermissions)
            {
                comboBoxLocations.Items.Add($"{location.LocationId} - {location.LocationName}");
            }

            if (comboBoxLocations.Items.Count > 0)
            {
                comboBoxLocations.SelectedIndex = 0;
            }
            else
            {
                // 場所データがない場合、テキストボックスとリストをクリア
                txtLocationId.Clear();
                txtLocationName.Clear();
                listBoxLocationPermissions.Items.Clear();
                listBoxAvailablePermissionsLocation.Items.Clear();
            }
        }

        private void comboBoxLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLocations.SelectedIndex >= 0)
            {
                var selectedLocation = _permissionManager.LocationPermissions[comboBoxLocations.SelectedIndex];

                txtLocationId.Text = selectedLocation.LocationId;
                txtLocationName.Text = selectedLocation.LocationName;

                // 付与済み権限リストの更新
                listBoxLocationPermissions.Items.Clear();
                foreach (var permId in selectedLocation.GrantedPermissions)
                {
                    listBoxLocationPermissions.Items.Add(permId);
                }

                // 利用可能権限リストの更新
                PopulateAvailablePermissionsList(listBoxAvailablePermissionsLocation, selectedLocation.GrantedPermissions);
            }
        }

        private void btnAddLocation_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocationId.Text))
            {
                MessageBox.Show("場所IDを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 既存の場所IDをチェック
            if (_permissionManager.LocationPermissions.Any(l => l.LocationId == txtLocationId.Text))
            {
                MessageBox.Show("同じIDの場所が既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _permissionManager.AddLocationPermission(txtLocationId.Text, txtLocationName.Text);
            RefreshLocationsView();

            // 新規追加した場所を選択
            int newIndex = _permissionManager.LocationPermissions.FindIndex(l => l.LocationId == txtLocationId.Text);
            if (newIndex >= 0)
            {
                comboBoxLocations.SelectedIndex = newIndex;
            }
        }

        private void btnRemoveLocation_Click(object sender, EventArgs e)
        {
            if (comboBoxLocations.SelectedIndex >= 0)
            {
                var selectedLocation = _permissionManager.LocationPermissions[comboBoxLocations.SelectedIndex];
                _permissionManager.RemoveLocationPermission(selectedLocation.LocationId);
                RefreshLocationsView();
            }
            else
            {
                MessageBox.Show("削除する場所を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGrantLocationPermission_Click(object sender, EventArgs e)
        {
            if (comboBoxLocations.SelectedIndex < 0)
            {
                MessageBox.Show("場所を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listBoxAvailablePermissionsLocation.SelectedIndex < 0)
            {
                MessageBox.Show("付与する権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedLocation = _permissionManager.LocationPermissions[comboBoxLocations.SelectedIndex];
            var permissionId = listBoxAvailablePermissionsLocation.SelectedItem.ToString();

            _permissionManager.GrantPermissionToLocation(selectedLocation.LocationId, permissionId);

            // リスト表示を更新
            listBoxLocationPermissions.Items.Add(permissionId);
            listBoxAvailablePermissionsLocation.Items.RemoveAt(listBoxAvailablePermissionsLocation.SelectedIndex);
        }

        private void btnRevokeLocationPermission_Click(object sender, EventArgs e)
        {
            if (comboBoxLocations.SelectedIndex < 0)
            {
                MessageBox.Show("場所を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listBoxLocationPermissions.SelectedIndex < 0)
            {
                MessageBox.Show("削除する権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedLocation = _permissionManager.LocationPermissions[comboBoxLocations.SelectedIndex];
            var permissionId = listBoxLocationPermissions.SelectedItem.ToString();

            _permissionManager.RevokePermissionFromLocation(selectedLocation.LocationId, permissionId);

            // リスト表示を更新
            listBoxAvailablePermissionsLocation.Items.Add(permissionId);
            listBoxLocationPermissions.Items.RemoveAt(listBoxLocationPermissions.SelectedIndex);
        }

        #endregion

        #region Control Permissions Tab Methods

        private void RefreshControlsView()
        {
            listViewControlPermissions.Items.Clear();

            foreach (var controlPerm in _permissionManager.ControlPermissions)
            {
                var item = new ListViewItem(controlPerm.FormName);
                item.SubItems.Add(controlPerm.ControlName);
                item.SubItems.Add(controlPerm.RequiredPermission);
                item.Tag = controlPerm;
                listViewControlPermissions.Items.Add(item);
            }

            // テキストボックスをクリア
            txtFormName.Clear();
            txtControlName.Clear();
            txtRequiredPermission.Clear();
        }

        private void listViewControlPermissions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewControlPermissions.SelectedItems.Count > 0)
            {
                var controlPerm = (ControlPermission)listViewControlPermissions.SelectedItems[0].Tag;
                txtFormName.Text = controlPerm.FormName;
                txtControlName.Text = controlPerm.ControlName;
                txtRequiredPermission.Text = controlPerm.RequiredPermission;
            }
        }

        private void btnAddControlPermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFormName.Text) || string.IsNullOrWhiteSpace(txtControlName.Text) ||
                string.IsNullOrWhiteSpace(txtRequiredPermission.Text))
            {
                MessageBox.Show("フォーム名、コントロール名、必要な権限をすべて入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 既存のコントロール権限をチェック
            if (_permissionManager.GetControlPermission(txtFormName.Text, txtControlName.Text) != null)
            {
                MessageBox.Show("同じフォームとコントロールの組み合わせが既に存在します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _permissionManager.AddControlPermission(txtFormName.Text, txtControlName.Text, txtRequiredPermission.Text);
            RefreshControlsView();
        }

        private void btnRemoveControlPermission_Click(object sender, EventArgs e)
        {
            if (listViewControlPermissions.SelectedItems.Count > 0)
            {
                var controlPerm = (ControlPermission)listViewControlPermissions.SelectedItems[0].Tag;
                _permissionManager.RemoveControlPermission(controlPerm.FormName, controlPerm.ControlName);
                RefreshControlsView();
            }
            else
            {
                MessageBox.Show("削除するコントロール権限を選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Common Button Actions

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 保存ダイアログを表示
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*";
                    dlg.DefaultExt = ".json";
                    dlg.FileName = "permissions.json";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _permissionManager.Save(dlg.FileName);
                        MessageBox.Show("権限設定を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // 読み込みダイアログを表示
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "JSONファイル (*.json)|*.json|すべてのファイル (*.*)|*.*";
                    dlg.DefaultExt = ".json";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        _permissionManager.Load(dlg.FileName);
                        RefreshAllViews();
                        MessageBox.Show("権限設定を読み込みました。", "読み込み完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"読み込み中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInitDefault_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("デフォルト権限を初期化しますか？既存の権限設定は失われます。", "確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 既存の権限をクリア
                _permissionManager.AvailablePermissions.Clear();

                // デフォルト権限を初期化
                _permissionManager.InitializeDefaultPermissions();

                RefreshAllViews();
                MessageBox.Show("デフォルト権限を初期化しました。", "初期化完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}
