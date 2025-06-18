using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var roleWithPermissions = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(role.Id);
                if (roleWithPermissions != null)
                {
                    roleDtos.Add(MapToRoleDto(roleWithPermissions));
                }
            }

            return roleDtos;
        }

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(id);
            return role != null ? MapToRoleDto(role) : null;
        }

        public async Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto)
        {
            // Validate permission IDs
            if (createRoleDto.PermissionIds.Any())
            {
                var permissions = await _unitOfWork.Permissions.GetByIdsAsync(createRoleDto.PermissionIds);
                if (permissions.Count() != createRoleDto.PermissionIds.Count)
                {
                    throw new InvalidOperationException("One or more permission IDs are invalid.");
                }
            }

            // Create role
            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Roles.CreateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            // Assign permissions to role
            foreach (var permissionId in createRoleDto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _unitOfWork.RolePermissions.CreateAsync(rolePermission);
            }

            await _unitOfWork.SaveChangesAsync();

            // Return created role with permissions
            var createdRole = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(role.Id);
            return MapToRoleDto(createdRole!);
        }

        public async Task<RoleDto> UpdateAsync(int id, UpdateRoleDto updateRoleDto)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
            {
                return null;
            }

            if (updateRoleDto.PermissionIds.Any())
            {
                var permissions = await _unitOfWork.Permissions.GetByIdsAsync(updateRoleDto.PermissionIds);
                if (permissions.Count() != updateRoleDto.PermissionIds.Count)
                {
                    throw new InvalidOperationException("One or more permission IDs are invalid.");
                }
            }

            // Update role
            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;
            role.UpdatedAt = DateTime.Now;

            _unitOfWork.Roles.Update(role);

            await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(id);

            foreach (var permissionId in updateRoleDto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _unitOfWork.RolePermissions.CreateAsync(rolePermission);
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedRole = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(role.Id);
            return MapToRoleDto(updatedRole!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(id);

            _unitOfWork.Roles.Delete(role);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static RoleDto MapToRoleDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                Permissions = role.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Endpoint = rp.Permission.Endpoint,
                    Method = rp.Permission.Method
                }).ToList()
            };
        }
    }
}
