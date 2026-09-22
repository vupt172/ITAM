using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Catalogs
{
    public class ViTriTaiSan : CatalogEntity
    {
        public long PhongBanId { get; set; }

        public PhongBan PhongBan { get; set; } = null!;

        /// <summary>
        /// True nếu đây là 1 trong các Vị Trí hệ thống cố định do DbSeeder tạo sẵn
        /// (Kho Lưu Trữ / Kho Thất Lạc / Kho Chờ Thanh Lý / Kho Đã Thanh Lý — xem
        /// ITAM.Shared.Constants.ViTriTaiSanCodes). False = Vị Trí đại diện 1 Khoa/Phòng Ban
        /// chức năng bình thường. Dùng để UI phân biệt khi cần (VD: lọc combobox, cảnh báo).
        /// </summary>
        public bool IsSystem { get; set; }
    }
}