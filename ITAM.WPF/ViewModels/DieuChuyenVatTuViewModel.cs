using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.Shared.Constants;
using ITAM.WPF.Helpers;
using ITAM.WPF.Services.Interfaces;
using ITAM.WPF.ViewModels.Dialogs;
using ITAM.WPF.Views;
using ITAM.WPF.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels
{
    public partial class DieuChuyenVatTuViewModel : BaseViewModel
    {
        private readonly IDieuChuyenVatTuService _dieuChuyenVatTuService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly ICatalogService<ViTriTaiSan> _viTriTaiSanService;
        private readonly ICurrentUserContext _currentUser;

        public override string Title => "Phiếu Điều Chuyển Vật Tư";

        public ObservableCollection<DieuChuyenVatTuChiTietFormDto> ChiTiets { get; } = [];

        /// <summary>Phòng ban hợp lệ cho Phòng Ban Chuyển Đi (đã loại 3 kho đặc biệt).</summary>
        public ObservableCollection<PhongBan> PhongBansChuyenDi { get; } = [];

        /// <summary>Phòng ban hợp lệ cho Phòng Ban Chuyển Đến (đã loại 3 kho đặc biệt).</summary>
        public ObservableCollection<PhongBan> PhongBansChuyenDen { get; } = [];
        public SearchableCollectionView<PhongBan> PhongBansChuyenDenView { get; set; }

        /// <summary>Vị trí thuộc Phòng Ban Chuyển Đến.</summary>
        public ObservableCollection<ViTriTaiSan> ViTriTaiSansChuyenDen { get; } = [];

        /// <summary>Các cặp (Hàng hóa, Vị trí) còn tồn khả dụng tại Phòng Ban Chuyển Đi.</summary>
        public ObservableCollection<VatTuKhaDungDto> VatTusKhaDung { get; } = [];

        private List<ViTriTaiSan> _tatCaViTriTaiSan = new();
        private DieuChuyenVatTuChiTietFormDto? _dangSuaChiTiet;
        private readonly List<long> _chiTietIdsXoa = new();

        [ObservableProperty]
        private DieuChuyenVatTuFormDto currentPhieu = new();

        [ObservableProperty]
        private DieuChuyenVatTuChiTietFormDto currentChiTiet = new();

        [ObservableProperty]
        private bool isAddingChiTiet;

        [ObservableProperty]
        private long phongBanChuyenDenChonId;

        [ObservableProperty]
        private VatTuKhaDungDto? vatTuDangChon;

        public bool IsPhieuMoi => CurrentPhieu.Id == 0;

        public bool IsPending =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            !IsEditing;

        private long? PhongBanMacDinhId => _currentUser.Instance?.PhongBan?.Id;

        /// <summary>Được sửa/xóa khi phiếu PENDING và Phòng Ban Chuyển Đi là phòng ban mặc định của user.</summary>
        public bool CoTheSuaPhieu =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            CurrentPhieu.PhongBanChuyenDiId == PhongBanMacDinhId;

        /// <summary>Chỉ đổi Phòng Ban Chuyển Đến khi đang sửa và chưa có dòng chi tiết.</summary>
        public bool CoTheDoiPhongBanChuyenDen => IsEditing && ChiTiets.Count == 0;

        /// <summary>Thêm dòng mới (chọn vật tư) thay vì sửa dòng đã có.</summary>
        public bool DangThemMoiDong => _dangSuaChiTiet == null;
        public bool DangSuaDong => !DangThemMoiDong;

        protected override bool HasSelection =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING";

        public DieuChuyenVatTuViewModel(
            IDieuChuyenVatTuService dieuChuyenVatTuService,
            ICatalogService<PhongBan> phongBanService,
            ICatalogService<ViTriTaiSan> viTriTaiSanService,
            ICurrentUserContext currentUser,
            IErrorDialogService errorDialogService,
            INavigationService navigationService)
            : base(navigationService, errorDialogService)
        {
            _dieuChuyenVatTuService = dieuChuyenVatTuService;
            _phongBanService = phongBanService;
            _viTriTaiSanService = viTriTaiSanService;
            _currentUser = currentUser;

            PhongBansChuyenDenView = new SearchableCollectionView<PhongBan>(PhongBansChuyenDen, x => x.Code, x => x.Name);
            _ = LoadDanhMucAsync();
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
            CanSearch = true;
        }

        // ------------------------------------------------------------------
        // Tải dữ liệu
        // ------------------------------------------------------------------

        private async Task LoadDanhMucAsync()
        {
            try
            {
                var tatCaPhongBan = await _phongBanService.GetAllAsync();
                _tatCaViTriTaiSan = (await _viTriTaiSanService.GetAllAsync()).ToList();

                var phongBansHopLe = tatCaPhongBan
                    .Where(pb => !VatTuKhoRules.PhongBanBiChan.Contains(pb.Code))
                    .ToList();

                PhongBansChuyenDi.Clear();
                foreach (var pb in phongBansHopLe)
                    PhongBansChuyenDi.Add(pb);

                PhongBansChuyenDen.Clear();
                foreach (var pb in phongBansHopLe)
                    PhongBansChuyenDen.Add(pb);

                if (PhongBanMacDinhId is long phongBanId)
                    await LoadVatTuKhaDungAsync(phongBanId, null);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        private async Task LoadVatTuKhaDungAsync(long phongBanId, long? phieuId)
        {
            VatTusKhaDung.Clear();

            foreach (var x in await _dieuChuyenVatTuService.GetVatTuKhaDungAsync(phongBanId, phieuId))
                VatTusKhaDung.Add(x);
        }

        partial void OnPhongBanChuyenDenChonIdChanged(long value)
        {
            ViTriTaiSansChuyenDen.Clear();

            foreach (var viTri in _tatCaViTriTaiSan.Where(v => v.PhongBanId == value))
                ViTriTaiSansChuyenDen.Add(viTri);
        }

        private async Task LoadPhieuAsync(long dieuChuyenVatTuId)
        {
            var p = await _dieuChuyenVatTuService.GetByIdAsync(dieuChuyenVatTuId);
            if (p == null)
                return;

            CurrentPhieu = new DieuChuyenVatTuFormDto
            {
                Id = p.Id,
                SoPhieu = p.SoPhieu,
                NgayTao = p.NgayTao,
                NguoiTaoId = p.NguoiTaoId,
                TenNguoiTao = p.NguoiTao?.FullName ?? string.Empty,
                PhongBanChuyenDiId = p.PhongBanChuyenDiId,
                TenPhongBanChuyenDi = p.PhongBanChuyenDi?.Name ?? string.Empty,
                PhongBanChuyenDenId = p.PhongBanChuyenDenId,
                TenPhongBanChuyenDen = p.PhongBanChuyenDen?.Name ?? string.Empty,
                NguoiGiao = p.NguoiGiao,
                NguoiNhan = p.NguoiNhan,
                TenNguoiDuyet = p.NguoiDuyet?.FullName ?? "Chưa duyệt",
                NgayDuyet = p.NgayDuyet,
                GhiChu = p.GhiChu,
                TrangThai = p.TrangThai.ToString()
            };

            PhongBanChuyenDenChonId = p.PhongBanChuyenDenId;
            OnPhongBanChuyenDenChonIdChanged(PhongBanChuyenDenChonId);

            // Tồn khả dụng tính theo phòng ban thực tế của phiếu, bỏ qua phần chính phiếu này đang giữ
            await LoadVatTuKhaDungAsync(p.PhongBanChuyenDiId, p.Id);

            ChiTiets.Clear();
            foreach (var x in p.ChiTiets)
            {
                ChiTiets.Add(new DieuChuyenVatTuChiTietFormDto
                {
                    Id = x.Id,
                    HangHoaId = x.HangHoaId,
                    MaHangHoa = x.HangHoa.Code,
                    TenHangHoa = x.HangHoa.Name,            // TODO xác nhận: HangHoa có Name?
                    ViTriChuyenDiId = x.ViTriChuyenDiId,
                    TenViTriChuyenDi = x.ViTriChuyenDi.Name,
                    ViTriChuyenDenId = x.ViTriChuyenDenId,
                    TenViTriChuyenDen = x.ViTriChuyenDen.Name,
                    SoLuong = x.SoLuong,
                    GhiChu = x.GhiChu
                });
            }

            _chiTietIdsXoa.Clear();
            _dangSuaChiTiet = null;

            IsEditing = false;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();

            NotifyPhieuStateChanged();
        }

        // ------------------------------------------------------------------
        // Toolbar
        // ------------------------------------------------------------------

        protected override async void Add()
        {
            var phongBanMacDinh = _currentUser.Instance?.PhongBan;

            if (phongBanMacDinh != null && VatTuKhoRules.PhongBanBiChan.Contains(phongBanMacDinh.Code))
            {
                MessageBox.Show(
                    $"Phòng ban mặc định '{phongBanMacDinh.Name}' không được điều chuyển vật tư.");
                return;
            }

            CurrentPhieu = new();

            if (phongBanMacDinh != null)
            {
                CurrentPhieu.PhongBanChuyenDiId = phongBanMacDinh.Id;
                CurrentPhieu.TenPhongBanChuyenDi = phongBanMacDinh.Name;

                try
                {
                    await LoadVatTuKhaDungAsync(phongBanMacDinh.Id, null);
                }
                catch (Exception ex)
                {
                    _errorDialogService.Show(ex);
                }
            }

            PhongBanChuyenDenChonId = 0;

            ChiTiets.Clear();
            _chiTietIdsXoa.Clear();
            _dangSuaChiTiet = null;

            CurrentChiTiet = new();
            IsAddingChiTiet = false;

            IsEditing = true;
            NotifyPhieuStateChanged();
        }

        protected override void Edit()
        {
            if (!CoTheSuaPhieu)
            {
                MessageBox.Show("Bạn không được sửa phiếu của phòng ban khác hoặc phiếu đã được duyệt.");
                return;
            }

            IsEditing = true;
            NotifyPhieuStateChanged();
        }

        protected override async void Save()
        {
            if (_currentUser.Instance == null)
                return;

            if (CurrentPhieu.PhongBanChuyenDiId <= 0)
            {
                MessageBox.Show("Không xác định được Phòng Ban Chuyển Đi của bạn.");
                return;
            }

            if (!IsPhieuMoi && CurrentPhieu.PhongBanChuyenDiId != PhongBanMacDinhId)
            {
                MessageBox.Show("Bạn không được sửa phiếu của phòng ban khác.");
                return;
            }

            if (PhongBanChuyenDenChonId <= 0)
            {
                MessageBox.Show("Chưa chọn Phòng Ban Chuyển Đến.");
                return;
            }

            if (ChiTiets.Count == 0)
            {
                MessageBox.Show("Phiếu điều chuyển phải có ít nhất 1 vật tư.");
                return;
            }

            var dto = new SaveDieuChuyenVatTuDto
            {
                Id = CurrentPhieu.Id == 0 ? null : CurrentPhieu.Id,
                NguoiTaoId = _currentUser.Instance.Id,
                PhongBanChuyenDiId = CurrentPhieu.PhongBanChuyenDiId,
                PhongBanChuyenDenId = PhongBanChuyenDenChonId,
                NguoiGiao = CurrentPhieu.NguoiGiao,
                NguoiNhan = CurrentPhieu.NguoiNhan,
                GhiChu = CurrentPhieu.GhiChu,
                ChiTiets = ChiTiets.Select(x => new SaveDieuChuyenVatTuChiTietDto
                {
                    Id = x.Id,
                    HangHoaId = x.HangHoaId,
                    ViTriChuyenDiId = x.ViTriChuyenDiId,
                    ViTriChuyenDenId = x.ViTriChuyenDenId,
                    SoLuong = x.SoLuong,
                    GhiChu = x.GhiChu
                }).ToList(),
                ChiTietIdsXoa = _chiTietIdsXoa.ToList()
            };

            try
            {
                var id = await _dieuChuyenVatTuService.LuuPhieuAsync(dto);
                await LoadPhieuAsync(id);
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override void Cancel()
        {
            IsEditing = false;
            IsAddingChiTiet = false;
            _dangSuaChiTiet = null;
            CurrentChiTiet = new();
            VatTuDangChon = null;

            if (CurrentPhieu.Id == 0)
            {
                CurrentPhieu = new();
                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();
                PhongBanChuyenDenChonId = 0;
            }
            else
            {
                _ = LoadPhieuAsync(CurrentPhieu.Id);
            }

            NotifyPhieuStateChanged();
        }

        protected override async void Search()
        {
            var dialogVm = App.Services.GetRequiredService<DieuChuyenVatTuSearchViewModel>();
            var window = App.Services.GetRequiredService<DieuChuyenVatTuSearchWindow>();

            window.DataContext = dialogVm;
            window.Owner = Application.Current.MainWindow;

            dialogVm.RequestClose += (s, chosen) =>
            {
                window.DialogResult = chosen;
                window.Close();
            };

            if (window.ShowDialog() == true &&
                dialogVm.SelectedDieuChuyenVatTuId is long id)
            {
                try
                {
                    await LoadPhieuAsync(id);
                }
                catch (Exception ex)
                {
                    _errorDialogService.Show(ex);
                }
            }
        }

        protected override async void Delete()
        {
            if (!CoTheSuaPhieu)
            {
                MessageBox.Show("Bạn không được xóa phiếu của phòng ban khác hoặc phiếu đã được duyệt.");
                return;
            }

            var result = MessageBox.Show(
                $"Xóa phiếu điều chuyển vật tư '{CurrentPhieu.SoPhieu}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _dieuChuyenVatTuService.DeleteAsync(CurrentPhieu.Id);

                CurrentPhieu = new();
                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();

                NotifyPhieuStateChanged();
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        // ------------------------------------------------------------------
        // Dòng chi tiết
        // ------------------------------------------------------------------

        [RelayCommand]
        private void ThemVatTu()
        {
            if (!IsEditing)
                return;

            if (PhongBanChuyenDenChonId <= 0)
            {
                MessageBox.Show("Chọn Phòng Ban Chuyển Đến trước khi thêm vật tư.");
                return;
            }

            _dangSuaChiTiet = null;
            VatTuDangChon = null;
            CurrentChiTiet = new();
            IsAddingChiTiet = true;

            NotifyDangSuaDongChanged();
        }

        [RelayCommand]
        private void SuaDong(DieuChuyenVatTuChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null)
                return;

            // Gán _dangSuaChiTiet trước để OnCurrentChiTietChanged báo đúng trạng thái
            _dangSuaChiTiet = dong;

            CurrentChiTiet = new()
            {
                Id = dong.Id,
                HangHoaId = dong.HangHoaId,
                MaHangHoa = dong.MaHangHoa,
                TenHangHoa = dong.TenHangHoa,
                ViTriChuyenDiId = dong.ViTriChuyenDiId,
                TenViTriChuyenDi = dong.TenViTriChuyenDi,
                ViTriChuyenDenId = dong.ViTriChuyenDenId,
                TenViTriChuyenDen = dong.TenViTriChuyenDen,
                SoLuong = dong.SoLuong,
                GhiChu = dong.GhiChu
            };

            IsAddingChiTiet = true;
            NotifyDangSuaDongChanged();
        }

        [RelayCommand]
        private void XoaDong(DieuChuyenVatTuChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null)
                return;

            if (dong.Id is long id)
                _chiTietIdsXoa.Add(id);

            ChiTiets.Remove(dong);

            OnPropertyChanged(nameof(CoTheDoiPhongBanChuyenDen));
            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void LuuDong()
        {
            int? tonKhaDung;

            if (_dangSuaChiTiet == null)
            {
                // Thêm dòng mới
                if (VatTuDangChon == null)
                {
                    MessageBox.Show("Chọn vật tư cần điều chuyển.");
                    return;
                }

                if (ChiTiets.Any(x =>
                        x.HangHoaId == VatTuDangChon.HangHoaId &&
                        x.ViTriChuyenDiId == VatTuDangChon.ViTriTaiSanId))
                {
                    MessageBox.Show("Vật tư này tại vị trí này đã có trong phiếu — hãy sửa dòng đã có.");
                    return;
                }

                CurrentChiTiet.HangHoaId = VatTuDangChon.HangHoaId;
                CurrentChiTiet.MaHangHoa = VatTuDangChon.MaHangHoa;
                CurrentChiTiet.TenHangHoa = VatTuDangChon.TenHangHoa;
                CurrentChiTiet.ViTriChuyenDiId = VatTuDangChon.ViTriTaiSanId;
                CurrentChiTiet.TenViTriChuyenDi = VatTuDangChon.TenViTri;
                tonKhaDung = VatTuDangChon.TonKhaDung;
            }
            else
            {
                // Sửa dòng: vật tư và vị trí đi là snapshot, không đổi
                tonKhaDung = VatTusKhaDung.FirstOrDefault(v =>
                    v.HangHoaId == CurrentChiTiet.HangHoaId &&
                    v.ViTriTaiSanId == CurrentChiTiet.ViTriChuyenDiId)?.TonKhaDung;
            }

            if (CurrentChiTiet.ViTriChuyenDenId == 0)
            {
                MessageBox.Show("Chọn Vị Trí Chuyển Đến.");
                return;
            }

            if (CurrentChiTiet.ViTriChuyenDenId == CurrentChiTiet.ViTriChuyenDiId)
            {
                MessageBox.Show("Vị Trí Chuyển Đến không được trùng Vị Trí Chuyển Đi.");
                return;
            }

            if (CurrentChiTiet.SoLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0.");
                return;
            }

            // Kiểm tra sơ bộ phía client; service vẫn kiểm tra lại khi Lưu
            if (tonKhaDung is int ton && CurrentChiTiet.SoLuong > ton)
            {
                MessageBox.Show($"Số lượng vượt tồn khả dụng ({ton}).");
                return;
            }

            CurrentChiTiet.TenViTriChuyenDen =
                ViTriTaiSansChuyenDen.FirstOrDefault(v => v.Id == CurrentChiTiet.ViTriChuyenDenId)?.Name
                ?? string.Empty;

            if (_dangSuaChiTiet != null)
            {
                var index = ChiTiets.IndexOf(_dangSuaChiTiet);
                if (index >= 0)
                    ChiTiets[index] = CurrentChiTiet;
            }
            else
            {
                ChiTiets.Add(CurrentChiTiet);
            }

            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;
            VatTuDangChon = null;
            CurrentChiTiet = new();

            OnPropertyChanged(nameof(CoTheDoiPhongBanChuyenDen));
            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void HuyDong()
        {
            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;
            VatTuDangChon = null;
            CurrentChiTiet = new();
        }

        // ------------------------------------------------------------------
        // Duyệt / Từ chối
        // ------------------------------------------------------------------

        [RelayCommand(CanExecute = nameof(CanDuyet))]
        private async Task Duyet()
        {
            try
            {
                await _dieuChuyenVatTuService.DuyetPhieuAsync(CurrentPhieu.Id, _currentUser.Instance!.Id);
                await LoadPhieuAsync(CurrentPhieu.Id);
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        private bool CanDuyet() => IsPending && ChiTiets.Count > 0;

        [RelayCommand(CanExecute = nameof(CanTuChoi))]
        private async Task TuChoi()
        {
            try
            {
                await _dieuChuyenVatTuService.TuChoiPhieuAsync(CurrentPhieu.Id, _currentUser.Instance!.Id);
                await LoadPhieuAsync(CurrentPhieu.Id);
            }
            catch (InvalidBusinessRuleException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        private bool CanTuChoi() => IsPending;

        // ------------------------------------------------------------------
        // Thông báo trạng thái UI
        // ------------------------------------------------------------------

        partial void OnCurrentChiTietChanged(DieuChuyenVatTuChiTietFormDto value) =>
            NotifyDangSuaDongChanged();

        partial void OnCurrentPhieuChanged(DieuChuyenVatTuFormDto value) =>
            NotifyPhieuStateChanged();

        private void NotifyDangSuaDongChanged()
        {
            OnPropertyChanged(nameof(DangThemMoiDong));
            OnPropertyChanged(nameof(DangSuaDong));
        }

        private void NotifyPhieuStateChanged()
        {
            OnPropertyChanged(nameof(IsPending));
            OnPropertyChanged(nameof(IsPhieuMoi));
            OnPropertyChanged(nameof(CoTheSuaPhieu));
            OnPropertyChanged(nameof(CoTheDoiPhongBanChuyenDen));

            DuyetCommand.NotifyCanExecuteChanged();
            TuChoiCommand.NotifyCanExecuteChanged();

            NotifySelectionChanged();
        }
    }
}