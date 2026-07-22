using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.WPF.Constants;
using ITAM.WPF.Services;
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
        public ToolbarViewModel ToolbarViewModel { get; }

     

        public MainViewModel(INavigationService navigationService, MenuBarViewModel menuBarViewModel, ToolbarViewModel toolbarViewModel)
        {
            NavigationService = navigationService;
            MenuBarViewModel = menuBarViewModel;
            ToolbarViewModel = toolbarViewModel;
            ToolbarViewModel.Default();
        }


        //private void SetToolbar()
        //{
        //    ToolbarViewModel.AddCommand = AddCommand;
        //    ToolbarViewModel.EditCommand = EditCommand;
        //    ToolbarViewModel.DeleteCommand = DeleteCommand;
        //    ToolbarViewModel.SaveCommand=SaveCommand;
        //    ToolbarViewModel.CancelCommand=CancelCommand;
        //    ToolbarViewModel.CloseCommand = CloseCommand;
        //}
        //private void SetToolbarStatus()
        //{

        //}

    


    }
}
