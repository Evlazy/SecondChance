using FluentValidation;
using SecondChance.Application.DTOs.Product;

namespace SecondChance.Application.Validators;

public sealed class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10).MaximumLength(5_000);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(1_000_000);
        RuleFor(x => x.Condition).IsInEnum();
    }
}
