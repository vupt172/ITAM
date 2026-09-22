using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs.Identity;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Common;
using ITAM.WPF.Constants;
using ITAM.WPF.Models;
using ITAM.WPF.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ITAM.WPF.ViewModels
{
    public partial class ThamSoNguoiDungViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly IErrorDialogService _errorDialogService;

        private Dictionary<long, bool> _originalSelection = new();
        private long? _originalDefaultPhongBanId;
        private bool _isSelectingUser; // tránh vòng lặp khi tự set SearchText sau khi chọn User

        public override string Title => PageTitles.ThamSoNguoiDung;

        // Tìm kiếm User theo Họ tên / Tài khoản — gõ để lọc, dòng đầu tự bôi đen (HighlightedItem),
        // chỉ khi Enter/chọn xác nhận thì mới gán vào SelectedUser thật sự.
        public SearchableCollectionView<UserListDto> UserSearch { get; }

        [ObservableProperty]
        private UserListDto? selectedUser;

        [ObservableProperty]
        private long? defaultPhongBanId;

        [ObservableProperty]
        private bool isDetailEnabled;

        public ObservableCollection<PhongBanCheckItem> AllPhongBans { get; } = new();

        // Nguồn cho ComboBox "Phòng ban mặc định" — chỉ gồm các phòng ban đang được tick
        public ObservableCollection<PhongBanCheckItem> AccessiblePhongBans { get; } = new();

        protected override bool CanAdd() => false;
        protected override bool CanDelete() => false;
        protected override bool HasSelection => SelectedUser != null;

        public ThamSoNguoiDungViewModel(
            INavigationService navigationService,
            IUserService userService,
            ICatalogService<PhongBan> phongBanService,
            IErrorDialogService errorDialogService) : base(navigationService)
        {
            _userService = userService;
            _phongBanService = phongBanService;
            _errorDialogService = errorDialogService;

            UserSearch = new SearchableCollectionView<UserListDto>((u, text) =>
                u.FullName.Contains(text, System.StringComparison.OrdinalIgnoreCase) ||
                u.Username.Contains(text, System.StringComparison.OrdinalIgnoreCase));

            _ = LoadUsersAsync();
        }

        protected override void InitToolbarState()
        {
            CanRefresh = true;
            CanClose = true;
        }

        private async Task LoadUsersAsync()
        {
            var list = await _userService.GetAllAsync();

            UserSearch.Reset(list.Select(u => new UserListDto
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

        protected override async void Refresh() => await LoadUsersAsync();

        // Enter hoặc đóng dropdown -> chốt luôn dòng đang được bôi đen (HighlightedItem)
        // thành lựa chọn chính thức (SelectedUser).
        [RelayCommand]
        private void ConfirmUserSearch()
        {
            if (UserSearch.HighlightedItem != null)
                SelectedUser = UserSearch.HighlightedItem;
        }

        partial void OnSelectedUserChanged(UserListDto? value)
        {
            _isSelectingUser = true;
            UserSearch.SetDisplayTextSilently(value != null ? $"{value.FullName} - {value.Username}" : string.Empty);
            _isSelectingUser = false;

            _ = LoadDetailAsync(value);
            NotifySelectionChanged();
        }

        private async Task LoadDetailAsync(UserListDto? user)
        {
            foreach (var item in AllPhongBans)
                item.PropertyChanged -= OnCheckItemChanged;

            AllPhongBans.Clear();
            AccessiblePhongBans.Clear();
            DefaultPhongBanId = null;
            _originalSelection = new();
            _originalDefaultPhongBanId = null;

            if (user == null)
            {
                IsDetailEnabled = false;
                return;
            }

            var allPhongBans = await _phongBanService.GetAllAsync();
            var accessibleIds = (await _userService.GetAccessiblePhongBanIdsAsync(user.Id)).ToHashSet();
            var fullUser = await _userService.GetByIdAsync(user.Id);

            foreach (var pb in allPhongBans)
            {
                var isSelected = accessibleIds.Contains(pb.Id);
                var item = new PhongBanCheckItem
                {
                    PhongBanId = pb.Id,
                    Code = pb.Code,
                    PhongBanName = pb.Name,
                    IsSelected = isSelected
                };
                item.PropertyChanged += OnCheckItemChanged;
                AllPhongBans.Add(item);
                _originalSelection[pb.Id] = isSelected;
            }

            RebuildAccessibleList();
            DefaultPhongBanId = fullUser?.PhongBanId;
            _originalDefaultPhongBanId = fullUser?.PhongBanId;

            IsDetailEnabled = true;
        }

        private void OnCheckItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(PhongBanCheckItem.IsSelected)) return;
            RebuildAccessibleList();

            if (DefaultPhongBanId.HasValue &&
                !AllPhongBans.Any(p => p.PhongBanId == DefaultPhongBanId.Value && p.IsSelected))
            {
                DefaultPhongBanId = null;
            }
        }

        private void RebuildAccessibleList()
        {
            AccessiblePhongBans.Clear();
            foreach (var item in AllPhongBans.Where(p => p.IsSelected))
                AccessiblePhongBans.Add(item);
        }

        protected override void Edit()
        {
            base.Edit(); // IsEditing = true -> tự cập nhật Save/Cancel/Edit qua OnIsEditingChanged
        }

        protected override async void Save()
        {
            if (SelectedUser == null) return;

            var userId = SelectedUser.Id;
            var selectedIds = AllPhongBans.Where(p => p.IsSelected).Select(p => p.PhongBanId).ToList();

            try
            {
                await _userService.SetPhongBanAccessAsync(userId, DefaultPhongBanId, selectedIds);

                base.Save(); // IsEditing = false

                await LoadUsersAsync();

                // Chọn lại đúng User vừa lưu (object mới trong danh sách mới)
                // -> tự trigger OnSelectedUserChanged -> LoadDetailAsync, refresh dữ liệu mới nhất từ DB
                SelectedUser = UserSearch.Items.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override void Cancel()
        {
            foreach (var item in AllPhongBans)
                item.IsSelected = _originalSelection.TryGetValue(item.PhongBanId, out var v) && v;

            DefaultPhongBanId = _originalDefaultPhongBanId;

            base.Cancel(); // IsEditing = false
        }
    }
}