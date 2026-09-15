namespace ITAM.Domain.Entities
{
    /// <summary>Hàng hóa cụ thể được chọn trên chứng từ, ví dụ Mực HP 12A.</summary>
    public class HangHoa : CatalogEntity
    {
        public string HangSanXuat { get; set; }
        public string Model { get; set; }
        public long DMTaiSanId { get; set; }
        public string DonViTinh { get; set; }
        public bool RequireSerial { get; set; }
        public DMTaiSan DMTaiSan { get; set; } = null!;
    }
}
