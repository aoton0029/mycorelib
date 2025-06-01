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
            btnRegistPerm = new Button();
            gridPerm = new DataGridView();
            ColumnPermId = new DataGridViewTextBoxColumn();
            ColumnPermName = new DataGridViewTextBoxColumn();
            ColumnPermDeleteButton = new DataGridViewButtonColumn();
            lblTitlePerm = new Label();
            txtPerm = new TextBox();
            combinedPermissionsTab = new TabPage();
            btnRolePermAvailable = new Button();
            btnRolePermSelected = new Button();
            gridRolePermAvailable = new DataGridView();
            gridRolePermSelected = new DataGridView();
            lblTItleSelectedRole = new Label();
            btnRegistRole = new Button();
            gridRole = new DataGridView();
            ColumnRoleId = new DataGridViewTextBoxColumn();
            ColumnRoleName = new DataGridViewTextBoxColumn();
            ColumnRoleEditButton = new DataGridViewButtonColumn();
            ColumnRoleDeleteButton = new DataGridViewButtonColumn();
            lblTitleRole = new Label();
            txtRoleName = new TextBox();
            userPermissionsTab = new TabPage();
            btnAssignUserRole = new Button();
            lblTItleUserRole = new Label();
            cmbUserRole = new ComboBox();
            btnUserPermAvailable = new Button();
            btnUserPremSelected = new Button();
            gridUserPermAvailable = new DataGridView();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            gridUserPermSelected = new DataGridView();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            lblTitleSelectedUserId = new Label();
            btnRegistUser = new Button();
            gridUser = new DataGridView();
            lblTitleUserId = new Label();
            txtUserId = new TextBox();
            btnCancel = new Button();
            btnSave = new Button();
            ColumnUserId = new DataGridViewTextBoxColumn();
            ColumnUserName = new DataGridViewTextBoxColumn();
            ColumnUserEdit = new DataGridViewButtonColumn();
            ColumnUserDelete = new DataGridViewButtonColumn();
            ColumnRoleAvailableId = new DataGridViewTextBoxColumn();
            ColumnRoleAvailableName = new DataGridViewTextBoxColumn();
            ColumnRoleSelectedId = new DataGridViewTextBoxColumn();
            ColumnRoleSelectedName = new DataGridViewTextBoxColumn();
            tabControl.SuspendLayout();
            basicPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridPerm).BeginInit();
            combinedPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRolePermAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermSelected).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRole).BeginInit();
            userPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUserPermAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUserPermSelected).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridUser).BeginInit();
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
            basicPermissionsTab.Controls.Add(btnRegistPerm);
            basicPermissionsTab.Controls.Add(gridPerm);
            basicPermissionsTab.Controls.Add(lblTitlePerm);
            basicPermissionsTab.Controls.Add(txtPerm);
            basicPermissionsTab.Location = new Point(4, 33);
            basicPermissionsTab.Name = "basicPermissionsTab";
            basicPermissionsTab.Size = new Size(886, 635);
            basicPermissionsTab.TabIndex = 0;
            basicPermissionsTab.Text = "基本権限";
            // 
            // btnRegistPerm
            // 
            btnRegistPerm.Location = new Point(386, 18);
            btnRegistPerm.Name = "btnRegistPerm";
            btnRegistPerm.Size = new Size(120, 34);
            btnRegistPerm.TabIndex = 11;
            btnRegistPerm.Text = "追加";
            btnRegistPerm.UseVisualStyleBackColor = true;
            // 
            // gridPerm
            // 
            gridPerm.AllowUserToAddRows = false;
            gridPerm.AllowUserToDeleteRows = false;
            gridPerm.AllowUserToResizeRows = false;
            gridPerm.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPerm.Columns.AddRange(new DataGridViewColumn[] { ColumnPermId, ColumnPermName, ColumnPermDeleteButton });
            gridPerm.Location = new Point(13, 58);
            gridPerm.Name = "gridPerm";
            gridPerm.Size = new Size(493, 559);
            gridPerm.TabIndex = 10;
            // 
            // ColumnPermId
            // 
            ColumnPermId.HeaderText = "ID";
            ColumnPermId.Name = "ColumnPermId";
            ColumnPermId.Width = 80;
            // 
            // ColumnPermName
            // 
            ColumnPermName.HeaderText = "Name";
            ColumnPermName.Name = "ColumnPermName";
            ColumnPermName.Width = 250;
            // 
            // ColumnPermDeleteButton
            // 
            ColumnPermDeleteButton.HeaderText = "";
            ColumnPermDeleteButton.Name = "ColumnPermDeleteButton";
            ColumnPermDeleteButton.Text = "Delete";
            ColumnPermDeleteButton.UseColumnTextForButtonValue = true;
            ColumnPermDeleteButton.Width = 80;
            // 
            // lblTitlePerm
            // 
            lblTitlePerm.AutoSize = true;
            lblTitlePerm.Location = new Point(17, 23);
            lblTitlePerm.Name = "lblTitlePerm";
            lblTitlePerm.Size = new Size(58, 24);
            lblTitlePerm.TabIndex = 9;
            lblTitlePerm.Text = "権限名";
            // 
            // txtPerm
            // 
            txtPerm.Location = new Point(123, 20);
            txtPerm.Name = "txtPerm";
            txtPerm.Size = new Size(256, 31);
            txtPerm.TabIndex = 8;
            // 
            // combinedPermissionsTab
            // 
            combinedPermissionsTab.Controls.Add(btnRolePermAvailable);
            combinedPermissionsTab.Controls.Add(btnRolePermSelected);
            combinedPermissionsTab.Controls.Add(gridRolePermAvailable);
            combinedPermissionsTab.Controls.Add(gridRolePermSelected);
            combinedPermissionsTab.Controls.Add(lblTItleSelectedRole);
            combinedPermissionsTab.Controls.Add(btnRegistRole);
            combinedPermissionsTab.Controls.Add(gridRole);
            combinedPermissionsTab.Controls.Add(lblTitleRole);
            combinedPermissionsTab.Controls.Add(txtRoleName);
            combinedPermissionsTab.Location = new Point(4, 33);
            combinedPermissionsTab.Name = "combinedPermissionsTab";
            combinedPermissionsTab.Size = new Size(886, 635);
            combinedPermissionsTab.TabIndex = 1;
            combinedPermissionsTab.Text = "役割権限";
            // 
            // btnRolePermAvailable
            // 
            btnRolePermAvailable.Location = new Point(400, 449);
            btnRolePermAvailable.Name = "btnRolePermAvailable";
            btnRolePermAvailable.Size = new Size(84, 72);
            btnRolePermAvailable.TabIndex = 20;
            btnRolePermAvailable.Text = "＞＞";
            btnRolePermAvailable.UseVisualStyleBackColor = true;
            // 
            // btnRolePermSelected
            // 
            btnRolePermSelected.Location = new Point(400, 371);
            btnRolePermSelected.Name = "btnRolePermSelected";
            btnRolePermSelected.Size = new Size(84, 72);
            btnRolePermSelected.TabIndex = 19;
            btnRolePermSelected.Text = "＜＜";
            btnRolePermSelected.UseVisualStyleBackColor = true;
            // 
            // gridRolePermAvailable
            // 
            gridRolePermAvailable.AllowUserToAddRows = false;
            gridRolePermAvailable.AllowUserToDeleteRows = false;
            gridRolePermAvailable.AllowUserToResizeRows = false;
            gridRolePermAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRolePermAvailable.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleAvailableId, ColumnRoleAvailableName });
            gridRolePermAvailable.Location = new Point(492, 246);
            gridRolePermAvailable.Name = "gridRolePermAvailable";
            gridRolePermAvailable.RowHeadersWidth = 20;
            gridRolePermAvailable.Size = new Size(383, 375);
            gridRolePermAvailable.TabIndex = 18;
            // 
            // gridRolePermSelected
            // 
            gridRolePermSelected.AllowUserToAddRows = false;
            gridRolePermSelected.AllowUserToDeleteRows = false;
            gridRolePermSelected.AllowUserToResizeRows = false;
            gridRolePermSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRolePermSelected.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleSelectedId, ColumnRoleSelectedName });
            gridRolePermSelected.Location = new Point(11, 246);
            gridRolePermSelected.Name = "gridRolePermSelected";
            gridRolePermSelected.RowHeadersWidth = 20;
            gridRolePermSelected.Size = new Size(383, 375);
            gridRolePermSelected.TabIndex = 17;
            // 
            // lblTItleSelectedRole
            // 
            lblTItleSelectedRole.AutoSize = true;
            lblTItleSelectedRole.Location = new Point(11, 215);
            lblTItleSelectedRole.Name = "lblTItleSelectedRole";
            lblTItleSelectedRole.Size = new Size(138, 24);
            lblTItleSelectedRole.TabIndex = 16;
            lblTItleSelectedRole.Text = "選択中のロール：";
            // 
            // btnRegistRole
            // 
            btnRegistRole.Location = new Point(384, 12);
            btnRegistRole.Name = "btnRegistRole";
            btnRegistRole.Size = new Size(120, 33);
            btnRegistRole.TabIndex = 15;
            btnRegistRole.Text = "追加";
            btnRegistRole.UseVisualStyleBackColor = true;
            // 
            // gridRole
            // 
            gridRole.AllowUserToAddRows = false;
            gridRole.AllowUserToDeleteRows = false;
            gridRole.AllowUserToResizeRows = false;
            gridRole.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRole.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleId, ColumnRoleName, ColumnRoleEditButton, ColumnRoleDeleteButton });
            gridRole.Location = new Point(11, 50);
            gridRole.Name = "gridRole";
            gridRole.RowHeadersWidth = 20;
            gridRole.Size = new Size(864, 149);
            gridRole.TabIndex = 14;
            // 
            // ColumnRoleId
            // 
            ColumnRoleId.HeaderText = "ID";
            ColumnRoleId.Name = "ColumnRoleId";
            ColumnRoleId.Width = 80;
            // 
            // ColumnRoleName
            // 
            ColumnRoleName.HeaderText = "Name";
            ColumnRoleName.Name = "ColumnRoleName";
            ColumnRoleName.Width = 250;
            // 
            // ColumnRoleEditButton
            // 
            ColumnRoleEditButton.HeaderText = "";
            ColumnRoleEditButton.Name = "ColumnRoleEditButton";
            ColumnRoleEditButton.Text = "Edit";
            ColumnRoleEditButton.UseColumnTextForButtonValue = true;
            ColumnRoleEditButton.Width = 80;
            // 
            // ColumnRoleDeleteButton
            // 
            ColumnRoleDeleteButton.HeaderText = "";
            ColumnRoleDeleteButton.Name = "ColumnRoleDeleteButton";
            ColumnRoleDeleteButton.Text = "Delete";
            ColumnRoleDeleteButton.UseColumnTextForButtonValue = true;
            ColumnRoleDeleteButton.Width = 80;
            // 
            // lblTitleRole
            // 
            lblTitleRole.AutoSize = true;
            lblTitleRole.Location = new Point(11, 16);
            lblTitleRole.Name = "lblTitleRole";
            lblTitleRole.Size = new Size(90, 24);
            lblTitleRole.TabIndex = 13;
            lblTitleRole.Text = "ロール名：";
            // 
            // txtRoleName
            // 
            txtRoleName.Location = new Point(117, 13);
            txtRoleName.Name = "txtRoleName";
            txtRoleName.Size = new Size(256, 31);
            txtRoleName.TabIndex = 12;
            // 
            // userPermissionsTab
            // 
            userPermissionsTab.Controls.Add(btnAssignUserRole);
            userPermissionsTab.Controls.Add(lblTItleUserRole);
            userPermissionsTab.Controls.Add(this.cmbUserRole);
            userPermissionsTab.Controls.Add(btnUserPermAvailable);
            userPermissionsTab.Controls.Add(btnUserPremSelected);
            userPermissionsTab.Controls.Add(gridUserPermAvailable);
            userPermissionsTab.Controls.Add(gridUserPermSelected);
            userPermissionsTab.Controls.Add(lblTitleSelectedUserId);
            userPermissionsTab.Controls.Add(btnRegistUser);
            userPermissionsTab.Controls.Add(gridUser);
            userPermissionsTab.Controls.Add(lblTitleUserId);
            userPermissionsTab.Controls.Add(txtUserId);
            userPermissionsTab.Location = new Point(4, 33);
            userPermissionsTab.Name = "userPermissionsTab";
            userPermissionsTab.Size = new Size(886, 635);
            userPermissionsTab.TabIndex = 2;
            userPermissionsTab.Text = "ユーザー権限";
            // 
            // btnAssignUserRole
            // 
            btnAssignUserRole.Location = new Point(322, 248);
            btnAssignUserRole.Name = "btnAssignUserRole";
            btnAssignUserRole.Size = new Size(120, 33);
            btnAssignUserRole.TabIndex = 32;
            btnAssignUserRole.Text = "適用";
            btnAssignUserRole.UseVisualStyleBackColor = true;
            // 
            // lblTItleUserRole
            // 
            lblTItleUserRole.AutoSize = true;
            lblTItleUserRole.Location = new Point(11, 251);
            lblTItleUserRole.Name = "lblTItleUserRole";
            lblTItleUserRole.Size = new Size(58, 24);
            lblTItleUserRole.TabIndex = 31;
            lblTItleUserRole.Text = "役割：";
            // 
            // cmbUserRole
            // 
            this.cmbUserRole.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbUserRole.FlatStyle = FlatStyle.Flat;
            this.cmbUserRole.FormattingEnabled = true;
            this.cmbUserRole.Location = new Point(75, 248);
            this.cmbUserRole.Name = "cmbUserRole";
            this.cmbUserRole.Size = new Size(241, 32);
            this.cmbUserRole.TabIndex = 30;
            // 
            // btnUserPermAvailable
            // 
            btnUserPermAvailable.Location = new Point(400, 459);
            btnUserPermAvailable.Name = "btnUserPermAvailable";
            btnUserPermAvailable.Size = new Size(84, 72);
            btnUserPermAvailable.TabIndex = 29;
            btnUserPermAvailable.Text = "＞＞";
            btnUserPermAvailable.UseVisualStyleBackColor = true;
            // 
            // btnUserPremSelected
            // 
            btnUserPremSelected.Location = new Point(400, 381);
            btnUserPremSelected.Name = "btnUserPremSelected";
            btnUserPremSelected.Size = new Size(84, 72);
            btnUserPremSelected.TabIndex = 28;
            btnUserPremSelected.Text = "＜＜";
            btnUserPremSelected.UseVisualStyleBackColor = true;
            // 
            // gridUserPermAvailable
            // 
            gridUserPermAvailable.AllowUserToAddRows = false;
            gridUserPermAvailable.AllowUserToDeleteRows = false;
            gridUserPermAvailable.AllowUserToResizeRows = false;
            gridUserPermAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserPermAvailable.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6 });
            gridUserPermAvailable.Location = new Point(492, 289);
            gridUserPermAvailable.Name = "gridUserPermAvailable";
            gridUserPermAvailable.RowHeadersWidth = 20;
            gridUserPermAvailable.Size = new Size(383, 338);
            gridUserPermAvailable.TabIndex = 27;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "ID";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.Width = 80;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.HeaderText = "Name";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.Width = 250;
            // 
            // gridUserPermSelected
            // 
            gridUserPermSelected.AllowUserToAddRows = false;
            gridUserPermSelected.AllowUserToDeleteRows = false;
            gridUserPermSelected.AllowUserToResizeRows = false;
            gridUserPermSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUserPermSelected.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8 });
            gridUserPermSelected.Location = new Point(11, 289);
            gridUserPermSelected.Name = "gridUserPermSelected";
            gridUserPermSelected.RowHeadersWidth = 20;
            gridUserPermSelected.Size = new Size(383, 338);
            gridUserPermSelected.TabIndex = 26;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.HeaderText = "ID";
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.Width = 80;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.HeaderText = "Name";
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.Width = 250;
            // 
            // lblTitleSelectedUserId
            // 
            lblTitleSelectedUserId.AutoSize = true;
            lblTitleSelectedUserId.Location = new Point(11, 214);
            lblTitleSelectedUserId.Name = "lblTitleSelectedUserId";
            lblTitleSelectedUserId.Size = new Size(154, 24);
            lblTitleSelectedUserId.TabIndex = 25;
            lblTitleSelectedUserId.Text = "選択中のユーザー：";
            // 
            // btnRegistUser
            // 
            btnRegistUser.Location = new Point(384, 11);
            btnRegistUser.Name = "btnRegistUser";
            btnRegistUser.Size = new Size(120, 33);
            btnRegistUser.TabIndex = 24;
            btnRegistUser.Text = "追加";
            btnRegistUser.UseVisualStyleBackColor = true;
            // 
            // gridUser
            // 
            gridUser.AllowUserToAddRows = false;
            gridUser.AllowUserToDeleteRows = false;
            gridUser.AllowUserToResizeRows = false;
            gridUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUser.Columns.AddRange(new DataGridViewColumn[] { ColumnUserId, ColumnUserName, ColumnUserEdit, ColumnUserDelete });
            gridUser.Location = new Point(11, 49);
            gridUser.Name = "gridUser";
            gridUser.RowHeadersWidth = 20;
            gridUser.Size = new Size(864, 149);
            gridUser.TabIndex = 23;
            // 
            // lblTitleUserId
            // 
            lblTitleUserId.AutoSize = true;
            lblTitleUserId.Location = new Point(11, 15);
            lblTitleUserId.Name = "lblTitleUserId";
            lblTitleUserId.Size = new Size(108, 24);
            lblTitleUserId.TabIndex = 22;
            lblTitleUserId.Text = "ユーザーID：";
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(117, 12);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(256, 31);
            txtUserId.TabIndex = 21;
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
            // ColumnUserId
            // 
            ColumnUserId.HeaderText = "ID";
            ColumnUserId.Name = "ColumnUserId";
            ColumnUserId.Width = 80;
            // 
            // ColumnUserName
            // 
            ColumnUserName.HeaderText = "Name";
            ColumnUserName.Name = "ColumnUserName";
            ColumnUserName.Width = 250;
            // 
            // ColumnUserEdit
            // 
            ColumnUserEdit.HeaderText = "";
            ColumnUserEdit.Name = "ColumnUserEdit";
            ColumnUserEdit.Text = "Edit";
            ColumnUserEdit.UseColumnTextForButtonValue = true;
            ColumnUserEdit.Width = 80;
            // 
            // ColumnUserDelete
            // 
            ColumnUserDelete.HeaderText = "";
            ColumnUserDelete.Name = "ColumnUserDelete";
            ColumnUserDelete.Text = "Delete";
            ColumnUserDelete.UseColumnTextForButtonValue = true;
            ColumnUserDelete.Width = 80;
            // 
            // ColumnRoleAvailableId
            // 
            ColumnRoleAvailableId.HeaderText = "ID";
            ColumnRoleAvailableId.Name = "ColumnRoleAvailableId";
            ColumnRoleAvailableId.Width = 80;
            // 
            // ColumnRoleAvailableName
            // 
            ColumnRoleAvailableName.HeaderText = "Name";
            ColumnRoleAvailableName.Name = "ColumnRoleAvailableName";
            ColumnRoleAvailableName.Width = 250;
            // 
            // ColumnRoleSelectedId
            // 
            ColumnRoleSelectedId.HeaderText = "ID";
            ColumnRoleSelectedId.Name = "ColumnRoleSelectedId";
            ColumnRoleSelectedId.Width = 80;
            // 
            // ColumnRoleSelectedName
            // 
            ColumnRoleSelectedName.HeaderText = "Name";
            ColumnRoleSelectedName.Name = "ColumnRoleSelectedName";
            ColumnRoleSelectedName.Width = 250;
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
            Text = "FormPermission";
            tabControl.ResumeLayout(false);
            basicPermissionsTab.ResumeLayout(false);
            basicPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridPerm).EndInit();
            combinedPermissionsTab.ResumeLayout(false);
            combinedPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridRolePermAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermSelected).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRole).EndInit();
            userPermissionsTab.ResumeLayout(false);
            userPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridUserPermAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUserPermSelected).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridUser).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage basicPermissionsTab;
        private TabPage combinedPermissionsTab;
        private TabPage userPermissionsTab;
        private Button btnCancel;
        private Button btnSave;
        private Button btnRegistPerm;
        private DataGridView gridPerm;
        private Label lblTitlePerm;
        private TextBox txtPerm;
        private DataGridViewTextBoxColumn ColumnPermId;
        private DataGridViewTextBoxColumn ColumnPermName;
        private DataGridViewButtonColumn ColumnPermDeleteButton;
        private Button btnRegistRole;
        private DataGridView gridRole;
        private Label lblTitleRole;
        private TextBox txtRoleName;
        private DataGridViewTextBoxColumn ColumnRoleId;
        private DataGridViewTextBoxColumn ColumnRoleName;
        private DataGridViewButtonColumn ColumnRoleEditButton;
        private DataGridViewButtonColumn ColumnRoleDeleteButton;
        private Label lblTItleSelectedRole;
        private DataGridView gridRolePermSelected;
        private DataGridView gridRolePermAvailable;
        private Button btnRolePermAvailable;
        private Button btnRolePermSelected;
        private Button btnUserPermAvailable;
        private Button btnUserPremSelected;
        private DataGridView gridUserPermAvailable;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridView gridUserPermSelected;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private Label lblTitleSelectedUserId;
        private Button btnRegistUser;
        private DataGridView gridUser;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewButtonColumn dataGridViewButtonColumn1;
        private DataGridViewButtonColumn dataGridViewButtonColumn2;
        private Label lblTitleUserId;
        private TextBox txtUserId;
        private ComboBox cmbUserRole;
        private Label lblTItleUserRole;
        private Button btnAssignUserRole;
        private DataGridViewTextBoxColumn ColumnUserId;
        private DataGridViewTextBoxColumn ColumnUserName;
        private DataGridViewButtonColumn ColumnUserEdit;
        private DataGridViewButtonColumn ColumnUserDelete;
        private DataGridViewTextBoxColumn ColumnRoleAvailableId;
        private DataGridViewTextBoxColumn ColumnRoleAvailableName;
        private DataGridViewTextBoxColumn ColumnRoleSelectedId;
        private DataGridViewTextBoxColumn ColumnRoleSelectedName;
    }
}