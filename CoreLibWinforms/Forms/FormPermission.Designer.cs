using System.Windows.Forms;

namespace CoreLibWinforms.Forms
{
    partial class FormPermission
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabUserPermissions = new TabPage();
            gridUserAdditionalPermissionsAvailable = new DataGridView();
            ColumnUserAdditionalPermissionAvailableId = new DataGridViewTextBoxColumn();
            ColumnUserAdditionalPermissionAvailableName = new DataGridViewTextBoxColumn();
            btnSelectUserAdditionalPermission = new Button();
            btnDeSelectUserAdditionalPermission = new Button();
            gridUserAdditionalPermissionsSelected = new DataGridView();
            ColumnUserAdditionalPermissionSelectedId = new DataGridViewTextBoxColumn();
            ColumnUserAdditionalPermissionSelectedName = new DataGridViewTextBoxColumn();
            btnSave = new Button();
            controlMappingTab = new TabPage();
            splitContainer2 = new SplitContainer();
            gridDept = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            lblTitleDepartment = new Label();
            btnSelectDeptPerm = new Button();
            btnDeSelectDeptPerm = new Button();
            gridDeptPermAvailable = new DataGridView();
            ColumnDeptPermIdAvailable = new DataGridViewTextBoxColumn();
            ColumnDeptPermNameAvailable = new DataGridViewTextBoxColumn();
            gridDeptPermSelected = new DataGridView();
            ColumnDeptPermIdSelected = new DataGridViewTextBoxColumn();
            ColumnDeptPermNameSelected = new DataGridViewTextBoxColumn();
            lblTitleSelectedDept = new Label();
            lblTitleUserAllPermission = new Label();
            lblSelectedUser = new Label();
            lblSelectedRole = new Label();
            btnAddRole = new Button();
            lblRoleName = new Label();
            txtRoleName = new TextBox();
            combinedPermissionsTab = new TabPage();
            splitContainer1 = new SplitContainer();
            gridRole = new DataGridView();
            ColumnRoleId = new DataGridViewTextBoxColumn();
            ColumnRoleName = new DataGridViewTextBoxColumn();
            ColumnRoleDeleteButton = new DataGridViewButtonColumn();
            btnSelectRole = new Button();
            btnDeSelectRole = new Button();
            gridRoleAvailable = new DataGridView();
            ColumnRoleAvailableId = new DataGridViewTextBoxColumn();
            ColumnRoleAvailableName = new DataGridViewTextBoxColumn();
            gridRoleSelected = new DataGridView();
            ColumnRoleSelectedId = new DataGridViewTextBoxColumn();
            ColumnRoleSelectedName = new DataGridViewTextBoxColumn();
            txtPermissionName = new TextBox();
            btnAddPermission = new Button();
            lblPermissionName = new Label();
            basicPermissionsTab = new TabPage();
            gridPermission = new DataGridView();
            ColumnPermId = new DataGridViewTextBoxColumn();
            ColumnPermName = new DataGridViewTextBoxColumn();
            ColumnPermDeleteButton = new DataGridViewButtonColumn();
            tabControl = new TabControl();
            userPermissionsTab = new TabPage();
            splitContainer3 = new SplitContainer();
            panel5 = new Panel();
            gridUser = new DataGridView();
            ColumnUserId = new DataGridViewTextBoxColumn();
            ColumnUserDeleteButton = new DataGridViewButtonColumn();
            btnAddUserId = new Button();
            lblUserId = new Label();
            txtUserId = new TextBox();
            gridUserAllPermissions = new DataGridView();
            ColumnUserAllPermissionId = new DataGridViewTextBoxColumn();
            ColumnUserAllPermissionsName = new DataGridViewTextBoxColumn();
            tabControlUser = new TabControl();
            tabUserRoles = new TabPage();
            gridUserRoleAvailable = new DataGridView();
            ColumnUserRoleAvailableId = new DataGridViewTextBoxColumn();
            ColumnUserRoleAvailableName = new DataGridViewTextBoxColumn();
            btnSelectUserRole = new Button();
            btnDeSelectUserRole = new Button();
            gridUserRoleSelected = new DataGridView();
            ColumnUserRoleSelectedId = new DataGridViewTextBoxColumn();
            ColumnUserRoleSelectedName = new DataGridViewTextBoxColumn();
            tabPage1 = new TabPage();
            splitContainer4 = new SplitContainer();
            btnAddControl = new Button();
            txtControlControlName = new TextBox();
            lblTitleControlControlName = new Label();
            txtControlFormName = new TextBox();
            gridControl = new DataGridView();
            ColumnControlFormName = new DataGridViewTextBoxColumn();
            ColumnControlControlName = new DataGridViewTextBoxColumn();
            ColumnControlDeleteButton = new DataGridViewButtonColumn();
            label3 = new Label();
            tabControlControl = new TabControl();
            tabPageControlRole = new TabPage();
            gridControlRole = new DataGridView();
            ColumnControlRoleId = new DataGridViewTextBoxColumn();
            ColumnControlRoleName = new DataGridViewTextBoxColumn();
            ColumnControlRoleVisible = new DataGridViewCheckBoxColumn();
            ColumnControlRoleEnabled = new DataGridViewCheckBoxColumn();
            ColumnControlRoleReadOnly = new DataGridViewCheckBoxColumn();
            tabPageControlDept = new TabPage();
            gridControlDept = new DataGridView();
            ColumnControlDeptId = new DataGridViewTextBoxColumn();
            ColumnControlDeptName = new DataGridViewTextBoxColumn();
            ColumnControlDeptVisible = new DataGridViewCheckBoxColumn();
            ColumnControlDeptEnabled = new DataGridViewCheckBoxColumn();
            ColumnControlDeptReadOnly = new DataGridViewCheckBoxColumn();
            lblTitleSelectedControl = new Label();
            btnCancel = new Button();
            tabUserPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsSelected).BeginInit();
            controlMappingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridDept).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridDeptPermAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridDeptPermSelected).BeginInit();
            combinedPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRole).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRoleAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRoleSelected).BeginInit();
            basicPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridPermission).BeginInit();
            tabControl.SuspendLayout();
            userPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUser).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAllPermissions).BeginInit();
            tabControlUser.SuspendLayout();
            tabUserRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUserRoleAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUserRoleSelected).BeginInit();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            tabControlControl.SuspendLayout();
            tabPageControlRole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlRole).BeginInit();
            tabPageControlDept.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlDept).BeginInit();
            SuspendLayout();
            // 
            // tabUserPermissions
            // 
            tabUserPermissions.Controls.Add(gridUserAdditionalPermissionsAvailable);
            tabUserPermissions.Controls.Add(btnSelectUserAdditionalPermission);
            tabUserPermissions.Controls.Add(btnDeSelectUserAdditionalPermission);
            tabUserPermissions.Controls.Add(gridUserAdditionalPermissionsSelected);
            tabUserPermissions.Location = new Point(4, 24);
            tabUserPermissions.Name = "tabUserPermissions";
            tabUserPermissions.Size = new Size(328, 537);
            tabUserPermissions.TabIndex = 1;
            tabUserPermissions.Text = "追加権限";
            // 
            // gridUserAdditionalPermissionsAvailable
            // 
            gridUserAdditionalPermissionsAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserAdditionalPermissionsAvailable.Columns.AddRange(new DataGridViewColumn[] { ColumnUserAdditionalPermissionAvailableId, ColumnUserAdditionalPermissionAvailableName });
            gridUserAdditionalPermissionsAvailable.Location = new Point(3, 290);
            gridUserAdditionalPermissionsAvailable.Name = "gridUserAdditionalPermissionsAvailable";
            gridUserAdditionalPermissionsAvailable.Size = new Size(322, 233);
            gridUserAdditionalPermissionsAvailable.TabIndex = 14;
            // 
            // ColumnUserAdditionalPermissionAvailableId
            // 
            ColumnUserAdditionalPermissionAvailableId.DataPropertyName = "Id";
            ColumnUserAdditionalPermissionAvailableId.HeaderText = "ID";
            ColumnUserAdditionalPermissionAvailableId.Name = "ColumnUserAdditionalPermissionAvailableId";
            // 
            // ColumnUserAdditionalPermissionAvailableName
            // 
            ColumnUserAdditionalPermissionAvailableName.DataPropertyName = "Name";
            ColumnUserAdditionalPermissionAvailableName.HeaderText = "Name";
            ColumnUserAdditionalPermissionAvailableName.Name = "ColumnUserAdditionalPermissionAvailableName";
            ColumnUserAdditionalPermissionAvailableName.Width = 200;
            // 
            // btnSelectUserAdditionalPermission
            // 
            btnSelectUserAdditionalPermission.Location = new Point(167, 244);
            btnSelectUserAdditionalPermission.Name = "btnSelectUserAdditionalPermission";
            btnSelectUserAdditionalPermission.Size = new Size(158, 40);
            btnSelectUserAdditionalPermission.TabIndex = 13;
            btnSelectUserAdditionalPermission.Text = "<< 追加";
            btnSelectUserAdditionalPermission.UseVisualStyleBackColor = true;
            btnSelectUserAdditionalPermission.Click += btnSelectUserAdditionalPermission_Click;
            // 
            // btnDeSelectUserAdditionalPermission
            // 
            btnDeSelectUserAdditionalPermission.Location = new Point(3, 244);
            btnDeSelectUserAdditionalPermission.Name = "btnDeSelectUserAdditionalPermission";
            btnDeSelectUserAdditionalPermission.Size = new Size(158, 40);
            btnDeSelectUserAdditionalPermission.TabIndex = 12;
            btnDeSelectUserAdditionalPermission.Text = "削除 >>";
            btnDeSelectUserAdditionalPermission.UseVisualStyleBackColor = true;
            btnDeSelectUserAdditionalPermission.Click += btnDeSelectUserAdditionalPermission_Click;
            // 
            // gridUserAdditionalPermissionsSelected
            // 
            gridUserAdditionalPermissionsSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserAdditionalPermissionsSelected.Columns.AddRange(new DataGridViewColumn[] { ColumnUserAdditionalPermissionSelectedId, ColumnUserAdditionalPermissionSelectedName });
            gridUserAdditionalPermissionsSelected.Location = new Point(3, 5);
            gridUserAdditionalPermissionsSelected.Name = "gridUserAdditionalPermissionsSelected";
            gridUserAdditionalPermissionsSelected.Size = new Size(322, 233);
            gridUserAdditionalPermissionsSelected.TabIndex = 11;
            // 
            // ColumnUserAdditionalPermissionSelectedId
            // 
            ColumnUserAdditionalPermissionSelectedId.DataPropertyName = "Id";
            ColumnUserAdditionalPermissionSelectedId.HeaderText = "ID";
            ColumnUserAdditionalPermissionSelectedId.Name = "ColumnUserAdditionalPermissionSelectedId";
            // 
            // ColumnUserAdditionalPermissionSelectedName
            // 
            ColumnUserAdditionalPermissionSelectedName.DataPropertyName = "Name";
            ColumnUserAdditionalPermissionSelectedName.HeaderText = "Name";
            ColumnUserAdditionalPermissionSelectedName.Name = "ColumnUserAdditionalPermissionSelectedName";
            ColumnUserAdditionalPermissionSelectedName.Width = 200;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Font = new Font("メイリオ", 14.25F);
            btnSave.Location = new Point(735, 677);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(158, 49);
            btnSave.TabIndex = 6;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // controlMappingTab
            // 
            controlMappingTab.BackColor = SystemColors.Control;
            controlMappingTab.Controls.Add(splitContainer2);
            controlMappingTab.Location = new Point(4, 33);
            controlMappingTab.Name = "controlMappingTab";
            controlMappingTab.Padding = new Padding(3);
            controlMappingTab.Size = new Size(870, 635);
            controlMappingTab.TabIndex = 3;
            controlMappingTab.Text = "部署権限";
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(3, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(gridDept);
            splitContainer2.Panel1.Controls.Add(lblTitleDepartment);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(btnSelectDeptPerm);
            splitContainer2.Panel2.Controls.Add(btnDeSelectDeptPerm);
            splitContainer2.Panel2.Controls.Add(gridDeptPermAvailable);
            splitContainer2.Panel2.Controls.Add(gridDeptPermSelected);
            splitContainer2.Panel2.Controls.Add(lblTitleSelectedDept);
            splitContainer2.Size = new Size(864, 629);
            splitContainer2.SplitterDistance = 335;
            splitContainer2.TabIndex = 1;
            // 
            // gridDept
            // 
            gridDept.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDept.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            gridDept.Location = new Point(3, 33);
            gridDept.Name = "gridDept";
            gridDept.Size = new Size(324, 592);
            gridDept.TabIndex = 3;
            gridDept.SelectionChanged += gridDept_SelectionChanged;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.DataPropertyName = "Id";
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.DataPropertyName = "Name";
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 200;
            // 
            // lblTitleDepartment
            // 
            lblTitleDepartment.AutoSize = true;
            lblTitleDepartment.Location = new Point(3, 6);
            lblTitleDepartment.Name = "lblTitleDepartment";
            lblTitleDepartment.Size = new Size(49, 24);
            lblTitleDepartment.TabIndex = 0;
            lblTitleDepartment.Text = "部署:";
            // 
            // btnSelectDeptPerm
            // 
            btnSelectDeptPerm.Location = new Point(265, 585);
            btnSelectDeptPerm.Name = "btnSelectDeptPerm";
            btnSelectDeptPerm.Size = new Size(256, 40);
            btnSelectDeptPerm.TabIndex = 7;
            btnSelectDeptPerm.Text = "<< 追加";
            btnSelectDeptPerm.UseVisualStyleBackColor = true;
            btnSelectDeptPerm.Click += btnSelectDeptPerm_Click;
            // 
            // btnDeSelectDeptPerm
            // 
            btnDeSelectDeptPerm.Location = new Point(3, 585);
            btnDeSelectDeptPerm.Name = "btnDeSelectDeptPerm";
            btnDeSelectDeptPerm.Size = new Size(256, 40);
            btnDeSelectDeptPerm.TabIndex = 6;
            btnDeSelectDeptPerm.Text = "削除 >>";
            btnDeSelectDeptPerm.UseVisualStyleBackColor = true;
            btnDeSelectDeptPerm.Click += btnDeSelectDeptPerm_Click;
            // 
            // gridDeptPermAvailable
            // 
            gridDeptPermAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDeptPermAvailable.Columns.AddRange(new DataGridViewColumn[] { ColumnDeptPermIdAvailable, ColumnDeptPermNameAvailable });
            gridDeptPermAvailable.Location = new Point(265, 70);
            gridDeptPermAvailable.Name = "gridDeptPermAvailable";
            gridDeptPermAvailable.Size = new Size(256, 509);
            gridDeptPermAvailable.TabIndex = 5;
            // 
            // ColumnDeptPermIdAvailable
            // 
            ColumnDeptPermIdAvailable.DataPropertyName = "Id";
            ColumnDeptPermIdAvailable.HeaderText = "ID";
            ColumnDeptPermIdAvailable.Name = "ColumnDeptPermIdAvailable";
            // 
            // ColumnDeptPermNameAvailable
            // 
            ColumnDeptPermNameAvailable.DataPropertyName = "Name";
            ColumnDeptPermNameAvailable.HeaderText = "Name";
            ColumnDeptPermNameAvailable.Name = "ColumnDeptPermNameAvailable";
            ColumnDeptPermNameAvailable.Width = 200;
            // 
            // gridDeptPermSelected
            // 
            gridDeptPermSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDeptPermSelected.Columns.AddRange(new DataGridViewColumn[] { ColumnDeptPermIdSelected, ColumnDeptPermNameSelected });
            gridDeptPermSelected.Location = new Point(3, 70);
            gridDeptPermSelected.Name = "gridDeptPermSelected";
            gridDeptPermSelected.Size = new Size(256, 509);
            gridDeptPermSelected.TabIndex = 4;
            // 
            // ColumnDeptPermIdSelected
            // 
            ColumnDeptPermIdSelected.DataPropertyName = "Id";
            ColumnDeptPermIdSelected.HeaderText = "ID";
            ColumnDeptPermIdSelected.Name = "ColumnDeptPermIdSelected";
            // 
            // ColumnDeptPermNameSelected
            // 
            ColumnDeptPermNameSelected.DataPropertyName = "Name";
            ColumnDeptPermNameSelected.HeaderText = "Name";
            ColumnDeptPermNameSelected.Name = "ColumnDeptPermNameSelected";
            ColumnDeptPermNameSelected.Width = 200;
            // 
            // lblTitleSelectedDept
            // 
            lblTitleSelectedDept.AutoSize = true;
            lblTitleSelectedDept.Location = new Point(3, 6);
            lblTitleSelectedDept.Name = "lblTitleSelectedDept";
            lblTitleSelectedDept.Size = new Size(150, 24);
            lblTitleSelectedDept.TabIndex = 0;
            lblTitleSelectedDept.Text = "選択中の部署: なし";
            // 
            // lblTitleUserAllPermission
            // 
            lblTitleUserAllPermission.AutoSize = true;
            lblTitleUserAllPermission.Location = new Point(3, 40);
            lblTitleUserAllPermission.Name = "lblTitleUserAllPermission";
            lblTitleUserAllPermission.Size = new Size(74, 24);
            lblTitleUserAllPermission.TabIndex = 1;
            lblTitleUserAllPermission.Text = "権限一覧";
            // 
            // lblSelectedUser
            // 
            lblSelectedUser.AutoSize = true;
            lblSelectedUser.Location = new Point(3, 6);
            lblSelectedUser.Name = "lblSelectedUser";
            lblSelectedUser.Size = new Size(182, 24);
            lblSelectedUser.TabIndex = 0;
            lblSelectedUser.Text = "選択中のユーザー: なし";
            // 
            // lblSelectedRole
            // 
            lblSelectedRole.AutoSize = true;
            lblSelectedRole.Location = new Point(3, 6);
            lblSelectedRole.Name = "lblSelectedRole";
            lblSelectedRole.Size = new Size(166, 24);
            lblSelectedRole.TabIndex = 0;
            lblSelectedRole.Text = "選択中のロール: なし";
            // 
            // btnAddRole
            // 
            btnAddRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddRole.Location = new Point(253, 33);
            btnAddRole.Name = "btnAddRole";
            btnAddRole.Size = new Size(74, 32);
            btnAddRole.TabIndex = 2;
            btnAddRole.Text = "追加";
            btnAddRole.UseVisualStyleBackColor = true;
            btnAddRole.Click += btnAddRole_Click;
            // 
            // lblRoleName
            // 
            lblRoleName.AutoSize = true;
            lblRoleName.Location = new Point(3, 6);
            lblRoleName.Name = "lblRoleName";
            lblRoleName.Size = new Size(81, 24);
            lblRoleName.TabIndex = 0;
            lblRoleName.Text = "ロール名:";
            // 
            // txtRoleName
            // 
            txtRoleName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRoleName.Location = new Point(8, 33);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.Size = new Size(243, 31);
            txtRoleName.TabIndex = 1;
            // 
            // combinedPermissionsTab
            // 
            combinedPermissionsTab.Controls.Add(splitContainer1);
            combinedPermissionsTab.Location = new Point(4, 33);
            combinedPermissionsTab.Name = "combinedPermissionsTab";
            combinedPermissionsTab.Size = new Size(870, 635);
            combinedPermissionsTab.TabIndex = 1;
            combinedPermissionsTab.Text = "ロール権限";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(gridRole);
            splitContainer1.Panel1.Controls.Add(btnAddRole);
            splitContainer1.Panel1.Controls.Add(txtRoleName);
            splitContainer1.Panel1.Controls.Add(lblRoleName);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnSelectRole);
            splitContainer1.Panel2.Controls.Add(btnDeSelectRole);
            splitContainer1.Panel2.Controls.Add(gridRoleAvailable);
            splitContainer1.Panel2.Controls.Add(gridRoleSelected);
            splitContainer1.Panel2.Controls.Add(lblSelectedRole);
            splitContainer1.Size = new Size(870, 635);
            splitContainer1.SplitterDistance = 338;
            splitContainer1.TabIndex = 0;
            // 
            // gridRole
            // 
            gridRole.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRole.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleId, ColumnRoleName, ColumnRoleDeleteButton });
            gridRole.Location = new Point(8, 70);
            gridRole.Name = "gridRole";
            gridRole.Size = new Size(324, 559);
            gridRole.TabIndex = 3;
            gridRole.SelectionChanged += gridRole_SelectionChanged;
            // 
            // ColumnRoleId
            // 
            ColumnRoleId.DataPropertyName = "Id";
            ColumnRoleId.HeaderText = "ID";
            ColumnRoleId.Name = "ColumnRoleId";
            // 
            // ColumnRoleName
            // 
            ColumnRoleName.DataPropertyName = "Name";
            ColumnRoleName.HeaderText = "Name";
            ColumnRoleName.Name = "ColumnRoleName";
            ColumnRoleName.Width = 200;
            // 
            // ColumnRoleDeleteButton
            // 
            ColumnRoleDeleteButton.HeaderText = "Delete";
            ColumnRoleDeleteButton.Name = "ColumnRoleDeleteButton";
            // 
            // btnSelectRole
            // 
            btnSelectRole.Location = new Point(265, 589);
            btnSelectRole.Name = "btnSelectRole";
            btnSelectRole.Size = new Size(256, 40);
            btnSelectRole.TabIndex = 7;
            btnSelectRole.Text = "<< 追加";
            btnSelectRole.UseVisualStyleBackColor = true;
            btnSelectRole.Click += btnSelectRole_Click;
            // 
            // btnDeSelectRole
            // 
            btnDeSelectRole.Location = new Point(3, 589);
            btnDeSelectRole.Name = "btnDeSelectRole";
            btnDeSelectRole.Size = new Size(256, 40);
            btnDeSelectRole.TabIndex = 6;
            btnDeSelectRole.Text = "削除 >>";
            btnDeSelectRole.UseVisualStyleBackColor = true;
            btnDeSelectRole.Click += btnDeSelectRole_Click;
            // 
            // gridRoleAvailable
            // 
            gridRoleAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRoleAvailable.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleAvailableId, ColumnRoleAvailableName });
            gridRoleAvailable.Location = new Point(265, 74);
            gridRoleAvailable.Name = "gridRoleAvailable";
            gridRoleAvailable.Size = new Size(256, 509);
            gridRoleAvailable.TabIndex = 5;
            // 
            // ColumnRoleAvailableId
            // 
            ColumnRoleAvailableId.DataPropertyName = "Id";
            ColumnRoleAvailableId.HeaderText = "ID";
            ColumnRoleAvailableId.Name = "ColumnRoleAvailableId";
            // 
            // ColumnRoleAvailableName
            // 
            ColumnRoleAvailableName.DataPropertyName = "Name";
            ColumnRoleAvailableName.HeaderText = "Name";
            ColumnRoleAvailableName.Name = "ColumnRoleAvailableName";
            ColumnRoleAvailableName.Width = 200;
            // 
            // gridRoleSelected
            // 
            gridRoleSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRoleSelected.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleSelectedId, ColumnRoleSelectedName });
            gridRoleSelected.Location = new Point(3, 74);
            gridRoleSelected.Name = "gridRoleSelected";
            gridRoleSelected.Size = new Size(256, 509);
            gridRoleSelected.TabIndex = 4;
            // 
            // ColumnRoleSelectedId
            // 
            ColumnRoleSelectedId.DataPropertyName = "Id";
            ColumnRoleSelectedId.HeaderText = "ID";
            ColumnRoleSelectedId.Name = "ColumnRoleSelectedId";
            // 
            // ColumnRoleSelectedName
            // 
            ColumnRoleSelectedName.DataPropertyName = "Name";
            ColumnRoleSelectedName.HeaderText = "Name";
            ColumnRoleSelectedName.Name = "ColumnRoleSelectedName";
            ColumnRoleSelectedName.Width = 200;
            // 
            // txtPermissionName
            // 
            txtPermissionName.Location = new Point(10, 38);
            txtPermissionName.Name = "txtPermissionName";
            txtPermissionName.Size = new Size(264, 31);
            txtPermissionName.TabIndex = 1;
            // 
            // btnAddPermission
            // 
            btnAddPermission.Location = new Point(280, 38);
            btnAddPermission.Name = "btnAddPermission";
            btnAddPermission.Size = new Size(89, 31);
            btnAddPermission.TabIndex = 2;
            btnAddPermission.Text = "追加";
            btnAddPermission.UseVisualStyleBackColor = true;
            btnAddPermission.Click += btnAddPermission_Click;
            // 
            // lblPermissionName
            // 
            lblPermissionName.AutoSize = true;
            lblPermissionName.Location = new Point(10, 11);
            lblPermissionName.Name = "lblPermissionName";
            lblPermissionName.Size = new Size(65, 24);
            lblPermissionName.TabIndex = 0;
            lblPermissionName.Text = "権限名:";
            // 
            // basicPermissionsTab
            // 
            basicPermissionsTab.Controls.Add(gridPermission);
            basicPermissionsTab.Controls.Add(btnAddPermission);
            basicPermissionsTab.Controls.Add(lblPermissionName);
            basicPermissionsTab.Controls.Add(txtPermissionName);
            basicPermissionsTab.Location = new Point(4, 33);
            basicPermissionsTab.Name = "basicPermissionsTab";
            basicPermissionsTab.Size = new Size(870, 635);
            basicPermissionsTab.TabIndex = 0;
            basicPermissionsTab.Text = "基本権限";
            // 
            // gridPermission
            // 
            gridPermission.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPermission.Columns.AddRange(new DataGridViewColumn[] { ColumnPermId, ColumnPermName, ColumnPermDeleteButton });
            gridPermission.Location = new Point(10, 75);
            gridPermission.Name = "gridPermission";
            gridPermission.Size = new Size(845, 541);
            gridPermission.TabIndex = 3;
            gridPermission.CellContentClick += gridPermission_CellContentClick;
            // 
            // ColumnPermId
            // 
            ColumnPermId.DataPropertyName = "Id";
            ColumnPermId.HeaderText = "ID";
            ColumnPermId.Name = "ColumnPermId";
            // 
            // ColumnPermName
            // 
            ColumnPermName.DataPropertyName = "Name";
            ColumnPermName.HeaderText = "Name";
            ColumnPermName.Name = "ColumnPermName";
            ColumnPermName.Width = 200;
            // 
            // ColumnPermDeleteButton
            // 
            ColumnPermDeleteButton.HeaderText = "Delete";
            ColumnPermDeleteButton.Name = "ColumnPermDeleteButton";
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(basicPermissionsTab);
            tabControl.Controls.Add(combinedPermissionsTab);
            tabControl.Controls.Add(userPermissionsTab);
            tabControl.Controls.Add(controlMappingTab);
            tabControl.Controls.Add(tabPage1);
            tabControl.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            tabControl.Location = new Point(15, 3);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(878, 672);
            tabControl.TabIndex = 4;
            // 
            // userPermissionsTab
            // 
            userPermissionsTab.Controls.Add(splitContainer3);
            userPermissionsTab.Location = new Point(4, 33);
            userPermissionsTab.Name = "userPermissionsTab";
            userPermissionsTab.Size = new Size(870, 635);
            userPermissionsTab.TabIndex = 2;
            userPermissionsTab.Text = "ユーザー権限";
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(panel5);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(gridUserAllPermissions);
            splitContainer3.Panel2.Controls.Add(lblTitleUserAllPermission);
            splitContainer3.Panel2.Controls.Add(tabControlUser);
            splitContainer3.Panel2.Controls.Add(lblSelectedUser);
            splitContainer3.Size = new Size(870, 635);
            splitContainer3.SplitterDistance = 271;
            splitContainer3.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Controls.Add(gridUser);
            panel5.Controls.Add(btnAddUserId);
            panel5.Controls.Add(lblUserId);
            panel5.Controls.Add(txtUserId);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(271, 635);
            panel5.TabIndex = 0;
            // 
            // gridUser
            // 
            gridUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUser.Columns.AddRange(new DataGridViewColumn[] { ColumnUserId, ColumnUserDeleteButton });
            gridUser.Location = new Point(6, 67);
            gridUser.Name = "gridUser";
            gridUser.Size = new Size(260, 561);
            gridUser.TabIndex = 4;
            gridUser.CellContentClick += gridUser_CellContentClick;
            gridUser.SelectionChanged += gridUser_SelectionChanged;
            // 
            // ColumnUserId
            // 
            ColumnUserId.DataPropertyName = "UserId";
            ColumnUserId.HeaderText = "UserId";
            ColumnUserId.Name = "ColumnUserId";
            ColumnUserId.Width = 200;
            // 
            // ColumnUserDeleteButton
            // 
            ColumnUserDeleteButton.HeaderText = "Delete";
            ColumnUserDeleteButton.Name = "ColumnUserDeleteButton";
            // 
            // btnAddUserId
            // 
            btnAddUserId.Location = new Point(186, 33);
            btnAddUserId.Name = "btnAddUserId";
            btnAddUserId.Size = new Size(80, 31);
            btnAddUserId.TabIndex = 2;
            btnAddUserId.Text = "追加";
            btnAddUserId.UseVisualStyleBackColor = true;
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Location = new Point(4, 6);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(99, 24);
            lblUserId.TabIndex = 0;
            lblUserId.Text = "ユーザーID:";
            // 
            // txtUserId
            // 
            txtUserId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUserId.Location = new Point(6, 33);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(178, 31);
            txtUserId.TabIndex = 1;
            // 
            // gridUserAllPermissions
            // 
            gridUserAllPermissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserAllPermissions.Columns.AddRange(new DataGridViewColumn[] { ColumnUserAllPermissionId, ColumnUserAllPermissionsName });
            gridUserAllPermissions.Location = new Point(3, 67);
            gridUserAllPermissions.Name = "gridUserAllPermissions";
            gridUserAllPermissions.Size = new Size(247, 561);
            gridUserAllPermissions.TabIndex = 2;
            // 
            // ColumnUserAllPermissionId
            // 
            ColumnUserAllPermissionId.HeaderText = "Id";
            ColumnUserAllPermissionId.Name = "ColumnUserAllPermissionId";
            // 
            // ColumnUserAllPermissionsName
            // 
            ColumnUserAllPermissionsName.DataPropertyName = "Name";
            ColumnUserAllPermissionsName.HeaderText = "Name";
            ColumnUserAllPermissionsName.Name = "ColumnUserAllPermissionsName";
            ColumnUserAllPermissionsName.Width = 200;
            // 
            // tabControlUser
            // 
            tabControlUser.Controls.Add(tabUserRoles);
            tabControlUser.Controls.Add(tabUserPermissions);
            tabControlUser.Location = new Point(256, 67);
            tabControlUser.Name = "tabControlUser";
            tabControlUser.SelectedIndex = 0;
            tabControlUser.Size = new Size(336, 565);
            tabControlUser.TabIndex = 0;
            // 
            // tabUserRoles
            // 
            tabUserRoles.Controls.Add(gridUserRoleAvailable);
            tabUserRoles.Controls.Add(btnSelectUserRole);
            tabUserRoles.Controls.Add(btnDeSelectUserRole);
            tabUserRoles.Controls.Add(gridUserRoleSelected);
            tabUserRoles.Location = new Point(4, 33);
            tabUserRoles.Name = "tabUserRoles";
            tabUserRoles.Size = new Size(328, 528);
            tabUserRoles.TabIndex = 0;
            tabUserRoles.Text = "ロール";
            // 
            // gridUserRoleAvailable
            // 
            gridUserRoleAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserRoleAvailable.Columns.AddRange(new DataGridViewColumn[] { ColumnUserRoleAvailableId, ColumnUserRoleAvailableName });
            gridUserRoleAvailable.Location = new Point(3, 288);
            gridUserRoleAvailable.Name = "gridUserRoleAvailable";
            gridUserRoleAvailable.Size = new Size(322, 233);
            gridUserRoleAvailable.TabIndex = 10;
            // 
            // ColumnUserRoleAvailableId
            // 
            ColumnUserRoleAvailableId.DataPropertyName = "Id";
            ColumnUserRoleAvailableId.HeaderText = "ID";
            ColumnUserRoleAvailableId.Name = "ColumnUserRoleAvailableId";
            // 
            // ColumnUserRoleAvailableName
            // 
            ColumnUserRoleAvailableName.DataPropertyName = "Name";
            ColumnUserRoleAvailableName.HeaderText = "Name";
            ColumnUserRoleAvailableName.Name = "ColumnUserRoleAvailableName";
            ColumnUserRoleAvailableName.Width = 200;
            // 
            // btnSelectUserRole
            // 
            btnSelectUserRole.Location = new Point(167, 242);
            btnSelectUserRole.Name = "btnSelectUserRole";
            btnSelectUserRole.Size = new Size(158, 40);
            btnSelectUserRole.TabIndex = 9;
            btnSelectUserRole.Text = "<< 追加";
            btnSelectUserRole.UseVisualStyleBackColor = true;
            btnSelectUserRole.Click += btnSelectUserRole_Click;
            // 
            // btnDeSelectUserRole
            // 
            btnDeSelectUserRole.Location = new Point(3, 242);
            btnDeSelectUserRole.Name = "btnDeSelectUserRole";
            btnDeSelectUserRole.Size = new Size(158, 40);
            btnDeSelectUserRole.TabIndex = 8;
            btnDeSelectUserRole.Text = "削除 >>";
            btnDeSelectUserRole.UseVisualStyleBackColor = true;
            btnDeSelectUserRole.Click += btnDeSelectUserRole_Click;
            // 
            // gridUserRoleSelected
            // 
            gridUserRoleSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserRoleSelected.Columns.AddRange(new DataGridViewColumn[] { ColumnUserRoleSelectedId, ColumnUserRoleSelectedName });
            gridUserRoleSelected.Location = new Point(3, 3);
            gridUserRoleSelected.Name = "gridUserRoleSelected";
            gridUserRoleSelected.Size = new Size(322, 233);
            gridUserRoleSelected.TabIndex = 3;
            // 
            // ColumnUserRoleSelectedId
            // 
            ColumnUserRoleSelectedId.DataPropertyName = "Id";
            ColumnUserRoleSelectedId.HeaderText = "ID";
            ColumnUserRoleSelectedId.Name = "ColumnUserRoleSelectedId";
            // 
            // ColumnUserRoleSelectedName
            // 
            ColumnUserRoleSelectedName.DataPropertyName = "Name";
            ColumnUserRoleSelectedName.HeaderText = "Name";
            ColumnUserRoleSelectedName.Name = "ColumnUserRoleSelectedName";
            ColumnUserRoleSelectedName.Width = 200;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.Control;
            tabPage1.Controls.Add(splitContainer4);
            tabPage1.Location = new Point(4, 33);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(870, 635);
            tabPage1.TabIndex = 4;
            tabPage1.Text = "コントロール権限";
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(3, 3);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(btnAddControl);
            splitContainer4.Panel1.Controls.Add(txtControlControlName);
            splitContainer4.Panel1.Controls.Add(lblTitleControlControlName);
            splitContainer4.Panel1.Controls.Add(txtControlFormName);
            splitContainer4.Panel1.Controls.Add(gridControl);
            splitContainer4.Panel1.Controls.Add(label3);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(tabControlControl);
            splitContainer4.Panel2.Controls.Add(lblTitleSelectedControl);
            splitContainer4.Size = new Size(864, 629);
            splitContainer4.SplitterDistance = 335;
            splitContainer4.TabIndex = 2;
            // 
            // btnAddControl
            // 
            btnAddControl.Location = new Point(244, 93);
            btnAddControl.Name = "btnAddControl";
            btnAddControl.Size = new Size(80, 31);
            btnAddControl.TabIndex = 7;
            btnAddControl.Text = "追加";
            btnAddControl.UseVisualStyleBackColor = true;
            btnAddControl.Click += btnAddControl_Click;
            // 
            // txtControlControlName
            // 
            txtControlControlName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtControlControlName.Location = new Point(3, 93);
            txtControlControlName.Name = "txtControlControlName";
            txtControlControlName.Size = new Size(235, 31);
            txtControlControlName.TabIndex = 6;
            // 
            // lblTitleControlControlName
            // 
            lblTitleControlControlName.AutoSize = true;
            lblTitleControlControlName.Location = new Point(3, 66);
            lblTitleControlControlName.Name = "lblTitleControlControlName";
            lblTitleControlControlName.Size = new Size(129, 24);
            lblTitleControlControlName.TabIndex = 5;
            lblTitleControlControlName.Text = "コントロール名:";
            // 
            // txtControlFormName
            // 
            txtControlFormName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtControlFormName.Location = new Point(3, 31);
            txtControlFormName.Name = "txtControlFormName";
            txtControlFormName.Size = new Size(235, 31);
            txtControlFormName.TabIndex = 4;
            // 
            // gridControl
            // 
            gridControl.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControl.Columns.AddRange(new DataGridViewColumn[] { ColumnControlFormName, ColumnControlControlName, ColumnControlDeleteButton });
            gridControl.Location = new Point(3, 140);
            gridControl.Name = "gridControl";
            gridControl.Size = new Size(324, 485);
            gridControl.TabIndex = 3;
            gridControl.SelectionChanged += gridControl_SelectionChanged;
            // 
            // ColumnControlFormName
            // 
            ColumnControlFormName.DataPropertyName = "FormName";
            ColumnControlFormName.HeaderText = "FormName";
            ColumnControlFormName.Name = "ColumnControlFormName";
            // 
            // ColumnControlControlName
            // 
            ColumnControlControlName.DataPropertyName = "ControlName";
            ColumnControlControlName.HeaderText = "ControlName";
            ColumnControlControlName.Name = "ColumnControlControlName";
            ColumnControlControlName.Width = 200;
            // 
            // ColumnControlDeleteButton
            // 
            ColumnControlDeleteButton.HeaderText = "";
            ColumnControlDeleteButton.Name = "ColumnControlDeleteButton";
            ColumnControlDeleteButton.Text = "削除";
            ColumnControlDeleteButton.UseColumnTextForButtonValue = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 4);
            label3.Name = "label3";
            label3.Size = new Size(97, 24);
            label3.TabIndex = 0;
            label3.Text = "フォーム名:";
            // 
            // tabControlControl
            // 
            tabControlControl.Controls.Add(tabPageControlRole);
            tabControlControl.Controls.Add(tabPageControlDept);
            tabControlControl.Location = new Point(3, 33);
            tabControlControl.Name = "tabControlControl";
            tabControlControl.SelectedIndex = 0;
            tabControlControl.Size = new Size(522, 593);
            tabControlControl.TabIndex = 8;
            // 
            // tabPageControlRole
            // 
            tabPageControlRole.Controls.Add(gridControlRole);
            tabPageControlRole.Location = new Point(4, 33);
            tabPageControlRole.Name = "tabPageControlRole";
            tabPageControlRole.Padding = new Padding(3);
            tabPageControlRole.Size = new Size(514, 556);
            tabPageControlRole.TabIndex = 0;
            tabPageControlRole.Text = "ロール";
            tabPageControlRole.UseVisualStyleBackColor = true;
            // 
            // gridControlRole
            // 
            gridControlRole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridControlRole.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControlRole.Columns.AddRange(new DataGridViewColumn[] { ColumnControlRoleId, ColumnControlRoleName, ColumnControlRoleVisible, ColumnControlRoleEnabled, ColumnControlRoleReadOnly });
            gridControlRole.Location = new Point(3, 3);
            gridControlRole.Name = "gridControlRole";
            gridControlRole.Size = new Size(508, 550);
            gridControlRole.TabIndex = 4;
            gridControlRole.CellClick += gridControlRole_CellClick;
            // 
            // ColumnControlRoleId
            // 
            ColumnControlRoleId.DataPropertyName = "Id";
            ColumnControlRoleId.HeaderText = "ID";
            ColumnControlRoleId.Name = "ColumnControlRoleId";
            // 
            // ColumnControlRoleName
            // 
            ColumnControlRoleName.DataPropertyName = "Name";
            ColumnControlRoleName.HeaderText = "Name";
            ColumnControlRoleName.Name = "ColumnControlRoleName";
            ColumnControlRoleName.Width = 200;
            // 
            // ColumnControlRoleVisible
            // 
            ColumnControlRoleVisible.HeaderText = "Visible";
            ColumnControlRoleVisible.Name = "ColumnControlRoleVisible";
            // 
            // ColumnControlRoleEnabled
            // 
            ColumnControlRoleEnabled.HeaderText = "Enabled";
            ColumnControlRoleEnabled.Name = "ColumnControlRoleEnabled";
            // 
            // ColumnControlRoleReadOnly
            // 
            ColumnControlRoleReadOnly.HeaderText = "ReadOnly";
            ColumnControlRoleReadOnly.Name = "ColumnControlRoleReadOnly";
            // 
            // tabPageControlDept
            // 
            tabPageControlDept.Controls.Add(gridControlDept);
            tabPageControlDept.Location = new Point(4, 33);
            tabPageControlDept.Name = "tabPageControlDept";
            tabPageControlDept.Padding = new Padding(3);
            tabPageControlDept.Size = new Size(514, 556);
            tabPageControlDept.TabIndex = 1;
            tabPageControlDept.Text = "部署";
            tabPageControlDept.UseVisualStyleBackColor = true;
            // 
            // gridControlDept
            // 
            gridControlDept.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridControlDept.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControlDept.Columns.AddRange(new DataGridViewColumn[] { ColumnControlDeptId, ColumnControlDeptName, ColumnControlDeptVisible, ColumnControlDeptEnabled, ColumnControlDeptReadOnly });
            gridControlDept.Location = new Point(3, 3);
            gridControlDept.Name = "gridControlDept";
            gridControlDept.Size = new Size(508, 550);
            gridControlDept.TabIndex = 5;
            gridControlDept.CellClick += gridControlDept_CellClick;
            // 
            // ColumnControlDeptId
            // 
            ColumnControlDeptId.DataPropertyName = "Id";
            ColumnControlDeptId.HeaderText = "ID";
            ColumnControlDeptId.Name = "ColumnControlDeptId";
            // 
            // ColumnControlDeptName
            // 
            ColumnControlDeptName.DataPropertyName = "Name";
            ColumnControlDeptName.HeaderText = "Name";
            ColumnControlDeptName.Name = "ColumnControlDeptName";
            ColumnControlDeptName.Width = 200;
            // 
            // ColumnControlDeptVisible
            // 
            ColumnControlDeptVisible.HeaderText = "Visible";
            ColumnControlDeptVisible.Name = "ColumnControlDeptVisible";
            // 
            // ColumnControlDeptEnabled
            // 
            ColumnControlDeptEnabled.HeaderText = "Enabled";
            ColumnControlDeptEnabled.Name = "ColumnControlDeptEnabled";
            // 
            // ColumnControlDeptReadOnly
            // 
            ColumnControlDeptReadOnly.HeaderText = "ReadOnly";
            ColumnControlDeptReadOnly.Name = "ColumnControlDeptReadOnly";
            // 
            // lblTitleSelectedControl
            // 
            lblTitleSelectedControl.AutoSize = true;
            lblTitleSelectedControl.Location = new Point(3, 6);
            lblTitleSelectedControl.Name = "lblTitleSelectedControl";
            lblTitleSelectedControl.Size = new Size(214, 24);
            lblTitleSelectedControl.TabIndex = 0;
            lblTitleSelectedControl.Text = "選択中のコントロール: なし";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Font = new Font("メイリオ", 14.25F);
            btnCancel.Location = new Point(15, 677);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(158, 49);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormPermission
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(905, 728);
            Controls.Add(tabControl);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Name = "FormPermission";
            Text = "Form1";
            tabUserPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsSelected).EndInit();
            controlMappingTab.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridDept).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridDeptPermAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridDeptPermSelected).EndInit();
            combinedPermissionsTab.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridRole).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRoleAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRoleSelected).EndInit();
            basicPermissionsTab.ResumeLayout(false);
            basicPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridPermission).EndInit();
            tabControl.ResumeLayout(false);
            userPermissionsTab.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridUser).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAllPermissions).EndInit();
            tabControlUser.ResumeLayout(false);
            tabUserRoles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUserRoleAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUserRoleSelected).EndInit();
            tabPage1.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel1.PerformLayout();
            splitContainer4.Panel2.ResumeLayout(false);
            splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            tabControlControl.ResumeLayout(false);
            tabPageControlRole.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControlRole).EndInit();
            tabPageControlDept.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControlDept).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnRemoveDeniedPerm;
        private ListView lstDeniedPermissions;
        private Button btnRemovePermFromUser;
        private ListView lstAdditionalPermissions;
        private ListView lstEffectivePermissions;
        private TabControl tabControlUserPermissions;
        private TabPage tabEffectivePermissions;
        private TabPage tabAdditionalPermissions;
        private TabPage tabDeniedPermissions;
        private TabPage tabUserPermissions;
        private SplitContainer splitContainer5;
        private ListView lstAvailableUserPerms;
        private TextBox txtUserPermFilter;
        private Label lblUserPermFilter;
        private Button btnDenyPermForUser;
        private Button btnAddPermToUser;
        private Button btnAddRoleToUser;
        private Button btnRemoveRoleFromUser;
        private ListView lstAssignedRoles;
        private ListView lstAvailableRoles;
        private Button btnSave;
        private Label lblTitleSelectedPermission;
        private DataGridView gridRole;
        private TabPage controlMappingTab;
        private TextBox txtUserRoleFilter;
        private Label lblTitleUserAllPermission;
        private Label lblSelectedUser;
        private Label lblSelectedRole;
        private Button btnAddRole;
        private Label lblRoleName;
        private TextBox txtRoleName;
        private TabPage combinedPermissionsTab;
        private SplitContainer splitContainer1;
        private TextBox txtPermissionName;
        private Button btnAddPermission;
        private Label lblPermissionName;
        private TabPage basicPermissionsTab;
        private TabControl tabControl;
        private TabPage userPermissionsTab;
        private SplitContainer splitContainer3;
        private Panel panel5;
        private ListView lstUsers;
        private ComboBox cmbDefaultRole;
        private Label lblDefaultRole;
        private Button btnAddUser;
        private Label lblUserId;
        private TextBox txtUserId;
        private TabControl tabControlUser;
        private TabPage tabUserRoles;
        private SplitContainer splitContainer4;
        private Button btnCancel;
        private DataGridView gridUserAllPermissions;
        private Button btnAddUserId;
        private DataGridView gridUserRoleSelected;
        private DataGridViewTextBoxColumn ColumnControlId;
        private DataGridViewTextBoxColumn ColumnControlName;
        private DataGridViewCheckBoxColumn ColumnVisivilityCheckbox;
        private DataGridViewCheckBoxColumn ColumnEnabledCheckbox;
        private DataGridViewCheckBoxColumn ColumnControlReadOnly;
        private DataGridView gridRoleAvailable;
        private DataGridView gridRoleSelected;
        private Button btnDeSelectRole;
        private Button btnSelectRole;
        private DataGridView gridUser;
        private Button btnSelectUserRole;
        private Button btnDeSelectUserRole;
        private DataGridView gridUserRoleAvailable;
        private DataGridView gridUserAdditionalPermissionsAvailable;
        private Button btnSelectUserAdditionalPermission;
        private Button btnDeSelectUserAdditionalPermission;
        private DataGridView gridUserAdditionalPermissionsSelected;
        private DataGridView gridPermission;
        private DataGridViewTextBoxColumn ColumnPermissionName;
        private DataGridViewTextBoxColumn ColumnPermId;
        private DataGridViewTextBoxColumn ColumnPermName;
        private DataGridViewButtonColumn ColumnPermDeleteButton;
        private DataGridViewTextBoxColumn ColumnUserId;
        private DataGridViewButtonColumn ColumnUserDeleteButton;
        private DataGridViewTextBoxColumn ColumnRoleId;
        private DataGridViewTextBoxColumn ColumnRoleName;
        private DataGridViewButtonColumn ColumnRoleDeleteButton;
        private DataGridViewTextBoxColumn ColumnRoleAvailableId;
        private DataGridViewTextBoxColumn ColumnRoleAvailableName;
        private DataGridViewTextBoxColumn ColumnRoleSelectedId;
        private DataGridViewTextBoxColumn ColumnRoleSelectedName;
        private DataGridViewTextBoxColumn ColumnUserAllPermissionId;
        private DataGridViewTextBoxColumn ColumnUserAllPermissionsName;
        private DataGridViewTextBoxColumn ColumnUserRoleAvailableId;
        private DataGridViewTextBoxColumn ColumnUserRoleAvailableName;
        private DataGridViewTextBoxColumn ColumnUserRoleSelectedId;
        private DataGridViewTextBoxColumn ColumnUserRoleSelectedName;
        private DataGridViewTextBoxColumn ColumnUserAdditionalPermissionSelectedId;
        private DataGridViewTextBoxColumn ColumnUserAdditionalPermissionSelectedName;
        private DataGridViewTextBoxColumn ColumnUserAdditionalPermissionAvailableId;
        private DataGridViewTextBoxColumn ColumnUserAdditionalPermissionAvailableName;
        private TabPage tabPage1;
        private SplitContainer splitContainer2;
        private DataGridView gridDept;
        private Label lblTitleDepartment;
        private Button btnSelectDeptPerm;
        private Button btnDeSelectDeptPerm;
        private DataGridView gridDeptPermAvailable;
        private DataGridView gridDeptPermSelected;
        private Label lblTitleSelectedDept;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridView gridControl;
        private Label label3;
        private TabControl tabControlControl;
        private TabPage tabPageControlRole;
        private DataGridView gridControlRole;
        private TabPage tabPageControlDept;
        private DataGridView gridControlDept;
        private Label lblTitleSelectedControl;
        private DataGridViewTextBoxColumn ColumnControlRoleId;
        private DataGridViewTextBoxColumn ColumnControlRoleName;
        private DataGridViewCheckBoxColumn ColumnControlRoleVisible;
        private DataGridViewCheckBoxColumn ColumnControlRoleEnabled;
        private DataGridViewCheckBoxColumn ColumnControlRoleReadOnly;
        private DataGridViewTextBoxColumn ColumnControlDeptId;
        private DataGridViewTextBoxColumn ColumnControlDeptName;
        private DataGridViewCheckBoxColumn ColumnControlDeptVisible;
        private DataGridViewCheckBoxColumn ColumnControlDeptEnabled;
        private DataGridViewCheckBoxColumn ColumnControlDeptReadOnly;
        private DataGridViewTextBoxColumn ColumnDeptPermIdAvailable;
        private DataGridViewTextBoxColumn ColumnDeptPermNameAvailable;
        private DataGridViewTextBoxColumn ColumnDeptPermIdSelected;
        private DataGridViewTextBoxColumn ColumnDeptPermNameSelected;
        private DataGridViewTextBoxColumn ColumnControlFormName;
        private DataGridViewTextBoxColumn ColumnControlControlName;
        private DataGridViewButtonColumn ColumnControlDeleteButton;
        private Button btnAddControl;
        private TextBox txtControlControlName;
        private Label lblTitleControlControlName;
        private TextBox txtControlFormName;
    }
}