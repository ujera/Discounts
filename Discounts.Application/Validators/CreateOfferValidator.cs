using Discounts.Application.DTOs.Offer;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class CreateOfferValidator : AbstractValidator<CreateOfferDto>
    {
        public CreateOfferValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.OriginalPrice)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.DiscountPrice)
                .GreaterThan(0)
                .LessThan(x => x.OriginalPrice)
                .WithMessage("Discount price must be lower than the original price.");

            RuleFor(x => x.CouponsCount)
                .GreaterThan(0).WithMessage("You must offer at least one coupon.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date cannot be in the past.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
        }
    }
}
