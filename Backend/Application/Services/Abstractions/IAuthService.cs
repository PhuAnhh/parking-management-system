using Final_year_Project.Application.Models;
using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<UserInfo?> GetUserInfoAsync(int userId);
        string GenerateJwtToken(User user, List<Permission> permissions);
    }
}
