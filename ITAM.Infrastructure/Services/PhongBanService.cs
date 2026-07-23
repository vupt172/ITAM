using ITAM.Domain.Entities;
using ITAM.Domain.Interfaces;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITAM.Infrastructure.Services
{
    public class PhongBanService : IPhongBanService
    {
        private readonly AppDbContext _context;
        public PhongBanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhongBan>> GetAllAsync()
        {
            return await _context.PhongBan.AsNoTracking().ToListAsync();
        }
        public async Task<PhongBan?> GetByIdAsync(int id)
        {
            return await _context.PhongBan.FindAsync(id);
        }
        public Task<PhongBan> CreateAsync(PhongBan phongBan)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

   

  

        public Task<bool> UpdateAsync(int id, PhongBan phongBan)
        {
            throw new NotImplementedException();
        }
    }
}
