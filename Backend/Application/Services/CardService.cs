using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class CardService : ICardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CardDto>> GetAllAsync()
        {
            var cards = await _unitOfWork.Cards.GetAllAsync();
            var cardDtos = new List<CardDto>();

            foreach (var card in cards)
            {
                cardDtos.Add(new CardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Code = card.Code,
                    CardGroupId = card.CardGroupId,
                    CustomerId = card.CustomerId,
                    Note = card.Note,
                    Status = card.Status,
                    CreatedAt = card.CreatedAt,
                    UpdatedAt = card.UpdatedAt
                });
            }

            return cardDtos;
        }

        public async Task<CardDto> GetByIdAsync(int id)
        {
            var card = await _unitOfWork.Cards.GetByIdAsync(id);

            if (card == null)
                return null;

            return new CardDto
            {
                Id = card.Id,
                Name = card.Name,
                Code = card.Code,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                Note = card.Note,
                Status = card.Status,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };
        }

        public async Task<CardDto> CreateAsync(CreateCardDto createCardDto)
        {
            var card = new Card
            {
                Name = createCardDto.Name,
                Code = createCardDto.Code,
                CardGroupId = createCardDto.CardGroupId,
                CustomerId = createCardDto.CustomerId,
                Note = createCardDto.Note,
                Status = createCardDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Cards.CreateAsync(card);
            await _unitOfWork.SaveChangesAsync();

            return new CardDto
            {
                Id = card.Id,
                Name = card.Name,
                Code = card.Code,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId, 
                Note = card.Note,
                Status = card.Status,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt,
            };
        }

        public async Task<CardDto> UpdateAsync(int id, UpdateCardDto updateCardDto)
        {
            var card = await _unitOfWork.Cards.GetByIdAsync(id);

            if (card == null)
                return null;

            card.Name = updateCardDto.Name;
            card.Code = updateCardDto.Code;
            card.CardGroupId = updateCardDto.CardGroupId;
            card.CustomerId = updateCardDto.CustomerId;
            card.Note = updateCardDto.Note;
            card.Status = updateCardDto.Status;
            card.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Cards.Update(card);
            await _unitOfWork.SaveChangesAsync();

            return new CardDto
            {
                Id = card.Id,
                Name = card.Name,
                Code = card.Code,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                Note = card.Note,
                Status = card.Status,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var card = await _unitOfWork.Cards.GetByIdAsync(id);

            if (card == null)
                return false;

            _unitOfWork.Cards.Delete(card);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

