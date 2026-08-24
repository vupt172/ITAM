using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class LoaiTaiSan:CatalogEntity
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}
