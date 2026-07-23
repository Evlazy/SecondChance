using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.Interfaces;
using SecondChance.Infrastructure.Data;
using System.Security.Claims;

namespace SecondChance.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [Authorize]
        [HttpPost("products/{productId}/reserve")]
        public async Task<IActionResult> ReserveProductAsync([FromRoute] Guid productId, [FromQuery] int quantity = 1)
        {
            if(quantity <= 0)
            {
                return BadRequest(new { message = "Purchase number must bigger than 0" });
            }

            var buyerIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(buyerIdStr)) return Unauthorized();

            var success = await _purchaseService.ReserveProductAsync(productId, quantity, buyerIdStr);

            if (!success)
            {
                return Conflict(new { message = "Product don't have enough stock or the product is currently unavliable" });
            }

            return Ok(new { message = "Congrates" });
        }

        [HttpGet("my-purchases")]
        public async Task<IActionResult> GetMyPurchases()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var purchases = await _purchaseService.GetMyPurchasesAsync(currentUserId);
            return Ok(purchases);
        }

        [HttpGet("my-sales")]
        public async Task<IActionResult> GetMySales()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var sales = await _purchaseService.GetMySalesAsync(currentUserId);
            return Ok(sales);
        }

        [HttpPost("orders/{orderId}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(Guid orderId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var success = await _purchaseService.ConfirmPaymentAsync(orderId, currentUserId);
            if (!success)
            {
                return BadRequest(new { message = "Cannot confirm payment. Either order doesn't exist, already processed, or access denied." });
            }

            return Ok(new { message = "Payment confirmed successfully." });
        }

        [HttpPost("orders/{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var success = await _purchaseService.CancelOrderAsync(orderId, currentUserId);
            if (!success)
            {
                return BadRequest(new { message = "Cannot cancel order. It may have already been paid/cancelled or access is denied." });
            }

            return Ok(new { message = "Order cancelled and stock returned successfully." });
        }
    }
}
