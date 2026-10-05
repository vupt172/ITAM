using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Domain.Enums;

namespace ITAM.Domain.Entities
{
    public class PhieuThatLac:AuditableEntity
    {
        public long Id { get; set; }

        /// <summary>Format TL-{năm}-{stt:D4}, reset theo năm.</summary>
        public string SoPhieu { get; set; } = null!;

        /// <summary>Phòng Ban của phiếu, cố định = Phòng Ban mặc định của người tạo.</summary>
        public long PhongBanId { get; set; }
        public PhongBan PhongBan { get; set; } = null!;

        public DateTime NgayPhatHien { get; set; }

        /// <summary>Người phát hiện thất lạc (text tự do, có thể không phải user hệ thống).</summary>
        public string? NguoiPhatHien { get; set; }

        /// <summary>Lý do / mô tả sự việc.</summary>
        public string LyDo { get; set; } = null!;

        public long NguoiTaoId { get; set; }
        public User NguoiTao { get; set; } = null!;
        public DateTime NgayTao { get; set; }

        public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.PENDING;

        public long? NguoiDuyetId { get; set; }
        public User? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public ICollection<PhieuThatLacChiTiet> ChiTiets { get; set; } = new List<PhieuThatLacChiTiet>();
    }

 
}