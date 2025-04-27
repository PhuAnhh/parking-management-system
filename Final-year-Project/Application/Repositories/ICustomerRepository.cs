using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task CreateAsync(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
    }
}
