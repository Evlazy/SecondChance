using FluentValidation;
using Microsoft.AspNetCore.Http;
using SecondChance.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;


namespace SecondChance.Application.Validators
{
    public class UploadproductImageDtoValidator : AbstractValidator<UploadProductImageDto>
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        private const long MaxFileSize = 2 * 1024 * 1024;

        public UploadproductImageDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Uploading image can not be empty")
                .Must(HaveValidExtension).WithMessage($"Do not support this format, only {string.Join(",", AllowedExtensions)}")
                .Must(HaveValidSize).WithMessage("Image size can not over 2MB");
        }

        private bool HaveValidExtension(IFormFile file)
        {
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return false;

            return file.Length <= MaxFileSize;
        }
    }
}
