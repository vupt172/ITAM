using ITAM.AppCore.DTOs;
using ITAM.Domain.Enums;
namespace ITAM.AppCore.DTOs;
//===== BAO CAO 1. NHAP TAI SAN
public class NhapTaiSanLookupDto
{
    public List<LookupItemDto> NhaCungCaps { get; set; } = new();
    public List<LookupItemDto> NguoiLaps { get; set; } = new();
    public List<LookupItemDto> DanhMucs { get; set; } = new();
    public List<LookupItemDto> HangHoas { get; set; } = new();
}
public class BaoCaoNhapTaiSanFilterDto
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public long? NhaCungCapId { get; set; }
    public string? SoPhieu { get; set; }
    public string? MaHoaDon { get; set; }
    public long? NguoiLapPhieuId { get; set; }
    public TrangThaiPhieu? TrangThai { get; set; }   // TODO xác nhận tên enum
    public long? DMTaiSanId { get; set; }
    public long? HangHoaId { get; set; }
}

public class BaoCaoNhapTaiSanRowDto
{
    public string SoPhieu { get; set; } = "";
    public DateTime NgayNhap { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public string NhaCungCap { get; set; } = "";
    public string? MaHoaDon { get; set; }
    public string MaHangHoa { get; set; } = "";
    public string TenHangHoa { get; set; } = "";
    public string DanhMuc { get; set; } = "";
    public string LoaiTaiSan { get; set; } = "";
    public int SoLuongNhap { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien => SoLuongNhap * DonGia;
    public string NguoiLapPhieu { get; set; } = "";
    public string TrangThai { get; set; } = "";
}
//==========BAOCAO 2.PHIEU NHAP
public class PhieuNhapLookupDto
{
    public List<LookupItemDto> NhaCungCaps { get; set; } = new();
    public List<LookupItemDto> NguoiLaps { get; set; } = new();
}
public class DanhSachPhieuNhapFilterDto
{
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public long? NhaCungCapId { get; set; }
    public string? SoPhieu { get; set; }
    public string? MaHoaDon { get; set; }
    public long? NguoiLapPhieuId { get; set; }
    public TrangThaiPhieu? TrangThai { get; set; }
}

public class DanhSachPhieuNhapRowDto
{
    public string SoPhieu { get; set; } = "";
    public DateTime NgayNhap { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public string NhaCungCap { get; set; } = "";
    public string? MaHoaDon { get; set; }
    public string NguoiLapPhieu { get; set; } = "";
    public string TrangThai { get; set; } = "";
    public int SoDong { get; set; }
    public int TongSoLuong { get; set; }
    public decimal TongTien { get; set; }
}