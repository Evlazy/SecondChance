using SecondChance.Domain.Entities;
using SecondChance.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductBysellerIdAsync(string sellerId);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(ProductQueryDto query);
        Task<Product?> GetByIdWithImagesAsync(Guid id);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<bool> DecreaseStockAsync(Guid productId, int quantity);
        void Update(Product product);
    }
}
