using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.Domain.Enums;

namespace ITAM.AppCore.DTOs
{
    public partial class PhieuThanhLyFormDto : ObservableObject
    {
        public long Id { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime? NgayTao { get; set; }
        public long NguoiTaoId { get; set; }
        public string TenNguoiTao { get; set; } = string.Empty;
        public long PhongBanId { get; set; }
        public string TenPhongBan { get; set; } = string.Empty;
        public string TenNguoiDuyet { get; set; } = "Chưa duyệt";
        public DateTime? NgayDuyet { get; set; }
        public string TrangThai { get; set; } = "PENDING";

        [ObservableProperty] private string lyDo = string.Empty;
    }

    public partial class PhieuThanhLyChiTietFormDto : ObservableObject
    {
        public long? Id { get; set; }
        public long TaiSanDinhDanhId { get; set; }
        public string MaTaiSan { get; set; } = string.Empty;
        public string TenTaiSan { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public long ViTriTruocId { get; set; }
        public string TenViTriTruoc { get; set; } = string.Empty;

        [ObservableProperty] private decimal giaThanhLy;
        [ObservableProperty] private string? ghiChu;
    }
    public class SavePhieuThanhLyDto
    {
        public long? Id { get; set; }                 // null/0 = tạo mới
        public long PhongBanId { get; set; }          // ViewModel gán = Phòng Ban mặc định của user
        public long NguoiTaoId { get; set; }
        public string LyDo { get; set; } = string.Empty;

        public List<SavePhieuThanhLyChiTietDto> ChiTiets { get; set; } = new();
        public List<long> ChiTietIdsXoa { get; set; } = new();
    }

    public class SavePhieuThanhLyChiTietDto
    {
        public long? Id { get; set; }                 // null/0 = dòng mới
        public long TaiSanDinhDanhId { get; set; }
        public decimal GiaThanhLy { get; set; }
        public string? GhiChu { get; set; }
    }

    public class PhieuThanhLySearchDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiPhieu? TrangThai { get; set; }
    }
}