using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IAdminProductService
    {
        Task<bool> FreezeProductAsync(Guid productId, string reason);
        Task<bool> UnFreezeProductAsync(Guid productId);
    }
}
