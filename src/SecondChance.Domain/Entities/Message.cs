using SecondChance.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public class Message : AuditableEntity
    {
        public Guid ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public virtual ApplicationUser Sender { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; } = false;
    }
}
