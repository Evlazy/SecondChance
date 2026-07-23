using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Shipped = 2,
        Completed = 3,
        Cancelled = 4
    }
}
