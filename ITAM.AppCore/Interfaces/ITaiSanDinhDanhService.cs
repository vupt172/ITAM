using System.Collections.Generic;
using System.Threading.Tasks;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;
using ITAM.Domain.Enums;

namespace ITAM.AppCore.Interfaces
{
    /// <summary>
    /// Truy vấn Tài Sản Định Danh (TSCĐ/CCDC) — nền tảng phục vụ các chức năng sau này
    /// như Điều chuyển phòng ban, thanh lý... Hiện tại chỉ có các method query cơ bản.
    /// </summary>
    public interface ITaiSanDinhDanhService
    {
        Task<List<TaiSanDinhDanh>> GetAllAsync();
        Task<List<TaiSanDinhDanh>> GetAllAsync(long phongBanId);
        Task<TaiSanDinhDanh?> GetByCodeAsync(string code);
        Task<TaiSanDinhDanh?> GetByIdAsync(long id);

        /// <summary>Danh sách tài sản đang ở 1 vị trí (kho/phòng ban) cụ thể.</summary>
        Task<List<TaiSanDinhDanh>> GetByViTriAsync(long viTriTaiSanId);

        /// <summary>Danh sách tài sản theo trạng thái (IN_STOCK, DANG_SU_DUNG...).</summary>
        Task<List<TaiSanDinhDanh>> GetByTrangThaiAsync(TrangThaiTaiSan trangThai);
        /// <summary>Sửa Name/Serial — KHÔNG cho đổi ViTriTaiSanId ở đây, phải qua chức năng Điều chuyển.</summary>
        Task UpdateAsync(UpdateTaiSanDinhDanhDto dto);
    }
}