using Microsoft.AspNetCore.Identity;
using SecondChance.Application.DTOs.Auth;
using SecondChance.Application.Exceptions;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task RegisterAsync(UserRegisterDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if(userExists != null)
            {
                throw new BadRequestException("Email Registered.");
            }

            var newUser = ApplicationUser.Create(
                request.Email,
                request.FirstName,
                request.LastName,
                request.AvatarUrl
                );

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Register Failed: {errors}");
            }
        }
    }
}
