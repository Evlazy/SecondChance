using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.DTOs.Category;
using SecondChance.Application.DTOs.Common;
using SecondChance.Application.DTOs.Product;
using SecondChance.Application.Interfaces;
using SecondChance.Application.Mapping;
using SecondChance.Domain.Queries;
using System.Linq.Expressions;
using System.Security.Claims;

namespace SecondChance.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all-categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var result =  await _productService.GetAllCategories();
            return Ok(result);
        }

        [HttpPost("create-product")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateProudct([FromBody] CreateProductDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<Guid>.Fail("Please login again."));
            }

            var result = await _productService.CreateProductAsync(dto, currentUserId);

            return StatusCode(201, result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryDto query)
        {
            var (items, totalCount) = await _productService.GetPagedProductsAsync(query);

            var productDtos = items.Select(p => p.ToDto()).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Data = productDtos
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { Message = "Product not found." });

            return Ok(product.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("uid")?.Value;

                if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

                var updatedProduct = await _productService.UpdateProductAsync(id, dto, currentUserId);
                return Ok(updatedProduct.ToDto());
            }
            catch(UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("uid")?.Value;

                if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();
                var result = await _productService.DeleteProductAsync(id, currentUserId);
                if (!result) return NotFound(new { Message = "Product do not exisit or already deleted" });

                return Ok(new { Message = "Product deleted" });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{id}/images")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid id, [FromForm] UploadProductImageDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Login not valid, please login again");
            }

            try
            {
                var productImages = await _productService.UploadProductImageAsync(id, dto, currentUserId);

                var response = new ProductImageResponseDto
                {
                    Id = productImages.Id,
                    ImageUrl = productImages.ImageUrl,
                    IsMain = productImages.IsMain,
                    ProductId = productImages.ProductId
                };
                return Ok(response);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}/images/{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] Guid id, [FromRoute] Guid imageId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Login expired, please login again");
            }

            try
            {
                await _productService.DeleteProductImageAsync(id, imageId, currentUserId);

                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{id}/images")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages([FromRoute] Guid id)
        {
            try
            {
                var images = await _productService.GetProductImagesAsync(id);
                return Ok(images);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> ToggleFavorite([FromRoute] Guid id)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You have to login first to favorite a product");
            }

            try
            {
                bool isFavorited = await _productService.ToggleProductFavoriteAsync(id, currentUserId);

                if (isFavorited)
                {
                    return Ok(new { message = "Favorite added", isFavorited = true });
                }
                else
                {
                    return Ok(new { message = "Removed favorite", isFavorited = false });
                }
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("favorites/me")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("You must login to see your favorites");
            }

            var myFavorites = await _productService.GetUserFavoritesAsync(currentUserId);
            return Ok(myFavorites);
        }

    }
}
