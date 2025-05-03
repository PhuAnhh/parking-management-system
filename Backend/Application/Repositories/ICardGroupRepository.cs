using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface ICardGroupRepository
    {
        Task<IEnumerable<CardGroup>> GetAllAsync();
        Task<CardGroup> GetByIdAsync(int id);
        Task CreateAsync(CardGroup cardGroup);
        void Update(CardGroup cardGroup);
        void Delete(CardGroup cardGroup);
    }
}
