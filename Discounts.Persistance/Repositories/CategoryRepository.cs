// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.Interfaces.Repositories;
using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Persistance.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DiscountsManagementContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesWithActiveOffersAsync()
        {
            return await _context.Categories
                .Where(c => c.Offers.Any(o => o.EndDate > DateTime.UtcNow))
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
