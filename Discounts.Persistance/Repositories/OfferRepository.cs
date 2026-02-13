// Copyright (C) TBC Bank. All Rights Reserved.


using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Domain.Enums;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class OfferRepository : BaseRepository<Offer>, IOfferRepository
    {
        public OfferRepository(DiscountsManagementContext context) : base(context)
        {
        }

        public async Task<Offer?> GetOfferWithDetailsAsync(int id)
        {
            return await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.Merchant)
                .FirstOrDefaultAsync(o => o.Id == id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Offer>> GetActiveOffersAsync()
        {
            return await _context.Offers
                .Where(o => o.Status == OfferStatus.Active && o.EndDate > DateTime.UtcNow)
                .Include(o => o.Category)
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
