// Copyright (C) TBC Bank. All Rights Reserved.

using Discounts.Application.DTOs.Admin;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class SystemSettingsValidator : AbstractValidator<SystemSettingsDto>
    {
        public SystemSettingsValidator()
        {
            RuleFor(x => x.ReservationDurationInMinutes)
                .InclusiveBetween(5, 1440)
                .WithMessage("Reservation time must be reasonable (5 mins - 24 hours).");

            RuleFor(x => x.MerchantEditWindowInHours)
                .InclusiveBetween(1, 168)
                .WithMessage("Edit window must be between 1 hour and 1 week.");
        }
    }
}
