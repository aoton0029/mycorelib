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
    public partial class FormLoadingBW : Form
    {
        private BackgroundWorker _worker;
        private string _message;
        private bool _canCancel;
        private Action<BackgroundWorker, DoWorkEventArgs> _workAction;

        public FormLoadingBW()
        {
            InitializeComponent();
        }

        /// <summary>
        /// BackgroundWorkerを使用する処理を実行するためのコンストラクタ
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="canCancel">キャンセル可能かどうか</param>
        /// <param name="workAction">バックグラウンドで実行する処理</param>
        public FormLoadingBW(string message, bool canCancel, Action<BackgroundWorker, DoWorkEventArgs> workAction)
        {
            InitializeComponent();
            _message = message;
            _canCancel = canCancel;
            _workAction = workAction;

            InitializeBackgroundWorker();
            SetupUI();
        }

        private void InitializeBackgroundWorker()
        {
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = _canCancel
            };

            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void SetupUI()
        {
            lblMsg.Text = _message;
            btnCancel.Visible = _canCancel;
            btnCancel.Enabled = _canCancel;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            lblProgress.Text = "0%";
        }

        /// <summary>
        /// 進捗パーセント表示を持たないマーキースタイルの設定を適用します
        /// </summary>
        private void UseIndeterminateProgress()
        {
            // UIスレッドで実行する必要がある
            if (InvokeRequired)
            {
                Invoke(new Action(UseIndeterminateProgress));
                return;
            }

            // マーキースタイルに設定
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;

            // 進捗ラベルを非表示
            lblProgress.Visible = false;
        }

        /// <summary>
        /// フォームがロードされたときにバックグラウンド処理を開始
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 設定されたワークアクションを実行
            _workAction?.Invoke(_worker, e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // マーキースタイルの場合は進捗表示を更新しない
            if (progressBar1.Style == ProgressBarStyle.Blocks)
            {
                // 進捗を更新
                progressBar1.Value = e.ProgressPercentage;
                lblProgress.Text = $"{e.ProgressPercentage}%";
            }

            // ユーザー状態がある場合はメッセージを更新
            if (e.UserState is string userMessage)
            {
                lblMsg.Text = userMessage;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 処理完了時はフォームを閉じる
            if (e.Error != null)
            {
                MessageBox.Show($"エラーが発生しました: {e.Error.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                lblMsg.Text = "処理はキャンセルされました。";
            }

            // 閉じるのを少し遅らせる（キャンセル時など表示確認のため）
            if (e.Cancelled)
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    Close();
                };
                timer.Start();
            }
            else
            {
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_worker != null && _worker.IsBusy && _worker.WorkerSupportsCancellation)
            {
                _worker.CancelAsync();
                btnCancel.Enabled = false;
                lblMsg.Text = "キャンセル中...";
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_worker != null && _worker.IsBusy)
            {
                // 閉じる前に処理をキャンセル
                _worker.CancelAsync();
            }
            base.OnFormClosing(e);
        }

        /// <summary>
        /// バックグラウンドワーカーを使用してタスクを実行し、ダイアログを表示します
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="canCancel">キャンセル可能かどうか</param>
        /// <param name="workAction">バックグラウンドで実行する処理</param>
        /// <returns>処理がキャンセルされたかどうか</returns>
        public static bool ShowDialog(string message, bool canCancel, Action<BackgroundWorker, DoWorkEventArgs> workAction)
        {
            bool cancelled = false;
            using (var form = new FormLoadingBW(message, canCancel, (worker, e) =>
            {
                try
                {
                    workAction(worker, e);
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                    e.Cancel = true;
                    cancelled = true;
                }
            }))
            {
                form.ShowDialog();
            }
            return cancelled;
        }

        /// <summary>
        /// バックグラウンドワーカーを使用してタスクを実行し、進捗表示なしのマーキースタイルでダイアログを表示します
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="canCancel">キャンセル可能かどうか</param>
        /// <param name="workAction">バックグラウンドで実行する処理</param>
        /// <returns>処理がキャンセルされたかどうか</returns>
        public static bool ShowIndeterminateDialog(string message, bool canCancel, Action<BackgroundWorker, DoWorkEventArgs> workAction)
        {
            bool cancelled = false;
            using (var form = new FormLoadingBW(message, canCancel, (worker, e) =>
            {
                try
                {
                    workAction(worker, e);
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                    e.Cancel = true;
                    cancelled = true;
                }
            }))
            {
                // マーキースタイルに設定し、進捗表示を非表示にする
                form.UseIndeterminateProgress();
                form.ShowDialog();
            }
            return cancelled;
        }
    }
}
