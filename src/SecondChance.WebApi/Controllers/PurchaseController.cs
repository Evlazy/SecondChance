using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.Interfaces;
using System.Security.Claims;

namespace SecondChance.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService) => _purchaseService = purchaseService;

    [HttpPost("products/{productId}/reserve")]
    public async Task<IActionResult> ReserveProductAsync(Guid productId, [FromQuery] int quantity = 1)
    {
        if (quantity is < 1 or > 100) return BadRequest(new { message = "Quantity must be between 1 and 100." });
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        return await _purchaseService.ReserveProductAsync(productId, quantity, userId)
            ? Ok(new { message = "Product reserved. Complete payment through the payment provider." })
            : Conflict(new { message = "The product is unavailable, stock is insufficient, or you own this product." });
    }

    [HttpGet("my-purchases")]
    public async Task<IActionResult> GetMyPurchases() => Ok(await _purchaseService.GetMyPurchasesAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

    [HttpGet("my-sales")]
    public async Task<IActionResult> GetMySales() => Ok(await _purchaseService.GetMySalesAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!));

    [HttpPost("orders/{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        var success = await _purchaseService.CancelOrderAsync(orderId, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return success ? Ok(new { message = "Order cancelled and stock restored." })
            : BadRequest(new { message = "This order cannot be cancelled." });
    }
}
