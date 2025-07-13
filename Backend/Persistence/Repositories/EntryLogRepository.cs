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

        public async Task<IEnumerable<EntryLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, bool? exited = null)
        {
            var query = _context.EntryLogs
                .AsNoTracking() 
                .Where(r => r.EntryTime >= fromDate && r.EntryTime <= toDate);

            if (exited.HasValue)
                query = query.Where(r => r.Exited == exited.Value);

            return await query.ToListAsync();
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

        public async Task<bool> HasActiveEntryAsync(int? cardId = null, int? customerId = null)
        {
            var query = _context.EntryLogs.AsQueryable();

            if (cardId.HasValue)
                query = query.Where(e => e.CardId == cardId.Value);

            if (customerId.HasValue)
                query = query.Where(e => e.CustomerId == customerId.Value);

            // Kiểm tra xe chưa ra khỏi bãi
            return await query
                .AnyAsync(e => !_context.ExitLogs.Any(ex => ex.EntryLogId == e.Id));
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
