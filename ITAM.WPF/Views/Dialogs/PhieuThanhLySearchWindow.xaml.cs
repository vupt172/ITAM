using ITAM.WPF.ViewModels;
using ITAM.WPF.ViewModels.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace ITAM.WPF.Views.Dialogs
{
    public partial class PhieuThanhLySearchWindow : Window
    {
        public PhieuThanhLySearchWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) =>
            {
                if (DataContext is PhieuThanhLySearchViewModel vm)
                    await vm.TimKiemAsync();
            };
        }

        private void DgKetQua_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is PhieuThanhLySearchViewModel vm && vm.ChonCommand.CanExecute(null))
                vm.ChonCommand.Execute(null);
        }
    }
}