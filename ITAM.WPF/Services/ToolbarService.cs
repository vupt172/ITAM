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

namespace ITAM.WPF.Services
{
    public class ToolbarService : IToolbarService
    {
        public event Action<ToolbarContext>? StateChanged;
        public ToolbarContext CurrentState { get; set; }
        private ICommand DisableCommand { get; }

        public ToolbarService()
        {
            DisableCommand = new RelayCommand(() => { }, () => false);
        }

        public void Apply(ToolbarContext state)
        {
            CurrentState = state;
            StateChanged?.Invoke(state);
        }
        public void SetDefault()
        {   Apply(new ToolbarContext
            {
                AddCommand = DisableCommand,
                EditCommand = DisableCommand,
                DeleteCommand = DisableCommand,
                SaveCommand = DisableCommand,
                CancelCommand = DisableCommand,
                CloseCommand = DisableCommand
            });

        }
    }
}
