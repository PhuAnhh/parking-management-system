using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface IControlUnitRepository
    {
        Task<IEnumerable<ControlUnit>> GetAllAsync();
        Task<ControlUnit> GetByIdAsync(int id);
        Task CreateAsync(ControlUnit controlUnit);
        void Update(ControlUnit controlUnit);
        void Delete(ControlUnit controlUnit);
    }
}
