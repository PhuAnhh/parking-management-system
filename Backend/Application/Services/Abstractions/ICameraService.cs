using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ICameraService
    {
        Task<IEnumerable<CameraDto>> GetAllAsync();
        Task<CameraDto> GetByIdAsync(int id);
        Task<CameraDto> CreateAsync(CreateCameraDto createCameraDto);
        Task<CameraDto> UpdateAsync(int id, UpdateCameraDto updateCameraDto);
        Task<bool> DeleteAsync(int id);
    }
}
