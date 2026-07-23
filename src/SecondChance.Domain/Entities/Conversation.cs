using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public class Conversation : AuditableEntity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public string BuyerId { get; set; } = null!;
        public virtual ApplicationUser Buyer { get; set; } = null!;
        public string SellerId { get; set; } = null!;
        public virtual ApplicationUser Seller { get; set; } = null!;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
