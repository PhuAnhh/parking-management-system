using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var customerDtos = new List<CustomerDto>();

            foreach (var customer in customers)
            {
                customerDtos.Add(new CustomerDto
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Code = customer.Code,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    CustomerGroupId = customer.CustomerGroupId,
                    CreatedAt = customer.CreatedAt,
                    UpdatedAt = customer.UpdatedAt
                });
            }

            return customerDtos;
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Code = customer.Code,
                Phone = customer.Phone,
                Address = customer.Address,
                CustomerGroupId = customer.CustomerGroupId,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = new Customer
            {
                Name = createCustomerDto.Name,
                Code = createCustomerDto.Code,
                Phone = createCustomerDto.Phone,
                Address= createCustomerDto.Address,
                CustomerGroupId = createCustomerDto.CustomerGroupId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Customers.CreateAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Code = customer.Code,
                Phone = customer.Phone,
                Address = customer.Address,
                CustomerGroupId = customer.CustomerGroupId,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }

        public async Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer == null)
                return null;

            customer.Name = updateCustomerDto.Name;
            customer.Code = updateCustomerDto.Code;
            customer.Phone = updateCustomerDto.Phone;
            customer.Address = updateCustomerDto.Address;
            customer.CustomerGroupId = updateCustomerDto.CustomerGroupId;
            customer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Code = customer.Code,
                Phone = customer.Phone,
                Address = customer.Address,
                CustomerGroupId = customer.CustomerGroupId,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);

            if (customer == null)
                return false;

            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(customerId: id);
            if (hasActiveEntry)
            {
                throw new Exception("Không thể xóa khi khách hàng đang gửi xe");
            }

            customer.Deleted = true;
            _unitOfWork.Customers.Update(customer);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
