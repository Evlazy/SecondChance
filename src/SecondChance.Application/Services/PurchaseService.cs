using Microsoft.EntityFrameworkCore;
using SecondChance.Application.DTOs.Orders;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Domain.Interfaces;

namespace SecondChance.Application.Services;

public sealed class PurchaseService : IPurchaseService
{
    // Keeps the in-memory test provider (which shares one DbContext across parallel tasks) safe.
    // Relational deployments still use the atomic stock update as the source of truth.
    private static readonly SemaphoreSlim ReservationGate = new(1, 1);
    private readonly IProductRepository _productRepo;
    private readonly IApplicationDbContext _context;

    public PurchaseService(IProductRepository productRepo, IApplicationDbContext context)
    {
        _productRepo = productRepo;
        _context = context;
    }

    public async Task<bool> ReserveProductAsync(Guid productId, int quantity, string buyerId)
    {
        await ReservationGate.WaitAsync();
        try
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
            if (product is null || product.SellerId == buyerId || quantity <= 0)
                return false;

            await _context.BeginTransactionAsync();
            if (!await _productRepo.DecreaseStockAsync(productId, quantity))
            {
                await _context.RollbackTransactionAsync();
                return false;
            }

            _context.Orders.Add(new Order
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                SellerId = product.SellerId,
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = buyerId
            });

            await _context.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.RollbackTransactionAsync();
            throw;
        }
        finally
        {
            ReservationGate.Release();
        }
    }

    public async Task<bool> ConfirmPaymentAsync(Guid orderId, string buyerId)
    {
        await _context.BeginTransactionAsync();
        try
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order is null || order.BuyerId != buyerId || order.Status != OrderStatus.Pending)
            {
                await _context.RollbackTransactionAsync();
                return false;
            }

            // Only call this after a payment-provider webhook has been verified.
            order.Status = OrderStatus.Paid;
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
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order is null || order.BuyerId != buyerId || order.Status != OrderStatus.Pending)
            {
                await _context.RollbackTransactionAsync();
                return false;
            }

            var product = await _context.Products.FindAsync(order.ProductId);
            if (product is null)
            {
                await _context.RollbackTransactionAsync();
                return false;
            }

            order.Status = OrderStatus.Cancelled;
            product.Stock += order.Quantity;
            product.IsAvailable = true;
            product.Status = ProductStatus.Available;
            await _context.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _context.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<OrderDto>> GetMyPurchasesAsync(string buyerId) =>
        await _context.Orders.AsNoTracking().Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedAt).Select(ToOrderDto()).ToListAsync();

    public async Task<IEnumerable<OrderDto>> GetMySalesAsync(string sellerId) =>
        await _context.Orders.AsNoTracking().Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.CreatedAt).Select(ToOrderDto()).ToListAsync();

    private static System.Linq.Expressions.Expression<Func<Order, OrderDto>> ToOrderDto() => o => new OrderDto
    {
        Id = o.Id, BuyerId = o.BuyerId, SellerId = o.SellerId, ProductId = o.ProductId,
        ProductName = o.Product != null ? o.Product.Title : "Unknown product",
        Price = o.Price, Quantity = o.Quantity, Status = o.Status, CreatedAt = o.CreatedAt
    };
}
