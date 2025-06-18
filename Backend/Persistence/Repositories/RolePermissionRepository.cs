using Final_year_Project.Application.Repositories;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Final_year_Project.Persistence.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        public readonly ParkingManagementContext _context;

        public RolePermissionRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task CreateAsync(RolePermission rolePermission)
        {
            await _context.RolePermissions.AddAsync(rolePermission);
        }

        public void Delete(RolePermission rolePermission)
        {
            _context.RolePermissions.Remove(rolePermission);
        }

        public async Task DeleteByRoleIdAsync(int roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(rolePermissions);
        }
    }
}
