using Microsoft.EntityFrameworkCore;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Domain.Interfaces;
using SecondChance.Domain.Queries;
using SecondChance.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        //public async Task<bool> DecreaseStockAsync(Guid productId, int quantity)
        //{
        //    var affectedRows = await _context.Database.ExecuteSqlRawAsync(
        //            @"UPDATE Products
        //                SET Stock = Stock - {0}
        //                WHERE Id = {1}
        //                    AND Status = {2} 
        //                    AND Stock >= {0}",
        //            quantity,
        //            productId,
        //            (int)ProductStatus.Available
        //    );

        //    return affectedRows > 0;
        //}

        private static readonly object _dbLock = new object();

        public async Task<bool> DecreaseStockAsync(Guid productId, int quantity)
        {
            if (_context.Database.IsRelational())
            {
                var affectedRows = await _context.Database.ExecuteSqlRawAsync(
                    @"UPDATE Products
                      SET Stock = Stock - {0},
                          Status = CASE WHEN Stock - {0} = 0 THEN {3} ELSE Status END,
                          IsAvailable = CASE WHEN Stock - {0} = 0 THEN 0 ELSE IsAvailable END
                      WHERE Id = {1}
                        AND Status = {2}
                        AND Stock >= {0}",
                    quantity,                            // {0}
                    productId,                           // {1}
                    (int)ProductStatus.Available,        // {2}
                    (int)ProductStatus.Unavailable       // {3}
                );

                return affectedRows > 0;
            }

            lock (_dbLock)
            {
                var product = _context.Products.Find(productId);
                if (product == null || !product.IsAvailable || product.Status != ProductStatus.Available || product.Stock < quantity)
                {
                    return false;
                }

                product.Stock -= quantity;

                if (product.Stock == 0)
                {
                    product.IsAvailable = false;
                    product.Status = ProductStatus.Unavailable;
                }

                _context.SaveChanges();
                return true;
            }
        }

        public async Task<Product?> GetByIdWithImagesAsync(Guid id)
        {
            return await _context.Products
               .Include(p => p.Images)
               .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(ProductQueryDto query)
        {
            var queryable = _dbSet.AsQueryable();

            queryable = queryable.Where(p => p.IsAvailable == true && p.Status == ProductStatus.Available);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                queryable = queryable.Where(p => p.Title.Contains(query.SearchTerm) || p.Description.Contains(query.SearchTerm));
            }

            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            if (query.MinPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);
            }

            int totalCount = await queryable.CountAsync();

            queryable = query.SortBy?.ToLower() switch
            {
                "price_asc" => queryable.OrderBy(p => p.Price),
                "price_desc" => queryable.OrderByDescending(p => p.Price),
                _ => queryable.OrderByDescending(p => p.Id)
            };

            var items = await queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetProductBysellerIdAsync(string sellerId)
        {
            return await _dbSet
                .Where(p => p.SellerId == sellerId && p.Status == ProductStatus.Available)
                .ToListAsync();
        }

    }
}
