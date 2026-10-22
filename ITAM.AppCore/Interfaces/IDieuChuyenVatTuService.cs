using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
namespace ITAM.AppCore.Interfaces;
public interface IDieuChuyenVatTuService
{
    /// <summary>
    /// Lưu nháp phiếu (thêm mới nếu dto.Id null, ngược lại cập nhật phiếu PENDING). Trả về Id phiếu để reselect.
    /// Ném InvalidBusinessRuleException khi vi phạm quy tắc: kho bị chặn, vị trí không thuộc phòng ban,
    /// số lượng vượt tồn khả dụng, trùng cặp (Hàng hóa, Vị trí đi) trong phiếu...
    /// </summary>
    Task<long> LuuPhieuAsync(SaveDieuChuyenVatTuDto dto);

    /// <summary>
    /// Duyệt phiếu PENDING: kiểm tra lại tồn thực tế, trừ tồn nguồn, cộng/tạo tồn đích, ghi LichSuBienDongVatTu.
    /// Ném InvalidBusinessRuleException nếu phiếu không còn PENDING hoặc tồn không còn đủ.
    /// </summary>
    Task DuyetPhieuAsync(long dieuChuyenVatTuId, long nguoiDuyetId);

    /// <summary>Từ chối phiếu PENDING, không đổi tồn kho.</summary>
    Task TuChoiPhieuAsync(long dieuChuyenVatTuId, long nguoiDuyetId);

    /// <summary>Xóa phiếu, chỉ khi còn PENDING.</summary>
    Task DeleteAsync(long dieuChuyenVatTuId);

    Task<DieuChuyenVatTu?> GetByIdAsync(long dieuChuyenVatTuId);

    Task<List<DieuChuyenVatTu>> TimKiemAsync(DieuChuyenVatTuSearchDto dto);

    /// <summary>
    /// Các cặp (Hàng hóa, Vị trí) thuộc phòng ban còn tồn khả dụng (đã trừ số lượng các phiếu PENDING khác).
    /// boQuaPhieuId: phiếu đang sửa, không tính phần phiếu này đang giữ.
    /// </summary>
    Task<List<VatTuKhaDungDto>> GetVatTuKhaDungAsync(long phongBanId, long? boQuaPhieuId = null);
}