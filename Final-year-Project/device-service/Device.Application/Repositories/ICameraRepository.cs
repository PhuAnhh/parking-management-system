using Final_year_Project.Device.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Repositories
{
    public interface ICameraRepository
    {
        Task<IEnumerable<Camera>> GetAllAsync();
        Task<Camera> GetByIdAsync(int id);
        Task CreateAsync(Camera camera);
        void Update(Camera camera);
        void Delete(Camera camera);
    }
}
