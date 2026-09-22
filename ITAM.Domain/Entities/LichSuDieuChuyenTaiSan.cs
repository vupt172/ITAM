using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using System;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Lịch sử đầy đủ các lần thay đổi Vị Trí Tài Sản thực tế của 1 TaiSanDinhDanh. Chỉ ghi các sự kiện
    /// làm đổi ViTriTaiSan thật sự (không ghi khi vào MAINTENANCE — sửa chữa có "Lịch Sử Sửa Chữa" riêng
    /// qua PhieuSuaChua/PhieuSuaChuaChiTiet).
    ///
    /// Sinh ra khi:
    ///  (1) Duyệt Lô Nhập — mỗi TaiSanDinhDanh mới tạo có 1 dòng: ViTriChuyenDi = null,
    ///      ViTriChuyenDen = Kho Lưu Trữ, DieuChuyenChiTietId = null.
    ///  (2) Duyệt Phiếu Điều Chuyển — 1 dòng cho mỗi DieuChuyenChiTiet đã duyệt.
    /// </summary>
    public class LichSuDieuChuyenTaiSan
    {
        public long Id { get; set; }

        public long TaiSanDinhDanhId { get; set; }
        public TaiSanDinhDanh TaiSanDinhDanh { get; set; } = null!;

        /// <summary>Null khi bản ghi sinh ra từ Duyệt Lô Nhập (tài sản mới tạo, chưa từng có vị trí trước đó).</summary>
        public long? ViTriChuyenDiId { get; set; }
        public ViTriTaiSan? ViTriChuyenDi { get; set; }

        public long ViTriChuyenDenId { get; set; }
        public ViTriTaiSan ViTriChuyenDen { get; set; } = null!;

        /// <summary>Thời điểm duyệt (Duyệt Lô Nhập hoặc Duyệt Phiếu Điều Chuyển) — đây là thời điểm tài sản thật sự đổi vị trí.</summary>
        public DateTime NgayDuyet { get; set; }

        public long NguoiDuyetId { get; set; }
        public User NguoiDuyet { get; set; } = null!;

        /// <summary>Null khi sinh từ Duyệt Lô Nhập. Khi sinh từ Điều Chuyển, trỏ tới dòng chi tiết tương ứng.</summary>
        public long? DieuChuyenChiTietId { get; set; }
        public DieuChuyenChiTiet? DieuChuyenChiTiet { get; set; }

        /// <summary>Copy từ DieuChuyenChiTiet.GhiChu tại thời điểm duyệt. Null khi sinh từ Duyệt Lô Nhập.</summary>
        public string? GhiChu { get; set; }
    }
}