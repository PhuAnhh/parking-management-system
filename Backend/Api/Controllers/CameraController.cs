using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Final_year_Project.Api.Authorization;
using Final_year_Project.Application.Services;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly ICameraService _cameraService;

        public CameraController(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [RequirePermission("GET", "/api/camera")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CameraDto>>> GetAll()
        {
            var cameras = await _cameraService.GetAllAsync();
            return Ok(cameras);
        }

        [RequirePermission("GET", "/api/camera/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CameraDto>> GetById(int id)
        {
            var camera = await _cameraService.GetByIdAsync(id);

            if (camera == null)
                return NotFound();

            return Ok(camera);
        }

        [RequirePermission("POST", "/api/camera")]
        [HttpPost]
        public async Task<ActionResult<CameraDto>> Create(CreateCameraDto createCameraDto)
        {
            var createdCamera = await _cameraService.CreateAsync(createCameraDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCamera.Id }, createdCamera);
        }

        [RequirePermission("PUT", "/api/camera/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CameraDto>> Update(int id, UpdateCameraDto updateCameraDto)
        {
            var updatedCamera = await _cameraService.UpdateAsync(id, updateCameraDto);

            if (updatedCamera == null)
                return NotFound();

            return Ok(updatedCamera);
        }

        [RequirePermission("DELETE", "/api/camera/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cameraService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [RequirePermission("PATCH", "/api/camera/{id}/status")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            var success = await _cameraService.ChangeStatusAsync(id, changeStatusDto.Status);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
