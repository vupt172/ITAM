using ITAM.Domain.Enums;

namespace ITAM.AppCore.Interfaces
{
    /// <summary>
    /// Kiểm tra tính hợp lệ của việc chuyển trạng thái tài sản (TaiSanDinhDanh)
    /// theo state machine: IN_STOCK / ASSIGNED / MAINTENANCE / LOST / DISPOSAL_PENDING / DISPOSED.
    /// </summary>
    public interface IAssetStatusService
    {
        /// <summary>Trả về true nếu chuyển từ <paramref name="from"/> sang <paramref name="to"/> là hợp lệ.</summary>
        bool CanTransition(TrangThaiTaiSan from, TrangThaiTaiSan to);

        /// <summary>
        /// Ném <see cref="InvalidOperationException"/> nếu chuyển trạng thái không hợp lệ.
        /// Dùng ở đầu các service method (DieuChuyen, PhieuSuaChua, LOST, DISPOSAL...) trước khi cập nhật TrangThai.
        /// </summary>
        void ValidateTransition(TrangThaiTaiSan from, TrangThaiTaiSan to);

        /// <summary>Danh sách trạng thái đích hợp lệ kế tiếp từ một trạng thái, dùng để filter UI (ví dụ ComboBox chọn hành động).</summary>
        IReadOnlyCollection<TrangThaiTaiSan> GetAllowedNextStates(TrangThaiTaiSan from);
    }
}