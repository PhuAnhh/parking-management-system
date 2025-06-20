using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface IControlUnitService
    {
        Task<IEnumerable<ControlUnitDto>> GetAllAsync();
        Task<ControlUnitDto> GetByIdAsync(int id);
        Task<ControlUnitDto> CreateAsync(CreateControlUnitDto createControlUnitDto);
        Task<ControlUnitDto> UpdateAsync(int id, UpdateControlUnitDto updateControlUnitDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id, bool status);
    }
}
