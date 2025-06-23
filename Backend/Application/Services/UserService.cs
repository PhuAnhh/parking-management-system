using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Services
{
    public class UserService :IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userWithRole = await _unitOfWork.Users.GetByIdWithRoleAsync(user.Id);
                if (userWithRole != null)
                {
                    userDtos.Add(MapToUserDto(userWithRole));
                }
            }

            return userDtos;
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdWithRoleAsync(id);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(createUserDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {createUserDto.RoleId} not found.");
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Password = HashPassword(createUserDto.Password),
                Name = createUserDto.Name,
                RoleId = createUserDto.RoleId,
                Status = createUserDto.Status,
                Deleted = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Users.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var createdUser = await _unitOfWork.Users.GetByIdWithRoleAsync(user.Id);
            return MapToUserDto(createdUser!);
        }

        public async Task<UserDto> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var role = await _unitOfWork.Roles.GetByIdAsync(updateUserDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with ID {updateUserDto.RoleId} not found.");
            }

            user.Username = updateUserDto.Username;
            user.Name = updateUserDto.Name;
            user.RoleId = updateUserDto.RoleId;
            user.Status = updateUserDto.Status;
            user.UpdatedAt = DateTime.Now;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var updatedUser = await _unitOfWork.Users.GetByIdWithRoleAsync(user.Id);
            return MapToUserDto(updatedUser!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, bool status)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            user.Status = status;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new InvalidOperationException("New password and confirmation password do not match.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.Password))
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            user.Password = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                throw new InvalidOperationException("New password and confirmation password do not match.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Update password without verifying current password
            user.Password = HashPassword(resetPasswordDto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Name = user.Name,
                RoleId = user.RoleId,
                Status = user.Status,
                Deleted = user.Deleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
