using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IRevenueReportRepository
    {
        Task<IEnumerable<RevenueReport>> GetAllAsync();
        Task<RevenueReport> GetByIdAsync(int id);
        Task<RevenueReport> GetByCardGroupIdAsync(int cardGroupId);
        Task CreateAsync(RevenueReport revenueReport);
        void Update(RevenueReport revenueReport);
        void Delete(RevenueReport revenueReport);
    }
}
