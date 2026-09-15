using System.Collections.Generic;
using System.Threading.Tasks;
using ITAM.AppCore.DTOs;
using ITAM.Domain.Entities;

namespace ITAM.AppCore.Interfaces
{
    public interface ILoNhapService
    {

        /// <summary>
        /// Duyệt phiếu Lô Nhập: sinh TaiSanDinhDanh cho các dòng IsTrackedById = true (Serial để trống,
        /// sẽ nhập ở bước Điều chuyển khi bàn giao cho phòng ban — Lô Nhập chỉ tính theo số lượng),
        /// cộng dồn SoLuongTon vào VatTu cho các dòng còn lại. Chuyển TrangThai sang APPROVED.
        /// </summary>
        /// <summary>Duyệt phiếu Lô Nhập, gán NguoiDuyetId = người đang thao tác — sinh tài sản/vật tư như cũ.</summary>
        Task DuyetLoNhapAsync(long loNhapId, long nguoiDuyetId);

        /// <summary>Từ chối phiếu, gán NguoiDuyetId = người đang thao tác.</summary>
        Task TuChoiLoNhapAsync(long loNhapId, long nguoiDuyetId);

        /// <summary>Tìm kiếm phiếu Lô Nhập theo số phiếu / khoảng ngày nhập / trạng thái (query DB, AsNoTracking).</summary>
        Task<List<LoNhap>> TimKiemAsync(LoNhapSearchDto criteria);
        Task<LoNhap?> GetByIdAsync(long loNhapId);
        Task<List<LoNhap>> GetAllAsync();
        /// <summary>
        /// Lưu toàn bộ Phiếu Nhập (header + chi tiết) trong 1 
        /// — tạo mới nếu Id=null,
        /// cập nhật nếu có Id (chỉ khi phiếu còn PENDING). Đồng bộ chi tiết: thêm dòng mới (Id=null),
        /// cập nhật dòng đã có (Id khác null), xóa các dòng trong ChiTietIdsXoa.
        /// Validate lại toàn bộ ở server (không tin tưởng validate phía client). Trả về Id phiếu.
        /// </summary>
        Task<long> LuuPhieuNhapAsync(SaveLoNhapDto dto);
        /// <summary>Xóa cứng phiếu Lô Nhập — chỉ cho phép khi TrangThai = PENDING (chưa duyệt/từ chối).</summary>
        Task DeleteAsync(long loNhapId);
    }
}