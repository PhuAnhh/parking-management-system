using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class ComputerService : IComputerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComputerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ComputerDto>> GetAllAsync()
        {
            var computers = await _unitOfWork.Computers.GetAllAsync();
            var computerDtos = new List<ComputerDto>();

            foreach (var computer in computers)
            {
                computerDtos.Add(new ComputerDto
                {
                    Id = computer.Id,
                    Name = computer.Name,
                    IpAddress = computer.IpAddress,
                    GateId = computer.GateId,
                    Status = computer.Status,
                    CreatedAt = computer.CreatedAt,
                    UpdatedAt = computer.UpdatedAt
                });
            }

            return computerDtos;
        }

        public async Task<ComputerDto> GetByIdAsync(int id)
        {
            var computer = await _unitOfWork.Computers.GetByIdAsync(id);

            if (computer == null)
                return null;

            return new ComputerDto
            {
                Id = computer.Id,
                Name = computer.Name,
                IpAddress = computer.IpAddress,
                GateId = computer.GateId,
                Status = computer.Status,
                CreatedAt = computer.CreatedAt,
                UpdatedAt = computer.UpdatedAt
            };
        }

        public async Task<ComputerDto> CreateAsync(CreateComputerDto createComputerDto)
        {
            var computer = new Computer
            {
                Name = createComputerDto.Name,
                IpAddress = createComputerDto.IpAddress,
                GateId = createComputerDto.GateId,
                Status = createComputerDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Computers.CreateAsync(computer);
            await _unitOfWork.SaveChangesAsync();

            return new ComputerDto
            {
                Id = computer.Id,
                Name = computer.Name,
                IpAddress = computer.IpAddress,
                GateId = computer.GateId,
                Status = computer.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<ComputerDto> UpdateAsync(int id, UpdateComputerDto updateComputerDto)
        {
            var computer = await _unitOfWork.Computers.GetByIdAsync(id);

            if (computer == null)
                return null;

            computer.Name = updateComputerDto.Name;
            computer.IpAddress = updateComputerDto.IpAddress;
            computer.GateId = updateComputerDto.GateId;
            computer.Status = updateComputerDto.Status;
            computer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Computers.Update(computer);
            await _unitOfWork.SaveChangesAsync();

            return new ComputerDto
            {
                Id = computer.Id,
                Name = computer.Name,
                IpAddress = computer.IpAddress,
                GateId = computer.GateId,
                Status = computer.Status,
                CreatedAt = computer.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(int id, bool useSoftDelete)
        {
            try
            {
                var computer = await _unitOfWork.Computers.GetByIdAsync(id);
                if (computer == null) return false;

                if (useSoftDelete)
                {
                    computer.Deleted = true;
                    computer.UpdatedAt = DateTime.UtcNow;

                    //if (computer.Cameras != null && computer.Cameras.Any())
                    //{
                    //    foreach (var camera in computer.Cameras)
                    //    {
                    //        camera.Deleted = true;
                    //        camera.UpdatedAt = DateTime.UtcNow;
                    //        _unitOfWork.Cameras.Update(camera);  
                    //    }
                    //}

                    //// Soft delete cho tất cả control_units liên quan
                    //if (computer.ControlUnits != null && computer.ControlUnits.Any())
                    //{
                    //    foreach (var controlUnit in computer.ControlUnits)
                    //    {
                    //        controlUnit.Deleted = true;
                    //        controlUnit.UpdatedAt = DateTime.UtcNow;
                    //        _unitOfWork.ControlUnits.Update(controlUnit); 
                    //    }
                    //}

                    //if (computer.Lanes != null && computer.Lanes.Any())
                    //{
                    //    foreach (var lane in computer.Lanes)
                    //    {
                    //        lane.Deleted = true;
                    //        lane.UpdatedAt = DateTime.UtcNow;
                    //        _unitOfWork.Lanes.Update(lane);  // Cập nhật lane
                    //    }
                    //}

                    //if (computer.Leds != null && computer.Leds.Any())
                    //{
                    //    foreach (var led in computer.Leds)
                    //    {
                    //        led.Deleted = true;
                    //        led.UpdatedAt = DateTime.UtcNow;
                    //        _unitOfWork.Leds.Update(led);  // Cập nhật led
                    //    }
                    //}

                    _unitOfWork.Computers.Update(computer);
                }
                else
                {

                    if (computer.Cameras != null && computer.Cameras.Any())
                    {
                        foreach (var camera in computer.Cameras)
                        {
                            _unitOfWork.Cameras.Delete(camera);  // Xóa camera
                        }
                    }

                    if (computer.ControlUnits != null && computer.ControlUnits.Any())
                    {
                        foreach (var controlUnit in computer.ControlUnits)
                        {
                            _unitOfWork.ControlUnits.Delete(controlUnit);  // Xóa control_unit
                        }
                    }

                    if (computer.Lanes != null && computer.Lanes.Any())
                    {
                        foreach (var lane in computer.Lanes)
                        {
                            _unitOfWork.Lanes.Delete(lane);  // Xóa lane
                        }
                    }

                    if (computer.Leds != null && computer.Leds.Any())
                    {
                        foreach (var led in computer.Leds)
                        {
                            _unitOfWork.Leds.Delete(led);  // Xóa led
                        }
                    }

                    // Xóa computer
                    _unitOfWork.Computers.Delete(computer);
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeStatusAsync(int id, bool status)
        {
            var computer = await _unitOfWork.Computers.GetByIdAsync(id);
            if (computer == null) return false;

            computer.Status = status;
            computer.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Computers.Update(computer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
