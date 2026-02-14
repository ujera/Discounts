
using Discounts.Application.DTOs.Offer;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class UpdateOfferValidator : AbstractValidator<UpdateOfferDto>
    {
        public UpdateOfferValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DiscountPrice)
                .LessThan(x => x.OriginalPrice)
                .WithMessage("Discount price must be lower than original.");
        }
    }
}
