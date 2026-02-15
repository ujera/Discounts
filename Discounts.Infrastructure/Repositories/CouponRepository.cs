// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(DiscountsManagementContext context) : base(context)
        {
        }

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct)
        {
            return await _context.Coupons
                .Include(c => c.Offer)
                .FirstOrDefaultAsync(c => c.Code == code, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Coupon>> GetByCustomerIdAsync(string customerId, CancellationToken ct)
        {
            return await _context.Coupons
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Offer)
                .ToListAsync(ct).ConfigureAwait(false);
        }
    }
}
