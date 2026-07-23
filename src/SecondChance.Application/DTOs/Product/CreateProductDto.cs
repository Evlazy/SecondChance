using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Product
{
    public class CreateProductDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ProductCondition Condition { get; set; }
        public Guid CategoryId { get; set; }
    }
}
