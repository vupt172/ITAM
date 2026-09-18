using ITAM.AppCore.Interfaces;
using ITAM.Domain.Enums;

namespace ITAM.Infrastructure.Services
{
    public class AssetStatusService : IAssetStatusService
    {
        // Sơ đồ chuyển trạng thái hợp lệ — khớp state machine đã xác nhận:
        // IN_STOCK  <-> ASSIGNED            (điều chuyển)
        // IN_STOCK  -> MAINTENANCE          (mang đi sửa chữa)
        // ASSIGNED  -> MAINTENANCE          (mang đi sửa chữa)
        // MAINTENANCE -> IN_STOCK/ASSIGNED  (hoàn thành sửa chữa, quay về trạng thái trước đó —
        //                                    trạng thái cụ thể do PhieuSuaChuaChiTiet.TrangThaiTruocBaoTri quyết định,
        //                                    service này chỉ xác nhận việc quay lại 1 trong 2 trạng thái đó là hợp lệ)
        // IN_STOCK  -> LOST                 (thất lạc, không khôi phục)
        // ASSIGNED  -> LOST                 (thất lạc, không khôi phục)
        // IN_STOCK  -> DISPOSAL_PENDING     (chờ thanh lý — asset ASSIGNED hỏng không sửa được
        //                                    phải chuyển về IN_STOCK trước, không có đường thẳng ASSIGNED -> DISPOSAL_PENDING)
        // DISPOSAL_PENDING -> DISPOSED      (thanh lý xong)
        // LOST, DISPOSED: trạng thái cuối, không có transition đi ra
        private static readonly Dictionary<TrangThaiTaiSan, TrangThaiTaiSan[]> AllowedTransitions = new()
        {
            [TrangThaiTaiSan.IN_STOCK] = new[]
            {
                TrangThaiTaiSan.ASSIGNED,
                TrangThaiTaiSan.MAINTENANCE,
                TrangThaiTaiSan.LOST,
                TrangThaiTaiSan.DISPOSAL_PENDING
            },
            [TrangThaiTaiSan.ASSIGNED] = new[]
            {
                TrangThaiTaiSan.IN_STOCK,
                TrangThaiTaiSan.MAINTENANCE,
                TrangThaiTaiSan.LOST
            },
            [TrangThaiTaiSan.MAINTENANCE] = new[]
            {
                TrangThaiTaiSan.IN_STOCK,
                TrangThaiTaiSan.ASSIGNED
            },
            [TrangThaiTaiSan.LOST] = Array.Empty<TrangThaiTaiSan>(),
            [TrangThaiTaiSan.DISPOSAL_PENDING] = new[]
            {
                TrangThaiTaiSan.DISPOSED
            },
            [TrangThaiTaiSan.DISPOSED] = Array.Empty<TrangThaiTaiSan>()
        };

        public bool CanTransition(TrangThaiTaiSan from, TrangThaiTaiSan to)
        {
            return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
        }

        public void ValidateTransition(TrangThaiTaiSan from, TrangThaiTaiSan to)
        {
            if (!CanTransition(from, to))
            {
                throw new InvalidOperationException(
                    $"Không thể chuyển trạng thái tài sản từ '{from}' sang '{to}'.");
            }
        }

        public IReadOnlyCollection<TrangThaiTaiSan> GetAllowedNextStates(TrangThaiTaiSan from)
        {
            return AllowedTransitions.TryGetValue(from, out var allowed)
                ? allowed
                : Array.Empty<TrangThaiTaiSan>();
        }
    }
}