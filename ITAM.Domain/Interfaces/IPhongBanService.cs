using ITAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Domain.Interfaces
{
    public interface IPhongBanService
    {
        Task<IEnumerable<PhongBan>> GetAllAsync();
        Task<PhongBan?> GetByIdAsync(int id);

        Task<PhongBan> CreateAsync(PhongBan phongBan);
        Task<bool> UpdateAsync(int id,PhongBan phongBan);
        Task<bool> DeleteAsync(int id);
    }
}
