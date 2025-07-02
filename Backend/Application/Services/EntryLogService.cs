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
            // Xử lý lưu file ảnh base64 thành file và lấy đường dẫn mới
            if (!string.IsNullOrEmpty(createEntryLogDto.ImageUrl) && createEntryLogDto.ImageUrl.StartsWith("data:image"))
            {
                var imageUrl = await SaveImageFileAsync(createEntryLogDto.ImageUrl);
                createEntryLogDto.ImageUrl = imageUrl; // Gán lại URL ảnh để lưu vào DB
            }

            if (string.IsNullOrEmpty(createEntryLogDto.PlateNumber))
                throw new ArgumentException("Vui lòng nhập biển số xe");

            // Chuẩn hóa biển số xe trước khi kiểm tra và lưu
            createEntryLogDto.PlateNumber = StandardizePlateNumber(createEntryLogDto.PlateNumber);

            if (!IsValidPlateNumberFormat(createEntryLogDto.PlateNumber))
                throw new ArgumentException("Biển số không hợp lệ");


            // 1. Lấy thông tin thẻ
            var card = await _unitOfWork.Cards.GetByIdAsync(createEntryLogDto.CardId);
            if (card == null)
                throw new Exception("Không tìm thấy thẻ");

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

                throw new Exception("Thẻ bị khóa");
            }
            else if (card.Status == CardStatus.Active)
            {
                throw new Exception("Thẻ đang được sử dụng");
            }
            else if (card.Status != CardStatus.Inactive)
            {
                throw new Exception("Thẻ không hợp lệ để vào bãi");
            }

            // 2. Lấy thông tin nhóm thẻ
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(card.CardGroupId);
            if (cardGroup == null)
                throw new Exception("Không tìm thấy nhóm thẻ");

            if (!cardGroup.Status)
                throw new Exception("Nhóm thẻ không hoạt động");

            // Kiểm tra hiệu lực thẻ nếu là thẻ tháng
            if (cardGroup.Type == CardGroupType.Month)
            {
                if (card.StartDate == null || card.EndDate == null)
                    throw new Exception("Thẻ không còn hiệu lực sử dụng");

                var now = DateTime.Now;
                if (card.StartDate > now || card.EndDate < now)
                {
                    await _warningEventService.CreateAsync(new CreateWarningEventDto
                    {
                        PlateNumber = createEntryLogDto.PlateNumber,
                        LaneId = createEntryLogDto.LaneId,
                        WarningType = WarningType.CardExpired,
                        Note = "Thẻ đã hết hạn",
                        ImageUrl = createEntryLogDto.ImageUrl
                    });

                    throw new Exception("Vui lòng gia hạn thẻ");
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

                throw new Exception("Nhóm thẻ không được sử dụng làn");
            }

            // 4. Kiểm tra trạng thái làn
            var lane = await _unitOfWork.Lanes.GetByIdAsync(createEntryLogDto.LaneId);
            if (lane == null)
                throw new Exception("Không tìm thấy làn");

            if (!lane.Status)
                throw new Exception("Làn không hoạt động");

            if (lane.Type != LaneType.In && lane.Type != LaneType.KioskIn && lane.Type != LaneType.Dynamic)
                throw new Exception("Không được phép sử dụng làn");

            // 5. Kiểm tra thẻ đã vào mà chưa ra
            var hasActiveEntry = await _unitOfWork.EntryLogs.HasActiveEntryAsync(card.Id);
            if (hasActiveEntry)
                throw new Exception("Thẻ đang được sử dụng");

            // 6. Kiểm tra biển số xe đã vào mà chưa ra
            if (!string.IsNullOrWhiteSpace(createEntryLogDto.PlateNumber))
            {
                var plateInUse = await _unitOfWork.EntryLogs.IsPlateNumberInUseAsync(createEntryLogDto.PlateNumber);
                if (plateInUse)
                    throw new Exception("Xe hiện đang trong bãi");
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

            card.Status = CardStatus.Active;
            _unitOfWork.Cards.Update(card);

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
            // - 1-2 số + 1 chữ cái + 5-6 số (30A12345)
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{1,2}[A-Z]\d{5,6}$");
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
