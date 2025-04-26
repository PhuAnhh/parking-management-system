using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class LedRepository : ILedRepository
    {
        public readonly DeviceServiceContext _context;

        public LedRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Led>> GetAllAsync()
        {
            return await _context.Leds.ToListAsync();
        }

        public async Task<Led> GetByIdAsync(int id)
        {
            return await _context.Leds.FindAsync(id);
        }

        public async Task CreateAsync(Led led)
        {
            await _context.Leds.AddAsync(led);
        }

        public void Update(Led led)
        {
            _context.Leds.Update(led);
        }

        public void Delete(Led led)
        {
            _context.Leds.Remove(led);
        }
    }
}
