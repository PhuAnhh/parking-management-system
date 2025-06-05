using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class RevenueReportRepository : IRevenueReportRepository
    {
        public readonly ParkingManagementContext _context;

        public RevenueReportRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RevenueReport>> GetAllAsync()
        {
            return await _context.RevenueReports.ToListAsync();
        }

        public async Task<RevenueReport> GetByIdAsync(int id)
        {
            return await _context.RevenueReports.FindAsync(id);
        }

        public async Task<RevenueReport> GetByCardGroupIdAsync(int cardGroupId)
        {
            return await _context.RevenueReports.FirstOrDefaultAsync(r => r.CardGroupId == cardGroupId);
        }

        public async Task<IEnumerable<RevenueReport>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var revenueData = await _context.ExitLogs
                .Include(el => el.CardGroup)
                .Where(el => el.ExitTime >= fromDate && el.ExitTime <= toDate)
                .GroupBy(el => new { 
                    el.CardGroupId, 
                    CardGroupName = el.CardGroup.Name 
                })
                .Select(g => new RevenueReport
                {
                    CardGroupId = g.Key.CardGroupId,
                    ExitCount = g.Count(),
                    Revenue = g.Sum(el => el.TotalPrice),
                    CreatedAt = fromDate, 
                    UpdatedAt = DateTime.Now
                })
                .ToListAsync();
                
            return revenueData;
        }

        public async Task CreateAsync(RevenueReport revenueReport)
        {
            await _context.RevenueReports.AddAsync(revenueReport);
        }

        public void Update(RevenueReport revenueReport)
        {
            _context.RevenueReports.Update(revenueReport);
        }

        public void Delete(RevenueReport revenueReport)
        {
            _context.RevenueReports.Remove(revenueReport);
        }
    }
}
