// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(DateTime expirationThreshold, CancellationToken ct);
        Task<Reservation?> GetUserReservationForOfferAsync(string userId, int offerId, CancellationToken ct);
    }
}
