using SecondChance.Application.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<bool> ReserveProductAsync(Guid productId, int quantity, string buyerId);
        Task<IEnumerable<OrderDto>> GetMyPurchasesAsync(string buyerId);
        Task<IEnumerable<OrderDto>> GetMySalesAsync(string sellerId);
        Task<bool> ConfirmPaymentAsync(Guid orderId, string buyerId);
        Task<bool> CancelOrderAsync(Guid orderId, string buyerId);
    }
}
