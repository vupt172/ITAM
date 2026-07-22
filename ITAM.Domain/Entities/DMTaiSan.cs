using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities
{
    public class DMTaiSan : CatalogEntity
    {
        public bool IsTrackedById { get; set; }
        public bool RequireSerial { get; set; }
        public int DisplayOrder { get; set; }= 0;

    }
}
