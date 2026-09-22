using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums;
using ITAM.Domain.Exceptions;
using ITAM.Domain.Interfaces;
using ITAM.Shared.Constants;
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
    public partial class DieuChuyenViewModel : BaseViewModel
    {
        private readonly IDieuChuyenService _dieuChuyenService;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly ICatalogService<ViTriTaiSan> _viTriTaiSanService;
        private readonly ITaiSanDinhDanhService _taiSanDinhDanhService;
        private readonly ICurrentUserContext _currentUser;

        public override string Title => "Phiếu Điều Chuyển";

        public ObservableCollection<DieuChuyenChiTietFormDto> ChiTiets { get; } = [];

        /// <summary>
        /// Tất cả phòng ban hợp lệ dùng cho Phòng Ban Chuyển Đi.
        /// Khi tạo phiếu mới sẽ tự chọn phòng ban mặc định của user.
        /// Khi mở phiếu cũ sẽ hiển thị phòng ban thực tế của phiếu.
        /// </summary>
        public ObservableCollection<PhongBan> PhongBansChuyenDiHopLe { get; } = [];

        /// <summary>
        /// Tất cả phòng ban hợp lệ dùng cho Phòng Ban Chuyển Đến.
        /// </summary>
        public ObservableCollection<PhongBan> PhongBansChuyenDenHopLe { get; } = [];

        /// <summary>
        /// Vị trí tài sản thuộc Phòng Ban Chuyển Đến.
        /// </summary>
        public ObservableCollection<ViTriTaiSan> ViTriTaiSansChuyenDen { get; } = [];

        /// <summary>
        /// Tài sản đang thuộc Phòng Ban Chuyển Đi.
        /// </summary>
        public ObservableCollection<TaiSanDinhDanh> TaiSansTaiPhongBanDi { get; } = [];

        private List<ViTriTaiSan> _tatCaViTriTaiSan = new();
        private DieuChuyenChiTietFormDto? _dangSuaChiTiet;
        private readonly List<long> _chiTietIdsXoa = new();

        [ObservableProperty]
        private DieuChuyenFormDto currentPhieu = new();

        [ObservableProperty]
        private DieuChuyenChiTietFormDto currentChiTiet = new();

        [ObservableProperty]
        private bool isAddingChiTiet;

        [ObservableProperty]
        private long phongBanChuyenDenChonId;

        [ObservableProperty]
        private TaiSanDinhDanh? taiSanDangChon;

        public bool IsPhieuMoi => CurrentPhieu.Id == 0;

        public bool IsPending =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            !IsEditing;

        /// <summary>
        /// Phòng ban mặc định của user đăng nhập.
        /// </summary>
        private long? PhongBanMacDinhId =>
            _currentUser.Instance?.PhongBan?.Id;

        /// <summary>
        /// User có được sửa phiếu này hay không.
        ///
        /// Điều kiện:
        /// - Phiếu tồn tại
        /// - Phiếu đang PENDING
        /// - Phòng ban chuyển đi của phiếu là phòng ban mặc định của user
        /// </summary>
        public bool CoTheSuaPhieu =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            CurrentPhieu.PhongBanChuyenDiId == PhongBanMacDinhId;

        /// <summary>
        /// Chỉ cho đổi Phòng Ban Chuyển Đến
        /// khi đang sửa và chưa có dòng chi tiết.
        /// </summary>
        public bool CoTheDoiPhongBanChuyenDen =>
            IsEditing &&
            ChiTiets.Count == 0;

        /// <summary>
        /// Khi thêm dòng mới thì cho phép chọn tài sản.
        /// </summary>
        public bool DangThemMoiDong => CurrentChiTiet.Id == null;

        /// <summary>
        /// Khi sửa dòng cũ thì tài sản và vị trí chuyển đi là snapshot,
        /// không cho thay đổi.
        /// </summary>
        public bool DangSuaDong => !DangThemMoiDong;

        /// <summary>
        /// Selection dùng cho các thao tác duyệt/từ chối.
        /// Không phụ thuộc phòng ban chuyển đi.
        /// </summary>
        protected override bool HasSelection =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING";

        public DieuChuyenViewModel(
            IDieuChuyenService dieuChuyenService,
            ICatalogService<PhongBan> phongBanService,
            ICatalogService<ViTriTaiSan> viTriTaiSanService,
            ITaiSanDinhDanhService taiSanDinhDanhService,
            ICurrentUserContext currentUser,
            IErrorDialogService errorDialogService,
            INavigationService navigationService)
            : base(navigationService, errorDialogService)
        {
            _dieuChuyenService = dieuChuyenService;
            _phongBanService = phongBanService;
            _viTriTaiSanService = viTriTaiSanService;
            _taiSanDinhDanhService = taiSanDinhDanhService;
            _currentUser = currentUser;

            _ = LoadDanhMucAsync();
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
            CanSearch = true;
        }

        private async Task LoadDanhMucAsync()
        {
            var tatCaPhongBan = await _phongBanService.GetAllAsync();

            _tatCaViTriTaiSan =
                (await _viTriTaiSanService.GetAllAsync()).ToList();

            var phongBansHopLe = tatCaPhongBan
                .Where(pb =>
                    pb.Code != PhongBanCodes.KHO_THAT_LAC &&
                    pb.Code != PhongBanCodes.KHO_THANH_LY)
                .ToList();

            // Phòng Ban Chuyển Đi
            PhongBansChuyenDiHopLe.Clear();

            foreach (var pb in phongBansHopLe)
                PhongBansChuyenDiHopLe.Add(pb);

            // Phòng Ban Chuyển Đến
            PhongBansChuyenDenHopLe.Clear();

            foreach (var pb in phongBansHopLe)
                PhongBansChuyenDenHopLe.Add(pb);

            // Tải tài sản thuộc phòng ban mặc định của user.
            if (PhongBanMacDinhId is long phongBanId)
                await LoadTaiSanTheoPhongBanDiAsync(phongBanId);
        }

        private async Task LoadTaiSanTheoPhongBanDiAsync(long phongBanId)
        {
            TaiSansTaiPhongBanDi.Clear();

            var viTriCuaPhongBan = _tatCaViTriTaiSan
                .Where(v => v.PhongBanId == phongBanId)
                .ToList();

            foreach (var viTri in viTriCuaPhongBan)
            {
                var list =
                    await _taiSanDinhDanhService.GetByViTriAsync(viTri.Id);

                foreach (var taiSan in list.Where(t =>
                    t.TrangThaiTaiSan != TrangThaiTaiSan.MAINTENANCE &&
                    t.TrangThaiTaiSan != TrangThaiTaiSan.LOST))
                {
                    TaiSansTaiPhongBanDi.Add(taiSan);
                }
            }
        }

        partial void OnPhongBanChuyenDenChonIdChanged(long value)
        {
            ViTriTaiSansChuyenDen.Clear();

            foreach (var viTri in _tatCaViTriTaiSan
                .Where(v => v.PhongBanId == value))
            {
                ViTriTaiSansChuyenDen.Add(viTri);
            }
        }

        private async Task LoadPhieuAsync(long dieuChuyenId)
        {
            var p = await _dieuChuyenService.GetByIdAsync(dieuChuyenId);

            if (p == null)
                return;

            CurrentPhieu = new DieuChuyenFormDto
            {
                Id = p.Id,
                SoPhieu = p.SoPhieu,
                NgayTao = p.NgayTao,
                NguoiTaoId = p.NguoiTaoId,

                TenNguoiTao =
                    p.NguoiTao?.FullName ?? string.Empty,

                PhongBanChuyenDiId =
                    p.PhongBanChuyenDiId,

                TenPhongBanChuyenDi =
                    p.PhongBanChuyenDi?.Name ?? string.Empty,

                PhongBanChuyenDenId =
                    p.PhongBanChuyenDenId,

                TenPhongBanChuyenDen =
                    p.PhongBanChuyenDen?.Name ?? string.Empty,

                NguoiGiao = p.NguoiGiao,
                NguoiNhan = p.NguoiNhan,

                TenNguoiDuyet =
                    p.NguoiDuyet?.FullName ?? "Chưa duyệt",

                NgayDuyet = p.NgayDuyet,
                GhiChu = p.GhiChu,
                TrangThai = p.TrangThai.ToString()
            };

            // Phòng ban chuyển đến
            PhongBanChuyenDenChonId =
                p.PhongBanChuyenDenId;

            // Tải vị trí chuyển đến
            OnPhongBanChuyenDenChonIdChanged(
                PhongBanChuyenDenChonId);

            // Tải tài sản theo phòng ban thực tế của phiếu.
            // Điều này quan trọng khi user mở phiếu của phòng ban khác.
            await LoadTaiSanTheoPhongBanDiAsync(
                p.PhongBanChuyenDiId);

            ChiTiets.Clear();

            foreach (var x in p.ChiTiets)
            {
                ChiTiets.Add(new DieuChuyenChiTietFormDto
                {
                    Id = x.Id,

                    TaiSanDinhDanhId =
                        x.TaiSanDinhDanhId,

                    MaTaiSan =
                        x.TaiSanDinhDanh.Code,

                    TenTaiSan =
                        x.TaiSanDinhDanh.Name,

                    ViTriChuyenDiId =
                        x.ViTriChuyenDiId,

                    TenViTriChuyenDi =
                        x.ViTriChuyenDi.Name,

                    ViTriChuyenDenId =
                        x.ViTriChuyenDenId,

                    TenViTriChuyenDen =
                        x.ViTriChuyenDen.Name,

                    GhiChu = x.GhiChu
                });
            }

            _chiTietIdsXoa.Clear();

            IsEditing = false;
            IsAddingChiTiet = false;
            CurrentChiTiet = new();

            NotifyPhieuStateChanged();
        }

        protected override void Add()
        {
            CurrentPhieu = new();

            // Khi tạo phiếu mới:
            // Phòng Ban Chuyển Đi = phòng ban mặc định của user.
            var phongBanMacDinh =
                _currentUser.Instance?.PhongBan;

            if (phongBanMacDinh != null)
            {
                CurrentPhieu.PhongBanChuyenDiId =
                    phongBanMacDinh.Id;

                CurrentPhieu.TenPhongBanChuyenDi =
                    phongBanMacDinh.Name;
            }

            PhongBanChuyenDenChonId = 0;

            ChiTiets.Clear();
            _chiTietIdsXoa.Clear();

            CurrentChiTiet = new();
            IsAddingChiTiet = false;

            IsEditing = true;

            NotifyPhieuStateChanged();
        }

        protected override void Edit()
        {
            if (!CoTheSuaPhieu)
            {
               MessageBox.Show(
                    "Bạn không được sửa phiếu của phòng ban khác hoặc phiếu đã được duyệt.");
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
                MessageBox.Show(
                    "Không xác định được Phòng Ban Chuyển Đi của bạn.");
                return;
            }

            // Không cho user sửa phiếu của phòng ban khác.
            if (!IsPhieuMoi &&
                CurrentPhieu.PhongBanChuyenDiId != PhongBanMacDinhId)
            {
                MessageBox.Show(
                    "Bạn không được sửa phiếu của phòng ban khác.");
                return;
            }

            if (PhongBanChuyenDenChonId <= 0)
            {
                MessageBox.Show(
                    "Chưa chọn Phòng Ban Chuyển Đến.");
                return;
            }

            if (ChiTiets.Count == 0)
            {
                MessageBox.Show(
                    "Phiếu điều chuyển phải có ít nhất 1 tài sản.");
                return;
            }

            var dto = new SaveDieuChuyenDto
            {
                Id = CurrentPhieu.Id == 0
                    ? null
                    : CurrentPhieu.Id,

                NguoiTaoId =
                    _currentUser.Instance.Id,

                PhongBanChuyenDiId =
                    CurrentPhieu.PhongBanChuyenDiId,

                PhongBanChuyenDenId =
                    PhongBanChuyenDenChonId,

                NguoiGiao =
                    CurrentPhieu.NguoiGiao,

                NguoiNhan =
                    CurrentPhieu.NguoiNhan,

                GhiChu =
                    CurrentPhieu.GhiChu,

                ChiTiets =
                    ChiTiets.Select(x =>
                        new SaveDieuChuyenChiTietDto
                        {
                            Id = x.Id,

                            TaiSanDinhDanhId =
                                x.TaiSanDinhDanhId,

                            ViTriChuyenDiId =
                                x.ViTriChuyenDiId,

                            ViTriChuyenDenId =
                                x.ViTriChuyenDenId,

                            GhiChu = x.GhiChu
                        })
                    .ToList(),

                ChiTietIdsXoa =
                    _chiTietIdsXoa.ToList()
            };

            try
            {
                var id =
                    await _dieuChuyenService.LuuPhieuAsync(dto);

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
            CurrentChiTiet = new();

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
            var dialogVm =
                App.Services.GetRequiredService<DieuChuyenSearchViewModel>();

            var window =
                App.Services.GetRequiredService<DieuChuyenSearchWindow>();

            window.DataContext = dialogVm;
            window.Owner =
                Application.Current.MainWindow;

            dialogVm.RequestClose += (s, chosen) =>
            {
                window.DialogResult = chosen;
                window.Close();
            };

            if (window.ShowDialog() == true &&
                dialogVm.SelectedDieuChuyenId is long id)
            {
                await LoadPhieuAsync(id);
            }
        }

        protected override async void Delete()
        {
            // Chỉ được xóa phiếu của phòng ban mặc định.
            if (!CoTheSuaPhieu)
            {
                MessageBox.Show(
                    "Bạn không được xóa phiếu của phòng ban khác hoặc phiếu đã được duyệt.");
                return;
            }

            var result = MessageBox.Show(
                $"Xóa phiếu điều chuyển '{CurrentPhieu.SoPhieu}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _dieuChuyenService.DeleteAsync(
                    CurrentPhieu.Id);

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

        [RelayCommand]
        private void ThemTaiSan()
        {
            if (!IsEditing)
                return;

            if (PhongBanChuyenDenChonId <= 0)
            {
                MessageBox.Show(
                    "Chọn Phòng Ban Chuyển Đến trước khi thêm tài sản.");

                return;
            }

            CurrentChiTiet = new();

            TaiSanDangChon = null;
            _dangSuaChiTiet = null;

            IsAddingChiTiet = true;
        }

        [RelayCommand]
        private void SuaDong(
            DieuChuyenChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null)
                return;

            CurrentChiTiet = new()
            {
                Id = dong.Id,

                TaiSanDinhDanhId =
                    dong.TaiSanDinhDanhId,

                MaTaiSan =
                    dong.MaTaiSan,

                TenTaiSan =
                    dong.TenTaiSan,

                ViTriChuyenDiId =
                    dong.ViTriChuyenDiId,

                TenViTriChuyenDi =
                    dong.TenViTriChuyenDi,

                ViTriChuyenDenId =
                    dong.ViTriChuyenDenId,

                GhiChu =
                    dong.GhiChu
            };

            _dangSuaChiTiet = dong;
            IsAddingChiTiet = true;
        }

        [RelayCommand]
        private void XoaDong(
            DieuChuyenChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null)
                return;

            if (dong.Id is long id)
                _chiTietIdsXoa.Add(id);

            ChiTiets.Remove(dong);

            OnPropertyChanged(
                nameof(CoTheDoiPhongBanChuyenDen));

            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void LuuDong()
        {
            // Thêm dòng mới
            if (_dangSuaChiTiet == null)
            {
                if (TaiSanDangChon == null)
                {
                    MessageBox.Show(
                        "Chọn tài sản cần điều chuyển.");

                    return;
                }

                if (ChiTiets.Any(x =>
                    x.TaiSanDinhDanhId ==
                    TaiSanDangChon.Id))
                {
                    MessageBox.Show(
                        "Tài sản này đã có trong phiếu.");

                    return;
                }

                CurrentChiTiet.TaiSanDinhDanhId =
                    TaiSanDangChon.Id;

                CurrentChiTiet.MaTaiSan =
                    TaiSanDangChon.Code;

                CurrentChiTiet.TenTaiSan =
                    TaiSanDangChon.Name;

                CurrentChiTiet.ViTriChuyenDiId =
                    TaiSanDangChon.ViTriTaiSanId;

                CurrentChiTiet.TenViTriChuyenDi =
                    _tatCaViTriTaiSan
                        .FirstOrDefault(v =>
                            v.Id ==
                            TaiSanDangChon.ViTriTaiSanId)
                        ?.Name ?? string.Empty;
            }

            if (CurrentChiTiet.ViTriChuyenDenId == 0)
            {
                MessageBox.Show(
                    "Chọn Vị Trí Chuyển Đến.");

                return;
            }

            var viTriDen =
                ViTriTaiSansChuyenDen.FirstOrDefault(
                    v => v.Id ==
                         CurrentChiTiet.ViTriChuyenDenId);

            CurrentChiTiet.TenViTriChuyenDen =
                viTriDen?.Name ?? string.Empty;

            if (_dangSuaChiTiet != null)
            {
                var index =
                    ChiTiets.IndexOf(_dangSuaChiTiet);

                if (index >= 0)
                    ChiTiets[index] = CurrentChiTiet;
            }
            else
            {
                ChiTiets.Add(CurrentChiTiet);
            }

            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;

            CurrentChiTiet = new();
            TaiSanDangChon = null;

            OnPropertyChanged(
                nameof(CoTheDoiPhongBanChuyenDen));

            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void HuyDong()
        {
            _dangSuaChiTiet = null;
            IsAddingChiTiet = false;

            CurrentChiTiet = new();
            TaiSanDangChon = null;
        }

        [RelayCommand(CanExecute = nameof(CanDuyet))]
        private async Task Duyet()
        {
            try
            {
                await _dieuChuyenService.DuyetPhieuAsync(
                    CurrentPhieu.Id,
                    _currentUser.Instance!.Id);

                await LoadPhieuAsync(
                    CurrentPhieu.Id);
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

        private bool CanDuyet()
        {
            return IsPending &&
                   ChiTiets.Count > 0;
        }

        [RelayCommand(CanExecute = nameof(CanTuChoi))]
        private async Task TuChoi()
        {
            try
            {
                await _dieuChuyenService.TuChoiPhieuAsync(
                    CurrentPhieu.Id,
                    _currentUser.Instance!.Id);

                await LoadPhieuAsync(
                    CurrentPhieu.Id);
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

        private bool CanTuChoi()
        {
            return IsPending;
        }

        partial void OnCurrentChiTietChanged(
            DieuChuyenChiTietFormDto value)
        {
            OnPropertyChanged(
                nameof(DangThemMoiDong));

            OnPropertyChanged(
                nameof(DangSuaDong));
        }

        partial void OnCurrentPhieuChanged(
            DieuChuyenFormDto value)
        {
            NotifyPhieuStateChanged();
        }

        /// <summary>
        /// Thông báo lại toàn bộ trạng thái UI phụ thuộc vào phiếu hiện tại.
        /// </summary>
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