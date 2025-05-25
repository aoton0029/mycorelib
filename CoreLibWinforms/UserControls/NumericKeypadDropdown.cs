using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms.UserControls
{
    public partial class NumericKeypadDropdown : DropdownBase
    {
        #region イベント

        /// <summary>
        /// 値が変更された時に発生するイベント
        /// </summary>
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        #endregion

        #region プロパティ

        private decimal _value = 0;
        /// <summary>
        /// 現在の入力値
        /// </summary>
        [Description("現在の入力値")]
        [Category("データ")]
        public decimal Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    decimal oldValue = _value;
                    _value = value;
                    UpdateDisplayText();
                    OnValueChanged(oldValue, _value);
                }
            }
        }

        private decimal _minimum = decimal.MinValue;
        /// <summary>
        /// 最小値
        /// </summary>
        [Description("最小値")]
        [Category("データ")]
        public decimal Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum)
                {
                    Value = _minimum;
                }
            }
        }

        private decimal _maximum = decimal.MaxValue;
        /// <summary>
        /// 最大値
        /// </summary>
        [Description("最大値")]
        [Category("データ")]
        public decimal Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum)
                {
                    Value = _maximum;
                }
            }
        }

        private int _decimalPlaces = 0;
        /// <summary>
        /// 小数点以下の桁数
        /// </summary>
        [Description("小数点以下の桁数")]
        [Category("データ")]
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                if (value < 0) value = 0;
                if (value > 28) value = 28; // decimal型の最大精度
                _decimalPlaces = value;
                UpdateDisplayText();
            }
        }

        private bool _allowNegative = true;
        /// <summary>
        /// 負の値を許可するかどうか
        /// </summary>
        [Description("負の値を許可するかどうか")]
        [Category("データ")]
        [DefaultValue(true)]
        public bool AllowNegative
        {
            get => _allowNegative;
            set
            {
                _allowNegative = value;
                if (!_allowNegative && _value < 0)
                {
                    Value = 0;
                }
            }
        }

        private bool _showThousandsSeparator = true;
        /// <summary>
        /// 桁区切りを表示するかどうか
        /// </summary>
        [Description("桁区切りを表示するかどうか")]
        [Category("表示")]
        [DefaultValue(true)]
        public bool ShowThousandsSeparator
        {
            get => _showThousandsSeparator;
            set
            {
                _showThousandsSeparator = value;
                UpdateDisplayText();
            }
        }
        private bool _isEditing = false;
        private string _inputBuffer = "0";
        private bool _decimalPressed = false;

        #endregion

        public NumericKeypadDropdown()
        {
            InitializeComponent();
            // ボタンの配置
            CreateButtons();

            // 初期表示更新
            UpdateDisplayText();
        }

        /// <summary>
        /// ボタンの作成と配置
        /// </summary>
        private void CreateButtons()
        {
            // 数字キー（7, 8, 9）
            AddButton("7", 0, 0, DigitButtonClick);
            AddButton("8", 0, 1, DigitButtonClick);
            AddButton("9", 0, 2, DigitButtonClick);

            // 数字キー（4, 5, 6）
            AddButton("4", 1, 0, DigitButtonClick);
            AddButton("5", 1, 1, DigitButtonClick);
            AddButton("6", 1, 2, DigitButtonClick);

            // 数字キー（1, 2, 3）
            AddButton("1", 2, 0, DigitButtonClick);
            AddButton("2", 2, 1, DigitButtonClick);
            AddButton("3", 2, 2, DigitButtonClick);

            // 数字キー（0, ., +/-）
            AddButton("0", 3, 0, DigitButtonClick);
            AddButton(".", 3, 1, DecimalButtonClick);
            AddButton("±", 3, 2, SignButtonClick);

            // コントロールボタン（BS, C, OK）
            AddButton("BS", 4, 0, BackSpaceButtonClick);
            AddButton("C", 4, 1, ClearButtonClick);

            // OKとキャンセルボタン
            Button btnOk = AddButton("OK", 4, 2, (s, e) => OnOKClicked());
            btnOk.BackColor = Color.LightGreen;
        }

        /// <summary>
        /// ボタンを追加
        /// </summary>
        private Button AddButton(string text, int row, int col, EventHandler clickHandler)
        {
            Button btn = new Button();
            btn.Dock = DockStyle.Fill;
            btn.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btn.Location = new Point(0, 0);
            btn.Margin = new Padding(2);
            btn.Name = $"btn{text}";
            btn.Size = new Size(70, 40);
            btn.TabIndex = row * 3 + col;
            btn.Text = text;
            btn.UseVisualStyleBackColor = true;
            btn.Click += clickHandler;

            tableLayoutPanel1.Controls.Add(btn, col, row);
            return btn;
        }
        #region イベントハンドラー

        /// <summary>
        /// 数字キー押下時の処理
        /// </summary>
        private void DigitButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Text;

            if (!_isEditing)
            {
                _inputBuffer = digit;
                _isEditing = true;
                _decimalPressed = false;
            }
            else
            {
                // 先頭が0の場合は置き換える（ただし小数点以下の場合を除く）
                if (_inputBuffer == "0" && !_decimalPressed)
                {
                    _inputBuffer = digit;
                }
                else
                {
                    _inputBuffer += digit;
                }
            }

            ParseAndUpdateValue();
        }

        /// <summary>
        /// 小数点キー押下時の処理
        /// </summary>
        private void DecimalButtonClick(object sender, EventArgs e)
        {
            // 小数点が許可されていない場合は何もしない
            if (_decimalPlaces <= 0)
                return;

            // 既に小数点が入力されている場合は何もしない
            if (_decimalPressed)
                return;

            if (!_isEditing)
            {
                _inputBuffer = "0.";
                _isEditing = true;
            }
            else
            {
                _inputBuffer += ".";
            }

            _decimalPressed = true;
            ParseAndUpdateValue();
        }

        /// <summary>
        /// 符号反転キー押下時の処理
        /// </summary>
        private void SignButtonClick(object sender, EventArgs e)
        {
            if (!_allowNegative)
                return;

            if (_isEditing)
            {
                // 入力中の場合は入力バッファの符号を反転
                if (_inputBuffer.StartsWith("-"))
                {
                    _inputBuffer = _inputBuffer.Substring(1);
                }
                else
                {
                    _inputBuffer = "-" + _inputBuffer;
                }
            }
            else
            {
                // 入力中でない場合は値の符号を反転
                _inputBuffer = (-_value).ToString(CultureInfo.InvariantCulture);
                _isEditing = true;
                _decimalPressed = _inputBuffer.Contains(".");
            }

            ParseAndUpdateValue();
        }

        /// <summary>
        /// バックスペースキー押下時の処理
        /// </summary>
        private void BackSpaceButtonClick(object sender, EventArgs e)
        {
            if (!_isEditing || _inputBuffer.Length <= 1)
            {
                _inputBuffer = "0";
                _isEditing = true;
                _decimalPressed = false;
            }
            else
            {
                // 最後の文字を削除
                char removedChar = _inputBuffer[_inputBuffer.Length - 1];
                _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);

                // 小数点を削除した場合
                if (removedChar == '.')
                {
                    _decimalPressed = false;
                }

                // 空になった場合は0に
                if (string.IsNullOrEmpty(_inputBuffer) || _inputBuffer == "-")
                {
                    _inputBuffer = "0";
                }
            }

            ParseAndUpdateValue();
        }

        /// <summary>
        /// クリアボタン押下時の処理
        /// </summary>
        private void ClearButtonClick(object sender, EventArgs e)
        {
            _inputBuffer = "0";
            _isEditing = true;
            _decimalPressed = false;
            ParseAndUpdateValue();
        }

        #endregion

        #region ヘルパーメソッド

        /// <summary>
        /// 入力バッファを解析して値を更新
        /// </summary>
        private void ParseAndUpdateValue()
        {
            if (decimal.TryParse(_inputBuffer, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedValue))
            {
                // 値の制限チェック
                if (parsedValue < _minimum)
                    parsedValue = _minimum;
                if (parsedValue > _maximum)
                    parsedValue = _maximum;

                // 小数点以下の桁数制限
                if (_decimalPlaces > 0)
                {
                    parsedValue = Math.Round(parsedValue, _decimalPlaces);
                }
                else
                {
                    parsedValue = Math.Floor(parsedValue);
                }

                // 値を更新
                decimal oldValue = _value;
                _value = parsedValue;
                UpdateDisplayText();

                // イベント発火
                OnValueChanged(oldValue, _value);
            }
            else
            {
                // 解析できない場合は入力バッファを以前の値に戻す
                _inputBuffer = _value.ToString(CultureInfo.InvariantCulture);
                UpdateDisplayText();
            }
        }

        /// <summary>
        /// 表示テキストを更新
        /// </summary>
        private void UpdateDisplayText()
        {
            string format = "0";

            // 小数点以下の形式
            if (_decimalPlaces > 0)
            {
                format += "." + new string('0', _decimalPlaces);
            }

            // 桁区切り
            if (_showThousandsSeparator)
            {
                format = "#," + format;
            }

            // 入力中の場合はそのまま表示、それ以外は書式設定
            //txtDisplay.Text = _isEditing ?
            //    _inputBuffer :
            //    _value.ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 値変更イベントの発行
        /// </summary>
        protected virtual void OnValueChanged(decimal oldValue, decimal newValue)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(oldValue, newValue));
        }

        /// <summary>
        /// OKボタンがクリックされた時の処理
        /// </summary>
        protected override void OnOKClicked()
        {
            // 入力中の値を確定
            if (_isEditing)
            {
                ParseAndUpdateValue();
                _isEditing = false;
            }

            base.OnOKClicked();
        }

        /// <summary>
        /// キャンセルボタンがクリックされた時の処理
        /// </summary>
        protected override void OnCancelClicked()
        {
            base.OnCancelClicked();
        }

        #endregion
    }

    /// <summary>
    /// 値変更イベントの引数クラス
    /// </summary>
    public class ValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 古い値
        /// </summary>
        public decimal OldValue { get; }

        /// <summary>
        /// 新しい値
        /// </summary>
        public decimal NewValue { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ValueChangedEventArgs(decimal oldValue, decimal newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
