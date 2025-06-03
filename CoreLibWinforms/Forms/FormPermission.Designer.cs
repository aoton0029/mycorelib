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
            tabControl = new TabControl();
            basicPermissionsTab = new TabPage();
            lstPermissions = new ListView();
            btnAddPermission = new Button();
            lblPermissionName = new Label();
            txtPermissionName = new TextBox();
            combinedPermissionsTab = new TabPage();
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            lstRoles = new ListView();
            btnAddRole = new Button();
            lblRoleName = new Label();
            txtRoleName = new TextBox();
            splitContainer2 = new SplitContainer();
            panel2 = new Panel();
            lstAvailablePermissions = new ListView();
            txtRolePermFilter = new TextBox();
            lblRolePermFilter = new Label();
            lblSelectedRole = new Label();
            panel3 = new Panel();
            lstAssignedPermissions = new ListView();
            panel4 = new Panel();
            btnRemovePermFromRole = new Button();
            btnAddPermToRole = new Button();
            userPermissionsTab = new TabPage();
            splitContainer3 = new SplitContainer();
            panel5 = new Panel();
            lstUsers = new ListView();
            cmbDefaultRole = new ComboBox();
            lblDefaultRole = new Label();
            btnAddUser = new Button();
            lblUserId = new Label();
            txtUserId = new TextBox();
            tabControlUser = new TabControl();
            tabUserRoles = new TabPage();
            splitContainer4 = new SplitContainer();
            panel6 = new Panel();
            lstAvailableRoles = new ListView();
            txtUserRoleFilter = new TextBox();
            lblUserRoleFilter = new Label();
            lblSelectedUser = new Label();
            panel7 = new Panel();
            lstAssignedRoles = new ListView();
            panel8 = new Panel();
            btnRemoveRoleFromUser = new Button();
            btnAddRoleToUser = new Button();
            tabUserPermissions = new TabPage();
            splitContainer5 = new SplitContainer();
            tabControlUserPermissions = new TabControl();
            tabEffectivePermissions = new TabPage();
            lstEffectivePermissions = new ListView();
            tabAdditionalPermissions = new TabPage();
            lstAdditionalPermissions = new ListView();
            panel9 = new Panel();
            btnRemovePermFromUser = new Button();
            tabDeniedPermissions = new TabPage();
            lstDeniedPermissions = new ListView();
            panel10 = new Panel();
            btnRemoveDeniedPerm = new Button();
            panel11 = new Panel();
            lstAvailableUserPerms = new ListView();
            txtUserPermFilter = new TextBox();
            lblUserPermFilter = new Label();
            btnDenyPermForUser = new Button();
            btnAddPermToUser = new Button();
            btnCancel = new Button();
            btnSave = new Button();
            tabControl.SuspendLayout();
            basicPermissionsTab.SuspendLayout();
            combinedPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            userPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            panel5.SuspendLayout();
            tabControlUser.SuspendLayout();
            tabUserRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            panel8.SuspendLayout();
            tabUserPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer5).BeginInit();
            splitContainer5.Panel1.SuspendLayout();
            splitContainer5.Panel2.SuspendLayout();
            splitContainer5.SuspendLayout();
            tabControlUserPermissions.SuspendLayout();
            tabEffectivePermissions.SuspendLayout();
            tabAdditionalPermissions.SuspendLayout();
            panel9.SuspendLayout();
            tabDeniedPermissions.SuspendLayout();
            panel10.SuspendLayout();
            panel11.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(basicPermissionsTab);
            tabControl.Controls.Add(combinedPermissionsTab);
            tabControl.Controls.Add(userPermissionsTab);
            tabControl.Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            tabControl.Location = new Point(4, 2);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(894, 672);
            tabControl.TabIndex = 1;
            // 
            // basicPermissionsTab
            // 
            basicPermissionsTab.Controls.Add(lstPermissions);
            basicPermissionsTab.Controls.Add(btnAddPermission);
            basicPermissionsTab.Controls.Add(lblPermissionName);
            basicPermissionsTab.Controls.Add(txtPermissionName);
            basicPermissionsTab.Location = new Point(4, 33);
            basicPermissionsTab.Name = "basicPermissionsTab";
            basicPermissionsTab.Size = new Size(886, 635);
            basicPermissionsTab.TabIndex = 0;
            basicPermissionsTab.Text = "基本権限";
            // 
            // lstPermissions
            // 
            lstPermissions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstPermissions.FullRowSelect = true;
            lstPermissions.Location = new Point(20, 90);
            lstPermissions.Name = "lstPermissions";
            lstPermissions.Size = new Size(846, 525);
            lstPermissions.TabIndex = 3;
            lstPermissions.UseCompatibleStateImageBehavior = false;
            lstPermissions.View = View.Details;
            // 
            // btnAddPermission
            // 
            btnAddPermission.Location = new Point(464, 39);
            btnAddPermission.Name = "btnAddPermission";
            btnAddPermission.Size = new Size(94, 32);
            btnAddPermission.TabIndex = 2;
            btnAddPermission.Text = "追加";
            btnAddPermission.UseVisualStyleBackColor = true;
            // 
            // lblPermissionName
            // 
            lblPermissionName.AutoSize = true;
            lblPermissionName.Location = new Point(20, 15);
            lblPermissionName.Name = "lblPermissionName";
            lblPermissionName.Size = new Size(65, 24);
            lblPermissionName.TabIndex = 0;
            lblPermissionName.Text = "権限名:";
            // 
            // txtPermissionName
            // 
            txtPermissionName.Location = new Point(20, 42);
            txtPermissionName.Name = "txtPermissionName";
            txtPermissionName.Size = new Size(425, 31);
            txtPermissionName.TabIndex = 1;
            // 
            // combinedPermissionsTab
            // 
            combinedPermissionsTab.Controls.Add(splitContainer1);
            combinedPermissionsTab.Location = new Point(4, 33);
            combinedPermissionsTab.Name = "combinedPermissionsTab";
            combinedPermissionsTab.Size = new Size(886, 635);
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
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(886, 635);
            splitContainer1.SplitterDistance = 275;
            splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(lstRoles);
            panel1.Controls.Add(btnAddRole);
            panel1.Controls.Add(lblRoleName);
            panel1.Controls.Add(txtRoleName);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(275, 635);
            panel1.TabIndex = 0;
            // 
            // lstRoles
            // 
            lstRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstRoles.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstRoles.FullRowSelect = true;
            lstRoles.Location = new Point(14, 90);
            lstRoles.Name = "lstRoles";
            lstRoles.Size = new Size(247, 529);
            lstRoles.TabIndex = 3;
            lstRoles.UseCompatibleStateImageBehavior = false;
            lstRoles.View = View.Details;
            // 
            // btnAddRole
            // 
            btnAddRole.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddRole.Location = new Point(167, 42);
            btnAddRole.Name = "btnAddRole";
            btnAddRole.Size = new Size(94, 32);
            btnAddRole.TabIndex = 2;
            btnAddRole.Text = "追加";
            btnAddRole.UseVisualStyleBackColor = true;
            // 
            // lblRoleName
            // 
            lblRoleName.AutoSize = true;
            lblRoleName.Location = new Point(14, 15);
            lblRoleName.Name = "lblRoleName";
            lblRoleName.Size = new Size(81, 24);
            lblRoleName.TabIndex = 0;
            lblRoleName.Text = "ロール名:";
            // 
            // txtRoleName
            // 
            txtRoleName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRoleName.Location = new Point(14, 42);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.Size = new Size(147, 31);
            txtRoleName.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(panel2);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(panel3);
            splitContainer2.Panel2.Controls.Add(panel4);
            splitContainer2.Size = new Size(607, 635);
            splitContainer2.SplitterDistance = 302;
            splitContainer2.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(lstAvailablePermissions);
            panel2.Controls.Add(txtRolePermFilter);
            panel2.Controls.Add(lblRolePermFilter);
            panel2.Controls.Add(lblSelectedRole);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(302, 635);
            panel2.TabIndex = 0;
            // 
            // lstAvailablePermissions
            // 
            lstAvailablePermissions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstAvailablePermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAvailablePermissions.FullRowSelect = true;
            lstAvailablePermissions.Location = new Point(16, 90);
            lstAvailablePermissions.Name = "lstAvailablePermissions";
            lstAvailablePermissions.Size = new Size(272, 529);
            lstAvailablePermissions.TabIndex = 3;
            lstAvailablePermissions.UseCompatibleStateImageBehavior = false;
            lstAvailablePermissions.View = View.Details;
            // 
            // txtRolePermFilter
            // 
            txtRolePermFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRolePermFilter.Location = new Point(87, 53);
            txtRolePermFilter.Name = "txtRolePermFilter";
            txtRolePermFilter.Size = new Size(201, 31);
            txtRolePermFilter.TabIndex = 2;
            // 
            // lblRolePermFilter
            // 
            lblRolePermFilter.AutoSize = true;
            lblRolePermFilter.Location = new Point(16, 56);
            lblRolePermFilter.Name = "lblRolePermFilter";
            lblRolePermFilter.Size = new Size(81, 24);
            lblRolePermFilter.TabIndex = 1;
            lblRolePermFilter.Text = "フィルタ:";
            // 
            // lblSelectedRole
            // 
            lblSelectedRole.AutoSize = true;
            lblSelectedRole.Location = new Point(16, 16);
            lblSelectedRole.Name = "lblSelectedRole";
            lblSelectedRole.Size = new Size(166, 24);
            lblSelectedRole.TabIndex = 0;
            lblSelectedRole.Text = "選択中のロール: なし";
            // 
            // panel3
            // 
            panel3.Controls.Add(lstAssignedPermissions);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(301, 635);
            panel3.TabIndex = 1;
            // 
            // lstAssignedPermissions
            // 
            lstAssignedPermissions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstAssignedPermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAssignedPermissions.FullRowSelect = true;
            lstAssignedPermissions.Location = new Point(18, 90);
            lstAssignedPermissions.Name = "lstAssignedPermissions";
            lstAssignedPermissions.Size = new Size(271, 529);
            lstAssignedPermissions.TabIndex = 1;
            lstAssignedPermissions.UseCompatibleStateImageBehavior = false;
            lstAssignedPermissions.View = View.Details;
            // 
            // panel4
            // 
            panel4.Controls.Add(btnRemovePermFromRole);
            panel4.Controls.Add(btnAddPermToRole);
            panel4.Location = new Point(3, 16);
            panel4.Name = "panel4";
            panel4.Size = new Size(295, 67);
            panel4.TabIndex = 0;
            // 
            // btnRemovePermFromRole
            // 
            btnRemovePermFromRole.Font = new Font("メイリオ", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            btnRemovePermFromRole.Location = new Point(15, 37);
            btnRemovePermFromRole.Name = "btnRemovePermFromRole";
            btnRemovePermFromRole.Size = new Size(50, 30);
            btnRemovePermFromRole.TabIndex = 1;
            btnRemovePermFromRole.Text = "<<";
            btnRemovePermFromRole.UseVisualStyleBackColor = true;
            // 
            // btnAddPermToRole
            // 
            btnAddPermToRole.Font = new Font("メイリオ", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            btnAddPermToRole.Location = new Point(15, 3);
            btnAddPermToRole.Name = "btnAddPermToRole";
            btnAddPermToRole.Size = new Size(50, 30);
            btnAddPermToRole.TabIndex = 0;
            btnAddPermToRole.Text = ">>";
            btnAddPermToRole.UseVisualStyleBackColor = true;
            // 
            // userPermissionsTab
            // 
            userPermissionsTab.Controls.Add(splitContainer3);
            userPermissionsTab.Location = new Point(4, 33);
            userPermissionsTab.Name = "userPermissionsTab";
            userPermissionsTab.Size = new Size(886, 635);
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
            splitContainer3.Panel2.Controls.Add(tabControlUser);
            splitContainer3.Size = new Size(886, 635);
            splitContainer3.SplitterDistance = 277;
            splitContainer3.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Controls.Add(lstUsers);
            panel5.Controls.Add(cmbDefaultRole);
            panel5.Controls.Add(lblDefaultRole);
            panel5.Controls.Add(btnAddUser);
            panel5.Controls.Add(lblUserId);
            panel5.Controls.Add(txtUserId);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(277, 635);
            panel5.TabIndex = 0;
            // 
            // lstUsers
            // 
            lstUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstUsers.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstUsers.FullRowSelect = true;
            lstUsers.Location = new Point(14, 152);
            lstUsers.Name = "lstUsers";
            lstUsers.Size = new Size(249, 467);
            lstUsers.TabIndex = 5;
            lstUsers.UseCompatibleStateImageBehavior = false;
            lstUsers.View = View.Details;
            // 
            // cmbDefaultRole
            // 
            cmbDefaultRole.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDefaultRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDefaultRole.FormattingEnabled = true;
            cmbDefaultRole.Location = new Point(14, 113);
            cmbDefaultRole.Name = "cmbDefaultRole";
            cmbDefaultRole.Size = new Size(161, 32);
            cmbDefaultRole.TabIndex = 3;
            // 
            // lblDefaultRole
            // 
            lblDefaultRole.AutoSize = true;
            lblDefaultRole.Location = new Point(14, 86);
            lblDefaultRole.Name = "lblDefaultRole";
            lblDefaultRole.Size = new Size(145, 24);
            lblDefaultRole.TabIndex = 2;
            lblDefaultRole.Text = "デフォルトロール:";
            // 
            // btnAddUser
            // 
            btnAddUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddUser.Location = new Point(181, 113);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(82, 32);
            btnAddUser.TabIndex = 4;
            btnAddUser.Text = "追加";
            btnAddUser.UseVisualStyleBackColor = true;
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Location = new Point(14, 20);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(99, 24);
            lblUserId.TabIndex = 0;
            lblUserId.Text = "ユーザーID:";
            // 
            // txtUserId
            // 
            txtUserId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUserId.Location = new Point(14, 47);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(249, 31);
            txtUserId.TabIndex = 1;
            // 
            // tabControlUser
            // 
            tabControlUser.Controls.Add(tabUserRoles);
            tabControlUser.Controls.Add(tabUserPermissions);
            tabControlUser.Dock = DockStyle.Fill;
            tabControlUser.Location = new Point(0, 0);
            tabControlUser.Name = "tabControlUser";
            tabControlUser.SelectedIndex = 0;
            tabControlUser.Size = new Size(605, 635);
            tabControlUser.TabIndex = 0;
            // 
            // tabUserRoles
            // 
            tabUserRoles.Controls.Add(splitContainer4);
            tabUserRoles.Location = new Point(4, 33);
            tabUserRoles.Name = "tabUserRoles";
            tabUserRoles.Size = new Size(597, 598);
            tabUserRoles.TabIndex = 0;
            tabUserRoles.Text = "ロール";
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(panel6);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(panel7);
            splitContainer4.Panel2.Controls.Add(panel8);
            splitContainer4.Size = new Size(597, 598);
            splitContainer4.SplitterDistance = 297;
            splitContainer4.TabIndex = 0;
            // 
            // panel6
            // 
            panel6.Controls.Add(lstAvailableRoles);
            panel6.Controls.Add(txtUserRoleFilter);
            panel6.Controls.Add(lblUserRoleFilter);
            panel6.Controls.Add(lblSelectedUser);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(297, 598);
            panel6.TabIndex = 0;
            // 
            // lstAvailableRoles
            // 
            lstAvailableRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstAvailableRoles.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAvailableRoles.FullRowSelect = true;
            lstAvailableRoles.Location = new Point(16, 90);
            lstAvailableRoles.Name = "lstAvailableRoles";
            lstAvailableRoles.Size = new Size(268, 492);
            lstAvailableRoles.TabIndex = 3;
            lstAvailableRoles.UseCompatibleStateImageBehavior = false;
            lstAvailableRoles.View = View.Details;
            // 
            // txtUserRoleFilter
            // 
            txtUserRoleFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUserRoleFilter.Location = new Point(87, 53);
            txtUserRoleFilter.Name = "txtUserRoleFilter";
            txtUserRoleFilter.Size = new Size(197, 31);
            txtUserRoleFilter.TabIndex = 2;
            // 
            // lblUserRoleFilter
            // 
            lblUserRoleFilter.AutoSize = true;
            lblUserRoleFilter.Location = new Point(3, 56);
            lblUserRoleFilter.Name = "lblUserRoleFilter";
            lblUserRoleFilter.Size = new Size(81, 24);
            lblUserRoleFilter.TabIndex = 1;
            lblUserRoleFilter.Text = "フィルタ:";
            // 
            // lblSelectedUser
            // 
            lblSelectedUser.AutoSize = true;
            lblSelectedUser.Location = new Point(16, 16);
            lblSelectedUser.Name = "lblSelectedUser";
            lblSelectedUser.Size = new Size(182, 24);
            lblSelectedUser.TabIndex = 0;
            lblSelectedUser.Text = "選択中のユーザー: なし";
            // 
            // panel7
            // 
            panel7.Controls.Add(lstAssignedRoles);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(0, 0);
            panel7.Name = "panel7";
            panel7.Size = new Size(296, 598);
            panel7.TabIndex = 1;
            // 
            // lstAssignedRoles
            // 
            lstAssignedRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstAssignedRoles.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAssignedRoles.FullRowSelect = true;
            lstAssignedRoles.Location = new Point(15, 90);
            lstAssignedRoles.Name = "lstAssignedRoles";
            lstAssignedRoles.Size = new Size(268, 492);
            lstAssignedRoles.TabIndex = 1;
            lstAssignedRoles.UseCompatibleStateImageBehavior = false;
            lstAssignedRoles.View = View.Details;
            // 
            // panel8
            // 
            panel8.Controls.Add(btnRemoveRoleFromUser);
            panel8.Controls.Add(btnAddRoleToUser);
            panel8.Location = new Point(3, 16);
            panel8.Name = "panel8";
            panel8.Size = new Size(290, 67);
            panel8.TabIndex = 0;
            // 
            // btnRemoveRoleFromUser
            // 
            btnRemoveRoleFromUser.Font = new Font("メイリオ", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            btnRemoveRoleFromUser.Location = new Point(12, 37);
            btnRemoveRoleFromUser.Name = "btnRemoveRoleFromUser";
            btnRemoveRoleFromUser.Size = new Size(50, 30);
            btnRemoveRoleFromUser.TabIndex = 1;
            btnRemoveRoleFromUser.Text = "<<";
            btnRemoveRoleFromUser.UseVisualStyleBackColor = true;
            // 
            // btnAddRoleToUser
            // 
            btnAddRoleToUser.Font = new Font("メイリオ", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
            btnAddRoleToUser.Location = new Point(12, 3);
            btnAddRoleToUser.Name = "btnAddRoleToUser";
            btnAddRoleToUser.Size = new Size(50, 30);
            btnAddRoleToUser.TabIndex = 0;
            btnAddRoleToUser.Text = ">>";
            btnAddRoleToUser.UseVisualStyleBackColor = true;
            // 
            // tabUserPermissions
            // 
            tabUserPermissions.Controls.Add(splitContainer5);
            tabUserPermissions.Location = new Point(4, 33);
            tabUserPermissions.Name = "tabUserPermissions";
            tabUserPermissions.Size = new Size(597, 598);
            tabUserPermissions.TabIndex = 1;
            tabUserPermissions.Text = "権限";
            // 
            // splitContainer5
            // 
            splitContainer5.Dock = DockStyle.Fill;
            splitContainer5.Location = new Point(0, 0);
            splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            splitContainer5.Panel1.Controls.Add(tabControlUserPermissions);
            // 
            // splitContainer5.Panel2
            // 
            splitContainer5.Panel2.Controls.Add(panel11);
            splitContainer5.Size = new Size(597, 598);
            splitContainer5.SplitterDistance = 297;
            splitContainer5.TabIndex = 0;
            // 
            // tabControlUserPermissions
            // 
            tabControlUserPermissions.Controls.Add(tabEffectivePermissions);
            tabControlUserPermissions.Controls.Add(tabAdditionalPermissions);
            tabControlUserPermissions.Controls.Add(tabDeniedPermissions);
            tabControlUserPermissions.Dock = DockStyle.Fill;
            tabControlUserPermissions.Location = new Point(0, 0);
            tabControlUserPermissions.Name = "tabControlUserPermissions";
            tabControlUserPermissions.SelectedIndex = 0;
            tabControlUserPermissions.Size = new Size(297, 598);
            tabControlUserPermissions.TabIndex = 0;
            // 
            // tabEffectivePermissions
            // 
            tabEffectivePermissions.Controls.Add(lstEffectivePermissions);
            tabEffectivePermissions.Location = new Point(4, 33);
            tabEffectivePermissions.Name = "tabEffectivePermissions";
            tabEffectivePermissions.Size = new Size(289, 561);
            tabEffectivePermissions.TabIndex = 0;
            tabEffectivePermissions.Text = "有効な権限";
            // 
            // lstEffectivePermissions
            // 
            lstEffectivePermissions.Dock = DockStyle.Fill;
            lstEffectivePermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstEffectivePermissions.FullRowSelect = true;
            lstEffectivePermissions.Location = new Point(0, 0);
            lstEffectivePermissions.Name = "lstEffectivePermissions";
            lstEffectivePermissions.Size = new Size(289, 561);
            lstEffectivePermissions.TabIndex = 0;
            lstEffectivePermissions.UseCompatibleStateImageBehavior = false;
            lstEffectivePermissions.View = View.Details;
            // 
            // tabAdditionalPermissions
            // 
            tabAdditionalPermissions.Controls.Add(lstAdditionalPermissions);
            tabAdditionalPermissions.Controls.Add(panel9);
            tabAdditionalPermissions.Location = new Point(4, 33);
            tabAdditionalPermissions.Name = "tabAdditionalPermissions";
            tabAdditionalPermissions.Size = new Size(289, 561);
            tabAdditionalPermissions.TabIndex = 1;
            tabAdditionalPermissions.Text = "追加権限";
            // 
            // lstAdditionalPermissions
            // 
            lstAdditionalPermissions.Dock = DockStyle.Fill;
            lstAdditionalPermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAdditionalPermissions.FullRowSelect = true;
            lstAdditionalPermissions.Location = new Point(0, 0);
            lstAdditionalPermissions.Name = "lstAdditionalPermissions";
            lstAdditionalPermissions.Size = new Size(289, 526);
            lstAdditionalPermissions.TabIndex = 1;
            lstAdditionalPermissions.UseCompatibleStateImageBehavior = false;
            lstAdditionalPermissions.View = View.Details;
            // 
            // panel9
            // 
            panel9.Controls.Add(btnRemovePermFromUser);
            panel9.Dock = DockStyle.Bottom;
            panel9.Location = new Point(0, 526);
            panel9.Name = "panel9";
            panel9.Size = new Size(289, 35);
            panel9.TabIndex = 0;
            // 
            // btnRemovePermFromUser
            // 
            btnRemovePermFromUser.Anchor = AnchorStyles.None;
            btnRemovePermFromUser.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnRemovePermFromUser.Location = new Point(104, 3);
            btnRemovePermFromUser.Name = "btnRemovePermFromUser";
            btnRemovePermFromUser.Size = new Size(80, 29);
            btnRemovePermFromUser.TabIndex = 0;
            btnRemovePermFromUser.Text = "削除";
            btnRemovePermFromUser.UseVisualStyleBackColor = true;
            // 
            // tabDeniedPermissions
            // 
            tabDeniedPermissions.Controls.Add(lstDeniedPermissions);
            tabDeniedPermissions.Controls.Add(panel10);
            tabDeniedPermissions.Location = new Point(4, 33);
            tabDeniedPermissions.Name = "tabDeniedPermissions";
            tabDeniedPermissions.Size = new Size(289, 561);
            tabDeniedPermissions.TabIndex = 2;
            tabDeniedPermissions.Text = "拒否権限";
            // 
            // lstDeniedPermissions
            // 
            lstDeniedPermissions.Dock = DockStyle.Fill;
            lstDeniedPermissions.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstDeniedPermissions.FullRowSelect = true;
            lstDeniedPermissions.Location = new Point(0, 0);
            lstDeniedPermissions.Name = "lstDeniedPermissions";
            lstDeniedPermissions.Size = new Size(289, 526);
            lstDeniedPermissions.TabIndex = 1;
            lstDeniedPermissions.UseCompatibleStateImageBehavior = false;
            lstDeniedPermissions.View = View.Details;
            // 
            // panel10
            // 
            panel10.Controls.Add(btnRemoveDeniedPerm);
            panel10.Dock = DockStyle.Bottom;
            panel10.Location = new Point(0, 526);
            panel10.Name = "panel10";
            panel10.Size = new Size(289, 35);
            panel10.TabIndex = 0;
            // 
            // btnRemoveDeniedPerm
            // 
            btnRemoveDeniedPerm.Anchor = AnchorStyles.None;
            btnRemoveDeniedPerm.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnRemoveDeniedPerm.Location = new Point(104, 3);
            btnRemoveDeniedPerm.Name = "btnRemoveDeniedPerm";
            btnRemoveDeniedPerm.Size = new Size(80, 29);
            btnRemoveDeniedPerm.TabIndex = 0;
            btnRemoveDeniedPerm.Text = "削除";
            btnRemoveDeniedPerm.UseVisualStyleBackColor = true;
            // 
            // panel11
            // 
            panel11.Controls.Add(lstAvailableUserPerms);
            panel11.Controls.Add(txtUserPermFilter);
            panel11.Controls.Add(lblUserPermFilter);
            panel11.Controls.Add(btnDenyPermForUser);
            panel11.Controls.Add(btnAddPermToUser);
            panel11.Dock = DockStyle.Fill;
            panel11.Location = new Point(0, 0);
            panel11.Name = "panel11";
            panel11.Size = new Size(296, 598);
            panel11.TabIndex = 0;
            // 
            // lstAvailableUserPerms
            // 
            lstAvailableUserPerms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstAvailableUserPerms.Font = new Font("メイリオ", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstAvailableUserPerms.FullRowSelect = true;
            lstAvailableUserPerms.Location = new Point(15, 53);
            lstAvailableUserPerms.Name = "lstAvailableUserPerms";
            lstAvailableUserPerms.Size = new Size(268, 492);
            lstAvailableUserPerms.TabIndex = 4;
            lstAvailableUserPerms.UseCompatibleStateImageBehavior = false;
            lstAvailableUserPerms.View = View.Details;
            // 
            // txtUserPermFilter
            // 
            txtUserPermFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUserPermFilter.Location = new Point(86, 16);
            txtUserPermFilter.Name = "txtUserPermFilter";
            txtUserPermFilter.Size = new Size(197, 31);
            txtUserPermFilter.TabIndex = 1;
            // 
            // lblUserPermFilter
            // 
            lblUserPermFilter.AutoSize = true;
            lblUserPermFilter.Location = new Point(15, 19);
            lblUserPermFilter.Name = "lblUserPermFilter";
            lblUserPermFilter.Size = new Size(81, 24);
            lblUserPermFilter.TabIndex = 0;
            lblUserPermFilter.Text = "フィルタ:";
            // 
            // btnDenyPermForUser
            // 
            btnDenyPermForUser.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDenyPermForUser.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnDenyPermForUser.Location = new Point(148, 551);
            btnDenyPermForUser.Name = "btnDenyPermForUser";
            btnDenyPermForUser.Size = new Size(135, 36);
            btnDenyPermForUser.TabIndex = 3;
            btnDenyPermForUser.Text = "権限を拒否";
            btnDenyPermForUser.UseVisualStyleBackColor = true;
            // 
            // btnAddPermToUser
            // 
            btnAddPermToUser.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddPermToUser.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnAddPermToUser.Location = new Point(15, 551);
            btnAddPermToUser.Name = "btnAddPermToUser";
            btnAddPermToUser.Size = new Size(127, 36);
            btnAddPermToUser.TabIndex = 2;
            btnAddPermToUser.Text = "権限を追加";
            btnAddPermToUser.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Font = new Font("メイリオ", 14.25F);
            btnCancel.Location = new Point(4, 676);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(158, 49);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Font = new Font("メイリオ", 14.25F);
            btnSave.Location = new Point(740, 676);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(158, 49);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // FormPermission
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(902, 728);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(tabControl);
            Name = "FormPermission";
            Text = "権限管理";
            Shown += FormPermission_Shown;
            tabControl.ResumeLayout(false);
            basicPermissionsTab.ResumeLayout(false);
            basicPermissionsTab.PerformLayout();
            combinedPermissionsTab.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            userPermissionsTab.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            tabControlUser.ResumeLayout(false);
            tabUserRoles.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel7.ResumeLayout(false);
            panel8.ResumeLayout(false);
            tabUserPermissions.ResumeLayout(false);
            splitContainer5.Panel1.ResumeLayout(false);
            splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer5).EndInit();
            splitContainer5.ResumeLayout(false);
            tabControlUserPermissions.ResumeLayout(false);
            tabEffectivePermissions.ResumeLayout(false);
            tabAdditionalPermissions.ResumeLayout(false);
            panel9.ResumeLayout(false);
            tabDeniedPermissions.ResumeLayout(false);
            panel10.ResumeLayout(false);
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage basicPermissionsTab;
        private TabPage combinedPermissionsTab;
        private TabPage userPermissionsTab;
        private Button btnCancel;
        private Button btnSave;

        // 基本権限タブのコントロール
        private Label lblPermissionName;
        private TextBox txtPermissionName;
        private Button btnAddPermission;
        private ListView lstPermissions;

        // ロール権限タブのコントロール
        private SplitContainer splitContainer1;
        private Panel panel1;
        private Label lblRoleName;
        private TextBox txtRoleName;
        private Button btnAddRole;
        private ListView lstRoles;
        private SplitContainer splitContainer2;
        private Panel panel2;
        private Label lblSelectedRole;
        private Label lblRolePermFilter;
        private TextBox txtRolePermFilter;
        private ListView lstAvailablePermissions;
        private Panel panel3;
        private Panel panel4;
        private Button btnAddPermToRole;
        private Button btnRemovePermFromRole;
        private ListView lstAssignedPermissions;

        // ユーザー権限タブのコントロール
        private SplitContainer splitContainer3;
        private Panel panel5;
        private Label lblUserId;
        private TextBox txtUserId;
        private Label lblDefaultRole;
        private ComboBox cmbDefaultRole;
        private Button btnAddUser;
        private ListView lstUsers;
        private TabControl tabControlUser;
        private TabPage tabUserRoles;
        private TabPage tabUserPermissions;
        private SplitContainer splitContainer4;
        private Panel panel6;
        private Label lblSelectedUser;
        private Label lblUserRoleFilter;
        private TextBox txtUserRoleFilter;
        private ListView lstAvailableRoles;
        private Panel panel7;
        private Panel panel8;
        private Button btnAddRoleToUser;
        private Button btnRemoveRoleFromUser;
        private ListView lstAssignedRoles;
        private SplitContainer splitContainer5;
        private TabControl tabControlUserPermissions;
        private TabPage tabEffectivePermissions;
        private TabPage tabAdditionalPermissions;
        private TabPage tabDeniedPermissions;
        private ListView lstEffectivePermissions;
        private Panel panel9;
        private Button btnRemovePermFromUser;
        private ListView lstAdditionalPermissions;
        private ListView lstDeniedPermissions;
        private Panel panel10;
        private Button btnRemoveDeniedPerm;
        private Panel panel11;
        private Label lblUserPermFilter;
        private TextBox txtUserPermFilter;
        private Button btnAddPermToUser;
        private Button btnDenyPermForUser;
        private ListView lstAvailableUserPerms;
    }
}