using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// コントロールへの権限適用に関する拡張メソッドを提供するクラス
    /// </summary>
    public static class ControlPermissionExtensions
    {
        /// <summary>
        /// コントロールに権限を適用します
        /// </summary>
        /// <param name="control">対象コントロール</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        /// <returns>同じコントロールを返し、メソッドチェーンを可能にします</returns>
        public static Control ApplyPermission(this Control control, string featureId, IPermission requiredPermission)
        {
            PermissionManager.Instance.ApplyPermissionToControl(control, featureId, requiredPermission);
            return control;
        }

        /// <summary>
        /// ToolStripItemに権限を適用します
        /// </summary>
        /// <param name="item">対象ToolStripItem</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        /// <returns>同じToolStripItemを返し、メソッドチェーンを可能にします</returns>
        public static ToolStripItem ApplyPermission(this ToolStripItem item, string featureId, IPermission requiredPermission)
        {
            PermissionManager.Instance.ApplyPermissionToControl(item, featureId, requiredPermission);
            return item;
        }

        /// <summary>
        /// コントロールコレクションに一括して権限を適用します
        /// </summary>
        /// <param name="controls">対象コントロールのコレクション</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        public static void ApplyPermissionToAll(this Control.ControlCollection controls, string featureId, IPermission requiredPermission)
        {
            foreach (Control control in controls)
            {
                control.ApplyPermission(featureId, requiredPermission);
            }
        }

        /// <summary>
        /// ToolStripItemコレクションに一括して権限を適用します
        /// </summary>
        /// <param name="items">対象ToolStripItemのコレクション</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        public static void ApplyPermissionToAll(this ToolStripItemCollection items, string featureId, IPermission requiredPermission)
        {
            foreach (ToolStripItem item in items)
            {
                item.ApplyPermission(featureId, requiredPermission);
            }
        }

        /// <summary>
        /// 指定したタグを持つコントロールにのみ権限を適用します
        /// </summary>
        /// <param name="controls">対象コントロールのコレクション</param>
        /// <param name="tag">適用対象のタグ</param>
        /// <param name="featureId">機能ID</param>
        /// <param name="requiredPermission">必要な権限</param>
        public static void ApplyPermissionByTag(this Control.ControlCollection controls, object tag, string featureId, IPermission requiredPermission)
        {
            foreach (Control control in controls)
            {
                if (control.Tag != null && control.Tag.Equals(tag))
                {
                    control.ApplyPermission(featureId, requiredPermission);
                }

                // 子コントロールも処理する
                if (control.Controls.Count > 0)
                {
                    control.Controls.ApplyPermissionByTag(tag, featureId, requiredPermission);
                }
            }
        }

        /// <summary>
        /// フォーム上のコントロールに権限設定を自動適用します
        /// コントロールのTagプロパティに "FeatureID:Permission" の形式で設定しておく必要があります
        /// </summary>
        /// <param name="form">対象フォーム</param>
        /// <param name="permissionResolver">権限文字列をIPermissionオブジェクトに解決する関数</param>
        public static void AutoApplyPermissions(this Form form, Func<string, IPermission> permissionResolver)
        {
            if (permissionResolver == null)
                throw new ArgumentNullException(nameof(permissionResolver));

            ApplyPermissionsByTag(form.Controls, permissionResolver);
        }

        /// <summary>
        /// 再帰的にコントロールのTagから権限設定を適用します
        /// </summary>
        /// <param name="controls">対象コントロールのコレクション</param>
        /// <param name="permissionResolver">権限文字列をIPermissionオブジェクトに解決する関数</param>
        private static void ApplyPermissionsByTag(Control.ControlCollection controls, Func<string, IPermission> permissionResolver)
        {
            foreach (Control control in controls)
            {
                // TagからFeatureIDと権限情報を抽出
                if (control.Tag is string tagString)
                {
                    var parts = tagString.Split(':');
                    if (parts.Length == 2)
                    {
                        string featureId = parts[0].Trim();
                        IPermission permission = permissionResolver(parts[1].Trim());

                        if (permission != null)
                        {
                            control.ApplyPermission(featureId, permission);
                        }
                    }
                }

                // 子コントロールにも再帰的に適用
                if (control.Controls.Count > 0)
                {
                    ApplyPermissionsByTag(control.Controls, permissionResolver);
                }

                // ToolStrip型なら内部アイテムにも適用
                if (control is ToolStrip toolStrip)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        if (item.Tag is string itemTagString)
                        {
                            var parts = itemTagString.Split(':');
                            if (parts.Length == 2)
                            {
                                string featureId = parts[0].Trim();
                                IPermission permission = permissionResolver(parts[1].Trim());

                                if (permission != null)
                                {
                                    item.ApplyPermission(featureId, permission);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
