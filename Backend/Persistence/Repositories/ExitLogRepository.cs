using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class ExitLogRepository : IExitLogRepository
    {
        public readonly ParkingManagementContext _context;

        public ExitLogRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExitLog>> GetAllAsync()
        {
            return await _context.ExitLogs
                .Include(e => e.EntryLog)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExitLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.ExitLogs
                .Include(e => e.EntryLog)
                .Where(r => r.ExitTime >= fromDate && r.ExitTime <= toDate)
                .ToListAsync();
        }

        public async Task<ExitLog> GetByIdAsync(int id)
        {
            return await _context.ExitLogs
                .Include(e => e.EntryLog)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task CreateAsync(ExitLog exitLog)
        {
            await _context.ExitLogs.AddAsync(exitLog);
        }
    }
}
