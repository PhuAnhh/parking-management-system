using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class EntryLogRepository : IEntryLogRepository
    {
        public readonly DeviceServiceContext _context;

        public EntryLogRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EntryLog>> GetAllAsync()
        {
            return await _context.EntryLogs.ToListAsync();
        }

        public async Task<EntryLog> GetByIdAsync(int id)
        {
            return await _context.EntryLogs.FindAsync(id);
        }

        public async Task CreateAsync(EntryLog entryLog)
        {
            await _context.EntryLogs.AddAsync(entryLog);
        }

        public void Delete(EntryLog entryLog)
        {
            _context.EntryLogs.Remove(entryLog);
        }
    }
}
