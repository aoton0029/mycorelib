using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoreLibWinforms.Validations
{
    internal class _Sample
    {
        //public class UserModel
        //{
        //    [Required(ErrorMessage = "名前は必須です")]
        //    [StringLength(50, MinimumLength = 3, ErrorMessage = "名前は3〜50文字で入力してください")]
        //    public string Name { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "メールアドレスは必須です")]
        //    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
        //    public string Email { get; set; } = string.Empty;

        //    [Range(18, 100, ErrorMessage = "年齢は18〜100歳の範囲で入力してください")]
        //    public int Age { get; set; }
        //}

        //public partial class UserForm : Form
        //{
        //    private UserModel _user = new UserModel();
        //    private ModelValidator<UserModel> _validator;

        //    public UserForm()
        //    {
        //        InitializeComponent();

        //        // モデルバインディングを設定
        //        _validator = this.BindModel(_user);
        //        _validator.BindTextBox(u => u.Name, txtName);
        //        _validator.BindTextBox(u => u.Email, txtEmail);
        //        _validator.BindProperty(u => u.Age, numericAge);

        //        // 追加のカスタムバリデーション
        //        var errorProvider = this.GetErrorProvider();
        //        errorProvider.AddValidationRule(txtName, new CustomValidationRule(
        //            value => !value?.ToString()?.Contains("admin") ?? true,
        //            "名前に 'admin' を含めることはできません"));

        //        // 保存ボタンのクリックイベント
        //        btnSave.Click += (s, e) =>
        //        {
        //            if (_validator.ValidateAll())
        //            {
        //                MessageBox.Show("保存しました！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //            else
        //            {
        //                MessageBox.Show("入力エラーがあります。修正してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        };
        //    }

        //    // その他、フォームのコードは省略
        //}
    }
}
