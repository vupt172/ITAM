using ITAM.Domain.Entities.Identity;

namespace ITAM.AppCore.Interfaces
{
    public interface IFeatureService
    {
        Task<List<Feature>> GetAllAsync();
        Task<Feature?> GetByIdAsync(long id);
        Task<Feature?> GetByCodeAsync(string code);
    }
}