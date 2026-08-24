using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels;
using ITAM.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.Services
{
    public class ErrorDialogService : IErrorDialogService
    {

        public void Show(Exception ex)
        {
            var vm = new ErrorDialogViewModel();
            vm.LoadFrom(ex); // Toàn bộ logic map Exception -> ViewModel nằm gọn ở đây

            var dialog = new ErrorDialog
            {
                DataContext = vm
            };
            vm.RequestClose = dialog.Close;

            var owner = Application.Current?.MainWindow;
            if (owner != null && owner.IsLoaded && !ReferenceEquals(owner, dialog))
            {
                dialog.Owner = owner;
            }

            dialog.ShowDialog();
        }
    }
}
