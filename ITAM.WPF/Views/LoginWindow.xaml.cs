using ITAM.WPF;
using ITAM.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ITAM.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        private bool _loginSucceeded = false;

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
            _loginSucceeded = true;
            var mainWindow = App.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Application.Current.MainWindow = mainWindow;
            // Đảm bảo app không tắt khi LoginWindow đóng (vì lúc này ShutdownMode mặc định
            // là OnMainWindowClose, mà MainWindow ban đầu của Application chính là cửa sổ đầu tiên Show() ra)
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Nếu cửa sổ đóng KHÔNG phải do đăng nhập thành công
            // (tức người dùng tự bấm nút X) -> thoát hẳn ứng dụng.
            if (!_loginSucceeded)
            {
                Application.Current.Shutdown();
            }
        }
    }
}