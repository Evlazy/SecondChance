using SecondChance.Application.DTOs.Orders;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SecondChance.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IProductRepository _productRepo;
        private readonly IApplicationDbContext _context;
        public PurchaseService(IProductRepository productRepo, IApplicationDbContext context)
        {
            _productRepo = productRepo;
            _context = context;
        }

        public async Task<bool> ConfirmPaymentAsync(Guid orderId, string buyerId)
        {
            await _context.BeginTransactionAsync();

            try
            {
                var order = _context.Orders
                    .Where(o => o.Id == orderId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (order == null || order.BuyerId != buyerId || order.Status != OrderStatus.Pending)
                {
                    return false;
                }

                order.Status = OrderStatus.Paid;
                _context.Orders.Update(order);

                var product = await _context.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.Status = ProductStatus.Unavailable;
                    _context.Products.Update(product);
                }

                await _context.SaveChangesAsync();
                await _context.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _context.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(Guid orderId, string buyerId)
        {
            await _context.BeginTransactionAsync();

            try
            {
                var order = _context.Orders
                    .Where(o => o.Id == orderId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (order == null || order.BuyerId != buyerId || order.Status != OrderStatus.Pending)
                {
                    return false;
                }

                order.Status = OrderStatus.Cancelled;
                _context.Orders.Update(order);

                var product = await _context.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.Stock += order.Quantity; 

                    if (product.Stock > 0 && product.Status == ProductStatus.Unavailable)
                    {
                        product.Status = ProductStatus.Available;
                    }
                    _context.Products.Update(product);
                }

                await _context.SaveChangesAsync();
                await _context.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _context.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderDto>> GetMyPurchasesAsync(string buyerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    BuyerId = o.BuyerId,
                    SellerId = o.SellerId,
                    ProductId = o.ProductId,

                    ProductName = o.Product != null ? o.Product.Title : "Unknown Product",

                    Price = o.Price,
                    Quantity = o.Quantity,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(); 
        }

        public async Task<IEnumerable<OrderDto>> GetMySalesAsync(string sellerId)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.SellerId == sellerId)
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedAt);

            var ordersList = query.AsEnumerable();

            return ordersList.Select(o => new OrderDto
            {
                Id = o.Id,
                BuyerId = o.BuyerId,
                SellerId = o.SellerId,
                ProductId = o.ProductId,
                ProductName = o.Product != null ? o.Product.Title : "Unknown Product",
                Price = o.Price,
                Quantity = o.Quantity,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            }).ToList();
        }

        public async Task<bool> ReserveProductAsync(Guid productId, int quantity, string buyerId)
        {
            await _context.BeginTransactionAsync();

            try
            {

            var isStockSecured = await _productRepo.DecreaseStockAsync(productId, quantity);

            if (!isStockSecured)
            {
                return false;
            }

            var product = await _context.Products.FindAsync(productId);
            if(product == null)
            {
                throw new Exception("Product unexpectedly not found after locking stock.");
            }

            var newOrder = new Order
            {
                BuyerId = buyerId,
                SellerId = product.SellerId,
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price,
                Status = Domain.Enums.OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = buyerId
            };

            _context.Orders.Add(newOrder);

            await _context.CommitTransactionAsync();

            return true;
            }
            catch (Exception)
            {
                await _context.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
