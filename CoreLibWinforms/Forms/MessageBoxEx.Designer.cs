namespace CoreLibWinforms.Forms
{
    partial class MessageBoxEx
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
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnRetry = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // picIcon
            // 
            this.picIcon.Location = new System.Drawing.Point(20, 20);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(32, 32);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcon.TabIndex = 0;
            this.picIcon.TabStop = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(70, 20);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(350, 60);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "メッセージ";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(160, 15);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(260, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point(60, 15);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(90, 30);
            this.btnYes.TabIndex = 4;
            this.btnYes.Text = "はい";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(160, 15);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(90, 30);
            this.btnNo.TabIndex = 5;
            this.btnNo.Text = "いいえ";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(160, 15);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(90, 30);
            this.btnRetry.TabIndex = 6;
            this.btnRetry.Text = "再試行";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(60, 15);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(90, 30);
            this.btnAbort.TabIndex = 7;
            this.btnAbort.Text = "中止";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(260, 15);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(90, 30);
            this.btnIgnore.TabIndex = 8;
            this.btnIgnore.Text = "無視";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(60, 15);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(90, 30);
            this.btnContinue.TabIndex = 9;
            this.btnContinue.Text = "続行";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnOk);
            this.panelButtons.Controls.Add(this.btnCancel);
            this.panelButtons.Controls.Add(this.btnYes);
            this.panelButtons.Controls.Add(this.btnNo);
            this.panelButtons.Controls.Add(this.btnRetry);
            this.panelButtons.Controls.Add(this.btnAbort);
            this.panelButtons.Controls.Add(this.btnIgnore);
            this.panelButtons.Controls.Add(this.btnContinue);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 100);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(440, 60);
            this.panelButtons.TabIndex = 10;
            // 
            // MessageBoxEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(440, 160);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.picIcon);
            this.Name = "MessageBoxEx";
            this.Text = "メッセージ";
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox picIcon;
        private Label lblMessage;
        private Button btnOk;
        private Button btnCancel;
        private Button btnYes;
        private Button btnNo;
        private Button btnRetry;
        private Button btnAbort;
        private Button btnIgnore;
        private Button btnContinue;
        private Panel panelButtons;
    }
}