using System.Collections.Generic;
using System.Threading.Tasks;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;

namespace ITAM.AppCore.Interfaces
{
    public interface ILoNhapService
    {
        /// <summary>Tạo phiếu Lô Nhập ở trạng thái Nháp, chưa có dòng chi tiết. Trả về Id phiếu.</summary>
        Task<long> TaoPhieuNhapAsync(CreateLoNhapDto dto);

        /// <summary>Thêm 1 dòng chi tiết vào phiếu đang ở trạng thái Nháp. Trả về Id dòng chi tiết.</summary>
        Task<long> ThemChiTietAsync(long loNhapId, CreateLoNhapChiTietDto dto);

        /// <summary>Xóa 1 dòng chi tiết — chỉ cho phép khi phiếu còn ở trạng thái Nháp.</summary>
        Task XoaChiTietAsync(long chiTietId);

        /// <summary>
        /// Duyệt phiếu Lô Nhập: sinh TaiSanDinhDanh cho các dòng IsTrackedById = true (Serial để trống,
        /// sẽ nhập ở bước Điều chuyển khi bàn giao cho phòng ban — Lô Nhập chỉ tính theo số lượng),
        /// cộng dồn SoLuongTon vào VatTu cho các dòng còn lại. Chuyển TrangThai sang APPROVED.
        /// </summary>
        Task DuyetLoNhapAsync(long loNhapId);

        /// <summary>Từ chối phiếu — chỉ cho phép khi còn ở trạng thái PENDING (chưa sinh tài sản/vật tư).</summary>
        Task TuChoiLoNhapAsync(long loNhapId);

        /// <summary>Tìm kiếm phiếu Lô Nhập theo số phiếu / khoảng ngày nhập / trạng thái (query DB, AsNoTracking).</summary>
        Task<List<LoNhap>> TimKiemAsync(LoNhapSearchDto criteria);
        Task<LoNhap?> GetByIdAsync(long loNhapId);
        Task<List<LoNhap>> GetAllAsync();
    }
}