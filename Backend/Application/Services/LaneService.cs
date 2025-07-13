using Final_year_Project.Domain.Entities;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Final_year_Project.Application.Services
{
    public class LaneService : ILaneService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LaneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LaneDto>> GetAllAsync()
        {
            var lanes = await _unitOfWork.Lanes.GetAllAsync();
            var laneDtos = new List<LaneDto>();

            foreach (var lane in lanes)
            {
                var laneDto = new LaneDto
                {
                    Id = lane.Id,
                    Name = lane.Name,
                    Code = lane.Code,
                    Type = lane.Type,
                    ReverseLane = lane.ReverseLane,
                    ComputerId = lane.ComputerId,
                    AutoOpenBarrier = lane.AutoOpenBarrier,
                    Loop = lane.Loop,
                    DisplayLed = lane.DisplayLed,
                    Status = lane.Status,
                    CreatedAt = lane.CreatedAt,
                    UpdatedAt = lane.UpdatedAt,
                    LaneCameras = lane.LaneCameras.Select(camera => new LaneCameraDto
                    {
                        Id = camera.Id,
                        LaneId = lane.Id,
                        CameraId = camera.CameraId,
                        Purpose = camera.Purpose,
                        DisplayPosition = camera.DisplayPosition,
                        CreatedAt = camera.CreatedAt,
                        UpdatedAt = camera.UpdatedAt,
                    }).ToList(),
                    LaneControlUnits = lane.LaneControlUnits.Select(controlUnit => new LaneControlUnitDto
                    {
                        Id = controlUnit.Id,
                        LaneId = lane.Id,
                        ControlUnitId = controlUnit.ControlUnitId,
                        Reader = controlUnit.Reader,
                        Input = controlUnit.Input,
                        Barrier = controlUnit.Barrier,
                        Alarm = controlUnit.Alarm,
                        CreatedAt = controlUnit.CreatedAt,
                        UpdatedAt = controlUnit.UpdatedAt
                    }).ToList(),
                };
                laneDtos.Add(laneDto);
            }

            return laneDtos;
        }

        public async Task<LaneDto> GetByIdAsync(int id)
        {
            var lane = await _unitOfWork.Lanes.GetByIdAsync(id);

            if (lane == null)
                return null;

            return new LaneDto
            {
                Id = lane.Id,
                Name = lane.Name,
                Code = lane.Code,
                Type = lane.Type,
                ReverseLane = lane.ReverseLane,
                ComputerId = lane.ComputerId,
                AutoOpenBarrier = lane.AutoOpenBarrier,
                Loop = lane.Loop,
                Status = lane.Status,
                CreatedAt = lane.CreatedAt,
                UpdatedAt = lane.UpdatedAt,
                LaneCameras = lane.LaneCameras.Select(camera => new LaneCameraDto
                {
                    Id = camera.CameraId,
                    LaneId = lane.Id,
                    CameraId = camera.CameraId,
                    Purpose = camera.Purpose,
                    DisplayPosition = camera.DisplayPosition,
                    CreatedAt = camera.CreatedAt,
                    UpdatedAt = camera.UpdatedAt
                }).ToList(),
                LaneControlUnits = lane.LaneControlUnits.Select(controlUnit => new LaneControlUnitDto
                {
                    Id = controlUnit.Id,
                    LaneId = lane.Id,
                    ControlUnitId = controlUnit.ControlUnitId,
                    Reader = controlUnit.Reader,
                    Input = controlUnit.Input,
                    Barrier = controlUnit.Barrier,
                    Alarm = controlUnit.Alarm,
                    CreatedAt = controlUnit.CreatedAt,
                    UpdatedAt = controlUnit.UpdatedAt
                }).ToList()
            };
        }

        public async Task<LaneDto> CreateAsync(CreateLaneDto createLaneDto)
        {
            var lane = new Lane
            {
                Name = createLaneDto.Name,
                Code = createLaneDto.Code,
                Type = createLaneDto.Type,
                ReverseLane = createLaneDto.ReverseLane,
                ComputerId = createLaneDto.ComputerId,
                AutoOpenBarrier = createLaneDto.AutoOpenBarrier,
                Loop = createLaneDto.Loop,
                DisplayLed = createLaneDto.DisplayLed,
                Status = createLaneDto.Status,
            };

            await _unitOfWork.Lanes.CreateAsync(lane);
            await _unitOfWork.SaveChangesAsync();

            foreach (var cameraDto in createLaneDto.LaneCameras)
            {
                var laneCamera = new LaneCamera
                {
                    CameraId = cameraDto.CameraId,
                    Purpose = cameraDto.Purpose,
                    DisplayPosition = cameraDto.DisplayPosition,
                    LaneId = lane.Id
                };
                await _unitOfWork.LaneCameras.CreateAsync(laneCamera);
            } 

            foreach (var controlUnitDto in createLaneDto.LaneControlUnits)
            {
                var laneControlUnit = new LaneControlUnit
                {
                    ControlUnitId = controlUnitDto.ControlUnitId,
                    Reader = controlUnitDto.Reader,
                    Input = controlUnitDto.Input,
                    Barrier = controlUnitDto.Barrier,
                    Alarm = controlUnitDto.Alarm,
                    LaneId = lane.Id
                };
                await _unitOfWork.LaneControlUnits.CreateAsync(laneControlUnit);
            }    
                
            await _unitOfWork.SaveChangesAsync();

            return new LaneDto
            {
                Id = lane.Id,
                Name = lane.Name,
                Code = lane.Code,
                Type = lane.Type,
                ReverseLane = lane.ReverseLane,
                ComputerId = lane.ComputerId,
                AutoOpenBarrier = lane.AutoOpenBarrier,
                Loop = lane.Loop,
                DisplayLed = lane.DisplayLed,
                Status = lane.Status,
                CreatedAt = lane.CreatedAt,
                UpdatedAt = lane.UpdatedAt,
                LaneCameras = lane.LaneCameras.Select(camera => new LaneCameraDto
                {
                    Id = camera.Id,
                    LaneId = lane.Id,
                    CameraId = camera.CameraId,
                    Purpose = camera.Purpose,
                    DisplayPosition = camera.DisplayPosition,
                    CreatedAt = camera.CreatedAt,
                    UpdatedAt = camera.UpdatedAt
                }).ToList(),
                LaneControlUnits = lane.LaneControlUnits.Select(controlUnit => new LaneControlUnitDto
                {
                    Id = controlUnit.Id,
                    LaneId = lane.Id,
                    ControlUnitId = controlUnit.ControlUnitId,
                    Reader = controlUnit.Reader,
                    Input = controlUnit.Input,
                    Barrier = controlUnit.Barrier,
                    Alarm = controlUnit.Alarm,
                    CreatedAt = controlUnit.CreatedAt,
                    UpdatedAt = controlUnit.UpdatedAt,
                }).ToList(),
            };
        }

        public async Task<LaneDto> UpdateAsync(int id, UpdateLaneDto updateLaneDto)
        {
            var lane = await _unitOfWork.Lanes.GetByIdAsync(id);

            if (lane == null)
                return null;

            lane.Name = updateLaneDto.Name;
            lane.Code = updateLaneDto.Code;
            lane.Type = updateLaneDto.Type;
            lane.ReverseLane = updateLaneDto.ReverseLane;
            lane.ComputerId = updateLaneDto.ComputerId;
            lane.AutoOpenBarrier = updateLaneDto.AutoOpenBarrier;
            lane.Loop = updateLaneDto.Loop;
            lane.DisplayLed = updateLaneDto.DisplayLed;
            lane.Status = updateLaneDto.Status;
            lane.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Lanes.Update(lane);

            var allCameras = await _unitOfWork.LaneCameras.GetAllAsync();
            var laneCameras = allCameras.Where(c => c.LaneId == id).ToList();

            foreach (var camera in laneCameras)
            {
                _unitOfWork.LaneCameras.Delete(camera);
            }

            foreach (var cameraDto in updateLaneDto.LaneCameras)
            {
                var laneCamera = new LaneCamera
                {
                    CameraId = cameraDto.CameraId,
                    Purpose = cameraDto.Purpose,
                    DisplayPosition = cameraDto.DisplayPosition,
                    LaneId = lane.Id,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.LaneCameras.CreateAsync(laneCamera);
            }

            var allControlUnits = await _unitOfWork.LaneControlUnits.GetAllAsync();
            var laneControlUnits = allControlUnits.Where(cu => cu.LaneId == id).ToList();

            foreach (var controlUnit in laneControlUnits)
            {
                _unitOfWork.LaneControlUnits.Delete(controlUnit);
            }

            foreach (var controlUnitDto in updateLaneDto.LaneControlUnits)
            {
                var laneControlUnit = new LaneControlUnit
                {
                    ControlUnitId = controlUnitDto.ControlUnitId,
                    Reader = controlUnitDto.Reader,
                    Input = controlUnitDto.Input,
                    Barrier = controlUnitDto.Barrier,
                    Alarm = controlUnitDto.Alarm,
                    LaneId = lane.Id,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.LaneControlUnits.CreateAsync(laneControlUnit);
            }

            await _unitOfWork.SaveChangesAsync();

            lane = await _unitOfWork.Lanes.GetByIdAsync(id);

            var updatedCameras = (await _unitOfWork.LaneCameras.GetAllAsync())
                .Where(c => c.LaneId == id)
                .ToList();

            var updatedControlUnits = (await _unitOfWork.LaneControlUnits.GetAllAsync())
                .Where(cu => cu.LaneId == id)
                .ToList();

            return new LaneDto
            {
                Id = lane.Id,
                Name = lane.Name,
                Code = lane.Code,
                Type = lane.Type,
                ReverseLane = lane.ReverseLane,
                ComputerId = lane.ComputerId,
                AutoOpenBarrier = lane.AutoOpenBarrier,
                Loop = lane.Loop,
                DisplayLed = lane.DisplayLed,
                Status = lane.Status,
                CreatedAt = lane.CreatedAt,
                UpdatedAt = lane.UpdatedAt,
                LaneCameras = updatedCameras.Select(camera => new LaneCameraDto
                {
                    Id = camera.Id,
                    LaneId = lane.Id,
                    CameraId = camera.CameraId,
                    Purpose = camera.Purpose,
                    DisplayPosition = camera.DisplayPosition,
                    CreatedAt = camera.CreatedAt,
                    UpdatedAt = camera.UpdatedAt
                }).ToList(),
                LaneControlUnits = updatedControlUnits.Select(controlUnit => new LaneControlUnitDto
                {
                    Id = controlUnit.Id,
                    LaneId = lane.Id,
                    ControlUnitId = controlUnit.ControlUnitId,
                    Reader = controlUnit.Reader,
                    Input = controlUnit.Input,
                    Barrier = controlUnit.Barrier,
                    Alarm = controlUnit.Alarm,
                    CreatedAt = controlUnit.CreatedAt,
                    UpdatedAt = controlUnit.UpdatedAt,
                }).ToList(),
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lane = await _unitOfWork.Lanes.GetByIdAsync(id);

            if (lane == null)
                return false;

            lane.Deleted = true;
            _unitOfWork.Lanes.Update(lane);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, bool status)
        {
            var lane = await _unitOfWork.Lanes.GetByIdAsync(id);
            if (lane == null) return false;

            lane.Status = status;
            lane.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Lanes.Update(lane);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
