using ITAM.Domain.Entities.Catalogs;

public class VatTu: AuditableEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public long HangHoaId { get; set; }
    public HangHoa HangHoa { get; set; } = null!;
    public string? DonViTinh { get; set; }
    public int SoLuongTon { get; set; }
    public long ViTriTaiSanId { get; set; }
    public ViTriTaiSan ViTriTaiSan { get; set; } = null!;
    public string? GhiChu { get; set; }
}