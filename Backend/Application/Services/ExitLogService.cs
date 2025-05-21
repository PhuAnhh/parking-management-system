using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Services
{
    public class ExitLogService : IExitLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExitLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ExitLogDto>> GetAllAsync()
        {
            var exitLogs = await _unitOfWork.ExitLogs.GetAllAsync();
            var exitLogDtos = new List<ExitLogDto>();

            foreach (var exitLog in exitLogs)
            {
                exitLogDtos.Add(new ExitLogDto
                {
                    Id = exitLog.Id,
                    EntryLogId = exitLog.EntryLogId,
                    ExitPlateNumber = exitLog.ExitPlateNumber,
                    CardId = exitLog.CardId,
                    CardGroupId = exitLog.CardGroupId,
                    EntryLaneId = exitLog.EntryLaneId,
                    ExitLaneId = exitLog.ExitLaneId,
                    EntryTime = exitLog.EntryTime,
                    ExitTime = exitLog.ExitTime,
                    TotalDuration = exitLog.TotalDuration,
                    TotalPrice = exitLog.TotalPrice,
                    ImageUrl = exitLog.ImageUrl,
                    Note = exitLog.Note,
                    CreatedAt = exitLog.CreatedAt,
                });
            }

            return exitLogDtos;
        }

        public async Task<ExitLogDto> GetByIdAsync(int id)
        {
            var exitLog = await _unitOfWork.ExitLogs.GetByIdAsync(id);

            if (exitLog == null)
                return null;

            return new ExitLogDto
            {
                Id = exitLog.Id,
                EntryLogId = exitLog.EntryLogId,
                ExitPlateNumber = exitLog.ExitPlateNumber,
                CardId = exitLog.CardId,
                CardGroupId = exitLog.CardGroupId,
                EntryLaneId = exitLog.EntryLaneId,
                ExitLaneId = exitLog.ExitLaneId,
                EntryTime = exitLog.EntryTime,
                ExitTime = exitLog.ExitTime,
                TotalDuration = exitLog.TotalDuration,
                TotalPrice = exitLog.TotalPrice,
                ImageUrl = exitLog.ImageUrl,
                Note = exitLog.Note,
                CreatedAt = exitLog.CreatedAt,
            };
        }

        public async Task<ExitLogDto> CreateAsync(CreateExitLogDto createExitLogDto)
        {
            if (createExitLogDto.EntryLogId <= 0)
                throw new ArgumentException("EntryLogId is required.");

            // Lấy EntryLog
            var entryLog = await _unitOfWork.EntryLogs.GetByIdAsync(createExitLogDto.EntryLogId);
            if (entryLog == null)
                throw new Exception("Entry log not found.");

            //Lấy CardGroup
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(entryLog.CardGroupId ?? 0);
            if (cardGroup == null)
                throw new Exception("Card group not found.");

            //Kiểm tra Lane ra trong CardGroup
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createExitLogDto.ExitLaneId))
                throw new Exception("Exit lane is not allowed for this card group.");

            //Kiểm tra trạng thái Lane ra
            var exitLane = await _unitOfWork.Lanes.GetByIdAsync(createExitLogDto.ExitLaneId);
            if (exitLane == null || !exitLane.Status)
                throw new Exception("Exit lane is invalid or inactive.");

            //Kiểm tra Lane vào 
            var entryLane = await _unitOfWork.Lanes.GetByIdAsync(entryLog.LaneId);
            if (entryLane == null || !entryLane.Status)
                throw new Exception("Entry lane is invalid or inactive.");

            //Xác định thời gian ra khỏi bãi
            var exitTime = createExitLogDto.ExitTime != default ? createExitLogDto.ExitTime : DateTime.UtcNow;
            if (exitTime <= entryLog.EntryTime)
                throw new Exception("Exit time must be after entry time.");

            //Tính tổng thời gian
            var duration = exitTime - entryLog.EntryTime;
            
            //Tính giá dựa trên thời gian và quy tắc CardGroup
            decimal totalPrice = CalculatePrice(duration, cardGroup);

            //Tạo ExitLog
            var exitLog = new ExitLog
            {
                EntryLogId = entryLog.Id,
                ExitPlateNumber = string.IsNullOrWhiteSpace(createExitLogDto.ExitPlateNumber) ? entryLog.PlateNumber : createExitLogDto.ExitPlateNumber,
                CardId = entryLog.CardId,
                CardGroupId = entryLog.CardGroupId ?? 0,
                EntryLaneId = entryLog.LaneId,
                ExitLaneId = createExitLogDto.ExitLaneId,
                EntryTime = entryLog.EntryTime,
                ExitTime = exitTime,
                TotalDuration = duration.Ticks,
                TotalPrice = totalPrice,
                Note = createExitLogDto.Note,
                ImageUrl = createExitLogDto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.ExitLogs.CreateAsync(exitLog);

            entryLog.Exited = true;
            _unitOfWork.EntryLogs.Update(entryLog);

            // Xóa liên kết Card (Day) khi xe ra khỏi bãi
            if (exitLog.CardId > 0)
            {
                var card = await _unitOfWork.Cards.GetByIdAsync(exitLog.CardId);
                if (card != null)
                {
                    var cardGroupOfCard = await _unitOfWork.CardGroups.GetByIdAsync(card.CardGroupId);
                    if (cardGroupOfCard != null && cardGroupOfCard.Type == CardGroupType.Day)
                    {
                        card.CustomerId = null;
                        card.Status = CardStatus.Inactive; // hoặc trạng thái phù hợp
                        card.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Cards.Update(card);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return new ExitLogDto
            {
                Id = exitLog.Id,
                EntryLogId = exitLog.EntryLogId,
                ExitPlateNumber = exitLog.ExitPlateNumber,
                CardId = exitLog.CardId,
                CardGroupId = exitLog.CardGroupId,
                EntryLaneId = exitLog.EntryLaneId,
                ExitLaneId = exitLog.ExitLaneId,
                EntryTime = exitLog.EntryTime,
                ExitTime = exitLog.ExitTime,
                TotalDuration = exitLog.TotalDuration,
                TotalPrice = exitLog.TotalPrice,
                Note = exitLog.Note,
                ImageUrl = exitLog.ImageUrl,
                CreatedAt = exitLog.CreatedAt,
            };
        }


        private decimal CalculatePrice(TimeSpan duration, CardGroup cardGroup)
        {
            if (duration.TotalMinutes <= 0)
                return 0m;

            // Miễn phí nếu có
            if (cardGroup.FreeMinutes.HasValue && duration.TotalMinutes <= cardGroup.FreeMinutes.Value)
                return 0m;

            double remainingMinutes = duration.TotalMinutes;

            decimal totalPrice = 0m;

            // Trừ free minutes nếu có
            if (cardGroup.FreeMinutes.HasValue)
            {
                remainingMinutes -= cardGroup.FreeMinutes.Value;
            }

            // Tính block đầu tiên
            if (cardGroup.FirstBlockMinutes.HasValue && cardGroup.FirstBlockPrice.HasValue)
            {
                if (remainingMinutes <= cardGroup.FirstBlockMinutes.Value)
                {
                    totalPrice += cardGroup.FirstBlockPrice.Value;
                    return totalPrice;
                }
                else
                {
                    totalPrice += cardGroup.FirstBlockPrice.Value;
                    remainingMinutes -= cardGroup.FirstBlockMinutes.Value;
                }
            }

            // Tính block tiếp theo
            if (cardGroup.NextBlockMinutes.HasValue && cardGroup.NextBlockPrice.HasValue && remainingMinutes > 0)
            {
                var nextBlocks = Math.Ceiling(remainingMinutes / cardGroup.NextBlockMinutes.Value);
                totalPrice += (decimal)nextBlocks * cardGroup.NextBlockPrice.Value;
            }

            return totalPrice;
        }
    }
}
