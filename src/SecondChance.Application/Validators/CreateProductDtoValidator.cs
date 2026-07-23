using FluentValidation;
using SecondChance.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title can not be empty")
                .MinimumLength(100).WithMessage("Title can not over 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description can not be empty")
                .MinimumLength(10).WithMessage("Description at least 10 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must bigger than 0");

            RuleFor(x => x.Condition)
                .IsInEnum().WithMessage("Please select correct condition");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Product must belong to one category");
        }
    }
}
