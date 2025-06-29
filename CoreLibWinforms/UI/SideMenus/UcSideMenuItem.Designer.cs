namespace CoreLibWinforms
{
    partial class UcSideMenuItem
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
            btnItem = new Button();
            SuspendLayout();
            // 
            // btnItem
            // 
            btnItem.Dock = DockStyle.Fill;
            btnItem.FlatAppearance.BorderSize = 0;
            btnItem.FlatStyle = FlatStyle.Flat;
            btnItem.ForeColor = Color.White;
            btnItem.Location = new Point(0, 0);
            btnItem.Name = "btnItem";
            btnItem.Size = new Size(200, 26);
            btnItem.TabIndex = 1;
            btnItem.Text = "TItle";
            btnItem.TextAlign = ContentAlignment.MiddleLeft;
            btnItem.UseVisualStyleBackColor = true;
            btnItem.Click += btnItem_Click;
            // 
            // UcSideMenuItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            Controls.Add(btnItem);
            Name = "UcSideMenuItem";
            Size = new Size(200, 26);
            ResumeLayout(false);
        }

        #endregion

        private Button btnItem;
    }
}
