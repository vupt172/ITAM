namespace ITAM.AppCore.DTOs.Identity
{
    /// <summary>
    /// DTO for listing roles in DataGrid
    /// </summary>
    public class RoleListDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int UserCount { get; set; }  // Số lượng người dùng có role này
        public int FeatureCount { get; set; }  // Số lượng chức năng được cấp
    }

    /// <summary>
    /// DTO for role add/edit form
    /// </summary>
    public class RoleDetailDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<long> SelectedFeatureIds { get; set; } = new();
    }

    /// <summary>
    /// DTO for feature checkbox list in role management
    /// </summary>
    public class FeatureCheckDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSelected { get; set; }
    }
}