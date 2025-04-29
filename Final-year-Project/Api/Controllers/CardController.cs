using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Services.Abstractions;
using Final_year_Project.Application.Models;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetAll()
        {
            var cards = await _cardService.GetAllAsync();
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardDto>> GetById(int id)
        {
            var card = await _cardService.GetByIdAsync(id);

            if (card == null)
                return NotFound();

            return Ok(card);
        }

        [HttpPost]
        public async Task<ActionResult<CardDto>> Create(CreateCardDto createCardDto)
        {
            var createdCard = await _cardService.CreateAsync(createCardDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCard.Id }, createdCard);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CardDto>> Update(int id, UpdateCardDto updateCardDto)
        {
            var updatedCard = await _cardService.UpdateAsync(id, updateCardDto);

            if (updatedCard == null)
                return NotFound();

            return Ok(updatedCard);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cardService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
