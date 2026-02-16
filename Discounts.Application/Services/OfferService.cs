// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Application.Pagination;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Mapster;

namespace Discounts.Application.Services
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OfferDto> CreateAsync(CreateOfferDto dto, string merchantId, CancellationToken ct)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, ct).ConfigureAwait(false);
            if (category == null)
                throw new NotFoundException("Category", dto.CategoryId);

            var offer = dto.Adapt<Offer>();
            offer.MerchantId = merchantId;
            offer.Status = OfferStatus.Pending;
            offer.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Offers.AddAsync(offer, ct).ConfigureAwait(false);
            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);

            return offer.Adapt<OfferDto>();
        }

        public async Task UpdateAsync(int id, UpdateOfferDto dto, string merchantId, CancellationToken ct)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(id, ct).ConfigureAwait(false);

            if (offer == null)
                throw new NotFoundException("Offer", id);

            if (offer.MerchantId != merchantId)
                throw new UnauthorizedAccessException("You do not own this offer.");

            var settings = await _unitOfWork.Settings.GetCurrentSettingsAsync(ct).ConfigureAwait(false);
            var editWindow = settings?.MerchantEditWindowInHours ?? 24;

            if (offer.CreatedAt.AddHours(editWindow) < DateTime.UtcNow)
            {
                throw new OfferNotEditableException($"The {editWindow}-hour edit window has passed.");
            }

            dto.Adapt(offer);

            _unitOfWork.Offers.Update(offer);
            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }

        public async Task<PagedResult<OfferDto>> GetAllActiveAsync(OfferFilterDto filter, CancellationToken ct)
        {
            var pagedData = await _unitOfWork.Offers.GetPagedOffersAsync(filter, ct).ConfigureAwait(false);

            var offerDtos = pagedData.Items.Adapt<IEnumerable<OfferDto>>();

            return new PagedResult<OfferDto>(
                offerDtos,
                pagedData.TotalCount,
                pagedData.PageNumber,
                pagedData.PageSize
            );
        }

        public async Task<OfferDto> GetByIdAsync(int id, CancellationToken ct)
        {
            var offer = await _unitOfWork.Offers.GetOfferWithDetailsAsync(id, ct).ConfigureAwait(false);
            if (offer == null)
                throw new NotFoundException("Offer", id);

            return offer.Adapt<OfferDto>();
        }

        public async Task<IEnumerable<OfferDto>> GetMyOffersAsync(string merchantId, CancellationToken ct)
        {
            var allOffers = await _unitOfWork.Offers.FindAsync(o => o.MerchantId == merchantId, ct).ConfigureAwait(false);
            return allOffers.Adapt<IEnumerable<OfferDto>>();
        }

        public async Task ApproveOfferAsync(AdminActionDto dto, CancellationToken ct)
        {
            var offer = await _unitOfWork.Offers.GetByIdAsync(dto.OfferId, ct).ConfigureAwait(false);

            if (offer == null)
                throw new NotFoundException("Offer", dto.OfferId);

            if (offer.Status == OfferStatus.Active && dto.IsApproved)
                throw new BadRequestException("Offer is already active.");

            if (offer.Status == OfferStatus.Rejected && !dto.IsApproved)
                throw new BadRequestException("Offer is already rejected.");

            if (dto.IsApproved)
            {
                offer.Status = OfferStatus.Active;
                offer.RejectionReason = null;
            }
            else
            {
                offer.Status = OfferStatus.Rejected;
                offer.RejectionReason = dto.RejectionReason;
            }

            _unitOfWork.Offers.Update(offer);

            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }

        public async Task CleanupExpiredOffersAsync(CancellationToken ct = default)
        {
            var expiredOffers = await _unitOfWork.Offers.FindAsync(
                o => o.Status == OfferStatus.Active && o.EndDate < DateTime.UtcNow,
                ct
            ).ConfigureAwait(false);

            if (!expiredOffers.Any()) return;

            foreach (var offer in expiredOffers)
            {
                offer.Status = Domain.Enums.OfferStatus.Expired;
            }

            _unitOfWork.Offers.UpdateRange(expiredOffers);
            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }
    }
}
