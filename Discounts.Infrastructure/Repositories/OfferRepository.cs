// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Pagination;
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

        public async Task<Offer?> GetOfferWithDetailsAsync(int id, CancellationToken ct)
        {
            return await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.Merchant)
                .FirstOrDefaultAsync(o => o.Id == id,ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Offer>> GetActiveOffersAsync(CancellationToken ct)
        {
            return await _context.Offers
                .Where(o => o.Status == OfferStatus.Active && o.EndDate > DateTime.UtcNow)
                .Include(o => o.Category)
                .ToListAsync(ct).ConfigureAwait(false);
        }

        public async Task<PagedResult<Offer>> GetPagedOffersAsync(OfferFilterDto filter, CancellationToken ct)
        {
            var query = _context.Offers
                .Include(o => o.Category)
                .Include(o => o.Merchant)
                .Where(o => o.Status == OfferStatus.Active && o.EndDate > DateTime.UtcNow)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(o => o.Title.Contains(filter.SearchTerm));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(o => o.CategoryId == filter.CategoryId.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(o => o.DiscountPrice <= filter.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync().ConfigureAwait(false);

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct).ConfigureAwait(false);

            return new PagedResult<Offer>(items, totalCount, filter.PageNumber, filter.PageSize);
        }
    }
}
