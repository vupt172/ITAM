using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Identity
{
    // Bảng trung gian Role <-> Feature
    public class RoleFeature
    {
        public long RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public long FeatureId { get; set; }
        public Feature Feature { get; set; } = null!;
    }
}
