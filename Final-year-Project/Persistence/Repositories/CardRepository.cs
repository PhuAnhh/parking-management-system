using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CardRepository : ICardRepository
    {
        public readonly DeviceServiceContext _context;

        public CardRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Cards.AsNoTracking().ToListAsync();
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            return await _context.Cards.FindAsync(id);
        }

        public async Task CreateAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
        }

        public void Update(Card card)
        {
            _context.Cards.Update(card);
        }

        public void Delete(Card card)
        {
            _context.Cards.Remove(card);
        }
    }
}
