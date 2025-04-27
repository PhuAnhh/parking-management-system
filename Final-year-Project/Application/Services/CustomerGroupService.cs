using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class CustomerGroupService : ICustomerGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerGroupDto>> GetAllAsync()
        {
            var customerGroups = await _unitOfWork.CustomerGroups.GetAllAsync();
            var customerGroupDtos = new List<CustomerGroupDto>();

            foreach (var customerGroup in customerGroups)
            {
                customerGroupDtos.Add(new CustomerGroupDto
                {
                    Id = customerGroup.Id,
                    Name = customerGroup.Name,
                    Code = customerGroup.Code,
                    CreatedAt = customerGroup.CreatedAt,
                    UpdatedAt = customerGroup.UpdatedAt
                });
            }

            return customerGroupDtos;
        }

        public async Task<CustomerGroupDto> GetByIdAsync(int id)
        {
            var customerGroup = await _unitOfWork.CustomerGroups.GetByIdAsync(id);

            if (customerGroup == null)
                return null;

            return new CustomerGroupDto
            {
                Id = customerGroup.Id,
                Name = customerGroup.Name,
                Code = customerGroup.Code,
                CreatedAt = customerGroup.CreatedAt,
                UpdatedAt = customerGroup.UpdatedAt
            };
        }

        public async Task<CustomerGroupDto> CreateAsync(CreateCustomerGroupDto createCustomerGroupDto)
        {
            var customerGroup = new CustomerGroup
            {
                Name = createCustomerGroupDto.Name,
                Code = createCustomerGroupDto.Code,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CustomerGroups.CreateAsync(customerGroup);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerGroupDto
            {
                Id = customerGroup.Id,
                Name = customerGroup.Name,
                Code = customerGroup.Code,
                CreatedAt = customerGroup.CreatedAt,
                UpdatedAt = customerGroup.UpdatedAt
            };
        }

        public async Task<CustomerGroupDto> UpdateAsync(int id, UpdateCustomerGroupDto updateCustomerGroupDto)
        {
            var customerGroup = await _unitOfWork.CustomerGroups.GetByIdAsync(id);

            if (customerGroup == null)
                return null;

            customerGroup.Name = updateCustomerGroupDto.Name;
            customerGroup.Code = updateCustomerGroupDto.Code;
            customerGroup.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CustomerGroups.Update(customerGroup);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerGroupDto
            {
                Id = customerGroup.Id,
                Name = customerGroup.Name,
                Code = customerGroup.Code,
                CreatedAt = customerGroup.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customerGroup = await _unitOfWork.CustomerGroups.GetByIdAsync(id);

            if (customerGroup == null)
                return false;

            _unitOfWork.CustomerGroups.Delete(customerGroup);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
