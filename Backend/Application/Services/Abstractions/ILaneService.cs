using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ILaneService
    {
        Task<IEnumerable<LaneDto>> GetAllAsync();
        Task<LaneDto> GetByIdAsync(int id);
        Task<LaneDto> CreateAsync(CreateLaneDto createLaneDto);
        Task<LaneDto> UpdateAsync(int id, UpdateLaneDto updateLaneDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id, bool status);
    }
}
