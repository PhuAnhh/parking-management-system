using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Device.Persistence.DbContexts;
using Final_year_Project.Device.Application.Services.Abstractions;
using Final_year_Project.Device.Application.Models;

namespace Final_year_Project.Device.Api.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CameraDto>>> GetAll()
        {
            var cameras = await _cameraService.GetAllAsync();
            return Ok(cameras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CameraDto>> GetById(int id)
        {
            var camera = await _cameraService.GetByIdAsync(id);

            if (camera == null)
                return NotFound();

            return Ok(camera);
        }

        [HttpPost]
        public async Task<ActionResult<CameraDto>> Create(CreateCameraDto createCameraDto)
        {
            var createdCamera = await _cameraService.CreateAsync(createCameraDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCamera.Id }, createdCamera);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CameraDto>> Update(int id, UpdateCameraDto updateCameraDto)
        {
            var updatedCamera = await _cameraService.UpdateAsync(id, updateCameraDto);

            if (updatedCamera == null)
                return NotFound();

            return Ok(updatedCamera);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cameraService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
