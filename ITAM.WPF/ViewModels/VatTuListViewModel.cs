using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.AppCore.Common;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class VatTuListViewModel : BaseViewModel
    {
        private readonly IVatTuService _vatTuService;
        private readonly ICurrentUserContext _currentUserContext;

        private List<VatTuListDto> _danhSachGoc = new();
        [ObservableProperty] private ObservableCollection<VatTuListDto> danhSach = new();
        public override string Title => "Danh Sách Vật Tư";
        private const string TAT_CA = "-- Tất cả --";

        [ObservableProperty] private string selectedDanhMuc = TAT_CA;
        [ObservableProperty] private VatTuListDto? selectedItem;
        [ObservableProperty] private string searchText = string.Empty;

        public ObservableCollection<FilterOptionViewModel> ViTriFilters { get; } = new();
        public ObservableCollection<string> DanhMucFilters { get; } = new();


        protected override bool HasSelection => SelectedItem != null;

        public VatTuListViewModel(
            IVatTuService vatTuService,
            IErrorDialogService errorDialogService,
            ICurrentUserContext currentUserContext,
            INavigationService navigationService) : base(navigationService,errorDialogService)
        {
            _vatTuService = vatTuService;
            _currentUserContext = currentUserContext;
            _ = LoadAsync();
        }

        protected override void InitToolbarState()
        {
            CanRefresh = true;
            CanClose = true;
        }

        protected override bool CanAdd() => false;
        protected override bool CanDelete() => false;

        private async Task LoadAsync()
        {
            try
            {
                var phongBanId = _currentUserContext.Instance?.PhongBanId
                    ?? throw new InvalidBusinessRuleException("Tài khoản chưa được gán Phòng Ban mặc định.");

                var list = await _vatTuService.GetAllAsync(phongBanId);

                _danhSachGoc = list.Select(x => new VatTuListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MaHangHoa = x.HangHoa?.Code ?? string.Empty,
                    DonViTinh = x.DonViTinh,
                    SoLuongTon = x.SoLuongTon,
                    GhiChu = x.GhiChu,
                    TenViTri = x.ViTriTaiSan?.Name ?? string.Empty,
                    TenDanhMuc = x.HangHoa?.DMTaiSan?.Name ?? string.Empty   // ⬅ thêm
                }).ToList();

                BuildFilterOptions();
                BuildDanhMucFilters();
                ApplyFilter();
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }

        private void BuildFilterOptions()
        {
            ViTriFilters.Clear();
            foreach (var v in _danhSachGoc.Select(x => x.TenViTri).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                ViTriFilters.Add(new FilterOptionViewModel(v));

            foreach (var item in ViTriFilters)
                item.PropertyChanged += OnFilterOptionChanged;
        }

        private void OnFilterOptionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterOptionViewModel.IsChecked))
                ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var viTriChecked = ViTriFilters.Where(x => x.IsChecked).Select(x => x.Value).ToHashSet();
            var keyword = SearchText?.Trim() ?? string.Empty;

            var result = _danhSachGoc.Where(x =>
                (viTriChecked.Count == 0 || viTriChecked.Contains(x.TenViTri)) &&
                (SelectedDanhMuc == TAT_CA || x.TenDanhMuc == SelectedDanhMuc) &&
                (string.IsNullOrEmpty(keyword) || ChuaKeyword(x, keyword))
            );

            DanhSach = new ObservableCollection<VatTuListDto>(result);
        }

        private static bool ChuaKeyword(VatTuListDto x, string keyword)
        {
            return (x.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.MaHangHoa?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.GhiChu?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        protected override async void Refresh() => await LoadAsync();

        partial void OnSelectedItemChanged(VatTuListDto? value) => NotifySelectionChanged();

        protected override void Edit()
        {
            if (SelectedItem == null) return;
            _ = OpenEditDialogAsync(SelectedItem.Id);
        }
        private void BuildDanhMucFilters()
        {
            DanhMucFilters.Clear();
            DanhMucFilters.Add(TAT_CA);
            foreach (var v in _danhSachGoc.Select(x => x.TenDanhMuc).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                DanhMucFilters.Add(v);

            SelectedDanhMuc = TAT_CA;
        }
        private async Task OpenEditDialogAsync(long id)
        {
            try
            {
                var dialogVm = App.Services.GetRequiredService<VatTuEditViewModel>();
                var window = App.Services.GetRequiredService<VatTuEditWindow>();

                window.DataContext = dialogVm;
                window.Owner = Application.Current.MainWindow;

                dialogVm.RequestClose += (s, saved) =>
                {
                    window.DialogResult = saved;
                    window.Close();
                };

                await dialogVm.InitializeAsync(id);

                if (window.ShowDialog() == true)
                    await LoadAsync();
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }
    }
}