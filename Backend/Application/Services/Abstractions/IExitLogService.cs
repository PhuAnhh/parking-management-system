using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IExitLogService
    {
        Task<IEnumerable<ExitLogDto>> GetAllAsync();
        Task<IEnumerable<ExitLogDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<ExitLogDto> GetByIdAsync(int id);
        Task<ExitLogDto> CreateAsync(CreateExitLogDto createExitLogDto);
    }
}
