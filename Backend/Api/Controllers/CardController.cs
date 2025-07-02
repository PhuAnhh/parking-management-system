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
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [RequirePermission("GET", "/api/card")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAll()
        {
            var cards = await _cardService.GetAllAsync();
            return Ok(cards);
        }

        [RequirePermission("GET", "/api/card/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CardDto>> GetById(int id)
        {
            var card = await _cardService.GetByIdAsync(id);

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        [RequirePermission("POST", "/api/card")]
        [HttpPost]
        public async Task<ActionResult<CardDto>> Create(CreateCardDto createCardDto)
        {
            var createdCard = await _cardService.CreateAsync(createCardDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCard.Id }, createdCard);
        }

        [RequirePermission("PUT", "/api/card/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CardDto>> Update(int id, UpdateCardDto updateCardDto)
        {
            try
            {
                var updatedCard = await _cardService.UpdateAsync(id, updateCardDto);

                if (updatedCard == null)
                    return NotFound();

                return Ok(updatedCard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [RequirePermission("DELETE", "/api/card/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _cardService.DeleteAsync(id);

                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
