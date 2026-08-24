using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ITAM.WPF.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainViewModel 
    {
        public  string Title => PageTitles.Default;
        public INavigationService NavigationService { get; }

        public MenuBarViewModel MenuBarViewModel { get; }
        public ToolbarViewModel ToolbarViewModel { get; }




        public MainViewModel(INavigationService navigationService, MenuBarViewModel menuBarViewModel, ToolbarViewModel toolbarViewModel)
        {
            NavigationService = navigationService;
            MenuBarViewModel = menuBarViewModel;
            ToolbarViewModel = toolbarViewModel;
        }

    }
}
