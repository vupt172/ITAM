using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Tài sản quản lý theo định danh riêng lẻ — mỗi dòng là 1 tài sản cụ thể (số lượng luôn = 1).
    /// </summary>
    public class TaiSanDinhDanh: AuditableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public long HangHoaId { get; set; }
        public HangHoa HangHoa { get; set; } = null!;

        /// <summary>Bắt buộc nhập nếu DMTaiSan.RequireSerial = true.</summary>
        public string? Serial { get; set; }

        public int? NamSuDung { get; set; }
        public decimal GiaNhap { get; set; }

        /// <summary>
        /// Phân loại TSCĐ/CCDC — snapshot tại thời điểm nhập, tự tính theo GiaNhap
        /// đối chiếu ngưỡng của PhanLoaiTaiSan lúc duyệt Lô Nhập.
        /// </summary>
        public long LoaiTaiSanId { get; set; }
        public LoaiTaiSan LoaiTaiSan { get; set; } = null!;

        /// <summary>Lưu DB dạng string (xem TaiSanDinhDanhConfiguration).</summary>
        public TrangThaiTaiSan TrangThaiTaiSan { get; set; } = TrangThaiTaiSan.IN_STOCK;

        /// <summary>Dòng chi tiết Lô Nhập đã sinh ra bản ghi này.</summary>
        public long LoNhapChiTietId { get; set; }
        public LoNhapChiTiet LoNhapChiTiet { get; set; } = null!;

        /// <summary>
        /// Vị trí hiện tại của tài sản. Khi nhập kho mặc định trỏ tới ViTriTaiSan "Kho nhập"
        /// (thuộc PhongBan "Kho Hệ thống"); Điều chuyển sẽ cập nhật lại giá trị này.
        /// </summary>
        public long ViTriTaiSanId { get; set; }
        public ViTriTaiSan ViTriTaiSan { get; set; } = null!;

        public string? GhiChu { get; set; }

        }
}
