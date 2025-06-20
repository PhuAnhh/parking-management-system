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
    public class CardGroupController : ControllerBase
    {
        private readonly ICardGroupService _cardGroupService;

        public CardGroupController(ICardGroupService cardGroupService)
        {
            _cardGroupService = cardGroupService;
        }

        [RequirePermission("GET", "/api/cardgroup")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardGroupDto>>> GetAll()
        {
            var cardGroups = await _cardGroupService.GetAllAsync();
            return Ok(cardGroups);
        }

        [RequirePermission("GET", "/api/cardgroup/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CardGroupDto>> GetById(int id)
        {
            var cardGroup = await _cardGroupService.GetByIdAsync(id);

            if (cardGroup == null)
                return NotFound();

            return Ok(cardGroup);
        }

        [RequirePermission("POST", "/api/cardgroup")]
        [HttpPost]
        public async Task<ActionResult<CardGroupDto>> Create(CreateCardGroupDto createCardGroupDto)
        {
            var createdCardGroup = await _cardGroupService.CreateAsync(createCardGroupDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCardGroup.Id }, createdCardGroup);
        }

        [RequirePermission("PUT", "/api/cardgroup/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CardGroupDto>> Update(int id, UpdateCardGroupDto updateCardGroupDto)
        {
            var updatedCardGroup = await _cardGroupService.UpdateAsync(id, updateCardGroupDto);

            if (updatedCardGroup == null)
                return NotFound();

            return Ok(updatedCardGroup);
        }

        [RequirePermission("DELETE", "/api/cardgroup/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cardGroupService.DeleteAsync(id, true);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [RequirePermission("PATCH", "/api/cardgroup/{id}/status")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            var success = await _cardGroupService.ChangeStatusAsync(id, changeStatusDto.Status);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
