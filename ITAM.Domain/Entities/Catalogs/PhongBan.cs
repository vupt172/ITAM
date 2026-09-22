using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Catalogs
{
    public class PhongBan : CatalogEntity
    {
        public ICollection<ViTriTaiSan> ViTriTaiSans { get; set; } = new List<ViTriTaiSan>();
    }
}
