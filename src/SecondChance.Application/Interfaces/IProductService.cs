using SecondChance.Application.DTOs.Category;
using SecondChance.Application.DTOs.Common;
using SecondChance.Application.DTOs.Product;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<Guid>> CreateProductAsync(CreateProductDto dto, string sellerId);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(ProductQueryDto query);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Product> UpdateProductAsync(Guid id, UpdateProductDto dto, string currentUserId);
        Task<bool> DeleteProductAsync(Guid id, string currentUserId);
        Task<ProductImage> UploadProductImageAsync(Guid productId, UploadProductImageDto dto, string currentUserId);
        Task DeleteProductImageAsync(Guid productId, Guid imageId, string currentUserId);
        Task<IEnumerable<ProductImageResponseDto>> GetProductImagesAsync(Guid productId);
        Task<bool> ToggleProductFavoriteAsync(Guid productId, string currentUserId);
        Task<IEnumerable<ProductCardDto>> GetUserFavoritesAsync(string userId);
        Task<IEnumerable<CategoryDto>> GetAllCategories();
    }
}
