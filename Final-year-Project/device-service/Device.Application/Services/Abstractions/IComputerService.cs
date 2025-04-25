using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services.Abstractions
{
    public interface IComputerService
    {
        Task<IEnumerable<ComputerDto>> GetAllAsync();
        Task<ComputerDto> GetByIdAsync(int id);
        Task<ComputerDto> CreateAsync(CreateComputerDto createComputerDto);
        Task<ComputerDto> UpdateAsync(int id, UpdateComputerDto updateComputerDto);
        Task<bool> DeleteAsync(int id, bool useSoftDelete);
    }
}
