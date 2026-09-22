using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    public interface IDieuChuyenService
    {
        /// <summary>Tạo mới (dto.Id null) hoặc cập nhật (dto.Id có giá trị) 1 phiếu điều chuyển đang PENDING. Trả về Id phiếu.</summary>
        Task<long> LuuPhieuAsync(SaveDieuChuyenDto dto);

        /// <summary>Duyệt phiếu: cập nhật ViTriTaiSan + TrangThaiTaiSan từng dòng chi tiết và ghi LichSuDieuChuyenTaiSan.</summary>
        Task DuyetPhieuAsync(long dieuChuyenId, long nguoiDuyetId);

        /// <summary>Từ chối phiếu — không thay đổi gì trên tài sản.</summary>
        Task TuChoiPhieuAsync(long dieuChuyenId, long nguoiDuyetId);

        /// <summary>Chỉ cho phép xóa phiếu đang PENDING.</summary>
        Task DeleteAsync(long dieuChuyenId);

        Task<DieuChuyen?> GetByIdAsync(long dieuChuyenId);

        Task<List<DieuChuyen>> TimKiemAsync(DieuChuyenSearchDto dto);
    }
}