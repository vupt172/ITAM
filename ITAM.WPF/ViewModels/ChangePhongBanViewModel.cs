using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace ITAM.WPF.ViewModels
{
    public partial class ChangePhongBanViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IErrorDialogService _errorDialogService;

        [ObservableProperty]
        private long? selectedPhongBanId;

        [ObservableProperty]
        private bool isLoading;

        public ObservableCollection<PhongBan> AccessiblePhongBans { get; } = new();

        public event EventHandler<bool>? RequestClose;

        public ChangePhongBanViewModel(
            IUserService userService,
            ICatalogService<PhongBan> phongBanService,
            ICurrentUserContext currentUserContext,
            IErrorDialogService errorDialogService)
        {
            _userService = userService;
            _phongBanService = phongBanService;
            _currentUserContext = currentUserContext;
            _errorDialogService = errorDialogService;
        }

        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var currentUser = _currentUserContext.Instance
                    ?? throw new InvalidOperationException("Phiên đăng nhập không hợp lệ.");

                var accessibleIds = (await _userService.GetAccessiblePhongBanIdsAsync(currentUser.Id)).ToHashSet();
                var allPhongBans = await _phongBanService.GetAllAsync();

                AccessiblePhongBans.Clear();
                foreach (var pb in allPhongBans.Where(p => accessibleIds.Contains(p.Id)))
                    AccessiblePhongBans.Add(pb);

                SelectedPhongBanId = currentUser.PhongBanId;
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (SelectedPhongBanId == null)
            {
                _errorDialogService.Show(new InvalidOperationException("Vui lòng chọn Khoa/Phòng ban."));
                return;
            }

            var currentUser = _currentUserContext.Instance;
            if (currentUser == null) return;

            try
            {
                await _userService.SetDefaultPhongBanAsync(currentUser.Id, SelectedPhongBanId.Value);

                // Cập nhật ngay trong bộ nhớ CurrentUserContext để MainView phản ánh tức thì,
                // không cần đăng xuất/đăng nhập lại.
                currentUser.PhongBanId = SelectedPhongBanId.Value;
                currentUser.PhongBan = AccessiblePhongBans.FirstOrDefault(p => p.Id == SelectedPhongBanId.Value);

                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        [RelayCommand]
        private void Cancel() => RequestClose?.Invoke(this, false);
    }
}