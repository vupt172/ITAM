using ITAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Interfaces
{
    public interface ICatalogService<T> where T : CatalogEntity
    {
        Task<IEnumerable<T>> GetAllActiveAsync(bool includeInactive = false);
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(int id, T entity);
        Task<bool> DeleteAsync(int id);

    }
}
