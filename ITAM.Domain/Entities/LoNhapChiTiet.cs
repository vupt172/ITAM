using System.Collections.Generic;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Chi tiết 1 dòng trong Lô Nhập. Khi duyệt Lô Nhập, xử lý theo danh mục của Hàng Hóa:
    /// - true  → sinh SoLuongNhap bản ghi TaiSanDinhDanh (mỗi bản ghi trỏ LoNhapChiTietId = dòng này,
    ///           GiaNhap = DonGia, LoaiTaiSanId tự tính theo ngưỡng MinValue/MaxValue, ViTriTaiSanId = "Kho nhập").
    /// - false → cộng dồn SoLuongNhap vào SoLuongTon của bản ghi VatTu tương ứng HangHoaId.
    /// </summary>
    public class LoNhapChiTiet
    {
        public long Id { get; set; }

        /// <summary>
        /// Mã hiển thị riêng của dòng chi tiết (VD: "{LoNhap.SoLo}-01") — dùng để hiển thị trên
        /// TaiSanDinhDanh (tài sản này nhập theo dòng nào), tránh lộ Id (long) vô nghĩa với người dùng.
        /// Khác với LoNhap.SoLo (mã của cả phiếu nhập).
        /// </summary>
        public string SoLo { get; set; } = string.Empty;

        public long LoNhapId { get; set; }
        public LoNhap LoNhap { get; set; } = null!;

        /// <summary>Mã hàng hóa được chọn trên phiếu nhập.</summary>
        public long HangHoaId { get; set; }
        public HangHoa HangHoa { get; set; } = null!;
        /// <summary>Tên hàng hóa tại thời điểm lập phiếu, phục vụ lịch sử chứng từ.</summary>
        public string TenVatPham { get; set; } = string.Empty;
        public int SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }

        /// <summary>
        /// Người dùng chọn khi nhập dòng chi tiết (combobox filter theo DonGia gợi ý, không tự suy luận).
        /// Bắt buộc khi DMTaiSan.IsTrackedById = true, bỏ qua (null) nếu là Vật Tư.
        /// </summary>
        public long? LoaiTaiSanId { get; set; }
        public LoaiTaiSan? LoaiTaiSan { get; set; }

        /// <summary>Tính toán ở tầng ứng dụng, không lưu DB — tránh lệch dữ liệu với DonGia/SoLuongNhap.</summary>
        public decimal ThanhTien => SoLuongNhap * DonGia;

        /// <summary>Chỉ có dữ liệu khi DMTaiSan.IsTrackedById = true.</summary>
        public ICollection<TaiSanDinhDanh> TaiSanDinhDanhs { get; set; } = new List<TaiSanDinhDanh>();
    }
}
