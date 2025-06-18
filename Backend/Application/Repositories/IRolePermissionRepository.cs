using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IRolePermissionRepository
    {
        Task<IEnumerable<RolePermission>> GetByRoleIdAsync(int roleId);
        Task CreateAsync(RolePermission rolePermission);
        void Delete(RolePermission rolePermission);
        Task DeleteByRoleIdAsync(int roleId);
    }
}
