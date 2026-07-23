using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Product
{
    public class ProductImageResponseDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public Guid ProductId { get; set; }
    }
}
