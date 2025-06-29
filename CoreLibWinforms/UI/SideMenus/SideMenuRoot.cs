using CoreLib.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms
{
    // サイドメニューの共通基底クラス
    public abstract class SideMenuNode : ModifiableHierarchical
    {
        public string Title { get; }
        public Image? Icon { get; }
        public object? Tag { get; set; } = null;

        protected SideMenuNode(string title, Image? icon=null)
        {
            Title = title;
            Icon = icon;
        }
    }

    // カテゴリ（展開可能、子カテゴリやアイテムを持てる）
    public class SideMenuCategory : SideMenuNode
    {
        public SideMenuCategory(string title) : base(title)
        {
        }
    }

    // アイテム（クリックで操作を実行）
    public class SideMenuItem : SideMenuNode
    {
        public Action? Command { get; }

        public SideMenuItem(string title, Action? command = null) : base(title)
        {
            Command = command;
        }

        public void Execute()
        {
            Command?.Invoke();
        }
    }
    public class SideMenuRoot : ModifiableHierarchical, IHierarchicalRoot
    {
        public event EventHandler<IHierarchical>? DescendantAttached;
        public event EventHandler<IHierarchical>? DescendantDetached;

        public void OnDescendantAttached(IHierarchical descendant)
        {
            DescendantAttached?.Invoke(this, descendant);
        }

        public void OnDescendantDetached(IHierarchical descendant)
        {
            DescendantDetached?.Invoke(this, descendant);
        }
    }
}
