using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Final_year_Project.Api.Authorization;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaneController : ControllerBase
    {
        private readonly ILaneService _laneService;

        public LaneController(ILaneService laneService)
        {
            _laneService = laneService;
        }

        [RequirePermission("GET", "/api/lane")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LaneDto>>> GetAll()
        {
            var lanes = await _laneService.GetAllAsync();
            return Ok(lanes);
        }

        [RequirePermission("GET", "/api/lane/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LaneDto>> GetById(int id)
        {
            var lane = await _laneService.GetByIdAsync(id);

            if (lane == null)
                return NotFound();

            return Ok(lane);
        }

        [RequirePermission("POST", "/api/lane")]
        [HttpPost]
        public async Task<ActionResult<LaneDto>> Create(CreateLaneDto createLaneDto)
        {
            var createdLane = await _laneService.CreateAsync(createLaneDto);
            return CreatedAtAction(nameof(GetById), new { id = createdLane.Id }, createdLane);
        }

        [RequirePermission("PUT", "/api/lane/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<LaneDto>> Update(int id, UpdateLaneDto updateLaneDto)
        {
            var updatedLane = await _laneService.UpdateAsync(id, updateLaneDto);

            if (updatedLane == null)
                return NotFound();

            return Ok(updatedLane);
        }

        [RequirePermission("DELETE", "/api/lane/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _laneService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}