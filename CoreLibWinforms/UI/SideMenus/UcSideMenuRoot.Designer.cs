namespace CoreLibWinforms
{
    partial class UcSideMenuRoot
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
            flp = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // btnExpand
            // 
            btnExpand.Dock = DockStyle.Top;
            btnExpand.Location = new Point(0, 0);
            btnExpand.Name = "btnExpand";
            btnExpand.Size = new Size(150, 29);
            btnExpand.TabIndex = 0;
            btnExpand.Text = "Ξ";
            btnExpand.UseVisualStyleBackColor = true;
            btnExpand.Click += btnExpand_Click;
            // 
            // flp
            // 
            flp.Dock = DockStyle.Fill;
            flp.Location = new Point(0, 29);
            flp.Name = "flp";
            flp.Size = new Size(150, 600);
            flp.TabIndex = 1;
            // 
            // UcSideMenuRoot
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            Controls.Add(flp);
            Controls.Add(btnExpand);
            Name = "UcSideMenuRoot";
            Size = new Size(150, 629);
            ResumeLayout(false);
        }

        #endregion

        private Button btnExpand;
        private FlowLayoutPanel flp;
    }
}
