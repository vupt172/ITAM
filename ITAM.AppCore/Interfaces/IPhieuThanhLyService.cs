using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;

namespace ITAM.AppCore.Interfaces
{
    public interface IPhieuThanhLyService
    {
        Task<long> LuuPhieuAsync(SavePhieuThanhLyDto dto);
        Task DuyetPhieuAsync(long phieuThanhLyId, long nguoiDuyetId);
        Task TuChoiPhieuAsync(long phieuThanhLyId, long nguoiDuyetId);
        Task DeleteAsync(long phieuThanhLyId);
        Task<PhieuThanhLy?> GetByIdAsync(long phieuThanhLyId);
        Task<List<PhieuThanhLy>> TimKiemAsync(PhieuThanhLySearchDto dto);
    }
}