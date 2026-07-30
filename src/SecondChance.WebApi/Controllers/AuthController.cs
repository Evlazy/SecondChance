using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SecondChance.Application.DTOs.Auth;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;

namespace SecondChance.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IAuthService authService, ITokenService tokenService,
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
    {
        await _authService.RegisterAsync(request);
        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "Invalid email or password." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password." });

        var userRoles = await _userManager.GetRolesAsync(user);
        return Ok(new
        {
            token = _tokenService.CreateToken(user, userRoles),
            email = user.Email,
            fullName = $"{user.FirstName} {user.LastName}"
        });
    }
}
