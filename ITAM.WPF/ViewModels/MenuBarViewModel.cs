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
    public partial class MenuBarViewModel:ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly ICurrentUserContext _currentUserContext;
        public bool HasFeatureDashboard =>_currentUserContext.HasFeature("DASHBOARD");
        public bool HasFeatureHeThongDM => _currentUserContext.HasFeature("HETHONG_DANHMUC");
        public bool HasFeatureUserManagement => _currentUserContext.HasFeature("HETHONG_NGUOIDUNG");
        public bool HasFeatureRoleManagement => _currentUserContext.HasFeature("HETHONG_QUYEN");
        
        public MenuBarViewModel(INavigationService navigationService, ICurrentUserContext currentUserContext)
        {
            _navigationService = navigationService;
            _currentUserContext = currentUserContext;
        }
        #region Commands

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
        private void NavigateLoNhap() => _navigationService.NavigateTo<LoNhapViewModel>();
        [RelayCommand]
        private void NavigateUserManagement()
        {
            _navigationService.NavigateTo<UserManagementViewModel>();
        }

        [RelayCommand]
        private void NavigateRoleManagement()
        {
            _navigationService.NavigateTo<RoleManagementViewModel>();
        }

        [RelayCommand]
        private void NavigateThamSoNguoiDung()
        {
            _navigationService.NavigateTo<ThamSoNguoiDungViewModel>();
        }
        [RelayCommand]
        private async Task ChangePhongBan()
        {
            var dialogVm = App.Services.GetRequiredService<ChangePhongBanViewModel>();

            var window = App.Services.GetRequiredService<ChangePhongBanWindow>();
            window.DataContext = dialogVm;
            window.Owner = Application.Current.MainWindow;

            dialogVm.RequestClose += (s, saved) =>
            {
                window.DialogResult = saved;
                window.Close();
            };

            await dialogVm.InitializeAsync();

            if (window.ShowDialog() == true)
            {
                App.Services.GetRequiredService<MainViewModel>().RefreshCurrentUser();
            }
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
            App.Services.GetRequiredService<MainViewModel>().RefreshCurrentUser();
            _navigationService.Reset();   // ⬅ thêm dòng này — xóa sạch cache màn hình đã mở

            // 2. Mở lại LoginWindow (Transient -> tạo instance mới, ViewModel mới sạch sẽ)
            var loginWindow = App.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            // 3. Cập nhật Application.MainWindow để tránh app bị shutdown nhầm
            Application.Current.MainWindow = loginWindow;

            // Đổi Close() -> Hide(): MainWindow là Singleton, Close() sẽ khiến Window
            // không thể Show() lại được nữa ở lần đăng nhập kế tiếp (WPF hạn chế này).
            // Hide() giữ nguyên Window ở trạng thái "ẩn", vẫn Show() lại được bình thường.
            Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault()?.Hide();
        }
        #endregion
        // Gọi lại sau khi đăng nhập thành công hoặc đăng xuất để cập nhật tên hiển thị trên MainWindow
        public void RefreshCurrentUser()
        {
            OnPropertyChanged(nameof(HasFeatureDashboard));
            OnPropertyChanged(nameof(HasFeatureHeThongDM));
            OnPropertyChanged(nameof(HasFeatureUserManagement));
            OnPropertyChanged(nameof(HasFeatureRoleManagement));
        }
    }
}
