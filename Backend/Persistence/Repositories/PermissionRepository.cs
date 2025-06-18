using Final_year_Project.Application.Repositories;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Final_year_Project.Persistence.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        public readonly ParkingManagementContext _context;

        public PermissionRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            return await _context.Permissions.FindAsync(id);
        }

        public async Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Permissions.Where(p => ids.Contains(p.Id)).ToListAsync();
        }
    }
}
