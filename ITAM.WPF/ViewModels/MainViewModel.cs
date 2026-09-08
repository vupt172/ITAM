using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Services.Interfaces;

namespace ITAM.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public string Title => PageTitles.Default;
        public INavigationService NavigationService { get; }
        public MenuBarViewModel MenuBarViewModel { get; }
        public ToolbarViewModel ToolbarViewModel { get; }

        private readonly ICurrentUserContext _currentUserContext;

        public string CurrentUserDisplayName =>
            _currentUserContext.Instance != null
                ? $"{_currentUserContext.Instance.FullName} ({_currentUserContext.Instance.Username})"
                : string.Empty;
        public string CurrentPhongBanName =>
    _currentUserContext.Instance?.PhongBan?.Name ?? "Chưa gán Phòng ban";

        public MainViewModel(
            INavigationService navigationService,
            MenuBarViewModel menuBarViewModel,
            ToolbarViewModel toolbarViewModel,
            ICurrentUserContext currentUserContext)
        {
            NavigationService = navigationService;
            MenuBarViewModel = menuBarViewModel;
            ToolbarViewModel = toolbarViewModel;
            _currentUserContext = currentUserContext;
        }

        // Gọi lại sau khi đăng nhập thành công hoặc đăng xuất để cập nhật tên hiển thị trên MainWindow
        public void RefreshCurrentUser()
        {
            OnPropertyChanged(nameof(CurrentUserDisplayName));
            OnPropertyChanged(nameof(CurrentPhongBanName));
        }
    }
}