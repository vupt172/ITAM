// ITAM.WPF/ViewModels/DashboardViewModel.cs
//
// Cần NuGet: LiveChartsCore.SkiaSharpView.WPF (đã thêm ở bước trước).
// Giả định chưa xác nhận (// TODO xác nhận):
//  - BaseViewModel có constructor không tham số, không có abstract member bắt buộc
//  - Cách báo lỗi chuẩn (đang dùng MessageBox tạm)
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITAM.AppCore.DTOs;
using ITAM.AppCore.Interfaces;
using ITAM.WPF.Services.Interfaces;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MaterialDesignThemes.Wpf;
using SkiaSharp;

namespace ITAM.WPF.ViewModels;

public record KpiItem(string Title, string Value, string Trend, Brush TrendBrush,
                      PackIconKind Icon, Brush Accent, Brush Tint);
public record LegendItem(string Name, string Percent, Brush Color);
public record TopTaiSanRow(int Stt, string Ma, string Ten, string DanhMuc, string PhongBan,
                           string ViTri, string GiaTri, string TrangThai, Brush TrangThaiBrush);
public record ActivityItem(string Title, string Detail, string TimeAgo, PackIconKind Icon, Brush Accent);

public partial class DashboardViewModel : BaseViewModel
{
    private static readonly string[] Palette =
        { "#2F80ED", "#27AE60", "#F2994A", "#9B51E0", "#2DCDDB", "#9AA5B1" };
    private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

    private readonly IDashboardService _service;

    // Service (và AppDbContext) được resolve 1 lần rồi dùng suốt phiên → mọi lần tải phải chạy tuần tự,
    // nếu không đổi ngày nhanh sẽ gây "second operation started on this context".
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    private DashboardDto? _dto;
    private XuHuongKyDto? _xuHuong;

    public ObservableCollection<KpiItem> Kpis { get; } = new();
    public ObservableCollection<LegendItem> LoaiLegend { get; } = new();
    public ObservableCollection<TopTaiSanRow> TopGiaTri { get; } = new();
    public ObservableCollection<ActivityItem> HoatDong { get; } = new();

    [ObservableProperty] private DateTime _tuNgay;
    [ObservableProperty] private DateTime _denNgay;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _loaiTongText = "0";

    [ObservableProperty] private ISeries[] _phongBanSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _phongBanYAxes = Array.Empty<Axis>();

    [ObservableProperty] private ISeries[] _danhMucSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _danhMucXAxes = Array.Empty<Axis>();

    [ObservableProperty] private ISeries[] _loaiSeries = Array.Empty<ISeries>();

    [ObservableProperty] private ISeries[] _nhapThangSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _nhapThangYAxes = Array.Empty<Axis>();

    [ObservableProperty] private ISeries[] _soSanhSeries = Array.Empty<ISeries>();

    [ObservableProperty] private ISeries[] _giaTriPbSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _giaTriPbYAxes = Array.Empty<Axis>();

    public Axis[] ThangXAxes { get; }

    public DashboardViewModel(IDashboardService service, INavigationService navigationService,ICurrentUserContext currentUserContext) : base(navigationService,currentUserContext)
    {
        _service = service;

        var now = DateTime.Today;
        _tuNgay = new DateTime(now.Year, now.Month, 1);   // gán field để không kích hoạt hook trước InitializeAsync
        _denNgay = now;

        ThangXAxes = new[] { CreateLabelAxis(Enumerable.Range(1, 12).Select(m => $"T{m}").ToArray()) };
    }

    protected override void InitToolbarState()
    {
        CanClose = true;
    }
    protected override bool CanAdd() => false;   // Dashboard không có nút Thêm
    public async Task InitializeAsync()
    {
        await RunExclusiveAsync(async () =>
        {
            _dto = await _service.GetTongQuanAsync();
            _xuHuong = await _service.GetXuHuongKyAsync(TuNgay, DenNgay);
            var thang = await _service.GetThongKeThangAsync(DenNgay.Year);

            ApplyTongQuan(_dto);
            ApplyThang(thang);
            RebuildKpis();
        });
    }

    [RelayCommand]
    private async Task RefreshAsync() => await InitializeAsync();

    // Đổi khoảng ngày: chỉ tải lại xu hướng KPI + 2 biểu đồ theo tháng (năm của "Đến ngày").
    // LoadKyAsync tự bắt lỗi trong RunExclusiveAsync nên fire-and-forget ở đây không nuốt exception.
    partial void OnTuNgayChanged(DateTime value) => _ = LoadKyAsync();
    partial void OnDenNgayChanged(DateTime value) => _ = LoadKyAsync();

    private async Task LoadKyAsync()
    {
        if (TuNgay > DenNgay) return;

        await RunExclusiveAsync(async () =>
        {
            _xuHuong = await _service.GetXuHuongKyAsync(TuNgay, DenNgay);
            var thang = await _service.GetThongKeThangAsync(DenNgay.Year);
            ApplyThang(thang);
            RebuildKpis();
        });
    }

