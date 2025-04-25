using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services.Abstractions
{
    public interface ILaneControlUnitService
    {
        Task<IEnumerable<LaneControlUnitDto>> GetAllAsync();
        Task<LaneControlUnitDto> GetByIdAsync(int id);
        Task<LaneControlUnitDto> CreateAsync(CreateLaneControlUnitDto createLaneControlUnitDto);
        Task<LaneControlUnitDto> UpdateAsync(int id, UpdateLaneControlUnitDto updateLaneControlUnitDto);
        Task<bool> DeleteAsync(int id);
    }
}
