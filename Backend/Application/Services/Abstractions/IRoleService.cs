using Final_year_Project.Application.Models;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto> GetByIdAsync(int id);
        Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
        Task<RoleDto> UpdateAsync(int id, UpdateRoleDto updateRoleDto);
        Task<bool> DeleteAsync(int id);
    }
}
