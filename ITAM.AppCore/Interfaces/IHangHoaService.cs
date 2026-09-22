using ITAM.Domain.Entities.Catalogs;

namespace ITAM.Domain.Interfaces
{
    public interface IHangHoaService : ICatalogService<HangHoa>
    {
        Task<IEnumerable<HangHoa>> GetAllWithDanhMucAsync(bool includeInactive = false);
    }
}
