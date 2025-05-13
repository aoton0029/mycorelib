namespace CoreLibWinforms.Forms
{
    partial class FormLoadingBW
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
            lblProgress = new Label();
            progressBar1 = new ProgressBar();
            lblMsg = new Label();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblProgress.Font = new Font("メイリオ", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblProgress.Location = new Point(12, 156);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(377, 35);
            lblProgress.TabIndex = 7;
            lblProgress.Text = "0%";
            lblProgress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 125);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(377, 28);
            progressBar1.TabIndex = 6;
            // 
            // lblMsg
            // 
            lblMsg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblMsg.Font = new Font("メイリオ", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblMsg.Location = new Point(12, 7);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(377, 115);
            lblMsg.TabIndex = 5;
            lblMsg.Text = "label1";
            lblMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("メイリオ", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnCancel.Location = new Point(109, 202);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(182, 48);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // FormLoadingBW
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(401, 256);
            ControlBox = false;
            Controls.Add(lblProgress);
            Controls.Add(progressBar1);
            Controls.Add(lblMsg);
            Controls.Add(btnCancel);
            Name = "FormLoadingBW";
            Text = "FormLoadingBW";
            ResumeLayout(false);
        }

        #endregion

        private Label lblProgress;
        private ProgressBar progressBar1;
        private Label lblMsg;
        private Button btnCancel;
    }
}