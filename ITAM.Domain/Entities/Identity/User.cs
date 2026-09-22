using ITAM.Domain.Entities.Catalogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Entities.Identity
{
    public class User:AuditableEntity
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;   // BCrypt hash
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }

        public long? PhongBanId { get; set; }   // ← đây chính là "Phòng Ban mặc định"
        public PhongBan? PhongBan { get; set; }

        // Quan hệ N-N với Role thông qua UserPhongBans
        public ICollection<UserPhongBan> UserPhongBans { get; set; } = new List<UserPhongBan>();

        // Quan hệ N-N với Role thông qua UserRole
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
