using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ILaneCameraService
    {
        Task<IEnumerable<LaneCameraDto>> GetAllAsync();
        Task<LaneCameraDto> GetByIdAsync(int id);
        Task<LaneCameraDto> CreateAsync(CreateLaneCameraDto createLaneCameraDto);
        Task<LaneCameraDto> UpdateAsync(int id, UpdateLaneCameraDto updateLaneCameraDto);
        Task<bool> DeleteAsync(int id);
    }
}
