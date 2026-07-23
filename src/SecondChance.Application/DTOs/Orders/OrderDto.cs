using SecondChance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.DTOs.Orders
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
