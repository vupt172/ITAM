using System.Windows.Controls;
using ITAM.WPF.ViewModels;

namespace ITAM.WPF.Views
{
    public partial class ThamSoNguoiDungView : UserControl
    {
        public ThamSoNguoiDungView()
        {
            InitializeComponent();
        }

        // Khi dropdown đóng lại (do click chọn item, Enter, hoặc click ra ngoài),
        // xác nhận luôn item đang bôi đen thành SelectedUser chính thức.
        private void UserComboBox_DropDownClosed(object sender, System.EventArgs e)
        {
            if (DataContext is ThamSoNguoiDungViewModel vm)
                vm.ConfirmUserSearchCommand.Execute(null);
        }
    }
}