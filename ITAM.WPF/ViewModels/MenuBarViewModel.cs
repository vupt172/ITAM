using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ITAM.WPF.ViewModels
{
    public partial class MenuBarViewModel 
    {
        private readonly INavigationService _navigationService;
        private readonly ICurrentUserContext _currentUserContext;

        public MenuBarViewModel(INavigationService navigationService, ICurrentUserContext currentUserContext)
        {
            _navigationService = navigationService;
            _currentUserContext = currentUserContext;

        }


        [RelayCommand]
        private void NavigateDashboard()
        {
            _navigationService.NavigateTo<DashboardViewModel>();
        }
        [RelayCommand]
        private void NavigatHeThongDM()
        {
            _navigationService.NavigateTo<HeThongDMViewModel>();
        }
        [RelayCommand]
        private void Logout()
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            // 1. Xóa thông tin user hiện tại
            _currentUserContext.Clear();

            // 2. Mở lại LoginWindow (Transient -> tạo instance mới, ViewModel mới sạch sẽ)
            var loginWindow = App.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            // 3. Cập nhật Application.MainWindow để tránh app bị shutdown nhầm
            Application.Current.MainWindow = loginWindow;

            // 4. Đóng MainWindow hiện tại
            Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault()?.Close();
        }
    }
}
