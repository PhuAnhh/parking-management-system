using Final_year_Project.Device.Domain.Entities;
using Final_year_Project.Device.Application.Models;
using Final_year_Project.Device.Application.Repositories;
using Final_year_Project.Device.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Device.Application.Services
{
    public class ControlUnitService : IControlUnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ControlUnitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ControlUnitDto>> GetAllAsync()
        {
            var controlUnits = await _unitOfWork.ControlUnits.GetAllAsync();
            var controlUnitDtos = new List<ControlUnitDto>();

            foreach (var controlUnit in controlUnits)
            {
                controlUnitDtos.Add(new ControlUnitDto
                {
                    Id = controlUnit.Id,
                    Name = controlUnit.Name,
                    Code = controlUnit.Code,
                    Username = controlUnit.Username,
                    Password = controlUnit.Password,
                    Comport = controlUnit.Comport,
                    Baudrate = controlUnit.Baudrate,
                    Type = controlUnit.Type,
                    ConnectionProtocol = controlUnit.ConnectionProtocol,
                    ComputerId = controlUnit.ComputerId,
                    Status = controlUnit.Status,
                    CreatedAt = controlUnit.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return controlUnitDtos;
        }

        public async Task<ControlUnitDto> GetByIdAsync(int id)
        {
            var controlUnit = await _unitOfWork.ControlUnits.GetByIdAsync(id);

            if (controlUnit == null)
                return null;

            return new ControlUnitDto
            {
                Id = controlUnit.Id,
                Name = controlUnit.Name,
                Code = controlUnit.Code,
                Username = controlUnit.Username,
                Password = controlUnit.Password,
                Comport = controlUnit.Comport,
                Baudrate = controlUnit.Baudrate,
                Type = controlUnit.Type,
                ConnectionProtocol = controlUnit.ConnectionProtocol,
                ComputerId = controlUnit.ComputerId,
                Status = controlUnit.Status,
                CreatedAt = controlUnit.CreatedAt,
                UpdatedAt = controlUnit.UpdatedAt
            };
        }

        public async Task<ControlUnitDto> CreateAsync(CreateControlUnitDto createControlUnitDto)
        {
            var controlUnit = new ControlUnit
            {
                Name = createControlUnitDto.Name,
                Code = createControlUnitDto.Code,
                Username = createControlUnitDto.Username,
                Password = createControlUnitDto.Password,
                Comport = createControlUnitDto.Comport,
                Baudrate = createControlUnitDto.Baudrate,
                Type = createControlUnitDto.Type,
                ConnectionProtocol = createControlUnitDto.ConnectionProtocol,
                ComputerId = createControlUnitDto.ComputerId,
                Status= createControlUnitDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ControlUnits.CreateAsync(controlUnit);
            await _unitOfWork.SaveChangesAsync();

            return new ControlUnitDto
            {
                Id = controlUnit.Id,
                Name = controlUnit.Name,
                Code = controlUnit.Code,
                Username = controlUnit.Username,
                Password = controlUnit.Password,
                Comport = controlUnit.Comport,
                Baudrate = controlUnit.Baudrate,
                Type = controlUnit.Type,
                ConnectionProtocol = controlUnit.ConnectionProtocol,
                ComputerId = controlUnit.ComputerId,
                Status = controlUnit.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<ControlUnitDto> UpdateAsync(int id, UpdateControlUnitDto updateControlUnitDto)
        {
            var controlUnit = await _unitOfWork.ControlUnits.GetByIdAsync(id);

            if (controlUnit == null)
                return null;

            controlUnit.Name = updateControlUnitDto.Name;
            controlUnit.Code = updateControlUnitDto.Code;
            controlUnit.Username = updateControlUnitDto.Username;
            controlUnit.Password = updateControlUnitDto.Password;
            controlUnit.Comport = updateControlUnitDto.Comport;
            controlUnit.Baudrate = updateControlUnitDto.Baudrate;
            controlUnit.Type = updateControlUnitDto.Type;
            controlUnit.ConnectionProtocol = updateControlUnitDto.ConnectionProtocol;
            controlUnit.ComputerId = updateControlUnitDto.ComputerId;
            controlUnit.Status = updateControlUnitDto.Status;
            controlUnit.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ControlUnits.Update(controlUnit);
            await _unitOfWork.SaveChangesAsync();

            return new ControlUnitDto
            {
                Id = controlUnit.Id,
                Name = controlUnit.Name,
                Code = controlUnit.Code,
                Username = controlUnit.Username,
                Password = controlUnit.Password,
                Comport = controlUnit.Comport,
                Baudrate = controlUnit.Baudrate,
                Type = controlUnit.Type,
                ConnectionProtocol = controlUnit.ConnectionProtocol,
                ComputerId = controlUnit.ComputerId,
                Status = controlUnit.Status,
                CreatedAt = controlUnit.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var controlUnit = await _unitOfWork.ControlUnits.GetByIdAsync(id);

            if (controlUnit == null)
                return false;

            _unitOfWork.ControlUnits.Delete(controlUnit);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
