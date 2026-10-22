using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Domain.Enums;

namespace ITAM.Domain.Entities
{
    public class PhieuThanhLy : AuditableEntity
    {
        public long Id { get; set; }

        /// <summary>Format THL-{năm}-{stt:D4}, reset theo năm (TL- là tiền tố của phiếu thất lạc).</summary>
        public string SoPhieu { get; set; } = null!;

        /// <summary>Phòng Ban của phiếu, cố định = Phòng Ban mặc định của người tạo (bắt buộc là Kho Chờ Thanh Lý).</summary>
        public long PhongBanId { get; set; }
        public PhongBan PhongBan { get; set; } = null!;

        /// <summary>Lý do thanh lý.</summary>
        public string LyDo { get; set; } = null!;

        public long NguoiTaoId { get; set; }
        public User NguoiTao { get; set; } = null!;
        public DateTime NgayTao { get; set; }

        public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.PENDING;

        public long? NguoiDuyetId { get; set; }
        public User? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public ICollection<PhieuThanhLyChiTiet> ChiTiets { get; set; } = new List<PhieuThanhLyChiTiet>();
    }
}