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
                    PlateNumber = card.PlateNumber,
                    CardGroupId = card.CardGroupId,
                    CustomerId = card.CustomerId,
                    Note = card.Note,
                    StartDate = card.StartDate,
                    EndDate = card.EndDate,
                    AutoRenewDays = card.AutoRenewDays,
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
                PlateNumber = card.PlateNumber,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                Note = card.Note,
                StartDate = card.StartDate,
                EndDate = card.EndDate,
                AutoRenewDays = card.AutoRenewDays,
                Status = card.Status,
                CreatedAt = card.CreatedAt,
                UpdatedAt = card.UpdatedAt
            };
        }

        public async Task<CardDto> CreateAsync(CreateCardDto createCardDto)
        {
            var rawPlate = createCardDto.PlateNumber;
            var standardizedPlate = StandardizePlateNumber(rawPlate);

            if (!string.IsNullOrEmpty(rawPlate) && !IsValidPlateNumberFormat(standardizedPlate))
            {
                throw new Exception("Biển số không hợp lệ");
            }

            if (!string.IsNullOrEmpty(standardizedPlate) && await IsPlateNumberDuplicate(standardizedPlate))
            {
                throw new Exception("Biển số đã được liên kết với thẻ khác");
            }

            var card = new Card
            {
                Name = createCardDto.Name,
                Code = createCardDto.Code,
                PlateNumber = standardizedPlate,
                CardGroupId = createCardDto.CardGroupId,
                CustomerId = createCardDto.CustomerId,
                Note = createCardDto.Note,
                StartDate = createCardDto.StartDate,
                EndDate = createCardDto.EndDate,
                AutoRenewDays = createCardDto.AutoRenewDays,
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
                PlateNumber = card.PlateNumber,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                Note = card.Note,
                StartDate = card.StartDate,
                EndDate = card.EndDate,
                AutoRenewDays = card.AutoRenewDays,
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

            var rawPlate = updateCardDto.PlateNumber;
            var standardizedPlate = StandardizePlateNumber(rawPlate);

            // Kiểm tra định dạng
            if (!string.IsNullOrEmpty(rawPlate) && !IsValidPlateNumberFormat(standardizedPlate))
            {
                throw new Exception("Biển số không hợp lệ");
            }

            // Kiểm tra trùng biển số
            if (!string.IsNullOrEmpty(standardizedPlate) && await IsPlateNumberDuplicate(standardizedPlate, id))
            {
                throw new Exception("Biển số đã được liên kết với thẻ khác");
            }

            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(id);

            if (hasActiveEntry)
            {
                if (card.Name != updateCardDto.Name ||
                    card.Code != updateCardDto.Code ||
                    card.PlateNumber != standardizedPlate ||
                    card.CardGroupId != updateCardDto.CardGroupId ||
                    card.CustomerId != updateCardDto.CustomerId ||
                    card.StartDate != updateCardDto.StartDate ||
                    card.EndDate != updateCardDto.EndDate ||
                    card.AutoRenewDays != updateCardDto.AutoRenewDays)
                {
                    throw new Exception("Chỉ được phép cập nhật trạng thái và ghi chú khi xe đang trong bãi");
                }
                card.Note = updateCardDto.Note;
                card.Status = updateCardDto.Status;
            }
            else
            {
                card.Name = updateCardDto.Name;
                card.Code = updateCardDto.Code;
                card.PlateNumber = standardizedPlate;
                card.CardGroupId = updateCardDto.CardGroupId;
                card.CustomerId = updateCardDto.CustomerId;
                card.Note = updateCardDto.Note;
                card.StartDate = updateCardDto.StartDate;
                card.EndDate = updateCardDto.EndDate;
                card.AutoRenewDays = updateCardDto.AutoRenewDays;
                card.Status = updateCardDto.Status;
            }

            card.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Cards.Update(card);
            await _unitOfWork.SaveChangesAsync();

            var updatedCard = await _unitOfWork.Cards.GetByIdAsync(id);
            return new CardDto
            {
                Id = card.Id,
                Name = card.Name,
                Code = card.Code,
                PlateNumber = card.PlateNumber,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                Note = card.Note,
                StartDate = card.StartDate,
                EndDate = card.EndDate,
                AutoRenewDays = card.AutoRenewDays,
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

            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(cardId: id);
            if (hasActiveEntry)
            {
                throw new Exception("Không thể xóa khi xe đang trong bãi");
            }

            card.Deleted = true;
            _unitOfWork.Cards.Update(card);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private string StandardizePlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return string.Empty;

            // Loại bỏ mọi ký tự không phải chữ cái hoặc số, sau đó chuyển thành chữ hoa
            return System.Text.RegularExpressions.Regex
                .Replace(plateNumber, @"[^A-Za-z0-9]", "")
                .ToUpperInvariant();
        }

        private bool IsValidPlateNumberFormat(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return false;

            var regex = new System.Text.RegularExpressions.Regex(
                @"^(?:\d{1,2}[A-Z]\d{4,6}|[A-Z]{1,2}\d{5,6})$"
            );

            return regex.IsMatch(plateNumber.ToUpperInvariant());
        }

        private async Task<bool> IsPlateNumberDuplicate(string plateNumber, int? excludeCardId = null)
        {
            var standardizedPlate = StandardizePlateNumber(plateNumber);
            if (string.IsNullOrEmpty(standardizedPlate))
                return false;

            var existingCards = await _unitOfWork.Cards.GetAllAsync();
            return existingCards.Any(c =>
                c.PlateNumber == standardizedPlate &&
                c.Id != excludeCardId);
        }
    }
}

