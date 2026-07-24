using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAM.WPF.ViewModels
{
    public partial class ToolbarViewModel : ObservableObject
    {
        private readonly IToolbarService _toolbarService;

        [ObservableProperty]
        private ToolbarContext? currentState;


        public ToolbarViewModel(IToolbarService toolbarService)
        {
            _toolbarService= toolbarService;
            CurrentState = toolbarService.CurrentState;
     

            toolbarService.StateChanged += state =>
            {
                CurrentState = state;
            };

        }
    

    }
}
