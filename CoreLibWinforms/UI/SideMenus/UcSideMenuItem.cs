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
    public partial class UcSideMenuItem : UserControl
    {
        private readonly SideMenuItem _item;

        public UcSideMenuItem(SideMenuItem item)
        {
            InitializeComponent();
            _item = item;
        }

        private void btnItem_Click(object sender, EventArgs e)
        {
            _item?.Execute();
        }
    }
}
