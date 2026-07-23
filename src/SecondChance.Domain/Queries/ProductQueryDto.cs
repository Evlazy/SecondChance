using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Queries
{
    public class ProductQueryDto
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
    }
}
