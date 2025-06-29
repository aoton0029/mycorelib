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
    public partial class UcSideMenuRoot : UserControl
    {
        public static int DefaultWidthCollapse { get; } = 50;
        public static int DefaultWidthExpand { get; } = 200;
        public static int DefaultItemHeight { get; } = 40;

        public UcSideMenuRoot()
        {
            InitializeComponent();
            this.Dock = DockStyle.Left;
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if(this.Width > DefaultWidthCollapse)
            {
                this.Width = DefaultWidthCollapse;
            }
            else
            {
                this.Width = DefaultWidthExpand;
            }
        }

        public void FromSideMenu(SideMenuRoot root)
        {
            flp.Controls.Clear();
            foreach (var child in root.HierarchicalChildren)
            {
                if (child is SideMenuCategory category)
                {
                    var catCtrl = new UcSideMenuCategory(category);
                    flp.Controls.Add(catCtrl);
                }
                else if (child is SideMenuItem item)
                {
                    var itemCtrl = new UcSideMenuItem(item);
                    flp.Controls.Add(itemCtrl);
                }
            }
        }

    }
}
