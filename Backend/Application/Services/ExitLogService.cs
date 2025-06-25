using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Final_year_Project.Application.Services
{
    public class ExitLogService : IExitLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarningEventService _warningEventService;
        private readonly IWebHostEnvironment _env;

        public ExitLogService(IUnitOfWork unitOfWork , IWebHostEnvironment env, IWarningEventService warningEventService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _warningEventService = warningEventService;
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
                    EntryLog = exitLog.EntryLog == null ? null : new EntryLogDto
                    {
                        Id = exitLog.EntryLog.Id,
                        PlateNumber = exitLog.EntryLog.PlateNumber,
                        CardId = exitLog.EntryLog.CardId,
                        CardGroupId = exitLog.EntryLog.CardGroupId,
                        LaneId = exitLog.EntryLog.LaneId,
                        CustomerId = exitLog.EntryLog.CustomerId,
                        ImageUrl = exitLog.EntryLog.ImageUrl,
                        Note = exitLog.EntryLog.Note
                    }
                });
            }

            return exitLogDtos;
        }

        public async Task<IEnumerable<ExitLogDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var exitLogs = await _unitOfWork.ExitLogs.GetByDateRangeAsync(fromDate, toDate);
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
                    EntryLog = exitLog.EntryLog == null ? null : new EntryLogDto
                    {
                        Id = exitLog.EntryLog.Id,
                        PlateNumber = exitLog.EntryLog.PlateNumber,
                        CardId = exitLog.EntryLog.CardId,
                        CardGroupId = exitLog.EntryLog.CardGroupId,
                        LaneId = exitLog.EntryLog.LaneId,
                        CustomerId = exitLog.EntryLog.CustomerId,
                        ImageUrl = exitLog.EntryLog.ImageUrl,
                        Note = exitLog.EntryLog.Note
                    }
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
                EntryLog = exitLog.EntryLog == null ? null : new EntryLogDto
                {
                    Id = exitLog.EntryLog.Id,
                    PlateNumber = exitLog.EntryLog.PlateNumber,
                    CardId = exitLog.EntryLog.CardId,
                    CardGroupId = exitLog.EntryLog.CardGroupId,
                    LaneId = exitLog.EntryLog.LaneId,
                    CustomerId = exitLog.EntryLog.CustomerId,
                    ImageUrl = exitLog.EntryLog.ImageUrl,
                    Note = exitLog.EntryLog.Note
                }
            };
        }

        public async Task<ExitLogDto> CreateAsync(CreateExitLogDto createExitLogDto)
        {
            // Xử lý lưu file ảnh base64 thành file và lấy đường dẫn mới
            if (!string.IsNullOrEmpty(createExitLogDto.ImageUrl) && createExitLogDto.ImageUrl.StartsWith("data:image"))
            {
                var imageUrl = await SaveImageFileAsync(createExitLogDto.ImageUrl);
                createExitLogDto.ImageUrl = imageUrl; // Gán lại URL ảnh để lưu vào DB
            }

            if (createExitLogDto.EntryLogId <= 0)
                throw new ArgumentException("EntryLogId is required.");

            // Lấy EntryLog
            var entryLog = await _unitOfWork.EntryLogs.GetByIdAsync(createExitLogDto.EntryLogId);
            if (entryLog == null)
                throw new Exception("Entry log not found.");

            // Lấy thông tin thẻ từ EntryLog
            var card = await _unitOfWork.Cards.GetByIdAsync(entryLog.CardId);
            if (card == null)
                throw new Exception("Card not found.");

            if (card.Status == CardStatus.Locked)
                throw new Exception("Card is locked.");

            if (card.Status == CardStatus.Inactive)
                throw new Exception("Card is not in use.");

            // Lấy CardGroup
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(entryLog.CardGroupId ?? 0);
            if (cardGroup == null)
                throw new Exception("Card group not found.");

            if (!cardGroup.Status)
                throw new Exception("Card group is not active.");

            //Kiểm tra Lane ra trong CardGroup
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createExitLogDto.ExitLaneId))
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = createExitLogDto.ExitPlateNumber,
                    LaneId = createExitLogDto.ExitLaneId,
                    WarningType = WarningType.CardGroupNotAllowedInLane,
                    Note = createExitLogDto.Note,
                    ImageUrl = createExitLogDto.ImageUrl
                });

                throw new Exception("Exit lane is not allowed for this card group.");
            }

            //Kiểm tra trạng thái Lane ra
            var exitLane = await _unitOfWork.Lanes.GetByIdAsync(createExitLogDto.ExitLaneId);
            if (exitLane == null || !exitLane.Status)
                throw new Exception("Exit lane is invalid or inactive.");

            if (exitLane.Type != LaneType.Out && exitLane.Type != LaneType.Dynamic && exitLane.Type != LaneType.KioskIn)
                throw new Exception("You are not allowed to exit through this lane.");

            //Kiểm tra Lane vào 
            var entryLane = await _unitOfWork.Lanes.GetByIdAsync(entryLog.LaneId);
            if (entryLane == null || !entryLane.Status)
                throw new Exception("Entry lane is invalid or inactive.");

            //Xác định thời gian ra khỏi bãi
            var exitTime = createExitLogDto.ExitTime != default ? createExitLogDto.ExitTime : DateTime.UtcNow;
            if (exitTime <= entryLog.EntryTime)
                throw new Exception("Exit time must be after entry time.");

            // Kiểm tra nếu biển số ra không khớp với biển số vào
            if (!string.Equals(entryLog.PlateNumber?.Trim(), createExitLogDto.ExitPlateNumber?.Trim(), StringComparison.OrdinalIgnoreCase))
            {   
                var warning = new WarningEvent
                {
                    PlateNumber = createExitLogDto.ExitPlateNumber,
                    LaneId = createExitLogDto.ExitLaneId,
                    WarningType = WarningType.LicensePlateMismatch,
                    Note = $"Vào: {entryLog.PlateNumber}, Ra: {createExitLogDto.ExitPlateNumber}",
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = createExitLogDto.ImageUrl
                };

                await _unitOfWork.WarningEvents.CreateAsync(warning);
            }

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
                TotalDuration = (long)duration.TotalMilliseconds,
                TotalPrice = totalPrice,
                Note = createExitLogDto.Note,
                ImageUrl = createExitLogDto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.ExitLogs.CreateAsync(exitLog);

            await _unitOfWork.WarningEvents.CreateAsync(new WarningEvent
            {
                PlateNumber = exitLog.ExitPlateNumber,
                LaneId = exitLog.ExitLaneId,
                WarningType = WarningType.TicketIssued,
                Note = "Ghi vé ra",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = exitLog.ImageUrl
            });

            entryLog.Exited = true;
            _unitOfWork.EntryLogs.Update(entryLog);

            // Xóa liên kết Card khi xe ra khỏi bãi
            if (exitLog.CardId > 0)
            {
                var exitCard = await _unitOfWork.Cards.GetByIdAsync(exitLog.CardId);
                if (exitCard != null)
                {
                    exitCard.Status = CardStatus.Inactive;
                    exitCard.UpdatedAt = DateTime.UtcNow;

                    // Nếu là thẻ ngày thì hủy gán khách
                    var cardGroupOfCard = await _unitOfWork.CardGroups.GetByIdAsync(exitCard.CardGroupId);
                    if (cardGroupOfCard != null && cardGroupOfCard.Type == CardGroupType.Day)
                    {
                        exitCard.CustomerId = null;
                    }

                    _unitOfWork.Cards.Update(exitCard);
                }
            }

            var revenueReport = await _unitOfWork.RevenueReports.GetByCardGroupIdAsync(exitLog.CardGroupId);

            if (revenueReport != null)
            {
                revenueReport.ExitCount += 1;
                revenueReport.Revenue += exitLog.TotalPrice;
                revenueReport.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.RevenueReports.Update(revenueReport);
            }
            else
            {
                var newReport = new RevenueReport
                {
                    CardGroupId = exitLog.CardGroupId,
                    ExitCount = 1,
                    Revenue = exitLog.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.RevenueReports.CreateAsync(newReport);
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

        private async Task<string?> SaveImageFileAsync(string? base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
                return null;

            // Xử lý loại bỏ header "data:image/png;base64,"
            var base64Data = Regex.Replace(base64Image, "^data:image/[^;]+;base64,", string.Empty);
            var bytes = Convert.FromBase64String(base64Data);

            var fileName = $"{Guid.NewGuid()}.png";
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var filePath = Path.Combine(imagesFolder, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            // Trả về đường dẫn relative cho FE dùng
            return $"/images/{fileName}";
        }
    }
}
