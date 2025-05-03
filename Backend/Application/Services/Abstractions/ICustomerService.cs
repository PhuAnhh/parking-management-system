using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto);
        Task<CustomerDto> UpdateAsync(int id, UpdateCustomerDto updateCustomerDto);
        Task<bool> DeleteAsync(int id);
    }
}
