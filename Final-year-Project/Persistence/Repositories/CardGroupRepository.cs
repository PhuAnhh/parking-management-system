using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CardGroupRepository : ICardGroupRepository
    {
        public readonly DeviceServiceContext _context;

        public CardGroupRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CardGroup>> GetAllAsync()
        {
            return await _context.CardGroups.AsNoTracking().ToListAsync();
        }

        public async Task<CardGroup> GetByIdAsync(int id)
        {
            return await _context.CardGroups.FindAsync(id);
        }

        public async Task CreateAsync(CardGroup cardGroup)
        {
            await _context.CardGroups.AddAsync(cardGroup);
        }

        public void Update(CardGroup cardGroup)
        {
            _context.CardGroups.Update(cardGroup);
        }

        public void Delete(CardGroup cardGroup)
        {
            _context.CardGroups.Remove(cardGroup);
        }
    }
}
