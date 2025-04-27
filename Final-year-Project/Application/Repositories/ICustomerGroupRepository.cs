using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ICustomerGroupRepository
    {
        Task<IEnumerable<CustomerGroup>> GetAllAsync();
        Task<CustomerGroup> GetByIdAsync(int id);
        Task CreateAsync(CustomerGroup customerGroup);
        void Update(CustomerGroup customerGroup);
        void Delete(CustomerGroup customerGroup);
    }
}
