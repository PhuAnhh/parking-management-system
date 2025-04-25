using Final_year_Project.Device.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Repositories
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
