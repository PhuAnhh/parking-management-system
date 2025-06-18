using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByIdWithRoleAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdWithRoleAndPermissionsAsync(int id);
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task CreateAsync(User user);
        void Update(User user);
        void Delete(User user);
    }
}
