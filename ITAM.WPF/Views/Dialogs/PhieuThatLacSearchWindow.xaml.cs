using ITAM.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ITAM.WPF.Views.Dialogs
{
    public partial class PhieuThatLacSearchWindow : Window
    {
        public PhieuThatLacSearchWindow()
        {
            InitializeComponent();

            // DataContext do PhieuThatLacViewModel.Search() gán (giống DieuChuyenSearchWindow).
            Loaded += async (_, _) =>
            {
                if (DataContext is PhieuThatLacSearchViewModel vm)
                    await vm.TimKiemAsync();
            };
        }

        private void DgKetQua_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Chỉ nhận double-click trên 1 dòng dữ liệu, không nhận trên header/vùng trống.
            if (ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) is not DataGridRow)
                return;

            if (DataContext is PhieuThatLacSearchViewModel vm && vm.DongChon != null)
                vm.ChonCommand.Execute(null);
        }
    }
}