namespace ITAM.AppCore.DTOs
{
    public class VatTuListDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TenHangHoa { get; set; } = string.Empty;
        public string? DonViTinh { get; set; }
        public int SoLuongTon { get; set; }
        public int SoLuongChoMuon { get; set; }
        public int SoLuongKhaDung { get; set; }
        public string TenViTri { get; set; } = string.Empty;
    }

    public class UpdateVatTuDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DonViTinh { get; set; }
    }
}