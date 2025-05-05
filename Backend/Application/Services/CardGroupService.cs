using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;
using Final_year_Project.Application.Models;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Final_year_Project.Application.Services
{
    public class CardGroupService : ICardGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CardGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CardGroupDto>> GetAllAsync()
        {
            var cardGroups = await _unitOfWork.CardGroups.GetAllAsync();
            var cardGroupsDtos = new List<CardGroupDto>();

            foreach (var cardGroup in cardGroups)
            {
                cardGroupsDtos.Add(new CardGroupDto
                {
                    Id = cardGroup.Id,
                    Name = cardGroup.Name,
                    Code = cardGroup.Code,
                    Type = cardGroup.Type,
                    VehicleType = cardGroup.VehicleType,
                    FreeMinutes = cardGroup.FreeMinutes,
                    FirstBlockMinutes = cardGroup.FirstBlockMinutes,
                    FirstBlockPrice = cardGroup.FirstBlockPrice,
                    NextBlockMinutes = cardGroup.NextBlockMinutes,
                    NextBlockPrice = cardGroup.NextBlockPrice,
                    Status = cardGroup.Status,
                    CreatedAt = cardGroup.CreatedAt,
                    UpdatedAt = cardGroup.UpdatedAt
                });
            }

            return cardGroupsDtos;
        }

        public async Task<CardGroupDto> GetByIdAsync(int id)
        {
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(id);

            if (cardGroup == null)
                return null;

            return new CardGroupDto
            {
                Id = cardGroup.Id,
                Name = cardGroup.Name,
                Code = cardGroup.Code,
                Type = cardGroup.Type,
                VehicleType = cardGroup.VehicleType,
                FreeMinutes = cardGroup.FreeMinutes,
                FirstBlockMinutes = cardGroup.FirstBlockMinutes,
                FirstBlockPrice = cardGroup.FirstBlockPrice,
                NextBlockMinutes = cardGroup.NextBlockMinutes,
                NextBlockPrice = cardGroup.NextBlockPrice,
                Status = cardGroup.Status,
                CreatedAt = cardGroup.CreatedAt,
                UpdatedAt = cardGroup.UpdatedAt
            };
        }

        public async Task<CardGroupDto> CreateAsync(CreateCardGroupDto createCardGroupDto)
        {
            var cardGroup = new CardGroup
            {
                Name = createCardGroupDto.Name,
                Code = createCardGroupDto.Code,
                Type = createCardGroupDto.Type,
                VehicleType = createCardGroupDto.VehicleType,
                FreeMinutes = createCardGroupDto.FreeMinutes,
                FirstBlockMinutes = createCardGroupDto.FirstBlockMinutes,
                FirstBlockPrice = createCardGroupDto.FirstBlockPrice,
                NextBlockMinutes = createCardGroupDto.NextBlockMinutes,
                NextBlockPrice = createCardGroupDto.NextBlockPrice,
                Status = createCardGroupDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            if (createCardGroupDto.LaneIds != null && createCardGroupDto.LaneIds.Any())
            {
                foreach (var laneId in createCardGroupDto.LaneIds)
                {
                    var cardGroupLane = new CardGroupLane
                    {
                        LaneId = laneId
                    };
                    cardGroup.CardGroupLanes.Add(cardGroupLane);
                }    
            }    

            await _unitOfWork.CardGroups.CreateAsync(cardGroup);
            await _unitOfWork.SaveChangesAsync();

            var laneIds = cardGroup.CardGroupLanes.Select(cgl => cgl.LaneId).ToList();

            return new CardGroupDto
            {
                Id = cardGroup.Id,
                Name = cardGroup.Name,
                Code = cardGroup.Code,
                Type = cardGroup.Type,
                VehicleType = cardGroup.VehicleType,
                FreeMinutes = cardGroup.FreeMinutes,
                FirstBlockMinutes = cardGroup.FirstBlockMinutes,
                FirstBlockPrice = cardGroup.FirstBlockPrice,
                NextBlockMinutes = cardGroup.NextBlockMinutes,
                NextBlockPrice = cardGroup.NextBlockPrice,
                Status = cardGroup.Status,
                CreatedAt = cardGroup.CreatedAt,
                UpdatedAt = cardGroup.UpdatedAt,
                LaneIds = laneIds
            };
        }

        public async Task<CardGroupDto> UpdateAsync(int id, UpdateCardGroupDto updateCardGroupDto)
        {
            var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(id);
            if (cardGroup == null)
                return null;

            // Cập nhật thông tin cơ bản
            cardGroup.Name = updateCardGroupDto.Name;
            cardGroup.Type = updateCardGroupDto.Type;
            cardGroup.VehicleType = updateCardGroupDto.VehicleType;
            cardGroup.FreeMinutes = updateCardGroupDto.FreeMinutes;
            cardGroup.FirstBlockMinutes = updateCardGroupDto.FirstBlockMinutes;
            cardGroup.FirstBlockPrice = updateCardGroupDto.FirstBlockPrice;
            cardGroup.NextBlockMinutes = updateCardGroupDto.NextBlockMinutes;
            cardGroup.NextBlockPrice = updateCardGroupDto.NextBlockPrice;
            cardGroup.Status = updateCardGroupDto.Status;
            cardGroup.UpdatedAt = DateTime.UtcNow;

            // Cập nhật danh sách Lane
            // Lấy tất cả CardGroupLane hiện tại của CardGroup này
            var currentCardGroupLanes = (await _unitOfWork.CardGroupLanes.GetAllAsync())
                .Where(cgl => cgl.CardGroupId == id)
                .ToList();

            // Xóa các liên kết không còn trong danh sách mới
            foreach (var cardGroupLane in currentCardGroupLanes)
            {
                if (updateCardGroupDto.LaneIds == null || !updateCardGroupDto.LaneIds.Contains(cardGroupLane.LaneId))
                {
                    _unitOfWork.CardGroupLanes.Delete(cardGroupLane);
                }
            }

            // Thêm các liên kết mới
            if (updateCardGroupDto.LaneIds != null)
            {
                var existingLaneIds = currentCardGroupLanes.Select(cgl => cgl.LaneId).ToList();
                var laneIdsToAdd = updateCardGroupDto.LaneIds.Except(existingLaneIds).ToList();

                foreach (var laneId in laneIdsToAdd)
                {
                    await _unitOfWork.CardGroupLanes.CreateAsync(new CardGroupLane
                    {
                        CardGroupId = cardGroup.Id,
                        LaneId = laneId
                    });
                }
            }

            _unitOfWork.CardGroups.Update(cardGroup);
            await _unitOfWork.SaveChangesAsync();

            // Lấy lại danh sách LaneIds sau khi cập nhật
            var updatedCardGroupLanes = (await _unitOfWork.CardGroupLanes.GetAllAsync())
                .Where(cgl => cgl.CardGroupId == id)
                .ToList();
            var laneIds = updatedCardGroupLanes.Select(cgl => cgl.LaneId).ToList();

            return new CardGroupDto
            {
                Id = cardGroup.Id,
                Name = cardGroup.Name,
                Code = cardGroup.Code,
                Type = cardGroup.Type,
                VehicleType = cardGroup.VehicleType,
                FreeMinutes = cardGroup.FreeMinutes,
                FirstBlockMinutes = cardGroup.FirstBlockMinutes,
                FirstBlockPrice = cardGroup.FirstBlockPrice,
                NextBlockMinutes = cardGroup.NextBlockMinutes,
                NextBlockPrice = cardGroup.NextBlockPrice,
                Status = cardGroup.Status,
                CreatedAt = cardGroup.CreatedAt,
                UpdatedAt = cardGroup.UpdatedAt,
                LaneIds = laneIds
            };
        }


        public async Task<bool> DeleteAsync(int id, bool useSoftDelete)
        {
            try
            {
                var cardGroup = await _unitOfWork.CardGroups.GetByIdAsync(id);
                if (cardGroup == null) return false;

                if (useSoftDelete)
                {
                    cardGroup.Deleted = true;
                    cardGroup.UpdatedAt = DateTime.UtcNow;

                    if (cardGroup.Cards != null && cardGroup.Cards.Any())
                    {
                        foreach (var card in cardGroup.Cards)
                        {
                            card.UpdatedAt = DateTime.UtcNow;
                            _unitOfWork.Cards.Update(card);
                        }
                    }

                    if (cardGroup.CardGroupLanes != null && cardGroup.CardGroupLanes.Any())
                    {
                        foreach (var cardGroupLane in cardGroup.CardGroupLanes)
                        {
                            _unitOfWork.CardGroupLanes.Delete(cardGroupLane);
                        }
                    }

                    _unitOfWork.CardGroups.Update(cardGroup);
                }
                else
                {
                    if (cardGroup.Cards != null && cardGroup.Cards.Any())
                    {
                        foreach (var card in cardGroup.Cards)
                        {
                            _unitOfWork.Cards.Delete(card);
                        }
                    }

                    if (cardGroup.CardGroupLanes != null && cardGroup.CardGroupLanes.Any())
                    {
                        foreach (var cardGroupLane in cardGroup.CardGroupLanes)
                        {
                            _unitOfWork.CardGroupLanes.Delete(cardGroupLane);
                        }
                    }

                    _unitOfWork.CardGroups.Delete(cardGroup);
                }
                await _unitOfWork.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
