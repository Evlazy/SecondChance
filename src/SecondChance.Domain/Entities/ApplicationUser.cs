using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ApplicationUser() { }

        public static ApplicationUser Create(
            string email,
            string firstName,
            string lastName,
            string? avatarUrl)
        {
            return new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                AvatarUrl = avatarUrl
            };
        }
    }
}
