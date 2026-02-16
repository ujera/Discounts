// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Merchant
{
    public class MerchantStatsDto
    {
        public int TotalOffers { get; set; }
        public int ActiveOffers { get; set; }
        public int TotalCouponsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
