// Copyright (C) TBC Bank. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discounts.Application.DTOs.Offer
{
    public class AdminActionDto
    {
        public int OfferId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; } 
    }
}
