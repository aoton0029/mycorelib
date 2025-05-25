using CoreLibWinforms.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core
{
    public class NotificationService
    {
        private readonly Form _parentForm;
        private ToastNotification _toastNotification;

        public NotificationService(Form parentForm)
        {
            _parentForm = parentForm;
            InitializeToastControl();
        }

        private void InitializeToastControl()
        {
            _toastNotification = new ToastNotification
            {
                Visible = false,
                // 右下に配置
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                AutoSize = true
            };

            _parentForm.Controls.Add(_toastNotification);

            // フォームサイズ変更時に位置を調整
            _parentForm.Resize += (sender, e) => UpdateToastPosition();
            UpdateToastPosition();
        }

        private void UpdateToastPosition()
        {
            // 右下に配置
            _toastNotification.Location = new System.Drawing.Point(
                _parentForm.ClientSize.Width - _toastNotification.Width - 20,
                _parentForm.ClientSize.Height - _toastNotification.Height - 20);
        }

        public void ShowNotification(string message, int displayTime = 3000)
        {
            if (_parentForm.InvokeRequired)
            {
                _parentForm.Invoke(new Action(() => ShowNotification(message, displayTime)));
                return;
            }

            _toastNotification.ShowNotification(message, displayTime);
            UpdateToastPosition();
        }
    }
}
