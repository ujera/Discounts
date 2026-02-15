// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Offer;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class AdminActionValidator : AbstractValidator<AdminActionDto>
    {
        public AdminActionValidator()
        {
            RuleFor(x => x.OfferId)
                .GreaterThan(0).WithMessage("Invalid Offer ID.");

            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .When(x => x.IsApproved == false)
                .WithMessage("You must provide a reason when rejecting an offer.");
        }
    }
}
