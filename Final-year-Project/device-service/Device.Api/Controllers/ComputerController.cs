using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Device.Persistence.DbContexts;
using Final_year_Project.Device.Application.Services.Abstractions;
using Final_year_Project.Device.Application.Models;

namespace Final_year_Project.Device.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        private readonly IComputerService _computerService;

        public ComputerController(IComputerService computerService)
        {
            _computerService = computerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComputerDto>>> GetAll()
        {
            var computers = await _computerService.GetAllAsync();
            return Ok(computers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComputerDto>> GetById(int id)
        {
            var computer = await _computerService.GetByIdAsync(id);

            if (computer == null)
                return NotFound();

            return Ok(computer);
        }

        [HttpPost]
        public async Task<ActionResult<ComputerDto>> Create(CreateComputerDto createComputerDto)
        {
            var createdComputer = await _computerService.CreateAsync(createComputerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdComputer.Id }, createdComputer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ComputerDto>> Update(int id, UpdateComputerDto updateComputerDto)
        {
            var updatedComputer = await _computerService.UpdateAsync(id, updateComputerDto);

            if (updatedComputer == null)
                return NotFound();

            return Ok(updatedComputer);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _computerService.DeleteAsync(id, true);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
