using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.UserControls
{
    public partial class ToastNotification : UserControl
    {
        private System.Windows.Forms.Timer _fadeTimer;
        private System.Windows.Forms.Timer _timeBarTimer;
        private float _opacity = 1.0f;
        private const float FadeStep = 0.05f;

        private ProgressBar _timeBar;
        private int _totalDisplayTime;
        private int _elapsedTime = 0;
        private const int TimeBarUpdateInterval = 50; // 50ミリ秒ごとに更新

        public ToastNotification()
        {
            InitializeComponent();
            SetupControl();
        }

        private void SetupControl()
        {
            // コントロールの基本設定
            AutoSize = true;
            BackColor = Color.FromArgb(50, 50, 50);
            ForeColor = Color.White;
            Padding = new Padding(15);
            BorderStyle = BorderStyle.FixedSingle;

            // フェードアウトタイマーの設定
            _fadeTimer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };
            _fadeTimer.Tick += FadeTimer_Tick;

            // タイムバー更新用のタイマー設定
            _timeBarTimer = new System.Windows.Forms.Timer
            {
                Interval = TimeBarUpdateInterval
            };
            _timeBarTimer.Tick += TimeBarTimer_Tick;

            // タイムバーの初期化
            _timeBar = new ProgressBar
            {
                Name = "timeBar",
                Height = 5,
                Dock = DockStyle.Bottom,
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.FromArgb(70, 70, 70),
                ForeColor = Color.FromArgb(0, 122, 204), // 青色のプログレスバー
                Value = 100,
                Margin = new Padding(0, 10, 0, 0)
            };

            Controls.Add(_timeBar);
        }

        // 通知を表示するためのメソッド
        public void ShowNotification(string message, int displayTime = 3000)
        {
            _totalDisplayTime = displayTime;
            _elapsedTime = 0;

            // ラベルがない場合は作成
            if (!Controls.ContainsKey("notificationLabel"))
            {
                Label notificationLabel = new Label
                {
                    Name = "notificationLabel",
                    AutoSize = true,
                    Text = message,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                    Dock = DockStyle.Top,
                    Padding = new Padding(0, 0, 0, 10)
                };
                Controls.Add(notificationLabel);
            }
            else
            {
                ((Label)Controls["notificationLabel"]).Text = message;
            }

            // タイムバーをリセット
            _timeBar.Value = 100;

            // コントロールを表示
            Visible = true;
            BringToFront();
            _opacity = 1.0f;
            //Opacity = _opacity;

            // タイムバータイマーを開始
            _timeBarTimer.Start();

            // 指定時間後にフェードアウト開始
            System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer
            {
                Interval = displayTime
            };
            displayTimer.Tick += (s, e) =>
            {
                displayTimer.Stop();
                displayTimer.Dispose();
                _timeBarTimer.Stop();
                _fadeTimer.Start();
            };
            displayTimer.Start();
        }

        private void TimeBarTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += TimeBarUpdateInterval;
            int percentRemaining = (int)(100 - (_elapsedTime * 100.0 / _totalDisplayTime));

            if (percentRemaining < 0)
                percentRemaining = 0;

            _timeBar.Value = percentRemaining;
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            _opacity -= FadeStep;

            if (_opacity <= 0)
            {
                _fadeTimer.Stop();
                Visible = false;
                return;
            }

            //Opacity = _opacity;
        }

        // リソース解放
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                _fadeTimer?.Dispose();
                _timeBarTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
