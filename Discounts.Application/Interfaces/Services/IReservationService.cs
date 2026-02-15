// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Customer;

namespace Discounts.Application.Interfaces.Services
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> ReserveOfferAsync(int offerId, string userId, CancellationToken ct = default);

        Task<CouponDto> PurchaseReservationAsync(int reservationId, string userId, CancellationToken ct = default);

        Task<IEnumerable<CouponDto>> GetMyCouponsAsync(string userId, CancellationToken ct = default);
    }
}
