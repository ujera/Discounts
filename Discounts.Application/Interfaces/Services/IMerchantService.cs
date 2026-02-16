// Copyright (C) TBC Bank. All Rights Reserved.
using Discounts.Application.DTOs.Merchant;

namespace Discounts.Application.Interfaces.Services
{
    public interface IMerchantService
    {
        Task<IEnumerable<MerchantSalesDto>> GetSalesHistoryAsync(string merchantId, CancellationToken ct = default);
        Task<MerchantStatsDto> GetDashboardStatsAsync(string merchantId, CancellationToken ct = default);
    }
}
