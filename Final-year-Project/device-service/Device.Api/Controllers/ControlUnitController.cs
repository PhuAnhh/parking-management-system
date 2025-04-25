using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Device.Persistence.DbContexts;
using Final_year_Project.Device.Application.Services.Abstractions;
using Final_year_Project.Device.Application.Models;

namespace Final_year_Project.Device.Api.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControlUnitDto>>> GetAll()
        {
            var controlUnits = await _controlUnitService.GetAllAsync();
            return Ok(controlUnits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ControlUnitDto>> GetById(int id)
        {
            var controlUnit = await _controlUnitService.GetByIdAsync(id);

            if (controlUnit == null)
                return NotFound();

            return Ok(controlUnit);
        }

        [HttpPost]
        public async Task<ActionResult<ControlUnitDto>> Create(CreateControlUnitDto createControlUnitDto)
        {
            var createdControlUnit = await _controlUnitService.CreateAsync(createControlUnitDto);
            return CreatedAtAction(nameof(GetById), new { id = createdControlUnit.Id }, createdControlUnit);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ControlUnitDto>> Update(int id, UpdateControlUnitDto updateControlUnitDto)
        {
            var updatedControlUnit = await _controlUnitService.UpdateAsync(id, updateControlUnitDto);

            if (updatedControlUnit == null)
                return NotFound();

            return Ok(updatedControlUnit);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _controlUnitService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
