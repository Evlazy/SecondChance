using SecondChance.Application.DTOs.Category;
using SecondChance.Application.DTOs.Common;
using SecondChance.Application.DTOs.Product;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Enums;
using SecondChance.Domain.Interfaces;
using SecondChance.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFavoriteRepository _favoriteRepository;

        public ProductService
            (IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IFavoriteRepository favoriteRepository)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _favoriteRepository = favoriteRepository;

        }



        public async Task<ApiResponse<Guid>> CreateProductAsync(CreateProductDto dto, string sellerId)
        {
            var categoryExisits = await _categoryRepo.ExistsAsync(dto.CategoryId);
            if (!categoryExisits)
            {
                throw new KeyNotFoundException("Category do not exist, please select again.");
            }

            var newProduct = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Condition = dto.Condition,
                CategoryId = dto.CategoryId,
                SellerId = sellerId,
                CreatedBy = sellerId,
                CreatedAt = DateTime.UtcNow,
                Status = ProductStatus.Available,
                IsAvailable = true
            };

            await _productRepo.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<Guid>.Ok(newProduct.Id, "Product created");
        }

        public async Task<bool> DeleteProductAsync(Guid id, string currentUserId)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return false;

            if(product.SellerId != currentUserId)
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this product");
            }

            _productRepo.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task DeleteProductImageAsync(Guid productId, Guid imageId, string currentUserId)
        {
            var product = await _productRepo.GetByIdWithImagesAsync(productId);
            if(product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            if(product.SellerId != currentUserId)
            {
                throw new UnauthorizedAccessException("No permission to delete this picture");
            }

            var imageToDelete = product.Images.FirstOrDefault(img => img.Id == imageId);
            if(imageToDelete == null)
            {
                throw new KeyNotFoundException("No picture found relate to this product");
            }

            bool deletedImageWasMain = imageToDelete.IsMain;

            product.Images.Remove(imageToDelete);

            if(deletedImageWasMain && product.Images.Any())
            {
                var nextMainImage = product.Images.First();
                nextMainImage.IsMain = true;
            }

            await _unitOfWork.SaveChangesAsync();

            await _fileStorageService.DeleteAsync(imageToDelete.ImageUrl);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllCategories();

            if(categories == null || !categories.Any())
            {
                throw new KeyNotFoundException("No category found");
            }

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                LastModifiedAt = c.LastModifiedAt,
                LastModifiedBy = c.LastModifiedBy
            }).ToList();
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsAsync(ProductQueryDto query)
        {
            return await _productRepo.GetPagedProductsAsync(query);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ProductImageResponseDto>> GetProductImagesAsync(Guid productId)
        {
            var product = await _productRepo.GetByIdWithImagesAsync(productId);
            if(product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            return product.Images.Select(img => new ProductImageResponseDto
            {
                Id = img.Id,
                ImageUrl = img.ImageUrl,
                IsMain = img.IsMain,
                ProductId = img.ProductId
            });
        }

        public async Task<IEnumerable<ProductCardDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId);

            return favorites.Select(f => new ProductCardDto
            {
                Id = f.Product.Id,
                Title = f.Product.Title,
                Price = f.Product.Price,
                ImageUrl = f.Product.Images.FirstOrDefault(img => img.IsMain)?.ImageUrl
                   ?? f.Product.Images.FirstOrDefault()?.ImageUrl
                   ?? "",
                FavoritedAt = f.CreatedAt
            });

        }

        public async Task<bool> ToggleProductFavoriteAsync(Guid productId, string currentUserId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if(product == null)
            {
                throw new KeyNotFoundException("Product do not exist, can not favorite");
            }

            if(product.SellerId == currentUserId)
            {
                throw new InvalidOperationException("You can not favorite the product you posted");
            }

            var existingFavorite = await _favoriteRepository.GetAsync(currentUserId, productId);

            if(existingFavorite != null)
            {
                _favoriteRepository.Remove(existingFavorite);

                await _unitOfWork.SaveChangesAsync();
                return false;
            }
            else
            {
                var newFavorite = new Favorite
                {
                    UserId = currentUserId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow
                };

                await _favoriteRepository.AddAsync(newFavorite);

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Product> UpdateProductAsync(Guid id, UpdateProductDto dto, string currentUserId)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) throw new Exception("Product dones't exsist");

            if(product.SellerId != currentUserId)
            {
                throw new UnauthorizedAccessException("You don't have access to this product.");
            }

            product.Title = dto.Title;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Condition = dto.Condition;
            product.Status = dto.Status;
            product.LastModifiedAt = DateTime.UtcNow;
            product.LastModifiedBy = currentUserId;

             _productRepo.Update(product);

            await _unitOfWork.SaveChangesAsync();

            return product;
        }

        public async Task<ProductImage> UploadProductImageAsync(Guid productId, UploadProductImageDto dto, string currentUserId)
        {
            var product = await _productRepo.GetByIdWithImagesAsync(productId);
            if(product == null)
            {
                throw new KeyNotFoundException("Product did not found, image upload failed");
            }

            if(product.SellerId != currentUserId)
            {
                throw new UnauthorizedAccessException("You don't have permission to upload image for this product");
            }

            var imageUrl = await _fileStorageService.UploadAsync(dto.File, "products");

            bool isMainImage = !product.Images.Any();

            var productImage = new ProductImage
            {
                Id = Guid.NewGuid(),
                ImageUrl = imageUrl,
                IsMain = isMainImage,
                ProductId = productId
            };

            product.Images.Add(productImage);
            _unitOfWork.ForceEntryAdded(productImage);
            await _unitOfWork.SaveChangesAsync();

            return productImage;
        }
    }
}
