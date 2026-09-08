using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Models;
using ITAM.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;

namespace ITAM.WPF.ViewModels
{
    public partial class AddEditRoleViewModel : ObservableValidator
    {
        private readonly IRoleService _roleService;
        private readonly IFeatureService _featureService;
        private readonly IErrorDialogService _errorDialogService;

        private long? _editingRoleId;

        [ObservableProperty]
        private string title = "Thêm vai trò";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Mã vai trò không được để trống.")]
        private string code = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên vai trò không được để trống.")]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isCodeReadOnly;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string? featureSearchText;

        [ObservableProperty]
        private int selectedFeatureCount;

        public ObservableCollection<FeatureCheckItem> Features { get; } = new();
        public ICollectionView FeaturesView { get; }

        public event EventHandler<bool>? RequestClose;

        public AddEditRoleViewModel(
            IRoleService roleService,
            IFeatureService featureService,
            IErrorDialogService errorDialogService)
        {
            _roleService = roleService;
            _featureService = featureService;
            _errorDialogService = errorDialogService;

            FeaturesView = CollectionViewSource.GetDefaultView(Features);
            FeaturesView.Filter = FilterFeature;
        }

        public async Task InitializeAsync(long? roleId)
        {
            IsLoading = true;
            try
            {
                _editingRoleId = roleId;
                Title = roleId == null ? "Thêm vai trò" : "Sửa vai trò";
                IsCodeReadOnly = roleId != null;

                var allFeatures = await _featureService.GetAllAsync();
                var selectedIds = new HashSet<long>();

                if (roleId != null)
                {
                    var roles = await _roleService.GetAllAsync();
                    var role = roles.FirstOrDefault(r => r.Id == roleId.Value);
                    if (role != null)
                    {
                        Code = role.Code;
                        Name = role.Name;
                        selectedIds = role.RoleFeatures.Select(rf => rf.FeatureId).ToHashSet();
                    }
                }

                foreach (var item in Features)
                    item.PropertyChanged -= OnFeatureChanged;

                Features.Clear();
                foreach (var f in allFeatures)
                {
                    var item = new FeatureCheckItem
                    {
                        FeatureId = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                        Description = f.Description,
                        IsSelected = selectedIds.Contains(f.Id)
                    };
                    item.PropertyChanged += OnFeatureChanged;
                    Features.Add(item);
                }

                UpdateSelectedCount();
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

        private void OnFeatureChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FeatureCheckItem.IsSelected))
                UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
            => SelectedFeatureCount = Features.Count(f => f.IsSelected);

        partial void OnFeatureSearchTextChanged(string? value) => FeaturesView.Refresh();

        private bool FilterFeature(object obj)
        {
            if (obj is not FeatureCheckItem item) return false;
            if (string.IsNullOrWhiteSpace(FeatureSearchText)) return true;
            return item.Name.Contains(FeatureSearchText, StringComparison.OrdinalIgnoreCase)
                || item.Code.Contains(FeatureSearchText, StringComparison.OrdinalIgnoreCase);
        }

        [RelayCommand]
        private async Task Save()
        {
            ValidateAllProperties();
            if (HasErrors) return;

            var selectedFeatureIds = Features.Where(f => f.IsSelected).Select(f => f.FeatureId).ToList();

            try
            {
                if (_editingRoleId == null)
                    await _roleService.CreateAsync(Code, Name, selectedFeatureIds);
                else
                    await _roleService.UpdateAsync(_editingRoleId.Value, Name, selectedFeatureIds);

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