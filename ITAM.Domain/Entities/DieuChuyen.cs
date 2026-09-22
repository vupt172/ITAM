using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Entities.Identity;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ITAM.Domain.Entities
{
    /// <summary>
    /// Phiếu Điều Chuyển tài sản — 1 phiếu có thể điều chuyển nhiều tài sản (nhiều dòng chi tiết).
    /// Workflow: Tạo (PENDING) → Duyệt (APPROVED, cập nhật Vị Trí + Trạng Thái từng tài sản,
    /// ghi LichSuDieuChuyenTaiSan) hoặc Từ chối (REJECTED, không đổi gì trên tài sản).
    /// </summary>
    public class DieuChuyen:AuditableEntity
    {
        public long Id { get; set; }

        /// <summary>Format DC-{năm}-{stt:D4}, reset theo năm — giống quy ước SoLo của LoNhap.</summary>
        public string SoPhieu { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; }

        public long NguoiTaoId { get; set; }
        public User NguoiTao { get; set; } = null!;

        /// <summary>Phòng Ban Chuyển Đi — luôn là Phòng Ban mặc định của người tạo phiếu, không cho chọn tay.</summary>
        public long PhongBanChuyenDiId { get; set; }
        public PhongBan PhongBanChuyenDi { get; set; } = null!;

        /// <summary>Phòng Ban Chuyển Đến — người tạo phiếu chọn. Có thể trùng PhongBanChuyenDiId (điều chuyển nội bộ giữa 2 Vị Trí của cùng 1 phòng ban).</summary>
        public long PhongBanChuyenDenId { get; set; }
        public PhongBan PhongBanChuyenDen { get; set; } = null!;

        /// <summary>Tên người giao — nhập tay, không bắt buộc.</summary>
        public string? NguoiGiao { get; set; }

        /// <summary>Tên người nhận — nhập tay, không bắt buộc.</summary>
        public string? NguoiNhan { get; set; }

        public TrangThaiDieuChuyen TrangThai { get; set; } = TrangThaiDieuChuyen.PENDING;

        public long? NguoiDuyetId { get; set; }
        public User? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public string? GhiChu { get; set; }

        public ICollection<DieuChuyenChiTiet> ChiTiets { get; set; } = new List<DieuChuyenChiTiet>();
    }
}