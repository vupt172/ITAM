namespace ITAM.AppCore.DTOs
{
    public class VatTuListDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MaHangHoa { get; set; } = string.Empty;
        public string? DonViTinh { get; set; }
        public int SoLuongTon { get; set; }
        public string TenViTri { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty; // ⬅ mới
    }

    public class UpdateVatTuDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DonViTinh { get; set; }
        public string? GhiChu { get; set; }
    }
}