using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAM.WPF.ViewModels
{
    public partial class MenuBarViewModel 
    {
        private readonly INavigationService _navigationService;
      

        public MenuBarViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        [RelayCommand]
        private void NavigateDashboard()
        {
            _navigationService.NavigateTo<DashboardViewModel>();
        }
        [RelayCommand]
        private void NavigatHeThongDM()
        {
            _navigationService.NavigateTo<HeThongDMViewModel>();
        }
    }
}
