using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ILedService
    {
        Task<IEnumerable<LedDto>> GetAllAsync();
        Task<LedDto> GetByIdAsync(int id);
        Task<LedDto> CreateAsync(CreateLedDto createLedDto);
        Task<LedDto> UpdateAsync(int id, UpdateLedDto updateLedDto);
        Task<bool> DeleteAsync(int id);
    }
}
