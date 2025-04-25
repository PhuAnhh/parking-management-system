using Final_year_Project.Device.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Repositories
{
    public interface ILaneCameraRepository
    {
        Task<IEnumerable<LaneCamera>> GetAllAsync();
        Task<LaneCamera> GetByIdAsync(int id);
        Task CreateAsync(LaneCamera laneCamera);
        void Update(LaneCamera laneCamera);
        void Delete(LaneCamera laneCamera);
    }
}
