using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new[] { "Admin", "User" };
            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin accounts must be provisioned through a protected operational process,
            // never from a predictable username and password committed to the repository.
        }
    }
}
