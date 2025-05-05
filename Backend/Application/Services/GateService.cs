using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;

namespace Final_year_Project.Application.Services
{
    public class GateService : IGateService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GateDto>> GetAllAsync()
        {
            var gates = await _unitOfWork.Gates.GetAllAsync();
            var gateDtos = new List<GateDto>();

            foreach (var gate in gates)
            {
                gateDtos.Add(new GateDto
                {
                    Id = gate.Id,
                    Name = gate.Name,
                    Code = gate.Code,
                    Status = gate.Status,
                    CreatedAt = gate.CreatedAt,
                    UpdatedAt = gate.UpdatedAt
                });
            }

            return gateDtos;
        }

        public async Task<GateDto> GetByIdAsync(int id)
        {
            var gate = await _unitOfWork.Gates.GetByIdAsync(id);

            if (gate == null)
                return null;

            return new GateDto
            {
                Id = gate.Id,
                Name = gate.Name,
                Code= gate.Code,
                Status = gate.Status,
                CreatedAt = gate.CreatedAt,
                UpdatedAt = gate.UpdatedAt
            };
        }

        public async Task<GateDto> CreateAsync(CreateGateDto createGateDto)
        {
            var gate = new Gate
            {
                Name = createGateDto.Name,
                Code = createGateDto.Code,
                Status = createGateDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Gates.CreateAsync(gate);
            await _unitOfWork.SaveChangesAsync();

            return new GateDto
            {
                Id = gate.Id,
                Name = gate.Name,
                Code = gate.Code,
                Status = gate.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<GateDto> UpdateAsync(int id, UpdateGateDto updateGateDto)
        {
            var gate = await _unitOfWork.Gates.GetByIdAsync(id);

            if (gate == null)
                return null;

            gate.Name = updateGateDto.Name;
            gate.Code = updateGateDto.Code;
            gate.Status = updateGateDto.Status;
            gate.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Gates.Update(gate);
            await _unitOfWork.SaveChangesAsync();

            return new GateDto
            {
                Id = gate.Id,
                Name = gate.Name,
                Code = gate.Code,
                Status = gate.Status,
                CreatedAt = gate.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(int id, bool useSoftDelete = true)
        {
            try
            {
                var gate = await _unitOfWork.Gates.GetByIdAsync(id);
                if (gate == null) return false;

                if (useSoftDelete)
                {
                    gate.Deleted = true;
                    gate.UpdatedAt = DateTime.UtcNow;

                    //if (gate.Computers != null && gate.Computers.Any())
                    //{
                    //    foreach (var computer in gate.Computers)
                    //    {
                    //        computer.Deleted = true;
                    //        computer.UpdatedAt = DateTime.UtcNow;
                    //        _unitOfWork.Computers.Update(computer);
                    //    }
                    //}

                    _unitOfWork.Gates.Update(gate);
                }
                else
                {
                    if (gate.Computers != null && gate.Computers.Any())
                    {
                        foreach (var computer in gate.Computers)
                        {
                            _unitOfWork.Computers.Delete(computer);
                        }
                    }
                    _unitOfWork.Gates.Delete(gate);
                }
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
