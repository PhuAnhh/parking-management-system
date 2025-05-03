using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface IComputerRepository
    {
        Task<IEnumerable<Computer>> GetAllAsync();
        Task<Computer> GetByIdAsync(int id);
        Task CreateAsync(Computer computer);
        void Update(Computer computer);
        void Delete(Computer computer);
    }
}
