using Final_year_Project.Device.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Repositories
{
    public interface ILedRepository
    {
        Task<IEnumerable<Led>> GetAllAsync();
        Task<Led> GetByIdAsync(int id);
        Task CreateAsync(Led led);
        void Update(Led led);
        void Delete(Led led);
    }
}
