// ITAM.WPF/ViewModels/LoginViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.WPF.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserContext _currentUserContext;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private bool isLoading;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public event Action? LoginSucceeded;

        public LoginViewModel(IAuthService authService, ICurrentUserContext currentUserContext)
        {
            _authService = authService;
            _currentUserContext = currentUserContext;
        }

        partial void OnUsernameChanged(string value) => ErrorMessage = string.Empty;

        [RelayCommand]
        private void Test()
        {
            MessageBox.Show("Đang thử đăng nhập...");
        }
        [RelayCommand(CanExecute = nameof(CanLogin))]
        private async Task LoginAsync(object? passwordBoxParam)
        {
    
            // passwordBoxParam sẽ là PasswordBox truyền từ XAML (MaterialDesign chưa hỗ trợ bind Password trực tiếp)
            var password = ExtractPassword(passwordBoxParam);

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập.";
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                var user = await _authService.LoginAsync(Username.Trim(), password);
                _currentUserContext.Set(user);
                App.Services.GetRequiredService<MainViewModel>().RefreshCurrentUser();
                App.Services.GetRequiredService<MenuBarViewModel>().RefreshCurrentUser();
                LoginSucceeded?.Invoke();
            }
            catch (AuthenticationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage = "Có lỗi xảy ra, vui lòng thử lại.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanLogin() => !IsLoading;

        private static string ExtractPassword(object? param)
        {
            if (param is System.Windows.Controls.PasswordBox pb)
                return pb.Password;
            return param as string ?? string.Empty;
        }
    }
}