using ITAM.Domain.Entities.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Identity
{
    public class UserPhongBan
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public long PhongBanId { get; set; }
        public PhongBan PhongBan { get; set; } = null!;
    }
}
