using CoreLibWinforms.Navigations;
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
    public partial class UcPageBase : UserControl, IPage
    {
        private readonly NavigationService _navigationService;

        private UcPageBase()
        {
            InitializeComponent();
        }

        public UcPageBase(NavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public virtual void OnPageLeave(NavigationContext context)
        {

        }

        public virtual void OnPageShown(NavigationContext context)
        {

        }
    }
}
