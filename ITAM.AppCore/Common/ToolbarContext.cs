using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITAM.AppCore.Common
{
    public partial class ToolbarContext : ObservableObject
    {
        [ObservableProperty]
        public ICommand? addCommand;
        [ObservableProperty]
        public ICommand? editCommand;
        [ObservableProperty]
        public ICommand? deleteCommand;
        [ObservableProperty]
        public ICommand? saveCommand;
        [ObservableProperty]
        public ICommand? cancelCommand;
        [ObservableProperty]
        public ICommand? closeCommand;
        [ObservableProperty]
        public ICommand? refreshCommand;
    }
}
