using ITAM.Domain.Entities.Catalogs;
using ITAM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class PhieuThatLacChiTiet
    {
        public long Id { get; set; }

        public long PhieuThatLacId { get; set; }
        public PhieuThatLac PhieuThatLac { get; set; } = null!;

        public long TaiSanDinhDanhId { get; set; }
        public TaiSanDinhDanh TaiSanDinhDanh { get; set; } = null!;

        /// <summary>Snapshot vị trí của tài sản lúc thêm vào phiếu, không cho sửa.</summary>
        public long ViTriTruocId { get; set; }
        public ViTriTaiSan ViTriTruoc { get; set; } = null!;

        /// <summary>Snapshot trạng thái lúc thêm vào phiếu (IN_STOCK hoặc ASSIGNED).</summary>
        public TrangThaiTaiSan TrangThaiTruoc { get; set; }

        public string? GhiChu { get; set; }
    }
}
