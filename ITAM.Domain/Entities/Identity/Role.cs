using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Identity
{
    public class Role
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;   // VD: "ADMIN", "IT", "MANAGER"
        public string Name { get; set; } = null!;   // VD: "Quản trị viên"

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RoleFeature> RoleFeatures { get; set; } = new List<RoleFeature>();
    }
}
