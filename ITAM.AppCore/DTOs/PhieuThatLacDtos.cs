using CommunityToolkit.Mvvm.ComponentModel;
using ITAM.Domain.Enums;

namespace ITAM.AppCore.DTOs
{
    public partial class PhieuThatLacFormDto : ObservableObject
    {
        public long Id { get; set; }
        public string SoPhieu { get; set; } = string.Empty;
        public DateTime? NgayTao { get; set; }
        public long NguoiTaoId { get; set; }
        public string TenNguoiTao { get; set; } = string.Empty;
        public long PhongBanId { get; set; }
        public string TenPhongBan { get; set; } = string.Empty;
        public string TenNguoiDuyet { get; set; } = string.Empty;
        public DateTime? NgayDuyet { get; set; }

        /// <summary>"PENDING" / "APPROVED" / "REJECTED" (cùng kiểu string với DieuChuyenFormDto để dùng chung converter).</summary>
        public string TrangThai { get; set; } = string.Empty;

        [ObservableProperty] private DateTime ngayPhatHien = DateTime.Today;
        [ObservableProperty] private string? nguoiPhatHien;
        [ObservableProperty] private string lyDo = string.Empty;
    }

    public partial class PhieuThatLacChiTietFormDto : ObservableObject
    {
        /// <summary>null = dòng mới chưa lưu.</summary>
        public long? Id { get; set; }
        public long TaiSanDinhDanhId { get; set; }
        public string MaTaiSan { get; set; } = string.Empty;
        public string TenTaiSan { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public long ViTriTruocId { get; set; }
        public string TenViTriTruoc { get; set; } = string.Empty;
        public TrangThaiTaiSan TrangThaiTruoc { get; set; }

        [ObservableProperty] private string? ghiChu;     // sửa trực tiếp trên grid
    }
    public class SavePhieuThatLacDto
    {
        public long? Id { get; set; }
        public long NguoiTaoId { get; set; }
        public long PhongBanId { get; set; }
        public DateTime NgayPhatHien { get; set; }
        public string? NguoiPhatHien { get; set; }
        public string LyDo { get; set; } = string.Empty;
        public List<SavePhieuThatLacChiTietDto> ChiTiets { get; set; } = new();
        public List<long> ChiTietIdsXoa { get; set; } = new();
    }

    public class SavePhieuThatLacChiTietDto
    {
        public long? Id { get; set; }
        public long TaiSanDinhDanhId { get; set; }
        public string? GhiChu { get; set; }
    }

    public class PhieuThatLacSearchDto
    {
        public string? SoPhieu { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public TrangThaiPhieu? TrangThai { get; set; }
    }
}