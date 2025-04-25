using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services.Abstractions
{
    public interface IControlUnitService
    {
        Task<IEnumerable<ControlUnitDto>> GetAllAsync();
        Task<ControlUnitDto> GetByIdAsync(int id);
        Task<ControlUnitDto> CreateAsync(CreateControlUnitDto createControlUnitDto);
        Task<ControlUnitDto> UpdateAsync(int id, UpdateControlUnitDto updateControlUnitDto);
        Task<bool> DeleteAsync(int id);
    }
}
