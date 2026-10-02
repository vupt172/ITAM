// ITAM.Infrastructure/Services/DashboardService.cs
//
// Giả định chưa xác nhận (// TODO xác nhận):
//  1. DbSet: LichSuDieuChuyenTaiSan; navigation LichSuDieuChuyenTaiSan.TaiSanDinhDanh
//  2. TrangThaiLoNhap.APPROVED là giá trị "đã duyệt" của LoNhap
//  3. Code vị trí kho: "VT_KHO_THAT_LAC", "VT_KHO_THANH_LY" (đã xác nhận trong ViTriTaiSanCodes)
//  4. LichSuDieuChuyenTaiSan.NgayDuyet (DateTime hoặc DateTime? đều được)
//  5. Chưa có nguồn dữ liệu cho hoạt động "Cập nhật thông tin" (không có audit trail) nên feed bỏ loại này.
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private const string CodeKhoThatLac = "VT_KHO_THAT_LAC";
    private const string CodeKhoThanhLy = "VT_KHO_THANH_LY";

    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context) => _context = context;

    // Toàn bộ là truy vấn đọc (AsNoTracking, không SaveChanges) nên không cần ChangeTracker.Clear().
    // Các query chạy tuần tự vì cùng một DbContext không cho chạy song song.
    public async Task<DashboardDto> GetTongQuanAsync()
    {
        var dto = new DashboardDto();
        var ts = _context.TaiSanDinhDanh.AsNoTracking();

        // ---- KPI ----
        var theoTrangThai = await ts
            .GroupBy(x => x.TrangThaiTaiSan)
            .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
            .ToListAsync();

        int Dem(TrangThaiTaiSan t) => theoTrangThai.Where(x => x.TrangThai == t).Sum(x => x.SoLuong);

        dto.TongTaiSan = theoTrangThai.Sum(x => x.SoLuong);
        dto.DangSuDung = Dem(TrangThaiTaiSan.ASSIGNED);
        dto.TrongKho = Dem(TrangThaiTaiSan.IN_STOCK);
        dto.ThatLac = Dem(TrangThaiTaiSan.LOST);
        dto.ChoThanhLy = Dem(TrangThaiTaiSan.DISPOSAL_PENDING);
        dto.TongGiaTri = await ts.SumAsync(x => (decimal?)x.GiaNhap) ?? 0m;

        // ---- Theo phòng ban: số lượng + giá trị (join qua ViTriTaiSan.PhongBanId) ----
        var theoPb = await (
            from x in ts
            join p in _context.PhongBan.AsNoTracking() on x.ViTriTaiSan.PhongBanId equals p.Id
            group x by p.Name into g
            select new { Ten = g.Key, SoLuong = g.Count(), GiaTri = g.Sum(t => t.GiaNhap) }
        ).ToListAsync();

        dto.TheoPhongBan = TopVaGop(theoPb.Select(x => new ChartPointDto(x.Ten, x.SoLuong)), 7);
        dto.GiaTriTheoPhongBan = TopVaGop(
            theoPb.Select(x => new ChartPointDto(x.Ten, (double)(x.GiaTri / 1_000_000m))), 6);

        // ---- Theo danh mục (DMTaiSan) — chỉ TaiSanDinhDanh ----
        var theoDanhMuc = await ts
            .GroupBy(x => x.HangHoa.DMTaiSan.Name)
            .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
            .ToListAsync();
        dto.TheoDanhMuc = TopVaGop(theoDanhMuc.Select(x => new ChartPointDto(x.Ten, x.SoLuong)), 5);

        // ---- Cơ cấu theo loại: TSCĐ/CCDC = đếm TaiSanDinhDanh, Vật Tư = tổng SoLuongTon ----
        var loais = await _context.LoaiTaiSan.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new { x.Code, x.Name })
            .ToListAsync();

        var tsTheoLoai = (await ts
                .GroupBy(x => x.LoaiTaiSan.Name)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync())
            .ToDictionary(x => x.Ten, x => (double)x.SoLuong);

        var slVatTu = await _context.VatTu.AsNoTracking()
            .SumAsync(x => (double?)x.SoLuongTon) ?? 0;   // TODO: đổi sang CountAsync() nếu muốn đếm số dòng vật tư

        dto.TheoLoai = loais
            .Select(l => new ChartPointDto(
                l.Name,
                l.Code == "VATTU" ? slVatTu : tsTheoLoai.GetValueOrDefault(l.Name)))
            .ToList();

        // ---- Top 5 tài sản giá trị lớn nhất ----
        var top = await (
            from x in ts
            join p in _context.PhongBan.AsNoTracking() on x.ViTriTaiSan.PhongBanId equals p.Id
            orderby x.GiaNhap descending
            select new
            {
                x.Code,
                x.Name,
                DanhMuc = x.HangHoa.DMTaiSan.Name,
                PhongBan = p.Name,
                ViTri = x.ViTriTaiSan.Name,
                x.GiaNhap,
                x.TrangThaiTaiSan
            }
        ).Take(5).ToListAsync();

        dto.TopGiaTri = top.Select(t => new TaiSanGiaTriDto
        {
            Ma = t.Code,
            Ten = t.Name,
            DanhMuc = t.DanhMuc,
            PhongBan = t.PhongBan,
            ViTri = t.ViTri,
            GiaTri = t.GiaNhap,
            TrangThai = t.TrangThaiTaiSan.ToString()
        }).ToList();

        dto.HoatDongGanDay = await GetHoatDongGanDayAsync(8);
        return dto;
    }

    public async Task<XuHuongKyDto> GetXuHuongKyAsync(DateTime tuNgay, DateTime denNgay)
    {
        var tu = tuNgay.Date;
        var denExclusive = denNgay.Date.AddDays(1);
        var ts = _context.TaiSanDinhDanh.AsNoTracking();

        // "Tổng tài sản" không loại trạng thái nào nên tài sản chỉ tăng khi Lô Nhập được duyệt
        // → tăng trưởng trong kỳ tính chính xác được từ ngày duyệt Lô Nhập.
        var truocKy = ts.Where(x => x.LoNhapChiTiet.LoNhap.NgayDuyet < tu);
        var trongKy = ts.Where(x => x.LoNhapChiTiet.LoNhap.NgayDuyet >= tu
                                    && x.LoNhapChiTiet.LoNhap.NgayDuyet < denExclusive);

        var slTruoc = await truocKy.CountAsync();
        var slKy = await trongKy.CountAsync();
        var gtTruoc = await truocKy.SumAsync(x => (decimal?)x.GiaNhap) ?? 0m;
        var gtKy = await trongKy.SumAsync(x => (decimal?)x.GiaNhap) ?? 0m;

        return new XuHuongKyDto
        {
            TongTaiSanPct = Pct(slKy, slTruoc),
            TongGiaTriPct = Pct((double)gtKy, (double)gtTruoc)
        };
    }

    public async Task<ThongKeThangDto> GetThongKeThangAsync(int nam)
    {
        var tu = new DateTime(nam, 1, 1);
        var den = tu.AddYears(1);
        var result = new ThongKeThangDto();

        // Nhập: theo Ngày duyệt Lô Nhập (khớp báo cáo Danh Sách Tài Sản)
        var nhap = await _context.TaiSanDinhDanh.AsNoTracking()
            .Where(x => x.LoNhapChiTiet.LoNhap.NgayDuyet >= tu && x.LoNhapChiTiet.LoNhap.NgayDuyet < den)
            .GroupBy(x => x.LoNhapChiTiet.LoNhap.NgayDuyet!.Value.Month)
            .Select(g => new { Thang = g.Key, SoLuong = g.Count(), GiaTri = g.Sum(t => t.GiaNhap) })
            .ToListAsync();
        foreach (var r in nhap)
        {
            result.Nhap[r.Thang - 1] = r.SoLuong;
            result.GiaTriNhapTrieu[r.Thang - 1] = (double)(r.GiaTri / 1_000_000m);
        }

        // Điều chuyển: dòng lịch sử sinh từ DieuChuyenChiTiet (DieuChuyenChiTietId != null)
        var dc = await _context.LichSuDieuChuyenTaiSan.AsNoTracking()
            .Where(x => x.DieuChuyenChiTietId != null && x.NgayDuyet >= tu && x.NgayDuyet < den)
            .GroupBy(x => ((DateTime)x.NgayDuyet).Month)
            .Select(g => new { Thang = g.Key, SoLuong = g.Count() })
            .ToListAsync();
        foreach (var r in dc) result.DieuChuyen[r.Thang - 1] = r.SoLuong;

        // Thanh lý: dòng lịch sử có đích là Kho Đã Thanh Lý (chức năng Thanh Lý chưa xây nên hiện có thể toàn 0)
        var tl = await _context.LichSuDieuChuyenTaiSan.AsNoTracking()
            .Where(x => x.ViTriChuyenDen.Code == CodeKhoThanhLy && x.NgayDuyet >= tu && x.NgayDuyet < den)
            .GroupBy(x => ((DateTime)x.NgayDuyet).Month)
            .Select(g => new { Thang = g.Key, SoLuong = g.Count() })
            .ToListAsync();
        foreach (var r in tl) result.ThanhLy[r.Thang - 1] = r.SoLuong;

        return result;
    }

    private async Task<List<HoatDongDto>> GetHoatDongGanDayAsync(int soLuong)
    {
        var list = new List<HoatDongDto>();

        // Nhập kho — TODO xác nhận TrangThaiLoNhap.APPROVED
        var loNhaps = await _context.LoNhap.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiLoNhap.APPROVED && x.NgayDuyet != null)
            .OrderByDescending(x => x.NgayDuyet)
            .Take(soLuong)
            .Select(x => new { x.SoLo, x.NgayDuyet, SoDong = x.ChiTiets.Count })
            .ToListAsync();
        list.AddRange(loNhaps.Select(x => new HoatDongDto
        {
            Loai = HoatDongLoai.Nhap,
            TieuDe = "Nhập kho tài sản",
            ChiTiet = $"{x.SoLo} - {x.SoDong} dòng chi tiết",
            ThoiGian = x.NgayDuyet!.Value
        }));

        // Điều chuyển đã duyệt
        var dieuChuyens = await _context.DieuChuyen.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiDieuChuyen.APPROVED && x.NgayDuyet != null)
            .OrderByDescending(x => x.NgayDuyet)
            .Take(soLuong)
            .Select(x => new
            {
                x.SoPhieu,
                x.NgayDuyet,
                Di = x.PhongBanChuyenDi.Name,
                Den = x.PhongBanChuyenDen.Name,
                SoTaiSan = x.ChiTiets.Count
            })
            .ToListAsync();
        list.AddRange(dieuChuyens.Select(x => new HoatDongDto
        {
            Loai = HoatDongLoai.DieuChuyen,
            TieuDe = "Điều chuyển tài sản",
            ChiTiet = $"{x.SoPhieu}: {x.Di} → {x.Den} ({x.SoTaiSan} tài sản)",
            ThoiGian = x.NgayDuyet!.Value
        }));

        // Báo mất / Thanh lý — lấy từ lịch sử có đích là Kho Thất Lạc / Kho Đã Thanh Lý
        var lichSus = await _context.LichSuDieuChuyenTaiSan.AsNoTracking()
            .Where(x => x.ViTriChuyenDen.Code == CodeKhoThatLac || x.ViTriChuyenDen.Code == CodeKhoThanhLy)
            .OrderByDescending(x => x.NgayDuyet)
            .Take(soLuong)
            .Select(x => new
            {
                DenCode = x.ViTriChuyenDen.Code,
                x.NgayDuyet,
                Ma = x.TaiSanDinhDanh.Code,
                Ten = x.TaiSanDinhDanh.Name
            })
            .ToListAsync();
        list.AddRange(lichSus.Select(x => new HoatDongDto
        {
            Loai = x.DenCode == CodeKhoThatLac ? HoatDongLoai.ThatLac : HoatDongLoai.ThanhLy,
            TieuDe = x.DenCode == CodeKhoThatLac ? "Báo mất tài sản" : "Thanh lý tài sản",
            ChiTiet = $"{x.Ma} - {x.Ten}",
            ThoiGian = (DateTime)x.NgayDuyet
        }));

        return list.OrderByDescending(x => x.ThoiGian).Take(soLuong).ToList();
    }

    private static double? Pct(double delta, double baseline) =>
        baseline <= 0 ? null : delta / baseline * 100;

    /// <summary>Lấy Top N theo giá trị giảm dần, phần còn lại gộp thành "Khác".</summary>
    private static List<ChartPointDto> TopVaGop(IEnumerable<ChartPointDto> source, int top)
    {
        var ordered = source.OrderByDescending(x => x.Value).ToList();
        if (ordered.Count <= top) return ordered;

        var head = ordered.Take(top).ToList();
        head.Add(new ChartPointDto("Khác", ordered.Skip(top).Sum(x => x.Value)));
        return head;
    }
}