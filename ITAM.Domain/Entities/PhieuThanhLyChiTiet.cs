using ITAM.Domain.Entities.Catalogs;

namespace ITAM.Domain.Entities
{
    public class PhieuThanhLyChiTiet
    {
        public long Id { get; set; }

        public long PhieuThanhLyId { get; set; }
        public PhieuThanhLy PhieuThanhLy { get; set; } = null!;

        public long TaiSanDinhDanhId { get; set; }
        public TaiSanDinhDanh TaiSanDinhDanh { get; set; } = null!;

        /// <summary>Snapshot vị trí lúc thêm dòng (= Kho Chờ Thanh Lý), không cho sửa.</summary>
        public long ViTriTruocId { get; set; }
        public ViTriTaiSan ViTriTruoc { get; set; } = null!;

        /// <summary>Giá thanh lý của tài sản. 0 = chưa có giá (hợp lệ).</summary>
        public decimal GiaThanhLy { get; set; }

        public string? GhiChu { get; set; }
    }
}