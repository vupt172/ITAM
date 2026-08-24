// ITAM.AppCore/Interfaces/IRoleService.cs
using ITAM.Domain.Entities.Identity;

namespace ITAM.AppCore.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<Role> CreateAsync(string code, string name, List<long> featureIds);
        Task UpdateAsync(long roleId, string name, List<long> featureIds);
        Task DeleteAsync(long roleId);
    }
}