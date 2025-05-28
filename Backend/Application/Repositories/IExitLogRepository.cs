using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IExitLogRepository
    {
        Task<IEnumerable<ExitLog>> GetAllAsync();
        Task<IEnumerable<ExitLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ExitLog> GetByIdAsync(int id);
        Task CreateAsync(ExitLog exitLog);
    }
}
