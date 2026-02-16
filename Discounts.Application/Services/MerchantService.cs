// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Merchant;
using Discounts.Application.Interfaces.Repositories;
using Discounts.Application.Interfaces.Services;
using Mapster;

namespace Discounts.Application.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MerchantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MerchantSalesDto>> GetSalesHistoryAsync(string merchantId, CancellationToken ct = default)
        {

            var coupons = await _unitOfWork.Coupons.GetByMerchantIdAsync(merchantId, ct).ConfigureAwait(false);

            return coupons.Adapt<IEnumerable<MerchantSalesDto>>();
        }

        public async Task<MerchantStatsDto> GetDashboardStatsAsync(string merchantId, CancellationToken ct = default)
        {
            var offers = await _unitOfWork.Offers.FindAsync(o => o.MerchantId == merchantId, ct).ConfigureAwait(false);

            var coupons = await _unitOfWork.Coupons.GetByMerchantIdAsync(merchantId, ct).ConfigureAwait(false);

            var stats = new MerchantStatsDto
            {
                TotalOffers = offers.Count(),

                ActiveOffers = offers.Count(o => o.Status == Domain.Enums.OfferStatus.Active),

                TotalCouponsSold = coupons.Count(),

                TotalRevenue = coupons.Sum(c => c.Offer.DiscountPrice)
            };

            return stats;
        }
    }
}
