using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Persistence.Repositories
{
    public class CardRepository : ICardRepository
    {
        public readonly ParkingManagementContext _context;

        public CardRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Cards.AsNoTracking().ToListAsync();
        }

        public async Task<Card> GetByIdAsync(int id)
        {
            return await _context.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
        }

        public void Update(Card card)
        {
            var tracked = _context.ChangeTracker.Entries<Card>()
                .FirstOrDefault(e => e.Entity.Id == card.Id);

            if (tracked != null)
            {
                _context.Entry(tracked.Entity).State = EntityState.Detached;
            }

            _context.Cards.Update(card);
        }

        public async Task<bool> HasActiveEntryInCardGroupAsync(int cardGroupId)
        {
            return await _context.EntryLogs
                .AnyAsync(e => !e.Exited && e.Card.CardGroupId == cardGroupId);
        }

        public void Delete(Card card)
        {
            _context.Cards.Remove(card);
        }
    }
}
