using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs.Identity;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Interfaces;

using ITAM.WPF.Constants;
using ITAM.WPF.Helpers;
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
        public override string Title => PageTitles.ThamSoNguoiDung;
        //services
        private readonly IUserService _userService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        // collections
        public ObservableCollection<UserListDto> Users { get; } = new(); // Nguồn cho ComboBox "Người dùng"
        public ObservableCollection<PhongBanCheckItem> AllPhongBans { get; } = new();
        public ObservableCollection<PhongBanCheckItem> AccessiblePhongBans { get; } = new(); // Nguồn cho ComboBox "Phòng ban mặc định" — chỉ gồm các phòng ban đang được tick
        // binding properties
        [ObservableProperty] private UserListDto? selectedUser; // selected value của ComboBox "Người dùng"
        [ObservableProperty] private long? defaultPhongBanId; // selected value của ComboBox "Phòng ban mặc định"
        [ObservableProperty] private bool isDetailEnabled;// Chỉ Enable chức năng chi tiết khi đã chọn User (SelectedUser != null)
        // others
        private long? _originalDefaultPhongBanId; //lưu giá trị DefaultPhongBanId ban đầu để Cancel về
        private Dictionary<long, bool> _originalSelection = new(); //lưu trạng thái tick ban đầu của từng phòng ban, phục vụ cho nút Cancel.
        protected override bool HasSelection => SelectedUser != null; // phục vụ cho nút Edit/Delete (Enable khi có dòng được chọn)

        #region Constructor & Initialization
        public ThamSoNguoiDungViewModel(
            IUserService userService,
            ICatalogService<PhongBan> phongBanService,
            INavigationService navigationService,
            ICurrentUserContext currentUserContext,
            IErrorDialogService errorDialogService) : base(navigationService, errorDialogService, currentUserContext)
        {
            _userService = userService;
            _phongBanService = phongBanService;

            _ = LoadAsync();
        }
        private async Task LoadAsync()
        {
            if (_currentUserContext.Instance == null)
            {
                throw new InvalidOperationException("Phiên đăng nhập không hợp lệ.");
            }
            Users.Clear();
            var availableUsers = await _userService.GetAllAsync();
            foreach (var user in availableUsers)
            {
                var userListDto = new UserListDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    PhongBanId = user.PhongBanId,
                    PhongBanName = user.PhongBan?.Name,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt
                };
                Users.Add(userListDto);
            }
        }
        // Load dữ liệu chi tiết khi chọn User trong ComboBox "Người dùng"
        private async Task LoadDetailAsync(UserListDto? user)
        {
            // Unsubscribe khỏi sự kiện PropertyChanged của các item cũ trước khi xóa chúng
            foreach (var item in AllPhongBans)
                item.PropertyChanged -= OnCheckItemChanged;

            // Reset dữ liệu chi tiết
            AllPhongBans.Clear();
            AccessiblePhongBans.Clear();
            DefaultPhongBanId = null;
            _originalSelection = new();
            _originalDefaultPhongBanId = null;

            // Nếu không có User được chọn, disable chi tiết
            if (user == null)
            {
                IsDetailEnabled = false;
                return;
            }

            var allPhongBans = await _phongBanService.GetAllAsync();
            var accessibleIds = (await _userService.GetAccessiblePhongBanIdsAsync(user.Id)).ToHashSet();
            var fullUser = await _userService.GetByIdAsync(user.Id);

            // Tạo các item cho AllPhongBans, đánh dấu tick nếu phòng ban đó có trong danh sách accessibleIds
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
            // 
            RebuildAccessibleList();
            DefaultPhongBanId = fullUser?.PhongBanId;
            _originalDefaultPhongBanId = fullUser?.PhongBanId;
            IsDetailEnabled = true;
        }
        private void RebuildAccessibleList()
        {
            AccessiblePhongBans.Clear();
            foreach (var item in AllPhongBans.Where(p => p.IsSelected))
                AccessiblePhongBans.Add(item);
        }
        #endregion

        #region protected override methods
        protected override void InitToolbarState()
        {
            CanRefresh = true;
            CanClose = true;
        }
        protected override bool CanAdd() => false;
        protected override bool CanDelete() => false;
        protected override async void Refresh() => await LoadAsync();
        protected override void Cancel()
        {
            foreach (var item in AllPhongBans)
                item.IsSelected = _originalSelection.TryGetValue(item.PhongBanId, out var v) && v;

            DefaultPhongBanId = _originalDefaultPhongBanId;

            base.Cancel(); // IsEditing = false
        }
        protected override void Edit()
        {
            base.Edit(); // IsEditing = true -> tự cập nhật Save/Cancel/Edit qua OnIsEditingChanged
        }

        protected override async void Save()
        {
            if (SelectedUser == null) return;

            var userId = SelectedUser.Id;

            var selectedIds =
                AllPhongBans
                    .Where(p => p.IsSelected)
                    .Select(p => p.PhongBanId)
                    .ToList();


            try
            {
                await _userService.SetPhongBanAccessAsync(
                    userId,
                    DefaultPhongBanId,
                    selectedIds);


                // Kết thúc chế độ Edit
                base.Save();


                // Load lại detail từ DB
                // để đảm bảo UI phản ánh dữ liệu thực tế.
                await LoadDetailAsync(SelectedUser);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }
        #endregion

        #region NotifyPropertyChanged handlers
        partial void OnSelectedUserChanged(UserListDto? value)
        {
            _ = LoadDetailAsync(value);
            NotifySelectionChanged();
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
        #endregion












    }
}