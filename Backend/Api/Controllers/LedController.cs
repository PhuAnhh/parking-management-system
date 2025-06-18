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
    public class LedController : ControllerBase
    {
        private readonly ILedService _ledService;

        public LedController(ILedService ledService)
        {
            _ledService = ledService;
        }

        [RequirePermission("GET", "/api/led")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LedDto>>> GetAll()
        {
            var leds = await _ledService.GetAllAsync();
            return Ok(leds);
        }

        [RequirePermission("GET", "/api/led/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LedDto>> GetById(int id)
        {
            var led = await _ledService.GetByIdAsync(id);

            if (led == null)
                return NotFound();

            return Ok(led);
        }

        [RequirePermission("POST", "/api/led")]
        [HttpPost]
        public async Task<ActionResult<LedDto>> Create(CreateLedDto createLedDto)
        {
            var createdLed = await _ledService.CreateAsync(createLedDto);
            return CreatedAtAction(nameof(GetById), new { id = createdLed.Id }, createdLed);
        }

        [RequirePermission("PUT", "/api/led/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<LedDto>> Update(int id, UpdateLedDto updateLedDto)
        {
            var updatedLed = await _ledService.UpdateAsync(id, updateLedDto);

            if (updatedLed == null)
                return NotFound();

            return Ok(updatedLed);
        }

        [RequirePermission("DELETE", "/api/led/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _ledService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
