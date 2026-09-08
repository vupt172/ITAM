using System.Collections.Generic;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Chi tiết 1 dòng trong Lô Nhập. Khi duyệt Lô Nhập, xử lý ở tầng Service theo DMTaiSan.IsTrackedById:
    /// - true  → sinh SoLuongNhap bản ghi TaiSanDinhDanh (mỗi bản ghi trỏ LoNhapChiTietId = dòng này,
    ///           GiaNhap = DonGia, PhanLoaiTaiSanId tự tính theo ngưỡng, ViTriTaiSanId = "Kho nhập").
    /// - false → cộng dồn SoLuongNhap vào SoLuongTon của bản ghi VatTu tương ứng DMTaiSanId.
    /// </summary>
    public class LoNhapChiTiet
    {
        public long Id { get; set; }

        public long LoNhapId { get; set; }
        public LoNhap LoNhap { get; set; } = null!;

        public long DMTaiSanId { get; set; }
        public DMTaiSan DMTaiSan { get; set; } = null!;

        public decimal SoLuongNhap { get; set; }
        public decimal DonGia { get; set; }

        /// <summary>Tính toán ở tầng ứng dụng, không lưu DB — tránh lệch dữ liệu với DonGia/SoLuongNhap.</summary>
        public decimal ThanhTien => SoLuongNhap * DonGia;

        /// <summary>Chỉ có dữ liệu khi DMTaiSan.IsTrackedById = true.</summary>
        public ICollection<TaiSanDinhDanh> TaiSanDinhDanhs { get; set; } = new List<TaiSanDinhDanh>();
    }
}