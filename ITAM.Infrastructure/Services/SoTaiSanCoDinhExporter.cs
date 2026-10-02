using ClosedXML.Excel;
using ITAM.AppCore.Interfaces;
using ITAM.Infrastructure.Data; // TODO xác nhận namespace AppDbContext
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ITAM.Infrastructure.Services;

/// <summary>
/// Xuất Sổ Tài Sản Cố Định (mẫu S24-H, TT 24/2024/TT-BTC).
/// Quy tắc (VUPT xác nhận):
///  - Chỉ lấy LoaiTaiSan = Tài Sản Cố Định
///  - Năm ghi tăng = NamSuDung; hao mòn tính TRÒN NĂM (không theo tháng)
///  - Ghi giảm để trống
///  - GiaNhap = 0 => vẫn liệt kê, KHÔNG tính (để trống nguyên giá/hao mòn/lũy kế)
/// </summary>
public class SoTaiSanCoDinhExporter : ISoTaiSanCoDinhService
{
    // TODO xác nhận: Code của LoaiTaiSan "Tài Sản Cố Định" (chưa có Constant/DbSeeder cho LoaiTaiSan)
    private const string MaLoaiTSCD = "TSCD";

    // ----- Chỉ số cột (mỗi cột dữ liệu đúng 1 cột Excel, không có cột gộp thừa) -----
    private const int cSTT = 1;
    private const int cSoCT = 2;        // B    Chứng từ - Số hiệu
    private const int cNgayCT = 3;      // C    Chứng từ - Ngày, tháng
    private const int cTen = 4;         // D    Tên, đặc điểm, ký hiệu TSCĐ
    private const int cNuocSX = 5;      // E    Nước sản xuất
    private const int cThangNam = 6;    // F    Tháng, năm đưa vào sử dụng
    private const int cSoHieu = 7;      // G    Số hiệu TSCĐ
    private const int cThe = 8;         // H    Thẻ TSCĐ
    private const int cNguyenGia = 9;   // I    Nguyên giá
    private const int cKhTyLe = 10;     // J    Khấu hao - Tỷ lệ %
    private const int cKhTien = 11;     // K    Khấu hao - Số tiền
    private const int cHmTyLe = 12;     // L    Hao mòn - Tỷ lệ %
    private const int cHmTien = 13;     // M    Hao mòn - Số tiền
    private const int cPhatSinh = 14;   // N    Tổng phát sinh trong năm
    private const int cLuyKe = 15;      // O    Lũy kế
    private const int cGgSoCT = 16;     // P    Ghi giảm - Chứng từ số hiệu
    private const int cGgNgay = 17;     // Q    Ghi giảm - Chứng từ ngày
    private const int cGgLyDo = 18;     // R    Lý do ghi giảm
    private const int cConLai = 19;     // S    Giá trị còn lại
    private const int cLkMa = 20;       // T    Liên kết ITAM - Mã tài sản
    private const int cLkPhu1 = 21;     // U    Liên kết ITAM - cột phụ 1 (chưa chọn nội dung)
    private const int cLkPhu2 = 22;     // V    Liên kết ITAM - cột phụ 2 (chưa chọn nội dung)
    private const int cCuoi = cLkPhu2;

    private readonly AppDbContext _context; // Transient theo convention

    public SoTaiSanCoDinhExporter(AppDbContext context) => _context = context;

    private sealed record Dong(
        string DanhMuc, string Ma, string Ten, int? NamSuDung, string? SoHieu,
        decimal GiaNhap, decimal TyLe);

