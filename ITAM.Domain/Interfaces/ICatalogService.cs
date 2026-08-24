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
        Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false);
        Task<T?> GetByIdAsync(long id);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(long id, T entity);
        Task<bool> DeleteAsync(long id);

    }
}
