namespace CoreLibWinforms.Forms
{
    partial class DeveloperErrorMessageBox
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
            this.lblUserMessage = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.lblErrorCode = new System.Windows.Forms.Label();
            this.btnToggleDetails = new System.Windows.Forms.Button();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.lblDetailsHeader = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.pnlDetails.SuspendLayout();
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
            // lblUserMessage
            // 
            this.lblUserMessage.AutoSize = true;
            this.lblUserMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblUserMessage.Location = new System.Drawing.Point(70, 20);
            this.lblUserMessage.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblUserMessage.Name = "lblUserMessage";
            this.lblUserMessage.Size = new System.Drawing.Size(107, 20);
            this.lblUserMessage.TabIndex = 1;
            this.lblUserMessage.Text = "Error Message";
            // 
            // lblTimestamp
            // 
            this.lblTimestamp.AutoSize = true;
            this.lblTimestamp.Location = new System.Drawing.Point(70, 50);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(165, 20);
            this.lblTimestamp.TabIndex = 2;
            this.lblTimestamp.Text = "Error occurred at: [Time]";
            // 
            // lblErrorCode
            // 
            this.lblErrorCode.AutoSize = true;
            this.lblErrorCode.Location = new System.Drawing.Point(70, 70);
            this.lblErrorCode.Name = "lblErrorCode";
            this.lblErrorCode.Size = new System.Drawing.Size(126, 20);
            this.lblErrorCode.TabIndex = 3;
            this.lblErrorCode.Text = "Error code: [Code]";
            // 
            // btnToggleDetails
            // 
            this.btnToggleDetails.Location = new System.Drawing.Point(70, 100);
            this.btnToggleDetails.Name = "btnToggleDetails";
            this.btnToggleDetails.Size = new System.Drawing.Size(150, 30);
            this.btnToggleDetails.TabIndex = 4;
            this.btnToggleDetails.Text = "Show Details";
            this.btnToggleDetails.UseVisualStyleBackColor = true;
            this.btnToggleDetails.Click += new System.EventHandler(this.btnToggleDetails_Click);
            // 
            // pnlDetails
            // 
            this.pnlDetails.Controls.Add(this.txtDetails);
            this.pnlDetails.Controls.Add(this.lblDetailsHeader);
            this.pnlDetails.Controls.Add(this.btnCopy);
            this.pnlDetails.Location = new System.Drawing.Point(20, 145);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(480, 250);
            this.pnlDetails.TabIndex = 5;
            // 
            // txtDetails
            // 
            this.txtDetails.BackColor = System.Drawing.SystemColors.Window;
            this.txtDetails.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtDetails.Location = new System.Drawing.Point(3, 30);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetails.Size = new System.Drawing.Size(474, 180);
            this.txtDetails.TabIndex = 0;
            // 
            // lblDetailsHeader
            // 
            this.lblDetailsHeader.AutoSize = true;
            this.lblDetailsHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDetailsHeader.Location = new System.Drawing.Point(3, 7);
            this.lblDetailsHeader.Name = "lblDetailsHeader";
            this.lblDetailsHeader.Size = new System.Drawing.Size(124, 20);
            this.lblDetailsHeader.TabIndex = 1;
            this.lblDetailsHeader.Text = "Technical Details";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(327, 213);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(150, 30);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(225, 408);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 30);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // DeveloperErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 200);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.btnToggleDetails);
            this.Controls.Add(this.lblErrorCode);
            this.Controls.Add(this.lblTimestamp);
            this.Controls.Add(this.lblUserMessage);
            this.Controls.Add(this.picIcon);
            this.Name = "DeveloperErrorMessageBox";
            this.Text = "Error Information";
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private PictureBox picIcon;
        private Label lblUserMessage;
        private Label lblTimestamp;
        private Label lblErrorCode;
        private Button btnToggleDetails;
        private Panel pnlDetails;
        private TextBox txtDetails;
        private Label lblDetailsHeader;
        private Button btnCopy;
        private Button btnOK;
    }
}