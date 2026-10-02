using ITAM.Domain.Enums; // TODO xác nhận namespace của enum TrangThaiTaiSan

namespace ITAM.AppCore.DTOs;

public enum NhomTheoTongHop
{
    PhongBan,
    ViTri,
    LoaiTaiSan,
    DanhMuc,
    HangSanXuat,
    TrangThai
}

/// <summary>Điều kiện lọc: null = "Tất cả".</summary>
public class TongHopTaiSanFilterDto
{
    public long? PhongBanId { get; set; }
    public long? ViTriTaiSanId { get; set; }
    public long? LoaiTaiSanId { get; set; }
    public long? DMTaiSanId { get; set; }          // Danh mục = DMTaiSan (qua HangHoa)
    public string? HangSanXuat { get; set; }
    public TrangThaiTaiSan? TrangThai { get; set; }
    public NhomTheoTongHop NhomTheo { get; set; } = NhomTheoTongHop.PhongBan;
}

public class TongHopTaiSanRowDto
{
    public string TenNhom { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal TongNguyenGia { get; set; }
}

public class LookupItemDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? ParentId { get; set; }            // ViTri -> PhongBanId
}

public class TongHopTaiSanLookupDto
{
    public List<LookupItemDto> PhongBans { get; set; } = new();
    public List<LookupItemDto> ViTris { get; set; } = new();
    public List<LookupItemDto> LoaiTaiSans { get; set; } = new();
    public List<LookupItemDto> DanhMucs { get; set; } = new();
    public List<string> HangSanXuats { get; set; } = new();
}