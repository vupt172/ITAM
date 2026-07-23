using ITAM.Domain.Entities;
using ITAM.Domain.Exceptions;
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
    public class CatalogService<T> : ICatalogService<T> where T : CatalogEntity
    {
        private readonly AppDbContext _context;
        public CatalogService(AppDbContext context)
        {
            _context = context;
        }
        // Lấy danh sách: Mặc định chỉ lấy các bản ghi đang Active (Hoạt động)
        // Nếu màn hình Quản trị (Admin) cần xem tất cả -> truyền includeInactive = true
        public async Task<IEnumerable<T>> GetAllActiveAsync(bool includeInactive = false)
        {
            var query = _context.Set<T>().AsNoTracking();

            if (!includeInactive)
            {
                query = query.Where(x => x.IsActive); // Lọc chỉ lấy hàng đang Kích hoạt
            }

            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> CreateAsync(T entity)
        {
            // Kiểm tra trùng code
            if (await IsCodeExistsAsync(entity.Code))
            {
                throw new InvalidBusinessRuleException($"Mã '{entity.Code}' đã tồn tại trong hệ thống!");
            }

            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(int id, T entity)
        {
            var existing = await _context.Set<T>().FindAsync(id);
            if (existing == null) return false;

            // Kiểm tra trùng mã (trừ Id hiện tại)
            if (await IsCodeExistsAsync(entity.Code, excludeId: id))
            {
                throw new InvalidBusinessRuleException($"Mã '{entity.Code}' đã được sử dụng!");
            }

            // Cập nhật giá trị qua DbContext Entry
            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Set<T>().FindAsync(id);
            if (existing == null) return false;

            _context.Set<T>().Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> IsCodeExistsAsync(string code, int? excludeId = null)
        {
            // Nếu mã trống thì không cần kiểm tra
            if (string.IsNullOrWhiteSpace(code)) return false;

            var query = _context.Set<T>().AsNoTracking();

            if (excludeId.HasValue)
            {
                // Trường hợp UPDATE: Bỏ qua bản ghi có Id hiện tại
                return await query.AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.Id != excludeId.Value);
            }

            // Trường hợp CREATE: Kiểm tra trên toàn bộ bản ghi
            return await query.AnyAsync(x => x.Code.ToLower() == code.ToLower());
        }


    }
}
