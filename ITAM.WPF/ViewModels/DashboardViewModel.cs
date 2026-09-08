using ITAM.WPF.Constants;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels
{
    public partial class DashboardViewModel:BaseViewModel
    {
        public override string Title => PageTitles.Dashboard;
        public DashboardViewModel(INavigationService navigationService) : base(navigationService) {
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
        }
    }
}
