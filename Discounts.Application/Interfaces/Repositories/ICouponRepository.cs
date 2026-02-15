// Copyright (C) TBC Bank. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IBaseRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct);
        Task<IEnumerable<Coupon>> GetByCustomerIdAsync(string customerId, CancellationToken ct);
    }
}
