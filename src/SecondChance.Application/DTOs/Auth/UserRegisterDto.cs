using System.ComponentModel.DataAnnotations;

namespace SecondChance.Application.DTOs.Auth;

public sealed class UserRegisterDto
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 12)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Url, StringLength(2_000)]
    public string? AvatarUrl { get; set; }
}
