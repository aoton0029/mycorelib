using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreLibWinforms
{
    public partial class UcSideMenuCategory : UserControl
    {
        private readonly SideMenuCategory _category;

        public UcSideMenuCategory(SideMenuCategory category)
        {
            InitializeComponent();
            _category = category;

            btnExpand.Text = category.Title;

            flpChildren.Controls.Clear();
            foreach (var child in category.HierarchicalChildren)
            {
                if (child is SideMenuCategory subCategory)
                {
                    var subCatCtrl = new UcSideMenuCategory(subCategory);
                    flpChildren.Controls.Add(subCatCtrl);
                }
                else if (child is SideMenuItem item)
                {
                    var itemCtrl = new UcSideMenuItem(item);
                    flpChildren.Controls.Add(itemCtrl);
                }
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (flpChildren.Visible)
            {
                flpChildren.Visible = false;
                btnExpand.Text = _category.Title; // Show only the title
            }
            else
            {
                flpChildren.Visible = true;
                btnExpand.Text = _category.Title + " ▼"; // Indicate expanded state
            }
        }
    }
}
