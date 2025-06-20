using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var permissions = await _unitOfWork.Permissions.GetAllAsync();
            var permissionDtos = new List<PermissionDto>();

            foreach (var permission in permissions)
            {
                permissionDtos.Add(new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Endpoint = permission.Endpoint,
                    Method = permission.Method
                });
            }

            return permissionDtos;
        }
    }
}
