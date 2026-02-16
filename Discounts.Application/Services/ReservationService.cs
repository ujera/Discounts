// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Customer;
using Discounts.Application.Exceptions;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Discounts.Domain.Entities;
using Mapster;

namespace Discounts.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReservationResponseDto> ReserveOfferAsync(int offerId, string userId, CancellationToken ct = default)
        {
            var settings = await _unitOfWork.Settings.GetCurrentSettingsAsync(ct).ConfigureAwait(false);
            var durationMinutes = settings?.ReservationDurationInMinutes ?? 30;

            // 2. Start Database Transaction (Critical for Stock Management)
            // We use execution strategy for retries if DB is busy
            using var transaction = await _unitOfWork.BeginTransactionAsync(ct).ConfigureAwait(false);
            try
            {
                var offer = await _unitOfWork.Offers.GetByIdAsync(offerId, ct).ConfigureAwait(false);
                if (offer == null) throw new NotFoundException("Offer", offerId);

                if (offer.CouponsCount <= 0)
                    throw new SoldOutException(offer.Title);

                var existingReservation = await _unitOfWork.Reservations.GetUserReservationForOfferAsync(userId, offerId, ct).ConfigureAwait(false);
                if (existingReservation != null)
                    throw new AlreadyReservedException();

                var reservation = new Reservation
                {
                    OfferId = offerId,
                    UserId = userId,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes)
                };

                offer.CouponsCount--;

                await _unitOfWork.Reservations.AddAsync(reservation, ct).ConfigureAwait(false);
                _unitOfWork.Offers.Update(offer);

                await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
                await transaction.CommitAsync(ct).ConfigureAwait(false);

                return reservation.Adapt<ReservationResponseDto>();
            }
            catch
            {
                await transaction.RollbackAsync(ct).ConfigureAwait(false);
                throw;
            }
        }

        public async Task<CouponDto> PurchaseReservationAsync(int reservationId, string userId, CancellationToken ct = default)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync(ct).ConfigureAwait(false);
            try
            {
                var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId, ct).ConfigureAwait(false);
                if (reservation == null)
                    throw new NotFoundException("Reservation", reservationId);

                if (reservation.UserId != userId)
                    throw new UnauthorizedAccessException("You cannot purchase someone else's reservation.");

                if (reservation.ExpiresAt < DateTime.UtcNow)
                    throw new BadRequestException("Reservation expired. Please reserve again.");

                var couponCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                var coupon = new Coupon
                {
                    Code = couponCode,
                    OfferId = reservation.OfferId,
                    CustomerId = userId,
                    SoldAt = DateTime.UtcNow,
                    IsUsed = false
                };

                await _unitOfWork.Coupons.AddAsync(coupon, ct).ConfigureAwait(false);
                _unitOfWork.Reservations.Remove(reservation);

                await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
                await transaction.CommitAsync(ct).ConfigureAwait(false);

                var offer = await _unitOfWork.Offers.GetByIdAsync(reservation.OfferId, ct).ConfigureAwait(false);

                var dto = coupon.Adapt<CouponDto>();
                dto.OfferTitle = offer?.Title ?? "Unknown Offer";

                return dto;
            }
            catch
            {
                await transaction.RollbackAsync(ct).ConfigureAwait(false);
                throw;
            }
        }

        public async Task<IEnumerable<CouponDto>> GetMyCouponsAsync(string userId, CancellationToken ct = default)
        {
            var coupons = await _unitOfWork.Coupons.GetByCustomerIdAsync(userId, ct).ConfigureAwait(false);
            return coupons.Adapt<IEnumerable<CouponDto>>();
        }

        public async Task CleanupExpiredReservationsAsync(CancellationToken ct = default)
        {
            // Note: For high performance, add "GetExpiredReservationsAsync()" to the Repository 
            // to avoid loading all reservations into memory. 
            // For now, this logic is functionally correct:

            var allReservations = await _unitOfWork.Reservations.GetAllAsync(ct).ConfigureAwait(false);
            var expiredReservations = allReservations.Where(r => r.ExpiresAt < DateTime.UtcNow).ToList();

            if (!expiredReservations.Any()) return;

            foreach (var reservation in expiredReservations)
            {
                var offer = await _unitOfWork.Offers.GetByIdAsync(reservation.OfferId, ct).ConfigureAwait(false);

                if (offer != null)
                {
                    offer.CouponsCount++;
                    _unitOfWork.Offers.Update(offer);
                }
                _unitOfWork.Reservations.Remove(reservation);
            }

            await _unitOfWork.SaveAsync(ct).ConfigureAwait(false);
        }
    }
}
