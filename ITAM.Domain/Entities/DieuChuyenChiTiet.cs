using ITAM.Domain.Entities.Catalogs;

namespace ITAM.Domain.Entities
{
    /// <summary>1 dòng chi tiết = 1 TaiSanDinhDanh được điều chuyển trong phiếu (không áp dụng cho VatTu).</summary>
    public class DieuChuyenChiTiet: AuditableEntity
    {
        public long Id { get; set; }

        public long DieuChuyenId { get; set; }
        public DieuChuyen DieuChuyen { get; set; } = null!;

        public long TaiSanDinhDanhId { get; set; }
        public TaiSanDinhDanh TaiSanDinhDanh { get; set; } = null!;

        /// <summary>
        /// Snapshot Vị Trí hiện tại của tài sản tại thời điểm thêm vào phiếu — tự bốc theo tài sản,
        /// không cho người dùng sửa tay.
        /// </summary>
        public long ViTriChuyenDiId { get; set; }
        public ViTriTaiSan ViTriChuyenDi { get; set; } = null!;

        public long ViTriChuyenDenId { get; set; }
        public ViTriTaiSan ViTriChuyenDen { get; set; } = null!;

        public string? GhiChu { get; set; }
    }
}