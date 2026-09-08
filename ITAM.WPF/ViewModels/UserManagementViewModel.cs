using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs.Identity;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class UserManagementViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IErrorDialogService _errorDialogService;

        public override string Title => PageTitles.UserManagement;

        [ObservableProperty]
        private ObservableCollection<UserListDto> users = new();

        [ObservableProperty]
        private UserListDto? selectedUser;
        protected override bool HasSelection => SelectedUser != null;

        public UserManagementViewModel(
            INavigationService navigationService,
            IUserService userService,
            IErrorDialogService errorDialogService) : base(navigationService)
        {
            _userService = userService;
            _errorDialogService = errorDialogService;
            _ = LoadAsync();
        }

        protected override void InitToolbarState()
        {
            CanRefresh = true; // canAdd/canEdit/canDelete giờ tự suy ra, không cần gán
            CanClose = true;
        }

        partial void OnSelectedUserChanged(UserListDto? value)
        {
            NotifySelectionChanged();
        }

        private async Task LoadAsync()
        {
            var list = await _userService.GetAllAsync();
            Users = new ObservableCollection<UserListDto>(list.Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                PhongBanId = u.PhongBanId,
                PhongBanName = u.PhongBan?.Name,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt
            }));
        }

        protected override async void Refresh() => await LoadAsync();

        protected override void Add() => _ = OpenDialogAsync(null);

        protected override void Edit()
        {
            if (SelectedUser == null) return;
            _ = OpenDialogAsync(SelectedUser.Id);
        }

        protected override async void Delete()
        {
            if (SelectedUser == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa vĩnh viễn tài khoản '{SelectedUser.Username}'?\nHành động này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _userService.DeleteAsync(SelectedUser.Id);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }
        [RelayCommand]
        private async Task ResetPassword(UserListDto? user)
        {
            if (user == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn đặt lại mật khẩu về mặc định (123) cho tài khoản '{user.Username}'?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _userService.ResetPasswordAsync(user.Id, "123");
                MessageBox.Show($"Đã đặt lại mật khẩu cho '{user.Username}' về mặc định (123).",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        private async Task OpenDialogAsync(long? userId)
        {
            var dialogVm = App.Services.GetRequiredService<AddEditUserViewModel>();

            var window = App.Services.GetRequiredService<AddEditUserWindow>();
            window.DataContext = dialogVm;
            window.Owner = Application.Current.MainWindow;

            dialogVm.RequestClose += (s, saved) =>
            {
                window.DialogResult = saved;
                window.Close();
            };

            await dialogVm.InitializeAsync(userId);   // ⬅ chờ load xong (FullName, Roles...) rồi mới hiện dialog

            if (window.ShowDialog() == true)
            {
                await LoadAsync();
                if (dialogVm.SavedUserId.HasValue)
                    SelectedUser = Users.FirstOrDefault(u => u.Id == dialogVm.SavedUserId.Value);
            }
        }


    }
}