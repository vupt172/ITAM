using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainViewModel : BaseViewModel
    {
        public override string Title => PageTitles.Default;

        public INavigationService NavigationService { get; }
        public MenuBarViewModel MenuBarViewModel { get; }

        public MainViewModel(INavigationService navigationService, MenuBarViewModel menuBarViewModel)
        {
            NavigationService = navigationService;
            MenuBarViewModel = menuBarViewModel;
            NavigationService.CurrentTitle= Title;
        }
    }
}
