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
            lblFormName = new Label();
            lblControlName = new Label();
            txtControlName = new TextBox();
            lblPermission = new Label();
            cmbPermission = new ComboBox();
            lblRestrictType = new Label();
            cmbRestrictType = new ComboBox();
            btnAddMapping = new Button();
            gridControlMapping = new DataGridView();
            columnControlName = new DataGridViewTextBoxColumn();
            columnPermissionName = new DataGridViewTextBoxColumn();
            columnRestrictType = new DataGridViewTextBoxColumn();
            columnDelete = new DataGridViewButtonColumn();
            lblTitleUserAllPermission = new Label();
            lblSelectedUser = new Label();
            lblSelectedRole = new Label();
            btnAddRole = new Button();
            lblRoleName = new Label();
            txtRoleName = new TextBox();
            combinedPermissionsTab = new TabPage();
            splitContainer1 = new SplitContainer();
            btnRoleUnique = new Button();
            lblTitleRoleId = new Label();
            txtRoleId = new TextBox();
            btnRoleUser = new Button();
            btnRoleManager = new Button();
            btnRoleAdmin = new Button();
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
            txtPermissionId = new TextBox();
            lblTitlePermissionId = new Label();
            btnPermissionCategoryUnique = new Button();
            btnPermissionCategoryDelete = new Button();
            btnPermissionCategoryEdit = new Button();
            btnPermissionCategoryCreate = new Button();
            btnPermissionCategoryView = new Button();
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
            btnCancel = new Button();
            txtFormName = new TextBox();
            tabUserPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsSelected).BeginInit();
            controlMappingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlMapping).BeginInit();
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
            gridUserAdditionalPermissionsAvailable.SelectionChanged += gridUserAdditionalPermissionsAvailable_SelectionChanged;
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
            gridUserAdditionalPermissionsSelected.SelectionChanged += gridUserAdditionalPermissionsSelected_SelectionChanged;
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
            btnSave.Click += btnSave_Click;
            // 
            // controlMappingTab
            // 
            controlMappingTab.BackColor = SystemColors.Control;
            controlMappingTab.Controls.Add(txtFormName);
            controlMappingTab.Controls.Add(lblFormName);
            controlMappingTab.Controls.Add(lblControlName);
            controlMappingTab.Controls.Add(txtControlName);
            controlMappingTab.Controls.Add(lblPermission);
            controlMappingTab.Controls.Add(cmbPermission);
            controlMappingTab.Controls.Add(lblRestrictType);
            controlMappingTab.Controls.Add(cmbRestrictType);
            controlMappingTab.Controls.Add(btnAddMapping);
            controlMappingTab.Controls.Add(gridControlMapping);
            controlMappingTab.Location = new Point(4, 33);
            controlMappingTab.Name = "controlMappingTab";
            controlMappingTab.Padding = new Padding(3);
            controlMappingTab.Size = new Size(870, 635);
            controlMappingTab.TabIndex = 3;
            controlMappingTab.Text = "コントロール権限マッピング";
            // 
            // lblFormName
            // 
            lblFormName.AutoSize = true;
            lblFormName.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblFormName.Location = new Point(20, 20);
            lblFormName.Name = "lblFormName";
            lblFormName.Size = new Size(97, 24);
            lblFormName.TabIndex = 0;
            lblFormName.Text = "フォーム名:";
            // 
            // lblControlName
            // 
            lblControlName.AutoSize = true;
            lblControlName.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblControlName.Location = new Point(206, 21);
            lblControlName.Name = "lblControlName";
            lblControlName.Size = new Size(129, 24);
            lblControlName.TabIndex = 2;
            lblControlName.Text = "コントロール名:";
            // 
            // txtControlName
            // 
            txtControlName.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtControlName.Location = new Point(206, 51);
            txtControlName.Name = "txtControlName";
            txtControlName.Size = new Size(180, 31);
            txtControlName.TabIndex = 3;
            // 
            // lblPermission
            // 
            lblPermission.AutoSize = true;
            lblPermission.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblPermission.Location = new Point(392, 21);
            lblPermission.Name = "lblPermission";
            lblPermission.Size = new Size(65, 24);
            lblPermission.TabIndex = 5;
            lblPermission.Text = "権限名:";
            // 
            // cmbPermission
            // 
            cmbPermission.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPermission.FlatStyle = FlatStyle.Flat;
            cmbPermission.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            cmbPermission.Location = new Point(392, 51);
            cmbPermission.Name = "cmbPermission";
            cmbPermission.Size = new Size(180, 32);
            cmbPermission.TabIndex = 6;
            // 
            // lblRestrictType
            // 
            lblRestrictType.AutoSize = true;
            lblRestrictType.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblRestrictType.Location = new Point(578, 21);
            lblRestrictType.Name = "lblRestrictType";
            lblRestrictType.Size = new Size(97, 24);
            lblRestrictType.TabIndex = 7;
            lblRestrictType.Text = "制限タイプ:";
            // 
            // cmbRestrictType
            // 
            cmbRestrictType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRestrictType.FlatStyle = FlatStyle.Flat;
            cmbRestrictType.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            cmbRestrictType.Location = new Point(578, 51);
            cmbRestrictType.Name = "cmbRestrictType";
            cmbRestrictType.Size = new Size(180, 32);
            cmbRestrictType.TabIndex = 8;
            // 
            // btnAddMapping
            // 
            btnAddMapping.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnAddMapping.Location = new Point(20, 149);
            btnAddMapping.Name = "btnAddMapping";
            btnAddMapping.Size = new Size(180, 40);
            btnAddMapping.TabIndex = 9;
            btnAddMapping.Text = "マッピング追加";
            btnAddMapping.UseVisualStyleBackColor = true;
            btnAddMapping.Click += btnAddMapping_Click;
            // 
            // gridControlMapping
            // 
            gridControlMapping.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridControlMapping.Columns.AddRange(new DataGridViewColumn[] { columnControlName, columnPermissionName, columnRestrictType, columnDelete });
            gridControlMapping.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            gridControlMapping.Location = new Point(6, 195);
            gridControlMapping.Name = "gridControlMapping";
            gridControlMapping.Size = new Size(858, 434);
            gridControlMapping.TabIndex = 10;
            gridControlMapping.CellContentClick += gridControlMapping_CellContentClick;
            // 
            // columnControlName
            // 
            columnControlName.HeaderText = "コントロール名";
            columnControlName.Name = "columnControlName";
            columnControlName.Width = 250;
            // 
            // columnPermissionName
            // 
            columnPermissionName.HeaderText = "権限名";
            columnPermissionName.Name = "columnPermissionName";
            columnPermissionName.Width = 200;
            // 
            // columnRestrictType
            // 
            columnRestrictType.HeaderText = "制限タイプ";
            columnRestrictType.Name = "columnRestrictType";
            columnRestrictType.Width = 150;
            // 
            // columnDelete
            // 
            columnDelete.HeaderText = "操作";
            columnDelete.Name = "columnDelete";
            columnDelete.Text = "削除";
            columnDelete.UseColumnTextForButtonValue = true;
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
            btnAddRole.Location = new Point(258, 132);
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
            lblRoleName.Location = new Point(8, 105);
            lblRoleName.Name = "lblRoleName";
            lblRoleName.Size = new Size(81, 24);
            lblRoleName.TabIndex = 0;
            lblRoleName.Text = "ロール名:";
            // 
            // txtRoleName
            // 
            txtRoleName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRoleName.Location = new Point(8, 132);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.Size = new Size(248, 31);
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
            splitContainer1.Panel1.Controls.Add(btnRoleUnique);
            splitContainer1.Panel1.Controls.Add(lblTitleRoleId);
            splitContainer1.Panel1.Controls.Add(txtRoleId);
            splitContainer1.Panel1.Controls.Add(btnRoleUser);
            splitContainer1.Panel1.Controls.Add(btnRoleManager);
            splitContainer1.Panel1.Controls.Add(btnRoleAdmin);
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
            // btnRoleUnique
            // 
            btnRoleUnique.Location = new Point(245, 33);
            btnRoleUnique.Name = "btnRoleUnique";
            btnRoleUnique.Size = new Size(75, 32);
            btnRoleUnique.TabIndex = 17;
            btnRoleUnique.Text = "unique";
            btnRoleUnique.UseVisualStyleBackColor = true;
            btnRoleUnique.Click += btnRoleUnique_Click;
            // 
            // lblTitleRoleId
            // 
            lblTitleRoleId.AutoSize = true;
            lblTitleRoleId.Location = new Point(8, 6);
            lblTitleRoleId.Name = "lblTitleRoleId";
            lblTitleRoleId.Size = new Size(83, 24);
            lblTitleRoleId.TabIndex = 16;
            lblTitleRoleId.Text = "ロールID:";
            // 
            // txtRoleId
            // 
            txtRoleId.Location = new Point(8, 71);
            txtRoleId.Name = "txtRoleId";
            txtRoleId.Size = new Size(324, 31);
            txtRoleId.TabIndex = 15;
            // 
            // btnRoleUser
            // 
            btnRoleUser.Location = new Point(164, 33);
            btnRoleUser.Name = "btnRoleUser";
            btnRoleUser.Size = new Size(75, 32);
            btnRoleUser.TabIndex = 14;
            btnRoleUser.Text = "user";
            btnRoleUser.UseVisualStyleBackColor = true;
            btnRoleUser.Click += btnRoleUser_Click;
            // 
            // btnRoleManager
            // 
            btnRoleManager.Location = new Point(86, 33);
            btnRoleManager.Name = "btnRoleManager";
            btnRoleManager.Size = new Size(75, 32);
            btnRoleManager.TabIndex = 13;
            btnRoleManager.Text = "manager";
            btnRoleManager.UseVisualStyleBackColor = true;
            btnRoleManager.Click += btnRoleManager_Click;
            // 
            // btnRoleAdmin
            // 
            btnRoleAdmin.Location = new Point(8, 33);
            btnRoleAdmin.Name = "btnRoleAdmin";
            btnRoleAdmin.Size = new Size(75, 32);
            btnRoleAdmin.TabIndex = 12;
            btnRoleAdmin.Text = "admin";
            btnRoleAdmin.UseVisualStyleBackColor = true;
            btnRoleAdmin.Click += btnRoleAdmin_Click;
            // 
            // gridRole
            // 
            gridRole.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRole.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleId, ColumnRoleName, ColumnRoleDeleteButton });
            gridRole.Location = new Point(8, 170);
            gridRole.Name = "gridRole";
            gridRole.Size = new Size(324, 459);
            gridRole.TabIndex = 3;
            gridRole.CellContentClick += gridRole_CellContentClick;
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
            gridRoleAvailable.SelectionChanged += gridRoleAvailable_SelectionChanged;
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
            gridRoleSelected.SelectionChanged += gridRoleSelected_SelectionChanged;
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
            txtPermissionName.Location = new Point(10, 175);
            txtPermissionName.Name = "txtPermissionName";
            txtPermissionName.Size = new Size(386, 31);
            txtPermissionName.TabIndex = 1;
            // 
            // btnAddPermission
            // 
            btnAddPermission.Location = new Point(10, 237);
            btnAddPermission.Name = "btnAddPermission";
            btnAddPermission.Size = new Size(386, 55);
            btnAddPermission.TabIndex = 2;
            btnAddPermission.Text = "追加";
            btnAddPermission.UseVisualStyleBackColor = true;
            btnAddPermission.Click += btnAddPermission_Click;
            // 
            // lblPermissionName
            // 
            lblPermissionName.AutoSize = true;
            lblPermissionName.Location = new Point(10, 148);
            lblPermissionName.Name = "lblPermissionName";
            lblPermissionName.Size = new Size(65, 24);
            lblPermissionName.TabIndex = 0;
            lblPermissionName.Text = "権限名:";
            // 
            // basicPermissionsTab
            // 
            basicPermissionsTab.Controls.Add(txtPermissionId);
            basicPermissionsTab.Controls.Add(lblTitlePermissionId);
            basicPermissionsTab.Controls.Add(btnPermissionCategoryUnique);
            basicPermissionsTab.Controls.Add(btnPermissionCategoryDelete);
            basicPermissionsTab.Controls.Add(btnPermissionCategoryEdit);
            basicPermissionsTab.Controls.Add(btnPermissionCategoryCreate);
            basicPermissionsTab.Controls.Add(btnPermissionCategoryView);
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
            // txtPermissionId
            // 
            txtPermissionId.Location = new Point(10, 86);
            txtPermissionId.Name = "txtPermissionId";
            txtPermissionId.Size = new Size(386, 31);
            txtPermissionId.TabIndex = 11;
            // 
            // lblTitlePermissionId
            // 
            lblTitlePermissionId.AutoSize = true;
            lblTitlePermissionId.Location = new Point(10, 18);
            lblTitlePermissionId.Name = "lblTitlePermissionId";
            lblTitlePermissionId.Size = new Size(67, 24);
            lblTitlePermissionId.TabIndex = 10;
            lblTitlePermissionId.Text = "権限ID:";
            // 
            // btnPermissionCategoryUnique
            // 
            btnPermissionCategoryUnique.Location = new Point(322, 48);
            btnPermissionCategoryUnique.Name = "btnPermissionCategoryUnique";
            btnPermissionCategoryUnique.Size = new Size(75, 32);
            btnPermissionCategoryUnique.TabIndex = 9;
            btnPermissionCategoryUnique.Text = "unique";
            btnPermissionCategoryUnique.UseVisualStyleBackColor = true;
            btnPermissionCategoryUnique.Click += btnPermissionCategoryUnique_Click;
            // 
            // btnPermissionCategoryDelete
            // 
            btnPermissionCategoryDelete.Location = new Point(244, 48);
            btnPermissionCategoryDelete.Name = "btnPermissionCategoryDelete";
            btnPermissionCategoryDelete.Size = new Size(75, 32);
            btnPermissionCategoryDelete.TabIndex = 7;
            btnPermissionCategoryDelete.Text = "delete";
            btnPermissionCategoryDelete.UseVisualStyleBackColor = true;
            btnPermissionCategoryDelete.Click += btnPermissionCategoryDelete_Click;
            // 
            // btnPermissionCategoryEdit
            // 
            btnPermissionCategoryEdit.Location = new Point(166, 48);
            btnPermissionCategoryEdit.Name = "btnPermissionCategoryEdit";
            btnPermissionCategoryEdit.Size = new Size(75, 32);
            btnPermissionCategoryEdit.TabIndex = 6;
            btnPermissionCategoryEdit.Text = "edit";
            btnPermissionCategoryEdit.UseVisualStyleBackColor = true;
            btnPermissionCategoryEdit.Click += btnPermissionCategoryEdit_Click;
            // 
            // btnPermissionCategoryCreate
            // 
            btnPermissionCategoryCreate.Location = new Point(88, 48);
            btnPermissionCategoryCreate.Name = "btnPermissionCategoryCreate";
            btnPermissionCategoryCreate.Size = new Size(75, 32);
            btnPermissionCategoryCreate.TabIndex = 5;
            btnPermissionCategoryCreate.Text = "create";
            btnPermissionCategoryCreate.UseVisualStyleBackColor = true;
            btnPermissionCategoryCreate.Click += btnPermissionCategoryCreate_Click;
            // 
            // btnPermissionCategoryView
            // 
            btnPermissionCategoryView.Location = new Point(10, 48);
            btnPermissionCategoryView.Name = "btnPermissionCategoryView";
            btnPermissionCategoryView.Size = new Size(75, 32);
            btnPermissionCategoryView.TabIndex = 4;
            btnPermissionCategoryView.Text = "view";
            btnPermissionCategoryView.UseVisualStyleBackColor = true;
            btnPermissionCategoryView.Click += btnPermissionCategoryView_Click;
            // 
            // gridPermission
            // 
            gridPermission.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPermission.Columns.AddRange(new DataGridViewColumn[] { ColumnPermId, ColumnPermName, ColumnPermDeleteButton });
            gridPermission.Location = new Point(402, 18);
            gridPermission.Name = "gridPermission";
            gridPermission.Size = new Size(453, 598);
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
            btnAddUserId.Click += btnAddUserId_Click;
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
            gridUserRoleAvailable.SelectionChanged += gridUserRoleAvailable_SelectionChanged;
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
            gridUserRoleSelected.SelectionChanged += gridUserRoleSelected_SelectionChanged;
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
            btnCancel.Click += btnCancel_Click;
            // 
            // txtFormName
            // 
            txtFormName.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtFormName.Location = new Point(20, 52);
            txtFormName.Name = "txtFormName";
            txtFormName.Size = new Size(180, 31);
            txtFormName.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(905, 728);
            Controls.Add(tabControl);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Name = "Form1";
            Text = "Form1";
            tabUserPermissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUserAdditionalPermissionsSelected).EndInit();
            controlMappingTab.ResumeLayout(false);
            controlMappingTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridControlMapping).EndInit();
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
        private Label lblFormName;
        private TextBox txtControlName;
        private Label lblControlName;
        private ComboBox cmbPermission;
        private Label lblPermission;
        private ComboBox cmbRestrictType;
        private Label lblRestrictType;
        private DataGridView gridControlMapping;
        private DataGridViewTextBoxColumn columnControlName;
        private DataGridViewTextBoxColumn columnPermissionName;
        private DataGridViewTextBoxColumn columnRestrictType;
        private DataGridViewButtonColumn columnDelete;
        private Button btnAddMapping;
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
        private Button btnPermissionCategoryDelete;
        private Button btnPermissionCategoryEdit;
        private Button btnPermissionCategoryCreate;
        private Button btnPermissionCategoryView;
        private Button btnPermissionCategoryUnique;
        private Label lblTitlePermissionId;
        private TextBox txtPermissionId;
        private TextBox txtRoleId;
        private Button btnRoleUser;
        private Button btnRoleManager;
        private Button btnRoleAdmin;
        private Label lblTitleRoleId;
        private Button btnRoleUnique;
        private TextBox txtFormName;
    }
}