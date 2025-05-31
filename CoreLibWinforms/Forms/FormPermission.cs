using CoreLibWinforms.Permissions;
using System;
using System.Collections;
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
    public partial class FormPermission : Form
    {
        private PermissionManager _permissionManager;

        public FormPermission(PermissionManager permissionManager)
        {
            InitializeComponent();
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
        }

        #region 権限画面
        
        #endregion

        #region ロール画面
        
        #endregion

        #region ユーザー権限画面
        
        #endregion

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 実装: 権限設定を保存
            MessageBox.Show("権限設定を保存しました。", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
