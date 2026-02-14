// Copyright (C) TBC Bank. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discounts.Application.DTOs.Auth;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Role)
                .Must(r => r == "Merchant" || r == "Customer")
                .WithMessage("Role must be 'Merchant' or 'Customer'");
        }
    }
}
