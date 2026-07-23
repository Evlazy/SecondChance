using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SecondChance.Application.DTOs.Auth
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password required")]
        public string Password { get; set; } = string.Empty;
    }
}
