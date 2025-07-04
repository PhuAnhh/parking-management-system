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

        public async Task<IEnumerable<EntryLogDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, bool? exited = null)
        {
            var entryLogs = await _unitOfWork.EntryLogs.GetByDateRangeAsync(fromDate, toDate, exited);
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
                    Exited = entryLog.Exited,
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

            if (string.IsNullOrEmpty(createEntryLogDto.PlateNumber))
                throw new ArgumentException("Vui lòng nhập biển số xe");

            // Xử lý lưu file ảnh base64 thành file và lấy đường dẫn mới
            if (!string.IsNullOrEmpty(createEntryLogDto.ImageUrl) && createEntryLogDto.ImageUrl.StartsWith("data:image"))
            {
                var imageUrl = await SaveImageFileAsync(createEntryLogDto.ImageUrl);
                createEntryLogDto.ImageUrl = imageUrl; // Gán lại URL ảnh để lưu vào DB
            }

            // Chuẩn hóa biển số xe trước khi kiểm tra và lưu
            var standardizedPlateNumber = StandardizePlateNumber(createEntryLogDto.PlateNumber);
            if (!IsValidPlateNumberFormat(standardizedPlateNumber))
                throw new ArgumentException("Biển số không hợp lệ");


            // 1. Lấy thông tin thẻ
            var card = await _unitOfWork.Cards.GetByIdAsync(createEntryLogDto.CardId);
            if (card == null)
                throw new Exception("Không tìm thấy thẻ");

            if (card.Status == CardStatus.Locked)
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = standardizedPlateNumber,
                    LaneId = createEntryLogDto.LaneId,
                    WarningType = WarningType.CardLocked,
                    Note = createEntryLogDto.Note,
                    ImageUrl = createEntryLogDto.ImageUrl
                });

                throw new Exception("Thẻ bị khóa");
            }

            if (card.Status != CardStatus.Inactive)
            {
                throw new Exception("Thẻ không hợp lệ để vào bãi");
            }

            // 2. Kiểm tra thẻ đã vào mà chưa ra
            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(card.Id);
            if (hasActiveEntry)
                throw new Exception("Thẻ đang được sử dụng");

            // 3. Kiểm tra biển số xe đã vào mà chưa ra
            var plateInUse = await _unitOfWork.EntryLogs.IsPlateNumberInUseAsync(standardizedPlateNumber);
            if (plateInUse)
                throw new Exception("Xe hiện đang trong bãi");

            // 4. Lấy thông tin nhóm thẻ
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(card.CardGroupId);
            if (cardGroup == null)
                throw new Exception("Không tìm thấy nhóm thẻ");

            if (!cardGroup.Status)
                throw new Exception("Nhóm thẻ không hoạt động");

            // 5. Kiểm tra biển số khớp với thẻ tháng
            if(cardGroup.Type == CardGroupType.Month)
            {
                //Chỉ kiểm tra biển số khớp khi thẻ tháng có biển số
                if (!string.IsNullOrEmpty(card.PlateNumber) && card.PlateNumber != standardizedPlateNumber)
                {
                    await _warningEventService.CreateAsync(new CreateWarningEventDto
                    {
                        PlateNumber = standardizedPlateNumber,
                        LaneId = createEntryLogDto.LaneId,
                        WarningType = WarningType.PlateNumberMismatch,
                        Note = $"Biển số đăng ký: {card.PlateNumber}",
                        ImageUrl = createEntryLogDto.ImageUrl
                    });

                    throw new Exception("Biển số khác với biển đăng ký");
                }    
            }    

            // 6. Kiểm tra hiệu lực thẻ (tháng)
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
                        LaneId = createEntryLogDto.LaneId,
                        WarningType = WarningType.CardExpired,
                        Note = "Thẻ đã hết hạn",
                        ImageUrl = createEntryLogDto.ImageUrl
                    });

                    throw new Exception("Vui lòng gia hạn thẻ");
                }
            }

            // 7. Kiểm tra trạng thái làn
            var lane = await _unitOfWork.Lanes.GetByIdAsync(createEntryLogDto.LaneId);
            if (lane == null)
                throw new Exception("Không tìm thấy làn");

            if (!lane.Status)
                throw new Exception("Làn không hoạt động");

            if (lane.Type != LaneType.In && lane.Type != LaneType.KioskIn && lane.Type != LaneType.Dynamic)
                throw new Exception("Không được phép sử dụng làn");

            // 8. Kiểm tra quyền sử dụng làn
            var allowedLaneIds = await _unitOfWork.CardGroups.GetAllowedLaneIdsAsync(cardGroup.Id);
            if (!allowedLaneIds.Contains(createEntryLogDto.LaneId))
            {
                await _warningEventService.CreateAsync(new CreateWarningEventDto
                {
                    PlateNumber = standardizedPlateNumber,
                    LaneId = createEntryLogDto.LaneId,
                    WarningType = WarningType.CardGroupNotAllowedInLane,
                    Note = createEntryLogDto.Note,
                    ImageUrl = createEntryLogDto.ImageUrl
                });

                throw new Exception("Nhóm thẻ không được sử dụng làn");
            }

            // 9. Tạo EntryLog mới
            var entryLog = new EntryLog
            {
                PlateNumber = standardizedPlateNumber,
                CardId = card.Id,
                CardGroupId = card.CardGroupId,
                CustomerId = card.CustomerId,
                LaneId = createEntryLogDto.LaneId,
                EntryTime = DateTime.UtcNow,
                ImageUrl = createEntryLogDto.ImageUrl,
                Note = createEntryLogDto.Note,
                CreatedAt = DateTime.UtcNow,
            };

            card.Status = CardStatus.Active;
            card.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.EntryLogs.CreateAsync(entryLog);
            _unitOfWork.Cards.Update(card);

            await _unitOfWork.WarningEvents.CreateAsync(new WarningEvent
            {
                PlateNumber = entryLog.PlateNumber,
                LaneId = entryLog.LaneId,
                WarningType = WarningType.TicketIssued,
                Note = "Xe đã vào bãi",
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

            var card = await _unitOfWork.Cards.GetByIdAsync(entryLog.CardId);
            if (card != null)
            {
                // Cập nhật trạng thái thẻ thành Inactive
                card.Status = CardStatus.Inactive;
                _unitOfWork.Cards.Update(card);
            }

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
