using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Common
{
    public interface IAuditable
    {
        string? CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
        string? LastModifiedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
    }
}
