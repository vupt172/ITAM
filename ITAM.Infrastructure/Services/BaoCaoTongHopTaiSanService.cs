using ClosedXML.Excel;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services;

public class BaoCaoTongHopTaiSanService : IBaoCaoTongHopTaiSanService
{
    private const string ChuaXacDinh = "(Chưa xác định)";
    private readonly AppDbContext _context; // TODO xác nhận namespace AppDbContext

    public BaoCaoTongHopTaiSanService(AppDbContext context) => _context = context;

    public async Task<TongHopTaiSanLookupDto> GetLookupAsync()
    {
        // TODO xác nhận tên DbSet (PhongBan/PhongBans, ...)
        return new TongHopTaiSanLookupDto
        {
            PhongBans = await _context.PhongBan.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),
            ViTris = await _context.ViTriTaiSan.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name, ParentId = x.PhongBanId }).ToListAsync(),
            LoaiTaiSans = await _context.LoaiTaiSan.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),
            DanhMucs = await _context.DMTaiSan.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),
            HangSanXuats = await _context.HangHoa.AsNoTracking()
                .Where(x => x.HangSanXuat != null && x.HangSanXuat != "")
                .Select(x => x.HangSanXuat!).Distinct().OrderBy(x => x).ToListAsync()
        };
    }

    public async Task<List<TongHopTaiSanRowDto>> GetAsync(TongHopTaiSanFilterDto f)
    {
        var q = _context.TaiSanDinhDanh.AsNoTracking().AsQueryable();

        if (f.PhongBanId.HasValue) q = q.Where(x => x.ViTriTaiSan.PhongBanId == f.PhongBanId.Value);
        if (f.ViTriTaiSanId.HasValue) q = q.Where(x => x.ViTriTaiSanId == f.ViTriTaiSanId.Value);
        if (f.LoaiTaiSanId.HasValue) q = q.Where(x => x.LoaiTaiSanId == f.LoaiTaiSanId.Value);
        if (f.DMTaiSanId.HasValue) q = q.Where(x => x.HangHoa.DMTaiSanId == f.DMTaiSanId.Value);
        if (!string.IsNullOrEmpty(f.HangSanXuat)) q = q.Where(x => x.HangHoa.HangSanXuat == f.HangSanXuat);
        if (f.TrangThai.HasValue) q = q.Where(x => x.TrangThaiTaiSan == f.TrangThai.Value);

        // Chỉ project cột cần dùng, rồi group trong bộ nhớ (tránh rủi ro EF không dịch được
        // GroupBy theo navigation/enum-string). Quy mô ~ vài nghìn tài sản nên chấp nhận được.
        var raw = await q.Select(x => new
        {
            PhongBan = x.ViTriTaiSan.PhongBan.Name,
            ViTri = x.ViTriTaiSan.Name,
            Loai = x.LoaiTaiSan.Name,
            DanhMuc = x.HangHoa.DMTaiSan.Name,
            Hang = x.HangHoa.HangSanXuat,
            TrangThai = x.TrangThaiTaiSan,
            x.GiaNhap
        }).ToListAsync();

        return raw
            .GroupBy(x => f.NhomTheo switch
            {
                NhomTheoTongHop.PhongBan => x.PhongBan,
                NhomTheoTongHop.ViTri => x.ViTri,
                NhomTheoTongHop.LoaiTaiSan => x.Loai,
                NhomTheoTongHop.DanhMuc => x.DanhMuc,
                NhomTheoTongHop.HangSanXuat => x.Hang,
                _ => x.TrangThai.ToString()
            } is { Length: > 0 } key ? key : ChuaXacDinh)
            .Select(g => new TongHopTaiSanRowDto
            {
                TenNhom = g.Key,
                SoLuong = g.Count(),
                TongNguyenGia = g.Sum(x => x.GiaNhap)
            })
            .OrderBy(r => r.TenNhom)
            .ToList();
    }

    public async Task ExportExcelAsync(TongHopTaiSanFilterDto f, string tenCotNhom, string moTaBoLoc, string filePath)
    {
        var rows = await GetAsync(f);

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Tổng hợp tài sản");

        ws.Cell(1, 1).Value = "BÁO CÁO TỔNG HỢP TÀI SẢN";
        ws.Range(1, 1, 1, 3).Merge().Style.Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(2, 1).Value = moTaBoLoc;
        ws.Range(2, 1, 2, 3).Merge().Style.Font.SetItalic()
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        const int headerRow = 4;
        ws.Cell(headerRow, 1).Value = tenCotNhom;
        ws.Cell(headerRow, 2).Value = "Số lượng";
        ws.Cell(headerRow, 3).Value = "Tổng nguyên giá";
        ws.Range(headerRow, 1, headerRow, 3).Style.Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        int r = headerRow + 1;
        foreach (var row in rows)
        {
            ws.Cell(r, 1).Value = row.TenNhom;
            ws.Cell(r, 2).Value = row.SoLuong;
            ws.Cell(r, 3).Value = row.TongNguyenGia;
            r++;
        }

        ws.Cell(r, 1).Value = "Tổng cộng";
        ws.Cell(r, 2).Value = rows.Sum(x => x.SoLuong);
        ws.Cell(r, 3).Value = rows.Sum(x => x.TongNguyenGia);
        ws.Range(r, 1, r, 3).Style.Font.SetBold();

        ws.Range(headerRow + 1, 2, r, 2).Style.NumberFormat.SetFormat("#,##0");
        ws.Range(headerRow + 1, 3, r, 3).Style.NumberFormat.SetFormat("#,##0");
        ws.Range(headerRow, 1, r, 3).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetInsideBorder(XLBorderStyleValues.Thin);

        ws.Column(1).Width = 40;
        ws.Column(2).Width = 14;
        ws.Column(3).Width = 22;

        wb.SaveAs(filePath);
    }
}