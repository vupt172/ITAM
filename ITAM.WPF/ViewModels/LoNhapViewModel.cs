using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.Common;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Services;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class LoNhapViewModel : BaseViewModel
    {
        private readonly ILoNhapService _loNhapService;
        private readonly ICatalogService<NhaCungCap> _nhaCungCapService;
        private readonly IHangHoaService _hangHoaService;
        private readonly ICatalogService<LoaiTaiSan> _loaiTaiSanService;
        private readonly ICurrentUserContext _currentUser;

        public override string Title => "Phiếu Nhập Kho";

        public ObservableCollection<LoNhapChiTietFormDto> ChiTiets { get; } = [];
        public ObservableCollection<NhaCungCap> NhaCungCaps { get; } = [];
        public ObservableCollection<HangHoa> HangHoas { get; } = [];
        public ObservableCollection<LoaiTaiSan> LoaiTaiSans { get; } = [];

        private readonly List<long> _chiTietIdsXoa = new();
        private LoNhapChiTietFormDto? _dangSuaChiTiet;

        [ObservableProperty] private LoNhapFormDto currentPhieu = new();
        [ObservableProperty] private LoNhapChiTietFormDto currentChiTiet = new();
        [ObservableProperty] private bool isAddingChiTiet;

        public bool IsPhieuMoi => CurrentPhieu.Id == 0;
        public bool IsPending => CurrentPhieu.Id > 0 && CurrentPhieu.TrangThai == "PENDING" && !IsEditing;
        public decimal TongTien => ChiTiets.Sum(x => x.ThanhTien);

        protected override bool HasSelection => CurrentPhieu.Id > 0 && CurrentPhieu.TrangThai == "PENDING";

        public LoNhapViewModel(ILoNhapService loNhapService, ICatalogService<NhaCungCap> nhaCungCapService,
            IHangHoaService hangHoaService, ICatalogService<LoaiTaiSan> loaiTaiSanService,
            ICurrentUserContext currentUser,IErrorDialogService errorDialogService, INavigationService navigationService) : base(navigationService,errorDialogService)
        {
            _loNhapService = loNhapService; _nhaCungCapService = nhaCungCapService;
            _hangHoaService = hangHoaService; _loaiTaiSanService = loaiTaiSanService; _currentUser = currentUser;
            _ = LoadDanhMucAsync();
        }



        protected override void InitToolbarState()
        {
            CanClose = true;
            CanSearch = true;
        }

        private async Task LoadDanhMucAsync()
        {
            NhaCungCaps.Clear(); foreach (var x in await _nhaCungCapService.GetAllAsync()) NhaCungCaps.Add(x);
            HangHoas.Clear(); foreach (var x in await _hangHoaService.GetAllWithDanhMucAsync()) HangHoas.Add(x);
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
                MaHoaDon = p.MaHoaDon,
                NguoiGiao = p.NguoiGiao,
                NhaCungCapId = p.NhaCungCapId,
                TenNhaCungCap = p.NhaCungCap?.Name,
                NguoiLapPhieuId = p.NguoiLapPhieuId,
                TenNguoiLapPhieu = p.NguoiLapPhieu?.FullName ?? string.Empty,
                TenNguoiDuyet = p.NguoiDuyet?.FullName ?? "Chưa duyệt",   // ⬅ mới
                GhiChu = p.GhiChu,
                TrangThai = p.TrangThai.ToString()
            };

            ChiTiets.Clear();
            foreach (var x in p.ChiTiets)
                ChiTiets.Add(new LoNhapChiTietFormDto
                {
                    Id = x.Id,
                    SoLo = x.SoLo,
                    HangHoaId = x.HangHoaId,
                    MaHangHoa = x.HangHoa.Code,
                    TenVatPham = x.TenVatPham,
                    SoLuongNhap = x.SoLuongNhap,
                    DonGia = x.DonGia,
                    LoaiTaiSanId = x.LoaiTaiSanId,
                    TenLoaiTaiSan = x.LoaiTaiSan.Name          // ⬅ mới
                });

            _chiTietIdsXoa.Clear();
            IsEditing = false;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();
            OnPropertyChanged(nameof(IsPending));
            OnPropertyChanged(nameof(IsPhieuMoi));
            OnPropertyChanged(nameof(TongTien));
            NotifySelectionChanged();
            // Phải gọi lại SAU khi ChiTiets đã có dữ liệu đầy đủ của phiếu mới — vì bước
            // CurrentPhieu = new LoNhapFormDto {...} ở trên đã tự kích hoạt NotifyCanExecuteChanged
            // sớm hơn (lúc ChiTiets vẫn còn dữ liệu của phiếu cũ), nên cần refresh lại lần nữa ở đây.
            DuyetCommand.NotifyCanExecuteChanged();
            TuChoiCommand.NotifyCanExecuteChanged();
        }

        protected override void Add()
        {
            CurrentPhieu = new();
            ChiTiets.Clear();
            _chiTietIdsXoa.Clear();
            CurrentChiTiet = new();
            IsAddingChiTiet = false;
            IsEditing = true;
            OnPropertyChanged(nameof(IsPending));
            OnPropertyChanged(nameof(IsPhieuMoi));
            OnPropertyChanged(nameof(TongTien));
        }

        protected override void Edit()
        {
            if (!HasSelection) return;
            IsEditing = true;
            OnPropertyChanged(nameof(IsPending));
        }

        protected override async void Save()
        {
            if (CurrentPhieu.NhaCungCapId == 0 || _currentUser.Instance == null)
            {
                MessageBox.Show("Chưa chọn nhà cung cấp");
                return;
            }
            if (ChiTiets.Count == 0)
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất 1 dòng chi tiết.");
                return;
            }

            var dto = new SaveLoNhapDto
            {
                Id = CurrentPhieu.Id == 0 ? null : CurrentPhieu.Id,
                NgayNhap = CurrentPhieu.NgayNhap,
                NguoiGiao = CurrentPhieu.NguoiGiao,
                MaHoaDon = CurrentPhieu.MaHoaDon,
                NhaCungCapId = CurrentPhieu.NhaCungCapId,
                NguoiLapPhieuId = _currentUser.Instance.Id,
                GhiChu = CurrentPhieu.GhiChu,
                ChiTiets = ChiTiets.Select(x => new SaveLoNhapChiTietDto
                {
                    Id = x.Id,
                    HangHoaId = x.HangHoaId,
                    TenVatPham = x.TenVatPham,      // ⬅ mới — gửi kèm tên do người dùng tự nhập
                    SoLuongNhap = x.SoLuongNhap,
                    DonGia = x.DonGia,
                    LoaiTaiSanId = x.LoaiTaiSanId
                }).ToList(),
                ChiTietIdsXoa = _chiTietIdsXoa.ToList()
            };

            try
            {
                var id = await _loNhapService.LuuPhieuNhapAsync(dto);
                await LoadPhieuAsync(id);
            }
            catch (InvalidBusinessRuleException ex) { MessageBox.Show(ex.Message); }
            catch(Exception ex) { _errorDialogService.Show(ex); }
        }

        protected override void Cancel()
        {
            IsEditing = false;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();

            if (CurrentPhieu.Id == 0)
            {
                CurrentPhieu = new();
                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();
                OnPropertyChanged(nameof(TongTien));
            }
            else
            {
                _ = LoadPhieuAsync(CurrentPhieu.Id); // bỏ mọi thay đổi chưa lưu, tải lại nguyên trạng từ DB
            }
        }


        protected override async void Search()
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
        protected override async void Delete()
        {
        
            var result = MessageBox.Show($"Xóa phiếu nhập '{CurrentPhieu.SoLo}'?", "Xác nhận xóa",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                await _loNhapService.DeleteAsync(CurrentPhieu.Id);
                CurrentPhieu = new();
                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();
                OnPropertyChanged(nameof(TongTien));
            }
            catch(InvalidBusinessRuleException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { 
                _errorDialogService.Show(ex); 
            }
        }

        [RelayCommand]
        private void ThemHangHoa()
        {
            if (!IsEditing) return;
            CurrentChiTiet = new();
            _dangSuaChiTiet = null;
            IsAddingChiTiet = true;
        }

        [RelayCommand]
        private void SuaDong(LoNhapChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null) return;
            CurrentChiTiet = new()
            {
                Id = dong.Id,
                HangHoaId = dong.HangHoaId,
                MaHangHoa = dong.MaHangHoa,
                SoLo = dong.SoLo,
                TenVatPham = dong.TenVatPham,
                SoLuongNhap = dong.SoLuongNhap,
                DonGia = dong.DonGia,
                LoaiTaiSanId = dong.LoaiTaiSanId,
                TenLoaiTaiSan = dong.TenLoaiTaiSan          // ⬅ mới
            };
            _dangSuaChiTiet = dong;
            IsAddingChiTiet = true;
        }

        [RelayCommand]
        private void XoaDong(LoNhapChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null) return;
            if (dong.Id.HasValue)
                _chiTietIdsXoa.Add(dong.Id.Value);
            ChiTiets.Remove(dong);
            OnPropertyChanged(nameof(TongTien));
            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void LuuDong()
        {
            if (CurrentChiTiet.HangHoaId == 0 || CurrentChiTiet.SoLuongNhap <= 0)
            {
                MessageBox.Show("Chọn hàng hóa và nhập số lượng hợp lệ.");
                return;
            }

            var hangHoa = HangHoas.FirstOrDefault(x => x.Id == CurrentChiTiet.HangHoaId);
            if (hangHoa == null) { MessageBox.Show("Không tìm thấy hàng hóa."); return; }

            if (CurrentChiTiet.LoaiTaiSanId <= 0)
            {
                MessageBox.Show("Bắt buộc chọn Loại Tài Sản.");
                return;
            }

            var loaiTaiSan = LoaiTaiSans.FirstOrDefault(x => x.Id == CurrentChiTiet.LoaiTaiSanId);
            if (loaiTaiSan == null) { MessageBox.Show("Không tìm thấy Loại Tài Sản đã chọn."); return; }

            if (CurrentChiTiet.DonGia != 0 && (CurrentChiTiet.DonGia < loaiTaiSan.MinValue || CurrentChiTiet.DonGia > loaiTaiSan.MaxValue))
            {
                MessageBox.Show($"Đơn giá {CurrentChiTiet.DonGia:N0} không nằm trong ngưỡng của '{loaiTaiSan.Name}' " +
                                 $"({loaiTaiSan.MinValue:N0} - {loaiTaiSan.MaxValue:N0}). Chọn lại Loại Tài Sản phù hợp.");
                return;
            }
            CurrentChiTiet.TenLoaiTaiSan = loaiTaiSan.Name;

            if (string.IsNullOrWhiteSpace(CurrentChiTiet.TenVatPham))
                CurrentChiTiet.TenVatPham = hangHoa.Name;

            CurrentChiTiet.MaHangHoa = hangHoa.Code;

            if (_dangSuaChiTiet != null)
            {
                var index = ChiTiets.IndexOf(_dangSuaChiTiet);
                if (index >= 0) ChiTiets[index] = CurrentChiTiet;
            }
            else
            {
                ChiTiets.Add(CurrentChiTiet);
            }

            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();
            OnPropertyChanged(nameof(TongTien));
            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void HuyDong()
        {
            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();
        }

        [RelayCommand(CanExecute = nameof(CanDuyet))]
        private async Task Duyet()
        {
            try { await _loNhapService.DuyetLoNhapAsync(CurrentPhieu.Id, _currentUser.Instance.Id); await LoadPhieuAsync(CurrentPhieu.Id); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private bool CanDuyet() => IsPending && ChiTiets.Count > 0;

        [RelayCommand(CanExecute = nameof(CanTuChoi))]
        private async Task TuChoi()
        {
            await _loNhapService.TuChoiLoNhapAsync(CurrentPhieu.Id, _currentUser.Instance.Id);
            await LoadPhieuAsync(CurrentPhieu.Id);
        }
        private bool CanTuChoi() => IsPending;

        partial void OnCurrentPhieuChanged(LoNhapFormDto value)
        {
            OnPropertyChanged(nameof(IsPending));
            OnPropertyChanged(nameof(IsPhieuMoi));
            DuyetCommand.NotifyCanExecuteChanged();
            TuChoiCommand.NotifyCanExecuteChanged();
            NotifySelectionChanged();
        }
    }
}