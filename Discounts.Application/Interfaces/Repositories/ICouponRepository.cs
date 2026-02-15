// Copyright (C) TBC Bank. All Rights Reserved.
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IBaseRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct);
        Task<IEnumerable<Coupon>> GetByCustomerIdAsync(string customerId, CancellationToken ct);
        Task<IEnumerable<Coupon>> GetByMerchantIdAsync(string merchantId, CancellationToken ct = default);
    }
}
