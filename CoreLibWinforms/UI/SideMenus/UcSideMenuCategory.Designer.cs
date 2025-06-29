namespace CoreLibWinforms
{
    partial class UcSideMenuCategory
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            btnExpand = new Button();
            flpChildren = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // btnExpand
            // 
            btnExpand.Dock = DockStyle.Top;
            btnExpand.FlatAppearance.BorderSize = 0;
            btnExpand.FlatStyle = FlatStyle.Flat;
            btnExpand.ForeColor = Color.White;
            btnExpand.Location = new Point(0, 0);
            btnExpand.Name = "btnExpand";
            btnExpand.Size = new Size(200, 29);
            btnExpand.TabIndex = 0;
            btnExpand.Text = "button1";
            btnExpand.TextAlign = ContentAlignment.MiddleLeft;
            btnExpand.UseVisualStyleBackColor = true;
            btnExpand.Click += btnExpand_Click;
            // 
            // flpChildren
            // 
            flpChildren.Dock = DockStyle.Fill;
            flpChildren.Location = new Point(0, 29);
            flpChildren.Name = "flpChildren";
            flpChildren.Size = new Size(200, 85);
            flpChildren.TabIndex = 1;
            // 
            // UcSideMenuCategory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            Controls.Add(flpChildren);
            Controls.Add(btnExpand);
            Name = "UcSideMenuCategory";
            Size = new Size(200, 114);
            ResumeLayout(false);
        }

        #endregion

        private Button btnExpand;
        private FlowLayoutPanel flpChildren;
    }
}
