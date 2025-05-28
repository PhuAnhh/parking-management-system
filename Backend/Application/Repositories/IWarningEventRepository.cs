using Final_year_Project.Domain.Entities;

namespace Final_year_Project.Application.Repositories
{
    public interface IWarningEventRepository
    {
        Task<IEnumerable<WarningEvent>> GetAllAsync();
        Task<WarningEvent> GetByIdAsync(int id);
        Task CreateAsync(WarningEvent warningEvent);
        void Delete(WarningEvent warningEvent);
    }
}
