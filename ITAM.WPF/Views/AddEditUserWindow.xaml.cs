using System.Windows;
using System.Windows.Controls;
using ITAM.WPF.ViewModels;

namespace ITAM.WPF.Views
{
    public partial class AddEditUserWindow : Window
    {
        public AddEditUserWindow()
        {
            InitializeComponent();
        }

        // PasswordBox không hỗ trợ Binding trực tiếp (bảo mật) nên đồng bộ thủ công vào ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddEditUserViewModel vm)
                vm.Password = PasswordBox.Password;
        }
    }
}