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
            lblTitlePerm = new Label();
            txtPerm = new TextBox();
            combinedPermissionsTab = new TabPage();
            userPermissionsTab = new TabPage();
            btnCancel = new Button();
            btnSave = new Button();
            ColumnPermId = new DataGridViewTextBoxColumn();
            ColumnPermName = new DataGridViewTextBoxColumn();
            ColumnPermDeleteButton = new DataGridViewButtonColumn();
            btnRegistRole = new Button();
            gridRole = new DataGridView();
            lblTitleRole = new Label();
            txtRoleName = new TextBox();
            ColumnRoleId = new DataGridViewTextBoxColumn();
            ColumnRoleName = new DataGridViewTextBoxColumn();
            ColumnRoleEditButton = new DataGridViewButtonColumn();
            ColumnRoleDeleteButton = new DataGridViewButtonColumn();
            label3 = new Label();
            gridRolePermSelected = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            gridRolePermAvailable = new DataGridView();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            btnRolePermSelected = new Button();
            btnRolePermAvailable = new Button();
            button1 = new Button();
            button2 = new Button();
            dataGridView1 = new DataGridView();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridView2 = new DataGridView();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            label1 = new Label();
            btnRegistUser = new Button();
            gridUser = new DataGridView();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            dataGridViewButtonColumn1 = new DataGridViewButtonColumn();
            dataGridViewButtonColumn2 = new DataGridViewButtonColumn();
            label2 = new Label();
            txtUserId = new TextBox();
            comboBox1 = new ComboBox();
            label4 = new Label();
            button3 = new Button();
            tabControl.SuspendLayout();
            basicPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridPerm).BeginInit();
            combinedPermissionsTab.SuspendLayout();
            userPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRole).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermSelected).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermAvailable).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
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
            basicPermissionsTab.Size = new Size(886, 568);
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
            gridPerm.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPerm.Columns.AddRange(new DataGridViewColumn[] { ColumnPermId, ColumnPermName, ColumnPermDeleteButton });
            gridPerm.Location = new Point(13, 58);
            gridPerm.Name = "gridPerm";
            gridPerm.Size = new Size(493, 500);
            gridPerm.TabIndex = 10;
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
            combinedPermissionsTab.Controls.Add(label3);
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
            // userPermissionsTab
            // 
            userPermissionsTab.Controls.Add(button3);
            userPermissionsTab.Controls.Add(label4);
            userPermissionsTab.Controls.Add(comboBox1);
            userPermissionsTab.Controls.Add(button1);
            userPermissionsTab.Controls.Add(button2);
            userPermissionsTab.Controls.Add(dataGridView1);
            userPermissionsTab.Controls.Add(dataGridView2);
            userPermissionsTab.Controls.Add(label1);
            userPermissionsTab.Controls.Add(btnRegistUser);
            userPermissionsTab.Controls.Add(gridUser);
            userPermissionsTab.Controls.Add(label2);
            userPermissionsTab.Controls.Add(txtUserId);
            userPermissionsTab.Location = new Point(4, 33);
            userPermissionsTab.Name = "userPermissionsTab";
            userPermissionsTab.Size = new Size(886, 635);
            userPermissionsTab.TabIndex = 2;
            userPermissionsTab.Text = "ユーザー権限";
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
            gridRole.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRole.Columns.AddRange(new DataGridViewColumn[] { ColumnRoleId, ColumnRoleName, ColumnRoleEditButton, ColumnRoleDeleteButton });
            gridRole.Location = new Point(11, 50);
            gridRole.Name = "gridRole";
            gridRole.RowHeadersWidth = 20;
            gridRole.Size = new Size(864, 149);
            gridRole.TabIndex = 14;
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
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 215);
            label3.Name = "label3";
            label3.Size = new Size(138, 24);
            label3.TabIndex = 16;
            label3.Text = "選択中のロール：";
            // 
            // gridRolePermSelected
            // 
            gridRolePermSelected.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRolePermSelected.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            gridRolePermSelected.Location = new Point(11, 246);
            gridRolePermSelected.Name = "gridRolePermSelected";
            gridRolePermSelected.RowHeadersWidth = 20;
            gridRolePermSelected.Size = new Size(383, 312);
            gridRolePermSelected.TabIndex = 17;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 250;
            // 
            // gridRolePermAvailable
            // 
            gridRolePermAvailable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRolePermAvailable.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4 });
            gridRolePermAvailable.Location = new Point(492, 246);
            gridRolePermAvailable.Name = "gridRolePermAvailable";
            gridRolePermAvailable.RowHeadersWidth = 20;
            gridRolePermAvailable.Size = new Size(383, 312);
            gridRolePermAvailable.TabIndex = 18;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "ID";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Name";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 250;
            // 
            // btnRolePermSelected
            // 
            btnRolePermSelected.Location = new Point(402, 307);
            btnRolePermSelected.Name = "btnRolePermSelected";
            btnRolePermSelected.Size = new Size(84, 72);
            btnRolePermSelected.TabIndex = 19;
            btnRolePermSelected.Text = "＜＜";
            btnRolePermSelected.UseVisualStyleBackColor = true;
            // 
            // btnRolePermAvailable
            // 
            btnRolePermAvailable.Location = new Point(402, 385);
            btnRolePermAvailable.Name = "btnRolePermAvailable";
            btnRolePermAvailable.Size = new Size(84, 72);
            btnRolePermAvailable.TabIndex = 20;
            btnRolePermAvailable.Text = "＞＞";
            btnRolePermAvailable.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(400, 459);
            button1.Name = "button1";
            button1.Size = new Size(84, 72);
            button1.TabIndex = 29;
            button1.Text = "＞＞";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(400, 381);
            button2.Name = "button2";
            button2.Size = new Size(84, 72);
            button2.TabIndex = 28;
            button2.Text = "＜＜";
            button2.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6 });
            dataGridView1.Location = new Point(492, 289);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 20;
            dataGridView1.Size = new Size(383, 338);
            dataGridView1.TabIndex = 27;
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
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8 });
            dataGridView2.Location = new Point(11, 289);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 20;
            dataGridView2.Size = new Size(383, 338);
            dataGridView2.TabIndex = 26;
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 214);
            label1.Name = "label1";
            label1.Size = new Size(154, 24);
            label1.TabIndex = 25;
            label1.Text = "選択中のユーザー：";
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
            gridUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUser.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10, dataGridViewButtonColumn1, dataGridViewButtonColumn2 });
            gridUser.Location = new Point(11, 49);
            gridUser.Name = "gridUser";
            gridUser.RowHeadersWidth = 20;
            gridUser.Size = new Size(864, 149);
            gridUser.TabIndex = 23;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.HeaderText = "ID";
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.Width = 80;
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.HeaderText = "Name";
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            dataGridViewTextBoxColumn10.Width = 250;
            // 
            // dataGridViewButtonColumn1
            // 
            dataGridViewButtonColumn1.HeaderText = "";
            dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            dataGridViewButtonColumn1.Text = "Edit";
            dataGridViewButtonColumn1.UseColumnTextForButtonValue = true;
            dataGridViewButtonColumn1.Width = 80;
            // 
            // dataGridViewButtonColumn2
            // 
            dataGridViewButtonColumn2.HeaderText = "";
            dataGridViewButtonColumn2.Name = "dataGridViewButtonColumn2";
            dataGridViewButtonColumn2.Text = "Delete";
            dataGridViewButtonColumn2.UseColumnTextForButtonValue = true;
            dataGridViewButtonColumn2.Width = 80;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 15);
            label2.Name = "label2";
            label2.Size = new Size(108, 24);
            label2.TabIndex = 22;
            label2.Text = "ユーザーID：";
            // 
            // txtUserId
            // 
            txtUserId.Location = new Point(117, 12);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(256, 31);
            txtUserId.TabIndex = 21;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(75, 246);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(241, 32);
            comboBox1.TabIndex = 30;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 250);
            label4.Name = "label4";
            label4.Size = new Size(58, 24);
            label4.TabIndex = 31;
            label4.Text = "役割：";
            // 
            // button3
            // 
            button3.Location = new Point(322, 245);
            button3.Name = "button3";
            button3.Size = new Size(120, 33);
            button3.TabIndex = 32;
            button3.Text = "適用";
            button3.UseVisualStyleBackColor = true;
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
            userPermissionsTab.ResumeLayout(false);
            userPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridRole).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermSelected).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRolePermAvailable).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
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
        private Label label3;
        private DataGridView gridRolePermSelected;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridView gridRolePermAvailable;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Button btnRolePermAvailable;
        private Button btnRolePermSelected;
        private Button button1;
        private Button button2;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridView dataGridView2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private Label label1;
        private Button btnRegistUser;
        private DataGridView gridUser;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewButtonColumn dataGridViewButtonColumn1;
        private DataGridViewButtonColumn dataGridViewButtonColumn2;
        private Label label2;
        private TextBox txtUserId;
        private ComboBox comboBox1;
        private Label label4;
        private Button button3;
    }
}