using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExitLogController : ControllerBase
    {
        private readonly IExitLogService _exitLogService;

        public ExitLogController(IExitLogService exitLogService)
        {
            _exitLogService = exitLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExitLogDto>>> GetAll()
        {
            var exitLogs = await _exitLogService.GetAllAsync();
            return Ok(exitLogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExitLogDto>> GetById(int id)
        {
            var exitLog = await _exitLogService.GetByIdAsync(id);

            if (exitLog == null)
                return NotFound();

            return Ok(exitLog);
        }

        [HttpPost]
        public async Task<ActionResult<ExitLogDto>> Create(CreateExitLogDto createExitLogDto)
        {
            try
            {
                var createdExitLog = await _exitLogService.CreateAsync(createExitLogDto);
                return CreatedAtAction(nameof(GetById), new { id = createdExitLog.Id }, createdExitLog);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
