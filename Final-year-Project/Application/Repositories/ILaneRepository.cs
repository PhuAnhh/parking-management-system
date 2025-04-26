using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ILaneRepository
    {
        Task<IEnumerable<Lane>> GetAllAsync();
        Task<Lane> GetByIdAsync(int id);
        Task CreateAsync(Lane lane);
        void Update(Lane lane);
        void Delete(Lane lane);
    }
}
