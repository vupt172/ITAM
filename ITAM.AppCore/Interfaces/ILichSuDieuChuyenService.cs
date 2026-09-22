using ITAM.AppCore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITAM.AppCore.Interfaces
{
    /// <summary>
    /// Tra cứu Lịch Sử Điều Chuyển — tách riêng khỏi IDieuChuyenService vì lịch sử không thuộc về
    /// vòng đời phiếu DieuChuyen: nó được ghi từ nhiều nguồn (Duyệt Lô Nhập, Duyệt Điều Chuyển, và sau
    /// này là Báo Mất / Thanh Lý — xem LichSuDieuChuyenTaiSan.DieuChuyenChiTietId nullable). Service này
    /// chỉ đọc, phục vụ: (1) tab "Lịch Sử Điều Chuyển" trên màn hình chi tiết TaiSanDinhDanh,
    /// (2) tìm kiếm/tổng hợp lịch sử (theo khoảng ngày, phòng ban, người duyệt...) cho báo cáo sau này.
    /// </summary>
    public interface ILichSuDieuChuyenService
    {
        /// <summary>
        /// Lịch sử điều chuyển của 1 tài sản, mới nhất trước — dùng cho tab Lịch Sử Điều Chuyển.
        /// Mặc định chỉ lấy 5 dòng gần nhất (đúng nhu cầu hiển thị tab); truyền <paramref name="soLuong"/>
        /// nếu sau này cần xem đầy đủ (VD: nút "Xem tất cả" mở màn hình dùng TimKiemAsync).
        /// </summary>
        Task<List<LichSuDieuChuyenTaiSanDto>> GetByTaiSanIdAsync(long taiSanDinhDanhId, int soLuong = 5);

        /// <summary>Tìm kiếm/tổng hợp lịch sử điều chuyển theo nhiều điều kiện — phục vụ báo cáo.</summary>
        Task<List<LichSuDieuChuyenTaiSanDto>> TimKiemAsync(LichSuDieuChuyenSearchDto dto);
    }
}