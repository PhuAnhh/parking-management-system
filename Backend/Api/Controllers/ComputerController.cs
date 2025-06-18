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
    public class ComputerController : ControllerBase
    {
        private readonly IComputerService _computerService;

        public ComputerController(IComputerService computerService)
        {
            _computerService = computerService;
        }

        [RequirePermission("GET", "/api/computer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComputerDto>>> GetAll()
        {
            var computers = await _computerService.GetAllAsync();
            return Ok(computers);
        }

        [RequirePermission("GET", "/api/computer/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ComputerDto>> GetById(int id)
        {
            var computer = await _computerService.GetByIdAsync(id);

            if (computer == null)
                return NotFound();

            return Ok(computer);
        }

        [RequirePermission("POST", "/api/computer")]
        [HttpPost]
        public async Task<ActionResult<ComputerDto>> Create(CreateComputerDto createComputerDto)
        {
            var createdComputer = await _computerService.CreateAsync(createComputerDto);
            return CreatedAtAction(nameof(GetById), new { id = createdComputer.Id }, createdComputer);
        }

        [RequirePermission("PUT", "/api/computer/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ComputerDto>> Update(int id, UpdateComputerDto updateComputerDto)
        {
            var updatedComputer = await _computerService.UpdateAsync(id, updateComputerDto);

            if (updatedComputer == null)
                return NotFound();

            return Ok(updatedComputer);
        }

        [RequirePermission("DELETE", "/api/computer/{id}")]
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
