// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(DiscountsManagementContext context) : base(context) { }

        public async Task<Reservation?> GetUserReservationForOfferAsync(string userId, int offerId, CancellationToken ct)
        {
            return await _context.Reservations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.OfferId == offerId, ct).ConfigureAwait(false);
        }
        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.ExpiresAt < DateTime.UtcNow)
                .ToListAsync(ct).ConfigureAwait(false);
        }
    }
}
