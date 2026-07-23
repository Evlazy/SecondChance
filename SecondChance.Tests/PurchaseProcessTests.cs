using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SecondChance.Application.Services;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Infrastructure.Data;
using SecondChance.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Tests
{
    public class PurchaseProcessTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions; public PurchaseProcessTests()
        {
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"SecondChance_Process_{Guid.NewGuid()}")
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        [Fact]
        public async Task ConfirmPaymentAsync_ShouldUpdateOrderStatusToPaid_AndSetProductToSoldOut()
        {
            var productId = Guid.NewGuid();
            var buyerId = "buyer-123";

            using (var context = new ApplicationDbContext(_dbOptions))
            {
                var testProduct = new Product
                {
                    Id = productId,
                    Title = "Test Laptop",
                    Price = 500m,
                    Stock = 1,
                    IsAvailable = true,
                    Status = ProductStatus.Available,
                    SellerId = "seller-999",
                    CategoryId = Guid.NewGuid()
                };
                context.Products.Add(testProduct);
                await context.SaveChangesAsync();

                var productRepo = new ProductRepository(context);
                var purchaseService = new PurchaseService(productRepo, context);
                await purchaseService.ReserveProductAsync(productId, 1, buyerId);
            } 

            Guid orderId;
            using (var context = new ApplicationDbContext(_dbOptions))
            {
                var order = await context.Orders.FirstAsync();
                orderId = order.Id;

                var productRepo = new ProductRepository(context);
                var purchaseService = new PurchaseService(productRepo, context);

                var result = await purchaseService.ConfirmPaymentAsync(orderId, buyerId);
                Assert.True(result); 
            }

            using (var verifyContext = new ApplicationDbContext(_dbOptions))
            {
                var orderInDb = await verifyContext.Orders.FindAsync(orderId);
                Assert.NotNull(orderInDb);
                Assert.Equal(OrderStatus.Paid, orderInDb.Status);

                var productInDb = await verifyContext.Products.FindAsync(productId);
                Assert.NotNull(productInDb);
                Assert.Equal(ProductStatus.Unavailable, productInDb.Status);
                Assert.False(productInDb.IsAvailable);
            }
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldUpdateOrderStatusToCancelled_AndRollbackStockAndStatus()
        {
            var productId = Guid.NewGuid();
            var buyerId = "buyer-123";

            using (var context = new ApplicationDbContext(_dbOptions))
            {
                var testProduct = new Product
                {
                    Id = productId,
                    Title = "Limited Action Figure",
                    Price = 150m,
                    Stock = 1,
                    IsAvailable = true,
                    Status = ProductStatus.Available,
                    SellerId = "seller-999",
                    CategoryId = Guid.NewGuid()
                };
                context.Products.Add(testProduct);
                await context.SaveChangesAsync();

                var productRepo = new ProductRepository(context);
                var purchaseService = new PurchaseService(productRepo, context);
                await purchaseService.ReserveProductAsync(productId, 1, buyerId);
            } 

            Guid orderId;
            using (var context = new ApplicationDbContext(_dbOptions))
            {
                var order = await context.Orders.FirstAsync();
                orderId = order.Id;

                var productRepo = new ProductRepository(context);
                var purchaseService = new PurchaseService(productRepo, context);

                var result = await purchaseService.CancelOrderAsync(orderId, buyerId);
                Assert.True(result); 
            }

            using (var verifyContext = new ApplicationDbContext(_dbOptions))
            {
                var orderInDb = await verifyContext.Orders.FindAsync(orderId);
                Assert.NotNull(orderInDb);
                Assert.Equal(OrderStatus.Cancelled, orderInDb.Status);

                var productInDb = await verifyContext.Products.FindAsync(productId);
                Assert.NotNull(productInDb);
                Assert.Equal(1, productInDb.Stock);
                Assert.Equal(ProductStatus.Available, productInDb.Status);
            }
        }
    }
}
