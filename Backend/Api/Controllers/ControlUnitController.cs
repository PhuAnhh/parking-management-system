using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Final_year_Project.Api.Authorization;
using Final_year_Project.Application.Services;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlUnitController : ControllerBase
    {
        private readonly IControlUnitService _controlUnitService;

        public ControlUnitController(IControlUnitService controlUnitService)
        {
            _controlUnitService = controlUnitService;
        }

        [RequirePermission("GET", "/api/controlunit")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControlUnitDto>>> GetAll()
        {
            var controlUnits = await _controlUnitService.GetAllAsync();
            return Ok(controlUnits);
        }

        [RequirePermission("GET", "/api/controlunit/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ControlUnitDto>> GetById(int id)
        {
            var controlUnit = await _controlUnitService.GetByIdAsync(id);

            if (controlUnit == null)
                return NotFound();

            return Ok(controlUnit);
        }

        [RequirePermission("POST", "/api/controlunit")]
        [HttpPost]
        public async Task<ActionResult<ControlUnitDto>> Create(CreateControlUnitDto createControlUnitDto)
        {
            var createdControlUnit = await _controlUnitService.CreateAsync(createControlUnitDto);
            return CreatedAtAction(nameof(GetById), new { id = createdControlUnit.Id }, createdControlUnit);
        }

        [RequirePermission("PUT", "/api/controlunit/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ControlUnitDto>> Update(int id, UpdateControlUnitDto updateControlUnitDto)
        {
            var updatedControlUnit = await _controlUnitService.UpdateAsync(id, updateControlUnitDto);

            if (updatedControlUnit == null)
                return NotFound();

            return Ok(updatedControlUnit);
        }

        [RequirePermission("DELETE", "/api/controlunit/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _controlUnitService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [RequirePermission("PATCH", "/api/controlunit/{id}/status")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            var success = await _controlUnitService.ChangeStatusAsync(id, changeStatusDto.Status);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
