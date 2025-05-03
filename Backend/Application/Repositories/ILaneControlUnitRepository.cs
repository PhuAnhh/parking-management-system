using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ILaneControlUnitRepository
    {
        Task<IEnumerable<LaneControlUnit>> GetAllAsync();
        Task<LaneControlUnit> GetByIdAsync(int id);
        Task CreateAsync(LaneControlUnit laneControlUnit);
        void Update(LaneControlUnit laneControlUnit);
        void Delete(LaneControlUnit laneControlUnit);
    }
}
