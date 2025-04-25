using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services.Abstractions
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
