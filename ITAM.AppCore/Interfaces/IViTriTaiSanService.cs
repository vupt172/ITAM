using ITAM.Domain.Entities.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Interfaces
{
    public interface IViTriTaiSanService : ICatalogService<ViTriTaiSan>
    {
        // Chỗ này để mở rộng sau, ví dụ: GetByPhongBanIdAsync(long phongBanId)
    }
}
