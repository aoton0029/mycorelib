using CoreLibWinforms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    /// <summary>
    /// 進捗状況レポーター
    /// </summary>
    public class ProgressReporter
    {
        private readonly FormLoading _form;

        internal ProgressReporter(FormLoading form)
        {
            _form = form;
        }

        /// <summary>
        /// 進捗状況を報告
        /// </summary>
        /// <param name="percentComplete">完了率（0-100）</param>
        public void ReportProgress(int percentComplete)
        {
            if (_form.IsDisposed) return;

            _form.Invoke((MethodInvoker)delegate {
                _form.UpdateProgress(percentComplete);
            });
        }

        /// <summary>
        /// キャンセルされたかどうかを取得
        /// </summary>
        public bool IsCancellationRequested => _form._cts.IsCancellationRequested;
    }
}
