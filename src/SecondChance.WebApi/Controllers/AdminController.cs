using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;

namespace SecondChance.WebApi.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminProductService _adminService;

        public AdminController(UserManager<ApplicationUser> userManager, IAdminProductService adminService)
        {
            _userManager = userManager;
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("promote/{userId}")]
        public async Task<IActionResult> PromoteToAdmin([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "User did not found" });

            if(!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            return Ok(new { message = $"Congrautes, user {user.UserName} become to admin" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard-stats")]
        public IActionResult GetDashboardStats()
        {
            return Ok(new
            {
                message = "Welcome to admin dashboard",
                serverTime = DateTime.UtcNow
            });
        }

        [Authorize(Roles ="Admin")]
        [HttpPut("products/{productId}/freeze")]
        public async Task<IActionResult> FreezeProduct([FromRoute] Guid productId, [FromQuery] string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                return BadRequest(new { message = "Must input the reason for this product" });
            }

            var success = await _adminService.FreezeProductAsync(productId, reason);
            if (!success) return NotFound(new { message = "Product did not found" });

            return Ok(new { message = $"Product deleted/invisiable success, reason: {reason}" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("products/{productId}/unfreeze")]
        public async Task<IActionResult> UnfreezeProduct([FromRoute] Guid productId)
        {
            var success = await _adminService.UnFreezeProductAsync(productId);
            if (!success) return NotFound(new { message = "Product did not found" });

            return Ok(new { message = "Product is avaliable now" });
        }
    }
}
