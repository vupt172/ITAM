using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class LoaiTaiSan:CatalogEntity
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}
