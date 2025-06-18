using Final_year_Project.Application.Repositories;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Final_year_Project.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public readonly ParkingManagementContext _context;

        public UserRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByIdWithRoleAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && !u.Deleted);
        }

        public async Task<User?> GetByIdWithRoleAndPermissionsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Deleted);
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.Deleted);

            if (user == null) return new List<Permission>();

            return await _context.RolePermissions
                .Where(rp => rp.RoleId == user.RoleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
