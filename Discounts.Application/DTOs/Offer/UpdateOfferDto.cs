// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Offer
{
    public class UpdateOfferDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public int CouponsCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
    }
}
