using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IEntryLogService
    {
        Task<IEnumerable<EntryLogDto>> GetAllAsync();
        Task<IEnumerable<EntryLogDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, bool? exited = null);
        Task<EntryLogDto> GetByIdAsync(int id);
        Task<EntryLogDto> CreateAsync(CreateEntryLogDto createEntryLogDto);
        Task<bool> DeleteAsync(int id);
    }
}
