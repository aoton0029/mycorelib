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
            combinedPermissionsTab = new TabPage();
            userPermissionsTab = new TabPage();
            btnCancel = new Button();
            btnSave = new Button();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewButtonColumn1 = new DataGridViewButtonColumn();
            label1 = new Label();
            textBox1 = new TextBox();
            tabControl.SuspendLayout();
            combinedPermissionsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
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
            tabControl.Size = new Size(894, 593);
            tabControl.TabIndex = 1;
            // 
            // basicPermissionsTab
            // 
            basicPermissionsTab.Location = new Point(4, 33);
            basicPermissionsTab.Name = "basicPermissionsTab";
            basicPermissionsTab.Size = new Size(886, 556);
            basicPermissionsTab.TabIndex = 0;
            basicPermissionsTab.Text = "基本権限";
            // 
            // combinedPermissionsTab
            // 
            combinedPermissionsTab.Controls.Add(button1);
            combinedPermissionsTab.Controls.Add(dataGridView1);
            combinedPermissionsTab.Controls.Add(label1);
            combinedPermissionsTab.Controls.Add(textBox1);
            combinedPermissionsTab.Location = new Point(4, 33);
            combinedPermissionsTab.Name = "combinedPermissionsTab";
            combinedPermissionsTab.Size = new Size(886, 556);
            combinedPermissionsTab.TabIndex = 1;
            combinedPermissionsTab.Text = "役割権限";
            // 
            // userPermissionsTab
            // 
            userPermissionsTab.Location = new Point(4, 33);
            userPermissionsTab.Name = "userPermissionsTab";
            userPermissionsTab.Size = new Size(886, 556);
            userPermissionsTab.TabIndex = 2;
            userPermissionsTab.Text = "ユーザー権限";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Font = new Font("メイリオ", 14.25F);
            btnCancel.Location = new Point(4, 597);
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
            btnSave.Location = new Point(740, 597);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(158, 49);
            btnSave.TabIndex = 3;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(336, 16);
            button1.Name = "button1";
            button1.Size = new Size(89, 33);
            button1.TabIndex = 7;
            button1.Text = "追加";
            button1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewButtonColumn1 });
            dataGridView1.Location = new Point(17, 63);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(408, 470);
            dataGridView1.TabIndex = 6;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Name";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Width = 200;
            // 
            // dataGridViewButtonColumn1
            // 
            dataGridViewButtonColumn1.HeaderText = "";
            dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            dataGridViewButtonColumn1.Text = "Delete";
            dataGridViewButtonColumn1.UseColumnTextForButtonValue = true;
            dataGridViewButtonColumn1.Width = 80;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 21);
            label1.Name = "label1";
            label1.Size = new Size(58, 24);
            label1.TabIndex = 5;
            label1.Text = "権限名";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(81, 18);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(249, 31);
            textBox1.TabIndex = 4;
            // 
            // FormPermission
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(902, 649);
            Controls.Add(btnSave);
            Controls.Add(btnCancel);
            Controls.Add(tabControl);
            Name = "FormPermission";
            Text = "FormPermission";
            tabControl.ResumeLayout(false);
            combinedPermissionsTab.ResumeLayout(false);
            combinedPermissionsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage basicPermissionsTab;
        private TabPage combinedPermissionsTab;
        private TabPage userPermissionsTab;
        private Button btnCancel;
        private Button btnSave;
        private Button button1;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewButtonColumn dataGridViewButtonColumn1;
        private Label label1;
        private TextBox textBox1;
    }
}