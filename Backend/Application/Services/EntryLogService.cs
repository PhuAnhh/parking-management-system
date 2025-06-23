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
    public class EntryLogService : IEntryLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarningEventService _warningEventService;
        private readonly IWebHostEnvironment _env;

        public EntryLogService(IUnitOfWork unitOfWork, IWebHostEnvironment env
            , IWarningEventService warningEventService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _warningEventService = warningEventService;
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
                    UpdatedAt = entryLog.UpdatedAt,
                });
            }

            return entryLogDtos;
        }

        public async Task<IEnumerable<EntryLogDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var entryLogs = await _unitOfWork.EntryLogs.GetByDateRangeAsync(fromDate, toDate);
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
                    UpdatedAt = entryLog.UpdatedAt,
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
                UpdatedAt = entryLog.UpdatedAt,
            };
        }

        public async Task<EntryLogDto> CreateAsync(CreateEntryLogDto createEntryLogDto)
        {
            // Xử lý lưu file ảnh base64 thành file và lấy đường dẫn mới
            if (!string.IsNullOrEmpty(createEntryLogDto.ImageUrl) && createEntryLogDto.ImageUrl.StartsWith("data:image"))
            {
                var imageUrl = await SaveImageFileAsync(createEntryLogDto.ImageUrl);
                createEntryLogDto.ImageUrl = imageUrl; // Gán lại URL ảnh để lưu vào DB
            }

            if (string.IsNullOrEmpty(createEntryLogDto.PlateNumber))
                throw new ArgumentException("Plate number is required.");
            if (createEntryLogDto.CardId <= 0)
                throw new ArgumentException("Invalid card ID.");
            if (createEntryLogDto.LaneId <= 0)
                throw new ArgumentException("Invalid lane ID.");

            // Chuẩn hóa biển số xe trước khi kiểm tra và lưu
            createEntryLogDto.PlateNumber = StandardizePlateNumber(createEntryLogDto.PlateNumber);

            if (!IsValidPlateNumberFormat(createEntryLogDto.PlateNumber))
                throw new ArgumentException("Invalid plate number format. Expected format is like 30A12345.");


            // 1. Lấy thông tin thẻ
            var card = await _unitOfWork.Cards.GetByIdAsync(createEntryLogDto.CardId);
            if (card == null)
                throw new Exception("Card not found.");

            if (card.Status == CardStatus.Locked)
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = createEntryLogDto.PlateNumber,
                    LaneId = createEntryLogDto.LaneId,
                    WarningType = WarningType.CardLocked,
                    Note = createEntryLogDto.Note,
                    ImageUrl = createEntryLogDto.ImageUrl
                });

                throw new Exception("Card is locked.");
            }
            else if (card.Status != CardStatus.Active)
            {
                throw new Exception("Card is not active.");
            }

            // 2. Lấy thông tin nhóm thẻ
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(card.CardGroupId);
            if (cardGroup == null)
                throw new Exception("Card group not found for the selected card.");

            if (!cardGroup.Status)
                throw new Exception("Card group is not active.");

            // Kiểm tra hiệu lực thẻ nếu là thẻ tháng
            if (cardGroup.Type == CardGroupType.Month)
            {
                if (card.StartDate == null || card.EndDate == null)
                    throw new Exception("The card has no valid usage period.");

                var now = DateTime.Now;
                if (card.StartDate > now || card.EndDate < now)
                {
                    await _warningEventService.CreateAsync(new CreateWarningEventDto
                    {
                        PlateNumber = createEntryLogDto.PlateNumber,
                        LaneId = createEntryLogDto.LaneId,
                        WarningType = WarningType.CardExpired,
                        Note = "The card has expired.",
                        ImageUrl = createEntryLogDto.ImageUrl
                    });

                    throw new Exception("The card has expired.");
                }
            }

            // 3. Kiểm tra làn có thuộc nhóm thẻ hay không
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createEntryLogDto.LaneId))
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = createEntryLogDto.PlateNumber,
                    LaneId = createEntryLogDto.LaneId,
                    WarningType = WarningType.CardGroupNotAllowedInLane,
                    Note = createEntryLogDto.Note,
                    ImageUrl = createEntryLogDto.ImageUrl
                });

                throw new Exception("Lane is not allowed for the selected card group.");
            }

            // 4. Kiểm tra trạng thái làn
            var lane = await _unitOfWork.Lanes.GetByIdAsync(createEntryLogDto.LaneId);
            if (lane == null)
                throw new Exception("Lane not found");

            if (!lane.Status)
                throw new Exception("Lane is not active.");

            if (lane.Type != LaneType.In && lane.Type != LaneType.KioskIn && lane.Type != LaneType.Dynamic)
                throw new Exception("You are not allowed to enter through this lane.");

            // 5. Kiểm tra thẻ đã vào mà chưa ra
            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(card.Id);
            if (hasActiveEntry)
                throw new Exception("Card already in use - vehicle has not exited.");

            // 6. Kiểm tra biển số xe đã vào mà chưa ra
            if (!string.IsNullOrWhiteSpace(createEntryLogDto.PlateNumber))
            {
                var plateInUse = await _unitOfWork.EntryLogs.IsPlateNumberInUseAsync(createEntryLogDto.PlateNumber);
                if (plateInUse)
                    throw new Exception("Plate number is already in the parking lot.");
            }

            // 7. Tạo EntryLog mới
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

            await _unitOfWork.WarningEvents.CreateAsync(new WarningEvent
            {
                PlateNumber = entryLog.PlateNumber,
                LaneId = entryLog.LaneId,
                WarningType = WarningType.TicketIssued,
                Note = "Ghi vé vào",
                CreatedAt = DateTime.UtcNow,
                ImageUrl = entryLog.ImageUrl
            });
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

            await _warningEventService.CreateAsync(new CreateWarningEventDto
            {
                PlateNumber = entryLog.PlateNumber,
                LaneId = entryLog.LaneId,
                WarningType = WarningType.DeletedWhileVehicleInParking,
                Note = entryLog.Note,
                ImageUrl = entryLog.ImageUrl
            });

            _unitOfWork.EntryLogs.Delete(entryLog);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        private string StandardizePlateNumber(string plateNumber)
        {
            if (string.IsNullOrEmpty(plateNumber))
                return string.Empty;

            // Loại bỏ tất cả khoảng trắng
            plateNumber = plateNumber.Replace(" ", "");
                
            // Loại bỏ các ký tự đặc biệt như dấu gạch ngang, dấu chấm, v.v.
            plateNumber = plateNumber.Replace("-", "").Replace(".", "").Replace("_", "");

            // Chuyển đổi về chữ hoa để thống nhất định dạng
            plateNumber = plateNumber.ToUpper();

            return plateNumber;
        }

        private bool IsValidPlateNumberFormat(string plateNumber)
        {
            if (string.IsNullOrEmpty(plateNumber))
                return false;

            // Định dạng biển số của Việt Nam là: 
            // - 1-2 số + 1 chữ cái + 5 số (30A12345)
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{1,2}[A-Z]\d{5}$");
            return regex.IsMatch(plateNumber);
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
