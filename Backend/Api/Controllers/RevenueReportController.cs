using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Services;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueReportController : ControllerBase
    {
        private readonly IRevenueReportService _revenueReportService;

        public RevenueReportController(IRevenueReportService revenueReportService)
        {
            _revenueReportService = revenueReportService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RevenueReportDto>>> GetAll()
        {
            var reports = await _revenueReportService.GetAllAsync();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RevenueReportDto>> GetById(int id)
        {
            var report = await _revenueReportService.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _revenueReportService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
