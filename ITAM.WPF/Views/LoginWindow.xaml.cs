using ITAM.WPF;
using ITAM.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ITAM.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.LoginSucceeded += OnLoginSucceeded;
            Loaded += (_, _) => TxtUsername.Focus();
        }

        private void OnLoginSucceeded()
        {
            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Application.Current.MainWindow = mainWindow;
            // Đảm bảo app không tắt khi LoginWindow đóng (vì lúc này ShutdownMode mặc định
            // là OnMainWindowClose, mà MainWindow ban đầu của Application chính là cửa sổ đầu tiên Show() ra)
            Close();
        }
    }
}