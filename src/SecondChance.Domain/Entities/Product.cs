using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public ProductCondition Condition { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Available;
        public Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public string SellerId { get; set; } = string.Empty;
        public int Stock { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public bool IsAvailable { get; set; } = true;
        public string? AuditReason { get; set; }
    }

    public enum ProductCondition
    {
        BrandNew,
        LikeNew,
        GentlyUsed,
        WellUsed
    }

    public enum ProductStatus
    {
        Unavailable,
        Available,
        Reserved,
        Sold
    }
}
