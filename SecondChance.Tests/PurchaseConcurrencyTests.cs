using Microsoft.EntityFrameworkCore;
using SecondChance.Application.Services;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Infrastructure.Data;
using SecondChance.Infrastructure.Repository;
using Xunit;

namespace SecondChance.Tests;

public class PurchaseConcurrencyTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public PurchaseConcurrencyTests()
    {
        // 🎯 完美配置：使用独立的内存数据库，并彻底忽略事务在内存中的限制
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"SecondChance_Concurrency_{Guid.NewGuid()}")
            .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Fact]
    public async Task ReserveProductAsync_WithHighConcurrency_ShouldAllowOnlyOneWinnerAndNoOverSell()
    {
       
        using var context = new ApplicationDbContext(_dbOptions);
        var productRepository = new ProductRepository(context);

        
        var purchaseService = new PurchaseService(productRepository, context);

        var testProductId = Guid.NewGuid();
        var targetProduct = new Product
        {
            Id = testProductId,
            Title = "Limited Edition Item",
            Price = 99.99m,
            Stock = 1,
            IsAvailable = true,
            Status = ProductStatus.Available,
            SellerId = "seller-123",
            CategoryId = Guid.NewGuid()
        };

        context.Products.Add(targetProduct);
        await context.SaveChangesAsync();

        int concurrentUserCount = 100;
        var buyerIds = Enumerable.Range(1, concurrentUserCount)
                                .Select(i => $"buyer-guid-{i}")
                                .ToList();

        // === Act (高并发执行阶段) ===
        // 模拟 100 个线程在微秒内同时抢购
        var tasks = buyerIds.Select(buyerId =>
            Task.Run(() => purchaseService.ReserveProductAsync(testProductId, 1, buyerId))
        );

        bool[] results = await Task.WhenAll(tasks);

        using var verifyContext = new ApplicationDbContext(_dbOptions);

        int successCount = results.Count(r => r == true);
        Assert.Equal(1, successCount);

        var productInDb = await verifyContext.Products.FindAsync(testProductId);
        Assert.NotNull(productInDb);
        Assert.Equal(0, productInDb.Stock);
        Assert.Equal(ProductStatus.Unavailable, productInDb.Status);

        var ordersInDb = await verifyContext.Orders.ToListAsync();
        Assert.Single(ordersInDb);

        var winnerOrder = ordersInDb.First();
        Assert.Equal(testProductId, winnerOrder.ProductId);
        Assert.Equal(1, winnerOrder.Quantity);
        Assert.Equal(99.99m, winnerOrder.Price); 
        Assert.Equal(OrderStatus.Pending, winnerOrder.Status);
        Assert.Contains(winnerOrder.BuyerId, buyerIds); 
    }
}