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
                // Tính tổng số thẻ (cards) của nhóm khách hàng
                var cardCount = customerGroup.Customers?
                    .Where(c => !c.Deleted)
                    .Sum(c => c.Cards?.Count(card => !card.Deleted) ?? 0) ?? 0;

                customerGroupDtos.Add(new CustomerGroupDto
                {
                    Id = customerGroup.Id,
                    Name = customerGroup.Name,
                    Code = customerGroup.Code,
                    CreatedAt = customerGroup.CreatedAt,
                    UpdatedAt = customerGroup.UpdatedAt,
                    CardCount = cardCount
                });
            }

            return customerGroupDtos;
        }

        public async Task<CustomerGroupDto> GetByIdAsync(int id)
        {
            var customerGroup = await _unitOfWork.CustomerGroups.GetByIdAsync(id);

            if (customerGroup == null)
                return null;

            // Tính tổng số thẻ cho nhóm khách hàng này
            var cardCount = customerGroup.Customers?
                .Where(c => !c.Deleted)
                .Sum(c => c.Cards?.Count(card => !card.Deleted) ?? 0) ?? 0;

            return new CustomerGroupDto
            {
                Id = customerGroup.Id,
                Name = customerGroup.Name,
                Code = customerGroup.Code,
                CreatedAt = customerGroup.CreatedAt,
                UpdatedAt = customerGroup.UpdatedAt,
                CardCount = cardCount
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
                UpdatedAt = customerGroup.UpdatedAt,
                CardCount = 0
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

            // Tính tổng số thẻ sau khi update
            var cardCount = customerGroup.Customers?
                .Where(c => !c.Deleted)
                .Sum(c => c.Cards?.Count(card => !card.Deleted) ?? 0) ?? 0;

            return new CustomerGroupDto
            {
                Id = customerGroup.Id,
                Name = customerGroup.Name,
                Code = customerGroup.Code,
                CreatedAt = customerGroup.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                CardCount = cardCount
            };
        }

        public async Task<bool> DeleteAsync(int id, bool useSoftDelete)
        {
            try
            {
                var customerGroup = await _unitOfWork.CustomerGroups.GetByIdAsync(id);
                if (customerGroup == null) return false;

                if (useSoftDelete)
                {
                    customerGroup.Deleted = true;
                    customerGroup.UpdatedAt = DateTime.UtcNow;

                    if (customerGroup.Customers != null && customerGroup.Customers.Any())
                    {
                        foreach (var customer in customerGroup.Customers)
                        {
                            customer.Deleted = true;
                            customer.UpdatedAt = DateTime.UtcNow;
                            _unitOfWork.Customers.Update(customer);
                        }
                    }
                    _unitOfWork.CustomerGroups.Update(customerGroup);
                }
                else
                {
                    if (customerGroup.Customers != null && customerGroup.Customers.Any())
                    {
                        foreach (var customer in customerGroup.Customers)
                        {
                            _unitOfWork.Customers.Delete(customer);
                        }
                    }

                    _unitOfWork.CustomerGroups.Delete(customerGroup);
                }
                await _unitOfWork.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
