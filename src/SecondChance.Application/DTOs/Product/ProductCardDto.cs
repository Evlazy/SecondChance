using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Product
{
    public class ProductCardDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime FavoritedAt { get; set; }
    }
}
