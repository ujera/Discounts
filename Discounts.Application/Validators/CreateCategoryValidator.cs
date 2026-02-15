using Discounts.Application.DTOs.Category;
using FluentValidation;

namespace Discounts.Application.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.")
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Special characters are not allowed.");
        }
    }
}
