using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class LoNhapViewModel : BaseViewModel
    {
        private readonly ILoNhapService _loNhapService;
        private readonly ICatalogService<NhaCungCap> _nhaCungCapService;
        private readonly ICatalogService<HangHoa> _hangHoaService;
        private readonly ICatalogService<LoaiTaiSan> _loaiTaiSanService;
        private readonly ICurrentUserContext _currentUser;

        public override string Title => "Phiếu Nhập Kho";

        public ObservableCollection<LoNhapChiTietFormDto> ChiTiets { get; } = [];
        public ObservableCollection<NhaCungCap> NhaCungCaps { get; } = [];
        public ObservableCollection<HangHoa> HangHoas { get; } = [];
        public ObservableCollection<LoaiTaiSan> LoaiTaiSans { get; } = [];

        [ObservableProperty] private LoNhapFormDto currentPhieu = new();
        [ObservableProperty] private LoNhapChiTietFormDto currentChiTiet = new();
        [ObservableProperty] private LoNhapChiTietFormDto? selectedChiTiet;

        // Phiếu phải đã lưu (Id > 0) và đang PENDING mới cho thêm/xóa dòng, Duyệt/Từ chối.
        public bool IsPending => CurrentPhieu.Id > 0 && CurrentPhieu.TrangThai == "PENDING";

        public LoNhapViewModel(ILoNhapService loNhapService, ICatalogService<NhaCungCap> nhaCungCapService,
            ICatalogService<HangHoa> hangHoaService, ICatalogService<LoaiTaiSan> loaiTaiSanService,
            ICurrentUserContext currentUser, INavigationService navigationService) : base(navigationService)
        {
            _loNhapService = loNhapService; _nhaCungCapService = nhaCungCapService;
            _hangHoaService = hangHoaService; _loaiTaiSanService = loaiTaiSanService; _currentUser = currentUser;
            _ = LoadDanhMucAsync();
        }

        public override ToolbarContext GetToolbarContext()
        {
            var ctx = base.GetToolbarContext();
            ctx.SearchCommand = SearchCommand;
            return ctx;
        }

        protected override void InitToolbarState() => CanClose = true;

        private async Task LoadDanhMucAsync()
        {
            NhaCungCaps.Clear(); foreach (var x in await _nhaCungCapService.GetAllAsync()) NhaCungCaps.Add(x);
            HangHoas.Clear(); foreach (var x in await _hangHoaService.GetAllAsync()) HangHoas.Add(x);
            LoaiTaiSans.Clear(); foreach (var x in await _loaiTaiSanService.GetAllAsync()) LoaiTaiSans.Add(x);
        }

        private async Task LoadPhieuAsync(long loNhapId)
        {
            var p = await _loNhapService.GetByIdAsync(loNhapId);
            if (p == null) return;

            CurrentPhieu = new LoNhapFormDto
            {
                Id = p.Id,
                SoLo = p.SoLo,
                NgayNhap = p.NgayNhap,
                NhaCungCapId = p.NhaCungCapId,
                TenNhaCungCap = p.NhaCungCap?.Name,
                NguoiLapPhieuId = p.NguoiLapPhieuId,
                TenNguoiLapPhieu = p.NguoiLapPhieu?.FullName ?? string.Empty,
                GhiChu = p.GhiChu,
                TrangThai = p.TrangThai.ToString()
            };

            ChiTiets.Clear();
            foreach (var x in p.ChiTiets)
                ChiTiets.Add(new()
                {
                    Id = x.Id,
                    SoLo = x.SoLo,
                    HangHoaId = x.HangHoaId,
                    TenVatPham = x.TenVatPham,
                    SoLuongNhap = x.SoLuongNhap,
                    DonGia = x.DonGia,
                    LoaiTaiSanId = x.LoaiTaiSanId
                });

            IsEditing = false;
        }

        protected override void Add()
        {
            CurrentPhieu = new();
            ChiTiets.Clear();
            IsEditing = true;
        }

        protected override async void Save()
        {
            if (CurrentPhieu.NhaCungCapId == 0 || _currentUser.Instance == null)
            {
                MessageBox.Show("Chọn nhà cung cấp và đăng nhập lại trước khi lập phiếu.");
                return;
            }
            try
            {
                var id = await _loNhapService.TaoPhieuNhapAsync(new()
                {
                    NgayNhap = CurrentPhieu.NgayNhap,
                    NhaCungCapId = CurrentPhieu.NhaCungCapId,
                    NguoiLapPhieuId = _currentUser.Instance.Id,
                    GhiChu = CurrentPhieu.GhiChu
                });
                await LoadPhieuAsync(id);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        protected override void Cancel()
        {
            IsEditing = false;
            if (CurrentPhieu.Id == 0) { CurrentPhieu = new(); ChiTiets.Clear(); }
        }

        [RelayCommand]
        private async Task Search()
        {
            var dialogVm = App.Services.GetRequiredService<LoNhapSearchViewModel>();
            var window = App.Services.GetRequiredService<LoNhapSearchWindow>();
            window.DataContext = dialogVm;
            window.Owner = Application.Current.MainWindow;

            dialogVm.RequestClose += (s, chosen) =>
            {
                window.DialogResult = chosen;
                window.Close();
            };

            if (window.ShowDialog() == true && dialogVm.SelectedLoNhapId is long id)
                await LoadPhieuAsync(id);
        }

        [RelayCommand(CanExecute = nameof(CanAddChiTiet))]
        private async Task AddChiTiet()
        {
            try
            {
                await _loNhapService.ThemChiTietAsync(CurrentPhieu.Id, new()
                {
                    HangHoaId = CurrentChiTiet.HangHoaId,
                    SoLuongNhap = CurrentChiTiet.SoLuongNhap,
                    DonGia = CurrentChiTiet.DonGia,
                    LoaiTaiSanId = CurrentChiTiet.LoaiTaiSanId
                });
                await LoadPhieuAsync(CurrentPhieu.Id);
                CurrentChiTiet = new();
            }
            catch (InvalidBusinessRuleException ex) { MessageBox.Show(ex.Message); }
        }
        private bool CanAddChiTiet() => IsPending;

        [RelayCommand(CanExecute = nameof(CanDeleteChiTiet))]
        private async Task DeleteChiTiet()
        {
            await _loNhapService.XoaChiTietAsync(SelectedChiTiet!.Id);
            await LoadPhieuAsync(CurrentPhieu.Id);
        }
        private bool CanDeleteChiTiet() => IsPending && SelectedChiTiet != null;

        [RelayCommand(CanExecute = nameof(CanDuyet))]
        private async Task Duyet()
        {
            try { await _loNhapService.DuyetLoNhapAsync(CurrentPhieu.Id); await LoadPhieuAsync(CurrentPhieu.Id); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool CanDuyet() => IsPending && ChiTiets.Count > 0;

        [RelayCommand(CanExecute = nameof(CanTuChoi))]
        private async Task TuChoi()
        {
            await _loNhapService.TuChoiLoNhapAsync(CurrentPhieu.Id);
            await LoadPhieuAsync(CurrentPhieu.Id);
        }
        private bool CanTuChoi() => IsPending;

        partial void OnSelectedChiTietChanged(LoNhapChiTietFormDto? value) => DeleteChiTietCommand.NotifyCanExecuteChanged();
        partial void OnCurrentChiTietChanged(LoNhapChiTietFormDto value) => AddChiTietCommand.NotifyCanExecuteChanged();

        partial void OnCurrentPhieuChanged(LoNhapFormDto value)
        {
            OnPropertyChanged(nameof(IsPending));
            AddChiTietCommand.NotifyCanExecuteChanged();
            DeleteChiTietCommand.NotifyCanExecuteChanged();
            DuyetCommand.NotifyCanExecuteChanged();
            TuChoiCommand.NotifyCanExecuteChanged();
        }
    }
}