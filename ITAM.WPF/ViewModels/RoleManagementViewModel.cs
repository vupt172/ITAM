using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class RoleManagementViewModel : BaseViewModel
    {
        private readonly IRoleService _roleService;
        private readonly IErrorDialogService _errorDialogService;

        public override string Title => PageTitles.RoleManagement;

        [ObservableProperty]
        private ObservableCollection<RoleListDto> roles = new();

        [ObservableProperty]
        private RoleListDto? selectedRole;

        protected override bool HasSelection => SelectedRole != null;

        public RoleManagementViewModel(
            INavigationService navigationService,
            IRoleService roleService,
            IErrorDialogService errorDialogService) : base(navigationService)
        {
            _roleService = roleService;
            _errorDialogService = errorDialogService;
            _ = LoadAsync();
        }

        protected override void InitToolbarState()
        {
            CanRefresh = true;
            CanClose = true;
        }

        private async Task LoadAsync()
        {
            var list = await _roleService.GetAllAsync();
            Roles = new ObservableCollection<RoleListDto>(list.Select(r => new RoleListDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                FeatureCount = r.RoleFeatures.Count,
                UserCount = r.UserRoles.Count
            }));
        }

        protected override async void Refresh() => await LoadAsync();

        partial void OnSelectedRoleChanged(RoleListDto? value)
        {
            NotifySelectionChanged();
        }

        protected override void Add() => _ = OpenDialogAsync(null);

        protected override void Edit()
        {
            
            if (SelectedRole == null) return;
            _ = OpenDialogAsync(SelectedRole.Id);
        }

        protected override async void Delete()
        {
            if (SelectedRole == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa vai trò '{SelectedRole.Name}'?\nHành động này không thể hoàn tác.",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _roleService.DeleteAsync(SelectedRole.Id);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        private async Task OpenDialogAsync(long? roleId)
        {
            try
            {
                var dialogVm = App.Services.GetRequiredService<AddEditRoleViewModel>();
                var window = App.Services.GetRequiredService<AddEditRoleWindow>();

                window.DataContext = dialogVm;
                window.Owner = Application.Current.MainWindow;

                dialogVm.RequestClose += (s, saved) =>
                {
                    window.DialogResult = saved;
                    window.Close();
                };

                await dialogVm.InitializeAsync(roleId);

                if (window.ShowDialog() == true)
                {
                    await LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LỖI: {ex.GetType().Name}\n{ex.Message}\n\n{ex.StackTrace}");
            }
        }
    }
}