using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllAsync();
        Task<Card> GetByIdAsync(int id);
        Task CreateAsync(Card card);
        void Update(Card card);
        void Delete(Card card);
    }
}
