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

        public ExitLogService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IWarningEventService warningEventService)
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

            // Chuẩn hóa biển số xe trước khi kiểm tra và lưu
            var standardizedPlateNumber = StandardizePlateNumber(createExitLogDto.ExitPlateNumber);
            if (!IsValidPlateNumberFormat(standardizedPlateNumber))
                throw new ArgumentException("Biển số không hợp lệ");


            if (createExitLogDto.EntryLogId <= 0)
                throw new ArgumentException("EntryLogId is required.");

            // 1. Lấy EntryLog
            var entryLog = await _unitOfWork.EntryLogs.GetByIdAsync(createExitLogDto.EntryLogId);
            if (entryLog == null)
                throw new Exception("Không tìm thấy xe vào bãi");

            // 2. Lấy thông tin thẻ từ EntryLog
            var card = await _unitOfWork.Cards.GetByIdAsync(entryLog.CardId);
            if (card == null)
                throw new Exception("Không tìm thấy thẻ");

            if (card.Status == CardStatus.Locked)
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = standardizedPlateNumber,
                    LaneId = createExitLogDto.ExitLaneId,
                    WarningType = WarningType.CardLocked,
                    Note = "Xe ra",
                    ImageUrl = createExitLogDto.ImageUrl
                });

                throw new Exception("Thẻ bị khóa");
            }

            if (card.Status == CardStatus.Inactive)
                throw new Exception("Thẻ chưa sử dụng để vào bãi");

            // 3. Lấy CardGroup
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(entryLog.CardGroupId ?? 0);
            if (cardGroup == null)
                throw new Exception("Không tìm thấy nhóm thẻ");

            if (!cardGroup.Status)
                throw new Exception("Nhóm thẻ không hoạt động");

            // 4. Kiểm tra hiệu lực thẻ (tháng)
            if (cardGroup.Type == CardGroupType.Month)
            {
                if (card.StartDate == null || card.EndDate == null)
                    throw new Exception("Thẻ không còn hiệu lực sử dụng");

                var now = DateTime.Now;
                if (card.StartDate > now || card.EndDate < now)
                {
                    await _warningEventService.CreateAsync(new CreateWarningEventDto
                    {
                        PlateNumber = standardizedPlateNumber,
                        LaneId = createExitLogDto.ExitLaneId,
                        WarningType = WarningType.CardExpired,
                        Note = "Thẻ đã hết hạn",
                        ImageUrl = createExitLogDto.ImageUrl
                    });

                    throw new Exception("Vui lòng gia hạn thẻ");
                }
            }

            // 5. Kiểm tra trạng thái làn
            var exitLane = await _unitOfWork.Lanes.GetByIdAsync(createExitLogDto.ExitLaneId);
            if (exitLane == null)
                throw new Exception("Không tìm thấy làn");

            if (!exitLane.Status)
                throw new Exception("Làn không hoạt động");

            if (exitLane.Type != LaneType.Out && exitLane.Type != LaneType.Dynamic && exitLane.Type != LaneType.KioskIn)
                throw new Exception("Không được phép sử dụng làn");

            // 6. Kiểm tra quyền sử dụng làn
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createExitLogDto.ExitLaneId))
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = standardizedPlateNumber,
                    LaneId = createExitLogDto.ExitLaneId,
                    WarningType = WarningType.CardGroupNotAllowedInLane,
                    Note = createExitLogDto.Note,
                    ImageUrl = createExitLogDto.ImageUrl
                });

                throw new Exception("Nhóm thẻ không được sử dụng làn");
            }

            // 7. Xác định thời gian ra khỏi bãi
            var exitTime = createExitLogDto.ExitTime != default ? createExitLogDto.ExitTime : DateTime.UtcNow;

            // 8. Kiểm tra nếu biển số ra không khớp với biển số vào
            if (!string.Equals(entryLog.PlateNumber?.Trim(), createExitLogDto.ExitPlateNumber?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = standardizedPlateNumber,
                    LaneId = createExitLogDto.ExitLaneId,
                    WarningType = WarningType.LicensePlateMismatch,
                    Note = $"Biển số vào: {entryLog.PlateNumber}",
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = createExitLogDto.ImageUrl
                });

                throw new Exception("Biển số vào ra không khớp");
            }

            if (cardGroup.Type == CardGroupType.Month && !string.IsNullOrEmpty(card.PlateNumber))
            {
                if (!string.Equals(card.PlateNumber?.Trim(), standardizedPlateNumber?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    // Tạo cảnh báo
                    await _warningEventService.CreateAsync(new CreateWarningEventDto
                    {
                        PlateNumber = standardizedPlateNumber,
                        LaneId = createExitLogDto.ExitLaneId,
                        WarningType = WarningType.LicensePlateMismatch,
                        Note = $"Biển số đăng ký: {card.PlateNumber}",
                        ImageUrl = createExitLogDto.ImageUrl
                    });

                    // Throw exception để không cho phép xe ra
                    throw new Exception("Biển số khác với biển đăng ký");
                }
            }

            // 9. Tính tổng thời gian
            var duration = exitTime - entryLog.EntryTime;

            // 10. Tính giá dựa trên thời gian và quy tắc CardGroup
            decimal totalPrice = CalculatePrice(duration, cardGroup);

            // 11. Tạo ExitLog
            var exitLog = new ExitLog
            {
                EntryLogId = entryLog.Id,
                ExitPlateNumber = standardizedPlateNumber,
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
                Note = "Xe đã rời bãi",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = exitLog.ImageUrl
            });

            entryLog.Exited = true;
            _unitOfWork.EntryLogs.Update(entryLog);

            // 12. Cập nhật trạng thái thẻ
            if (exitLog.CardId > 0)
            {
                var exitCard = await _unitOfWork.Cards.GetByIdAsync(exitLog.CardId);
                if (exitCard != null)
                {
                    exitCard.Status = CardStatus.Inactive;
                    exitCard.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Cards.Update(exitCard);
                }
            }

            // 13. Cập nhật báo cáo doanh thu
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