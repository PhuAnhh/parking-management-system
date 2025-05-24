using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IRevenueReportService
    {
        Task<IEnumerable<RevenueReportDto>> GetAllAsync();
        Task<RevenueReportDto> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
