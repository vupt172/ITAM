using ITAM.AppCore.Interfaces;
using ITAM.Domain.Entities.Identity;
using ITAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAM.Infrastructure.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly AppDbContext _context;

        public FeatureService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Feature>> GetAllAsync()
        {
            return await _context.Features
                .AsNoTracking()
                .OrderBy(f => f.Code)
                .ToListAsync();
        }

        public async Task<Feature?> GetByIdAsync(long id)
        {
            return await _context.Features
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feature?> GetByCodeAsync(string code)
        {
            return await _context.Features
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Code == code);
        }
    }
}