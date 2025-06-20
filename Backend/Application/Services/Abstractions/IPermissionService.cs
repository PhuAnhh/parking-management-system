using Final_year_Project.Application.Models;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllAsync();
    }
}
