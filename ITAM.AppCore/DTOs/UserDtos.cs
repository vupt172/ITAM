// ITAM.AppCore/DTOs/Identity/UserDtos.cs
namespace ITAM.AppCore.DTOs.Identity
{
    public class CreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public long? PhongBanId { get; set; }
        public List<long> RoleIds { get; set; } = new();
    }

    public class UpdateUserDto
    {
        public long Id { get; set; }
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; }
        public long? PhongBanId { get; set; }
        public List<long> RoleIds { get; set; } = new();
    }

    public class ChangePasswordDto
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}