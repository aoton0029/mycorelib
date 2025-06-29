using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms._Win32
{
    public class ClipboardMonitor : IDisposable
    {
        // Windows APIの定義
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        // クリップボード内容が変更されたときに発生するイベント
        public event EventHandler<ClipboardChangedEventArgs> ClipboardChanged;

        private readonly NotificationForm _form;
        private bool _disposed = false;

        public ClipboardMonitor()
        {
            // 非表示のフォームを作成してメッセージを受け取る
            _form = new NotificationForm();
            _form.ClipboardUpdate += OnClipboardUpdate;

            // クリップボード監視を開始
            bool success = AddClipboardFormatListener(_form.Handle);
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        private void OnClipboardUpdate(object sender, EventArgs e)
        {
            // クリップボード内容の取得と通知
            IDataObject data = null;
            try
            {
                data = Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                // クリップボードにアクセスできない場合がある
                return;
            }

            // イベントを発火
            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(data));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing && _form != null)
            {
                // クリップボード監視を停止
                RemoveClipboardFormatListener(_form.Handle);
                _form.ClipboardUpdate -= OnClipboardUpdate;
                _form.Dispose();
            }

            _disposed = true;
        }

        // 非表示のメッセージ受信用フォーム
        private class NotificationForm : Form
        {
            public event EventHandler ClipboardUpdate;

            public NotificationForm()
            {
                // 見えないフォームにする
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
                FormBorderStyle = FormBorderStyle.None;
                Opacity = 0;
                Size = new System.Drawing.Size(0, 0);
            }

            protected override void WndProc(ref Message m)
            {
                // WM_CLIPBOARDUPDATEを処理
                if (m.Msg == WM_CLIPBOARDUPDATE)
                {
                    ClipboardUpdate?.Invoke(this, EventArgs.Empty);
                }
                base.WndProc(ref m);
            }
        }
    }

    // クリップボード変更イベント引数クラス
    public class ClipboardChangedEventArgs : EventArgs
    {
        public IDataObject DataObject { get; }

        public ClipboardChangedEventArgs(IDataObject dataObject)
        {
            DataObject = dataObject;
        }
    }
}
