using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class WarningEventService : IWarningEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WarningEventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WarningEventDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var warnings = await _unitOfWork.WarningEvents.GetByDateRangeAsync(fromDate, toDate);
            var warningDtos = new List<WarningEventDto>();

            foreach (var warning in warnings)
            {
                warningDtos.Add(new WarningEventDto
                {
                    Id = warning.Id,
                    PlateNumber = warning.PlateNumber,
                    LaneId = warning.LaneId,
                    WarningType = warning.WarningType,
                    Note = warning.Note,
                    CreatedAt = warning.CreatedAt,
                    ImageUrl = warning.ImageUrl,
                });
            }

            return warningDtos;
        }

        public async Task<IEnumerable<WarningEventDto>> GetAllAsync()
        {
            var warnings = await _unitOfWork.WarningEvents.GetAllAsync();
            var warningDtos = new List<WarningEventDto>();

            foreach (var warning in warnings)
            {
                warningDtos.Add(new WarningEventDto
                {
                    Id = warning.Id,
                    PlateNumber = warning.PlateNumber,
                    LaneId = warning.LaneId,
                    WarningType = warning.WarningType,
                    Note = warning.Note,
                    CreatedAt = warning.CreatedAt,
                    ImageUrl = warning.ImageUrl,
                });
            }

            return warningDtos;
        }

        public async Task<WarningEventDto> GetByIdAsync(int id)
        {
            var warning = await _unitOfWork.WarningEvents.GetByIdAsync(id);

            if (warning == null)
                return null;

            return new WarningEventDto
            {
                Id = warning.Id,
                PlateNumber = warning.PlateNumber,
                LaneId = warning.LaneId,
                WarningType = warning.WarningType,
                Note = warning.Note,
                CreatedAt = warning.CreatedAt,
                ImageUrl = warning.ImageUrl,
            };
        }

        public async Task<WarningEventDto> CreateAsync(CreateWarningEventDto createWarningEventDto)
        {
            var warning = new WarningEvent
            {
                PlateNumber = createWarningEventDto.PlateNumber,
                LaneId = createWarningEventDto.LaneId,
                WarningType = createWarningEventDto.WarningType,
                Note = createWarningEventDto.Note,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = createWarningEventDto.ImageUrl,
            };

            await _unitOfWork.WarningEvents.CreateAsync(warning);
            await _unitOfWork.SaveChangesAsync();

            return new WarningEventDto
            {
                Id = warning.Id,
                PlateNumber = warning.PlateNumber,
                LaneId = warning.LaneId,
                WarningType = warning.WarningType,
                Note = warning.Note,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = warning.ImageUrl,
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var warning = await _unitOfWork.WarningEvents.GetByIdAsync(id);

            if (warning == null)
                return false;

            _unitOfWork.WarningEvents.Delete(warning);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
