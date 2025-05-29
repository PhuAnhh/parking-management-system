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
        public readonly ParkingManagementContext _context;

        public EntryLogRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EntryLog>> GetAllAsync()
        {
            return await _context.EntryLogs
                .Where(e => !e.Exited)   
                .ToListAsync();
        }

        public async Task<IEnumerable<EntryLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.EntryLogs
                .Where(r => r.EntryTime >= fromDate && r.EntryTime <= toDate && !r.Exited)
                .ToListAsync();
        }

        public async Task<EntryLog> GetByIdAsync(int id)
        {
            return await _context.EntryLogs.FindAsync(id);
        }

        public async Task CreateAsync(EntryLog entryLog)
        {
            await _context.EntryLogs.AddAsync(entryLog);
        }

        public void Update(EntryLog entryLog)
        {
            _context.EntryLogs.Update(entryLog);
        }

        public void Delete(EntryLog entryLog)
        {
            _context.EntryLogs.Remove(entryLog);
        }

        public async Task<bool> HasActiveEntryAsync(int cardId)
        {
            return await _context.EntryLogs
                .AsNoTracking()
                .AnyAsync(e => e.CardId == cardId && !e.Exited);
        }
        public async Task<bool> IsPlateNumberInUseAsync(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber)) return false;

            // Biển số đã được chuẩn hóa trước khi kiểm tra
            return await _context.EntryLogs
                .AsNoTracking()
                .Include(e => e.ExitLogs)
                .AnyAsync(e => e.PlateNumber == plateNumber && !e.Exited);
        }
    }
}
