using ITAM.Domain.Entities;

namespace ITAM.Domain.Interfaces
{
    public interface IHangHoaService : ICatalogService<HangHoa>
    {
        Task<IEnumerable<HangHoa>> GetAllWithDanhMucAsync(bool includeInactive = false);
    }
}
