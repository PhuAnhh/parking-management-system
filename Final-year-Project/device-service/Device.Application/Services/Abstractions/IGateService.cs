using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services.Abstractions
{
    public interface IGateService
    {
        Task<IEnumerable<GateDto>> GetAllAsync();
        Task<GateDto> GetByIdAsync(int id);
        Task<GateDto> CreateAsync(CreateGateDto createGateDto);
        Task<GateDto> UpdateAsync(int id, UpdateGateDto updateGateDto);
        Task<bool> DeleteAsync(int id, bool useSoftDelete);
    }
}
