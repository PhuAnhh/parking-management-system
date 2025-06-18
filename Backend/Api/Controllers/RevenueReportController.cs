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
    public class RevenueReportController : ControllerBase
    {
        private readonly IRevenueReportService _revenueReportService;

        public RevenueReportController(IRevenueReportService revenueReportService)
        {
            _revenueReportService = revenueReportService;
        }

        [RequirePermission("GET", "/api/revenue-report")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RevenueReportDto>>> GetAll()
        {
            var reports = await _revenueReportService.GetAllAsync();
            return Ok(reports);
        }

        [RequirePermission("GET", "/api/revenue-report/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RevenueReportDto>> GetById(int id)
        {
            var report = await _revenueReportService.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            return Ok(report);
        }

        [RequirePermission("GET", "/api/revenue-report/filter-by-date")]
        [HttpGet("filter-by-date")]
        public async Task<IActionResult> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var results = await _revenueReportService.GetByDateRangeAsync(fromDate, toDate);
            return Ok(results);
        }

        [RequirePermission("DELETE", "/api/revenue-report/{id}")]
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
