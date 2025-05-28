using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class WarningEventRepository : IWarningEventRepository
    {
        public readonly ParkingManagementContext _context;

        public WarningEventRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WarningEvent>> GetAllAsync()
        {
            return await _context.WarningEvents.ToListAsync();
        }

        public async Task<IEnumerable<WarningEvent>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.WarningEvents
                .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
                .ToListAsync();
        }

        public async Task<WarningEvent> GetByIdAsync(int id)
        {
            return await _context.WarningEvents.FindAsync(id);
        }

        public async Task CreateAsync(WarningEvent warningEvent)
        {
            await _context.WarningEvents.AddAsync(warningEvent);
        }

        public void Delete(WarningEvent warningEvent)
        {
            _context.WarningEvents.Remove(warningEvent);
        }
    }
}
