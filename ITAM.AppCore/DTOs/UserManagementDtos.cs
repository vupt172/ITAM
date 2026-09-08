namespace ITAM.AppCore.DTOs.Identity
{
    /// <summary>
    /// DTO for listing users in DataGrid
    /// </summary>
    public class UserListDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public long? PhongBanId { get; set; }
        public string? PhongBanName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>
    /// DTO for user add/edit form
    /// </summary>
    public class UserDetailDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public long? PhongBanId { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Selected role IDs for many-to-many binding
        /// </summary>
        public List<long> SelectedRoleIds { get; set; } = new();
    }
}