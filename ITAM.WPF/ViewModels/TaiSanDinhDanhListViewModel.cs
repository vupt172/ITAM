using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class TaiSanDinhDanhListViewModel : BaseViewModel
    {
        private readonly ITaiSanDinhDanhService _taiSanDinhDanhService;
        private readonly ICurrentUserContext _currentUserContext;

        public override string Title => "Danh Sách Tài Sản Định Danh";

        [ObservableProperty] private ObservableCollection<TaiSanDinhDanhListDto> danhSach = new();
        [ObservableProperty] private TaiSanDinhDanhListDto? selectedItem;
        [ObservableProperty] private string searchText = string.Empty;
        private const string TAT_CA = "-- Tất cả --";
        [ObservableProperty] private string selectedDanhMuc = TAT_CA;
        public ObservableCollection<string> DanhMucFilters { get; } = new();
        [ObservableProperty]
        private string maQuet;
        public ObservableCollection<FilterOptionViewModel> ViTriFilters { get; } = new();
        public ObservableCollection<FilterOptionViewModel> LoaiTaiSanFilters { get; } = new();
        public ObservableCollection<FilterOptionViewModel> TrangThaiFilters { get; } = new();

        private List<TaiSanDinhDanhListDto> _danhSachGoc = new();

        protected override bool HasSelection => SelectedItem != null;

        public TaiSanDinhDanhListViewModel(
            ITaiSanDinhDanhService taiSanDinhDanhService,
            IErrorDialogService errorDialogService,
            ICurrentUserContext currentUserContext,
            INavigationService navigationService) : base(navigationService,errorDialogService)
        {
            _taiSanDinhDanhService = taiSanDinhDanhService;
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

                var list = await _taiSanDinhDanhService.GetAllAsync(phongBanId);

                _danhSachGoc = list.Select(x => new TaiSanDinhDanhListDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Serial = x.Serial,
                    MaHangHoa = x.HangHoa?.Code ?? string.Empty,
                    TenLoaiTaiSan = x.LoaiTaiSan?.Name ?? string.Empty,
                    TrangThaiTaiSan = x.TrangThaiTaiSan.ToString(),
                    TenViTri = x.ViTriTaiSan?.Name ?? string.Empty,
                    NamSuDung = x.NamSuDung,
                    GiaNhap = x.GiaNhap,
                    GhiChu=x.GhiChu,
                    SoLoNhapChiTiet = x.LoNhapChiTiet?.SoLo ?? string.Empty,
                    TenDanhMuc = x.HangHoa?.DMTaiSan?.Name ?? string.Empty   // ⬅ thêm
                }).ToList();

                BuildFilterOptions();
                BuildDanhMucFilters();   // ⬅ thêm
                ApplyFilter();
            }
            catch (Exception ex) { _errorDialogService.Show(ex); }
        }


        private void BuildFilterOptions()
        {
            ViTriFilters.Clear();
            foreach (var v in _danhSachGoc.Select(x => x.TenViTri).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                ViTriFilters.Add(new FilterOptionViewModel(v));

            LoaiTaiSanFilters.Clear();
            foreach (var v in _danhSachGoc.Select(x => x.TenLoaiTaiSan).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                LoaiTaiSanFilters.Add(new FilterOptionViewModel(v));

            TrangThaiFilters.Clear();
            foreach (var v in _danhSachGoc.Select(x => x.TrangThaiTaiSan).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                TrangThaiFilters.Add(new FilterOptionViewModel(v));

            DangKyFilterEvents(ViTriFilters);
            DangKyFilterEvents(LoaiTaiSanFilters);
            DangKyFilterEvents(TrangThaiFilters);
        }

        private void DangKyFilterEvents(IEnumerable<FilterOptionViewModel> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += OnFilterOptionChanged;
            }
        }

        private void OnFilterOptionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FilterOptionViewModel.IsChecked))
                ApplyFilter();
        }

        partial void OnSearchTextChanged(string value) => ApplyFilter();
        partial void OnSelectedDanhMucChanged(string value) => ApplyFilter();
        private void ApplyFilter()
        {
            var viTriChecked = ViTriFilters.Where(x => x.IsChecked).Select(x => x.Value).ToHashSet();
            var loaiChecked = LoaiTaiSanFilters.Where(x => x.IsChecked).Select(x => x.Value).ToHashSet();
            var trangThaiChecked = TrangThaiFilters.Where(x => x.IsChecked).Select(x => x.Value).ToHashSet();
            var keyword = SearchText?.Trim() ?? string.Empty;

            var result = _danhSachGoc.Where(x =>
                (viTriChecked.Count == 0 || viTriChecked.Contains(x.TenViTri)) &&
                (loaiChecked.Count == 0 || loaiChecked.Contains(x.TenLoaiTaiSan)) &&
                (trangThaiChecked.Count == 0 || trangThaiChecked.Contains(x.TrangThaiTaiSan)) &&
                (SelectedDanhMuc == TAT_CA || x.TenDanhMuc == SelectedDanhMuc) &&
                (string.IsNullOrEmpty(keyword) || ChuaKeyword(x, keyword))
            );

            DanhSach = new ObservableCollection<TaiSanDinhDanhListDto>(result);
        }

        private static bool ChuaKeyword(TaiSanDinhDanhListDto x, string keyword)
        {
            return (x.Name?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.Code?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.Serial?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.MaHangHoa?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)
                || (x.SoLoNhapChiTiet?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        protected override async void Refresh() => await LoadAsync();

        partial void OnSelectedItemChanged(TaiSanDinhDanhListDto? value) => NotifySelectionChanged();

        [RelayCommand]
        private async Task QuetMaAsync()
        {
            var ma = MaQuet?.Trim();
            if (string.IsNullOrWhiteSpace(ma)) return;

            // 1. Tìm trong danh sách đang hiển thị (đã lọc theo khoa phòng) trước
            var found = DanhSach.FirstOrDefault(
                t => t.Code.Equals(ma, StringComparison.OrdinalIgnoreCase));

            if (found != null)
            {
                SelectedItem = found;
                MaQuet = string.Empty;
                return;
            }

            // 2. Không có trong list local -> tra toàn bộ DB
            var tsTuDb = await _taiSanDinhDanhService.GetByCodeAsync(ma);

            if (tsTuDb == null)
            {
                MessageBox.Show($"Mã tài sản '{ma}' không tồn tại trong hệ thống.",
                    "Không tìm thấy", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                var tenKhoaPhong = tsTuDb.ViTriTaiSan.PhongBan.Name;
                MessageBox.Show(
                    $"Tài sản '{ma}' hiện KHÔNG thuộc khoa phòng đang xem.\n" +
                    $"Phòng ban thực tế: {tsTuDb.ViTriTaiSan.PhongBan.Name}\n" +
                    $"Vị trí thực tế: {tsTuDb.ViTriTaiSan.Name}\n" +
                    $"Trạng thái: {tsTuDb.TrangThaiTaiSan}",
                    "Phát hiện sai lệch", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //MaQuet = string.Empty;
        }
        protected override void Edit()
        {
            if (SelectedItem == null) return;
            _ = OpenEditDialogAsync(SelectedItem.Id);
        }

        private async Task OpenEditDialogAsync(long id)
        {
            try
            {
                var dialogVm = App.Services.GetRequiredService<TaiSanDinhDanhEditViewModel>();
                var window = App.Services.GetRequiredService<TaiSanDinhDanhEditWindow>();

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
        private void BuildDanhMucFilters()
        {
            DanhMucFilters.Clear();
            DanhMucFilters.Add(TAT_CA);
            foreach (var v in _danhSachGoc.Select(x => x.TenDanhMuc).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x))
                DanhMucFilters.Add(v);
            SelectedDanhMuc = TAT_CA;
        }
    }
}