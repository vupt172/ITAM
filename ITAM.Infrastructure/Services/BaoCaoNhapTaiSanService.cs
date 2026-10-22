using ClosedXML.Excel;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.AppCore.Interfaces;
using ITAM.Infrastructure.Data;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services;

public class BaoCaoNhapTaiSanService : IBaoCaoNhapTaiSanService
{
    private readonly AppDbContext _context;

    public BaoCaoNhapTaiSanService(AppDbContext context) => _context = context;

    public async Task<NhapTaiSanLookupDto> GetLookupAsync()
    {
        return new NhapTaiSanLookupDto
        {
            NhaCungCaps = await _context.NhaCungCap.AsNoTracking().OrderBy(x => x.Name)   // TODO xác nhận tên DbSet
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),

            // Chỉ lấy những người đã từng lập phiếu nhập
            NguoiLaps = await _context.LoNhap.AsNoTracking()
                .Select(x => new LookupItemDto { Id = x.NguoiLapPhieuId, Name = x.NguoiLapPhieu.FullName })
                .Distinct().OrderBy(x => x.Name).ToListAsync(),

            DanhMucs = await _context.DMTaiSan.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),

            HangHoas = await _context.HangHoa.AsNoTracking().OrderBy(x => x.Code)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Code + " - " + x.Name }).ToListAsync()
        };
    }

    public Task<List<BaoCaoNhapTaiSanRowDto>> GetAsync(BaoCaoNhapTaiSanFilterDto f)
        => BuildQuery(f).ToListAsync();

    private IQueryable<BaoCaoNhapTaiSanRowDto> BuildQuery(BaoCaoNhapTaiSanFilterDto f)
    {
        var q = _context.LoNhapChiTiet.AsNoTracking().AsQueryable();

        if (f.TuNgay.HasValue) q = q.Where(x => x.LoNhap.NgayNhap >= f.TuNgay.Value.Date);
        if (f.DenNgay.HasValue) q = q.Where(x => x.LoNhap.NgayNhap < f.DenNgay.Value.Date.AddDays(1));
        if (f.NhaCungCapId.HasValue) q = q.Where(x => x.LoNhap.NhaCungCapId == f.NhaCungCapId.Value);
        if (f.NguoiLapPhieuId.HasValue) q = q.Where(x => x.LoNhap.NguoiLapPhieuId == f.NguoiLapPhieuId.Value);
        if (f.TrangThai.HasValue) q = q.Where(x => x.LoNhap.TrangThai == f.TrangThai.Value);
        if (f.HangHoaId.HasValue) q = q.Where(x => x.HangHoaId == f.HangHoaId.Value);
        if (f.DMTaiSanId.HasValue) q = q.Where(x => x.HangHoa.DMTaiSanId == f.DMTaiSanId.Value);
        if (!string.IsNullOrWhiteSpace(f.SoPhieu))
            q = q.Where(x => x.SoLo.Contains(f.SoPhieu.Trim()));
        if (!string.IsNullOrWhiteSpace(f.MaHoaDon))
            q = q.Where(x => x.LoNhap.MaHoaDon != null && x.LoNhap.MaHoaDon.Contains(f.MaHoaDon.Trim()));

        return q.OrderBy(x => x.SoLo).ThenBy(x => x.Id)
            .Select(x => new BaoCaoNhapTaiSanRowDto
            {
                SoPhieu = x.SoLo,
                NgayNhap = x.LoNhap.NgayNhap,
                NgayDuyet = x.LoNhap.NgayDuyet,
                NhaCungCap = x.LoNhap.NhaCungCap.Name,
                MaHoaDon = x.LoNhap.MaHoaDon,
                MaHangHoa = x.HangHoa.Code,
                TenHangHoa = x.HangHoa.Name,
                DanhMuc = x.HangHoa.DMTaiSan.Name,
                LoaiTaiSan = x.LoaiTaiSan.Name,
                SoLuongNhap = x.SoLuongNhap,
                DonGia = x.DonGia,
                NguoiLapPhieu = x.LoNhap.NguoiLapPhieu.FullName,
                TrangThai = x.LoNhap.TrangThai.ToString()
            });
    }

    public async Task ExportExcelAsync(BaoCaoNhapTaiSanFilterDto f, string moTaBoLoc, string filePath)
    {
        var rows = await GetAsync(f);
        const int cols = 14;

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Nhập tài sản");

        ws.Cell(1, 1).Value = "BÁO CÁO NHẬP TÀI SẢN TỪ NHÀ CUNG CẤP";
        ws.Range(1, 1, 1, cols).Merge().Style.Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(2, 1).Value = moTaBoLoc;
        ws.Range(2, 1, 2, cols).Merge().Style.Font.SetItalic()
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        const int headerRow = 4;
        string[] heads = { "Số phiếu", "Ngày nhập", "Ngày duyệt", "Nhà cung cấp", "Mã hóa đơn",
            "Mã hàng hóa", "Tên hàng hóa", "Danh mục", "Loại tài sản", "Số lượng",
            "Đơn giá", "Thành tiền", "Người lập phiếu", "Trạng thái" };
        for (int i = 0; i < cols; i++) ws.Cell(headerRow, i + 1).Value = heads[i];
        ws.Range(headerRow, 1, headerRow, cols).Style.Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        int r = headerRow + 1;
        foreach (var g in rows.GroupBy(x => x.SoPhieu))
        {
            ws.Cell(r, 1).Value = $"PHIẾU: {g.Key} - {g.First().NhaCungCap}";
            ws.Range(r, 1, r, cols).Merge().Style.Font.SetBold();
            r++;

            foreach (var x in g)
            {
                ws.Cell(r, 1).Value = x.SoPhieu;
                ws.Cell(r, 2).Value = x.NgayNhap;
                if (x.NgayDuyet.HasValue) ws.Cell(r, 3).Value = x.NgayDuyet.Value;
                ws.Cell(r, 4).Value = x.NhaCungCap;
                ws.Cell(r, 5).Value = x.MaHoaDon;
                ws.Cell(r, 6).Value = x.MaHangHoa;
                ws.Cell(r, 7).Value = x.TenHangHoa;
                ws.Cell(r, 8).Value = x.DanhMuc;
                ws.Cell(r, 9).Value = x.LoaiTaiSan;
                ws.Cell(r, 10).Value = x.SoLuongNhap;
                ws.Cell(r, 11).Value = x.DonGia;
                ws.Cell(r, 12).Value = x.ThanhTien;
                ws.Cell(r, 13).Value = x.NguoiLapPhieu;
                ws.Cell(r, 14).Value = x.TrangThai;
                r++;
            }

            ws.Cell(r, 9).Value = "Tổng phiếu";
            ws.Cell(r, 10).Value = g.Sum(x => x.SoLuongNhap);
            ws.Cell(r, 12).Value = g.Sum(x => x.ThanhTien);
            ws.Range(r, 9, r, 12).Style.Font.SetBold();
            r += 2;   // dòng trống giữa các phiếu
        }

        ws.Cell(r, 9).Value = "Tổng cộng";
        ws.Cell(r, 10).Value = rows.Sum(x => x.SoLuongNhap);
        ws.Cell(r, 12).Value = rows.Sum(x => x.ThanhTien);
        ws.Range(r, 9, r, 12).Style.Font.SetBold();

        ws.Range(headerRow + 1, 2, r, 3).Style.DateFormat.SetFormat("dd/MM/yyyy");
        ws.Range(headerRow + 1, 10, r, 12).Style.NumberFormat.SetFormat("#,##0");
        ws.Columns().AdjustToContents();

        wb.SaveAs(filePath);
    }
}
