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
    }
}
