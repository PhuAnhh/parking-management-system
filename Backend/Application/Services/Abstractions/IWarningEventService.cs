using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IWarningEventService
    {
        Task<IEnumerable<WarningEventDto>> GetAllAsync();
        Task<IEnumerable<WarningEventDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<WarningEventDto> GetByIdAsync(int id);
        Task<WarningEventDto> CreateAsync(CreateWarningEventDto createWarningEventDto);
        Task<bool> DeleteAsync(int id);
    }
}
