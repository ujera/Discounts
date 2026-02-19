// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IReservationRepository : IBaseRepository<Reservation>
    {
        Task<Reservation?> GetUserReservationForOfferAsync(string userId, int offerId, CancellationToken ct);
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(CancellationToken ct = default);
    }
}
