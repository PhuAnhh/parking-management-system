using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class LedService : ILedService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LedDto>> GetAllAsync()
        {
            var leds = await _unitOfWork.Leds.GetAllAsync();
            var ledDtos = new List<LedDto>();

            foreach (var led in leds)
            {
                ledDtos.Add(new LedDto
                {
                    Id = led.Id,
                    Name = led.Name,
                    Code = led.Code,
                    Comport = led.Comport,
                    Baudrate = led.Baudrate,
                    Type = led.Type,
                    ComputerId = led.ComputerId,
                    Status = led.Status,
                    CreatedAt = led.CreatedAt,
                    UpdatedAt = led.UpdatedAt
                });
            }

            return ledDtos;
        }

        public async Task<LedDto> GetByIdAsync(int id)
        {
            var led = await _unitOfWork.Leds.GetByIdAsync(id);

            if (led == null)
                return null;

            return new LedDto
            {
                Id = led.Id,
                Name = led.Name,
                Code = led.Code,
                Comport = led.Comport,
                Baudrate = led.Baudrate,
                Type = led.Type,
                ComputerId = led.ComputerId,
                Status = led.Status,
                CreatedAt = led.CreatedAt,
                UpdatedAt = led.UpdatedAt
            };
        }

        public async Task<LedDto> CreateAsync(CreateLedDto createLedDto)
        {
            var led = new Led
            {
                Name = createLedDto.Name,
                Code = createLedDto.Code,
                Comport = createLedDto.Comport,
                Baudrate = createLedDto.Baudrate,
                Type = createLedDto.Type,
                ComputerId = createLedDto.ComputerId,
                Status = createLedDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Leds.CreateAsync(led);
            await _unitOfWork.SaveChangesAsync();

            return new LedDto
            {
                Id = led.Id,
                Name = led.Name,
                Code = led.Code,
                Comport = led.Comport,
                Baudrate = led.Baudrate,
                Type = led.Type,
                ComputerId = led.ComputerId,
                Status = led.Status,
                CreatedAt = led.CreatedAt,
                UpdatedAt = led.UpdatedAt
            };
        }

        public async Task<LedDto> UpdateAsync(int id, UpdateLedDto updateLedDto)
        {
            var led = await _unitOfWork.Leds.GetByIdAsync(id);

            if (led == null)
                return null;

            led.Name = updateLedDto.Name;
            led.Code = updateLedDto.Code;
            led.Comport = updateLedDto.Comport;
            led.Baudrate = updateLedDto.Baudrate;
            led.Type = updateLedDto.Type;
            led.ComputerId = updateLedDto.ComputerId;
            led.Status = updateLedDto.Status;
            led.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Leds.Update(led);
            await _unitOfWork.SaveChangesAsync();

            return new LedDto
            {
                Id = led.Id,
                Name = led.Name,
                Code = led.Code,
                Comport = led.Comport,
                Baudrate = led.Baudrate,
                Type = led.Type,
                ComputerId = led.ComputerId,
                Status = led.Status,
                CreatedAt = led.CreatedAt,
                UpdatedAt = DateTime.UtcNow,    
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var led = await _unitOfWork.Leds.GetByIdAsync(id);

            if (led == null)
                return false;

            _unitOfWork.Leds.Delete(led);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
