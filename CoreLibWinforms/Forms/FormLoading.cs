using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.Forms
{
    public partial class FormLoading : Form
    {
        internal readonly CancellationTokenSource _cts;

        public FormLoading(string msg, bool aCanCancel, CancellationTokenSource cts = null)
        {
            InitializeComponent();
            _cts = cts ?? new CancellationTokenSource();

            lblMsg.Text = msg;
            if (aCanCancel)
            {
                btnCancel.Visible = true;
                btnCancel.Enabled = true;
                btnCancel.Click += (s, e) =>
                {
                    _cts.Cancel();
                    btnCancel.Enabled = false;
                };
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
            else
            {
                btnCancel.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        public void UpdateProgress(int progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    progressBar1.Value = progress;
                    lblProgress.Text = progress.ToString();
                }));
            }
            else
            {
                progressBar1.Value = progress;
                lblProgress.Text = progress.ToString();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
