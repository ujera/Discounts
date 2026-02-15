// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using Discounts.Application.Pagination;

namespace Discounts.Application.Interfaces.Services
{
    public interface IOfferService
    {
        // Customer
        Task<PagedResult<OfferDto>> GetAllActiveAsync(OfferFilterDto filter, CancellationToken ct);
        Task<OfferDto> GetByIdAsync(int id, CancellationToken ct);

        // Merchant
        Task<OfferDto> CreateAsync(CreateOfferDto dto, string merchantId, CancellationToken ct);
        Task UpdateAsync(int id, UpdateOfferDto dto, string merchantId, CancellationToken ct);
        Task<IEnumerable<OfferDto>> GetMyOffersAsync(string merchantId, CancellationToken ct);

        // Admin
        Task ApproveOfferAsync(AdminActionDto dto, CancellationToken ct);
    }
}
