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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPagePermissions = new System.Windows.Forms.TabPage();
            this.btnRemovePermission = new System.Windows.Forms.Button();
            this.btnAddPermission = new System.Windows.Forms.Button();
            this.txtPermissionDescription = new System.Windows.Forms.TextBox();
            this.txtPermissionId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewPermissions = new System.Windows.Forms.ListView();
            this.columnHeaderId = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDescription = new System.Windows.Forms.ColumnHeader();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.btnRevokeUserPermission = new System.Windows.Forms.Button();
            this.btnGrantUserPermission = new System.Windows.Forms.Button();
            this.listBoxAvailablePermissionsUser = new System.Windows.Forms.ListBox();
            this.listBoxUserPermissions = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtUserId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRemoveUser = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.comboBoxUsers = new System.Windows.Forms.ComboBox();
            this.tabPageLocations = new System.Windows.Forms.TabPage();
            this.btnRevokeLocationPermission = new System.Windows.Forms.Button();
            this.btnGrantLocationPermission = new System.Windows.Forms.Button();
            this.listBoxAvailablePermissionsLocation = new System.Windows.Forms.ListBox();
            this.listBoxLocationPermissions = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLocationName = new System.Windows.Forms.TextBox();
            this.txtLocationId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnRemoveLocation = new System.Windows.Forms.Button();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.comboBoxLocations = new System.Windows.Forms.ComboBox();
            this.tabPageControls = new System.Windows.Forms.TabPage();
            this.txtRequiredPermission = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtControlName = new System.Windows.Forms.TextBox();
            this.txtFormName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnRemoveControlPermission = new System.Windows.Forms.Button();
            this.btnAddControlPermission = new System.Windows.Forms.Button();
            this.listViewControlPermissions = new System.Windows.Forms.ListView();
            this.columnHeaderForm = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderControl = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderPermission = new System.Windows.Forms.ColumnHeader();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnInitDefault = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPagePermissions.SuspendLayout();
            this.tabPageUsers.SuspendLayout();
            this.tabPageLocations.SuspendLayout();
            this.tabPageControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPagePermissions);
            this.tabControl.Controls.Add(this.tabPageUsers);
            this.tabControl.Controls.Add(this.tabPageLocations);
            this.tabControl.Controls.Add(this.tabPageControls);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(776, 390);
            this.tabControl.TabIndex = 0;
            // 
            // tabPagePermissions
            // 
            this.tabPagePermissions.Controls.Add(this.btnRemovePermission);
            this.tabPagePermissions.Controls.Add(this.btnAddPermission);
            this.tabPagePermissions.Controls.Add(this.txtPermissionDescription);
            this.tabPagePermissions.Controls.Add(this.txtPermissionId);
            this.tabPagePermissions.Controls.Add(this.label2);
            this.tabPagePermissions.Controls.Add(this.label1);
            this.tabPagePermissions.Controls.Add(this.listViewPermissions);
            this.tabPagePermissions.Location = new System.Drawing.Point(4, 24);
            this.tabPagePermissions.Name = "tabPagePermissions";
            this.tabPagePermissions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePermissions.Size = new System.Drawing.Size(768, 362);
            this.tabPagePermissions.TabIndex = 0;
            this.tabPagePermissions.Text = "権限";
            this.tabPagePermissions.UseVisualStyleBackColor = true;
            // 
            // btnRemovePermission
            // 
            this.btnRemovePermission.Location = new System.Drawing.Point(687, 105);
            this.btnRemovePermission.Name = "btnRemovePermission";
            this.btnRemovePermission.Size = new System.Drawing.Size(75, 23);
            this.btnRemovePermission.TabIndex = 6;
            this.btnRemovePermission.Text = "削除";
            this.btnRemovePermission.UseVisualStyleBackColor = true;
            this.btnRemovePermission.Click += new System.EventHandler(this.btnRemovePermission_Click);
            // 
            // btnAddPermission
            // 
            this.btnAddPermission.Location = new System.Drawing.Point(687, 76);
            this.btnAddPermission.Name = "btnAddPermission";
            this.btnAddPermission.Size = new System.Drawing.Size(75, 23);
            this.btnAddPermission.TabIndex = 5;
            this.btnAddPermission.Text = "追加";
            this.btnAddPermission.UseVisualStyleBackColor = true;
            this.btnAddPermission.Click += new System.EventHandler(this.btnAddPermission_Click);
            // 
            // txtPermissionDescription
            // 
            this.txtPermissionDescription.Location = new System.Drawing.Point(105, 47);
            this.txtPermissionDescription.Name = "txtPermissionDescription";
            this.txtPermissionDescription.Size = new System.Drawing.Size(657, 23);
            this.txtPermissionDescription.TabIndex = 4;
            // 
            // txtPermissionId
            // 
            this.txtPermissionId.Location = new System.Drawing.Point(105, 18);
            this.txtPermissionId.Name = "txtPermissionId";
            this.txtPermissionId.Size = new System.Drawing.Size(657, 23);
            this.txtPermissionId.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "説明";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "権限ID（階層付）";
            // 
            // listViewPermissions
            // 
            this.listViewPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPermissions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderId,
            this.columnHeaderDescription});
            this.listViewPermissions.FullRowSelect = true;
            this.listViewPermissions.HideSelection = false;
            this.listViewPermissions.Location = new System.Drawing.Point(6, 76);
            this.listViewPermissions.Name = "listViewPermissions";
            this.listViewPermissions.Size = new System.Drawing.Size(675, 280);
            this.listViewPermissions.TabIndex = 0;
            this.listViewPermissions.UseCompatibleStateImageBehavior = false;
            this.listViewPermissions.View = System.Windows.Forms.View.Details;
            this.listViewPermissions.SelectedIndexChanged += new System.EventHandler(this.listViewPermissions_SelectedIndexChanged);
            // 
            // columnHeaderId
            // 
            this.columnHeaderId.Text = "権限ID";
            this.columnHeaderId.Width = 250;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "説明";
            this.columnHeaderDescription.Width = 400;
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Controls.Add(this.btnRevokeUserPermission);
            this.tabPageUsers.Controls.Add(this.btnGrantUserPermission);
            this.tabPageUsers.Controls.Add(this.listBoxAvailablePermissionsUser);
            this.tabPageUsers.Controls.Add(this.listBoxUserPermissions);
            this.tabPageUsers.Controls.Add(this.label5);
            this.tabPageUsers.Controls.Add(this.label4);
            this.tabPageUsers.Controls.Add(this.txtUserName);
            this.tabPageUsers.Controls.Add(this.txtUserId);
            this.tabPageUsers.Controls.Add(this.label3);
            this.tabPageUsers.Controls.Add(this.btnRemoveUser);
            this.tabPageUsers.Controls.Add(this.btnAddUser);
            this.tabPageUsers.Controls.Add(this.comboBoxUsers);
            this.tabPageUsers.Location = new System.Drawing.Point(4, 24);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUsers.Size = new System.Drawing.Size(768, 362);
            this.tabPageUsers.TabIndex = 1;
            this.tabPageUsers.Text = "ユーザー権限";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // btnRevokeUserPermission
            // 
            this.btnRevokeUserPermission.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnRevokeUserPermission.Location = new System.Drawing.Point(354, 200);
            this.btnRevokeUserPermission.Name = "btnRevokeUserPermission";
            this.btnRevokeUserPermission.Size = new System.Drawing.Size(60, 23);
            this.btnRevokeUserPermission.TabIndex = 11;
            this.btnRevokeUserPermission.Text = "<<<";
            this.btnRevokeUserPermission.UseVisualStyleBackColor = true;
            this.btnRevokeUserPermission.Click += new System.EventHandler(this.btnRevokeUserPermission_Click);
            // 
            // btnGrantUserPermission
            // 
            this.btnGrantUserPermission.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnGrantUserPermission.Location = new System.Drawing.Point(354, 171);
            this.btnGrantUserPermission.Name = "btnGrantUserPermission";
            this.btnGrantUserPermission.Size = new System.Drawing.Size(60, 23);
            this.btnGrantUserPermission.TabIndex = 10;
            this.btnGrantUserPermission.Text = ">>>";
            this.btnGrantUserPermission.UseVisualStyleBackColor = true;
            this.btnGrantUserPermission.Click += new System.EventHandler(this.btnGrantUserPermission_Click);
            // 
            // listBoxAvailablePermissionsUser
            // 
            this.listBoxAvailablePermissionsUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAvailablePermissionsUser.FormattingEnabled = true;
            this.listBoxAvailablePermissionsUser.ItemHeight = 15;
            this.listBoxAvailablePermissionsUser.Location = new System.Drawing.Point(420, 126);
            this.listBoxAvailablePermissionsUser.Name = "listBoxAvailablePermissionsUser";
            this.listBoxAvailablePermissionsUser.Size = new System.Drawing.Size(342, 229);
            this.listBoxAvailablePermissionsUser.TabIndex = 9;
            // 
            // listBoxUserPermissions
            // 
            this.listBoxUserPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxUserPermissions.FormattingEnabled = true;
            this.listBoxUserPermissions.ItemHeight = 15;
            this.listBoxUserPermissions.Location = new System.Drawing.Point(6, 126);
            this.listBoxUserPermissions.Name = "listBoxUserPermissions";
            this.listBoxUserPermissions.Size = new System.Drawing.Size(342, 229);
            this.listBoxUserPermissions.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(420, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "利用可能権限";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "付与権限";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(313, 76);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(449, 23);
            this.txtUserName.TabIndex = 5;
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(88, 76);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(219, 23);
            this.txtUserId.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "ユーザーID/名前";
            // 
            // btnRemoveUser
            // 
            this.btnRemoveUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveUser.Location = new System.Drawing.Point(687, 47);
            this.btnRemoveUser.Name = "btnRemoveUser";
            this.btnRemoveUser.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveUser.TabIndex = 2;
            this.btnRemoveUser.Text = "削除";
            this.btnRemoveUser.UseVisualStyleBackColor = true;
            this.btnRemoveUser.Click += new System.EventHandler(this.btnRemoveUser_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUser.Location = new System.Drawing.Point(606, 47);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(75, 23);
            this.btnAddUser.TabIndex = 1;
            this.btnAddUser.Text = "追加";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // comboBoxUsers
            // 
            this.comboBoxUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUsers.FormattingEnabled = true;
            this.comboBoxUsers.Location = new System.Drawing.Point(6, 18);
            this.comboBoxUsers.Name = "comboBoxUsers";
            this.comboBoxUsers.Size = new System.Drawing.Size(756, 23);
            this.comboBoxUsers.TabIndex = 0;
            this.comboBoxUsers.SelectedIndexChanged += new System.EventHandler(this.comboBoxUsers_SelectedIndexChanged);
            // 
            // tabPageLocations
            // 
            this.tabPageLocations.Controls.Add(this.btnRevokeLocationPermission);
            this.tabPageLocations.Controls.Add(this.btnGrantLocationPermission);
            this.tabPageLocations.Controls.Add(this.listBoxAvailablePermissionsLocation);
            this.tabPageLocations.Controls.Add(this.listBoxLocationPermissions);
            this.tabPageLocations.Controls.Add(this.label6);
            this.tabPageLocations.Controls.Add(this.label7);
            this.tabPageLocations.Controls.Add(this.txtLocationName);
            this.tabPageLocations.Controls.Add(this.txtLocationId);
            this.tabPageLocations.Controls.Add(this.label8);
            this.tabPageLocations.Controls.Add(this.btnRemoveLocation);
            this.tabPageLocations.Controls.Add(this.btnAddLocation);
            this.tabPageLocations.Controls.Add(this.comboBoxLocations);
            this.tabPageLocations.Location = new System.Drawing.Point(4, 24);
            this.tabPageLocations.Name = "tabPageLocations";
            this.tabPageLocations.Size = new System.Drawing.Size(768, 362);
            this.tabPageLocations.TabIndex = 2;
            this.tabPageLocations.Text = "場所権限";
            this.tabPageLocations.UseVisualStyleBackColor = true;
            // 
            // btnRevokeLocationPermission
            // 
            this.btnRevokeLocationPermission.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnRevokeLocationPermission.Location = new System.Drawing.Point(354, 200);
            this.btnRevokeLocationPermission.Name = "btnRevokeLocationPermission";
            this.btnRevokeLocationPermission.Size = new System.Drawing.Size(60, 23);
            this.btnRevokeLocationPermission.TabIndex = 23;
            this.btnRevokeLocationPermission.Text = "<<<";
            this.btnRevokeLocationPermission.UseVisualStyleBackColor = true;
            this.btnRevokeLocationPermission.Click += new System.EventHandler(this.btnRevokeLocationPermission_Click);
            // 
            // btnGrantLocationPermission
            // 
            this.btnGrantLocationPermission.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnGrantLocationPermission.Location = new System.Drawing.Point(354, 171);
            this.btnGrantLocationPermission.Name = "btnGrantLocationPermission";
            this.btnGrantLocationPermission.Size = new System.Drawing.Size(60, 23);
            this.btnGrantLocationPermission.TabIndex = 22;
            this.btnGrantLocationPermission.Text = ">>>";
            this.btnGrantLocationPermission.UseVisualStyleBackColor = true;
            this.btnGrantLocationPermission.Click += new System.EventHandler(this.btnGrantLocationPermission_Click);
            // 
            // listBoxAvailablePermissionsLocation
            // 
            this.listBoxAvailablePermissionsLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAvailablePermissionsLocation.FormattingEnabled = true;
            this.listBoxAvailablePermissionsLocation.ItemHeight = 15;
            this.listBoxAvailablePermissionsLocation.Location = new System.Drawing.Point(420, 126);
            this.listBoxAvailablePermissionsLocation.Name = "listBoxAvailablePermissionsLocation";
            this.listBoxAvailablePermissionsLocation.Size = new System.Drawing.Size(342, 229);
            this.listBoxAvailablePermissionsLocation.TabIndex = 21;
            // 
            // listBoxLocationPermissions
            // 
            this.listBoxLocationPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxLocationPermissions.FormattingEnabled = true;
            this.listBoxLocationPermissions.ItemHeight = 15;
            this.listBoxLocationPermissions.Location = new System.Drawing.Point(6, 126);
            this.listBoxLocationPermissions.Name = "listBoxLocationPermissions";
            this.listBoxLocationPermissions.Size = new System.Drawing.Size(342, 229);
            this.listBoxLocationPermissions.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(420, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 19;
            this.label6.Text = "利用可能権限";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "付与権限";
            // 
            // txtLocationName
            // 
            this.txtLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocationName.Location = new System.Drawing.Point(313, 76);
            this.txtLocationName.Name = "txtLocationName";
            this.txtLocationName.Size = new System.Drawing.Size(449, 23);
            this.txtLocationName.TabIndex = 17;
            // 
            // txtLocationId
            // 
            this.txtLocationId.Location = new System.Drawing.Point(88, 76);
            this.txtLocationId.Name = "txtLocationId";
            this.txtLocationId.Size = new System.Drawing.Size(219, 23);
            this.txtLocationId.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 15);
            this.label8.TabIndex = 15;
            this.label8.Text = "場所ID/名前";
            // 
            // btnRemoveLocation
            // 
            this.btnRemoveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveLocation.Location = new System.Drawing.Point(687, 47);
            this.btnRemoveLocation.Name = "btnRemoveLocation";
            this.btnRemoveLocation.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveLocation.TabIndex = 14;
            this.btnRemoveLocation.Text = "削除";
            this.btnRemoveLocation.UseVisualStyleBackColor = true;
            this.btnRemoveLocation.Click += new System.EventHandler(this.btnRemoveLocation_Click);
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddLocation.Location = new System.Drawing.Point(606, 47);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Size = new System.Drawing.Size(75, 23);
            this.btnAddLocation.TabIndex = 13;
            this.btnAddLocation.Text = "追加";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // comboBoxLocations
            // 
            this.comboBoxLocations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLocations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLocations.FormattingEnabled = true;
            this.comboBoxLocations.Location = new System.Drawing.Point(6, 18);
            this.comboBoxLocations.Name = "comboBoxLocations";
            this.comboBoxLocations.Size = new System.Drawing.Size(756, 23);
            this.comboBoxLocations.TabIndex = 12;
            this.comboBoxLocations.SelectedIndexChanged += new System.EventHandler(this.comboBoxLocations_SelectedIndexChanged);
            // 
            // tabPageControls
            // 
            this.tabPageControls.Controls.Add(this.txtRequiredPermission);
            this.tabPageControls.Controls.Add(this.label11);
            this.tabPageControls.Controls.Add(this.txtControlName);
            this.tabPageControls.Controls.Add(this.txtFormName);
            this.tabPageControls.Controls.Add(this.label9);
            this.tabPageControls.Controls.Add(this.label10);
            this.tabPageControls.Controls.Add(this.btnRemoveControlPermission);
            this.tabPageControls.Controls.Add(this.btnAddControlPermission);
            this.tabPageControls.Controls.Add(this.listViewControlPermissions);
            this.tabPageControls.Location = new System.Drawing.Point(4, 24);
            this.tabPageControls.Name = "tabPageControls";
            this.tabPageControls.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageControls.Size = new System.Drawing.Size(768, 362);
            this.tabPageControls.TabIndex = 3;
            this.tabPageControls.Text = "コントロール権限";
            this.tabPageControls.UseVisualStyleBackColor = true;
            // 
            // txtRequiredPermission
            // 
            this.txtRequiredPermission.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRequiredPermission.Location = new System.Drawing.Point(117, 76);
            this.txtRequiredPermission.Name = "txtRequiredPermission";
            this.txtRequiredPermission.Size = new System.Drawing.Size(564, 23);
            this.txtRequiredPermission.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 15);
            this.label11.TabIndex = 12;
            this.label11.Text = "必要な権限";
            // 
            // txtControlName
            // 
            this.txtControlName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtControlName.Location = new System.Drawing.Point(117, 47);
            this.txtControlName.Name = "txtControlName";
            this.txtControlName.Size = new System.Drawing.Size(564, 23);
            this.txtControlName.TabIndex = 11;
            // 
            // txtFormName
            // 
            this.txtFormName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFormName.Location = new System.Drawing.Point(117, 18);
            this.txtFormName.Name = "txtFormName";
            this.txtFormName.Size = new System.Drawing.Size(564, 23);
            this.txtFormName.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "コントロール名";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 15);
            this.label10.TabIndex = 8;
            this.label10.Text = "フォーム名";
            // 
            // btnRemoveControlPermission
            // 
            this.btnRemoveControlPermission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveControlPermission.Location = new System.Drawing.Point(687, 76);
            this.btnRemoveControlPermission.Name = "btnRemoveControlPermission";
            this.btnRemoveControlPermission.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveControlPermission.TabIndex = 7;
            this.btnRemoveControlPermission.Text = "削除";
            this.btnRemoveControlPermission.UseVisualStyleBackColor = true;
            this.btnRemoveControlPermission.Click += new System.EventHandler(this.btnRemoveControlPermission_Click);
            // 
            // btnAddControlPermission
            // 
            this.btnAddControlPermission.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddControlPermission.Location = new System.Drawing.Point(687, 47);
            this.btnAddControlPermission.Name = "btnAddControlPermission";
            this.btnAddControlPermission.Size = new System.Drawing.Size(75, 23);
            this.btnAddControlPermission.TabIndex = 6;
            this.btnAddControlPermission.Text = "追加";
            this.btnAddControlPermission.UseVisualStyleBackColor = true;
            this.btnAddControlPermission.Click += new System.EventHandler(this.btnAddControlPermission_Click);
            // 
            // listViewControlPermissions
            // 
            this.listViewControlPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewControlPermissions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderForm,
            this.columnHeaderControl,
            this.columnHeaderPermission});
            this.listViewControlPermissions.FullRowSelect = true;
            this.listViewControlPermissions.HideSelection = false;
            this.listViewControlPermissions.Location = new System.Drawing.Point(6, 105);
            this.listViewControlPermissions.Name = "listViewControlPermissions";
            this.listViewControlPermissions.Size = new System.Drawing.Size(756, 251);
            this.listViewControlPermissions.TabIndex = 0;
            this.listViewControlPermissions.UseCompatibleStateImageBehavior = false;
            this.listViewControlPermissions.View = System.Windows.Forms.View.Details;
            this.listViewControlPermissions.SelectedIndexChanged += new System.EventHandler(this.listViewControlPermissions_SelectedIndexChanged);
            // 
            // columnHeaderForm
            // 
            this.columnHeaderForm.Text = "フォーム名";
            this.columnHeaderForm.Width = 200;
            // 
            // columnHeaderControl
            // 
            this.columnHeaderControl.Text = "コントロール名";
            this.columnHeaderControl.Width = 200;
            // 
            // columnHeaderPermission
            // 
            this.columnHeaderPermission.Text = "必要な権限";
            this.columnHeaderPermission.Width = 350;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(713, 415);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(632, 415);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "読み込み";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnInitDefault
            // 
            this.btnInitDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInitDefault.Location = new System.Drawing.Point(12, 415);
            this.btnInitDefault.Name = "btnInitDefault";
            this.btnInitDefault.Size = new System.Drawing.Size(128, 23);
            this.btnInitDefault.TabIndex = 3;
            this.btnInitDefault.Text = "デフォルト権限初期化";
            this.btnInitDefault.UseVisualStyleBackColor = true;
            this.btnInitDefault.Click += new System.EventHandler(this.btnInitDefault_Click);
            // 
            // FormPermission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnInitDefault);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl);
            this.Name = "FormPermission";
            this.Text = "権限管理";
            this.Load += new System.EventHandler(this.FormPermission_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPagePermissions.ResumeLayout(false);
            this.tabPagePermissions.PerformLayout();
            this.tabPageUsers.ResumeLayout(false);
            this.tabPageUsers.PerformLayout();
            this.tabPageLocations.ResumeLayout(false);
            this.tabPageLocations.PerformLayout();
            this.tabPageControls.ResumeLayout(false);
            this.tabPageControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPagePermissions;
        private System.Windows.Forms.TabPage tabPageUsers;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnInitDefault;
        private System.Windows.Forms.ListView listViewPermissions;
        private System.Windows.Forms.ColumnHeader columnHeaderId;
        private System.Windows.Forms.ColumnHeader columnHeaderDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRemovePermission;
        private System.Windows.Forms.Button btnAddPermission;
        private System.Windows.Forms.TextBox txtPermissionDescription;
        private System.Windows.Forms.TextBox txtPermissionId;
        private System.Windows.Forms.ComboBox comboBoxUsers;
        private System.Windows.Forms.Button btnRemoveUser;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtUserId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRevokeUserPermission;
        private System.Windows.Forms.Button btnGrantUserPermission;
        private System.Windows.Forms.ListBox listBoxAvailablePermissionsUser;
        private System.Windows.Forms.ListBox listBoxUserPermissions;
        private System.Windows.Forms.TabPage tabPageLocations;
        private System.Windows.Forms.Button btnRevokeLocationPermission;
        private System.Windows.Forms.Button btnGrantLocationPermission;
        private System.Windows.Forms.ListBox listBoxAvailablePermissionsLocation;
        private System.Windows.Forms.ListBox listBoxLocationPermissions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLocationName;
        private System.Windows.Forms.TextBox txtLocationId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnRemoveLocation;
        private System.Windows.Forms.Button btnAddLocation;
        private System.Windows.Forms.ComboBox comboBoxLocations;
        private System.Windows.Forms.TabPage tabPageControls;
        private System.Windows.Forms.ListView listViewControlPermissions;
        private System.Windows.Forms.ColumnHeader columnHeaderForm;
        private System.Windows.Forms.ColumnHeader columnHeaderControl;
        private System.Windows.Forms.ColumnHeader columnHeaderPermission;
        private System.Windows.Forms.TextBox txtRequiredPermission;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtControlName;
        private System.Windows.Forms.TextBox txtFormName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnRemoveControlPermission;
        private System.Windows.Forms.Button btnAddControlPermission;
    }
}