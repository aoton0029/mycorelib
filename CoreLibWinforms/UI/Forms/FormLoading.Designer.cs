namespace CoreLibWinforms.Forms
{
    partial class FormLoading
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
            btnCancel = new Button();
            lblMsg = new Label();
            progressBar1 = new ProgressBar();
            lblProgress = new Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Font = new Font("メイリオ", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnCancel.Location = new Point(109, 204);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(182, 48);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "キャンセル";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblMsg
            // 
            lblMsg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblMsg.Font = new Font("メイリオ", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblMsg.Location = new Point(12, 9);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(377, 115);
            lblMsg.TabIndex = 1;
            lblMsg.Text = "label1";
            lblMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 127);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(377, 28);
            progressBar1.TabIndex = 2;
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblProgress.Font = new Font("メイリオ", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblProgress.Location = new Point(12, 158);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(377, 35);
            lblProgress.TabIndex = 3;
            lblProgress.Text = "0%";
            lblProgress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormLoading
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(401, 256);
            ControlBox = false;
            Controls.Add(lblProgress);
            Controls.Add(progressBar1);
            Controls.Add(lblMsg);
            Controls.Add(btnCancel);
            Name = "FormLoading";
            Text = "FormLoading";
            ResumeLayout(false);
        }

        #endregion

        private Button btnCancel;
        private Label lblMsg;
        private ProgressBar progressBar1;
        private Label lblProgress;
    }
}