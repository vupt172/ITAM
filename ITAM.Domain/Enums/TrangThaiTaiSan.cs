using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Enums
{
    public enum TrangThaiTaiSan
    {
        /// <summary>Tài sản đang ở Kho Lưu Trữ.</summary>
        IN_STOCK,

        /// <summary>Tài sản đã điều chuyển cho Phòng Ban Chức Năng.</summary>
        ASSIGNED,

        /// <summary>Tài sản đang được sửa chữa (từ IN_STOCK hoặc ASSIGNED).</summary>
        MAINTENANCE,

        /// <summary>Tài sản bị thất lạc. Trạng thái cuối, không khôi phục.</summary>
        LOST,

        /// <summary>Tài sản đang chờ thanh lý (chỉ đi từ IN_STOCK).</summary>
        DISPOSAL_PENDING,

        /// <summary>Tài sản đã thanh lý. Trạng thái cuối.</summary>
        DISPOSED
    }
}
