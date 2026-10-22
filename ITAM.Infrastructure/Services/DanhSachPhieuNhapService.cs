using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClosedXML.Excel;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services;

public class DanhSachPhieuNhapService : IDanhSachPhieuNhapService
{
    private readonly AppDbContext _context;

    public DanhSachPhieuNhapService(AppDbContext context) => _context = context;

    public async Task<PhieuNhapLookupDto> GetLookupAsync()
    {
        return new PhieuNhapLookupDto
        {
            NhaCungCaps = await _context.NhaCungCap.AsNoTracking().OrderBy(x => x.Name)   // TODO xác nhận tên DbSet
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name }).ToListAsync(),

            NguoiLaps = await _context.LoNhap.AsNoTracking()
                .Select(x => new LookupItemDto { Id = x.NguoiLapPhieuId, Name = x.NguoiLapPhieu.FullName })
                .Distinct().OrderBy(x => x.Name).ToListAsync()
        };
    }

    public Task<List<DanhSachPhieuNhapRowDto>> GetAsync(DanhSachPhieuNhapFilterDto f)
    {
        var q = _context.LoNhap.AsNoTracking().AsQueryable();

        if (f.TuNgay.HasValue) q = q.Where(x => x.NgayNhap >= f.TuNgay.Value.Date);
        if (f.DenNgay.HasValue) q = q.Where(x => x.NgayNhap < f.DenNgay.Value.Date.AddDays(1));
        if (f.NhaCungCapId.HasValue) q = q.Where(x => x.NhaCungCapId == f.NhaCungCapId.Value);
        if (f.NguoiLapPhieuId.HasValue) q = q.Where(x => x.NguoiLapPhieuId == f.NguoiLapPhieuId.Value);
        if (f.TrangThai.HasValue) q = q.Where(x => x.TrangThai == f.TrangThai.Value);
        if (!string.IsNullOrWhiteSpace(f.SoPhieu))
            q = q.Where(x => x.SoLo.Contains(f.SoPhieu.Trim()));           // TODO xác nhận: LoNhap.SoPhieu
        if (!string.IsNullOrWhiteSpace(f.MaHoaDon))
            q = q.Where(x => x.MaHoaDon != null && x.MaHoaDon.Contains(f.MaHoaDon.Trim()));

        return q.OrderByDescending(x => x.NgayNhap).ThenByDescending(x => x.SoLo)
            .Select(x => new DanhSachPhieuNhapRowDto
            {
                SoPhieu = x.SoLo,
                NgayNhap = x.NgayNhap,
                NgayDuyet = x.NgayDuyet,
                NhaCungCap = x.NhaCungCap.Name,
                MaHoaDon = x.MaHoaDon,
                NguoiLapPhieu = x.NguoiLapPhieu.FullName,
                TrangThai = x.TrangThai.ToString(),
                SoDong = x.ChiTiets.Count(),                                    // TODO xác nhận tên navigation
                TongSoLuong = x.ChiTiets.Sum(c => c.SoLuongNhap),
                TongTien = x.ChiTiets.Sum(c => c.SoLuongNhap * c.DonGia)
            })
            .ToListAsync();
    }

    public async Task ExportExcelAsync(DanhSachPhieuNhapFilterDto f, string moTaBoLoc, string filePath)
    {
        var rows = await GetAsync(f);
        const int cols = 11;

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Danh sách phiếu nhập");

        ws.Cell(1, 1).Value = "DANH SÁCH PHIẾU NHẬP";
        ws.Range(1, 1, 1, cols).Merge().Style.Font.SetBold().Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(2, 1).Value = moTaBoLoc;
        ws.Range(2, 1, 2, cols).Merge().Style.Font.SetItalic()
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        const int headerRow = 4;
        string[] heads = { "STT", "Số phiếu", "Ngày nhập", "Ngày duyệt", "Nhà cung cấp", "Mã hóa đơn",
            "Người lập phiếu", "Trạng thái", "Số dòng", "Tổng số lượng", "Tổng tiền" };
        for (int i = 0; i < cols; i++) ws.Cell(headerRow, i + 1).Value = heads[i];
        ws.Range(headerRow, 1, headerRow, cols).Style.Font.SetBold()
            .Fill.SetBackgroundColor(XLColor.LightGray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        int r = headerRow + 1;
        int stt = 1;
        foreach (var x in rows)
        {
            ws.Cell(r, 1).Value = stt++;
            ws.Cell(r, 2).Value = x.SoPhieu;
            ws.Cell(r, 3).Value = x.NgayNhap;
            if (x.NgayDuyet.HasValue) ws.Cell(r, 4).Value = x.NgayDuyet.Value;
            ws.Cell(r, 5).Value = x.NhaCungCap;
            ws.Cell(r, 6).Value = x.MaHoaDon;
            ws.Cell(r, 7).Value = x.NguoiLapPhieu;
            ws.Cell(r, 8).Value = x.TrangThai;
            ws.Cell(r, 9).Value = x.SoDong;
            ws.Cell(r, 10).Value = x.TongSoLuong;
            ws.Cell(r, 11).Value = x.TongTien;
            r++;
        }

        ws.Cell(r, 8).Value = "Tổng cộng";
        ws.Cell(r, 9).Value = rows.Sum(x => x.SoDong);
        ws.Cell(r, 10).Value = rows.Sum(x => x.TongSoLuong);
        ws.Cell(r, 11).Value = rows.Sum(x => x.TongTien);
        ws.Range(r, 8, r, 11).Style.Font.SetBold();

        ws.Range(headerRow + 1, 3, r, 4).Style.DateFormat.SetFormat("dd/MM/yyyy");
        ws.Range(headerRow + 1, 9, r, 11).Style.NumberFormat.SetFormat("#,##0");
        ws.Range(headerRow, 1, r, cols).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetInsideBorder(XLBorderStyleValues.Thin);
        ws.Columns().AdjustToContents();

        wb.SaveAs(filePath);
    }
}
