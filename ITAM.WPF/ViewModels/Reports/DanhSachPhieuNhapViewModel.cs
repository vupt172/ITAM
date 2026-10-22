using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums;
using ITAM.ViewModels.Reports;
using ITAM.WPF.ViewModels.Reports;
using System.Collections.ObjectModel;
using System.Windows;

namespace ITAM.WPF.ViewModels.Reports;

public partial class DanhSachPhieuNhapViewModel : ReportViewModel
{
    private readonly IDanhSachPhieuNhapService _service;

    public DanhSachPhieuNhapViewModel(IDanhSachPhieuNhapService service) => _service = service;

    public ObservableCollection<LookupItem> NhaCungCaps { get; } = new();
    public ObservableCollection<LookupItem> NguoiLaps { get; } = new();
    public ObservableCollection<TrangThaiItem> TrangThais { get; } = new();
    public ObservableCollection<DanhSachPhieuNhapRowDto> Rows { get; } = new();

    [ObservableProperty] private DateTime? tuNgay;
    [ObservableProperty] private DateTime? denNgay;
    [ObservableProperty] private long? nhaCungCapId;
    [ObservableProperty] private string? soPhieu;
    [ObservableProperty] private string? maHoaDon;
    [ObservableProperty] private long? nguoiLapPhieuId;
    [ObservableProperty] private TrangThaiPhieu? trangThai;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string tongKet = "";

    public override async Task InitializeAsync()
    {
        try
        {
            var lk = await _service.GetLookupAsync();
            Fill(NhaCungCaps, lk.NhaCungCaps);
            Fill(NguoiLaps, lk.NguoiLaps);

            TrangThais.Clear();
            TrangThais.Add(new TrangThaiItem(null, "Tất cả"));
            foreach (var t in Enum.GetValues<TrangThaiPhieu>())
                TrangThais.Add(new TrangThaiItem(t, t.ToString()));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void Fill(ObservableCollection<LookupItem> target, IEnumerable<LookupItemDto> items)
    {
        target.Clear();
        target.Add(new LookupItem(null, "Tất cả"));
        foreach (var i in items) target.Add(new LookupItem(i.Id, i.Name));
    }

    private DanhSachPhieuNhapFilterDto BuildFilter() => new()
    {
        TuNgay = TuNgay,
        DenNgay = DenNgay,
        NhaCungCapId = NhaCungCapId,
        SoPhieu = SoPhieu,
        MaHoaDon = MaHoaDon,
        NguoiLapPhieuId = NguoiLapPhieuId,
        TrangThai = TrangThai
    };

    private string BuildMoTaBoLoc()
    {
        var parts = new List<string>();
        if (TuNgay.HasValue) parts.Add($"Từ ngày: {TuNgay:dd/MM/yyyy}");
        if (DenNgay.HasValue) parts.Add($"Đến ngày: {DenNgay:dd/MM/yyyy}");
        if (NhaCungCapId.HasValue) parts.Add($"NCC: {TenCua(NhaCungCaps, NhaCungCapId)}");
        if (!string.IsNullOrWhiteSpace(SoPhieu)) parts.Add($"Số phiếu: {SoPhieu}");
        if (!string.IsNullOrWhiteSpace(MaHoaDon)) parts.Add($"Mã hóa đơn: {MaHoaDon}");
        if (NguoiLapPhieuId.HasValue) parts.Add($"Người lập: {TenCua(NguoiLaps, NguoiLapPhieuId)}");
        if (TrangThai.HasValue) parts.Add($"Trạng thái: {TrangThai}");
        return parts.Count == 0 ? "Tất cả điều kiện" : string.Join(" | ", parts);
    }

    private static string TenCua(IEnumerable<LookupItem> list, long? id)
        => list.FirstOrDefault(x => x.Id == id)?.Name ?? "";

    private bool ValidateFilter()
    {
        if (TuNgay.HasValue && DenNgay.HasValue && TuNgay > DenNgay)
        {
            MessageBox.Show("Từ ngày không được lớn hơn Đến ngày.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }

    [RelayCommand]
    private async Task XemBaoCaoAsync()
    {
        if (!ValidateFilter()) return;
        try
        {
            IsBusy = true;
            var data = await _service.GetAsync(BuildFilter());

            Rows.Clear();
            foreach (var r in data) Rows.Add(r);

            TongKet = data.Count == 0
                ? "Không có dữ liệu."
                : $"{data.Count:N0} phiếu · {data.Sum(x => x.TongSoLuong):N0} hàng · " +
                  $"Tổng tiền: {data.Sum(x => x.TongTien):N0}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task XuatExcelAsync()
    {
        if (!ValidateFilter()) return;

        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"DanhSachPhieuNhap_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };
        if (dlg.ShowDialog() != true) return;

        try
        {
            IsBusy = true;
            await _service.ExportExcelAsync(BuildFilter(), BuildMoTaBoLoc(), dlg.FileName);
            MessageBox.Show("Xuất Excel thành công.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void DatLai()
    {
        TuNgay = null;
        DenNgay = null;
        NhaCungCapId = null;
        NguoiLapPhieuId = null;
        SoPhieu = null;
        MaHoaDon = null;
        TrangThai = null;
        Rows.Clear();
        TongKet = "";
    }
}