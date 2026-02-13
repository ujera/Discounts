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

        public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(DateTime expirationThreshold)
        {
            return await _context.Reservations
                .Where(r => r.ReservedAt < expirationThreshold)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<Reservation?> GetUserReservationForOfferAsync(string userId, int offerId)
        {
            return await _context.Reservations
                .FirstOrDefaultAsync(r => r.UserId == userId && r.OfferId == offerId).ConfigureAwait(false);
        }
    }
}
