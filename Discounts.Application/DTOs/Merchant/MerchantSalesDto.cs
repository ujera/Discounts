// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Merchant
{
    public class MerchantSalesDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string OfferTitle { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public DateTime SoldAt { get; set; }
        public decimal PricePaid { get; set; }
    }
}
