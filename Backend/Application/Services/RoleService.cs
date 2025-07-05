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
                    roleDtos.Add(new RoleDto
                    {
                        Id = roleWithPermissions.Id,
                        Name = roleWithPermissions.Name,
                        Description = roleWithPermissions.Description,
                        CreatedAt = roleWithPermissions.CreatedAt,
                        UpdatedAt = roleWithPermissions.UpdatedAt,
                        Permissions = roleWithPermissions.RolePermissions.Select(rp => new PermissionDto
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Name,
                            Endpoint = rp.Permission.Endpoint,
                            Method = rp.Permission.Method
                        }).ToList()
                    });
                }
            }

            return roleDtos;
        }

        public async Task<RoleDto> GetByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(id);

            if (role == null)
                return null;

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

        public async Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto)
        {
            // Validate permission IDs
            if (createRoleDto.PermissionIds.Any())
            {
                var permissions = await _unitOfWork.Permissions.GetByIdsAsync(createRoleDto.PermissionIds);
                if (permissions.Count() != createRoleDto.PermissionIds.Count)
                {
                    throw new InvalidOperationException("Một hoặc nhiều ID có quyền không hợp lệ");
                }
            }

            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Roles.CreateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            foreach (var permissionId in createRoleDto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.RolePermissions.CreateAsync(rolePermission);
            }

            await _unitOfWork.SaveChangesAsync();

            var createdRole = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(role.Id);
            return new RoleDto
            {
                Id = createdRole.Id,
                Name = createdRole.Name,
                Description = createdRole.Description,
                CreatedAt = createdRole.CreatedAt,
                UpdatedAt = createdRole.UpdatedAt,
                Permissions = createdRole.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Endpoint = rp.Permission.Endpoint,
                    Method = rp.Permission.Method
                }).ToList()
            };
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
                    throw new InvalidOperationException("Một hoặc nhiều ID quyền không hợp lệ");
                }
            }

            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;
            role.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Roles.Update(role);

            await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(id);

            foreach (var permissionId in updateRoleDto.PermissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.RolePermissions.CreateAsync(rolePermission);
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedRole = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(role.Id);
            return new RoleDto
            {
                Id = updatedRole.Id,
                Name = updatedRole.Name,
                Description = updatedRole.Description,
                CreatedAt = updatedRole.CreatedAt,
                UpdatedAt = updatedRole.UpdatedAt,
                Permissions = updatedRole.RolePermissions.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Endpoint = rp.Permission.Endpoint,
                    Method = rp.Permission.Method
                }).ToList()
            };
        }

        public async Task<bool> DeleteAsync(int id, bool useSoftDelete)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null) return false;

            try
            {
                if (useSoftDelete)
                {
                    // xóa mềm
                    role.Deleted = true;
                    role.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Roles.Update(role);
                }
                else
                {
                    // Kiểm tra nếu có users đang dùng role này
                    if (role.Users != null && role.Users.Any())
                    {
                        throw new InvalidOperationException("Không thể xóa vai trò vì có người dùng đang sử dụng.");
                    }

                    // Xóa các phân quyền (role_permissions)
                    await _unitOfWork.RolePermissions.DeleteByRoleIdAsync(id);

                    // Xóa cứng 
                    _unitOfWork.Roles.Delete(role);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}