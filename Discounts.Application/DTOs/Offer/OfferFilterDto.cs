// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Offer
{
    public class OfferFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaxPrice { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