    private async Task RunExclusiveAsync(Func<Task> action)
    {
        await _loadLock.WaitAsync();
        try
        {
            IsLoading = true;
            await action();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không tải được Dashboard: {ex.Message}");   // TODO xác nhận cách báo lỗi chuẩn
        }
        finally
        {
            IsLoading = false;
            _loadLock.Release();
        }
    }

    // ---------------- KPI ----------------
    private void RebuildKpis()
    {
        if (_dto is null) return;

        var (tTong, bTong) = Trend(_xuHuong?.TongTaiSanPct);
        var (tGt, bGt) = Trend(_xuHuong?.TongGiaTriPct);
        var none = Hex("#828282");

        Kpis.Clear();
        Kpis.Add(new("Tổng tài sản", _dto.TongTaiSan.ToString("N0", Vi), tTong, bTong, PackIconKind.Monitor, Hex("#2F80ED"), Hex("#EAF2FE")));
        Kpis.Add(new("Đang sử dụng", _dto.DangSuDung.ToString("N0", Vi), "", none, PackIconKind.CheckCircleOutline, Hex("#27AE60"), Hex("#E8F7EE")));
        Kpis.Add(new("Trong kho", _dto.TrongKho.ToString("N0", Vi), "", none, PackIconKind.PackageVariantClosed, Hex("#F2994A"), Hex("#FEF3E7")));
        Kpis.Add(new("Kho thất lạc", _dto.ThatLac.ToString("N0", Vi), "", none, PackIconKind.AlertOutline, Hex("#EB5757"), Hex("#FDECEC")));
        Kpis.Add(new("Chờ thanh lý", _dto.ChoThanhLy.ToString("N0", Vi), "", none, PackIconKind.TrashCanOutline, Hex("#9B51E0"), Hex("#F3EAFC")));
        Kpis.Add(new("Tổng giá trị tài sản", _dto.TongGiaTri.ToString("N0", Vi) + " ₫", tGt, bGt, PackIconKind.Database, Hex("#2F80ED"), Hex("#EAF2FE")));
    }

    private static (string Text, Brush Brush) Trend(double? pct)
    {
        if (pct is null) return ("", Hex("#828282"));
        var p = pct.Value;
        var arrow = p > 0 ? "↑" : p < 0 ? "↓" : "→";
        var color = p > 0 ? "#27AE60" : p < 0 ? "#EB5757" : "#828282";
        return ($"{arrow} {Math.Abs(p):0.#}% so với đầu kỳ", Hex(color));
    }

