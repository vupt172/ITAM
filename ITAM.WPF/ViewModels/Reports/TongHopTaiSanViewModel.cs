using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums; // TODO xác nhận namespace TrangThaiTaiSan
using ITAM.Infrastructure.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace ITAM.WPF.ViewModels.Reports;

public record TrangThaiOption(TrangThaiTaiSan? Value, string Text);
public record NhomTheoOption(NhomTheoTongHop Value, string Text);

// TODO xác nhận: BaseViewModel kế thừa ObservableObject
public partial class TongHopTaiSanViewModel : BaseViewModel
{
    private const string TatCa = "Tất cả";
    private readonly IBaoCaoTongHopTaiSanService _service;
    private List<LookupItemDto> _allViTri = new();

    // Các collection chỉ mutate tại chỗ (Clear/Add), không gán lại instance.
    public ObservableCollection<LookupItemDto> PhongBans { get; } = new();
    public ObservableCollection<LookupItemDto> ViTris { get; } = new();
    public ObservableCollection<LookupItemDto> LoaiTaiSans { get; } = new();
    public ObservableCollection<LookupItemDto> DanhMucs { get; } = new();
    public ObservableCollection<string> HangSanXuats { get; } = new();
    public ObservableCollection<TrangThaiOption> TrangThais { get; } = new();
    public ObservableCollection<TongHopTaiSanRowDto> KetQua { get; } = new();

    public IReadOnlyList<NhomTheoOption> NhomTheos { get; } = new List<NhomTheoOption>
    {
        new(NhomTheoTongHop.PhongBan, "Phòng ban"),
        new(NhomTheoTongHop.ViTri, "Vị trí"),
        new(NhomTheoTongHop.LoaiTaiSan, "Loại tài sản"),
        new(NhomTheoTongHop.DanhMuc, "Danh mục tài sản"),
        new(NhomTheoTongHop.HangSanXuat, "Hãng sản xuất"),
        new(NhomTheoTongHop.TrangThai, "Trạng thái"),
    };

    // Id = 0 nghĩa là "Tất cả"
    [ObservableProperty] private long _phongBanId;
    [ObservableProperty] private long _viTriId;
    [ObservableProperty] private long _loaiTaiSanId;
    [ObservableProperty] private long _danhMucId;
    [ObservableProperty] private string _hangSanXuatChon = TatCa;
    [ObservableProperty] private TrangThaiOption? _trangThaiChon;
    [ObservableProperty] private NhomTheoOption? _nhomTheoChon;

    // Tiêu đề cột nhóm của kết quả đang hiển thị (chốt lúc bấm "Xem báo cáo")
    [ObservableProperty] private string _tenCotNhom = "Nhóm";
    [ObservableProperty] private int _tongSoLuong;
    [ObservableProperty] private decimal _tongNguyenGia;
    [ObservableProperty] private bool _isBusy;

    public TongHopTaiSanViewModel(IBaoCaoTongHopTaiSanService service)
    {
        _service = service;
        NhomTheoChon = NhomTheos[0];
        _=InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        var lookup = await _service.GetLookupAsync();

        Fill(PhongBans, lookup.PhongBans);
        Fill(LoaiTaiSans, lookup.LoaiTaiSans);
        Fill(DanhMucs, lookup.DanhMucs);
        _allViTri = lookup.ViTris;
        RebuildViTris();

        HangSanXuats.Clear();
        HangSanXuats.Add(TatCa);
        foreach (var h in lookup.HangSanXuats) HangSanXuats.Add(h);

        TrangThais.Clear();
        TrangThais.Add(new TrangThaiOption(null, TatCa));
        foreach (var t in Enum.GetValues<TrangThaiTaiSan>())
            TrangThais.Add(new TrangThaiOption(t, t.ToString())); // dùng thẳng enum, giống báo cáo Danh Sách Tài Sản

        // Gán selection SAU khi ItemsSource đã có dữ liệu
        TrangThaiChon = TrangThais[0];
        HangSanXuatChon = TatCa;
        // Không tự chạy báo cáo khi mở màn hình
    }

    private static void Fill(ObservableCollection<LookupItemDto> target, List<LookupItemDto> source)
    {
        target.Clear();
        target.Add(new LookupItemDto { Id = 0, Name = TatCa });
        foreach (var i in source) target.Add(i);
    }

    partial void OnPhongBanIdChanged(long value) => RebuildViTris();

    private void RebuildViTris()
    {
        ViTris.Clear();
        ViTris.Add(new LookupItemDto { Id = 0, Name = TatCa });
        foreach (var v in _allViTri.Where(x => PhongBanId == 0 || x.ParentId == PhongBanId))
            ViTris.Add(v);
        ViTriId = 0;
    }

    private TongHopTaiSanFilterDto BuildFilter() => new()
    {
        PhongBanId = PhongBanId == 0 ? null : PhongBanId,
        ViTriTaiSanId = ViTriId == 0 ? null : ViTriId,
        LoaiTaiSanId = LoaiTaiSanId == 0 ? null : LoaiTaiSanId,
        DMTaiSanId = DanhMucId == 0 ? null : DanhMucId,
        HangSanXuat = HangSanXuatChon == TatCa ? null : HangSanXuatChon,
        TrangThai = TrangThaiChon?.Value,
        NhomTheo = NhomTheoChon?.Value ?? NhomTheoTongHop.PhongBan
    };

    private string BuildMoTaBoLoc()
    {
        string Ten(ObservableCollection<LookupItemDto> src, long id) =>
            src.FirstOrDefault(x => x.Id == id)?.Name ?? TatCa;

        return $"Phòng ban: {Ten(PhongBans, PhongBanId)} | Vị trí: {Ten(ViTris, ViTriId)} | " +
               $"Loại tài sản: {Ten(LoaiTaiSans, LoaiTaiSanId)} | Danh mục: {Ten(DanhMucs, DanhMucId)} | " +
               $"Hãng SX: {HangSanXuatChon} | Trạng thái: {TrangThaiChon?.Text ?? TatCa} | " +
               $"Nhóm theo: {NhomTheoChon?.Text}";
    }

    [RelayCommand]
    private async Task XemBaoCaoAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var data = await _service.GetAsync(BuildFilter());

            KetQua.Clear();
            foreach (var r in data) KetQua.Add(r);

            TenCotNhom = NhomTheoChon?.Text ?? "Nhóm";
            TongSoLuong = data.Sum(x => x.SoLuong);
            TongNguyenGia = data.Sum(x => x.TongNguyenGia);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: dùng cơ chế báo lỗi chung của dự án nếu có
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task XuatExcelAsync()
    {
        if (IsBusy) return;

        var dlg = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"TongHopTaiSan_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };
        if (dlg.ShowDialog() != true) return;

        try
        {
            IsBusy = true;
            await _service.ExportExcelAsync(BuildFilter(), NhomTheoChon?.Text ?? "Nhóm", BuildMoTaBoLoc(), dlg.FileName);
            MessageBox.Show("Xuất Excel thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { IsBusy = false; }
    }
}