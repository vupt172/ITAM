using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Identity
{
    public class Feature
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;   // VD: "TAISAN_QUANLY"
        public string Name { get; set; } = null!;   // VD: "Quản lý tài sản"
        public string? Description { get; set; }
        public ICollection<RoleFeature> RoleFeatures { get; set; } = new List<RoleFeature>();
    }
}