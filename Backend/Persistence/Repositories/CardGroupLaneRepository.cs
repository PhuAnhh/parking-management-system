using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CardGroupLaneRepository : ICardGroupLaneRepository
    {
        public readonly DeviceServiceContext _context;

        public CardGroupLaneRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CardGroupLane>> GetAllAsync()
        {
            return await _context.CardGroupLanes.AsNoTracking().ToListAsync();

        }

        public async Task<CardGroupLane> GetByIdAsync(int id)
        {
            return await _context.CardGroupLanes.FindAsync(id);
        }

        public async Task CreateAsync(CardGroupLane cardGroupLane)
        {
            await _context.CardGroupLanes.AddAsync(cardGroupLane);
        }

        public void Update(CardGroupLane cardGroupLane)
        {
            _context.CardGroupLanes.Update(cardGroupLane);
        }

        public void Delete(CardGroupLane cardGroupLane)
        {
            _context.CardGroupLanes.Remove(cardGroupLane);
        }
    }
}
