using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> GetByIdAsync(int id);
        Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> ids);
    }
}
