using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IEntryLogRepository
    {
        Task<IEnumerable<EntryLog>> GetAllAsync();
        Task<IEnumerable<EntryLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<EntryLog> GetByIdAsync(int id);
        Task CreateAsync(EntryLog entryLog);
        void Update(EntryLog entryLog);
        void Delete(EntryLog entryLog);
        Task<bool> HasActiveEntryAsync(int cardId);
        Task<bool> IsPlateNumberInUseAsync(string plateNumber);
    }
}
