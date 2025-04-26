using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface IGateRepository
    {
        Task<IEnumerable<Gate>> GetAllAsync();
        Task<Gate> GetByIdAsync(int id);
        Task CreateAsync(Gate gate);
        void Update(Gate gate);
        void Delete(Gate gate);
    }
}
