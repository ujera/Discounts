// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Offer
{
    public class OfferDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int CouponsCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
    }
}
