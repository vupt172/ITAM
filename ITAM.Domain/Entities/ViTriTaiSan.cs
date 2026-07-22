using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class ViTriTaiSan : CatalogEntity
    {
        public int PhongBanId { get; set; }

        public PhongBan PhongBan { get; set; } = null!;
    }
}
