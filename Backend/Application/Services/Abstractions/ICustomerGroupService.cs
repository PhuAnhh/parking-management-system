using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ICustomerGroupService
    {
        Task<IEnumerable<CustomerGroupDto>> GetAllAsync();
        Task<CustomerGroupDto> GetByIdAsync(int id);
        Task<CustomerGroupDto> CreateAsync(CreateCustomerGroupDto createCustomerGroupDto);
        Task<CustomerGroupDto> UpdateAsync(int id, UpdateCustomerGroupDto updateCustomerGroupDto);
        Task<bool> DeleteAsync(int id, bool useSoftDelete);
    }
}
