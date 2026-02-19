// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using Discounts.Application.Pagination;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface IOfferRepository : IBaseRepository<Offer>
    {
        //offers with their Category and Merchant loaded
        Task<Offer?> GetOfferWithDetailsAsync(int id, CancellationToken ct);

        //all active offers for the homepage
        Task<IEnumerable<Offer>> GetActiveOffersAsync(CancellationToken ct);
        Task<PagedResult<Offer>> GetPagedOffersAsync(OfferFilterDto filter, CancellationToken ct);
    }
}
