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
    public partial class PhieuThanhLyViewModel : BaseViewModel
    {
        private const string FeatureThanhLy = "THANHLY";

        private readonly IPhieuThanhLyService _phieuThanhLyService;
        private readonly ICatalogService<ViTriTaiSan> _viTriTaiSanService;
        private readonly ITaiSanDinhDanhService _taiSanDinhDanhService;
        private readonly ICurrentUserContext _currentUser;

        public override string Title => "Thanh Lý Tài Sản";

        public ObservableCollection<PhieuThanhLyChiTietFormDto> ChiTiets { get; } = [];

        /// <summary>Tài sản đang DISPOSAL_PENDING trong Kho Chờ Thanh Lý. Chỉ mutate tại chỗ (Clear/Add).</summary>
        public ObservableCollection<TaiSanDinhDanh> TaiSansKhaDung { get; } = [];

        private List<ViTriTaiSan> _tatCaViTriTaiSan = new();
        private readonly List<long> _chiTietIdsXoa = new();

        /// <summary>Phòng ban mà TaiSansKhaDung đang chứa dữ liệu; 0 = chưa nạp hoặc cần nạp lại.</summary>
        private long _phongBanDangNapId;

        [ObservableProperty]
        private PhieuThanhLyFormDto currentPhieu = new();

        // ----- hàng "Thêm tài sản" -----
        [ObservableProperty]
        private TaiSanDinhDanh? taiSanDangChon;

        /// <summary>Giá thanh lý nhập cùng lúc với việc thêm tài sản. Để trống = 0 (chưa có giá).</summary>
        [ObservableProperty]
        private decimal? giaThanhLyDangNhap;

        [ObservableProperty]
        private string? ghiChuDangNhap;

        public bool IsPhieuMoi => CurrentPhieu.Id == 0;

        public bool IsPending =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            !IsEditing;

        private long? PhongBanMacDinhId => _currentUser.Instance?.PhongBan?.Id;

        private bool LaPhongBanKhoChoThanhLy =>
            _currentUser.Instance?.PhongBan?.Code == PhongBanCodes.KHO_CHO_THANH_LY;

        /// <summary>Chỉ sửa/xóa được phiếu PENDING của đúng phòng ban mặc định của mình.</summary>
        public bool CoTheSuaPhieu =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING" &&
            CurrentPhieu.PhongBanId == PhongBanMacDinhId;

        protected override bool HasSelection =>
            CurrentPhieu.Id > 0 &&
            CurrentPhieu.TrangThai == "PENDING";

        public PhieuThanhLyViewModel(
            IPhieuThanhLyService phieuThanhLyService,
            ICatalogService<ViTriTaiSan> viTriTaiSanService,
            ITaiSanDinhDanhService taiSanDinhDanhService,
            ICurrentUserContext currentUser,
            IErrorDialogService errorDialogService,
            INavigationService navigationService)
            : base(navigationService, errorDialogService)
        {
            _phieuThanhLyService = phieuThanhLyService;
            _viTriTaiSanService = viTriTaiSanService;
            _taiSanDinhDanhService = taiSanDinhDanhService;
            _currentUser = currentUser;
        }

        protected override void InitToolbarState()
        {
            CanClose = true;
            CanSearch = true;
        }

        // ---------- nạp dữ liệu ----------
        private async Task NapTaiSanKhaDungAsync(long phongBanId)
        {
            if (_tatCaViTriTaiSan.Count == 0)
                _tatCaViTriTaiSan = (await _viTriTaiSanService.GetAllAsync()).ToList();

            if (_phongBanDangNapId == phongBanId && TaiSansKhaDung.Count > 0)
                return;

            TaiSansKhaDung.Clear();

            foreach (var viTri in _tatCaViTriTaiSan.Where(v => v.PhongBanId == phongBanId))
            {
                var list = await _taiSanDinhDanhService.GetByViTriAsync(viTri.Id);

                foreach (var ts in list.Where(t => t.TrangThaiTaiSan == TrangThaiTaiSan.DISPOSAL_PENDING))
                    TaiSansKhaDung.Add(ts);
            }

            _phongBanDangNapId = phongBanId;
        }

        private string TenViTri(long viTriId) =>
            _tatCaViTriTaiSan.FirstOrDefault(v => v.Id == viTriId)?.Name ?? string.Empty;

        private async Task LoadPhieuAsync(long phieuId)
        {
            var p = await _phieuThanhLyService.GetByIdAsync(phieuId);
            if (p == null)
                return;

            CurrentPhieu = new PhieuThanhLyFormDto
            {
                Id = p.Id,
                SoPhieu = p.SoPhieu,
                NgayTao = p.NgayTao,
                NguoiTaoId = p.NguoiTaoId,
                TenNguoiTao = p.NguoiTao?.FullName ?? string.Empty,
                PhongBanId = p.PhongBanId,
                TenPhongBan = p.PhongBan?.Name ?? string.Empty,
                LyDo = p.LyDo,
                TenNguoiDuyet = p.NguoiDuyet?.FullName ?? "Chưa duyệt",
                NgayDuyet = p.NgayDuyet,
                TrangThai = p.TrangThai.ToString()
            };

            ChiTiets.Clear();
            foreach (var x in p.ChiTiets)
            {
                ChiTiets.Add(new PhieuThanhLyChiTietFormDto
                {
                    Id = x.Id,
                    TaiSanDinhDanhId = x.TaiSanDinhDanhId,
                    MaTaiSan = x.TaiSanDinhDanh.Code,
                    TenTaiSan = x.TaiSanDinhDanh.Name,
                    Serial = x.TaiSanDinhDanh.Serial,
                    ViTriTruocId = x.ViTriTruocId,
                    TenViTriTruoc = x.ViTriTruoc.Name,
                    GiaThanhLy = x.GiaThanhLy,
                    GhiChu = x.GhiChu
                });
            }

            _chiTietIdsXoa.Clear();
            ResetHangThemTaiSan();
            IsEditing = false;

            NotifyPhieuStateChanged();
        }

        private void ResetHangThemTaiSan()
        {
            TaiSanDangChon = null;
            GiaThanhLyDangNhap = null;
            GhiChuDangNhap = null;
        }

        // ---------- toolbar ----------
        protected override async void Add()
        {
            try
            {
                var phongBan = _currentUser.Instance?.PhongBan;
                if (phongBan == null || phongBan.Code != PhongBanCodes.KHO_CHO_THANH_LY)
                {
                    MessageBox.Show("Chỉ lập được phiếu thanh lý khi Phòng Ban mặc định của bạn là Kho Chờ Thanh Lý.");
                    return;
                }

                await NapTaiSanKhaDungAsync(phongBan.Id);

                CurrentPhieu = new PhieuThanhLyFormDto
                {
                    PhongBanId = phongBan.Id,
                    TenPhongBan = phongBan.Name,
                    TenNguoiTao = _currentUser.Instance?.FullName ?? string.Empty
                };

                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();
                ResetHangThemTaiSan();

                IsEditing = true;
                NotifyPhieuStateChanged();
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override async void Edit()
        {
            if (!CoTheSuaPhieu)
            {
                MessageBox.Show("Bạn không được sửa phiếu của phòng ban khác hoặc phiếu đã được duyệt.");
                return;
            }

            try
            {
                await NapTaiSanKhaDungAsync(CurrentPhieu.PhongBanId);
                _chiTietIdsXoa.Clear();
                ResetHangThemTaiSan();
                IsEditing = true;
                NotifyPhieuStateChanged();
            }
            catch (Exception ex)
            {
                _errorDialogService.Show(ex);
            }
        }

        protected override async void Save()
        {
            if (_currentUser.Instance == null)
                return;

            if (CurrentPhieu.PhongBanId <= 0)
            {
                MessageBox.Show("Không xác định được Phòng Ban của bạn.");
                return;
            }

            // User có thể đã đổi Phòng Ban mặc định khi màn hình đang mở.
            if (IsPhieuMoi && !LaPhongBanKhoChoThanhLy)
            {
                MessageBox.Show("Chỉ lập được phiếu thanh lý khi Phòng Ban mặc định của bạn là Kho Chờ Thanh Lý.");
                return;
            }

            if (!IsPhieuMoi && CurrentPhieu.PhongBanId != PhongBanMacDinhId)
            {
                MessageBox.Show("Bạn không được sửa phiếu của phòng ban khác.");
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentPhieu.LyDo))
            {
                MessageBox.Show("Chưa nhập lý do thanh lý.");
                return;
            }

            if (ChiTiets.Count == 0)
            {
                MessageBox.Show("Phiếu thanh lý phải có ít nhất 1 tài sản.");
                return;
            }

            var dto = new SavePhieuThanhLyDto
            {
                Id = CurrentPhieu.Id == 0 ? null : CurrentPhieu.Id,
                NguoiTaoId = _currentUser.Instance.Id,
                PhongBanId = CurrentPhieu.PhongBanId,
                LyDo = CurrentPhieu.LyDo,
                ChiTiets = ChiTiets.Select(x => new SavePhieuThanhLyChiTietDto
                {
                    Id = x.Id,
                    TaiSanDinhDanhId = x.TaiSanDinhDanhId,
                    GiaThanhLy = x.GiaThanhLy,
                    GhiChu = x.GhiChu
                }).ToList(),
                ChiTietIdsXoa = _chiTietIdsXoa.ToList()
            };

            try
            {
                var id = await _phieuThanhLyService.LuuPhieuAsync(dto);
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
            ResetHangThemTaiSan();

            if (CurrentPhieu.Id == 0)
            {
                CurrentPhieu = new();
                ChiTiets.Clear();
                _chiTietIdsXoa.Clear();
            }
            else
            {
                _ = LoadPhieuAsync(CurrentPhieu.Id);
            }

            NotifyPhieuStateChanged();
        }

        protected override async void Search()
        {
            var dialogVm = App.Services.GetRequiredService<PhieuThanhLySearchViewModel>();
            var window = App.Services.GetRequiredService<PhieuThanhLySearchWindow>();

            window.DataContext = dialogVm;
            window.Owner = Application.Current.MainWindow;

            dialogVm.RequestClose += (s, chosen) =>
            {
                window.DialogResult = chosen;
                window.Close();
            };

            if (window.ShowDialog() == true && dialogVm.SelectedPhieuThanhLyId is long id)
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
                $"Xóa phiếu thanh lý '{CurrentPhieu.SoPhieu}'?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _phieuThanhLyService.DeleteAsync(CurrentPhieu.Id);

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

        // ---------- dòng chi tiết ----------
        [RelayCommand]
        private void ThemTaiSan()
        {
            if (!IsEditing)
                return;

            var ts = TaiSanDangChon;
            if (ts == null)
            {
                MessageBox.Show("Chọn tài sản cần thanh lý.");
                return;
            }

            if (GiaThanhLyDangNhap < 0)
            {
                MessageBox.Show("Giá thanh lý không được âm.");
                return;
            }

            if (ChiTiets.Any(x => x.TaiSanDinhDanhId == ts.Id))
            {
                MessageBox.Show("Tài sản này đã có trong phiếu.");
                return;
            }

            ChiTiets.Add(new PhieuThanhLyChiTietFormDto
            {
                Id = null,
                TaiSanDinhDanhId = ts.Id,
                MaTaiSan = ts.Code,
                TenTaiSan = ts.Name,
                Serial = ts.Serial,
                ViTriTruocId = ts.ViTriTaiSanId,       // chỉ để hiển thị, service snapshot lại từ DB khi lưu
                TenViTriTruoc = TenViTri(ts.ViTriTaiSanId),
                GiaThanhLy = GiaThanhLyDangNhap ?? 0,
                GhiChu = string.IsNullOrWhiteSpace(GhiChuDangNhap) ? null : GhiChuDangNhap.Trim()
            });

            ResetHangThemTaiSan();
            DuyetCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void XoaDong(PhieuThanhLyChiTietFormDto? dong)
        {
            if (!IsEditing || dong == null)
                return;

            if (dong.Id is long id)
                _chiTietIdsXoa.Add(id);

            ChiTiets.Remove(dong);
            DuyetCommand.NotifyCanExecuteChanged();
        }

        // ---------- duyệt / từ chối: chỉ cần feature THANHLY ----------
        [RelayCommand(CanExecute = nameof(CanDuyet))]
        private async Task Duyet()
        {
            var ok = MessageBox.Show(
                "Sau khi duyệt, các tài sản trong phiếu sẽ chuyển sang trạng thái DISPOSED và vào Kho Thanh Lý. " +
                "Thao tác này KHÔNG thể hoàn tác.\n\nTiếp tục duyệt?",
                "Xác nhận duyệt", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (ok != MessageBoxResult.Yes)
                return;

            try
            {
                await _phieuThanhLyService.DuyetPhieuAsync(CurrentPhieu.Id, _currentUser.Instance!.Id);

                // Tài sản vừa chuyển DISPOSED, danh sách khả dụng phải nạp lại lần sau.
                _phongBanDangNapId = 0;
                TaiSansKhaDung.Clear();

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
                await _phieuThanhLyService.TuChoiPhieuAsync(CurrentPhieu.Id, _currentUser.Instance!.Id);
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

        // ---------- notify ----------
        partial void OnCurrentPhieuChanged(PhieuThanhLyFormDto value)
        {
            NotifyPhieuStateChanged();
        }

        /// <summary>Thông báo lại toàn bộ trạng thái UI phụ thuộc vào phiếu hiện tại.</summary>
        private void NotifyPhieuStateChanged()
        {
            OnPropertyChanged(nameof(IsPending));
            OnPropertyChanged(nameof(IsPhieuMoi));
            OnPropertyChanged(nameof(CoTheSuaPhieu));

            DuyetCommand.NotifyCanExecuteChanged();
            TuChoiCommand.NotifyCanExecuteChanged();

            NotifySelectionChanged();
        }
    }
}