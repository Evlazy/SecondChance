using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Product
{
    public class ProudctDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Condition { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string SellerId { get; set; } = string.Empty;
        //public string? MainImageUrl {get;set;}
    }
}
