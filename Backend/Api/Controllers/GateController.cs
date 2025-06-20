using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Final_year_Project.Api.Authorization;

namespace Final_year_Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GateController : ControllerBase
    {
        private readonly IGateService _gateService;

        public GateController(IGateService gateService)
        {
            _gateService = gateService;
        }

        [RequirePermission("GET", "/api/gate")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GateDto>>> GetAll()
        {
            var gates = await _gateService.GetAllAsync();
            return Ok(gates);
        }

        [RequirePermission("GET", "/api/gate/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GateDto>> GetById(int id)
        {
            var gate = await _gateService.GetByIdAsync(id);

            if (gate == null)
                return NotFound();

            return Ok(gate);
        }

        [RequirePermission("POST", "/api/gate")]
        [HttpPost]
        public async Task<ActionResult<GateDto>> Create(CreateGateDto createGateDto)
        {
            var createdGate = await _gateService.CreateAsync(createGateDto);
            return CreatedAtAction(nameof(GetById), new { id = createdGate.Id }, createdGate);
        }

        [RequirePermission("PUT", "/api/gate/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GateDto>> Update(int id, UpdateGateDto updateGateDto)
        {
            var updatedGate = await _gateService.UpdateAsync(id, updateGateDto);

            if (updatedGate == null)
                return NotFound();

            return Ok(updatedGate);
        }

        [RequirePermission("DELETE", "/api/gate/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _gateService.DeleteAsync(id, true);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [RequirePermission("PATCH", "/api/gate/{id}/status")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            var success = await _gateService.ChangeStatusAsync(id, changeStatusDto.Status);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
