using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
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
        private ICommand DisableCommand { get; }
        [ObservableProperty]
        private ICommand? addCommand;

        [ObservableProperty]
        private ICommand? editCommand;

        [ObservableProperty]
        private ICommand? deleteCommand;

        [ObservableProperty]
        private ICommand? saveCommand;

        [ObservableProperty]
        private ICommand? cancelCommand;

        [ObservableProperty]
        private ICommand? closeCommand;

        public ToolbarViewModel()
        {
            DisableCommand = new RelayCommand(() => { }, () => false);
        }
        public void Apply(ToolbarState state)
        {
            AddCommand = state.AddCommand;
            EditCommand = state.EditCommand;
            DeleteCommand = state.DeleteCommand;
            SaveCommand = state.SaveCommand;
            CancelCommand = state.CancelCommand;
            CloseCommand = state.CloseCommand;
        }
        public void Default()
        {
            AddCommand = DisableCommand;
            EditCommand = DisableCommand;
            DeleteCommand = DisableCommand;
            SaveCommand = DisableCommand;
            CancelCommand = DisableCommand;
            CloseCommand = DisableCommand;
        }

    }
}
