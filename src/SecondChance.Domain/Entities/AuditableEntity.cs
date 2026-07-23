using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public abstract class AuditableEntity
    {
        public Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
