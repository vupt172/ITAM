// ===== ITAM.AppCore/DTOs/DashboardDto.cs =====
namespace ITAM.AppCore.DTOs;

public record ChartPointDto(string Label, double Value);

public static class HoatDongLoai
{
    public const string Nhap = "NHAP";
    public const string DieuChuyen = "DIEUCHUYEN";
    public const string ThatLac = "THATLAC";
    public const string ThanhLy = "THANHLY";
}

public class HoatDongDto
{
    public string Loai { get; set; } = "";
    public string TieuDe { get; set; } = "";
    public string ChiTiet { get; set; } = "";
    public DateTime ThoiGian { get; set; }
}

public class TaiSanGiaTriDto
{
    public string Ma { get; set; } = "";
    public string Ten { get; set; } = "";
    public string DanhMuc { get; set; } = "";
    public string PhongBan { get; set; } = "";
    public string ViTri { get; set; } = "";
    public decimal GiaTri { get; set; }
    public string TrangThai { get; set; } = "";   // tên enum TrangThaiTaiSan (IN_STOCK, ASSIGNED, ...)
}

/// <summary>% tăng trưởng trong kỳ [TuNgay, DenNgay] so với tổng trước TuNgay. null = không tính được (tổng trước kỳ = 0).</summary>
public class XuHuongKyDto
{
    public double? TongTaiSanPct { get; set; }
    public double? TongGiaTriPct { get; set; }
}

/// <summary>Mỗi mảng 12 phần tử (T1..T12).</summary>
public class ThongKeThangDto
{
    public int[] Nhap { get; set; } = new int[12];
    public double[] GiaTriNhapTrieu { get; set; } = new double[12];   // tổng GiaNhap, đơn vị triệu đồng
    public int[] DieuChuyen { get; set; } = new int[12];
    public int[] ThanhLy { get; set; } = new int[12];
}

public class DashboardDto
{
    // KPI — chỉ tính TaiSanDinhDanh
    public int TongTaiSan { get; set; }
    public int DangSuDung { get; set; }    // ASSIGNED
    public int TrongKho { get; set; }      // IN_STOCK
    public int ThatLac { get; set; }       // LOST
    public int ChoThanhLy { get; set; }    // DISPOSAL_PENDING
    public decimal TongGiaTri { get; set; }

    public List<ChartPointDto> TheoPhongBan { get; set; } = new();        // số lượng, Top 7 + Khác
    public List<ChartPointDto> TheoDanhMuc { get; set; } = new();         // số lượng, Top 5 + Khác
    public List<ChartPointDto> TheoLoai { get; set; } = new();            // doughnut
    public List<ChartPointDto> GiaTriTheoPhongBan { get; set; } = new();  // triệu đồng, Top 6 + Khác
    public List<TaiSanGiaTriDto> TopGiaTri { get; set; } = new();         // Top 5
    public List<HoatDongDto> HoatDongGanDay { get; set; } = new();        // 8 mục mới nhất
}