    // ---------------- Biểu đồ + bảng + feed ----------------
    private void ApplyTongQuan(DashboardDto dto)
    {
        (PhongBanSeries, PhongBanYAxes) = BuildBar(dto.TheoPhongBan, "Số lượng", "#2F80ED");
        (GiaTriPbSeries, GiaTriPbYAxes) = BuildBar(dto.GiaTriTheoPhongBan, "Giá trị (triệu đồng)", "#27AE60");

        DanhMucSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Số lượng",
                Values = dto.TheoDanhMuc.Select(x => x.Value).ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#2F80ED")),
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#555555")),
                DataLabelsSize = 11,
                DataLabelsPosition = DataLabelsPosition.End,
                MaxBarWidth = 40
            }
        };
        DanhMucXAxes = new[] { CreateLabelAxis(dto.TheoDanhMuc.Select(x => x.Label).ToArray()) };

        // Doughnut + legend %
        LoaiSeries = dto.TheoLoai
            .Select((x, i) => (ISeries)new PieSeries<double>
            {
                Name = x.Label,
                Values = new[] { x.Value },
                InnerRadius = 70,
                Fill = new SolidColorPaint(SKColor.Parse(Palette[i % Palette.Length]))
            })
            .ToArray();

        var tong = dto.TheoLoai.Sum(x => x.Value);
        LoaiTongText = tong.ToString("N0", Vi);
        LoaiLegend.Clear();
        for (var i = 0; i < dto.TheoLoai.Count; i++)
        {
            var x = dto.TheoLoai[i];
            var pct = tong > 0 ? x.Value / tong * 100 : 0;
            LoaiLegend.Add(new(x.Label, $"{pct:0.#}%", Hex(Palette[i % Palette.Length])));
        }

        // Top giá trị
        TopGiaTri.Clear();
        var stt = 1;
        foreach (var t in dto.TopGiaTri)
        {
            var (text, color) = TrangThai(t.TrangThai);
            TopGiaTri.Add(new(stt++, t.Ma, t.Ten, t.DanhMuc, t.PhongBan, t.ViTri,
                              t.GiaTri.ToString("N0", Vi), text, Hex(color)));
        }

        // Hoạt động gần đây
        HoatDong.Clear();
        foreach (var h in dto.HoatDongGanDay)
        {
            var (icon, color) = h.Loai switch
            {
                HoatDongLoai.Nhap => (PackIconKind.Plus, "#27AE60"),
                HoatDongLoai.DieuChuyen => (PackIconKind.SwapHorizontal, "#2F80ED"),
                HoatDongLoai.ThatLac => (PackIconKind.BellAlertOutline, "#EB5757"),
                HoatDongLoai.ThanhLy => (PackIconKind.TrashCanOutline, "#9B51E0"),
                _ => (PackIconKind.InformationOutline, "#F2994A")
            };
            HoatDong.Add(new(h.TieuDe, h.ChiTiet, TimeAgo(h.ThoiGian), icon, Hex(color)));
        }
    }

    private void ApplyThang(ThongKeThangDto t)
    {
        // Nhập theo tháng: cột = số lượng (trục trái), đường = tổng giá trị triệu đồng (trục phải)
        NhapThangSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Số lượng nhập",
                Values = t.Nhap,
                Fill = new SolidColorPaint(SKColor.Parse("#2F80ED")),
                MaxBarWidth = 24
            },
            new LineSeries<double>
            {
                Name = "Tổng giá trị (triệu đồng)",
                Values = t.GiaTriNhapTrieu,
                ScalesYAt = 1,
                Fill = null,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(SKColor.Parse("#27AE60")) { StrokeThickness = 2 },
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#27AE60")) { StrokeThickness = 2 }
            }
        };
        NhapThangYAxes = new[]
        {
            new Axis { MinLimit = 0 },
            new Axis { MinLimit = 0, Position = AxisPosition.End, ShowSeparatorLines = false }
        };

        // So sánh nhập - điều chuyển - thanh lý
        SoSanhSeries = new ISeries[]
        {
            new ColumnSeries<int> { Name = "Nhập kho", Values = t.Nhap, Fill = new SolidColorPaint(SKColor.Parse("#2F80ED")), MaxBarWidth = 12 },
            new ColumnSeries<int> { Name = "Điều chuyển", Values = t.DieuChuyen, Fill = new SolidColorPaint(SKColor.Parse("#27AE60")), MaxBarWidth = 12 },
            new ColumnSeries<int> { Name = "Thanh lý", Values = t.ThanhLy, Fill = new SolidColorPaint(SKColor.Parse("#F2994A")), MaxBarWidth = 12 }
        };
    }

    // ---------------- Helper ----------------
    private static (ISeries[] Series, Axis[] Axes) BuildBar(IEnumerable<ChartPointDto> pts, string name, string color)
    {
        // RowSeries vẽ phần tử đầu ở dưới cùng → đảo ngược để giá trị lớn nhất nằm trên cùng
        var r = pts.Reverse().ToList();
        var series = new ISeries[]
        {
            new RowSeries<double>
            {
                Name = name,
                Values = r.Select(x => x.Value).ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse(color)),
                DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#555555")),
                DataLabelsSize = 11,
                DataLabelsPosition = DataLabelsPosition.End,
                MaxBarWidth = 16
            }
        };
        return (series, new[] { CreateLabelAxis(r.Select(x => x.Label).ToArray()) });
    }

    // Không dùng Axis.Labels (tùy phiên bản LiveCharts2) mà dùng Labeler: trục nhận index 0,1,2... và trả về tên.
    private static Axis CreateLabelAxis(IReadOnlyList<string> labels) => new()
    {
        Labeler = v =>
        {
            var i = (int)Math.Round(v);
            return i >= 0 && i < labels.Count ? labels[i] : "";
        },
        MinStep = 1,
        ForceStepToMin = true
    };

    private static (string Text, string Color) TrangThai(string t) => t switch
    {
        "ASSIGNED" => ("Đang sử dụng", "#27AE60"),
        "IN_STOCK" => ("Trong kho", "#F2994A"),
        "MAINTENANCE" => ("Sửa chữa", "#2F80ED"),
        "LOST" => ("Thất lạc", "#EB5757"),
        "DISPOSAL_PENDING" => ("Chờ thanh lý", "#9B51E0"),
        "DISPOSED" => ("Đã thanh lý", "#828282"),
        _ => (t, "#828282")
    };

    private static string TimeAgo(DateTime time)
    {
        var d = DateTime.Now - time;
        if (d.TotalMinutes < 1) return "vừa xong";
        if (d.TotalMinutes < 60) return $"{(int)d.TotalMinutes} phút trước";
        if (d.TotalHours < 24) return $"{(int)d.TotalHours} giờ trước";
        return $"{(int)d.TotalDays} ngày trước";
    }

    private static Brush Hex(string hex) => (Brush)new BrushConverter().ConvertFromString(hex)!;
}