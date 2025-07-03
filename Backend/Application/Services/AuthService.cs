using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Final_year_Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Vui lòng nhập tên đăng nhập và mật khẩu"
                    };
                }

                //Kiểm tra người dùng 
                var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không hợp lệ"
                    };
                }

                if (!user.Status || user.Deleted)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tài khoản đã bị khóa hoặc xóa"
                    };
                }

                //So sánh mật khẩu
                if (!VerifyPassword(request.Password, user.Password))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không hợp lệ"
                    };
                }

                // Get user permissions
                var permissions = await _unitOfWork.Users.GetUserPermissionsAsync(user.Id);

                // Generate JWT token
                var token = GenerateJwtToken(user, permissions);

                // Get user info
                var userInfo = await GetUserInfoAsync(user.Id);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Đăng nhập thành công",
                    Token = token,
                    User = userInfo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
                return new LoginResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đăng nhập"
                };
            }
        }

        public async Task<UserInfo> GetUserInfoAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdWithRoleAndPermissionsAsync(userId);
            if (user == null) return null;

            var permissions = await _unitOfWork.Users.GetUserPermissionsAsync(userId);

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Role = user.Role.Name,
                Permissions = permissions.Select(p => $"{p.Method}:{p.Endpoint}").ToList()
            };
        }

        public string GenerateJwtToken(User user, List<Permission> permissions)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new("role", user.Role.Name)
            };

            // Add permissions as claims
            foreach (var permission in permissions)
            {
                var method = permission.Method?.Trim().ToUpper(); 
                var endpoint = permission.Endpoint?.Trim().ToLower(); 

                claims.Add(new Claim("permission", $"{method}:{endpoint}"));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Phương thức hash mật khẩu khi tạo user mới
        public static string HashPassword(string password)
        {
            // Sử dụng BCrypt để hash password với salt tự động
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        // Phương thức verify mật khẩu khi login
        private static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Sử dụng BCrypt để verify password
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception)
            {
                // Nếu hashedPassword không đúng format BCrypt, return false
                return false;
            }
        }
    }
}