using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ICardGroupLaneRepository
    {
        Task<IEnumerable<CardGroupLane>> GetAllAsync();
        Task<CardGroupLane> GetByIdAsync(int id);
        Task CreateAsync(CardGroupLane cardGroupLane);
        void Update(CardGroupLane cardGroupLane);
        void Delete(CardGroupLane cardGroupLane);

    }
}
