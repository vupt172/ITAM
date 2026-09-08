using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs.Identity;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Models;
using ITAM.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ITAM.WPF.ViewModels
{
    public partial class AddEditUserViewModel : ObservableValidator
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly IErrorDialogService _errorDialogService;

        private long? _editingUserId;

        [ObservableProperty]
        private string title = "Thêm người dùng";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        private string fullName = string.Empty;

        [ObservableProperty]
        private long? phongBanId;

        [ObservableProperty]
        private bool isActive = true;

        [ObservableProperty]
        private bool isUsernameReadOnly;

        [ObservableProperty]
        private bool isPasswordFieldVisible;
        public long? SavedUserId { get; private set; }

        public ObservableCollection<PhongBan> PhongBans { get; } = new();
        public ObservableCollection<RoleCheckItem> Roles { get; } = new();

        public event EventHandler<bool>? RequestClose;

        public AddEditUserViewModel(
            IUserService userService,
            IRoleService roleService,
            ICatalogService<PhongBan> phongBanService,
            IErrorDialogService errorDialogService)
        {
            _userService = userService;
            _roleService = roleService;
            _phongBanService = phongBanService;
            _errorDialogService = errorDialogService;
        }

        public async Task InitializeAsync(long? userId)
        {
            _editingUserId = userId;
            Title = userId == null ? "Thêm người dùng" : "Sửa người dùng";
            IsUsernameReadOnly = userId != null;
            IsPasswordFieldVisible = userId == null;

            var phongBans = await _phongBanService.GetAllAsync(); // mặc định chỉ lấy Active
            PhongBans.Clear();
            foreach (var pb in phongBans) PhongBans.Add(pb);

            var roles = await _roleService.GetAllAsync();
            Roles.Clear();
            foreach (var r in roles)
                Roles.Add(new RoleCheckItem { RoleId = r.Id, RoleName = r.Name, IsSelected = false });

            if (userId != null)
            {
                var user = await _userService.GetByIdAsync(userId.Value);
                if (user == null) return;

                Username = user.Username;
                FullName = user.FullName;
                PhongBanId = user.PhongBanId;
                IsActive = user.IsActive;

                var selectedRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
                foreach (var r in Roles)
                    r.IsSelected = selectedRoleIds.Contains(r.RoleId);
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            ValidateAllProperties();
            if (HasErrors) return;

            if (_editingUserId == null && string.IsNullOrWhiteSpace(Password))
            {
                _errorDialogService.Show(new InvalidOperationException("Vui lòng nhập mật khẩu."));
                return;
            }

            var selectedRoleIds = Roles.Where(r => r.IsSelected).Select(r => r.RoleId).ToList();

            try
            {
                if (_editingUserId == null)
                {
                    var created= await _userService.CreateAsync(new CreateUserDto
                    {
                        Username = Username,
                        Password = Password,
                        FullName = FullName,
                        PhongBanId = PhongBanId,
                        RoleIds = selectedRoleIds
                    });
                    SavedUserId = created.Id;
                }
                else
                {
                    await _userService.UpdateAsync(new UpdateUserDto
                    {
                        Id = _editingUserId.Value,
                        FullName = FullName,
                        IsActive = IsActive,
                        PhongBanId = PhongBanId,
                        RoleIds = selectedRoleIds
                    });
                    SavedUserId = _editingUserId;
                }

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