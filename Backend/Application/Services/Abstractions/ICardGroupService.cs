using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ICardGroupService
    {
        Task<IEnumerable<CardGroupDto>> GetAllAsync();
        Task<CardGroupDto> GetByIdAsync(int id);
        Task<CardGroupDto> CreateAsync(CreateCardGroupDto createCardGroupDto);
        Task<CardGroupDto> UpdateAsync(int id, UpdateCardGroupDto updateCardGroupDto);
        Task<bool> DeleteAsync(int id, bool useSoftDelete);
        Task<bool> ChangeStatusAsync(int id, bool status);
    }
}
