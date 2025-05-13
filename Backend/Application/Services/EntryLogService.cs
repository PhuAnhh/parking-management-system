using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class EntryLogService : IEntryLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EntryLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EntryLogDto>> GetAllAsync()
        {
            var entryLogs = await _unitOfWork.EntryLogs.GetAllAsync();
            var entryLogDtos = new List<EntryLogDto>();

            foreach (var entryLog in entryLogs)
            {
                entryLogDtos.Add(new EntryLogDto
                {
                    Id = entryLog.Id,
                    PlateNumber = entryLog.PlateNumber,
                    CardId = entryLog.CardId,
                    CardGroupId = entryLog.CardGroupId,
                    LaneId = entryLog.LaneId,
                    CustomerId = entryLog.CustomerId,
                    EntryTime = entryLog.EntryTime,
                    ImageUrl = entryLog.ImageUrl,
                    Note = entryLog.Note,
                    CreatedAt = entryLog.CreatedAt,
                });
            }

            return entryLogDtos;
        }

        public async Task<EntryLogDto> GetByIdAsync(int id)
        {
            var entryLog = await _unitOfWork.EntryLogs.GetByIdAsync(id);

            if (entryLog == null)
                return null;

            return new EntryLogDto
            {
                Id = entryLog.Id,
                PlateNumber = entryLog.PlateNumber,
                CardId = entryLog.CardId,
                CardGroupId = entryLog.CardGroupId,
                LaneId = entryLog.LaneId,
                CustomerId = entryLog.CustomerId,
                EntryTime = entryLog.EntryTime,
                ImageUrl = entryLog.ImageUrl,
                Note = entryLog.Note,
                CreatedAt = entryLog.CreatedAt,
            };
        }

        public async Task<EntryLogDto> CreateAsync(CreateEntryLogDto createEntryLogDto)
        {
            // 1. Lấy thông tin thẻ
            var card = await _unitOfWork.Cards.GetByIdAsync(createEntryLogDto.CardId);
            if (card == null)
                throw new Exception("Card not found.");

            if (card.Status != CardStatus.Active)
                throw new Exception("Card is not active.");

            // 2. Lấy thông tin nhóm thẻ
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(card.CardGroupId);
            if (cardGroup == null)
                throw new Exception("Card group not found for the selected card.");

            if (!cardGroup.Status)
                throw new Exception("Card group is not active.");

            // 3. Kiểm tra làn có thuộc nhóm thẻ hay không (ví dụ kiểm tra bằng danh sách làn trong nhóm thẻ)
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createEntryLogDto.LaneId))
                throw new Exception("Lane is not allowed for the selected card group.");

            // 4. Kiểm tra trạng thái làn
            var lane = await _unitOfWork.Lanes.GetByIdAsync(createEntryLogDto.LaneId);
            if (lane == null)
                throw new Exception("Lane not found");

            if (!lane.Status)
                throw new Exception("Lane is not active.");

            // 5. Tạo EntryLog mới
            var entryLog = new EntryLog
            {
                PlateNumber = createEntryLogDto.PlateNumber,
                CardId = card.Id,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                LaneId = createEntryLogDto.LaneId,
                EntryTime = DateTime.UtcNow,
                ImageUrl = createEntryLogDto.ImageUrl,
                Note = createEntryLogDto.Note,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.EntryLogs.CreateAsync(entryLog);
            await _unitOfWork.SaveChangesAsync();

            return new EntryLogDto
            {
                Id = entryLog.Id,
                PlateNumber = entryLog.PlateNumber,
                CardId = entryLog.CardId,
                CardGroupId = entryLog.CardGroupId,
                CustomerId = entryLog.CustomerId,
                LaneId = entryLog.LaneId,
                EntryTime = entryLog.EntryTime,
                ImageUrl = entryLog.ImageUrl,
                Note = entryLog.Note,
                CreatedAt = entryLog.CreatedAt,
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var entryLog = await _unitOfWork.EntryLogs.GetByIdAsync(id);

            if (entryLog == null)
                return false;

            _unitOfWork.EntryLogs.Delete(entryLog);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
