// Copyright (C) TBC Bank. All Rights Reserved.

namespace Discounts.Application.DTOs.Offer
{
    public class AdminActionDto
    {
        public int OfferId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; } 
    }
}
