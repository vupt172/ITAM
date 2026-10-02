using ClosedXML.Excel; // TODO xác nhận: cài package ClosedXML
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums;
using ITAM.Domain.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ITAM.WPF.ViewModels.Reports
{
    public partial class BaoCaoTaiSanViewModel : ObservableObject
    {
        private readonly IBaoCaoTaiSanService _service;
        private readonly ICatalogService<PhongBan> _phongBanService;
        private readonly ICatalogService<ViTriTaiSan> _viTriTaiSanService;
        private readonly ICatalogService<LoaiTaiSan> _loaiTaiSanService;
        private readonly ICatalogService<DMTaiSan> _dmTaiSanService;

        [ObservableProperty] private DateTime? tuNgay;
        [ObservableProperty] private DateTime? denNgay;

        [ObservableProperty] private long? phongBanId;
        [ObservableProperty] private long? viTriTaiSanId;
        [ObservableProperty] private long? loaiTaiSanId;
        [ObservableProperty] private long? dmTaiSanId;
        [ObservableProperty] private TrangThaiTaiSan? trangThai;
        [ObservableProperty] private string? hangSanXuat;

        public ObservableCollection<PhongBan> DsPhongBan { get; } = [];
        public ObservableCollection<ViTriTaiSan> DsViTriTaiSan { get; } = [];
        public ObservableCollection<LoaiTaiSan> DsLoaiTaiSan { get; } = [];
        public ObservableCollection<DMTaiSan> DsDMTaiSan { get; } = [];

        // Nguồn cho ComboBox Trạng thái — enum trực tiếp, không map tên hiển thị
        public Array DsTrangThai { get; } = Enum.GetValues(typeof(TrangThaiTaiSan));

        public BaoCaoTaiSanViewModel(
            IBaoCaoTaiSanService service,
            ICatalogService<PhongBan> phongBanService,
            ICatalogService<ViTriTaiSan> viTriTaiSanService,
            ICatalogService<LoaiTaiSan> loaiTaiSanService,
            ICatalogService<DMTaiSan> dmTaiSanService)
        {
            _service = service;
            _phongBanService = phongBanService;
            _viTriTaiSanService = viTriTaiSanService;
            _loaiTaiSanService = loaiTaiSanService;
            _dmTaiSanService = dmTaiSanService;
        }

        public async Task InitializeAsync()
        {
            DsPhongBan.Clear();
            foreach (var x in await _phongBanService.GetAllAsync()) DsPhongBan.Add(x);

            DsViTriTaiSan.Clear();
            foreach (var x in await _viTriTaiSanService.GetAllAsync()) DsViTriTaiSan.Add(x);

            DsLoaiTaiSan.Clear();
            foreach (var x in await _loaiTaiSanService.GetAllAsync()) DsLoaiTaiSan.Add(x);

            DsDMTaiSan.Clear();
            foreach (var x in await _dmTaiSanService.GetAllAsync()) DsDMTaiSan.Add(x);
        }

        [RelayCommand]
        private void DatLai()
        {
            TuNgay = null; DenNgay = null;
            PhongBanId = null; ViTriTaiSanId = null;
            LoaiTaiSanId = null; DmTaiSanId = null;
            TrangThai = null; HangSanXuat = null;
        }

        [RelayCommand]
        private async Task XuatExcel()
        {
            var dto = new BaoCaoTaiSanSearchDto
            {
                TuNgay = TuNgay,
                DenNgay = DenNgay,
                PhongBanId = PhongBanId,
                ViTriTaiSanId = ViTriTaiSanId,
                LoaiTaiSanId = LoaiTaiSanId,
                DMTaiSanId = DmTaiSanId,
                TrangThai = TrangThai,
                HangSanXuat = HangSanXuat
            };

            var ketQua = await _service.TimKiemAsync(dto);

            if (ketQua.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu phù hợp với điều kiện lọc.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"DanhSachTaiSan_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };
            if (dialog.ShowDialog() != true) return;

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Danh sách tài sản");

            var cols = new[]
            {
        "STT", "Mã Tài Sản", "Tên Tài Sản", "Serial", "Mã Hàng Hóa", "Loại Tài Sản",
        "Trạng Thái", "Vị Trí", "Năm Sử Dụng", "Số Phiếu Nhập Chi Tiết","Giá Nhập","Ghi Chú"
    };
            int soCot = cols.Length;

            int row = 1;

            // Tiêu đề báo cáo
            ws.Cell(row, 1).Value = "BÁO CÁO DANH SÁCH TÀI SẢN";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Font.FontSize = 14;
            row += 2;

            ws.Cell(row, 1).Value = $"Từ ngày: {(TuNgay.HasValue ? TuNgay.Value.ToString("dd/MM/yyyy") : "Tất cả")}";
            row++;
            ws.Cell(row, 1).Value = $"Đến ngày: {(DenNgay.HasValue ? DenNgay.Value.ToString("dd/MM/yyyy") : "Tất cả")}";
            row += 2;

            int stt = 1;

            foreach (var nhom in ketQua.GroupBy(x => x.TenPhongBan))
            {
                // Tiêu đề nhóm phòng ban
                ws.Cell(row, 1).Value = $"PHÒNG BAN: {nhom.Key}";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0xD9, 0xE1, 0xF2);
                row++;

                // Header bảng
                int headerRow = row;
                for (int c = 0; c < soCot; c++)
                {
                    var cell = ws.Cell(headerRow, c + 1);
                    cell.Value = cols[c];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(0xBD, 0xD7, 0xEE);
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                row++;

                int firstDataRow = row;
                foreach (var item in nhom)
                {
                    ws.Cell(row, 1).Value = stt++;
                    ws.Cell(row, 2).Value = item.MaTaiSan;
                    ws.Cell(row, 3).Value = item.TenTaiSan;
                    ws.Cell(row, 4).Value = item.Serial;
                    ws.Cell(row, 5).Value = item.MaHangHoa;
                    ws.Cell(row, 6).Value = item.TenLoaiTaiSan;
                    ws.Cell(row, 7).Value = item.TrangThai.ToString();
                    ws.Cell(row, 8).Value = item.TenViTri;
                    ws.Cell(row, 9).Value = item.NamSuDung;
                    ws.Cell(row, 10).Value = item.SoPhieuNhapChiTiet;
                    ws.Cell(row,11).Value = item.GiaNhap;
                    ws.Cell(row, 12).Value = item.GhiChu;

                    row++;
                }
                int lastDataRow = row - 1;

                // Kẻ border cho toàn bộ vùng dữ liệu (header + data) của nhóm này
                var vungBang = ws.Range(headerRow, 1, lastDataRow, soCot);
                vungBang.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                vungBang.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Căn giữa cột STT
                ws.Range(firstDataRow, 1, lastDataRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                row++; // dòng trống giữa các nhóm phòng ban
            }

            ws.Columns().AdjustToContents();      // 1. auto-fit các cột theo nội dung
            ws.Column(1).Width = 6;               // 2. rồi mới ép lại cột STT
            ws.Column(3).Width = 40;              // 3. và cột Tên Tài Sản về độ rộng cố định
            ws.Column(3).Style.Alignment.WrapText = true;  // 4. bật wrap cho cột đó
            ws.Rows().AdjustToContents();

            wb.SaveAs(dialog.FileName);

            MessageBox.Show("Xuất Excel thành công.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}