    public async Task<byte[]> XuatAsync(int nam)
    {
        var data = await _context.TaiSanDinhDanh
            .AsNoTracking()
            .Where(x => x.LoaiTaiSan.Code == MaLoaiTSCD
                        && (x.NamSuDung == null || x.NamSuDung <= nam))
            .Select(x => new Dong(
                x.HangHoa.DMTaiSan.Name,      // TODO xác nhận: nhóm theo DMTaiSan (dòng "Loại tài sản: ...")
                x.Code,                       // Mã tài sản ITAM
                x.Name,
                x.NamSuDung,
                x.SoHieuTSCD,                 // TODO xác nhận tên property
                x.GiaNhap,
                x.LoaiTaiSan.TyLeHaoMon??0))     // TODO xác nhận tên property + kiểu (0.2 hay 20)
            .ToListAsync();

        var groups = data
            .GroupBy(x => x.DanhMuc)
            .OrderBy(g => g.Key);

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Sổ TSCĐ");

        VeTieuDe(ws, nam);

        int row = 12;
        foreach (var g in groups)
        {
            int groupRow = row++;
            int first = row;
            int stt = 1;

            foreach (var d in g.OrderBy(x => x.NamSuDung).ThenBy(x => x.SoHieu))
            {
                GhiDong(ws, row++, stt++, d, nam);
            }

            int last = row - 1;
            GhiDongNhom(ws, groupRow, g.Key, first, last);
        }

        if (row > 12)
        {
            var body = ws.Range(12, 1, row - 1, cCuoi);
            body.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            body.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            foreach (var col in new[] { cNguyenGia, cKhTien, cHmTien, cPhatSinh, cLuyKe, cConLai })
                ws.Range(12, col, row - 1, col).Style.NumberFormat.Format = "#,##0";
            foreach (var col in new[] { cKhTyLe, cHmTyLe })
                ws.Range(12, col, row - 1, col).Style.NumberFormat.Format = "0.00%";
        }

        ws.Column(cSTT).Width = 5;
        ws.Column(cSoCT).Width = 10;
        ws.Column(cNgayCT).Width = 12;
        ws.Column(cTen).Width = 40;
        ws.Column(cNuocSX).Width = 12;
        ws.Column(cThangNam).Width = 14;
        ws.Column(cSoHieu).Width = 22;
        ws.Column(cThe).Width = 22;
        ws.Column(cNguyenGia).Width = 16;
        ws.Column(cKhTyLe).Width = 9;
        ws.Column(cKhTien).Width = 13;
        ws.Column(cHmTyLe).Width = 9;
        ws.Column(cHmTien).Width = 14;
        ws.Column(cPhatSinh).Width = 18;
        ws.Column(cLuyKe).Width = 18;
        ws.Column(cConLai).Width = 14;
        ws.Column(cLkMa).Width = 18;
        ws.Column(cLkPhu1).Width = 16;
        ws.Column(cLkPhu2).Width = 16;

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ---------- Tính hao mòn (tròn năm) ----------

    private static decimal LuyKe(decimal giaNhap, decimal tyLe, int soNam)
    {
        if (soNam <= 0) return 0m;
        return Math.Min(giaNhap, giaNhap * tyLe * soNam);
    }

    private static void GhiDong(IXLWorksheet ws, int r, int stt, Dong d, int nam)
    {
        ws.Cell(r, cSTT).Value = stt;
        ws.Cell(r, cTen).Value = d.Ten;
        if (d.NamSuDung.HasValue)
            ws.Cell(r, cThangNam).Value = d.NamSuDung.Value;   // chỉ có năm (chưa có tháng)
        ws.Cell(r, cSoHieu).Value = d.SoHieu ?? "";
        ws.Cell(r, cThe).Value = d.SoHieu ?? "";                // Thẻ TSCĐ = Số hiệu TSCĐ
        ws.Cell(r, cLkMa).Value = d.Ma;                         // Liên kết ITAM - Mã tài sản

        // Chưa xác định giá trị hoặc thiếu năm sử dụng => liệt kê, không tính
        if (d.GiaNhap <= 0 || !d.NamSuDung.HasValue) return;

        var tyLe = d.TyLe > 1 ? d.TyLe / 100m : d.TyLe;         // chấp nhận cả 0.2 lẫn 20
        int soNam = nam - d.NamSuDung.Value + 1;                // tính đủ năm ghi tăng

        decimal luyKe = LuyKe(d.GiaNhap, tyLe, soNam);
        decimal phatSinh = luyKe - LuyKe(d.GiaNhap, tyLe, soNam - 1);

        ws.Cell(r, cNguyenGia).Value = d.GiaNhap;
        ws.Cell(r, cHmTyLe).Value = tyLe;
        if (phatSinh > 0)
        {
            ws.Cell(r, cHmTien).Value = phatSinh;
            ws.Cell(r, cPhatSinh).Value = phatSinh;             // = hao mòn (không có khấu hao)
        }
        ws.Cell(r, cLuyKe).Value = luyKe;
        // Ghi giảm (R..U) để trống
    }

    private static void GhiDongNhom(IXLWorksheet ws, int r, string ten, int first, int last)
    {
        ws.Cell(r, 1).Value = $"Loại tài sản: {ten}";
        ws.Range(r, 1, r, cThe).Merge();
        if (last >= first)
        {
            foreach (var col in new[] { cNguyenGia, cHmTien, cPhatSinh, cLuyKe })
            {
                var c = ws.Cell(r, col);
                var letter = c.Address.ColumnLetter;
                c.FormulaA1 = $"SUM({letter}{first}:{letter}{last})";
            }
        }
        ws.Range(r, 1, r, cCuoi).Style.Font.Bold = true;
    }

    // ---------- Tiêu đề, header mẫu S24-H ----------

    private static void VeTieuDe(IXLWorksheet ws, int nam)
    {
        ws.Cell(2, 1).Value = "Đơn vị: Bệnh Viện Đa Khoa Khu Vực Long Thành";
        ws.Cell(3, 1).Value = "Mã QHNS: 1073794";
        ws.Cell(2, cGgSoCT).Value = "Mẫu số: S24-H";
        ws.Cell(3, cGgSoCT).Value = "(Ban hành theo Thông tư số 24/2024/TT-BTC ngày 17/04/2024 của Bộ Tài chính)";

        ws.Cell(5, 1).Value = "SỔ TÀI SẢN CỐ ĐỊNH";
        ws.Range(5, 1, 5, cCuoi).Merge().Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        ws.Cell(6, 1).Value = $"Năm {nam}";
        ws.Range(6, 1, 6, cCuoi).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        void Gop(int r1, int c1, int r2, int c2, string text)
        {
            ws.Cell(r1, c1).Value = text;
            ws.Range(r1, c1, r2, c2).Merge();
        }

        // Hàng 8
        Gop(8, cSTT, 10, cSTT, "STT");
        Gop(8, cSoCT, 9, cNgayCT, "Chứng từ");
        Gop(8, cTen, 8, cNguyenGia, "Ghi tăng tài sản cố định");
        Gop(8, cKhTyLe, 8, cLuyKe, "Khấu hao (hao mòn) tài sản cố định");
        Gop(8, cGgSoCT, 8, cConLai, "Ghi giảm TSCĐ");
        Gop(8, cLkMa, 8, cLkPhu2, "Liên Kết Phần Mềm ITAM");

        // Hàng 9 (gộp xuống hàng 10 với cột đơn)
        Gop(9, cTen, 10, cTen, "Tên, đặc điểm, ký hiệu TSCĐ");
        Gop(9, cNuocSX, 10, cNuocSX, "Nước sản xuất");
        Gop(9, cThangNam, 10, cThangNam, "Tháng, năm đưa vào sử dụng ở đơn vị");
        Gop(9, cSoHieu, 10, cSoHieu, "Số hiệu TSCĐ");
        Gop(9, cThe, 10, cThe, "Thẻ TSCĐ");
        Gop(9, cNguyenGia, 10, cNguyenGia, "Nguyên giá TSCĐ");
        Gop(9, cKhTyLe, 9, cKhTien, "Khấu hao");
        Gop(9, cHmTyLe, 9, cHmTien, "Hao mòn");
        Gop(9, cPhatSinh, 10, cPhatSinh, "Tổng số khấu hao (hao mòn) phát sinh trong năm");
        Gop(9, cLuyKe, 10, cLuyKe, "Lũy kế khấu hao/hao mòn đã tính đến khi chuyển sổ hoặc ghi giảm TSCĐ");
        Gop(9, cGgSoCT, 9, cGgNgay, "Chứng từ");
        Gop(9, cGgLyDo, 10, cGgLyDo, "Lý do ghi giảm TSCĐ");
        Gop(9, cConLai, 10, cConLai, "Giá trị còn lại của TSCĐ");
        Gop(9, cLkMa, 10, cLkMa, "Mã tài sản");
        Gop(9, cLkPhu1, 10, cLkPhu1, "");   // chưa đặt tên
        Gop(9, cLkPhu2, 10, cLkPhu2, "");   // chưa đặt tên

        // Hàng 10
        ws.Cell(10, cSoCT).Value = "Số hiệu";
        ws.Cell(10, cNgayCT).Value = "Ngày, tháng";
        ws.Cell(10, cKhTyLe).Value = "Tỷ lệ %";
        ws.Cell(10, cKhTien).Value = "Số tiền";
        ws.Cell(10, cHmTyLe).Value = "Tỷ lệ %";
        ws.Cell(10, cHmTien).Value = "Số tiền";
        ws.Cell(10, cGgSoCT).Value = "Số hiệu";
        ws.Cell(10, cGgNgay).Value = "Ngày, tháng";

        // Hàng 11: ký hiệu cột
        var ky = new (int col, string val)[]
        {
            (cSTT,"A"),(cSoCT,"B"),(cNgayCT,"C"),(cTen,"D"),(cNuocSX,"E"),(cThangNam,"F"),
            (cSoHieu,"G"),(cThe,"H"),(cNguyenGia,"1"),(cKhTyLe,"2"),(cKhTien,"3"),
            (cHmTyLe,"4"),(cHmTien,"5"),(cPhatSinh,"6=3+5"),(cLuyKe,"7"),
            (cGgSoCT,"I"),(cGgNgay,"K"),(cGgLyDo,"L"),(cConLai,"8")
        };
        foreach (var (col, val) in ky) ws.Cell(11, col).Value = val;

        var header = ws.Range(8, 1, 11, cCuoi);
        header.Style.Font.Bold = true;
        header.Style.Alignment.WrapText = true;
        header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }
}