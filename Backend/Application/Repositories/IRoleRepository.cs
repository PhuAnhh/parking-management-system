using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByIdWithPermissionsAsync(int id);
        Task CreateAsync(Role role);
        void Update(Role role);
        void Delete(Role role);
    }
}
