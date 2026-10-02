// ===== ITAM.AppCore/Interfaces/IDashboardService.cs =====
namespace ITAM.AppCore.Interfaces;

using ITAM.AppCore.DTOs;

public interface IDashboardService
{
    Task<DashboardDto> GetTongQuanAsync();
    Task<XuHuongKyDto> GetXuHuongKyAsync(DateTime tuNgay, DateTime denNgay);
    Task<ThongKeThangDto> GetThongKeThangAsync(int nam);
}