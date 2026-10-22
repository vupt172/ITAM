using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class DieuChuyenVatTu
    {
        public long Id { get; set; }
        public string SoPhieu { get; set; } = default!;           // DCVT-{năm}-{stt:D4}
        public DateTime NgayTao { get; set; }
        public long NguoiTaoId { get; set; }
        public User NguoiTao { get; set; } = default!;
        public long PhongBanChuyenDiId { get; set; }
        public PhongBan PhongBanChuyenDi { get; set; } = default!;
        public long PhongBanChuyenDenId { get; set; }
        public PhongBan PhongBanChuyenDen { get; set; } = default!;
        public string? NguoiGiao { get; set; }
        public string? NguoiNhan { get; set; }
        public string? GhiChu { get; set; }
        public TrangThaiPhieu TrangThai { get; set; }
        public long? NguoiDuyetId { get; set; }
        public User? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public ICollection<DieuChuyenVatTuChiTiet> ChiTiets { get; set; } = new List<DieuChuyenVatTuChiTiet>();
    }

}
