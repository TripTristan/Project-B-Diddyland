using System;
using System.Collections.Generic;
using System.Linq;
using MyProject.DAL;

namespace MyProject.BLL
{
    public static class OfferManagementLogic
    {
        public static List<OfferListItemDto> GetAllOffers()
            => OfferManagementRepository.GetAllOffers()
                .Select(o => new OfferListItemDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Discount = o.Discount,
                    IsActive = o.IsActive,
                    Type = o.GetType().Name.Replace("Offer", "")
                })
                .ToList();

        public static OfferDto? GetOfferById(int id)
        {
            var offer = OfferManagementRepository.GetOfferById(id);
            return offer == null ? null : MapToDto(offer);
        }

        public static OfferValidationResultDto AddOffer(OfferDto dto)
        {
            var valid = ValidateOffer(dto);
            if (!valid.IsSuccess) return valid;

            try
            {
                var entity = MapFromDto(dto);
                int newId = OfferManagementRepository.InsertOffer(entity);
                return new OfferValidationResultDto { IsSuccess = true, Message = $"Offer added (ID={newId})." };
            }
            catch (Exception ex)
            {
                return new OfferValidationResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        public static OfferValidationResultDto UpdateOffer(int id, OfferDto dto)
        {
            if (OfferManagementRepository.GetOfferById(id) == null)
                return new OfferValidationResultDto { IsSuccess = false, Message = "Offer not found." };

            var valid = ValidateOffer(dto);
            if (!valid.IsSuccess) return valid;

            try
            {
                var entity = MapFromDto(dto, id);
                OfferManagementRepository.UpdateOffer(entity);
                return new OfferValidationResultDto { IsSuccess = true, Message = "Offer updated." };
            }
            catch (Exception ex)
            {
                return new OfferValidationResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        public static OfferValidationResultDto DeleteOffer(int id)
        {
            if (OfferManagementRepository.GetOfferById(id) == null)
                return new OfferValidationResultDto { IsSuccess = false, Message = "Offer not found." };

            int rows = OfferManagementRepository.DeleteOffer(id);
            return rows > 0
                ? new OfferValidationResultDto { IsSuccess = true, Message = "Offer deleted." }
                : new OfferValidationResultDto { IsSuccess = false, Message = "Delete failed." };
        }

        public static OfferValidationResultDto ToggleOfferStatus(int id)
        {
            var offer = OfferManagementRepository.GetOfferById(id);
            if (offer == null)
                return new OfferValidationResultDto { IsSuccess = false, Message = "Offer not found." };

            int rows = OfferManagementRepository.ToggleActive(id, !offer.IsActive);
            return rows > 0
                ? new OfferValidationResultDto { IsSuccess = true, Message = "Status toggled." }
                : new OfferValidationResultDto { IsSuccess = false, Message = "Toggle failed." };
        }

        private static OfferValidationResultDto ValidateOffer(OfferDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return new OfferValidationResultDto { IsSuccess = false, Message = "Name is required." };
            if (dto.Discount < 0 || dto.Discount > 1)
                return new OfferValidationResultDto { IsSuccess = false, Message = "Discount must be 0-1." };
            if (dto.StartDate > dto.EndDate)
                return new OfferValidationResultDto { IsSuccess = false, Message = "StartDate > EndDate." };
            return new OfferValidationResultDto { IsSuccess = true };
        }

        private static OfferDto MapToDto(OfferBase offer)
        {
            return new OfferDto
            {
                Id = offer.Id,
                Nr = offer.Nr,
                Name = offer.Name,
                Description = offer.Description,
                Discount = offer.Discount,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive,
                Max = offer.Max,
                Min = offer.Min,
                DaysBeforeExpiry = offer.DaysBeforeExpiry,
                Type = offer is OfferGroup ? OfferType.Group :
                        offer is OfferPromoCode ? OfferType.PromoCode :
                        offer is OfferVIP ? OfferType.VIP :
                        OfferType.Age,
                GroupType = (offer as OfferGroup)?.GroupType
            };
        }

        private static OfferBase MapFromDto(OfferDto dto, int? id = null)
        {
            OfferBase entity = dto.Type switch
            {
                OfferType.Group => new OfferGroup(dto.Nr, dto.Name, dto.Description, dto.Discount,
                    dto.StartDate, dto.EndDate, dto.IsActive, dto.DaysBeforeExpiry,
                    dto.Max ?? 0, dto.Min ?? 0, dto.GroupType ?? GroupType.Company),

                OfferType.PromoCode => new OfferPromoCode(dto.Nr, dto.PromoCode!, dto.ExpiryDate,
                    dto.MaxUses, dto.CurrentUses, dto.Discount),

                OfferType.VIP => new OfferVIP(dto.Nr, dto.Name, dto.Description, dto.Discount,
                    dto.StartDate, dto.EndDate, dto.IsActive, dto.DaysBeforeExpiry),

                _ => new OfferBase(dto.Nr, dto.Name, dto.Description, dto.Discount,
                    dto.StartDate, dto.EndDate, dto.Max ?? 0, dto.Min ?? 0, dto.IsActive, dto.DaysBeforeExpiry)
            };

            if (id.HasValue) entity.Id = id.Value;
            return entity;
        }
    }
}