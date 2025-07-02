using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Application.Services.Abstractions
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> GetAllAsync();
        Task<CardDto> GetByIdAsync(int id);
        Task<CardDto> CreateAsync(CreateCardDto createCardDto);
        Task<CardDto> UpdateAsync(int id, UpdateCardDto updateCardDto);
        Task<bool> DeleteAsync(int id);
    }
}
