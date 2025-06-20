using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Services;
using Final_year_Project.Api.Authorization;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarningEventController : ControllerBase
    {
        private readonly IWarningEventService _warningEventService;

        public WarningEventController(IWarningEventService warningEventService)
        {
            _warningEventService = warningEventService;
        }

        [RequirePermission("GET", "/api/warningevent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarningEventDto>>> GetAll()
        {
            var warnings = await _warningEventService.GetAllAsync();
            return Ok(warnings);
        }

        [HttpGet("filter-by-date")]
        public async Task<IActionResult> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var results = await _warningEventService.GetByDateRangeAsync(fromDate, toDate);
            return Ok(results);
        }

        [RequirePermission("GET", "/api/warningevent/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<WarningEventDto>> GetById(int id)
        {
            var warning = await _warningEventService.GetByIdAsync(id);

            if (warning == null)
                return NotFound();

            return Ok(warning);
        }

        [RequirePermission("POST", "/api/warningevent")]
        [HttpPost]
        public async Task<ActionResult<WarningEventDto>> Create(CreateWarningEventDto createWarningEventDto)
        {
            var createdWarning = await _warningEventService.CreateAsync(createWarningEventDto);
            return CreatedAtAction(nameof(GetById), new { id = createdWarning.Id }, createdWarning);
        }

        [RequirePermission("DELETE", "/api/warningevent/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _warningEventService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
