using Microsoft.AspNetCore.Mvc;
using SecondChance.Application.DTOs.Auth;
using SecondChance.Application.Interfaces;

namespace SecondChance.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<SecondChance.Domain.Entities.ApplicationUser> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<SecondChance.Domain.Entities.ApplicationUser> _userManager;

        public AuthController(
        IAuthService authService,
        ITokenService tokenService,
        Microsoft.AspNetCore.Identity.SignInManager<SecondChance.Domain.Entities.ApplicationUser> signInManager,
        Microsoft.AspNetCore.Identity.UserManager<SecondChance.Domain.Entities.ApplicationUser> userManager)
        {
            _authService = authService;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            await _authService.RegisterAsync(request);
            return Ok(new { message = "Registration Success" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null)
            {
                throw new UnauthorizedAccessException("Wrong Email or Password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Wrong Email or Password");

            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.CreateToken(user, userRoles);

            return Ok(new
            {
                token = token,
                email = user.Email,
                fullName = $"{user.FirstName} {user.LastName}"
            });
        }
    }
}
