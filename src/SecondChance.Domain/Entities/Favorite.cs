using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public class Favorite
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
