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
    public class EntryLogController : ControllerBase
    {
        private readonly IEntryLogService _entryLogService;

        public EntryLogController(IEntryLogService entryLogService)
        {
            _entryLogService = entryLogService;
        }

        [RequirePermission("GET", "/api/entrylog")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryLogDto>>> GetAll()
        {
            var entryLogs = await _entryLogService.GetAllAsync();
            return Ok(entryLogs);
        }

        [RequirePermission("GET", "/api/entrylog/filter-by-date")]
        [HttpGet("filter-by-date")]
        public async Task<IActionResult> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var results = await _entryLogService.GetByDateRangeAsync(fromDate, toDate);
            return Ok(results);
        }

        [RequirePermission("GET", "/api/entrylog/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<EntryLogDto>> GetById(int id)
        {
            var entryLog = await _entryLogService.GetByIdAsync(id);

            if (entryLog == null)
                return NotFound();

            return Ok(entryLog);
        }

        [RequirePermission("POST", "/api/entrylog")]
        [HttpPost]
        public async Task<ActionResult<EntryLogDto>> Create(CreateEntryLogDto createEntryLogDto)
        {
            try
            {
                var createdEntryLog = await _entryLogService.CreateAsync(createEntryLogDto);
                return CreatedAtAction(nameof(GetById), new { id = createdEntryLog.Id }, createdEntryLog);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [RequirePermission("DELETE", "/api/entrylog/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _entryLogService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
