using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
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
