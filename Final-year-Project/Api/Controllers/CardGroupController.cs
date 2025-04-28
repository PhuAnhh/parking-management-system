using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardGroupDto>>> GetAll()
        {
            var cardGroups = await _cardGroupService.GetAllAsync();
            return Ok(cardGroups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardGroupDto>> GetById(int id)
        {
            var cardGroup = await _cardGroupService.GetByIdAsync(id);

            if (cardGroup == null)
                return NotFound();

            return Ok(cardGroup);
        }

        [HttpPost]
        public async Task<ActionResult<CardGroupDto>> Create(CreateCardGroupDto createCardGroupDto)
        {
            var createdCardGroup = await _cardGroupService.CreateAsync(createCardGroupDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCardGroup.Id }, createdCardGroup);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CardGroupDto>> Update(int id, UpdateCardGroupDto updateCardGroupDto)
        {
            var updatedCardGroup = await _cardGroupService.UpdateAsync(id, updateCardGroupDto);

            if (updatedCardGroup == null)
                return NotFound();

            return Ok(updatedCardGroup);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cardGroupService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
