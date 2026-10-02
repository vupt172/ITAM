using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Catalogs
{
    public class LoaiTaiSan:CatalogEntity
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal? TyLeHaoMon { get; set; }   // % / năm, ví dụ 20 = 20%. null = không hao mòn (CCDC, Vật tư)
        public int DisplayOrder { get; set; } = 0;
    }
}
