using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs; // TODO xác nhận: namespace thật của LichSuDieuChuyenTaiSanDto
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Interfaces;
using ITAM.WPF.Constants;
using ITAM.WPF.Helpers;
using ITAM.WPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ITAM.WPF.ViewModels
{
    /// <summary>
    /// Màn hình xem Lịch Sử Điều Chuyển Tài Sản — CHỈ XEM, không có thao tác thêm/sửa/xóa.
    /// Layout: bộ lọc bên trái, kết quả (DataGrid) bên phải.
    ///
    /// Phòng Ban Đi/Đến: dùng <see cref="SearchableCollectionView{T}"/> (ComboBox autocomplete
    /// 1 cột, chỉ hiển thị Tên) — đúng convention hiện có của dự án (giống PhongBanChuyenDen
    /// trong DieuChuyenViewModel).
    /// Vị Trí Đi/Đến: dùng <see cref="MultiColumnSearchComboBox"/> (hiển thị nhiều cột Mã/Tên).
    ///
    /// Lúc mở màn hình KHÔNG tự động tìm kiếm (dữ liệu lịch sử có thể rất nhiều) — chỉ nạp
    /// danh mục và điền sẵn Khoa/Phòng Ban mặc định của user vào 2 điều kiện Phòng Ban Đi/Đến.
    /// User phải bấm "Tìm Kiếm" thì mới thật sự gọi service.
    /// </summary>
    public partial class LichSuDieuChuyenViewModel : BaseViewModel
    {
        public override string Title => PageTitles.LichSuDieuChuyenTaiSan;
        private readonly ILichSuDieuChuyenService _lichSuDieuChuyenService;
        private readonly ICatalogService<PhongBan> _phongBanService; // TODO xác nhận: giống cách dùng cho NhaCungCap/LoaiTaiSan
        private readonly ICatalogService<ViTriTaiSan> _viTriTaiSanService; // mirror DieuChuyenViewModel

        public ObservableCollection<PhongBan> PhongBans { get; }
        public ObservableCollection<ViTriTaiSan> ViTriTaiSans { get; }
        public ObservableCollection<LichSuDieuChuyenTaiSanDto> KetQua { get; }

        public SearchableCollectionView<PhongBan> PhongBanChuyenDiView { get; }
        public SearchableCollectionView<PhongBan> PhongBanChuyenDenView { get; }

        [ObservableProperty]
        private string? tuKhoaTaiSan;

        [ObservableProperty]
        private PhongBan? phongBanChuyenDiChon;

        [ObservableProperty]
        private PhongBan? phongBanChuyenDenChon;

        public long? PhongBanChuyenDiChonId => PhongBanChuyenDiChon?.Id;
        public long? PhongBanChuyenDenChonId => PhongBanChuyenDenChon?.Id;

        [ObservableProperty]
        private long? viTriChuyenDiChonId;

        [ObservableProperty]
        private long? viTriChuyenDenChonId;

        [ObservableProperty]
        private DateTime? tuNgay;

        [ObservableProperty]
        private DateTime? denNgay;

        [ObservableProperty]
        private bool isLoading;

        public LichSuDieuChuyenViewModel(
            ILichSuDieuChuyenService lichSuDieuChuyenService,
            ICatalogService<PhongBan> phongBanService,
            ICatalogService<ViTriTaiSan> viTriTaiSanService,INavigationService navigationService,ICurrentUserContext currentUserContext):base(navigationService,currentUserContext)
        {
            _lichSuDieuChuyenService = lichSuDieuChuyenService;
            _phongBanService = phongBanService;
            _viTriTaiSanService = viTriTaiSanService;

            PhongBans = new ObservableCollection<PhongBan>();
            ViTriTaiSans = new ObservableCollection<ViTriTaiSan>();
            KetQua = new ObservableCollection<LichSuDieuChuyenTaiSanDto>();

            // Canonical version: mỗi SearchableCollectionView<T> tự tạo CollectionViewSource
            // riêng nên 2 instance có thể dùng chung nguồn PhongBans mà không đụng nhau.
            PhongBanChuyenDiView = new SearchableCollectionView<PhongBan>(PhongBans, x => x.Code, x => x.Name);
            PhongBanChuyenDenView = new SearchableCollectionView<PhongBan>(PhongBans, x => x.Code, x => x.Name);

            _ = InitializeAsync();
        }



        partial void OnPhongBanChuyenDiChonChanged(PhongBan? value)
        {
            PhongBanChuyenDiView.SetDisplayTextSilently(value?.Name);
            OnPropertyChanged(nameof(PhongBanChuyenDiChonId));
        }

        partial void OnPhongBanChuyenDenChonChanged(PhongBan? value)
        {
            PhongBanChuyenDenView.SetDisplayTextSilently(value?.Name);
            OnPropertyChanged(nameof(PhongBanChuyenDenChonId));
        }

        /// <summary>
        /// Chỉ nạp danh mục + điền sẵn Phòng Ban mặc định — KHÔNG gọi TimKiemAsync ở đây.
        /// </summary>
        public  async Task InitializeAsync()
        {
            // Nạp danh mục TRƯỚC khi gán PhongBanChuyenDiChon/PhongBanChuyenDenChon (SelectedItem),
            // đúng thứ tự ItemsSource -> SelectedItem để tránh bug ComboBox reset về null.
            await LoadDanhMucAsync();

            var phongBanMacDinh = _currentUserContext.Instance?.PhongBan; // confirmed: navigation property (không phải PhongBanMacDinhId)
            if (phongBanMacDinh != null)
            {
                var macDinh = PhongBans.FirstOrDefault(x => x.Id == phongBanMacDinh.Id);
                PhongBanChuyenDiChon = macDinh;
                PhongBanChuyenDenChon = macDinh;
            }

            // Cố ý KHÔNG gọi TimKiemAsync() ở đây — dữ liệu lịch sử có thể rất nhiều,
            // chỉ tìm khi user bấm nút "Tìm Kiếm".
        }

        private async Task LoadDanhMucAsync()
        {
            var dsPhongBan = await _phongBanService.GetAllAsync(); // TODO xác nhận tên method trên ICatalogService<T>
            PhongBans.Clear();
            foreach (var pb in dsPhongBan)
                PhongBans.Add(pb);

            var dsViTri = await _viTriTaiSanService.GetAllAsync();
            ViTriTaiSans.Clear();
            foreach (var vt in dsViTri)
                ViTriTaiSans.Add(vt);
        }

        [RelayCommand]
        private async Task TimKiemAsync()
        {
            IsLoading = true;
            try
            {
                var filter = new LichSuDieuChuyenSearchDto
                {
                    TuKhoaTaiSan = string.IsNullOrWhiteSpace(TuKhoaTaiSan) ? null : TuKhoaTaiSan.Trim(),
                    PhongBanChuyenDiId = PhongBanChuyenDiChonId,
                    PhongBanChuyenDenId = PhongBanChuyenDenChonId,
                    ViTriChuyenDiId = ViTriChuyenDiChonId,
                    ViTriChuyenDenId = ViTriChuyenDenChonId,
                    TuNgay = TuNgay,
                    DenNgay = DenNgay
                };

                var ketQua = await _lichSuDieuChuyenService.TimKiemAsync(filter);

                KetQua.Clear();
                foreach (var item in ketQua)
                    KetQua.Add(item);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void XoaLoc()
        {
            TuKhoaTaiSan = null;
            PhongBanChuyenDiChon = null;
            PhongBanChuyenDenChon = null;
            ViTriChuyenDiChonId = null;
            ViTriChuyenDenChonId = null;
            TuNgay = null;
            DenNgay = null;

            // Xóa lọc chỉ reset điều kiện, không tự tìm lại — user tự bấm Tìm Kiếm lần nữa.
        }
    }
}